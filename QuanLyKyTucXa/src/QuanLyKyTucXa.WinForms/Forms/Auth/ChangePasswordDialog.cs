using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Auth;

public partial class ChangePasswordDialog : Form
{
    private readonly AuthService? _service;
    private readonly AuthSession? _session;
    private bool _busy;
    public ChangePasswordDialog() { InitializeComponent(); QuanLyKyTucXa.WinForms.Helpers.ModernDialog.Install(this, tlpFields, btnSave, btnCancel, lblMessage); }
    public ChangePasswordDialog(AuthService service, AuthSession session) : this() { _service = service; _session = session; }
    private async void Save_Click(object? sender, EventArgs e)
    {
        if (_busy || _service is null || _session is null) return;
        if (txtNewPassword.Text != txtConfirmPassword.Text) { lblMessage.Text = "Mật khẩu nhập lại chưa khớp."; return; }
        _busy = true;
        tlpFields.Enabled = btnSave.Enabled = btnCancel.Enabled = false;
        lblMessage.Text = "Đang đổi mật khẩu...";
        try
        {
            await _service.ChangePasswordAsync(_session, txtCurrentPassword.Text, txtNewPassword.Text);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) { lblMessage.Text = ex is InvalidOperationException or UnauthorizedAccessException ? ex.Message : "Không đổi được mật khẩu. Kiểm tra kết nối rồi thử lại."; }
        finally { _busy = false; tlpFields.Enabled = btnSave.Enabled = btnCancel.Enabled = true; }
        if (DialogResult == DialogResult.OK) Close();
    }
    private void Cancel_Click(object? sender, EventArgs e) { if (!_busy) Close(); }
    private void Auth_FormClosing(object? sender, FormClosingEventArgs e) { if (_busy) e.Cancel = true; }
}