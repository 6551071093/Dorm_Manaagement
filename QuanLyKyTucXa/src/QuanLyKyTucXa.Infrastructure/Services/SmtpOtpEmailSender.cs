using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;

namespace QuanLyKyTucXa.Infrastructure.Services;

public interface IOtpEmailSender
{
    void EnsureConfigured();
    Task SendAsync(string recipient,string otp);
}

public sealed record SmtpOtpOptions(string Host,int Port,string Username,string Password,string FromAddress,string FromName);

public sealed class SmtpOtpEmailSender(SmtpOtpOptions options) : IOtpEmailSender
{
    private string AppPassword => string.Concat((options.Password??"").Where(c=>!char.IsWhiteSpace(c)));
    private bool IsGmail => string.Equals(options.Host?.Trim(),"smtp.gmail.com",StringComparison.OrdinalIgnoreCase);
    public static SmtpOtpEmailSender FromEnvironment() => new(new(
        Environment.GetEnvironmentVariable("Email__Host")??"smtp.gmail.com",
        int.TryParse(Environment.GetEnvironmentVariable("Email__Port"),out var port)?port:587,
        Environment.GetEnvironmentVariable("Email__Username")??"",Environment.GetEnvironmentVariable("Email__Password")??"",
        Environment.GetEnvironmentVariable("Email__FromAddress")??"",Environment.GetEnvironmentVariable("Email__FromName")??"KTX Home"));
    public void EnsureConfigured()
    {
        if(string.IsNullOrWhiteSpace(options.Host) || options.Port is <1 or >65535 || string.IsNullOrWhiteSpace(options.Username)
            || string.IsNullOrWhiteSpace(AppPassword) || !MailAddress.TryCreate(options.FromAddress,out _))
            throw new InvalidOperationException("Chưa cấu hình gửi OTP. Admin cần cấu hình tài khoản gửi và mật khẩu ứng dụng Gmail trên máy.");
        if(IsGmail && (AppPassword.Length!=16 || !AppPassword.All(char.IsAsciiLetterOrDigit)))
            throw new InvalidOperationException("Gmail cần mật khẩu ứng dụng 16 ký tự. Admin hãy cập nhật Email:Password trong appsettings.local.json; không dùng mật khẩu Gmail thường.");
        if(IsGmail && options.Port!=587)
            throw new InvalidOperationException("Gmail SMTP của ứng dụng cần cổng 587 với TLS. Admin hãy cập nhật Email:Port rồi chạy lại.");
    }
    public async Task SendAsync(string recipient,string otp)
    {
        EnsureConfigured();
        using var message=new MailMessage {From=new MailAddress(options.FromAddress.Trim(),options.FromName,Encoding.UTF8),
            Subject="KTX Home — Mã OTP đặt lại mật khẩu",SubjectEncoding=Encoding.UTF8,BodyEncoding=Encoding.UTF8,
            Body=$"Mã OTP của bạn: {otp}\n\nMã có hiệu lực 5 phút, chỉ dùng một lần. Không chia sẻ mã này.\nNếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này."};
        message.To.Add(new MailAddress(recipient));
        using var client=new SmtpClient(options.Host.Trim(),options.Port) {EnableSsl=true,UseDefaultCredentials=false,
            Credentials=new NetworkCredential(options.Username.Trim(),IsGmail?AppPassword:options.Password),DeliveryMethod=SmtpDeliveryMethod.Network,Timeout=25000};
        using var timeout=new CancellationTokenSource(TimeSpan.FromSeconds(25));
        try { await client.SendMailAsync(message,timeout.Token); }
        catch(OperationCanceledException ex)
        { throw new InvalidOperationException("Gửi OTP quá thời gian chờ. Kiểm tra mạng và thử lại sau 60 giây.",ex); }
        catch(SmtpFailedRecipientsException ex)
        { throw new InvalidOperationException("Máy chủ thư từ chối địa chỉ nhận OTP. Kiểm tra lại email đã lưu trong tài khoản.",ex); }
        catch(SmtpException ex) when(IsAuthenticationFailure(ex))
        { throw new InvalidOperationException("Gmail từ chối xác thực. Admin cần tạo lại mật khẩu ứng dụng của tài khoản gửi, cập nhật cấu hình rồi chạy lại.",ex); }
        catch(SmtpException ex) when(HasCause<SocketException>(ex))
        { throw new InvalidOperationException("Không kết nối được máy chủ gửi thư. Kiểm tra mạng và kết nối smtp.gmail.com:587.",ex); }
        catch(SmtpException ex)
        { throw new InvalidOperationException("Máy chủ thư chưa chấp nhận gửi OTP. Kiểm tra cấu hình tài khoản gửi hoặc thử lại sau 60 giây.",ex); }
        catch(AuthenticationException ex)
        { throw new InvalidOperationException("Không thiết lập được kết nối TLS an toàn để gửi OTP. Kiểm tra ngày giờ và kết nối mạng.",ex); }
    }
    private static bool HasCause<T>(Exception ex) where T:Exception
    {
        for(Exception? cause=ex;cause is not null;cause=cause.InnerException)if(cause is T)return true;
        return false;
    }
    private static bool IsAuthenticationFailure(SmtpException ex)
    {
        // Chỉ phân loại, không hiển thị phản hồi SMTP chứa thông tin tài khoản.
        for(Exception? cause=ex;cause is not null;cause=cause.InnerException)
            if(cause is AuthenticationException || cause.Message.Contains("5.7.8",StringComparison.OrdinalIgnoreCase)
                || cause.Message.Contains("5.7.9",StringComparison.OrdinalIgnoreCase)
                || cause.Message.Contains("535",StringComparison.OrdinalIgnoreCase)
                || cause.Message.Contains("534",StringComparison.OrdinalIgnoreCase)
                || cause.Message.Contains("authentication",StringComparison.OrdinalIgnoreCase))return true;
        return ex.StatusCode==SmtpStatusCode.MustIssueStartTlsFirst;
    }
}