#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Auth;
partial class RecoveryCodeDialog
{
    private System.ComponentModel.IContainer? components;
    private Label lblMessage = null!;
    private TextBox txtCode = null!;
    private CheckBox chkSaved = null!;
    private Button btnClose = null!;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        var root = new TableLayoutPanel(); var title = new Label(); var note = new Label();
        lblMessage = new Label(); txtCode = new TextBox(); chkSaved = new CheckBox(); btnClose = new Button();
        root.Dock = DockStyle.Fill; root.Padding = new Padding(24); root.ColumnCount = 1; root.RowCount = 6;
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        title.Text = "Lưu mã khôi phục"; title.Font = new Font("Segoe UI", 20F, FontStyle.Bold); title.Dock = DockStyle.Fill;
        lblMessage.Dock = DockStyle.Fill; lblMessage.ForeColor = Color.FromArgb(23, 101, 183);
        txtCode.ReadOnly = true; txtCode.Multiline = true; txtCode.Dock = DockStyle.Fill; txtCode.Font = new Font("Consolas", 12F);
        txtCode.BackColor = Color.White; txtCode.MaxLength = 48;
        note.Text = "Chọn và sao chép mã này để cất ở nơi riêng. Mã chỉ hiển thị lần này, dùng để đặt lại mật khẩu. Không gửi mã cho người khác.";
        note.Dock = DockStyle.Fill; note.Padding = new Padding(0, 12, 0, 0);
        chkSaved.Text = "Tôi đã lưu mã khôi phục"; chkSaved.Dock = DockStyle.Fill; chkSaved.CheckedChanged += Confirm_Changed;
        btnClose.Text = "Hoàn tất"; btnClose.Size = new Size(128, 38); btnClose.Anchor = AnchorStyles.Right;
        btnClose.BackColor = Color.FromArgb(23, 101, 183); btnClose.ForeColor = Color.White; btnClose.FlatStyle = FlatStyle.Flat;
        btnClose.Enabled = false; btnClose.Click += Close_Click;
        root.Controls.Add(title, 0, 0); root.Controls.Add(lblMessage, 0, 1); root.Controls.Add(txtCode, 0, 2);
        root.Controls.Add(note, 0, 3); root.Controls.Add(chkSaved, 0, 4); root.Controls.Add(btnClose, 0, 5);
        Controls.Add(root); ClientSize = new Size(620, 390); Font = new Font("Segoe UI", 11F);
        BackColor = Color.FromArgb(246, 246, 250); ForeColor = Color.FromArgb(14, 34, 64);
        AutoScaleDimensions = new SizeF(96F, 96F); AutoScaleMode = AutoScaleMode.Dpi;
        StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false; MaximizeBox = false; Text = "KTX Home — Mã khôi phục"; Name = "RecoveryCodeDialog";
        FormClosing += Receipt_FormClosing;
    }
}
