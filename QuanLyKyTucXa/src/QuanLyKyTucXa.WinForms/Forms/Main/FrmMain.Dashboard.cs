#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Main;

partial class FrmMain
{
    
    private Panel pnlSidebar = null!, pnlMain = null!, pnlHeader = null!, pnlWorkspace = null!, pnlHome = null!, pnlProfile = null!;
    private GradientPanel pnlHero = null!;
    private Label lblLogoMark = null!, lblLogo = null!, lblLogoSub = null!, lblProfileAvatar = null!, lblProfileName = null!, lblProfileRole = null!, lblPageTitle = null!, lblDate = null!, lblHeroEyebrow = null!, lblHeroTitle = null!, lblHeroText = null!, lblSectionTitle = null!, lblSectionSub = null!, lblActivityTitle = null!, lblQuickTitle = null!, lblFooter = null!;
    private TextBox txtGlobalSearch = null!;
    private Button btnHeaderAdd = null!, btnNavHome = null!, btnNavStudents = null!, btnNavRooms = null!, btnNavRegistration = null!, btnNavLookup = null!, btnNavApprovals = null!, btnNavInvoices = null!, btnNavUtilities = null!, btnNavIncidents = null!, btnNavReports = null!;
    private FlowLayoutPanel flpNav = null!, flpRoomMap = null!;
    private Label lblRoomMapEmpty = null!, lblQuickSummary = null!;
    private Button btnOpenStudents = null!, btnOpenRooms = null!, btnOpenRegistration = null!, btnOpenLookup = null!, btnOpenApprovals = null!;
    private TableLayoutPanel tlpHome = null!, tlpStats = null!, tlpLower = null!;
    private Panel[] statCards = null!;
    private Label[] statHeadings = null!, statValues = null!, statCaptions = null!;
    private StatusStrip statusStripMain = null!;
    private ToolStripStatusLabel lblStatus = null!;

    

