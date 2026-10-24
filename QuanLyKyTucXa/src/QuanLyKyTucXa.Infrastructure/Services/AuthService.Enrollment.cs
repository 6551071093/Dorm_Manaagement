using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Data.SqlClient;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;

namespace QuanLyKyTucXa.Infrastructure.Services;

public sealed partial class AuthService
{
    public async Task RegisterAsync(string username,string fullName,string password,string email)
    {
        username=AccountValidation.Username(username);fullName=AccountValidation.FullName(fullName);email=AccountValidation.Email(email);
        PasswordHash.ValidateNew(password);
        var hash=await Task.Run(()=>PasswordHash.Create(password));
        await using var db=createDb();
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        if(await db.NguoiDungs.AnyAsync(x=>x.TenDangNhap==username))throw new InvalidOperationException("Tên đăng nhập đã được sử dụng.");
        if(await db.NguoiDungs.AnyAsync(x=>x.Email==email))throw new InvalidOperationException("Email đã được dùng cho tài khoản khác.");
        db.NguoiDungs.Add(new NguoiDung {TenDangNhap=username,HoTen=fullName,Email=email,MatKhauHash=hash,VaiTro="SinhVien",TrangThai=false,NgayTao=DateTime.Now});
        await db.SaveChangesAsync();await tx.CommitAsync();
    }

