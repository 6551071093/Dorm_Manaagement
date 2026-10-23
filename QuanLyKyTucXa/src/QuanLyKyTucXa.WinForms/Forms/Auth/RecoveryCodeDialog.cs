namespace QuanLyKyTucXa.WinForms.Forms.Auth;

public partial class RecoveryCodeDialog : Form
{
    public RecoveryCodeDialog() => InitializeComponent();
    public RecoveryCodeDialog(string code, string message) : this() { txtCode.Text = code; lblMessage.Text = message; }
    private void Confirm_Changed(object? sender, EventArgs e) => btnClose.Enabled = chkSaved.Checked;
    private void Close_Click(object? sender, EventArgs e) { if (chkSaved.Checked) Close(); }
    private void Receipt_FormClosing(object? sender, FormClosingEventArgs e) { if (!chkSaved.Checked) e.Cancel = true; }
}
