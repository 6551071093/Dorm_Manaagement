using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;
using QuanLyKyTucXa.WinForms.Forms.Auth;
using QuanLyKyTucXa.WinForms.Forms.Residents;
using QuanLyKyTucXa.WinForms.Forms.Users;

namespace QuanLyKyTucXa.WinForms.Forms.Main;

public partial class FrmMain
{
    private readonly AuthSession? _session;
    private Task<bool>? _sessionCheck;
    public bool LogoutRequested { get; private set; }

    private void ConfigureAuthenticationUi()
    {
        if (_session is null) return;
        Text = "Quản lý ký túc xá - " + _session.RoleDisplayName;
        lblProfileName.Text = _session.Username;
        lblProfileRole.Text = _session.RoleDisplayName;
        lblProfileAvatar.Text = new string(_session.Username.Take(2).ToArray()).ToUpperInvariant();
        classicAccountText.Text = $"Tài khoản: {_session.Username} ({_session.RoleDisplayName}) | {DateTime.Today:dd/MM/yyyy}";
        var users = new ToolStripMenuItem("Quản lý tài khoản") { Name = "mnuUsers", Visible = _session.IsAdmin };
        users.Click += (_, _) => ShowUsers();
        var password = new ToolStripMenuItem("Đổi mật khẩu") { Name = "mnuChangePassword" };
        password.Click += async (_, _) => {
            if (!await EnsureSessionAsync() || _session is null) return;
            using var dialog = new ChangePasswordDialog(new AuthService(() => AppDbContextFactory.Create(_connectionString)), _session);
            if (dialog.ShowDialog(this) == DialogResult.OK) MessageBox.Show(this, "Đã đổi mật khẩu.", "Tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };
        var logout = new ToolStripMenuItem("Đăng xuất") { Name = "mnuLogout" };
        logout.Click += (_, _) => RequestLogout();
        mnuSystem.DropDownItems.Insert(1, users);
        mnuSystem.DropDownItems.Insert(2, password);
        mnuSystem.DropDownItems.Insert(3, logout);
        mnuSystem.DropDownItems.Insert(4, new ToolStripSeparator());
        var staff = _session.IsStaff;
        mnuCatalog.Visible = staff;
        mnuContracts.Visible = mnuApprove.Visible = mnuUtility.Visible = mnuReports.Visible = staff;
        tsStudents.Visible = tsContracts.Visible = staff;
        btnHeaderAdd.Visible = staff;
        _searchShell.Visible = staff;
        btnNavStudents.Visible = btnNavApprovals.Visible = btnNavUtilities.Visible = btnNavReports.Visible = staff;
        btnOpenStudents.Visible = btnOpenRooms.Visible = btnOpenApprovals.Visible = staff;
        if (_session.IsStudent)
        {
            mnuHome.Text = mnuDashboard.Text = "Thông tin cá nhân";
            tsRooms.Text = "Phòng của tôi";
            tsInvoices.Text = "Hóa đơn của tôi";
            tsIncidents.Text = "Báo hỏng của tôi";
            tabDashboard.Text = "Cá nhân";
            tabDashboard.Controls.Remove(pnlHome);
            pnlHome.Visible = false;
        }
    }

    private bool RequireUi(AppPermission permission)
    {
        if (AuthPolicy.Can(_session, permission)) return true;
        MessageBox.Show(this, "Bạn chưa đăng nhập hoặc không có quyền mở chức năng này.", "Phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
    }

    private Task<bool> EnsureSessionAsync()
    {
        if (_sessionCheck is { IsCompleted: false }) return _sessionCheck;
        return _sessionCheck = ValidateSessionAsync();
    }
    private async Task<bool> ValidateSessionAsync()
    {
        if (_session?.IsActive != true) { RequestLogout(); return false; }
        try
        {
            await using var db = AppDbContextFactory.Create(_connectionString);
            await AuthPolicy.RequireAsync(db, _session, _session.IsStudent ? AppPermission.Personal : AppPermission.Staff);
            return !IsDisposed && !Disposing;
        }
        catch (UnauthorizedAccessException ex)
        {
            if (!IsDisposed && !Disposing) { MessageBox.Show(this, ex.Message, "Phiên đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Warning); RequestLogout(); }
        }
        catch (Exception) { if (!IsDisposed) MessageBox.Show(this, "Không xác nhận được phiên đăng nhập. Kiểm tra kết nối rồi thử lại.", "Kết nối", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        return false;
    }

    private void RequestLogout()
    {
        if (IsDisposed || Disposing) return;
        LogoutRequested = true;
        _session?.SignOut();
        Close();
    }

    private void ShowUsers()
    {
        if (!RequireUi(AppPermission.ManageUsers) || _session is null) return;
        var form = new FrmNguoiDung(_connectionString, _session);
        form.ReturnHomeRequested += (_, _) => CloseCurrentTab();
        ShowChild(form, "Tài khoản", "Quản lý người dùng và phân quyền");
    }

    private async void ShowPersonal(string? section = null)
    {
        if (!RequireUi(AppPermission.Personal) || _session is null) return;
        var form = tabDashboard.Controls.OfType<FrmCaNhan>().FirstOrDefault();
        if (form is null)
        {
            form = new FrmCaNhan(_connectionString, _session) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
            tabDashboard.Controls.Add(form);
            form.Show();
        }
        _activeChild = form;
        tabWorkspace.SelectedTab = tabDashboard;
        form.SelectSection(section);
        await form.RefreshViewAsync();
    }
}