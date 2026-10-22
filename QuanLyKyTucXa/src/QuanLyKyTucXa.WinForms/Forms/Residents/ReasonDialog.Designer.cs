#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Residents;

partial class ReasonDialog
{
    private System.ComponentModel.IContainer? components = null;
    private Label lblPrompt = null!;
    private TextBox txtReason = null!;
    private Button btnConfirm = null!;
    private Button btnCancel = null!;

    protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        lblPrompt = new Label(); txtReason = new TextBox(); btnConfirm = new Button(); btnCancel = new Button(); SuspendLayout();
        lblPrompt.AutoSize = true; lblPrompt.Location = new Point(16, 16); lblPrompt.Text = "Nhập lý do để lưu vào đơn đăng ký:";
        txtReason.Location = new Point(16, 46); txtReason.Size = new Size(388, 75); txtReason.Multiline = true; txtReason.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        btnConfirm.Text = "Xác nhận từ chối"; btnConfirm.Location = new Point(250, 136); btnConfirm.Size = new Size(154, 36); btnConfirm.DialogResult = DialogResult.OK;
        btnCancel.Text = "Hủy"; btnCancel.Location = new Point(164, 136); btnCancel.Size = new Size(76, 36); btnCancel.DialogResult = DialogResult.Cancel;
        AutoScaleDimensions = new SizeF(8F, 20F); AutoScaleMode = AutoScaleMode.Font; ClientSize = new Size(420, 190); Controls.Add(lblPrompt); Controls.Add(txtReason); Controls.Add(btnConfirm); Controls.Add(btnCancel); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false; Name = "ReasonDialog"; StartPosition = FormStartPosition.CenterParent; Text = "Lý do từ chối"; AcceptButton = btnConfirm; CancelButton = btnCancel;
        ResumeLayout(false); PerformLayout();
    }
}

