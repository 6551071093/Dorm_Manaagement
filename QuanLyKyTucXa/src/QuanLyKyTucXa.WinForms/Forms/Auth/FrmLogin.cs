using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Auth;

public partial class FrmLogin : Form
{
    private readonly AuthService? _service;
    private bool _busy;


    public AuthSession? Session { get; private set; }
    public FrmLogin()
    {
        InitializeComponent();
        footer.Layout += (_, _) => flpLinks.Left = Math.Max(0, (footer.ClientSize.Width - flpLinks.Width) / 2);
        footer.Paint += (_, e) => { using var pen = new Pen(Color.FromArgb(228, 236, 247)); e.Graphics.DrawLine(pen, 0, 0, footer.Width, 0); };
        Shown += (_, _) => txtUsername.Focus();
    }
    public FrmLogin(AuthService service) : this() => _service = service;
    private void ShowPassword_Changed(object? sender, EventArgs e) => txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
    private async void Login_Click(object? sender, EventArgs e)
    {
        if (_busy || _service is null) return;
        if (txtUsername.Text.Trim().Length == 0 || txtPassword.Text.Length == 0)
        { lblMessage.Text = "Vui lòng nhập tên đăng nhập và mật khẩu."; return; }
        _busy = true;
        tlpFields.Enabled = flpLinks.Enabled = btnSave.Enabled = btnCancel.Enabled = false;
        lblMessage.Text = "Đang đăng nhập...";
        UseWaitCursor = true;
        try { Session = await _service.LoginAsync(txtUsername.Text, txtPassword.Text); }
        catch (InvalidOperationException ex) { lblMessage.Text = ex.Message; }
        catch (Exception) { lblMessage.Text = "Không kết nối được hệ thống. Vui lòng thử lại hoặc liên hệ quản trị viên."; }
        finally
        {
            _busy = false;
            UseWaitCursor = false;
            tlpFields.Enabled = flpLinks.Enabled = btnSave.Enabled = btnCancel.Enabled = true;
        }
        txtPassword.Clear();
        if (Session is not null) Close();
        else txtPassword.Focus();
    }
    private void Register_Click(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        if (_busy || _service is null) return;
        using var dialog = new RegisterDialog(_service);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        txtUsername.Text = dialog.RegisteredUsername; txtPassword.Clear();
        lblMessage.Text = "Đã đăng ký. Liên hệ Admin để liên kết hồ sơ sinh viên và kích hoạt tài khoản.";
    }
    private void Forgot_Click(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        if (_busy || _service is null) return;
        using var dialog = new ForgotPasswordDialog(_service, txtUsername.Text);
        if (dialog.ShowDialog(this) == DialogResult.OK) {
            txtPassword.Clear(); lblMessage.Text = "Đã đặt lại mật khẩu. Hãy đăng nhập bằng mật khẩu mới.";
        }
    }
    private void Cancel_Click(object? sender, EventArgs e) { if (!_busy) Close(); }
    private void Auth_FormClosing(object? sender, FormClosingEventArgs e) { if (_busy) e.Cancel = true; }
}