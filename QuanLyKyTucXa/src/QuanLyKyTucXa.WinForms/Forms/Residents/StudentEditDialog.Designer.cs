#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Residents;

partial class StudentEditDialog
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel tlpRoot = null!;
    private TableLayoutPanel tlpFields = null!;
    private Panel pnlScroll = null!;
    private FlowLayoutPanel flpActions = null!;
    private Label lblError = null!;
    private Button btnLuu = null!;
    private Button btnHuy = null!;
    private TextBox txtMaSv = null!;
    private Label lblMaSv = null!;
    private TextBox txtHoTen = null!;
    private Label lblHoTen = null!;
    private DateTimePicker dtpNgaySinh = null!;
    private Label lblNgaySinh = null!;
    private ComboBox cboGioiTinh = null!;
    private Label lblGioiTinh = null!;
    private TextBox txtCccd = null!;
    private Label lblCccd = null!;
    private TextBox txtSoDienThoai = null!;
    private Label lblSoDienThoai = null!;
    private TextBox txtEmail = null!;
    private Label lblEmail = null!;
    private TextBox txtLop = null!;
    private Label lblLop = null!;
    private TextBox txtKhoa = null!;
    private Label lblKhoa = null!;
    private TextBox txtQueQuan = null!;
    private Label lblQueQuan = null!;
    private TextBox txtTrangThai = null!;
    private Label lblTrangThai = null!;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        tlpRoot = new TableLayoutPanel();
        pnlScroll = new Panel();
        tlpFields = new TableLayoutPanel();
        lblMaSv = new Label();
        txtMaSv = new TextBox();
        lblHoTen = new Label();
        txtHoTen = new TextBox();
        lblNgaySinh = new Label();
        dtpNgaySinh = new DateTimePicker();
        lblGioiTinh = new Label();
        cboGioiTinh = new ComboBox();
        lblCccd = new Label();
        txtCccd = new TextBox();
        lblSoDienThoai = new Label();
        txtSoDienThoai = new TextBox();
        lblEmail = new Label();
        txtEmail = new TextBox();
        lblLop = new Label();
        txtLop = new TextBox();
        lblKhoa = new Label();
        txtKhoa = new TextBox();
        lblQueQuan = new Label();
        txtQueQuan = new TextBox();
        lblTrangThai = new Label();
        txtTrangThai = new TextBox();
        lblError = new Label();
        flpActions = new FlowLayoutPanel();
        btnHuy = new Button();
        btnLuu = new Button();
        errorProvider1 = new ErrorProvider(components);
        tlpRoot.SuspendLayout();
        pnlScroll.SuspendLayout();
        tlpFields.SuspendLayout();
        flpActions.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
        SuspendLayout();
        // 
        // tlpRoot
        // 
        tlpRoot.ColumnCount = 1;
        tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tlpRoot.Controls.Add(pnlScroll, 0, 0);
        tlpRoot.Controls.Add(lblError, 0, 1);
        tlpRoot.Controls.Add(flpActions, 0, 2);
        tlpRoot.Dock = DockStyle.Fill;
        tlpRoot.Location = new Point(0, 0);
        tlpRoot.Name = "tlpRoot";
        tlpRoot.Padding = new Padding(18);
        tlpRoot.RowCount = 3;
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        tlpRoot.Size = new Size(560, 640);
        tlpRoot.TabIndex = 0;
        // 
        // pnlScroll
        // 
        pnlScroll.AutoScroll = true;
        pnlScroll.Controls.Add(tlpFields);
        pnlScroll.Dock = DockStyle.Fill;
        pnlScroll.Location = new Point(21, 21);
        pnlScroll.Name = "pnlScroll";
        pnlScroll.Size = new Size(518, 490);
        pnlScroll.TabIndex = 0;
        // 
        // tlpFields
        // 
        tlpFields.AutoSize = true;
        tlpFields.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tlpFields.ColumnCount = 2;
        tlpFields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        tlpFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tlpFields.Controls.Add(lblMaSv, 0, 0);
        tlpFields.Controls.Add(txtMaSv, 1, 0);
        tlpFields.Controls.Add(lblHoTen, 0, 1);
        tlpFields.Controls.Add(txtHoTen, 1, 1);
        tlpFields.Controls.Add(lblNgaySinh, 0, 2);
        tlpFields.Controls.Add(dtpNgaySinh, 1, 2);
        tlpFields.Controls.Add(lblGioiTinh, 0, 3);
        tlpFields.Controls.Add(cboGioiTinh, 1, 3);
        tlpFields.Controls.Add(lblCccd, 0, 4);
        tlpFields.Controls.Add(txtCccd, 1, 4);
        tlpFields.Controls.Add(lblSoDienThoai, 0, 5);
        tlpFields.Controls.Add(txtSoDienThoai, 1, 5);
        tlpFields.Controls.Add(lblEmail, 0, 6);
        tlpFields.Controls.Add(txtEmail, 1, 6);
        tlpFields.Controls.Add(lblLop, 0, 7);
        tlpFields.Controls.Add(txtLop, 1, 7);
        tlpFields.Controls.Add(lblKhoa, 0, 8);
        tlpFields.Controls.Add(txtKhoa, 1, 8);
        tlpFields.Controls.Add(lblQueQuan, 0, 9);
        tlpFields.Controls.Add(txtQueQuan, 1, 9);
        tlpFields.Controls.Add(lblTrangThai, 0, 10);
        tlpFields.Controls.Add(txtTrangThai, 1, 10);
        tlpFields.Dock = DockStyle.Top;
        tlpFields.Location = new Point(0, 0);
        tlpFields.Name = "tlpFields";
        tlpFields.Padding = new Padding(0, 0, 12, 0);
        tlpFields.RowCount = 11;
        tlpFields.RowStyles.Add(new RowStyle());
        tlpFields.RowStyles.Add(new RowStyle());
        tlpFields.RowStyles.Add(new RowStyle());
        tlpFields.RowStyles.Add(new RowStyle());
        tlpFields.RowStyles.Add(new RowStyle());
        tlpFields.RowStyles.Add(new RowStyle());
        tlpFields.RowStyles.Add(new RowStyle());
        tlpFields.RowStyles.Add(new RowStyle());
        tlpFields.RowStyles.Add(new RowStyle());
        tlpFields.RowStyles.Add(new RowStyle());
        tlpFields.RowStyles.Add(new RowStyle());
        tlpFields.Size = new Size(518, 474);
        tlpFields.TabIndex = 0;
        // 
        // lblMaSv
        // 
        lblMaSv.Anchor = AnchorStyles.Left;
        lblMaSv.AutoSize = true;
        lblMaSv.Location = new Point(0, 9);
        lblMaSv.Margin = new Padding(0, 8, 8, 10);
        lblMaSv.Name = "lblMaSv";
        lblMaSv.Size = new Size(118, 23);
        lblMaSv.TabIndex = 0;
        lblMaSv.Text = "Mã sinh viên *";
        // 
        // txtMaSv
        // 
        txtMaSv.Dock = DockStyle.Top;
        txtMaSv.Location = new Point(150, 5);
        txtMaSv.Margin = new Padding(0, 5, 0, 8);
        txtMaSv.MaxLength = 20;
        txtMaSv.Name = "txtMaSv";
        txtMaSv.Size = new Size(356, 30);
        txtMaSv.TabIndex = 0;
        // 
        // lblHoTen
        // 
        lblHoTen.Anchor = AnchorStyles.Left;
        lblHoTen.AutoSize = true;
        lblHoTen.Location = new Point(0, 52);
        lblHoTen.Margin = new Padding(0, 8, 8, 10);
        lblHoTen.Name = "lblHoTen";
        lblHoTen.Size = new Size(96, 23);
        lblHoTen.TabIndex = 1;
        lblHoTen.Text = "Họ và tên *";
        // 
        // txtHoTen
        // 
        txtHoTen.Dock = DockStyle.Top;
        txtHoTen.Location = new Point(150, 48);
        txtHoTen.Margin = new Padding(0, 5, 0, 8);
        txtHoTen.MaxLength = 100;
        txtHoTen.Name = "txtHoTen";
        txtHoTen.Size = new Size(356, 30);
        txtHoTen.TabIndex = 1;
        // 
        // lblNgaySinh
        // 
        lblNgaySinh.Anchor = AnchorStyles.Left;
        lblNgaySinh.AutoSize = true;
        lblNgaySinh.Location = new Point(0, 95);
        lblNgaySinh.Margin = new Padding(0, 8, 8, 10);
        lblNgaySinh.Name = "lblNgaySinh";
        lblNgaySinh.Size = new Size(98, 23);
        lblNgaySinh.TabIndex = 2;
        lblNgaySinh.Text = "Ngày sinh *";
        // 
        // dtpNgaySinh
        // 
        dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
        dtpNgaySinh.Dock = DockStyle.Top;
        dtpNgaySinh.Format = DateTimePickerFormat.Custom;
        dtpNgaySinh.Location = new Point(150, 91);
        dtpNgaySinh.Margin = new Padding(0, 5, 0, 8);
        dtpNgaySinh.Name = "dtpNgaySinh";
        dtpNgaySinh.Size = new Size(356, 30);
        dtpNgaySinh.TabIndex = 2;
        // 
        // lblGioiTinh
        // 
        lblGioiTinh.Anchor = AnchorStyles.Left;
        lblGioiTinh.AutoSize = true;
        lblGioiTinh.Location = new Point(0, 138);
        lblGioiTinh.Margin = new Padding(0, 8, 8, 10);
        lblGioiTinh.Name = "lblGioiTinh";
        lblGioiTinh.Size = new Size(87, 23);
        lblGioiTinh.TabIndex = 3;
        lblGioiTinh.Text = "Giới tính *";
        // 
        // cboGioiTinh
        // 
        cboGioiTinh.Dock = DockStyle.Top;
        cboGioiTinh.DropDownStyle = ComboBoxStyle.DropDownList;
        cboGioiTinh.Items.AddRange(new object[] { "Nam", "Nữ" });
        cboGioiTinh.Location = new Point(150, 134);
        cboGioiTinh.Margin = new Padding(0, 5, 0, 8);
        cboGioiTinh.Name = "cboGioiTinh";
        cboGioiTinh.Size = new Size(356, 31);
        cboGioiTinh.TabIndex = 3;
        // 
        // lblCccd
        // 
        lblCccd.Anchor = AnchorStyles.Left;
        lblCccd.AutoSize = true;
        lblCccd.Location = new Point(0, 182);
        lblCccd.Margin = new Padding(0, 8, 8, 10);
        lblCccd.Name = "lblCccd";
        lblCccd.Size = new Size(67, 23);
        lblCccd.TabIndex = 4;
        lblCccd.Text = "CCCD *";
        // 
        // txtCccd
        // 
        txtCccd.Dock = DockStyle.Top;
        txtCccd.Location = new Point(150, 178);
        txtCccd.Margin = new Padding(0, 5, 0, 8);
        txtCccd.MaxLength = 12;
        txtCccd.Name = "txtCccd";
        txtCccd.Size = new Size(356, 30);
        txtCccd.TabIndex = 4;
        // 
        // lblSoDienThoai
        // 
        lblSoDienThoai.Anchor = AnchorStyles.Left;
        lblSoDienThoai.AutoSize = true;
        lblSoDienThoai.Location = new Point(0, 225);
        lblSoDienThoai.Margin = new Padding(0, 8, 8, 10);
        lblSoDienThoai.Name = "lblSoDienThoai";
        lblSoDienThoai.Size = new Size(123, 23);
        lblSoDienThoai.TabIndex = 5;
        lblSoDienThoai.Text = "Số điện thoại *";
        // 
        // txtSoDienThoai
        // 
        txtSoDienThoai.Dock = DockStyle.Top;
        txtSoDienThoai.Location = new Point(150, 221);
        txtSoDienThoai.Margin = new Padding(0, 5, 0, 8);
        txtSoDienThoai.MaxLength = 10;
        txtSoDienThoai.Name = "txtSoDienThoai";
        txtSoDienThoai.Size = new Size(356, 30);
        txtSoDienThoai.TabIndex = 5;
        // 
        // lblEmail
        // 
        lblEmail.Anchor = AnchorStyles.Left;
        lblEmail.AutoSize = true;
        lblEmail.Location = new Point(0, 268);
        lblEmail.Margin = new Padding(0, 8, 8, 10);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(51, 23);
        lblEmail.TabIndex = 6;
        lblEmail.Text = "Email";
        // 
        // txtEmail
        // 
        txtEmail.Dock = DockStyle.Top;
        txtEmail.Location = new Point(150, 264);
        txtEmail.Margin = new Padding(0, 5, 0, 8);
        txtEmail.MaxLength = 100;
        txtEmail.Name = "txtEmail";
        txtEmail.Size = new Size(356, 30);
        txtEmail.TabIndex = 6;
        // 
        // lblLop
        // 
        lblLop.Anchor = AnchorStyles.Left;
        lblLop.AutoSize = true;
        lblLop.Location = new Point(0, 311);
        lblLop.Margin = new Padding(0, 8, 8, 10);
        lblLop.Name = "lblLop";
        lblLop.Size = new Size(50, 23);
        lblLop.TabIndex = 7;
        lblLop.Text = "Lớp *";
        // 
        // txtLop
        // 
        txtLop.Dock = DockStyle.Top;
        txtLop.Location = new Point(150, 307);
        txtLop.Margin = new Padding(0, 5, 0, 8);
        txtLop.MaxLength = 50;
        txtLop.Name = "txtLop";
        txtLop.Size = new Size(356, 30);
        txtLop.TabIndex = 7;
        // 
        // lblKhoa
        // 
        lblKhoa.Anchor = AnchorStyles.Left;
        lblKhoa.AutoSize = true;
        lblKhoa.Location = new Point(0, 354);
        lblKhoa.Margin = new Padding(0, 8, 8, 10);
        lblKhoa.Name = "lblKhoa";
        lblKhoa.Size = new Size(61, 23);
        lblKhoa.TabIndex = 8;
        lblKhoa.Text = "Khoa *";
        // 
        // txtKhoa
        // 
        txtKhoa.Dock = DockStyle.Top;
        txtKhoa.Location = new Point(150, 350);
        txtKhoa.Margin = new Padding(0, 5, 0, 8);
        txtKhoa.MaxLength = 100;
        txtKhoa.Name = "txtKhoa";
        txtKhoa.Size = new Size(356, 30);
        txtKhoa.TabIndex = 8;
        // 
        // lblQueQuan
        // 
        lblQueQuan.Anchor = AnchorStyles.Left;
        lblQueQuan.AutoSize = true;
        lblQueQuan.Location = new Point(0, 397);
        lblQueQuan.Margin = new Padding(0, 8, 8, 10);
        lblQueQuan.Name = "lblQueQuan";
        lblQueQuan.Size = new Size(86, 23);
        lblQueQuan.TabIndex = 9;
        lblQueQuan.Text = "Quê quán";
        // 
        // txtQueQuan
        // 
        txtQueQuan.Dock = DockStyle.Top;
        txtQueQuan.Location = new Point(150, 393);
        txtQueQuan.Margin = new Padding(0, 5, 0, 8);
        txtQueQuan.MaxLength = 200;
        txtQueQuan.Name = "txtQueQuan";
        txtQueQuan.Size = new Size(356, 30);
        txtQueQuan.TabIndex = 9;
        // 
        // lblTrangThai
        // 
        lblTrangThai.Anchor = AnchorStyles.Left;
        lblTrangThai.AutoSize = true;
        lblTrangThai.Location = new Point(0, 440);
        lblTrangThai.Margin = new Padding(0, 8, 8, 10);
        lblTrangThai.Name = "lblTrangThai";
        lblTrangThai.Size = new Size(87, 23);
        lblTrangThai.TabIndex = 10;
        lblTrangThai.Text = "Trạng thái";
        // 
        // txtTrangThai
        // 
        txtTrangThai.Dock = DockStyle.Top;
        txtTrangThai.Location = new Point(150, 436);
        txtTrangThai.Margin = new Padding(0, 5, 0, 8);
        txtTrangThai.MaxLength = 30;
        txtTrangThai.Name = "txtTrangThai";
        txtTrangThai.ReadOnly = true;
        txtTrangThai.Size = new Size(356, 30);
        txtTrangThai.TabIndex = 10;
        txtTrangThai.TabStop = false;
        txtTrangThai.Text = "Chờ xếp phòng";
        // 
        // lblError
        // 
        lblError.AutoEllipsis = true;
        lblError.Dock = DockStyle.Fill;
        lblError.ForeColor = Color.Firebrick;
        lblError.Location = new Point(21, 514);
        lblError.Name = "lblError";
        lblError.Size = new Size(518, 64);
        lblError.TabIndex = 1;
        lblError.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // flpActions
        // 
        flpActions.Controls.Add(btnHuy);
        flpActions.Controls.Add(btnLuu);
        flpActions.Dock = DockStyle.Fill;
        flpActions.FlowDirection = FlowDirection.RightToLeft;
        flpActions.Location = new Point(21, 581);
        flpActions.Name = "flpActions";
        flpActions.Size = new Size(518, 38);
        flpActions.TabIndex = 2;
        flpActions.WrapContents = false;
        // 
        // btnHuy
        // 
        btnHuy.DialogResult = DialogResult.Cancel;
        btnHuy.Location = new Point(423, 3);
        btnHuy.Name = "btnHuy";
        btnHuy.Size = new Size(92, 36);
        btnHuy.TabIndex = 1;
        btnHuy.Text = "Hủy";
        // 
        // btnLuu
        // 
        btnLuu.BackColor = Color.FromArgb(23, 101, 183);
        btnLuu.FlatStyle = FlatStyle.Flat;
        btnLuu.ForeColor = Color.White;
        btnLuu.Location = new Point(325, 3);
        btnLuu.Name = "btnLuu";
        btnLuu.Size = new Size(92, 36);
        btnLuu.TabIndex = 0;
        btnLuu.Text = "Lưu";
        btnLuu.UseVisualStyleBackColor = false;
        btnLuu.Click += Save_Click;
        // 
        // errorProvider1
        // 
        errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        errorProvider1.ContainerControl = this;
        // 
        // StudentEditDialog
        // 
        AcceptButton = btnLuu;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(246, 246, 250);
        CancelButton = btnHuy;
        ClientSize = new Size(560, 640);
        Controls.Add(tlpRoot);
        Font = new Font("Segoe UI", 10F);
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(460, 380);
        Name = "StudentEditDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thêm / Sửa sinh viên";
        FormClosing += Editor_FormClosing;
        tlpRoot.ResumeLayout(false);
        pnlScroll.ResumeLayout(false);
        pnlScroll.PerformLayout();
        tlpFields.ResumeLayout(false);
        tlpFields.PerformLayout();
        flpActions.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
        ResumeLayout(false);
    }

    private ErrorProvider errorProvider1;
}


