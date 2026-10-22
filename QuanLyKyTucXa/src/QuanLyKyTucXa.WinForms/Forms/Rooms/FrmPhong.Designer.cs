#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Rooms;

partial class FrmPhong
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel tlpRoom = null!;
    private Label lblTitle = null!, lblToa = null!, lblTrangThai = null!, lblRecordCount = null!;
    private GroupBox grpFilters = null!;
    private FlowLayoutPanel flpFilters = null!, flpActions = null!;
    private ComboBox cboToa = null!, cboTrangThai = null!;
    private TextBox txtMaPhong = null!;
    private Button btnTim = null!, btnThem = null!, btnSua = null!, btnXoa = null!, btnDong = null!;
    private DataGridView dgvPhong = null!;
    private DataGridViewTextBoxColumn MaPhong = null!, Toa = null!, Tang = null!, GiuongTrong = null!, TrangThai = null!;
    private Panel pnlActions = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        var dataGridViewCellStyle1 = new DataGridViewCellStyle();
        var dataGridViewCellStyle2 = new DataGridViewCellStyle();
        var dataGridViewCellStyle3 = new DataGridViewCellStyle();
        tlpRoom = new TableLayoutPanel();
        lblTitle = new Label();
        grpFilters = new GroupBox();
        flpFilters = new FlowLayoutPanel();
        lblToa = new Label();
        cboToa = new ComboBox();
        lblTrangThai = new Label();
        cboTrangThai = new ComboBox();
        txtMaPhong = new TextBox();
        btnTim = new Button();
        dgvPhong = new DataGridView();
        MaPhong = new DataGridViewTextBoxColumn();
        Toa = new DataGridViewTextBoxColumn();
        Tang = new DataGridViewTextBoxColumn();
        GiuongTrong = new DataGridViewTextBoxColumn();
        TrangThai = new DataGridViewTextBoxColumn();
        pnlActions = new Panel();
        flpActions = new FlowLayoutPanel();
        btnDong = new Button();
        btnXoa = new Button();
        btnSua = new Button();
        btnThem = new Button();
        lblRecordCount = new Label();
        toolTip1 = new ToolTip(components);
        tlpRoom.SuspendLayout();
        grpFilters.SuspendLayout();
        flpFilters.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPhong).BeginInit();
        pnlActions.SuspendLayout();
        flpActions.SuspendLayout();
        SuspendLayout();
        // 
        // tlpRoom
        // 
        tlpRoom.BackColor = Color.FromArgb(246, 246, 250);
        tlpRoom.ColumnCount = 1;
        tlpRoom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tlpRoom.Controls.Add(lblTitle, 0, 0);
        tlpRoom.Controls.Add(grpFilters, 0, 1);
        tlpRoom.Controls.Add(dgvPhong, 0, 2);
        tlpRoom.Controls.Add(pnlActions, 0, 3);
        tlpRoom.Dock = DockStyle.Fill;
        tlpRoom.Location = new Point(0, 0);
        tlpRoom.Name = "tlpRoom";
        tlpRoom.Padding = new Padding(20, 14, 20, 12);
        tlpRoom.RowCount = 4;
        tlpRoom.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        tlpRoom.RowStyles.Add(new RowStyle(SizeType.Absolute, 94F));
        tlpRoom.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpRoom.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        tlpRoom.Size = new Size(1100, 620);
        tlpRoom.TabIndex = 0;
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
        lblTitle.Text = "Quản lý phòng";
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
        flpFilters.Controls.Add(lblToa);
        flpFilters.Controls.Add(cboToa);
        flpFilters.Controls.Add(lblTrangThai);
        flpFilters.Controls.Add(cboTrangThai);
        flpFilters.Controls.Add(txtMaPhong);
        flpFilters.Controls.Add(btnTim);
        flpFilters.Dock = DockStyle.Fill;
        flpFilters.Location = new Point(14, 33);
        flpFilters.Name = "flpFilters";
        flpFilters.Padding = new Padding(0, 3, 0, 0);
        flpFilters.Size = new Size(1034, 37);
        flpFilters.TabIndex = 0;
        flpFilters.WrapContents = false;
        // 
        // lblToa
        // 
        lblToa.AutoSize = true;
        lblToa.Location = new Point(0, 3);
        lblToa.Margin = new Padding(0);
        lblToa.MinimumSize = new Size(44, 32);
        lblToa.Name = "lblToa";
        lblToa.Size = new Size(44, 32);
        lblToa.TabIndex = 0;
        lblToa.Text = "Tòa";
        lblToa.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // cboToa
        // 
        cboToa.DropDownStyle = ComboBoxStyle.DropDownList;
        cboToa.Items.AddRange(new object[] { "Tất cả", "A1", "A2", "B1", "B2" });
        cboToa.Location = new Point(44, 6);
        cboToa.Margin = new Padding(0, 3, 14, 0);
        cboToa.Name = "cboToa";
        cboToa.Size = new Size(160, 33);
        cboToa.TabIndex = 1;
        cboToa.SelectedIndexChanged += Filter_Changed;
        // 
        // lblTrangThai
        // 
        lblTrangThai.AutoSize = true;
        lblTrangThai.Location = new Point(218, 3);
        lblTrangThai.Margin = new Padding(0);
        lblTrangThai.MinimumSize = new Size(90, 32);
        lblTrangThai.Name = "lblTrangThai";
        lblTrangThai.Size = new Size(96, 32);
        lblTrangThai.TabIndex = 2;
        lblTrangThai.Text = "Trạng thái";
        lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // cboTrangThai
        // 
        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.Items.AddRange(new object[] { "Tất cả", "Còn chỗ", "Đã đầy", "Đang sửa chữa" });
        cboTrangThai.Location = new Point(314, 6);
        cboTrangThai.Margin = new Padding(0, 3, 14, 0);
        cboTrangThai.Name = "cboTrangThai";
        cboTrangThai.Size = new Size(160, 33);
        cboTrangThai.TabIndex = 3;
        cboTrangThai.SelectedIndexChanged += Filter_Changed;
        // 
        // txtMaPhong
        // 
        txtMaPhong.Location = new Point(488, 6);
        txtMaPhong.Margin = new Padding(0, 3, 12, 0);
        txtMaPhong.Name = "txtMaPhong";
        txtMaPhong.PlaceholderText = "Mã phòng";
        txtMaPhong.Size = new Size(200, 32);
        txtMaPhong.TabIndex = 4;
        toolTip1.SetToolTip(txtMaPhong, "Nhập mã phòng rồi nhấn Enter hoặc bấm Tìm.");
        txtMaPhong.KeyDown += Search_KeyDown;
        // 
        // btnTim
        // 
        btnTim.BackColor = Color.FromArgb(23, 101, 183);
        btnTim.FlatAppearance.BorderColor = Color.FromArgb(17, 82, 151);
        btnTim.FlatStyle = FlatStyle.Flat;
        btnTim.ForeColor = Color.White;
        btnTim.Location = new Point(700, 3);
        btnTim.Margin = new Padding(0);
        btnTim.Name = "btnTim";
        btnTim.Size = new Size(88, 34);
        btnTim.TabIndex = 5;
        btnTim.Text = "⌕  Tìm";
        btnTim.UseVisualStyleBackColor = false;
        btnTim.Click += Find_Click;
        // 
        // dgvPhong
        // 
        dgvPhong.AllowUserToAddRows = false;
        dgvPhong.AllowUserToDeleteRows = false;
        dgvPhong.AllowUserToResizeRows = false;
        dataGridViewCellStyle1.BackColor = Color.FromArgb(249, 249, 253);
        dgvPhong.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
        dgvPhong.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvPhong.BackgroundColor = Color.FromArgb(246, 246, 250);
        dgvPhong.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvPhong.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle2.BackColor = Color.FromArgb(231, 237, 248);
        dataGridViewCellStyle2.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        dataGridViewCellStyle2.ForeColor = Color.FromArgb(35, 40, 48);
        dataGridViewCellStyle2.Padding = new Padding(10, 0, 0, 0);
        dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(231, 237, 248);
        dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(35, 40, 48);
        dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
        dgvPhong.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
        dgvPhong.ColumnHeadersHeight = 42;
        dgvPhong.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvPhong.Columns.AddRange(new DataGridViewColumn[] { MaPhong, Toa, Tang, GiuongTrong, TrangThai });
        dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle3.BackColor = Color.White;
        dataGridViewCellStyle3.Font = new Font("Segoe UI", 11F);
        dataGridViewCellStyle3.ForeColor = Color.FromArgb(45, 51, 59);
        dataGridViewCellStyle3.Padding = new Padding(10, 0, 0, 0);
        dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(189, 221, 248);
        dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(35, 60, 80);
        dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
        dgvPhong.DefaultCellStyle = dataGridViewCellStyle3;
        dgvPhong.Dock = DockStyle.Fill;
        dgvPhong.EnableHeadersVisualStyles = false;
        dgvPhong.Font = new Font("Segoe UI", 11F);
        dgvPhong.GridColor = Color.FromArgb(216, 221, 230);
        dgvPhong.Location = new Point(20, 166);
        dgvPhong.Margin = new Padding(0);
        dgvPhong.MultiSelect = false;
        dgvPhong.Name = "dgvPhong";
        dgvPhong.ReadOnly = true;
        dgvPhong.RowHeadersVisible = false;
        dgvPhong.RowHeadersWidth = 51;
        dgvPhong.RowTemplate.Height = 44;
        dgvPhong.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvPhong.Size = new Size(1060, 386);
        dgvPhong.TabIndex = 2;
        dgvPhong.CellFormatting += FormatStatusCell;
        // 
        // MaPhong
        // 
        MaPhong.FillWeight = 22F;
        MaPhong.HeaderText = "Mã phòng";
        MaPhong.MinimumWidth = 6;
        MaPhong.Name = "MaPhong";
        MaPhong.ReadOnly = true;
        // 
        // Toa
        // 
        Toa.FillWeight = 11F;
        Toa.HeaderText = "Tòa";
        Toa.MinimumWidth = 6;
        Toa.Name = "Toa";
        Toa.ReadOnly = true;
        // 
        // Tang
        // 
        Tang.FillWeight = 13F;
        Tang.HeaderText = "Tầng";
        Tang.MinimumWidth = 6;
        Tang.Name = "Tang";
        Tang.ReadOnly = true;
        // 
        // GiuongTrong
        // 
        GiuongTrong.FillWeight = 27F;
        GiuongTrong.HeaderText = "Giường trống";
        GiuongTrong.MinimumWidth = 6;
        GiuongTrong.Name = "GiuongTrong";
        GiuongTrong.ReadOnly = true;
        // 
        // TrangThai
        // 
        TrangThai.FillWeight = 28F;
        TrangThai.HeaderText = "Trạng thái";
        TrangThai.MinimumWidth = 6;
        TrangThai.Name = "TrangThai";
        TrangThai.ReadOnly = true;
        // 
        // pnlActions
        // 
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
        btnXoa.TabIndex = 1;
        btnXoa.Text = "Xóa";
        btnXoa.Click += Delete_Click;
        // 
        // btnSua
        // 
        btnSua.FlatAppearance.BorderColor = Color.FromArgb(169, 175, 184);
        btnSua.FlatStyle = FlatStyle.Flat;
        btnSua.Location = new Point(122, 0);
        btnSua.Margin = new Padding(9, 0, 0, 0);
        btnSua.Name = "btnSua";
        btnSua.Size = new Size(90, 36);
        btnSua.TabIndex = 2;
        btnSua.Text = "✎  Sửa";
        btnSua.Click += Edit_Click;
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
        btnThem.TabIndex = 3;
        btnThem.Text = "＋  Thêm";
        btnThem.UseVisualStyleBackColor = false;
        btnThem.Click += Add_Click;
        // 
        // lblRecordCount
        // 
        lblRecordCount.Dock = DockStyle.Left;
        lblRecordCount.ForeColor = Color.FromArgb(100, 112, 128);
        lblRecordCount.Location = new Point(0, 12);
        lblRecordCount.Name = "lblRecordCount";
        lblRecordCount.Size = new Size(120, 44);
        lblRecordCount.TabIndex = 1;
        lblRecordCount.Text = "0 phòng";
        lblRecordCount.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // FrmPhong
        // 
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(246, 246, 250);
        ClientSize = new Size(1100, 620);
        Controls.Add(tlpRoom);
        Font = new Font("Segoe UI", 11F);
        Name = "FrmPhong";
        Text = "Quản lý phòng";
        tlpRoom.ResumeLayout(false);
        grpFilters.ResumeLayout(false);
        flpFilters.ResumeLayout(false);
        flpFilters.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPhong).EndInit();
        pnlActions.ResumeLayout(false);
        pnlActions.PerformLayout();
        flpActions.ResumeLayout(false);
        ResumeLayout(false);
    }

    private ToolTip toolTip1 = null!;
}


