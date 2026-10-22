#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Rooms;

partial class RoomEditDialog
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel tlpRoot = null!;
    private TableLayoutPanel tlpFields = null!;
    private Panel pnlScroll = null!;
    private FlowLayoutPanel flpActions = null!;
    private Label lblError = null!;
    private Label lblHint = null!;
    private Button btnLuu = null!;
    private Button btnHuy = null!;
    private Label lblMaPhong = null!;
    private TextBox txtMaPhong = null!;
    private Label lblTenPhong = null!;
    private TextBox txtTenPhong = null!;
    private Label lblToa = null!;
    private ComboBox cboToa = null!;
    private Label lblTang = null!;
    private NumericUpDown nudTang = null!;
    private Label lblLoaiPhong = null!;
    private TextBox txtLoaiPhong = null!;
    private Label lblSucChua = null!;
    private NumericUpDown nudSucChua = null!;
    private Label lblGiaPhong = null!;
    private NumericUpDown nudGiaPhong = null!;
    private Label lblBaoTri = null!;
    private CheckBox chkSuaChua = null!;
    private Label lblGhiChu = null!;
    private TextBox txtGhiChu = null!;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        tlpRoot = new TableLayoutPanel();
        pnlScroll = new Panel();
        tlpFields = new TableLayoutPanel();
        lblMaPhong = new Label();
        txtMaPhong = new TextBox();
        lblTenPhong = new Label();
        txtTenPhong = new TextBox();
        lblToa = new Label();
        cboToa = new ComboBox();
        lblTang = new Label();
        nudTang = new NumericUpDown();
        lblLoaiPhong = new Label();
        txtLoaiPhong = new TextBox();
        lblSucChua = new Label();
        nudSucChua = new NumericUpDown();
        lblGiaPhong = new Label();
        nudGiaPhong = new NumericUpDown();
        lblBaoTri = new Label();
        chkSuaChua = new CheckBox();
        lblGhiChu = new Label();
        txtGhiChu = new TextBox();
        lblHint = new Label();
        lblError = new Label();
        flpActions = new FlowLayoutPanel();
        btnHuy = new Button();
        btnLuu = new Button();
        tlpRoot.SuspendLayout();
        pnlScroll.SuspendLayout();
        tlpFields.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudTang).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudSucChua).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudGiaPhong).BeginInit();
        flpActions.SuspendLayout();
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
        tlpRoot.Size = new Size(560, 610);
        tlpRoot.TabIndex = 0;
        // 
        // pnlScroll
        // 
        pnlScroll.AutoScroll = true;
        pnlScroll.Controls.Add(tlpFields);
        pnlScroll.Dock = DockStyle.Fill;
        pnlScroll.Location = new Point(21, 21);
        pnlScroll.Name = "pnlScroll";
        pnlScroll.Size = new Size(518, 460);
        pnlScroll.TabIndex = 0;
        // 
        // tlpFields
        // 
        tlpFields.AutoSize = true;
        tlpFields.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tlpFields.ColumnCount = 2;
        tlpFields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        tlpFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tlpFields.Controls.Add(lblMaPhong, 0, 0);
        tlpFields.Controls.Add(txtMaPhong, 1, 0);
        tlpFields.Controls.Add(lblTenPhong, 0, 1);
        tlpFields.Controls.Add(txtTenPhong, 1, 1);
        tlpFields.Controls.Add(lblToa, 0, 2);
        tlpFields.Controls.Add(cboToa, 1, 2);
        tlpFields.Controls.Add(lblTang, 0, 3);
        tlpFields.Controls.Add(nudTang, 1, 3);
        tlpFields.Controls.Add(lblLoaiPhong, 0, 4);
        tlpFields.Controls.Add(txtLoaiPhong, 1, 4);
        tlpFields.Controls.Add(lblSucChua, 0, 5);
        tlpFields.Controls.Add(nudSucChua, 1, 5);
        tlpFields.Controls.Add(lblGiaPhong, 0, 6);
        tlpFields.Controls.Add(nudGiaPhong, 1, 6);
        tlpFields.Controls.Add(lblBaoTri, 0, 7);
        tlpFields.Controls.Add(chkSuaChua, 1, 7);
        tlpFields.Controls.Add(lblGhiChu, 0, 8);
        tlpFields.Controls.Add(txtGhiChu, 1, 8);
        tlpFields.Controls.Add(lblHint, 1, 9);
        tlpFields.Dock = DockStyle.Top;
        tlpFields.Location = new Point(0, 0);
        tlpFields.Name = "tlpFields";
        tlpFields.Padding = new Padding(0, 0, 12, 0);
        tlpFields.RowCount = 10;
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
        tlpFields.Size = new Size(518, 445);
        tlpFields.TabIndex = 0;
        // 
        // lblMaPhong
        // 
        lblMaPhong.Anchor = AnchorStyles.Left;
        lblMaPhong.AutoSize = true;
        lblMaPhong.Location = new Point(0, 9);
        lblMaPhong.Margin = new Padding(0, 8, 8, 10);
        lblMaPhong.Name = "lblMaPhong";
        lblMaPhong.Size = new Size(101, 23);
        lblMaPhong.TabIndex = 0;
        lblMaPhong.Text = "Mã phòng *";
        // 
        // txtMaPhong
        // 
        txtMaPhong.Dock = DockStyle.Top;
        txtMaPhong.Location = new Point(150, 5);
        txtMaPhong.Margin = new Padding(0, 5, 0, 8);
        txtMaPhong.MaxLength = 20;
        txtMaPhong.Name = "txtMaPhong";
        txtMaPhong.Size = new Size(356, 30);
        txtMaPhong.TabIndex = 1;
        // 
        // lblTenPhong
        // 
        lblTenPhong.Anchor = AnchorStyles.Left;
        lblTenPhong.AutoSize = true;
        lblTenPhong.Location = new Point(0, 52);
        lblTenPhong.Margin = new Padding(0, 8, 8, 10);
        lblTenPhong.Name = "lblTenPhong";
        lblTenPhong.Size = new Size(103, 23);
        lblTenPhong.TabIndex = 2;
        lblTenPhong.Text = "Tên phòng *";
        // 
        // txtTenPhong
        // 
        txtTenPhong.Dock = DockStyle.Top;
        txtTenPhong.Location = new Point(150, 48);
        txtTenPhong.Margin = new Padding(0, 5, 0, 8);
        txtTenPhong.MaxLength = 50;
        txtTenPhong.Name = "txtTenPhong";
        txtTenPhong.Size = new Size(356, 30);
        txtTenPhong.TabIndex = 3;
        // 
        // lblToa
        // 
        lblToa.Anchor = AnchorStyles.Left;
        lblToa.AutoSize = true;
        lblToa.Location = new Point(0, 94);
        lblToa.Margin = new Padding(0, 8, 8, 10);
        lblToa.Name = "lblToa";
        lblToa.Size = new Size(82, 23);
        lblToa.TabIndex = 4;
        lblToa.Text = "Tòa nhà *";
        // 
        // cboToa
        // 
        cboToa.Dock = DockStyle.Top;
        cboToa.DropDownStyle = ComboBoxStyle.DropDownList;
        cboToa.Location = new Point(150, 91);
        cboToa.Margin = new Padding(0, 5, 0, 8);
        cboToa.Name = "cboToa";
        cboToa.Size = new Size(356, 31);
        cboToa.TabIndex = 5;
        // 
        // lblTang
        // 
        lblTang.Anchor = AnchorStyles.Left;
        lblTang.AutoSize = true;
        lblTang.Location = new Point(0, 136);
        lblTang.Margin = new Padding(0, 8, 8, 10);
        lblTang.Name = "lblTang";
        lblTang.Size = new Size(60, 23);
        lblTang.TabIndex = 6;
        lblTang.Text = "Tầng *";
        // 
        // nudTang
        // 
        nudTang.Dock = DockStyle.Top;
        nudTang.Location = new Point(150, 132);
        nudTang.Margin = new Padding(0, 5, 0, 8);
        nudTang.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        nudTang.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudTang.Name = "nudTang";
        nudTang.Size = new Size(356, 30);
        nudTang.TabIndex = 7;
        nudTang.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // lblLoaiPhong
        // 
        lblLoaiPhong.Anchor = AnchorStyles.Left;
        lblLoaiPhong.AutoSize = true;
        lblLoaiPhong.Location = new Point(0, 179);
        lblLoaiPhong.Margin = new Padding(0, 8, 8, 10);
        lblLoaiPhong.Name = "lblLoaiPhong";
        lblLoaiPhong.Size = new Size(108, 23);
        lblLoaiPhong.TabIndex = 8;
        lblLoaiPhong.Text = "Loại phòng *";
        // 
        // txtLoaiPhong
        // 
        txtLoaiPhong.Dock = DockStyle.Top;
        txtLoaiPhong.Location = new Point(150, 175);
        txtLoaiPhong.Margin = new Padding(0, 5, 0, 8);
        txtLoaiPhong.MaxLength = 50;
        txtLoaiPhong.Name = "txtLoaiPhong";
        txtLoaiPhong.Size = new Size(356, 30);
        txtLoaiPhong.TabIndex = 9;
        // 
        // lblSucChua
        // 
        lblSucChua.Anchor = AnchorStyles.Left;
        lblSucChua.AutoSize = true;
        lblSucChua.Location = new Point(0, 222);
        lblSucChua.Margin = new Padding(0, 8, 8, 10);
        lblSucChua.Name = "lblSucChua";
        lblSucChua.Size = new Size(91, 23);
        lblSucChua.TabIndex = 10;
        lblSucChua.Text = "Sức chứa *";
        // 
        // nudSucChua
        // 
        nudSucChua.Dock = DockStyle.Top;
        nudSucChua.Location = new Point(150, 218);
        nudSucChua.Margin = new Padding(0, 5, 0, 8);
        nudSucChua.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        nudSucChua.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudSucChua.Name = "nudSucChua";
        nudSucChua.Size = new Size(356, 30);
        nudSucChua.TabIndex = 11;
        nudSucChua.Value = new decimal(new int[] { 4, 0, 0, 0 });
        // 
        // lblGiaPhong
        // 
        lblGiaPhong.Anchor = AnchorStyles.Left;
        lblGiaPhong.AutoSize = true;
        lblGiaPhong.Location = new Point(0, 265);
        lblGiaPhong.Margin = new Padding(0, 8, 8, 10);
        lblGiaPhong.Name = "lblGiaPhong";
        lblGiaPhong.Size = new Size(127, 23);
        lblGiaPhong.TabIndex = 12;
        lblGiaPhong.Text = "Giá phòng (đ) *";
        // 
        // nudGiaPhong
        // 
        nudGiaPhong.DecimalPlaces = 2;
        nudGiaPhong.Dock = DockStyle.Top;
        nudGiaPhong.Location = new Point(150, 261);
        nudGiaPhong.Margin = new Padding(0, 5, 0, 8);
        nudGiaPhong.Maximum = new decimal(new int[] { -1486618625, 232830643, 0, 131072 });
        nudGiaPhong.Name = "nudGiaPhong";
        nudGiaPhong.Size = new Size(356, 30);
        nudGiaPhong.TabIndex = 13;
        nudGiaPhong.ThousandsSeparator = true;
        // 
        // lblBaoTri
        // 
        lblBaoTri.Anchor = AnchorStyles.Left;
        lblBaoTri.AutoSize = true;
        lblBaoTri.Location = new Point(0, 307);
        lblBaoTri.Margin = new Padding(0, 8, 8, 10);
        lblBaoTri.Name = "lblBaoTri";
        lblBaoTri.Size = new Size(60, 23);
        lblBaoTri.TabIndex = 14;
        lblBaoTri.Text = "Bảo trì";
        // 
        // chkSuaChua
        // 
        chkSuaChua.AutoSize = true;
        chkSuaChua.Dock = DockStyle.Top;
        chkSuaChua.Location = new Point(150, 304);
        chkSuaChua.Margin = new Padding(0, 5, 0, 8);
        chkSuaChua.Name = "chkSuaChua";
        chkSuaChua.Size = new Size(356, 27);
        chkSuaChua.TabIndex = 15;
        chkSuaChua.Text = "Đang sửa chữa";
        // 
        // lblGhiChu
        // 
        lblGhiChu.Anchor = AnchorStyles.Left;
        lblGhiChu.AutoSize = true;
        lblGhiChu.Location = new Point(0, 349);
        lblGhiChu.Margin = new Padding(0, 8, 8, 10);
        lblGhiChu.Name = "lblGhiChu";
        lblGhiChu.Size = new Size(69, 23);
        lblGhiChu.TabIndex = 16;
        lblGhiChu.Text = "Ghi chú";
        // 
        // txtGhiChu
        // 
        txtGhiChu.Dock = DockStyle.Top;
        txtGhiChu.Location = new Point(150, 345);
        txtGhiChu.Margin = new Padding(0, 5, 0, 8);
        txtGhiChu.MaxLength = 255;
        txtGhiChu.Name = "txtGhiChu";
        txtGhiChu.Size = new Size(356, 30);
        txtGhiChu.TabIndex = 17;
        // 
        // lblHint
        // 
        lblHint.AutoSize = true;
        lblHint.Dock = DockStyle.Top;
        lblHint.Location = new Point(150, 391);
        lblHint.Margin = new Padding(0, 8, 0, 8);
        lblHint.Name = "lblHint";
        lblHint.Size = new Size(356, 46);
        lblHint.TabIndex = 18;
        lblHint.Text = "Số giường đồng bộ theo sức chứa. Trạng thái còn chỗ/đầy được tính tự động.";
        // 
        // lblError
        // 
        lblError.AutoEllipsis = true;
        lblError.Dock = DockStyle.Fill;
        lblError.ForeColor = Color.Firebrick;
        lblError.Location = new Point(21, 484);
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
        flpActions.Location = new Point(21, 551);
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
        // RoomEditDialog
        // 
        AcceptButton = btnLuu;
        AutoScaleDimensions = new SizeF(120F, 120F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(246, 246, 250);
        CancelButton = btnHuy;
        ClientSize = new Size(560, 610);
        Controls.Add(tlpRoot);
        Font = new Font("Segoe UI", 10F);
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(460, 380);
        Name = "RoomEditDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thêm / Sửa phòng";
        FormClosing += Editor_FormClosing;
        tlpRoot.ResumeLayout(false);
        pnlScroll.ResumeLayout(false);
        pnlScroll.PerformLayout();
        tlpFields.ResumeLayout(false);
        tlpFields.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudTang).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudSucChua).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudGiaPhong).EndInit();
        flpActions.ResumeLayout(false);
        ResumeLayout(false);
    }
}

