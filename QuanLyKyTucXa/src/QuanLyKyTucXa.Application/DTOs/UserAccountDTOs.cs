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
    public string? Email { get; set; }
    public string Role { get; set; } = "NhanVien";
    public bool Enabled { get; set; } = true;
    public string? StudentId { get; set; }
    public string? NewPassword { get; set; }
}
