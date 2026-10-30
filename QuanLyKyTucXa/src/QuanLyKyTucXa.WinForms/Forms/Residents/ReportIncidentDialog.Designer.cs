#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Residents;
partial class ReportIncidentDialog
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel layout = null!;
    private Label lblTitle = null!;
    private TextBox txtTitle = null!;
    private Label lblDescription = null!;
    private TextBox txtDescription = null!;
    private Label lblError = null!;
    private FlowLayoutPanel actions = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        layout = new TableLayoutPanel(); lblTitle = new Label(); txtTitle = new TextBox(); lblDescription = new Label(); txtDescription = new TextBox(); lblError = new Label(); actions = new FlowLayoutPanel(); btnSave = new Button(); btnCancel = new Button();
        layout.SuspendLayout(); actions.SuspendLayout(); SuspendLayout();
        layout.Dock = DockStyle.Fill; layout.Padding = new Padding(20); layout.ColumnCount = 1; layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowCount = 6; layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F)); layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(lblTitle, 0, 0); layout.Controls.Add(txtTitle, 0, 1); layout.Controls.Add(lblDescription, 0, 2); layout.Controls.Add(txtDescription, 0, 3); layout.Controls.Add(lblError, 0, 4); layout.Controls.Add(actions, 0, 5);
        lblTitle.Text = "Tiêu đề *"; lblTitle.Dock = DockStyle.Fill;
        txtTitle.Dock = DockStyle.Top; txtTitle.MaxLength = 200;
        lblDescription.Text = "Mô tả *"; lblDescription.Dock = DockStyle.Fill;
        txtDescription.Dock = DockStyle.Fill; txtDescription.Multiline = true; txtDescription.ScrollBars = ScrollBars.Vertical; txtDescription.MaxLength = 2000;
        lblError.Dock = DockStyle.Fill; lblError.ForeColor = Color.Firebrick;
        actions.AutoSize = true; actions.AutoSizeMode = AutoSizeMode.GrowAndShrink; actions.Dock = DockStyle.Fill; actions.FlowDirection = FlowDirection.RightToLeft; actions.Controls.Add(btnCancel); actions.Controls.Add(btnSave);
        btnSave.Text = "Gửi báo hỏng"; btnSave.Size = new Size(140, 36); btnSave.BackColor = Color.FromArgb(23, 101, 183); btnSave.ForeColor = Color.White; btnSave.FlatStyle = FlatStyle.Flat; btnSave.Click += Save_Click;
        btnCancel.Text = "Hủy"; btnCancel.Size = new Size(84, 36); btnCancel.Click += Cancel_Click;
        AutoScaleDimensions = new SizeF(96F, 96F); AutoScaleMode = AutoScaleMode.Dpi; Font = new Font("Segoe UI", 11F); ClientSize = new Size(560, 460);
        StartPosition = FormStartPosition.CenterParent; Name = "ReportIncidentDialog"; Text = "Gửi báo hỏng phòng đang ở"; MinimizeBox = false; MaximizeBox = false; FormBorderStyle = FormBorderStyle.FixedDialog;
        Controls.Add(layout); FormClosing += Editor_Closing;
        actions.ResumeLayout(false); layout.ResumeLayout(false); layout.PerformLayout(); ResumeLayout(false);
    }
}