    private void InitializeDashboardComponents()
    {
        
        pnlSidebar = new Panel(); pnlMain = new Panel(); pnlHeader = new Panel(); pnlWorkspace = new Panel(); pnlHome = new Panel(); pnlHero = new GradientPanel(); pnlProfile = new Panel();
        lblLogoMark = new Label(); lblLogo = new Label(); lblLogoSub = new Label(); lblProfileAvatar = new Label(); lblProfileName = new Label(); lblProfileRole = new Label(); lblPageTitle = new Label(); lblDate = new Label(); lblHeroEyebrow = new Label(); lblHeroTitle = new Label(); lblHeroText = new Label(); lblSectionTitle = new Label(); lblSectionSub = new Label(); lblActivityTitle = new Label(); lblQuickTitle = new Label(); lblFooter = new Label();
        txtGlobalSearch = new TextBox(); btnHeaderAdd = new Button(); flpNav = new FlowLayoutPanel(); btnNavHome = new Button(); btnNavStudents = new Button(); btnNavRooms = new Button(); btnNavRegistration = new Button(); btnNavLookup = new Button(); btnNavApprovals = new Button(); btnNavInvoices = new Button(); btnNavUtilities = new Button(); btnNavIncidents = new Button(); btnNavReports = new Button();
        flpRoomMap = new FlowLayoutPanel(); lblRoomMapEmpty = new Label(); lblQuickSummary = new Label(); btnOpenStudents = new Button(); btnOpenRooms = new Button(); btnOpenRegistration = new Button(); btnOpenLookup = new Button(); btnOpenApprovals = new Button();
        tlpHome = new TableLayoutPanel(); tlpStats = new TableLayoutPanel(); tlpLower = new TableLayoutPanel(); statCards = new Panel[4]; statHeadings = new Label[4]; statValues = new Label[4]; statCaptions = new Label[4];
        statusStripMain = new StatusStrip(); lblStatus = new ToolStripStatusLabel();
        SuspendLayout(); pnlMain.SuspendLayout(); pnlHeader.SuspendLayout(); pnlWorkspace.SuspendLayout(); pnlHome.SuspendLayout(); pnlHero.SuspendLayout(); flpNav.SuspendLayout(); tlpHome.SuspendLayout(); tlpStats.SuspendLayout(); tlpLower.SuspendLayout(); pnlSidebar.SuspendLayout(); pnlProfile.SuspendLayout();

        pnlSidebar.BackColor = Color.FromArgb(14, 34, 64); pnlSidebar.Dock = DockStyle.Left; pnlSidebar.Padding = new Padding(12, 18, 12, 12); pnlSidebar.Width = 200; pnlSidebar.Controls.Add(pnlProfile); pnlSidebar.Controls.Add(flpNav); pnlSidebar.Controls.Add(lblLogoSub); pnlSidebar.Controls.Add(lblLogo);
        lblLogoMark.Text = "K"; lblLogoMark.TextAlign = ContentAlignment.MiddleCenter; lblLogoMark.Font = new Font("Segoe UI", 13F, FontStyle.Bold); lblLogoMark.ForeColor = Color.White; lblLogoMark.BackColor = Color.FromArgb(31, 111, 235); lblLogoMark.Location = new Point(14, 18); lblLogoMark.Size = new Size(38, 38); lblLogoMark.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        lblLogo.Text = "KTX Home"; lblLogo.TextAlign = ContentAlignment.MiddleCenter; lblLogo.Font = new Font("Segoe UI", 14F, FontStyle.Bold); lblLogo.ForeColor = Color.White; lblLogo.Location = new Point(12, 17); lblLogo.Size = new Size(176, 28); lblLogo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblLogoSub.Text = "QUẢN LÝ KÝ TÚC XÁ"; lblLogoSub.TextAlign = ContentAlignment.MiddleCenter; lblLogoSub.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold); lblLogoSub.ForeColor = Color.FromArgb(143, 166, 203); lblLogoSub.Location = new Point(12, 43); lblLogoSub.Size = new Size(176, 20); lblLogoSub.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        flpNav.Location = new Point(10, 94); flpNav.Size = new Size(176, 530); flpNav.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; flpNav.FlowDirection = FlowDirection.TopDown; flpNav.WrapContents = false; flpNav.AutoScroll = true; flpNav.BackColor = Color.Transparent;
        SetNavButton(btnNavHome, "▦   Tổng quan", true); SetNavButton(btnNavStudents, "♙   Sinh viên"); SetNavButton(btnNavRooms, "▤   Phòng"); SetNavButton(btnNavRegistration, "▧   Đăng ký KTX"); SetNavButton(btnNavLookup, "⌕   Tra cứu đơn"); SetNavButton(btnNavApprovals, "✓   Duyệt đơn"); SetNavButton(btnNavInvoices, "▤   Hóa đơn"); SetNavButton(btnNavUtilities, "ϟ   Điện nước"); SetNavButton(btnNavIncidents, "⚒   Sự cố"); SetNavButton(btnNavReports, "▥   Thống kê"); flpNav.Controls.AddRange(new Control[] { btnNavHome, btnNavStudents, btnNavRooms, btnNavRegistration, btnNavLookup, btnNavApprovals, btnNavInvoices, btnNavUtilities, btnNavIncidents, btnNavReports });
        pnlProfile.Dock = DockStyle.Bottom; pnlProfile.Height = 68; pnlProfile.BackColor = Color.FromArgb(14, 34, 64); pnlProfile.Padding = new Padding(2, 12, 0, 0); pnlProfile.Controls.Add(lblProfileAvatar); pnlProfile.Controls.Add(lblProfileName); pnlProfile.Controls.Add(lblProfileRole);
        lblProfileAvatar.Text = "TA"; lblProfileAvatar.TextAlign = ContentAlignment.MiddleCenter; lblProfileAvatar.Font = new Font("Segoe UI", 9F, FontStyle.Bold); lblProfileAvatar.ForeColor = Color.White; lblProfileAvatar.BackColor = Color.FromArgb(76, 154, 255); lblProfileAvatar.Location = new Point(5, 15); lblProfileAvatar.Size = new Size(38, 38);
        lblProfileName.Text = "tu.admin"; lblProfileName.Font = new Font("Segoe UI", 9F, FontStyle.Bold); lblProfileName.ForeColor = Color.White; lblProfileName.Location = new Point(53, 12); lblProfileName.Size = new Size(145, 22);
        lblProfileRole.Text = "Quản trị viên"; lblProfileRole.Font = new Font("Segoe UI", 8.5F); lblProfileRole.ForeColor = Color.FromArgb(143, 166, 203); lblProfileRole.Location = new Point(53, 34); lblProfileRole.Size = new Size(145, 22);