    private sealed record OtpRow(byte[] Hash,byte[] Salt,DateTime Expires,int Attempts,DateTime Sent,DateTime Window,int Sends,bool Ready,string Email,byte[] Credential,int Stage);
    private static byte[] CodeHash(string code,byte[] salt) => SHA256.HashData(Encoding.UTF8.GetBytes(Convert.ToHexString(salt)+":"+code));
    private static byte[] CredentialFingerprint(string hash)=>SHA256.HashData(Encoding.UTF8.GetBytes(hash));
    private static async Task<OtpRow?> ReadOtpAsync(AppDbContext db,int id)
    {
        await using var command=db.Database.GetDbConnection().CreateCommand();
        command.Transaction=db.Database.CurrentTransaction!.GetDbTransaction();
        command.CommandText="SELECT CodeHash,Salt,ExpiresAt,Attempts,SentAt,WindowStartedAt,SendCount,Ready,Email,Credential,Stage FROM dbo.TaiKhoanOtp WITH(UPDLOCK,HOLDLOCK) WHERE MaNguoiDung=@id";
        command.Parameters.Add(new SqlParameter("@id",id));
        await using var reader=await command.ExecuteReaderAsync();
        if(!await reader.ReadAsync())return null;
        return new((byte[])reader[0],(byte[])reader[1],reader.GetDateTime(2),reader.GetInt32(3),reader.GetDateTime(4),reader.GetDateTime(5),reader.GetInt32(6),reader.GetBoolean(7),reader.GetString(8),(byte[])reader[9],reader.GetInt32(10));
    }
    public async Task RequestPasswordOtpAsync(string email)
    {
        email=AccountValidation.Email(email);
        mailer.EnsureConfigured();
        var code=RandomNumberGenerator.GetInt32(0,1_000_000).ToString("D6");
        var salt=RandomNumberGenerator.GetBytes(16);var hash=CodeHash(code,salt);
        int id;
        await using(var db=createDb())
        {
            await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var users=await db.NguoiDungs.Where(x=>x.Email==email).Take(2).ToListAsync();
            // Không tiết lộ tài khoản/email có tồn tại qua kết quả gửi.
            if(users.Count!=1)return;
            var user=users[0];
            id=user.MaNguoiDung;
            var now=await db.Database.SqlQueryRaw<DateTime>("SELECT SYSUTCDATETIME() AS [Value]").SingleAsync();
            var previous=await ReadOtpAsync(db,id);
            if(previous is not null && (now<previous.Sent.AddSeconds(60) || (now<previous.Window.AddHours(1) && previous.Sends>=5)))return;
            var window=previous is null || now>=previous.Window.AddHours(1)?now:previous.Window;
            var sends=previous is null || now>=previous.Window.AddHours(1)?1:previous.Sends+1;
            var credential=CredentialFingerprint(user.MatKhauHash);var expires=now.AddMinutes(5);
            if(previous is null)
                await db.Database.ExecuteSqlInterpolatedAsync($"INSERT dbo.TaiKhoanOtp(MaNguoiDung,CodeHash,Salt,ExpiresAt,Attempts,SentAt,WindowStartedAt,SendCount,Ready,Email,Credential,Stage) VALUES({id},{hash},{salt},{expires},0,{now},{window},{sends},0,{email},{credential},0)");
            else
                await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.TaiKhoanOtp SET CodeHash={hash},Salt={salt},ExpiresAt={expires},Attempts=0,SentAt={now},WindowStartedAt={window},SendCount={sends},Ready=0,Stage=0,Email={email},Credential={credential} WHERE MaNguoiDung={id}");
            await tx.CommitAsync();
        }
        await mailer.SendAsync(email,code);
        await using var readyDb=createDb();
        await readyDb.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.TaiKhoanOtp SET Ready=1 WHERE MaNguoiDung={id} AND CodeHash={hash} AND ExpiresAt>SYSUTCDATETIME()");
    }
    public async Task<string> VerifyPasswordOtpAsync(string email,string code)
    {
        email=AccountValidation.Email(email);code=(code??"").Trim();
        const string invalid="OTP không đúng, đã hết hạn hoặc đã dùng. Yêu cầu mã mới nếu cần.";
        if(!Regex.IsMatch(code,@"\A[0-9]{6}\z"))throw new InvalidOperationException("OTP phải có đúng 6 chữ số.");
        await using var db=createDb();
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var users=await db.NguoiDungs.Where(x=>x.Email==email).Take(2).ToListAsync();
        if(users.Count!=1)throw new InvalidOperationException(invalid);
        var user=users[0];
        var otp=await ReadOtpAsync(db,user.MaNguoiDung);
        var now=await db.Database.SqlQueryRaw<DateTime>("SELECT SYSUTCDATETIME() AS [Value]").SingleAsync();
        if(otp is null || otp.Stage!=0 || !otp.Ready || otp.Expires<=now || otp.Attempts>=5
            || !string.Equals(otp.Email,email,StringComparison.OrdinalIgnoreCase)
            || !CryptographicOperations.FixedTimeEquals(otp.Credential,CredentialFingerprint(user.MatKhauHash)))throw new InvalidOperationException(invalid);
        if(!CryptographicOperations.FixedTimeEquals(otp.Hash,CodeHash(code,otp.Salt)))
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.TaiKhoanOtp SET Attempts=Attempts+1,Ready=CASE WHEN Attempts+1>=5 THEN 0 ELSE Ready END WHERE MaNguoiDung={user.MaNguoiDung}");
            await tx.CommitAsync();throw new InvalidOperationException(invalid);
        }
        // OTP chỉ dùng để cấp một phiên đặt lại mật khẩu, không đổi mật khẩu tại bước này.
        var grant=Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var salt=RandomNumberGenerator.GetBytes(16);var hash=CodeHash(grant,salt);var expires=now.AddMinutes(5);
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.TaiKhoanOtp SET CodeHash={hash},Salt={salt},Ready=0,Stage=1,Attempts=0,ExpiresAt={expires} WHERE MaNguoiDung={user.MaNguoiDung}");
        await tx.CommitAsync();return grant;
    }

    public async Task ResetPasswordWithGrantAsync(string username,string email,string grant,string newPassword)
    {
        username=AccountValidation.Username(username);email=AccountValidation.Email(email);
        PasswordHash.ValidateNew(newPassword);
        const string invalid="Phiên xác nhận OTP không hợp lệ hoặc đã hết hạn. Hãy quay lại nhập email để nhận mã mới.";
        if(string.IsNullOrEmpty(grant) || !Regex.IsMatch(grant,@"\A[0-9A-F]{64}\z"))throw new InvalidOperationException(invalid);
        var passwordHash=await Task.Run(()=>PasswordHash.Create(newPassword));
        await using var db=createDb();
        await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var user=await db.NguoiDungs.SingleOrDefaultAsync(x=>x.TenDangNhap==username && x.Email==email);
        if(user is null)throw new InvalidOperationException("Tên tài khoản không khớp email đã xác nhận.");
        var otp=await ReadOtpAsync(db,user.MaNguoiDung);
        var now=await db.Database.SqlQueryRaw<DateTime>("SELECT SYSUTCDATETIME() AS [Value]").SingleAsync();
        if(otp is null || otp.Stage!=1 || otp.Ready || otp.Expires<=now || otp.Attempts>=5
            || !string.Equals(otp.Email,email,StringComparison.OrdinalIgnoreCase)
            || !CryptographicOperations.FixedTimeEquals(otp.Credential,CredentialFingerprint(user.MatKhauHash))
            || !CryptographicOperations.FixedTimeEquals(otp.Hash,CodeHash(grant,otp.Salt)))throw new InvalidOperationException(invalid);
        user.MatKhauHash=passwordHash;
        // Tiêu thụ phiên một lần, vẫn giữ bộ đếm gửi để không vượt giới hạn một giờ.
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.TaiKhoanOtp SET Ready=0,Stage=2,Attempts=5 WHERE MaNguoiDung={user.MaNguoiDung}");
        await db.SaveChangesAsync();await tx.CommitAsync();
        // Không mở khóa tài khoản hoặc thay đổi vai trò.
    }

}
