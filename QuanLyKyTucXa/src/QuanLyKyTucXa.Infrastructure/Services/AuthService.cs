using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Repositories;
using QuanLyKyTucXa.Infrastructure.Security;

namespace QuanLyKyTucXa.Infrastructure.Services;

public sealed partial class AuthService(Func<AppDbContext> createDb)
{
    private readonly NguoiDungRepository repository = new(createDb);
    private static readonly ConcurrentDictionary<string, (int Count, DateTimeOffset Until)> Attempts = new(StringComparer.OrdinalIgnoreCase);
    private static readonly string DummyHash = PasswordHash.Create("dummy-" + Guid.NewGuid().ToString("N"));

    public async Task<AuthSession> LoginAsync(string username, string password)
    {
        username = (username ?? "").Trim();
        if (username.Length is < 1 or > 50 || string.IsNullOrEmpty(password) || password.Length > 128)
            throw new InvalidOperationException("Vui lòng nhập tên đăng nhập và mật khẩu hợp lệ.");
        var now = DateTimeOffset.UtcNow;
        if (Attempts.TryGetValue(username, out var attempt) && attempt.Count >= 5 && attempt.Until > now)
            throw new InvalidOperationException("Đăng nhập sai nhiều lần. Vui lòng đợi 30 giây rồi thử lại.");
        var account = await repository.FindByUsernameAsync(username);
        var result = await Task.Run(() => {
            var valid = PasswordHash.Verify(password, account?.MatKhauHash ?? DummyHash, out var upgrade);
            return (Valid: valid, Upgrade: upgrade);
        });
        if (account is not null && !account.TrangThai)
            throw new InvalidOperationException("Tài khoản đang bị khóa hoặc chưa được kích hoạt. Liên hệ Admin để mở khóa.");
        if (account is null || !result.Valid)
        {
            Attempts.AddOrUpdate(username, (1, now.AddSeconds(30)), (_, old) => old.Until <= now ? (1, now.AddSeconds(30)) : (old.Count + 1, now.AddSeconds(30)));
            if (account is not null)
            {
                var state = await repository.RecordFailureAsync(account);
                if (state.Locked) throw new InvalidOperationException("Tài khoản đã bị khóa do sai mật khẩu 5 lần. Liên hệ Admin mở khóa.");
                throw new InvalidOperationException($"Tên đăng nhập hoặc mật khẩu không đúng. Còn {5 - state.FailedAttempts} lần thử trước khi khóa tài khoản.");
            }
            throw new InvalidOperationException("Tên đăng nhập hoặc mật khẩu không đúng.");
        }
        if (account.VaiTro is not ("Admin" or "NhanVien" or "SinhVien"))
            throw new InvalidOperationException("Tài khoản chưa được cấp vai trò hợp lệ. Liên hệ Admin.");
        string? studentId = null;
        if (account.VaiTro == "SinhVien")
        {
            var links = await repository.GetStudentLinksAsync(account.MaNguoiDung);
            if (links.Count != 1) throw new InvalidOperationException("Tài khoản chưa được liên kết với một hồ sơ sinh viên. Liên hệ quản trị viên.");
            studentId = links[0];
        }
        var upgraded = result.Upgrade ? await Task.Run(() => PasswordHash.Create(password)) : null;
        if (!await repository.CompleteLoginAsync(account, upgraded))
            throw new InvalidOperationException("Tài khoản đã bị khóa hoặc vừa thay đổi. Vui lòng đăng nhập lại hoặc liên hệ Admin.");
        Attempts.TryRemove(username, out _);
        return new AuthSession(account.MaNguoiDung, account.TenDangNhap, account.HoTen, account.VaiTro, studentId, upgraded ?? account.MatKhauHash);
    }

    public async Task ChangePasswordAsync(AuthSession session, string currentPassword, string newPassword)
    {
        PasswordHash.ValidateNew(newPassword);
        if (currentPassword == newPassword) throw new InvalidOperationException("Mật khẩu mới phải khác mật khẩu hiện tại.");
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, session.IsStudent ? AppPermission.Personal : AppPermission.Staff);
        var user = await db.NguoiDungs.AsNoTracking().SingleAsync(x => x.MaNguoiDung == session.UserId);
        if (!await Task.Run(() => PasswordHash.Verify(currentPassword, user.MatKhauHash, out _)))
            throw new InvalidOperationException("Mật khẩu hiện tại không đúng.");
        var newHash = await Task.Run(() => PasswordHash.Create(newPassword));
        var affected = await db.NguoiDungs.Where(x => x.MaNguoiDung == session.UserId && x.TrangThai && x.MatKhauHash == user.MatKhauHash)
            .ExecuteUpdateAsync(set => set.SetProperty(x => x.MatKhauHash, newHash));
        if (affected != 1) throw new InvalidOperationException("Tài khoản đã bị khóa hoặc vừa thay đổi. Vui lòng đăng nhập lại hoặc liên hệ Admin.");
        session.CredentialHash = newHash;
    }
}