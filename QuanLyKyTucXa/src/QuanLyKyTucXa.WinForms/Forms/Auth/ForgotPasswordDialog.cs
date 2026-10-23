using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Auth;

public partial class ForgotPasswordDialog : Form
{
    private readonly AuthService? _service;
    private bool _busy;
    public ForgotPasswordDialog()
    {
        InitializeComponent();
        footer.Paint += (_, e) => { using var pen = new Pen(Color.FromArgb(228, 236, 247)); e.Graphics.DrawLine(pen, 0, 0, footer.Width, 0); };
        Shown += (_, _) => txtUsername.Focus();
    }
    private void ShowPassword_Changed(object? sender, EventArgs e)
    {
        txtPassword.UseSystemPasswordChar = txtConfirm.UseSystemPasswordChar = !chkShowPassword.Checked;
    }
    public ForgotPasswordDialog(AuthService service, string username) : this() { _service = service; txtUsername.Text = username; }
    private async void Submit_Click(object? sender, EventArgs e)
    {
        if (_busy || _service is null) return;
        if (txtPassword.Text != txtConfirm.Text) { lblMessage.Text = "Mật khẩu xác nhận không trùng khớp."; return; }
        _busy = true; fields.Enabled = actions.Enabled = false; UseWaitCursor = true;
        lblMessage.Text = "Đang khôi phục mật khẩu...";
        var succeeded = false;
        try
        {
            var code = await _service.ResetPasswordAsync(txtUsername.Text, txtRecovery.Text, txtPassword.Text);
            txtRecovery.Clear(); txtPassword.Clear(); txtConfirm.Clear();
            UseWaitCursor = false;
            using var receipt = new RecoveryCodeDialog(code, "Đã đặt lại mật khẩu. Mã cũ hết hiệu lực; hãy lưu mã khôi phục mới.");
            receipt.ShowDialog(this);
            succeeded = true;
        }
        catch (Exception ex) { lblMessage.Text = ex is InvalidOperationException ? ex.Message : "Không khôi phục được mật khẩu. Kiểm tra kết nối hoặc liên hệ Admin."; }
        finally { _busy = false; fields.Enabled = actions.Enabled = true; UseWaitCursor = false; }
        if (succeeded) { DialogResult = DialogResult.OK; Close(); }
    }
    private void Cancel_Click(object? sender, EventArgs e) { if (!_busy) Close(); }
    private void Busy_FormClosing(object? sender, FormClosingEventArgs e) { if (_busy) e.Cancel = true; }
}
