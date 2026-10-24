using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace QuanLyKyTucXa.Infrastructure.Security;

public static class AccountValidation
{
    public static string Username(string? value)
    {
        var text=(value??"").Trim();
        if(!Regex.IsMatch(text,@"\A[A-Za-z0-9][A-Za-z0-9_.@-]{1,48}[A-Za-z0-9]\z") || text.Contains(".."))
            throw new InvalidOperationException("Tên đăng nhập: 3–50 ký tự, bắt đầu/kết thúc bằng chữ hoặc số; chỉ dùng chữ không dấu, số, . _ @ - và không có khoảng trắng.");
        return text;
    }
    public static string FullName(string? value)
    {
        var text=Regex.Replace((value??"").Normalize(NormalizationForm.FormC).Trim(),@"\s+"," ");
        if(text.Length is <2 or >100 || !Regex.IsMatch(text,@"\A\p{L}[\p{L}\p{M}]*(?:[ '\u2019-]\p{L}[\p{L}\p{M}]*)*\z"))
            throw new InvalidOperationException("Họ tên: 2–100 ký tự chữ, có thể có dấu, khoảng trắng, dấu nối hoặc dấu nháy; không nhập số/ký tự đặc biệt khác.");
        return text;
    }
    public static string Email(string? value)
    {
        var text=(value??"").Trim();
        if(text.Length is <5 or >100 || text.Any(char.IsWhiteSpace) || !MailAddress.TryCreate(text,out var mail)
            || !string.Equals(mail.Address,text,StringComparison.Ordinal) || !mail.Host.Contains('.')
            || mail.User.StartsWith('.') || mail.User.EndsWith('.') || mail.User.Contains("..") || mail.Host.EndsWith(".invalid",StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Nhập email đang sử dụng, ví dụ ten@gmail.com. Không có khoảng trắng hoặc dấu chấm ngay trước @.");
        return text.ToLowerInvariant();
    }
}
