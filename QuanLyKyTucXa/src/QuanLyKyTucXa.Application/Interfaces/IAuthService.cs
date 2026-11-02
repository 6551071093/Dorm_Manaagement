using QuanLyKyTucXa.Infrastructure.Security;

namespace QuanLyKyTucXa.Application.Interfaces;

public interface IAuthService
{
    Task<AuthSession> LoginAsync(string username, string password);
    Task ChangePasswordAsync(AuthSession session, string currentPassword, string newPassword);
    Task RequestPasswordOtpAsync(string email);
    Task<string> VerifyPasswordOtpAsync(string email, string code);
    Task ResetPasswordWithGrantAsync(string username, string email, string grant, string newPassword);

    // Phương thức nghiệp vụ giao diện
    Task SendResetOtpAsync(string email);
    Task ResetPasswordWithOtpAsync(string username, string email, string grant, string newPassword);
}
