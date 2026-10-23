namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public sealed partial class ReasonDialog : Form
{
    public string Reason => txtReason.Text.Trim();
    public ReasonDialog()
    {
        InitializeComponent();
        QuanLyKyTucXa.WinForms.Helpers.DialogLayout.ConfigureFixed(this, btnConfirm, btnCancel);
        txtReason.MaxLength = 200;
        btnConfirm.DialogResult = DialogResult.None;
        btnConfirm.Click += (_, _) => {
            if (Reason.Length == 0) { MessageBox.Show(this, "Vui lòng nhập lý do từ chối."); txtReason.Focus(); return; }
            DialogResult = DialogResult.OK;
        };
    }
}
