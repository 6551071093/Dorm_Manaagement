using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;

namespace QuanLyKyTucXa.Infrastructure.Services;

public sealed record UserAccountRow(int Id, string Username, string FullName, string Role, bool Enabled, DateTime? LastLogin)
{
    public int FailedAttempts { get; init; }
    public bool LoginLocked { get; init; }
}
public sealed record StudentAccountChoice(string Id, string Name);
public sealed class UserAccountEdit
{
    public int? Id { get; set; }
    public string Username { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Role { get; set; } = "NhanVien";
    public bool Enabled { get; set; } = true;
    public string? StudentId { get; set; }
    public string? NewPassword { get; set; }
}

public sealed class NguoiDungService(Func<AppDbContext> createDb, AuthSession session)
{
    public async Task<List<UserAccountRow>> ListAsync()
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageUsers);
        var rows = await db.NguoiDungs.AsNoTracking().OrderBy(x => x.TenDangNhap)
            .Select(x => new UserAccountRow(x.MaNguoiDung, x.TenDangNhap, x.HoTen, x.VaiTro, x.TrangThai, x.LanDangNhapCuoi)).ToListAsync();
        await db.Database.OpenConnectionAsync();
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT MaNguoiDung, SoLanSai, BiKhoa FROM dbo.TaiKhoanDangNhap";
        var states = new Dictionary<int, (int Count, bool Locked)>();
        await using (var reader = await command.ExecuteReaderAsync())
            while (await reader.ReadAsync()) states[reader.GetInt32(0)] = (reader.GetInt32(1), reader.GetBoolean(2));
        return rows.Select(row => states.TryGetValue(row.Id, out var state)
            ? row with { FailedAttempts = state.Count, LoginLocked = state.Locked } : row).ToList();
    }
    public async Task<UserAccountEdit> GetAsync(int id)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageUsers);
        var user = await db.NguoiDungs.AsNoTracking().SingleOrDefaultAsync(x => x.MaNguoiDung == id)
            ?? throw new InvalidOperationException("Tài khoản không còn tồn tại.");
        var studentId = await db.SinhViens.Where(x => x.MaNguoiDung == id).OrderBy(x => x.MaSv).Select(x => x.MaSv).FirstOrDefaultAsync();
        return new UserAccountEdit { Id = id, Username = user.TenDangNhap, FullName = user.HoTen, Role = user.VaiTro, Enabled = user.TrangThai, StudentId = studentId };
    }
    public async Task<List<StudentAccountChoice>> StudentChoicesAsync(int? userId)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageUsers);
        return await db.SinhViens.AsNoTracking().Where(x => x.MaNguoiDung == null || (userId != null && x.MaNguoiDung == userId))
            .OrderBy(x => x.MaSv).Select(x => new StudentAccountChoice(x.MaSv, x.HoTen)).ToListAsync();
    }

    public async Task SaveAsync(UserAccountEdit value)
    {
        value.Username = (value.Username ?? "").Trim();
        value.FullName = (value.FullName ?? "").Trim();
        if (!Regex.IsMatch(value.Username, @"^[A-Za-z0-9_.@-]{3,50}$"))
            throw new InvalidOperationException("Tên đăng nhập gồm 3–50 chữ cái không dấu, số hoặc ký tự . _ @ -.");
        if (value.FullName.Length is < 1 or > 100) throw new InvalidOperationException("Họ tên phải có từ 1 đến 100 ký tự.");
        if (value.Role is not ("Admin" or "NhanVien" or "SinhVien")) throw new InvalidOperationException("Vai trò không hợp lệ.");
        if (value.Id is null || !string.IsNullOrEmpty(value.NewPassword)) PasswordHash.ValidateNew(value.NewPassword ?? "");
        var passwordHash = string.IsNullOrEmpty(value.NewPassword) ? null : await Task.Run(() => PasswordHash.Create(value.NewPassword));
        await using var db = createDb();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageUsers);
        var user = value.Id is null ? null : await db.NguoiDungs.SingleOrDefaultAsync(x => x.MaNguoiDung == value.Id);
        if (value.Id is not null && user is null) throw new InvalidOperationException("Tài khoản không còn tồn tại.");
        if (user is not null && user.TenDangNhap != value.Username) throw new InvalidOperationException("Không đổi tên đăng nhập của tài khoản đã tạo.");
        if (user?.MaNguoiDung == session.UserId && (!value.Enabled || value.Role != session.Role))
            throw new InvalidOperationException("Không thể khóa hoặc đổi vai trò của chính tài khoản đang đăng nhập.");
        if (user?.VaiTro == "Admin" && user.TrangThai && (!value.Enabled || value.Role != "Admin")
            && await db.NguoiDungs.CountAsync(x => x.VaiTro == "Admin" && x.TrangThai) <= 1)
            throw new InvalidOperationException("Phải giữ lại ít nhất một tài khoản Admin đang hoạt động.");
        if (await db.NguoiDungs.AnyAsync(x => x.TenDangNhap == value.Username && (value.Id == null || x.MaNguoiDung != value.Id)))
            throw new InvalidOperationException("Tên đăng nhập đã tồn tại.");
        SinhVien? student = null;
        if (value.Role == "SinhVien")
        {
            student = await db.SinhViens.SingleOrDefaultAsync(x => x.MaSv == value.StudentId)
                ?? throw new InvalidOperationException("Tài khoản Sinh viên phải chọn một hồ sơ sinh viên.");
            if (student.MaNguoiDung is not null && student.MaNguoiDung != value.Id)
                throw new InvalidOperationException("Hồ sơ này đã liên kết với tài khoản khác.");
        }
        if (user is null)
        {
            user = new NguoiDung { TenDangNhap = value.Username, NgayTao = DateTime.Now, MatKhauHash = passwordHash! };
            db.NguoiDungs.Add(user);
        }
        user.HoTen = value.FullName; user.VaiTro = value.Role; user.TrangThai = value.Enabled;
        if (passwordHash is not null) user.MatKhauHash = passwordHash;
        if (value.Id is not null)
        {
            var oldLinks = await db.SinhViens.Where(x => x.MaNguoiDung == value.Id).ToListAsync();
            foreach (var old in oldLinks) old.MaNguoiDung = null;
        }
        await db.SaveChangesAsync();
        if (student is not null) { student.MaNguoiDung = user.MaNguoiDung; await db.SaveChangesAsync(); }
        // Admin đặt lại mật khẩu sẽ thu hồi mã khôi phục cũ của tài khoản đó.
        if (value.Id is not null && passwordHash is not null)
            await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM dbo.TaiKhoanKhoiPhuc WHERE MaNguoiDung = {user.MaNguoiDung}");
        // Admin mở khóa/kích hoạt sẽ xóa bộ đếm sai; lần đăng nhập tiếp theo bắt đầu từ 0.
        if (value.Enabled)
            await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM dbo.TaiKhoanDangNhap WHERE MaNguoiDung = {user.MaNguoiDung}");
        await tx.CommitAsync();
    }
}