using System.Net;
using System.Net.Mail;
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
    public static SmtpOtpEmailSender FromEnvironment() => new(new(
        Environment.GetEnvironmentVariable("Email__Host")??"smtp.gmail.com",587,
        Environment.GetEnvironmentVariable("Email__Username")??"",Environment.GetEnvironmentVariable("Email__Password")??"",
        Environment.GetEnvironmentVariable("Email__FromAddress")??"", "KTX Home"));
    public void EnsureConfigured()
    {
        if(string.IsNullOrWhiteSpace(options.Host) || options.Port is <1 or >65535 || string.IsNullOrWhiteSpace(options.Username)
            || string.IsNullOrWhiteSpace(options.Password) || !MailAddress.TryCreate(options.FromAddress,out _))
            throw new InvalidOperationException("Chưa cấu hình gửi OTP. Quản trị viên cần nhập mật khẩu ứng dụng Gmail trên máy triển khai.");
    }
    public async Task SendAsync(string recipient,string otp)
    {
        EnsureConfigured();
        using var message=new MailMessage {From=new MailAddress(options.FromAddress,options.FromName,Encoding.UTF8),
            Subject="KTX Home — Mã OTP đặt lại mật khẩu",SubjectEncoding=Encoding.UTF8,BodyEncoding=Encoding.UTF8,
            Body=$"Mã OTP của bạn: {otp}\n\nMã có hiệu lực 5 phút, chỉ dùng một lần. Không chia sẻ mã này.\nNếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này."};
        message.To.Add(new MailAddress(recipient));
        using var client=new SmtpClient(options.Host,options.Port) {EnableSsl=true,UseDefaultCredentials=false,
            Credentials=new NetworkCredential(options.Username,options.Password.Replace(" ","")),DeliveryMethod=SmtpDeliveryMethod.Network,Timeout=20000};
        try {await client.SendMailAsync(message,CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(25));}
        catch(Exception ex) when(ex is SmtpException or TimeoutException or OperationCanceledException)
        {throw new InvalidOperationException("Không gửi được OTP. Kiểm tra mạng và cấu hình Gmail; chưa thể đặt lại mật khẩu.");}
    }
}