        pnlMain.Dock = DockStyle.Fill; pnlMain.BackColor = Color.FromArgb(242, 246, 252); pnlMain.Controls.Add(pnlWorkspace); pnlMain.Controls.Add(pnlHeader); pnlMain.Controls.Add(statusStripMain);
        pnlHeader.Dock = DockStyle.Top; pnlHeader.Height = 80; pnlHeader.BackColor = Color.White; pnlHeader.Padding = new Padding(26, 14, 26, 10); pnlHeader.Controls.Add(btnHeaderAdd); pnlHeader.Controls.Add(txtGlobalSearch); pnlHeader.Controls.Add(lblDate); pnlHeader.Controls.Add(lblPageTitle);
        lblPageTitle.Text = "Chào mừng đến với KTX Home"; lblPageTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold); lblPageTitle.ForeColor = Color.FromArgb(14, 34, 64); lblPageTitle.Location = new Point(25, 11); lblPageTitle.Size = new Size(490, 30);
        lblDate.Text = "Hệ thống quản lý nội trú"; lblDate.Font = new Font("Segoe UI", 9F); lblDate.ForeColor = Color.FromArgb(91, 107, 133); lblDate.Location = new Point(27, 42); lblDate.Size = new Size(450, 22);
        txtGlobalSearch.PlaceholderText = "Tìm sinh viên, phòng, hóa đơn"; txtGlobalSearch.Font = new Font("Segoe UI", 9.5F); txtGlobalSearch.BorderStyle = BorderStyle.FixedSingle; txtGlobalSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right; txtGlobalSearch.Location = new Point(910, 23); txtGlobalSearch.Size = new Size(240, 36);
        btnHeaderAdd.Text = "Thêm sinh viên"; btnHeaderAdd.BackColor = Color.FromArgb(31, 111, 235); btnHeaderAdd.ForeColor = Color.White; btnHeaderAdd.FlatStyle = FlatStyle.Flat; btnHeaderAdd.FlatAppearance.BorderSize = 0; btnHeaderAdd.Font = new Font("Segoe UI", 8F, FontStyle.Bold); btnHeaderAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right; btnHeaderAdd.Location = new Point(1328, 23); btnHeaderAdd.Size = new Size(120, 32); btnHeaderAdd.Cursor = Cursors.Hand;
        statusStripMain.Dock = DockStyle.Bottom; statusStripMain.BackColor = Color.FromArgb(14, 34, 64); statusStripMain.ForeColor = Color.White; statusStripMain.SizingGrip = false; lblStatus.Text = "Sẵn sàng"; lblStatus.ForeColor = Color.White; statusStripMain.Items.Add(lblStatus);
        pnlWorkspace.Dock = DockStyle.Fill; pnlWorkspace.BackColor = Color.FromArgb(242, 246, 252); pnlWorkspace.Padding = new Padding(24, 20, 24, 20); pnlWorkspace.Controls.Add(pnlHome);

        pnlHome.Dock = DockStyle.Fill; pnlHome.BackColor = Color.FromArgb(242, 246, 252); pnlHome.Controls.Add(tlpHome);
        tlpHome.Dock = DockStyle.Fill; tlpHome.BackColor = Color.FromArgb(242, 246, 252); tlpHome.ColumnCount = 1; tlpHome.RowCount = 5; tlpHome.Padding = Padding.Empty; tlpHome.RowStyles.Add(new RowStyle(SizeType.Absolute, 174F)); tlpHome.RowStyles.Add(new RowStyle(SizeType.Absolute, 116F)); tlpHome.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F)); tlpHome.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); tlpHome.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F)); tlpHome.Controls.Add(pnlHero, 0, 0); tlpHome.Controls.Add(tlpStats, 0, 1); tlpHome.Controls.Add(lblSectionTitle, 0, 2); tlpHome.Controls.Add(tlpLower, 0, 3); tlpHome.Controls.Add(lblFooter, 0, 4);
        pnlHero.Dock = DockStyle.Fill; pnlHero.BackColor = Color.FromArgb(14, 34, 64); pnlHero.Margin = new Padding(0, 0, 0, 14); pnlHero.Padding = new Padding(28, 22, 28, 16); pnlHero.Controls.Add(lblHeroEyebrow); pnlHero.Controls.Add(lblHeroTitle); pnlHero.Controls.Add(lblHeroText);
        lblHeroEyebrow.Text = "TRUNG TÂM ĐIỀU HÀNH KÝ TÚC XÁ"; lblHeroEyebrow.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold); lblHeroEyebrow.ForeColor = Color.FromArgb(125, 190, 255); lblHeroEyebrow.Location = new Point(24, 19); lblHeroEyebrow.Size = new Size(520, 22);
        lblHeroTitle.Text = "Quản lý ký túc xá"; lblHeroTitle.Font = new Font("Segoe UI", 21F, FontStyle.Bold); lblHeroTitle.ForeColor = Color.White; lblHeroTitle.AutoSize = false; lblHeroTitle.AutoEllipsis = false; lblHeroTitle.TextAlign = ContentAlignment.MiddleLeft; lblHeroTitle.Location = new Point(22, 43); lblHeroTitle.Size = new Size(600, 52);
        lblHeroText.Text = "Theo dõi sinh viên, phòng ở và quy trình đăng ký với giao diện thống nhất."; lblHeroEyebrow.BackColor = Color.Transparent; lblHeroTitle.BackColor = Color.Transparent; lblHeroText.BackColor = Color.Transparent; lblHeroText.Font = new Font("Segoe UI", 10F); lblHeroText.ForeColor = Color.FromArgb(197, 216, 250); lblHeroText.Location = new Point(27, 100); lblHeroText.Size = new Size(760, 28);
        tlpStats.Dock = DockStyle.Fill; tlpStats.ColumnCount = 4; tlpStats.RowCount = 1; tlpStats.Margin = new Padding(0, 0, 0, 12); for (int i=0;i<4;i++){tlpStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25F));tlpStats.RowStyles.Clear();} tlpStats.RowStyles.Add(new RowStyle(SizeType.Percent,100F));
        string[] titles={"Sinh viên","Giường trống","Hóa đơn chưa thu","Sự cố đang xử lý"}; string[] values={"0","0","0","0"}; string[] captions={"Sinh viên","Giường trống","Hóa đơn chưa thu","Sự cố đang xử lý"}; Color[] accents={Color.FromArgb(31,111,235),Color.FromArgb(15,167,122),Color.FromArgb(226,61,94),Color.FromArgb(217,138,11)}; Color[] iconBacks={Color.FromArgb(232,241,255),Color.FromArgb(221,246,236),Color.FromArgb(255,228,234),Color.FromArgb(255,240,211)}; string[] icons={"♙","▰","▤","⚒"};
        for(int i=0;i<4;i++)
        {
            var card=new RoundedDashboardPanel{Dock=DockStyle.Fill,BackColor=Color.White,Margin=new Padding(0,0,14,0),Padding=new Padding(12,4,12,4)};
            var content=new Panel{Dock=DockStyle.Fill,BackColor=Color.Transparent,Margin=new Padding(2)};
            var icon=new Label{Text=icons[i],Font=new Font("Segoe UI Symbol",12F),ForeColor=accents[i],BackColor=iconBacks[i],TextAlign=ContentAlignment.MiddleCenter,AutoSize=false,Size=new Size(36,36),Location=new Point(0,0),Margin=Padding.Empty};
            var value=new Label{Text=values[i],Font=new Font("Segoe UI",13F,FontStyle.Bold),ForeColor=Color.FromArgb(14,34,64),BackColor=Color.Transparent,TextAlign=ContentAlignment.MiddleLeft,AutoSize=false,AutoEllipsis=true,Location=new Point(46,0),Size=new Size(100,26)};
            var caption=new Label{Text=captions[i],Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(91,107,133),BackColor=Color.Transparent,TextAlign=ContentAlignment.MiddleLeft,AutoSize=false,AutoEllipsis=true,Location=new Point(46,26),Size=new Size(160,20)};
            content.Resize += (_, _) => icon.Top = Math.Max(0, (content.ClientSize.Height - icon.Height) / 2);
            content.Resize += (_, _) => { var top = Math.Max(0, (content.ClientSize.Height - value.Height - caption.Height) / 2); value.Top = top; caption.Top = top + value.Height; };
            var h=new Label{Text=titles[i],Font=new Font("Segoe UI",8F,FontStyle.Bold),ForeColor=accents[i],Visible=false};
            content.Controls.Add(icon);content.Controls.Add(value);content.Controls.Add(caption);card.Controls.Add(content);statCards[i]=card;statHeadings[i]=h;statValues[i]=value;statCaptions[i]=caption;tlpStats.Controls.Add(card,i,0);
        }
        lblSectionTitle.Text = "Truy cập nhanh"; lblSectionTitle.Dock = DockStyle.Fill; lblSectionTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold); lblSectionTitle.ForeColor = Color.FromArgb(14, 34, 64); lblSectionTitle.TextAlign = ContentAlignment.MiddleLeft; lblSectionTitle.Margin = new Padding(0,0,0,8);
        tlpLower.Dock = DockStyle.Fill; tlpLower.ColumnCount=2;tlpLower.RowCount=1;tlpLower.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,68F));tlpLower.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,32F));tlpLower.RowStyles.Add(new RowStyle(SizeType.Percent,100F));
        var boxModules=new Panel{Dock=DockStyle.Fill,BackColor=Color.White,Padding=new Padding(17),Margin=new Padding(0,0,14,0)};var boxQuick=new Panel{Dock=DockStyle.Fill,BackColor=Color.White,Padding=new Padding(17),Margin=Padding.Empty};boxModules.Controls.Add(flpRoomMap);boxModules.Controls.Add(lblSectionSub);boxModules.Controls.Add(lblActivityTitle);boxQuick.Controls.Add(lblQuickSummary);boxQuick.Controls.Add(lblQuickTitle);boxQuick.Controls.Add(btnOpenApprovals);boxQuick.Controls.Add(btnOpenLookup);tlpLower.Controls.Add(boxModules,0,0);tlpLower.Controls.Add(boxQuick,1,0);
        lblActivityTitle.Text="Sơ đồ phòng theo tòa";lblActivityTitle.Font=new Font("Segoe UI",11F,FontStyle.Bold);lblActivityTitle.ForeColor=Color.FromArgb(14,34,64);lblActivityTitle.Location=new Point(17,13);lblActivityTitle.Size=new Size(500,25);
        lblSectionSub.Text="Bấm vào một phòng để mở danh sách quản lý phòng.";lblSectionSub.Font=new Font("Segoe UI",8.5F);lblSectionSub.ForeColor=Color.FromArgb(91,107,133);lblSectionSub.Location=new Point(19,39);lblSectionSub.Size=new Size(500,22);
        flpRoomMap.Location=new Point(17,71);flpRoomMap.Size=new Size(720,300);flpRoomMap.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;flpRoomMap.FlowDirection=FlowDirection.TopDown;flpRoomMap.WrapContents=false;flpRoomMap.AutoScroll=true;flpRoomMap.BackColor=Color.White;lblRoomMapEmpty.Text="Chưa có dữ liệu phòng để hiển thị.";lblRoomMapEmpty.Font=new Font("Segoe UI",10F);lblRoomMapEmpty.ForeColor=Color.FromArgb(91,107,133);lblRoomMapEmpty.TextAlign=ContentAlignment.MiddleCenter;lblRoomMapEmpty.AutoSize=false;lblRoomMapEmpty.Size=new Size(760,230);flpRoomMap.Controls.Add(lblRoomMapEmpty);
        lblQuickTitle.Text="Việc cần theo dõi";lblQuickTitle.Font=new Font("Segoe UI",11F,FontStyle.Bold);lblQuickTitle.ForeColor=Color.FromArgb(14,34,64);lblQuickTitle.Location=new Point(17,13);lblQuickTitle.Size=new Size(250,25);SetShortcut(btnOpenLookup,"Tra cứu đơn",Color.FromArgb(217,138,11));SetShortcut(btnOpenApprovals,"Duyệt đơn",Color.FromArgb(15,167,122));btnOpenLookup.Location=new Point(17,55);btnOpenLookup.Size=new Size(260,42);btnOpenApprovals.Location=new Point(17,105);btnOpenApprovals.Size=new Size(260,42);lblQuickSummary.Text="Đơn đăng ký đang chờ\r\nHóa đơn chưa thanh toán\r\nSự cố chưa hoàn tất";lblQuickSummary.Font=new Font("Segoe UI",9F);lblQuickSummary.ForeColor=Color.FromArgb(91,107,133);lblQuickSummary.Location=new Point(17,164);lblQuickSummary.Size=new Size(280,80);
        lblFooter.Text="KTX Home  •  Hệ thống quản lý ký túc xá";lblFooter.Font=new Font("Segoe UI",8F);lblFooter.ForeColor=Color.FromArgb(91,107,133);lblFooter.Dock=DockStyle.Bottom;lblFooter.Height=24;


