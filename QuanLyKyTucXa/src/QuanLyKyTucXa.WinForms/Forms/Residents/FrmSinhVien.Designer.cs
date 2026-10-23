#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Residents;

partial class FrmSinhVien
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel tlpRoot = null!;
    private Label lblTitle = null!, lblKhoa = null!, lblLop = null!, lblRecordCount = null!;
    private GroupBox grpFilters = null!;
    private FlowLayoutPanel flpFilters = null!, flpActions = null!;
    private Panel pnlActions = null!;
    private ComboBox cboKhoa = null!, cboLop = null!;
    private TextBox txtTimKiem = null!;
    private Button btnTimKiem = null!;
    private Button btnXuatExcel = null!;
    private Button btnNhapExcel = null!;
    private Button btnTaiMau = null!;
    private ProgressBar prgExcel = null!;
    private Button btnThem = null!;
    private Button btnSua = null!;
    private Button btnXoa = null!;
    private Button btnDong = null!;
    private DataGridView dgvSinhVien = null!;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1 = null!;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2 = null!;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3 = null!;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4 = null!;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5 = null!;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6 = null!;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7 = null!;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8 = null!;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9 = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var dataGridViewCellStyle4 = new DataGridViewCellStyle();
        var dataGridViewCellStyle5 = new DataGridViewCellStyle();
        var dataGridViewCellStyle6 = new DataGridViewCellStyle();
        tlpRoot = new TableLayoutPanel();
        lblTitle = new Label();
        grpFilters = new GroupBox();
        flpFilters = new FlowLayoutPanel();
        lblKhoa = new Label();
        cboKhoa = new ComboBox();
        lblLop = new Label();
        cboLop = new ComboBox();
        txtTimKiem = new TextBox();
        btnTimKiem = new Button();
        btnXuatExcel = new Button();
        btnNhapExcel = new Button();
        btnTaiMau = new Button();
        dgvSinhVien = new DataGridView();
        pnlActions = new Panel();
        flpActions = new FlowLayoutPanel();
        btnDong = new Button();
        btnXoa = new Button();
        btnSua = new Button();
        btnThem = new Button();
        lblRecordCount = new Label();
        prgExcel = new ProgressBar();
        tlpRoot.SuspendLayout();
        grpFilters.SuspendLayout();
        flpFilters.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvSinhVien).BeginInit();
        pnlActions.SuspendLayout();
        flpActions.SuspendLayout();
        SuspendLayout();
        // 
        // tlpRoot
        // 
        tlpRoot.BackColor = Color.FromArgb(246, 246, 250);
        tlpRoot.ColumnCount = 1;
        tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tlpRoot.Controls.Add(lblTitle, 0, 0);
        tlpRoot.Controls.Add(grpFilters, 0, 1);
        tlpRoot.Controls.Add(dgvSinhVien, 0, 2);
        tlpRoot.Controls.Add(pnlActions, 0, 3);
        tlpRoot.Dock = DockStyle.Fill;
        tlpRoot.Location = new Point(0, 0);
        tlpRoot.Name = "tlpRoot";
        tlpRoot.Padding = new Padding(20, 14, 20, 12);
        tlpRoot.RowCount = 4;
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 94F));
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
        lblTitle.Text = "Quản lý sinh viên";
        lblTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // grpFilters
        // 
        grpFilters.Controls.Add(flpFilters);
        grpFilters.Dock = DockStyle.Fill;
        grpFilters.Font = new Font("Segoe UI", 11F);
        grpFilters.Location = new Point(20, 72);
        grpFilters.Margin = new Padding(0, 0, 0, 14);
        grpFilters.Name = "grpFilters";
        grpFilters.Padding = new Padding(14, 8, 12, 10);
        grpFilters.Size = new Size(1060, 80);
        grpFilters.TabIndex = 1;
        grpFilters.TabStop = false;
        grpFilters.Text = "Bộ lọc";
        // 
        // flpFilters
        // 
        flpFilters.AutoScroll = true;
        flpFilters.Controls.Add(lblKhoa);
        flpFilters.Controls.Add(cboKhoa);
        flpFilters.Controls.Add(lblLop);
        flpFilters.Controls.Add(cboLop);
        flpFilters.Controls.Add(txtTimKiem);
        flpFilters.Controls.Add(btnTimKiem);
        flpFilters.Controls.Add(btnXuatExcel);
        flpFilters.Controls.Add(btnNhapExcel);
        flpFilters.Controls.Add(btnTaiMau);
        flpFilters.Dock = DockStyle.Fill;
        flpFilters.Location = new Point(14, 33);
        flpFilters.Name = "flpFilters";
        flpFilters.Padding = new Padding(0, 3, 0, 0);
        flpFilters.Size = new Size(1034, 37);
        flpFilters.TabIndex = 0;
        flpFilters.WrapContents = false;
        // 
        // lblKhoa
        // 
        lblKhoa.AutoSize = true;
        lblKhoa.Location = new Point(0, 3);
        lblKhoa.Margin = new Padding(0);
        lblKhoa.MinimumSize = new Size(60, 32);
        lblKhoa.Name = "lblKhoa";
        lblKhoa.Size = new Size(60, 32);
        lblKhoa.TabIndex = 0;
        lblKhoa.Text = "Khoa";
        lblKhoa.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // cboKhoa
        // 
        cboKhoa.DropDownStyle = ComboBoxStyle.DropDownList;
        cboKhoa.DropDownWidth = 240;
        cboKhoa.Items.AddRange(new object[] { "Tất cả khoa" });
        cboKhoa.Location = new Point(60, 6);
        cboKhoa.Margin = new Padding(0, 3, 12, 0);
        cboKhoa.Name = "cboKhoa";
        cboKhoa.Size = new Size(160, 33);
        cboKhoa.TabIndex = 1;
        // 
        // lblLop
        // 
        lblLop.AutoSize = true;
        lblLop.Location = new Point(232, 3);
        lblLop.Margin = new Padding(0);
        lblLop.MinimumSize = new Size(50, 32);
        lblLop.Name = "lblLop";
        lblLop.Size = new Size(50, 32);
        lblLop.TabIndex = 2;
        lblLop.Text = "Lớp";
        lblLop.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // cboLop
        // 
        cboLop.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLop.Items.AddRange(new object[] { "Tất cả lớp" });
        cboLop.Location = new Point(282, 6);
        cboLop.Margin = new Padding(0, 3, 12, 0);
        cboLop.Name = "cboLop";
        cboLop.Size = new Size(120, 33);
        cboLop.TabIndex = 3;
        // 
        // txtTimKiem
        // 
        txtTimKiem.Location = new Point(414, 6);
        txtTimKiem.Margin = new Padding(0, 3, 12, 0);
        txtTimKiem.Name = "txtTimKiem";
        txtTimKiem.PlaceholderText = "Mã SV, họ tên, CCCD, SĐT";
        txtTimKiem.Size = new Size(200, 32);
        txtTimKiem.TabIndex = 4;
        // 
        // btnTimKiem
        // 
        btnTimKiem.BackColor = Color.FromArgb(23, 101, 183);
        btnTimKiem.FlatAppearance.BorderColor = Color.FromArgb(17, 82, 151);
        btnTimKiem.FlatStyle = FlatStyle.Flat;
        btnTimKiem.ForeColor = Color.White;
        btnTimKiem.Location = new Point(626, 3);
        btnTimKiem.Margin = new Padding(0, 0, 8, 0);
        btnTimKiem.Name = "btnTimKiem";
        btnTimKiem.Size = new Size(88, 34);
        btnTimKiem.TabIndex = 5;
        btnTimKiem.Text = "⌕  Tìm";
        btnTimKiem.UseVisualStyleBackColor = false;
        // 
        // btnXuatExcel
        // 
        btnXuatExcel.FlatAppearance.BorderColor = Color.FromArgb(169, 175, 184);
        btnXuatExcel.FlatStyle = FlatStyle.Flat;
        btnXuatExcel.Location = new Point(722, 3);
        btnXuatExcel.Margin = new Padding(0);
        btnXuatExcel.Name = "btnXuatExcel";
        btnXuatExcel.Size = new Size(96, 34);
        btnXuatExcel.TabIndex = 6;
        btnXuatExcel.Text = "Xuất Excel";
        // 
        // btnNhapExcel
        // 
        btnNhapExcel.FlatStyle = FlatStyle.Flat;
        btnNhapExcel.Location = new Point(826, 3);
        btnNhapExcel.Margin = new Padding(8, 0, 0, 0);
        btnNhapExcel.Name = "btnNhapExcel";
        btnNhapExcel.Size = new Size(104, 34);
        btnNhapExcel.TabIndex = 7;
        btnNhapExcel.Text = "Nhập Excel";
        btnNhapExcel.Click += ImportStudents_Click;
        // 
        // btnTaiMau
        // 
        btnTaiMau.FlatStyle = FlatStyle.Flat;
        btnTaiMau.Location = new Point(938, 3);
        btnTaiMau.Margin = new Padding(8, 0, 0, 0);
        btnTaiMau.Name = "btnTaiMau";
        btnTaiMau.Size = new Size(112, 34);
        btnTaiMau.TabIndex = 8;
        btnTaiMau.Text = "Tải mẫu Excel";
        // 
        // dgvSinhVien
        // 
        dgvSinhVien.AllowUserToAddRows = false;
        dgvSinhVien.AllowUserToDeleteRows = false;
        dgvSinhVien.AllowUserToResizeRows = false;
        dataGridViewCellStyle4.BackColor = Color.FromArgb(249, 249, 253);
        dgvSinhVien.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
        dgvSinhVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvSinhVien.BackgroundColor = Color.FromArgb(246, 246, 250);
        dgvSinhVien.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvSinhVien.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle5.BackColor = Color.FromArgb(231, 237, 248);
        dataGridViewCellStyle5.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        dataGridViewCellStyle5.ForeColor = Color.FromArgb(35, 40, 48);
        dataGridViewCellStyle5.Padding = new Padding(10, 0, 0, 0);
        dataGridViewCellStyle5.SelectionBackColor = Color.FromArgb(231, 237, 248);
        dataGridViewCellStyle5.SelectionForeColor = Color.FromArgb(35, 40, 48);
        dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
        dgvSinhVien.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
        dgvSinhVien.ColumnHeadersHeight = 42;
        dgvSinhVien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle6.BackColor = Color.White;
        dataGridViewCellStyle6.Font = new Font("Segoe UI", 11F);
        dataGridViewCellStyle6.ForeColor = Color.FromArgb(45, 51, 59);
        dataGridViewCellStyle6.Padding = new Padding(10, 0, 0, 0);
        dataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(189, 221, 248);
        dataGridViewCellStyle6.SelectionForeColor = Color.FromArgb(35, 60, 80);
        dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
        dgvSinhVien.DefaultCellStyle = dataGridViewCellStyle6;
        dgvSinhVien.Dock = DockStyle.Fill;
        dgvSinhVien.EnableHeadersVisualStyles = false;
        dgvSinhVien.Font = new Font("Segoe UI", 11F);
        dgvSinhVien.GridColor = Color.FromArgb(216, 221, 230);
        dgvSinhVien.Location = new Point(20, 166);
        dgvSinhVien.Margin = new Padding(0);
        dgvSinhVien.MultiSelect = false;
        dgvSinhVien.Name = "dgvSinhVien";
        dgvSinhVien.ReadOnly = true;
        dgvSinhVien.RowHeadersVisible = false;
        dgvSinhVien.RowHeadersWidth = 51;
        dgvSinhVien.RowTemplate.Height = 44;
        dgvSinhVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSinhVien.Size = new Size(1060, 386);
        dgvSinhVien.TabIndex = 2;
        dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1"; dataGridViewTextBoxColumn1.DataPropertyName = "MaSv"; dataGridViewTextBoxColumn1.HeaderText = "Mã SV"; dataGridViewTextBoxColumn1.FillWeight = 11F; dataGridViewTextBoxColumn1.MinimumWidth = 95; dataGridViewTextBoxColumn1.SortMode = DataGridViewColumnSortMode.NotSortable;
        dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2"; dataGridViewTextBoxColumn2.DataPropertyName = "HoTen"; dataGridViewTextBoxColumn2.HeaderText = "Họ và tên"; dataGridViewTextBoxColumn2.FillWeight = 17F; dataGridViewTextBoxColumn2.MinimumWidth = 145; dataGridViewTextBoxColumn2.SortMode = DataGridViewColumnSortMode.NotSortable;
        dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3"; dataGridViewTextBoxColumn3.DataPropertyName = "NgaySinh"; dataGridViewTextBoxColumn3.HeaderText = "Ngày sinh"; dataGridViewTextBoxColumn3.FillWeight = 11F; dataGridViewTextBoxColumn3.MinimumWidth = 100; dataGridViewTextBoxColumn3.SortMode = DataGridViewColumnSortMode.NotSortable;
        dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4"; dataGridViewTextBoxColumn4.DataPropertyName = "GioiTinh"; dataGridViewTextBoxColumn4.HeaderText = "Giới tính"; dataGridViewTextBoxColumn4.FillWeight = 8F; dataGridViewTextBoxColumn4.MinimumWidth = 100; dataGridViewTextBoxColumn4.SortMode = DataGridViewColumnSortMode.NotSortable;
        dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5"; dataGridViewTextBoxColumn5.DataPropertyName = "Cccd"; dataGridViewTextBoxColumn5.HeaderText = "CCCD"; dataGridViewTextBoxColumn5.FillWeight = 13F; dataGridViewTextBoxColumn5.MinimumWidth = 125; dataGridViewTextBoxColumn5.SortMode = DataGridViewColumnSortMode.NotSortable;
        dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6"; dataGridViewTextBoxColumn6.DataPropertyName = "SoDienThoai"; dataGridViewTextBoxColumn6.HeaderText = "Số điện thoại"; dataGridViewTextBoxColumn6.FillWeight = 12F; dataGridViewTextBoxColumn6.MinimumWidth = 120; dataGridViewTextBoxColumn6.SortMode = DataGridViewColumnSortMode.NotSortable;
        dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7"; dataGridViewTextBoxColumn7.DataPropertyName = "Lop"; dataGridViewTextBoxColumn7.HeaderText = "Lớp"; dataGridViewTextBoxColumn7.FillWeight = 11F; dataGridViewTextBoxColumn7.MinimumWidth = 105; dataGridViewTextBoxColumn7.SortMode = DataGridViewColumnSortMode.NotSortable;
        dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8"; dataGridViewTextBoxColumn8.DataPropertyName = "Khoa"; dataGridViewTextBoxColumn8.HeaderText = "Khoa"; dataGridViewTextBoxColumn8.FillWeight = 17F; dataGridViewTextBoxColumn8.MinimumWidth = 155; dataGridViewTextBoxColumn8.SortMode = DataGridViewColumnSortMode.NotSortable;
        dataGridViewTextBoxColumn9 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9"; dataGridViewTextBoxColumn9.DataPropertyName = "TrangThai"; dataGridViewTextBoxColumn9.HeaderText = "Trạng thái"; dataGridViewTextBoxColumn9.FillWeight = 13F; dataGridViewTextBoxColumn9.MinimumWidth = 125; dataGridViewTextBoxColumn9.SortMode = DataGridViewColumnSortMode.NotSortable;
        dgvSinhVien.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5, dataGridViewTextBoxColumn6, dataGridViewTextBoxColumn7, dataGridViewTextBoxColumn8, dataGridViewTextBoxColumn9 });
        dgvSinhVien.CellFormatting += FormatStudentStatusCell;
        // 
        // pnlActions
        // 
        pnlActions.Controls.Add(prgExcel);
        pnlActions.Controls.Add(flpActions);
        pnlActions.Controls.Add(lblRecordCount);
        pnlActions.Dock = DockStyle.Fill;
        pnlActions.Location = new Point(20, 552);
        pnlActions.Margin = new Padding(0);
        pnlActions.Name = "pnlActions";
        pnlActions.Padding = new Padding(0, 12, 0, 0);
        pnlActions.Size = new Size(1060, 56);
        pnlActions.TabIndex = 3;
        // 
        // flpActions
        // 
        flpActions.AutoSize = true;
        flpActions.Controls.Add(btnDong);
        flpActions.Controls.Add(btnXoa);
        flpActions.Controls.Add(btnSua);
        flpActions.Controls.Add(btnThem);
        flpActions.Dock = DockStyle.Right;
        flpActions.FlowDirection = FlowDirection.RightToLeft;
        flpActions.Location = new Point(656, 12);
        flpActions.Name = "flpActions";
        flpActions.Size = new Size(404, 44);
        flpActions.TabIndex = 0;
        flpActions.WrapContents = false;
        // 
        // btnDong
        // 
        btnDong.FlatAppearance.BorderColor = Color.FromArgb(169, 175, 184);
        btnDong.FlatStyle = FlatStyle.Flat;
        btnDong.Location = new Point(320, 0);
        btnDong.Margin = new Padding(9, 0, 0, 0);
        btnDong.Name = "btnDong";
        btnDong.Size = new Size(84, 36);
        btnDong.TabIndex = 0;
        btnDong.Text = "Đóng";
        btnDong.Click += Close_Click;
        // 
        // btnXoa
        // 
        btnXoa.FlatAppearance.BorderColor = Color.FromArgb(163, 51, 60);
        btnXoa.FlatStyle = FlatStyle.Flat;
        btnXoa.ForeColor = Color.FromArgb(163, 51, 60);
        btnXoa.Location = new Point(221, 0);
        btnXoa.Margin = new Padding(9, 0, 0, 0);
        btnXoa.Name = "btnXoa";
        btnXoa.Size = new Size(90, 36);
        btnXoa.TabIndex = 2;
        btnXoa.Text = "Xóa";
        // 
        // btnSua
        // 
        btnSua.FlatAppearance.BorderColor = Color.FromArgb(169, 175, 184);
        btnSua.FlatStyle = FlatStyle.Flat;
        btnSua.Location = new Point(122, 0);
        btnSua.Margin = new Padding(9, 0, 0, 0);
        btnSua.Name = "btnSua";
        btnSua.Size = new Size(90, 36);
        btnSua.TabIndex = 3;
        btnSua.Text = "✎  Sửa";
        // 
        // btnThem
        // 
        btnThem.BackColor = Color.FromArgb(23, 101, 183);
        btnThem.FlatAppearance.BorderColor = Color.FromArgb(17, 82, 151);
        btnThem.FlatStyle = FlatStyle.Flat;
        btnThem.ForeColor = Color.White;
        btnThem.Location = new Point(9, 0);
        btnThem.Margin = new Padding(9, 0, 0, 0);
        btnThem.Name = "btnThem";
        btnThem.Size = new Size(104, 36);
        btnThem.TabIndex = 4;
        btnThem.Text = "＋  Thêm";
        btnThem.UseVisualStyleBackColor = false;
        // 
        // lblRecordCount
        // 
        lblRecordCount.Dock = DockStyle.Left;
        lblRecordCount.ForeColor = Color.FromArgb(100, 112, 128);
        lblRecordCount.Location = new Point(0, 12);
        lblRecordCount.Name = "lblRecordCount";
        lblRecordCount.Size = new Size(160, 44);
        lblRecordCount.TabIndex = 1;
        lblRecordCount.Text = "Đang tải...";
        lblRecordCount.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // prgExcel
        // 
        prgExcel.Dock = DockStyle.Fill;
        prgExcel.Location = new Point(160, 12);
        prgExcel.MarqueeAnimationSpeed = 25;
        prgExcel.Name = "prgExcel";
        prgExcel.Size = new Size(496, 44);
        prgExcel.TabIndex = 2;
        prgExcel.Visible = false;
        // 
        // FrmSinhVien
        // 
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(246, 246, 250);
        ClientSize = new Size(1100, 620);
        Controls.Add(tlpRoot);
        Font = new Font("Segoe UI", 11F);
        Name = "FrmSinhVien";
        Text = "Quản lý sinh viên";
        tlpRoot.ResumeLayout(false);
        grpFilters.ResumeLayout(false);
        flpFilters.ResumeLayout(false);
        flpFilters.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvSinhVien).EndInit();
        pnlActions.ResumeLayout(false);
        pnlActions.PerformLayout();
        flpActions.ResumeLayout(false);
        ResumeLayout(false);
    }
}



