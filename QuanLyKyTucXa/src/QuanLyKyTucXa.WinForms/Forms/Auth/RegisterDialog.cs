using QuanLyKyTucXa.Infrastructure.Services;
using QuanLyKyTucXa.Infrastructure.Security;

namespace QuanLyKyTucXa.WinForms.Forms.Auth;

public partial class RegisterDialog : Form
{
    private readonly AuthService? _service;
    private bool _busy;
    public string RegisteredUsername { get; private set; } = "";
    public RegisterDialog()
    {
        InitializeComponent();
        QuanLyKyTucXa.WinForms.Helpers.ModernDialog.InstallAuth(this, fields, actions, btnSubmit, btnCancel, lblMessage, lblNotice, chkShowPassword, new(label0, txtUsername), new(label1, txtFullName), new(labelEmail, txtEmail, true), new(label2, txtPassword, true), new(label3, txtConfirm, true));
        footer.Paint += (_, e) => { using var pen = new Pen(Color.FromArgb(228, 236, 247)); e.Graphics.DrawLine(pen, 0, 0, footer.Width, 0); };
        Shown += (_, _) => txtUsername.Focus();
    }
    private void ShowPassword_Changed(object? sender, EventArgs e)
    {
        txtPassword.UseSystemPasswordChar = txtConfirm.UseSystemPasswordChar = !chkShowPassword.Checked;
    }
    public RegisterDialog(AuthService service) : this() => _service = service;
    private async void Submit_Click(object? sender, EventArgs e)
    {
        if (_busy || _service is null) return;
        if (txtPassword.Text != txtConfirm.Text) { lblMessage.Text = "Mật khẩu xác nhận không trùng khớp."; return; }
        try { AccountValidation.Username(txtUsername.Text); AccountValidation.FullName(txtFullName.Text); AccountValidation.Email(txtEmail.Text); PasswordHash.ValidateNew(txtPassword.Text); }
        catch(InvalidOperationException ex) { lblMessage.Text=ex.Message; return; }
        _busy = true; fields.Enabled = actions.Enabled = false; UseWaitCursor = true;
        lblMessage.Text = "Đang tạo tài khoản...";
        var succeeded = false;
        try
        {
            await _service.RegisterAsync(txtUsername.Text, txtFullName.Text, txtPassword.Text, txtEmail.Text);
            RegisteredUsername = txtUsername.Text.Trim();
            txtPassword.Clear(); txtConfirm.Clear();
            UseWaitCursor = false;
            MessageBox.Show(this, "Đăng ký thành công. Tài khoản chờ Admin liên kết hồ sơ và kích hoạt; email đã được lưu để nhận OTP.", "KTX Home", MessageBoxButtons.OK, MessageBoxIcon.Information);
            succeeded = true;
        }
        catch (Exception ex) { lblMessage.Text = ex is InvalidOperationException ? ex.Message : "Không tạo được tài khoản. Kiểm tra kết nối hoặc liên hệ Admin."; }
        finally { _busy = false; fields.Enabled = actions.Enabled = true; UseWaitCursor = false; }
        if (succeeded) { DialogResult = DialogResult.OK; Close(); }
    }
    private void Cancel_Click(object? sender, EventArgs e) { if (!_busy) Close(); }
    private void Busy_FormClosing(object? sender, FormClosingEventArgs e) { if (_busy) e.Cancel = true; }
}
