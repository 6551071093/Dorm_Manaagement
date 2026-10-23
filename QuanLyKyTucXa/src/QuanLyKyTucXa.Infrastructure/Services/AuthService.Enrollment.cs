using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QuanLyKyTucXa.Infrastructure.Security;

namespace QuanLyKyTucXa.Infrastructure.Services;

public sealed partial class AuthService
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, (int Count, DateTimeOffset Until)> RecoveryAttempts = new(StringComparer.OrdinalIgnoreCase);
    public static string NewRecoveryCode() => Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
    private static string RecoveryHash(string code) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code.Trim().ToUpperInvariant())));

    public async Task<string> RegisterAsync(string username, string fullName, string password)
    {
        username = username.Trim(); fullName = fullName.Trim();
        if (!Regex.IsMatch(username, @"^[A-Za-z0-9_.@-]{3,50}$"))
            throw new InvalidOperationException("Tên đăng nhập: 3–50 chữ cái không dấu, số hoặc . _ @ -.");
        if (fullName.Length is < 2 or > 100) throw new InvalidOperationException("Họ tên phải có từ 2 đến 100 ký tự.");
        PasswordHash.ValidateNew(password);
        var hash = await Task.Run(() => PasswordHash.Create(password));
        var recoveryCode = NewRecoveryCode();
        var recoveryHash = RecoveryHash(recoveryCode);
        await using var db = createDb();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        if (await db.NguoiDungs.AnyAsync(x => x.TenDangNhap == username))
            throw new InvalidOperationException("Tên đăng nhập đã được sử dụng. Vui lòng chọn tên khác.");
        // Tự đăng ký không được tự cấp quyền hoặc chiếm liên kết hồ sơ sinh viên.
        var account = new NguoiDung {
            TenDangNhap = username, HoTen = fullName, MatKhauHash = hash,
            VaiTro = "SinhVien", TrangThai = false, NgayTao = DateTime.Now
        };
        db.NguoiDungs.Add(account);
        await db.SaveChangesAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO dbo.TaiKhoanKhoiPhuc (MaNguoiDung, MaKhoiPhucHash) VALUES ({account.MaNguoiDung}, {recoveryHash})");
        await transaction.CommitAsync();
        return recoveryCode;
    }

    public async Task<string> ResetPasswordAsync(string username, string recoveryCode, string newPassword)
    {
        username = username.Trim(); recoveryCode = recoveryCode.Trim();
        if (username.Length is < 3 or > 50) throw new InvalidOperationException("Tên đăng nhập hoặc mã khôi phục không đúng.");
        var now = DateTimeOffset.UtcNow;
        var attempt = RecoveryAttempts.AddOrUpdate(username, (1, now.AddMinutes(15)),
            (_, previous) => previous.Until <= now ? (1, now.AddMinutes(15)) : (previous.Count + 1, previous.Until));
        if (attempt.Count > 5) throw new InvalidOperationException("Đã thử khôi phục nhiều lần. Vui lòng đợi 15 phút hoặc liên hệ Admin.");
        PasswordHash.ValidateNew(newPassword);
        if (username.Length is < 3 or > 50 || !Regex.IsMatch(recoveryCode, @"\A(?:[0-9A-Fa-f]{16}|[0-9A-Fa-f]{48})\z"))
            throw new InvalidOperationException("Tên đăng nhập hoặc mã khôi phục không đúng.");
        var nextCode = NewRecoveryCode();
        var nextRecoveryHash = RecoveryHash(nextCode);
        var nextPasswordHash = await Task.Run(() => PasswordHash.Create(newPassword));
        await using var db = createDb();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var account = await db.NguoiDungs.SingleOrDefaultAsync(x => x.TenDangNhap == username);
        if (account is null) throw new InvalidOperationException("Tên đăng nhập hoặc mã khôi phục không đúng.");
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.Transaction = transaction.GetDbTransaction();
        command.CommandText = "SELECT MaKhoiPhucHash FROM dbo.TaiKhoanKhoiPhuc WITH (UPDLOCK, HOLDLOCK) WHERE MaNguoiDung = @id";
        var parameter = command.CreateParameter(); parameter.ParameterName = "@id"; parameter.Value = account.MaNguoiDung;
        command.Parameters.Add(parameter);
        var stored = await command.ExecuteScalarAsync() as string;
        var supplied = RecoveryHash(recoveryCode);
        if (stored is null || !CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(stored), Encoding.ASCII.GetBytes(supplied)))
            throw new InvalidOperationException("Tên đăng nhập hoặc mã khôi phục không đúng.");
        account.MatKhauHash = nextPasswordHash;
        // Không mở khóa tài khoản hoặc thay đổi vai trò khi khôi phục mật khẩu.
        await db.SaveChangesAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.TaiKhoanKhoiPhuc SET MaKhoiPhucHash = {nextRecoveryHash}, NgayCap = SYSUTCDATETIME() WHERE MaNguoiDung = {account.MaNguoiDung}");
        await transaction.CommitAsync();
        Attempts.TryRemove(username, out _);
        RecoveryAttempts.TryRemove(username, out _);
        return nextCode;
    }
}
