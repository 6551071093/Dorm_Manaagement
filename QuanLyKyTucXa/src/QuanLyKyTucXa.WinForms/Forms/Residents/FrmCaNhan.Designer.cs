#nullable enable
using QuanLyKyTucXa.WinForms.Forms.Auth;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;
partial class FrmCaNhan
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel tlpRoot=null!;
    private Label lblTitle=null!,lblName=null!,lblProfile=null!,lblMessage=null!,lblEmptyTitle=null!,lblEmptySubtitle=null!;
    private PersonalSurface profileCard=null!,tableCard=null!;
    private PersonalBadge avatar=null!,profileStatus=null!;
    private PersonalSectionButton navRooms=null!,navBills=null!,navIncidents=null!;
    private FlowLayoutPanel navigation=null!,flpActions=null!;
    private Panel pnlFooter=null!,emptyPanel=null!,profileText=null!;
    private PersonalEmptySymbol emptySymbol=null!;
    private DataGridView gridRooms=null!,gridBills=null!,gridIncidents=null!;
    private ToolTip hints=null!;
    protected override void Dispose(bool disposing){if(disposing)components?.Dispose();base.Dispose(disposing);}

    private void InitializeComponent()
    {
        components=new System.ComponentModel.Container();hints=new ToolTip(components);
        tlpRoot=new TableLayoutPanel();lblTitle=new Label();lblName=new Label();lblProfile=new Label();lblMessage=new Label();
        profileCard=new PersonalSurface();tableCard=new PersonalSurface();avatar=new PersonalBadge();profileStatus=new PersonalBadge();profileText=new Panel();
        navigation=new FlowLayoutPanel();navRooms=new PersonalSectionButton();navBills=new PersonalSectionButton();navIncidents=new PersonalSectionButton();
        gridRooms=new DataGridView();gridBills=new DataGridView();gridIncidents=new DataGridView();
        emptyPanel=new Panel();emptySymbol=new PersonalEmptySymbol();lblEmptyTitle=new Label();lblEmptySubtitle=new Label();
        pnlFooter=new Panel();flpActions=new FlowLayoutPanel();btnRefresh=new LoginActionButton();btnReport=new LoginActionButton{Primary=true};
        SuspendLayout();tlpRoot.SuspendLayout();profileCard.SuspendLayout();tableCard.SuspendLayout();
        tlpRoot.Dock=DockStyle.Fill;tlpRoot.ColumnCount=1;tlpRoot.RowCount=5;tlpRoot.Padding=new Padding(24,14,24,12);
        tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute,44));tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute,96));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute,0));tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent,100));tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute,56));
        tlpRoot.Controls.Add(lblTitle,0,0);tlpRoot.Controls.Add(profileCard,0,1);tlpRoot.Controls.Add(navigation,0,2);tlpRoot.Controls.Add(tableCard,0,3);tlpRoot.Controls.Add(pnlFooter,0,4);
        lblTitle.Text="Thông tin cá nhân";lblTitle.Font=new Font("Segoe UI",18F,FontStyle.Bold);lblTitle.Dock=DockStyle.Fill;lblTitle.TextAlign=ContentAlignment.MiddleLeft;lblTitle.Margin=Padding.Empty;
        profileCard.Dock=DockStyle.Fill;profileCard.Margin=new Padding(0,0,0,12);profileCard.Padding=new Padding(18,14,18,14);
        avatar.Circle=true;avatar.Text="SV";avatar.Font=new Font("Segoe UI",14F,FontStyle.Bold);avatar.ForeColor=Color.White;avatar.BackColor=Color.FromArgb(23,101,194);avatar.Dock=DockStyle.Left;avatar.Width=52;
        profileStatus.Text="Đang tải";profileStatus.Dock=DockStyle.Right;profileStatus.Width=132;profileStatus.Font=new Font("Segoe UI",9F);profileStatus.BackColor=Color.FromArgb(237,242,249);profileStatus.ForeColor=Color.FromArgb(80,106,142);
        profileText.Dock=DockStyle.Fill;profileText.Padding=new Padding(14,0,10,0);
        lblName.Text="Đang tải hồ sơ...";lblName.Font=new Font("Segoe UI",12F,FontStyle.Bold);lblName.Dock=DockStyle.Top;lblName.Height=30;lblName.AutoEllipsis=true;lblName.TextAlign=ContentAlignment.MiddleLeft;
        lblProfile.Dock=DockStyle.Fill;lblProfile.Font=new Font("Segoe UI",9.5F);lblProfile.ForeColor=Color.FromArgb(77,103,141);lblProfile.AutoEllipsis=true;
        profileText.Controls.Add(lblProfile);profileText.Controls.Add(lblName);
        profileCard.Controls.Add(profileText);profileCard.Controls.Add(profileStatus);profileCard.Controls.Add(avatar);
        navigation.Visible=false;navigation.Dock=DockStyle.Fill;navigation.WrapContents=false;navigation.AutoScroll=true;navigation.Margin=new Padding(0,0,0,12);
        navRooms.Text="Phòng / Hợp đồng";navBills.Text="Hóa đơn của tôi";navIncidents.Text="Báo hỏng của tôi";
        navigation.Controls.Add(navRooms);navigation.Controls.Add(navBills);navigation.Controls.Add(navIncidents);
        navRooms.Click+=(_,_)=>ShowSection(PersonalSection.Rooms);navBills.Click+=(_,_)=>ShowSection(PersonalSection.Bills);navIncidents.Click+=(_,_)=>ShowSection(PersonalSection.Incidents);
        tableCard.Dock=DockStyle.Fill;tableCard.Margin=Padding.Empty;tableCard.Padding=new Padding(1);
        foreach(var grid in new[]{gridRooms,gridBills,gridIncidents})
        {
            grid.Dock=DockStyle.Fill;grid.ReadOnly=true;grid.AutoGenerateColumns=false;grid.AllowUserToAddRows=false;grid.AllowUserToDeleteRows=false;
            grid.RowHeadersVisible=false;grid.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;grid.BackgroundColor=Color.White;
            tableCard.Controls.Add(grid);
        }
        AddColumns(gridRooms,["MaPhong","TenToa","TenGiuong","BatDau","KetThuc","TienPhong","TrangThai"],["Mã phòng","Tòa","Giường","Bắt đầu","Kết thúc","Tiền phòng/tháng","Trạng thái"]);
        AddColumns(gridBills,["MaHoaDon","Thang","Nam","TongTien","DaThu","ConNo","HanThanhToan","TrangThai"],["Mã hóa đơn","Tháng","Năm","Tổng tiền","Đã thu","Còn nợ","Hạn thanh toán","Trạng thái"]);
        AddColumns(gridIncidents,["MaSuCo","MaPhong","TieuDe","TrangThai","NgayBao"],["Mã sự cố","Phòng","Tiêu đề","Trạng thái","Ngày báo"]);
        emptyPanel.BackColor=Color.White;emptyPanel.Controls.Add(emptySymbol);emptyPanel.Controls.Add(lblEmptyTitle);emptyPanel.Controls.Add(lblEmptySubtitle);tableCard.Controls.Add(emptyPanel);
        lblEmptyTitle.Font=new Font("Segoe UI",11F,FontStyle.Bold);lblEmptyTitle.TextAlign=ContentAlignment.MiddleCenter;
        lblEmptySubtitle.Font=new Font("Segoe UI",9.5F);lblEmptySubtitle.ForeColor=Color.FromArgb(80,109,150);lblEmptySubtitle.TextAlign=ContentAlignment.TopCenter;
        tableCard.SizeChanged+=(_,_)=>LayoutEmptyState();
        pnlFooter.Dock=DockStyle.Fill;pnlFooter.Margin=Padding.Empty;pnlFooter.Padding=new Padding(0,12,0,0);
        lblMessage.Dock=DockStyle.Fill;lblMessage.Font=new Font("Segoe UI",9F);lblMessage.ForeColor=Color.FromArgb(80,106,142);lblMessage.TextAlign=ContentAlignment.MiddleLeft;
        flpActions.AutoSize=true;flpActions.Dock=DockStyle.Right;flpActions.FlowDirection=FlowDirection.RightToLeft;flpActions.WrapContents=false;
        btnRefresh.Text="Làm mới";btnReport.Text="Gửi báo hỏng";btnRefresh.Click+=Refresh_Click;btnReport.Click+=Report_Click;
        flpActions.Controls.Add(btnReport);flpActions.Controls.Add(btnRefresh);pnlFooter.Controls.Add(lblMessage);pnlFooter.Controls.Add(flpActions);
        AutoScaleDimensions=new SizeF(96,96);AutoScaleMode=AutoScaleMode.Dpi;Font=new Font("Segoe UI",10F);
        BackColor=Color.FromArgb(244,247,253);ForeColor=Color.FromArgb(20,47,81);ClientSize=new Size(1100,650);Name="FrmCaNhan";Text="Thông tin cá nhân";Controls.Add(tlpRoot);
        tableCard.ResumeLayout(false);profileCard.ResumeLayout(false);tlpRoot.ResumeLayout(false);ResumeLayout(false);
    }
    private Button btnRefresh=null!,btnReport=null!;
    private static void AddColumns(DataGridView grid,string[] names,string[] captions)
    {
        for(var i=0;i<names.Length;i++)grid.Columns.Add(new DataGridViewTextBoxColumn{Name=names[i],DataPropertyName=names[i],HeaderText=captions[i],MinimumWidth=70,SortMode=DataGridViewColumnSortMode.NotSortable});
    }
}
