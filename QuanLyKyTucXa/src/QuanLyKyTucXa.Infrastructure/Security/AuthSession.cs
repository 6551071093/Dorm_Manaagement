using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;

namespace QuanLyKyTucXa.Infrastructure.Security;

public enum AppPermission { Staff, ManageResidents, ManageRooms, Register, ViewApplications, ApproveApplications, ManageUsers, Personal, DeleteResidents, DeleteRooms, ImportResidents, ViewReports, ConfigureRooms }

public sealed class AuthSession
{
    internal AuthSession(int userId, string username, string fullName, string role, string? studentId, string credentialHash)
    {
        UserId = userId; Username = username; FullName = fullName; Role = role; StudentId = studentId; CredentialHash = credentialHash;
    }
    internal string CredentialHash { get; set; }
    public int UserId { get; }
    public string Username { get; }
    public string FullName { get; }
    public string Role { get; }
    public string? StudentId { get; }
    public bool IsActive { get; private set; } = true;
    public bool IsStaff => Role is "Admin" or "NhanVien";
    public bool IsAdmin => Role == "Admin";
    public bool IsStudent => Role == "SinhVien";
    public string RoleDisplayName => Role switch { "Admin" => "Quản trị viên", "NhanVien" => "Nhân viên", "SinhVien" => "Sinh viên", _ => "Không xác định" };
    public void SignOut() => IsActive = false;
}

public static class AuthPolicy
{
    public static bool Can(AuthSession? session, AppPermission permission)
    {
        if (session?.IsActive != true) return false;
        return permission switch {
            AppPermission.ConfigureRooms or AppPermission.ManageUsers or AppPermission.DeleteResidents or AppPermission.DeleteRooms or AppPermission.ImportResidents or AppPermission.ViewReports => session.IsAdmin,
            AppPermission.Personal => session.IsStudent && session.StudentId is not null,
            AppPermission.Register or AppPermission.ViewApplications => session.IsStaff || (session.IsStudent && session.StudentId is not null),
            AppPermission.Staff or AppPermission.ManageResidents or AppPermission.ManageRooms or AppPermission.ApproveApplications => session.IsStaff,
            _ => false
        };
    }

    public static async Task RequireAsync(AppDbContext db, AuthSession? session, AppPermission permission)
    {
        if (!Can(session, permission)) throw new UnauthorizedAccessException("Bạn chưa đăng nhập hoặc không có quyền thực hiện chức năng này.");
        var account = await db.NguoiDungs.AsNoTracking().Where(x => x.MaNguoiDung == session!.UserId)
            .Select(x => new { x.TrangThai, x.VaiTro, x.MatKhauHash }).SingleOrDefaultAsync();
        if (account is null || !account.TrangThai || account.VaiTro != session!.Role || account.MatKhauHash != session.CredentialHash)
        {
            session!.SignOut();
            throw new UnauthorizedAccessException("Tài khoản, mật khẩu hoặc quyền truy cập đã thay đổi. Vui lòng đăng nhập lại.");
        }
        if (session.IsStudent)
        {
            var linked = await db.SinhViens.Where(x => x.MaNguoiDung == session.UserId).Select(x => x.MaSv).ToListAsync();
            if (linked.Count != 1 || linked[0] != session.StudentId)
            {
                session.SignOut();
                throw new UnauthorizedAccessException("Liên kết hồ sơ sinh viên đã thay đổi. Vui lòng đăng nhập lại.");
            }
        }
    }
}