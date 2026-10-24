using System.Security.Cryptography;
using System.Text;

namespace QuanLyKyTucXa.Infrastructure.Security;

public static class PasswordHash
{
    private const int Iterations = 600_000;
    public static string Create(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return $"PBKDF2-SHA256${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string stored, out bool needsUpgrade)
    {
        needsUpgrade = false;
        try
        {
            if (stored.Length == 64 && stored.All(Uri.IsHexDigit))
            {
                needsUpgrade = true;
                return CryptographicOperations.FixedTimeEquals(SHA256.HashData(Encoding.UTF8.GetBytes(password)), Convert.FromHexString(stored));
            }
            var parts = stored.Split('$');
            if (parts.Length != 4 || parts[0] != "PBKDF2-SHA256" || !int.TryParse(parts[1], out var rounds) || rounds < 100_000 || rounds > 2_000_000) return false;
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            if (salt.Length != 16 || expected.Length != 32) return false;
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, rounds, HashAlgorithmName.SHA256, 32);
            needsUpgrade = rounds < Iterations;
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
    }
    public static void ValidateNew(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8 || password.Length > 128)
            throw new InvalidOperationException("Mật khẩu mới phải có từ 8 đến 128 ký tự.");
        if(password.Any(char.IsWhiteSpace) || password.Any(char.IsControl) || !password.Any(char.IsUpper)
            || !password.Any(char.IsLower) || !password.Any(char.IsDigit) || !password.Any(c=>!char.IsLetterOrDigit(c)))
            throw new InvalidOperationException("Mật khẩu phải có chữ hoa, chữ thường, số, ký tự đặc biệt và không chứa khoảng trắng.");
    }
}