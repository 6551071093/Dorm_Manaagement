#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Residents;

partial class FrmDangKyKtx
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel tlpRoot = null!;
    private Label lblTitle = null!;
    private Label lblNotice = null!;
    private GroupBox grpRegistration = null!;
    private Panel pnlFormScroll = null!;
    private TableLayoutPanel tlpFields = null!;
    private Label lblSinhVien = null!;
    private ComboBox cboSinhVien = null!;
    private Label lblPhong = null!;
    private ComboBox cboPhong = null!;
    private Label lblHocKy = null!;
    private ComboBox cboHocKy = null!;
    private Label lblNamHoc = null!;
    private TextBox txtNamHoc = null!;
    private Label lblGhiChu = null!;
    private TextBox txtGhiChu = null!;
    private Panel pnlFooter = null!;
    private FlowLayoutPanel flpActions = null!;
    private Label lblMessage = null!;
    private Button btnGuiDon = null!;
    private Button btnClose = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        tlpRoot = new TableLayoutPanel();
        lblTitle = new Label();
        lblNotice = new Label();
        grpRegistration = new GroupBox();
        pnlFormScroll = new Panel();
        tlpFields = new TableLayoutPanel();
        lblSinhVien = new Label();
        cboSinhVien = new ComboBox();
        lblPhong = new Label();
        cboPhong = new ComboBox();
        lblHocKy = new Label();
        cboHocKy = new ComboBox();
        lblNamHoc = new Label();
        txtNamHoc = new TextBox();
        lblGhiChu = new Label();
        txtGhiChu = new TextBox();
        pnlFooter = new Panel();
        lblMessage = new Label();
        flpActions = new FlowLayoutPanel();
        btnClose = new Button();
        btnGuiDon = new Button();
        tlpRoot.SuspendLayout();
        grpRegistration.SuspendLayout();
        pnlFormScroll.SuspendLayout();
        tlpFields.SuspendLayout();
        pnlFooter.SuspendLayout();
        flpActions.SuspendLayout();
        SuspendLayout();
        // 
        // tlpRoot
        // 
        tlpRoot.BackColor = Color.FromArgb(246, 246, 250);
        tlpRoot.ColumnCount = 1;
        tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tlpRoot.Controls.Add(lblTitle, 0, 0);
        tlpRoot.Controls.Add(lblNotice, 0, 1);
        tlpRoot.Controls.Add(grpRegistration, 0, 2);
        tlpRoot.Controls.Add(pnlFooter, 0, 3);
        tlpRoot.Dock = DockStyle.Fill;
        tlpRoot.Location = new Point(0, 0);
        tlpRoot.Name = "tlpRoot";
        tlpRoot.Padding = new Padding(20, 14, 20, 12);
        tlpRoot.RowCount = 4;
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        tlpRoot.Size = new Size(1100, 620);
        tlpRoot.TabIndex = 0;
        // 
        // lblTitle
        // 
        lblTitle.Dock = DockStyle.Fill;
        lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(35, 40, 48);
        lblTitle.Location = new Point(20, 14);
        lblTitle.Margin = new Padding(0, 0, 0, 6);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(1060, 52);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Đăng ký ký túc xá";
        lblTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblNotice
        // 
        lblNotice.BackColor = Color.FromArgb(219, 234, 254);
        lblNotice.Dock = DockStyle.Fill;
        lblNotice.ForeColor = Color.FromArgb(30, 64, 175);
        lblNotice.Location = new Point(20, 72);
        lblNotice.Margin = new Padding(0, 0, 0, 8);
        lblNotice.Name = "lblNotice";
        lblNotice.Padding = new Padding(12, 0, 12, 0);
        lblNotice.Size = new Size(1060, 44);
        lblNotice.TabIndex = 1;
        lblNotice.Text = "Đơn đăng ký lưu phòng đề xuất. Giường sẽ được chọn khi nhân viên duyệt đơn.";
        lblNotice.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // grpRegistration
        // 
        grpRegistration.Controls.Add(pnlFormScroll);
        grpRegistration.Dock = DockStyle.Fill;
        grpRegistration.Location = new Point(20, 124);
        grpRegistration.Margin = new Padding(0);
        grpRegistration.Name = "grpRegistration";
        grpRegistration.Padding = new Padding(14, 8, 12, 10);
        grpRegistration.Size = new Size(1060, 428);
        grpRegistration.TabIndex = 2;
        grpRegistration.TabStop = false;
        grpRegistration.Text = "Thông tin đăng ký";
        // 
        // pnlFormScroll
        // 
        pnlFormScroll.AutoScroll = true;
        pnlFormScroll.Controls.Add(tlpFields);
        pnlFormScroll.Dock = DockStyle.Fill;
        pnlFormScroll.Location = new Point(14, 33);
        pnlFormScroll.Name = "pnlFormScroll";
        pnlFormScroll.Size = new Size(1034, 385);
        pnlFormScroll.TabIndex = 0;
        // 
        // tlpFields
        // 
        tlpFields.AutoSize = true;
        tlpFields.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tlpFields.ColumnCount = 2;
        tlpFields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
        tlpFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tlpFields.Controls.Add(lblSinhVien, 0, 0);
        tlpFields.Controls.Add(cboSinhVien, 1, 0);
        tlpFields.Controls.Add(lblPhong, 0, 1);
        tlpFields.Controls.Add(cboPhong, 1, 1);
        tlpFields.Controls.Add(lblHocKy, 0, 2);
        tlpFields.Controls.Add(cboHocKy, 1, 2);
        tlpFields.Controls.Add(lblNamHoc, 0, 3);
        tlpFields.Controls.Add(txtNamHoc, 1, 3);
        tlpFields.Controls.Add(lblGhiChu, 0, 4);
        tlpFields.Controls.Add(txtGhiChu, 1, 4);
        tlpFields.Dock = DockStyle.Top;
        tlpFields.Location = new Point(0, 0);
        tlpFields.MaximumSize = new Size(1000, 0);
        tlpFields.Name = "tlpFields";
        tlpFields.Padding = new Padding(0, 8, 12, 8);
        tlpFields.RowCount = 5;
        tlpFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        tlpFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        tlpFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        tlpFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        tlpFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 112F));
        tlpFields.Size = new Size(1000, 320);
        tlpFields.TabIndex = 0;
        // 
        // lblSinhVien
        // 
        lblSinhVien.Dock = DockStyle.Fill;
        lblSinhVien.Location = new Point(0, 8);
        lblSinhVien.Margin = new Padding(0, 0, 10, 0);
        lblSinhVien.Name = "lblSinhVien";
        lblSinhVien.Size = new Size(150, 48);
        lblSinhVien.TabIndex = 0;
        lblSinhVien.Text = "Sinh viên *";
        lblSinhVien.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // cboSinhVien
        // 
        cboSinhVien.Dock = DockStyle.Top;
        cboSinhVien.DropDownStyle = ComboBoxStyle.DropDownList;
        cboSinhVien.DropDownWidth = 620;
        cboSinhVien.FormattingEnabled = true;
        cboSinhVien.Location = new Point(160, 13);
        cboSinhVien.Margin = new Padding(0, 5, 0, 8);
        cboSinhVien.Name = "cboSinhVien";
        cboSinhVien.Size = new Size(828, 33);
        cboSinhVien.TabIndex = 0;
        // 
        // lblPhong
        // 
        lblPhong.Dock = DockStyle.Fill;
        lblPhong.Location = new Point(0, 56);
        lblPhong.Margin = new Padding(0, 0, 10, 0);
        lblPhong.Name = "lblPhong";
        lblPhong.Size = new Size(150, 48);
        lblPhong.TabIndex = 1;
        lblPhong.Text = "Phòng đề xuất";
        lblPhong.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // cboPhong
        // 
        cboPhong.Dock = DockStyle.Top;
        cboPhong.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPhong.DropDownWidth = 620;
        cboPhong.FormattingEnabled = true;
        cboPhong.Location = new Point(160, 61);
        cboPhong.Margin = new Padding(0, 5, 0, 8);
        cboPhong.Name = "cboPhong";
        cboPhong.Size = new Size(828, 33);
        cboPhong.TabIndex = 1;
        // 
        // lblHocKy
        // 
        lblHocKy.Dock = DockStyle.Fill;
        lblHocKy.Location = new Point(0, 104);
        lblHocKy.Margin = new Padding(0, 0, 10, 0);
        lblHocKy.Name = "lblHocKy";
        lblHocKy.Size = new Size(150, 48);
        lblHocKy.TabIndex = 2;
        lblHocKy.Text = "Học kỳ *";
        lblHocKy.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // cboHocKy
        // 
        cboHocKy.Dock = DockStyle.Top;
        cboHocKy.DropDownStyle = ComboBoxStyle.DropDownList;
        cboHocKy.Location = new Point(160, 109);
        cboHocKy.Margin = new Padding(0, 5, 0, 8);
        cboHocKy.MaximumSize = new Size(160, 0);
        cboHocKy.Name = "cboHocKy";
        cboHocKy.Size = new Size(160, 33);
        cboHocKy.TabIndex = 2;
        // 
        // lblNamHoc
        // 
        lblNamHoc.Dock = DockStyle.Fill;
        lblNamHoc.Location = new Point(0, 152);
        lblNamHoc.Margin = new Padding(0, 0, 10, 0);
        lblNamHoc.Name = "lblNamHoc";
        lblNamHoc.Size = new Size(150, 48);
        lblNamHoc.TabIndex = 3;
        lblNamHoc.Text = "Năm học *";
        lblNamHoc.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtNamHoc
        // 
        txtNamHoc.Dock = DockStyle.Top;
        txtNamHoc.Location = new Point(160, 157);
        txtNamHoc.Margin = new Padding(0, 5, 0, 8);
        txtNamHoc.MaximumSize = new Size(200, 0);
        txtNamHoc.MaxLength = 9;
        txtNamHoc.Name = "txtNamHoc";
        txtNamHoc.Size = new Size(200, 32);
        txtNamHoc.TabIndex = 3;
        // 
        // lblGhiChu
        // 
        lblGhiChu.Dock = DockStyle.Fill;
        lblGhiChu.Location = new Point(0, 200);
        lblGhiChu.Margin = new Padding(0, 0, 10, 0);
        lblGhiChu.Name = "lblGhiChu";
        lblGhiChu.Padding = new Padding(0, 7, 0, 0);
        lblGhiChu.Size = new Size(150, 112);
        lblGhiChu.TabIndex = 4;
        lblGhiChu.Text = "Ghi chú";
        // 
        // txtGhiChu
        // 
        txtGhiChu.Dock = DockStyle.Fill;
        txtGhiChu.Location = new Point(160, 205);
        txtGhiChu.Margin = new Padding(0, 5, 0, 8);
        txtGhiChu.MaxLength = 255;
        txtGhiChu.Multiline = true;
        txtGhiChu.Name = "txtGhiChu";
        txtGhiChu.ScrollBars = ScrollBars.Vertical;
        txtGhiChu.Size = new Size(828, 99);
        txtGhiChu.TabIndex = 4;
        // 
        // pnlFooter
        // 
        pnlFooter.Controls.Add(lblMessage);
        pnlFooter.Controls.Add(flpActions);
        pnlFooter.Dock = DockStyle.Fill;
        pnlFooter.Location = new Point(20, 552);
        pnlFooter.Margin = new Padding(0);
        pnlFooter.Name = "pnlFooter";
        pnlFooter.Padding = new Padding(0, 12, 0, 0);
        pnlFooter.Size = new Size(1060, 56);
        pnlFooter.TabIndex = 3;
        // 
        // lblMessage
        // 
        lblMessage.AutoEllipsis = true;
        lblMessage.Dock = DockStyle.Fill;
        lblMessage.ForeColor = Color.FromArgb(100, 112, 128);
        lblMessage.Location = new Point(0, 12);
        lblMessage.Name = "lblMessage";
        lblMessage.Size = new Size(806, 44);
        lblMessage.TabIndex = 0;
        lblMessage.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // flpActions
        // 
        flpActions.AutoSize = true;
        flpActions.Controls.Add(btnClose);
        flpActions.Controls.Add(btnGuiDon);
        flpActions.Dock = DockStyle.Right;
        flpActions.FlowDirection = FlowDirection.RightToLeft;
        flpActions.Location = new Point(806, 12);
        flpActions.Name = "flpActions";
        flpActions.Size = new Size(254, 44);
        flpActions.TabIndex = 1;
        flpActions.WrapContents = false;
        // 
        // btnClose
        // 
        btnClose.FlatAppearance.BorderColor = Color.FromArgb(169, 175, 184);
        btnClose.FlatStyle = FlatStyle.Flat;
        btnClose.Location = new Point(170, 0);
        btnClose.Margin = new Padding(9, 0, 0, 0);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(84, 36);
        btnClose.TabIndex = 6;
        btnClose.Text = "Đóng";
        btnClose.Click += Close_Click;
        // 
        // btnGuiDon
        // 
        btnGuiDon.BackColor = Color.FromArgb(23, 101, 183);
        btnGuiDon.FlatAppearance.BorderColor = Color.FromArgb(17, 82, 151);
        btnGuiDon.FlatStyle = FlatStyle.Flat;
        btnGuiDon.ForeColor = Color.White;
        btnGuiDon.Location = new Point(9, 0);
        btnGuiDon.Margin = new Padding(9, 0, 0, 0);
        btnGuiDon.Name = "btnGuiDon";
        btnGuiDon.Size = new Size(152, 36);
        btnGuiDon.TabIndex = 5;
        btnGuiDon.Text = "Gửi đơn đăng ký";
        btnGuiDon.UseVisualStyleBackColor = false;
        // 
        // FrmDangKyKtx
        // 
        AutoScaleDimensions = new SizeF(120F, 120F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(246, 246, 250);
        ClientSize = new Size(1100, 620);
        Controls.Add(tlpRoot);
        Font = new Font("Segoe UI", 11F);
        Name = "FrmDangKyKtx";
        Text = "Đăng ký KTX";
        tlpRoot.ResumeLayout(false);
        grpRegistration.ResumeLayout(false);
        pnlFormScroll.ResumeLayout(false);
        pnlFormScroll.PerformLayout();
        tlpFields.ResumeLayout(false);
        tlpFields.PerformLayout();
        pnlFooter.ResumeLayout(false);
        pnlFooter.PerformLayout();
        flpActions.ResumeLayout(false);
        ResumeLayout(false);
    }
}