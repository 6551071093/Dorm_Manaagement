using QuanLyKyTucXa.Infrastructure.Security;
using System.Data;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;

namespace QuanLyKyTucXa.Infrastructure.Services;

public sealed class SinhVienService(Func<AppDbContext> createDb, AuthSession? session = null)
{
    public async Task<SinhVien> GetAsync(string id)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageResidents);
        return await db.SinhViens.AsNoTracking().SingleOrDefaultAsync(x => x.MaSv == id)
            ?? throw new InvalidOperationException("Sinh viên không còn tồn tại. Hãy làm mới danh sách.");
    }

    public async Task SaveAsync(SinhVien value, bool isNew)
    {
        ValidateAndNormalize(value);
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageResidents);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var entity = await db.SinhViens.SingleOrDefaultAsync(x => x.MaSv == value.MaSv);
        if (isNew && entity is not null) throw new InvalidOperationException("Mã sinh viên đã tồn tại.");
        if (!isNew && entity is null) throw new InvalidOperationException("Sinh viên không còn tồn tại.");
        if (await db.SinhViens.AnyAsync(x => x.MaSv != value.MaSv && (x.Cccd == value.Cccd || x.SoDienThoai == value.SoDienThoai)))
            throw new InvalidOperationException("CCCD hoặc số điện thoại đã được sử dụng bởi sinh viên khác.");
        if (isNew)
        {
            entity = new SinhVien { MaSv = value.MaSv, TrangThai = "Chờ xếp phòng" };
            db.SinhViens.Add(entity);
        }
        entity!.HoTen = value.HoTen;
        entity.NgaySinh = value.NgaySinh;
        entity.GioiTinh = value.GioiTinh;
        entity.Cccd = value.Cccd;
        entity.SoDienThoai = value.SoDienThoai;
        entity.Email = value.Email;
        entity.Lop = value.Lop;
        entity.Khoa = value.Khoa;
        entity.QueQuan = value.QueQuan?.Trim();
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task DeleteAsync(string id)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageResidents);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var entity = await db.SinhViens.SingleOrDefaultAsync(x => x.MaSv == id)
            ?? throw new InvalidOperationException("Sinh viên không còn tồn tại.");
        if (entity.TrangThai == "Đang ở" || await db.HopDongs.AnyAsync(x => x.MaSv == id)
            || await db.DangKyKtxes.AnyAsync(x => x.MaSv == id) || await db.HoaDons.AnyAsync(x => x.MaSv == id)
            || await db.SuCos.AnyAsync(x => x.MaSv == id))
            throw new InvalidOperationException("Không thể xóa sinh viên đang ở hoặc đã có đăng ký, hợp đồng, hóa đơn, sự cố. Hãy giữ hồ sơ để bảo toàn lịch sử.");
        db.SinhViens.Remove(entity);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public static void ValidateAndNormalize(SinhVien value)
    {
        value.MaSv = Required(value.MaSv, "Mã sinh viên", 20);
        value.HoTen = Required(value.HoTen, "Họ tên", 100);
        value.Lop = Required(value.Lop, "Lớp", 50);
        value.Khoa = Required(value.Khoa, "Khoa", 100);
        value.Cccd = (value.Cccd ?? "").Trim();
        value.SoDienThoai = (value.SoDienThoai ?? "").Trim();
        if (!Regex.IsMatch(value.Cccd, @"^[0-9]{12}$")) throw new InvalidOperationException("CCCD phải có 12 chữ số.");
        if (!Regex.IsMatch(value.SoDienThoai, @"^0[0-9]{9}$")) throw new InvalidOperationException("Số điện thoại phải có 10 chữ số, bắt đầu bằng 0.");
        if (value.NgaySinh < new DateOnly(1900, 1, 1)) throw new InvalidOperationException("Ngày sinh phải từ năm 1900.");
        if (value.NgaySinh >= DateOnly.FromDateTime(DateTime.Today)) throw new InvalidOperationException("Ngày sinh phải trước ngày hiện tại.");
        value.Email = string.IsNullOrWhiteSpace(value.Email) ? null : value.Email.Trim();
        if (value.Email is not null && (value.Email.Length > 100 || !MailAddress.TryCreate(value.Email, out var mail) || mail.Address != value.Email))
            throw new InvalidOperationException("Email không hợp lệ hoặc dài quá 100 ký tự.");
        if (value.GioiTinh is not ("Nam" or "Nữ")) throw new InvalidOperationException("Vui lòng chọn giới tính.");
        if (value.QueQuan?.Length > 200) throw new InvalidOperationException("Quê quán tối đa 200 ký tự.");
    }

    public async Task<List<SinhVien>> GetForExportAsync(IReadOnlyCollection<string> ids)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageResidents);
        return await db.SinhViens.AsNoTracking().Where(x => ids.Contains(x.MaSv)).OrderBy(x => x.MaSv).ToListAsync();
    }

    public async Task<List<string>> GetImportErrorsAsync(IReadOnlyList<SinhVien> students)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageResidents);
        return await GetImportErrorsAsync(db, students);
    }

    private static async Task<List<string>> GetImportErrorsAsync(AppDbContext db, IReadOnlyList<SinhVien> students)
    {
        var errors = new List<string>();
        if (students.Count is < 1 or > 5000) { errors.Add("Mỗi lần nhập phải có từ 1 đến 5000 sinh viên."); return errors; }
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var cards = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var phones = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < students.Count; i++)
        {
            var s = students[i];
            try { ValidateAndNormalize(s); }
            catch (InvalidOperationException ex) { errors.Add($"Bản ghi {i + 1} ({s.MaSv}): {ex.Message}"); continue; }
            if (!ids.Add(s.MaSv)) errors.Add($"Mã SV {s.MaSv} bị lặp trong file.");
            if (!cards.Add(s.Cccd)) errors.Add($"CCCD của {s.MaSv} bị lặp trong file.");
            if (!phones.Add(s.SoDienThoai)) errors.Add($"Số điện thoại của {s.MaSv} bị lặp trong file.");
        }
        var existing = await db.SinhViens.AsNoTracking().Select(x => new { x.MaSv, x.Cccd, x.SoDienThoai }).ToListAsync();
        var existingIds = existing.Select(x => x.MaSv).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingCards = existing.Select(x => x.Cccd).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingPhones = existing.Select(x => x.SoDienThoai).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var s in students)
        {
            if (existingIds.Contains(s.MaSv)) errors.Add($"Mã SV {s.MaSv} đã có trong database.");
            if (existingCards.Contains(s.Cccd)) errors.Add($"CCCD của {s.MaSv} đã có trong database.");
            if (existingPhones.Contains(s.SoDienThoai)) errors.Add($"Số điện thoại của {s.MaSv} đã có trong database.");
        }
        return errors;
    }

    public async Task<int> ImportAsync(IReadOnlyList<SinhVien> students)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageResidents);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var errors = await GetImportErrorsAsync(db, students);
        if (errors.Count > 0)
            throw new InvalidOperationException("Chưa nhập dữ liệu. " + string.Join(Environment.NewLine, errors.Take(15)));
        foreach (var s in students)
        {
            db.SinhViens.Add(new SinhVien {
                MaSv = s.MaSv, HoTen = s.HoTen, NgaySinh = s.NgaySinh, GioiTinh = s.GioiTinh,
                Cccd = s.Cccd, SoDienThoai = s.SoDienThoai, Email = s.Email, Lop = s.Lop,
                Khoa = s.Khoa, QueQuan = s.QueQuan, TrangThai = "Chờ xếp phòng"
            });
        }
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return students.Count;
    }
    internal static string Required(string text, string label, int max)
    {
        text = (text ?? "").Trim();
        if (text.Length == 0 || text.Length > max) throw new InvalidOperationException($"{label} phải có từ 1 đến {max} ký tự.");
        return text;
    }
}