tlpLower.ResumeLayout(false);tlpStats.ResumeLayout(false);tlpHome.ResumeLayout(false);pnlHero.ResumeLayout(false);pnlHome.ResumeLayout(false);pnlWorkspace.ResumeLayout(false);pnlHeader.ResumeLayout(false);pnlHeader.PerformLayout();statusStripMain.ResumeLayout(false);statusStripMain.PerformLayout();pnlProfile.ResumeLayout(false);flpNav.ResumeLayout(false);pnlSidebar.ResumeLayout(false);pnlSidebar.PerformLayout();pnlMain.ResumeLayout(false);pnlMain.PerformLayout();ResumeLayout(false);
    }
    private static void SetNavButton(Button b,string text,bool selected=false){b.Text=text;b.TextAlign=ContentAlignment.MiddleLeft;b.Font=new Font("Segoe UI",10F,selected?FontStyle.Bold:FontStyle.Regular);b.ForeColor=selected?Color.FromArgb(14,34,64):Color.FromArgb(201,216,242);b.BackColor=selected?Color.White:Color.FromArgb(14,34,64);b.FlatStyle=FlatStyle.Flat;b.FlatAppearance.BorderSize=0;b.FlatAppearance.MouseOverBackColor=selected?Color.FromArgb(239,245,255):Color.FromArgb(76,139,220);b.Margin=new Padding(0,3,0,3);b.Padding=new Padding(9,0,0,0);b.Size=new Size(174,46);b.Cursor=Cursors.Hand;}
    private static void SetShortcut(Button b,string text,Color accent){b.Text=text;b.Font=new Font("Segoe UI",10F,FontStyle.Bold);b.ForeColor=accent;b.BackColor=Color.FromArgb(242,246,252);b.FlatStyle=FlatStyle.Flat;b.FlatAppearance.BorderColor=Color.FromArgb(227,234,245);b.FlatAppearance.MouseOverBackColor=Color.FromArgb(232,241,255);b.Size=new Size(190,66);b.Margin=new Padding(0,0,10,0);b.Cursor=Cursors.Hand;}
}









