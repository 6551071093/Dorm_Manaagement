namespace QuanLyKyTucXa.Infrastructure.Security;

public enum AppPermission
{
    Staff,
    ManageResidents,
    ManageRooms,
    Register,
    ViewApplications,
    ApproveApplications,
    ManageUsers,
    Personal,
    DeleteResidents,
    DeleteRooms,
    ImportResidents,
    ViewReports,
    ConfigureRooms
}

public sealed class AuthSession
{
    public AuthSession(int userId, string username, string fullName, string role, string? studentId, string credentialHash)
    {
        UserId = userId;
        Username = username;
        FullName = fullName;
        Role = role;
        StudentId = studentId;
        CredentialHash = credentialHash;
    }

    public string CredentialHash { get; set; }
    public int UserId { get; }
    public string Username { get; }
    public string FullName { get; }
    public string Role { get; }
    public string? StudentId { get; }
    public bool IsActive { get; private set; } = true;
    public bool IsStaff => Role is "Admin" or "NhanVien";
    public bool IsAdmin => Role == "Admin";
    public bool IsStudent => Role == "SinhVien";
    public string RoleDisplayName => Role switch
    {
        "Admin" => "Quản trị viên",
        "NhanVien" => "Nhân viên",
        "SinhVien" => "Sinh viên",
        _ => "Không xác định"
    };

    public void SignOut() => IsActive = false;
}
