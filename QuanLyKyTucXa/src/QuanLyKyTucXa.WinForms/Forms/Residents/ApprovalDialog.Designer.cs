#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Residents;

partial class ApprovalDialog
{
    private System.ComponentModel.IContainer? components = null;
    private Label lblStudent = null!;
    private TableLayoutPanel tlpFields = null!;
    private Label lblBed = null!;
    private ComboBox _beds = null!;
    private Label lblStart = null!;
    private DateTimePicker _start = null!;
    private Label lblEnd = null!;
    private DateTimePicker _end = null!;
    private Label lblDeposit = null!;
    private NumericUpDown _deposit = null!;
    private Button btnApprove = null!;
    private Button btnCancel = null!;

    protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        lblStudent = new Label(); tlpFields = new TableLayoutPanel(); lblBed = new Label(); _beds = new ComboBox(); lblStart = new Label(); _start = new DateTimePicker(); lblEnd = new Label(); _end = new DateTimePicker(); lblDeposit = new Label(); _deposit = new NumericUpDown(); btnApprove = new Button(); btnCancel = new Button();
        tlpFields.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)_deposit).BeginInit(); SuspendLayout();
        lblStudent.AutoSize = true; lblStudent.Font = new Font("Segoe UI", 15F, FontStyle.Bold); lblStudent.ForeColor = Color.FromArgb(14, 34, 64); lblStudent.Location = new Point(22, 18);
        tlpFields.Location = new Point(22, 65); tlpFields.Size = new Size(456, 190); tlpFields.ColumnCount = 2; tlpFields.RowCount = 4; tlpFields.BackColor = Color.White; tlpFields.Padding = new Padding(12); tlpFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F)); tlpFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));  tlpFields.RowStyles.Add(new RowStyle(SizeType.Percent, 25F)); tlpFields.RowStyles.Add(new RowStyle(SizeType.Percent, 25F)); tlpFields.RowStyles.Add(new RowStyle(SizeType.Percent, 25F)); tlpFields.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        lblBed.Text = "Giường trống"; lblBed.Dock = DockStyle.Fill; lblBed.TextAlign = ContentAlignment.MiddleLeft; _beds.Dock = DockStyle.Fill; tlpFields.Controls.Add(lblBed, 0, 0); tlpFields.Controls.Add(_beds, 1, 0); _beds.DropDownStyle = ComboBoxStyle.DropDownList; lblStart.Text = "Bắt đầu"; lblStart.Dock = DockStyle.Fill; lblStart.TextAlign = ContentAlignment.MiddleLeft; _start.Dock = DockStyle.Fill; tlpFields.Controls.Add(lblStart, 0, 1); tlpFields.Controls.Add(_start, 1, 1); _start.Format = DateTimePickerFormat.Short; lblEnd.Text = "Kết thúc"; lblEnd.Dock = DockStyle.Fill; lblEnd.TextAlign = ContentAlignment.MiddleLeft; _end.Dock = DockStyle.Fill; tlpFields.Controls.Add(lblEnd, 0, 2); tlpFields.Controls.Add(_end, 1, 2); _end.Format = DateTimePickerFormat.Short; lblDeposit.Text = "Tiền cọc (VNĐ)"; lblDeposit.Dock = DockStyle.Fill; lblDeposit.TextAlign = ContentAlignment.MiddleLeft; _deposit.Dock = DockStyle.Fill; tlpFields.Controls.Add(lblDeposit, 0, 3); tlpFields.Controls.Add(_deposit, 1, 3); _deposit.Maximum = 100000000; _deposit.Increment = 100000; _deposit.Value = 500000; _deposit.ThousandsSeparator = true;
        btnApprove.Text = "Duyệt và lập hợp đồng"; btnApprove.Location = new Point(254, 275); btnApprove.Size = new Size(224, 38); btnApprove.BackColor = Color.FromArgb(59, 160, 72); btnApprove.ForeColor = Color.White; btnApprove.FlatStyle = FlatStyle.Flat; btnApprove.FlatAppearance.BorderSize = 0; btnApprove.DialogResult = DialogResult.OK;
        btnCancel.Text = "Hủy"; btnCancel.Location = new Point(164, 275); btnCancel.Size = new Size(78, 38); btnCancel.DialogResult = DialogResult.Cancel;
        AutoScaleDimensions = new SizeF(96F, 96F); AutoScaleMode = AutoScaleMode.Dpi; BackColor = Color.FromArgb(242, 246, 252); ClientSize = new Size(500, 330); Controls.Add(lblStudent); Controls.Add(tlpFields); Controls.Add(btnApprove); Controls.Add(btnCancel); Font = new Font("Segoe UI", 9F); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false; Name = "ApprovalDialog"; StartPosition = FormStartPosition.CenterParent; Text = "Duyệt đơn"; AcceptButton = btnApprove; CancelButton = btnCancel;
        tlpFields.ResumeLayout(false); tlpFields.PerformLayout(); ((System.ComponentModel.ISupportInitialize)_deposit).EndInit(); ResumeLayout(false); PerformLayout();
    }

}
