using QuanLyKyTucXa.Infrastructure.Security;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.WinForms.Forms.Residents;
using QuanLyKyTucXa.WinForms.Forms.Rooms;

namespace QuanLyKyTucXa.WinForms.Forms.Main;

public partial class FrmMain : Form
{
    private readonly string _connectionString;
    private Form? _activeChild;
    private Button? _selectedNav;
    private readonly FlowLayoutPanel _towerTabs = new();
    private readonly FlowLayoutPanel _incidentRows = new();
    private readonly Label _incidentEmpty = new();
    private readonly Label _roomEmptyState = new();
    private readonly Label _utilitySummary = new();
    private readonly ProgressBar _utilityProgress = new();
    private readonly Label _utilityHint = new();
    private readonly Label[] _heroMetrics = { new(), new(), new() };
    private readonly RoundedSearchPanel _searchShell = new();
    private readonly Label _searchIcon = new();
    private string? _selectedTower;
    private bool _refreshingDashboard;
    private ToolTip? _roomMapToolTip;
    private ToolTip? _dashboardStatsToolTip;
    private Label? _utilityTitle;

    public FrmMain(string connectionString) : this(connectionString, null) { }
    public FrmMain(string connectionString, AuthSession? session)
    {
        _connectionString = connectionString;
        _session = session;
        InitializeComponent();
        InitializeDashboardComponents();
        var dayName = CultureInfo.GetCultureInfo("vi-VN").DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);
        lblDate.Text = $"{char.ToUpper(dayName[0], CultureInfo.GetCultureInfo("vi-VN"))}{dayName[1..]}, {DateTime.Today:dd} tháng {DateTime.Today.Month} {DateTime.Today:yyyy}";
        ConfigureRoundedSearch();
        BuildReferenceLowerLayout();
        ConfigureClassicShell();
        ConfigureAuthenticationUi();
        btnNavHome.Click += (_, _) => ShowHome();
        btnNavStudents.Click += (_, _) => ShowStudents();
        btnNavRooms.Click += (_, _) => ShowRooms();
        btnNavRegistration.Click += (_, _) => ShowRegistration();
        btnNavLookup.Click += (_, _) => ShowLookup();
        btnNavApprovals.Click += (_, _) => ShowApprovals();
        btnNavInvoices.Click += (_, _) => ShowComingSoon("Hóa đơn");
        btnNavUtilities.Click += (_, _) => ShowComingSoon("Điện nước");
        btnNavIncidents.Click += (_, _) => ShowComingSoon("Sự cố");
        btnNavReports.Click += (_, _) => ShowComingSoon("Thống kê");
        btnOpenStudents.Click += (_, _) => ShowStudents();
        btnOpenRooms.Click += (_, _) => ShowRooms();
        btnOpenRegistration.Click += (_, _) => ShowRegistration();
        btnOpenLookup.Click += (_, _) => ShowLookup();
        btnOpenApprovals.Click += (_, _) => ShowApprovals();
        pnlHeader.Resize += (_, _) => PositionHeaderActions();
        RoundedCorners.Apply(btnHeaderAdd, 10);
        btnHeaderAdd.Click += (_, _) => ShowStudents();
        txtGlobalSearch.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ShowStudents();
            }
        };
        Load += Main_Load;
        Activated += async (_, _) => { if (Visible && _session?.IsActive == true) await EnsureSessionAsync(); };
    }

    private void PositionHeaderActions()
    {
        btnHeaderAdd.Size = new Size(120, 32);
        btnHeaderAdd.Left = pnlHeader.ClientSize.Width - 26 - btnHeaderAdd.Width;
        btnHeaderAdd.Top = (pnlHeader.ClientSize.Height - btnHeaderAdd.Height) / 2;
        _searchShell.Left = btnHeaderAdd.Left - 10 - _searchShell.Width;
        _searchShell.Top = (pnlHeader.ClientSize.Height - _searchShell.Height) / 2;
        txtGlobalSearch.Width = Math.Max(80, _searchShell.ClientSize.Width - 42);

        lblPageTitle.TextAlign = ContentAlignment.MiddleLeft;
        lblPageTitle.Left = 25;
        lblDate.TextAlign = ContentAlignment.MiddleLeft;
        lblDate.Left = 27;
    }

    private void ConfigureRoundedSearch()
    {
        pnlHeader.Controls.Remove(txtGlobalSearch);
        _searchShell.Size = new Size(240, 36);
        _searchShell.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _searchShell.BackColor = Color.White;
        _searchShell.Controls.Add(_searchIcon);
        _searchIcon.Text = "⌕";
        _searchIcon.Font = new Font("Segoe UI Symbol", 11F);
        _searchIcon.ForeColor = Color.FromArgb(91, 107, 133);
        _searchIcon.TextAlign = ContentAlignment.MiddleCenter;
        _searchIcon.AutoSize = false;
        _searchIcon.Size = new Size(28, 32);
        _searchIcon.Location = new Point(4, 2);
        txtGlobalSearch.BorderStyle = BorderStyle.None;
        txtGlobalSearch.Font = new Font("Segoe UI", 8.5F);
        txtGlobalSearch.Location = new Point(32, 7);
        txtGlobalSearch.Size = new Size(_searchShell.Width - 42, 22);
        txtGlobalSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _searchShell.Controls.Add(txtGlobalSearch);
        RoundedCorners.Apply(_searchShell, 10);
        pnlHeader.Controls.Add(_searchShell);
        _searchShell.BringToFront();
        btnHeaderAdd.BringToFront();
        PositionHeaderActions();
    }

    private void BuildReferenceLowerLayout()
    {
        // Keep the home dashboard proportionate to the compact reference layout.
        tlpHome.RowStyles[0].Height = 210;
        tlpHome.RowStyles[1].Height = 74;
        tlpStats.Margin = new Padding(0, 0, 0, 10);
        foreach (var value in statValues) value.Text = "—";
        lblSectionTitle.Visible = false;
        lblFooter.Visible = false;
        lblHeroEyebrow.Visible = true;
        lblHeroText.Visible = true;
        tlpHome.RowStyles[2].Height = 0;
        tlpHome.RowStyles[4].Height = 0;
        tlpLower.SuspendLayout();
        tlpLower.Controls.Clear();
        tlpLower.ColumnStyles.Clear();
        tlpLower.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 63F));
        tlpLower.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37F));

        var metricCaptions = new[] { "SV đang ở", "Giường", "Phòng quản lý" };
        for (var i = 0; i < _heroMetrics.Length; i++)
        {
            _heroMetrics[i].Text = "—\r\n" + metricCaptions[i];
            _heroMetrics[i].AutoSize = false;
            _heroMetrics[i].Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            _heroMetrics[i].ForeColor = Color.White;
            _heroMetrics[i].BackColor = Color.FromArgb(48, 255, 255, 255);
            _heroMetrics[i].TextAlign = ContentAlignment.MiddleLeft;
            _heroMetrics[i].Padding = new Padding(9, 3, 5, 3);
            _heroMetrics[i].Size = new Size(102, 40);
            _heroMetrics[i].Location = new Point(22 + i * 110, 136);
            pnlHero.Controls.Add(_heroMetrics[i]);
            _heroMetrics[i].BringToFront();
            RoundedCorners.Apply(_heroMetrics[i], 8);
            _heroMetrics[i].Visible = true;
        }

        var roomCard = MakeDashboardCard(new Padding(20, 16, 20, 14));
        roomCard.Margin = new Padding(0, 0, 12, 0);
        var roomLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, BackColor = Color.White, Margin = Padding.Empty };
        roomLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        roomLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        roomLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        roomLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        var roomHeader = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = Padding.Empty };
        lblActivityTitle.Text = "Sơ đồ phòng theo tòa"; lblActivityTitle.Location = new Point(0, 2); lblActivityTitle.Size = new Size(250, 26); lblActivityTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _towerTabs.Dock = DockStyle.Right; _towerTabs.Width = 300; _towerTabs.FlowDirection = FlowDirection.LeftToRight; _towerTabs.WrapContents = false; _towerTabs.BackColor = Color.FromArgb(242, 246, 252); _towerTabs.Padding = new Padding(4); _towerTabs.Margin = Padding.Empty;
        roomHeader.Controls.Add(lblActivityTitle); roomHeader.Controls.Add(_towerTabs);
        lblSectionSub.Text = "Bấm vào một phòng để mở danh sách quản lý phòng."; lblSectionSub.Dock = DockStyle.Fill; lblSectionSub.Location = Point.Empty; lblSectionSub.Font = new Font("Segoe UI", 8.5F);
        flpRoomMap.Dock = DockStyle.Fill; flpRoomMap.Location = Point.Empty; flpRoomMap.Margin = Padding.Empty; flpRoomMap.Padding = new Padding(0, 6, 0, 0); flpRoomMap.FlowDirection = FlowDirection.TopDown; flpRoomMap.WrapContents = false; flpRoomMap.AutoScroll = true; flpRoomMap.BackColor = Color.White;
        var mapHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _roomEmptyState.Text = "Chưa có dữ liệu phòng.\r\nKhi có phòng, trạng thái giường sẽ xuất hiện tại đây."; _roomEmptyState.Dock = DockStyle.Fill; _roomEmptyState.TextAlign = ContentAlignment.MiddleCenter; _roomEmptyState.Font = new Font("Segoe UI", 11F); _roomEmptyState.ForeColor = Color.FromArgb(91, 107, 133);
        mapHost.Controls.Add(flpRoomMap); mapHost.Controls.Add(_roomEmptyState);
        var mapFooter = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        lblRoomMapEmpty.Text = "Chưa có dữ liệu phòng.\r\nKhi có phòng, trạng thái giường sẽ xuất hiện tại đây."; lblRoomMapEmpty.Dock = DockStyle.Fill; lblRoomMapEmpty.TextAlign = ContentAlignment.MiddleCenter; lblRoomMapEmpty.Font = new Font("Segoe UI", 10F); lblRoomMapEmpty.ForeColor = Color.FromArgb(91, 107, 133); lblRoomMapEmpty.Size = new Size(600, 100);
        var legend = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 22, FlowDirection = FlowDirection.LeftToRight, BackColor = Color.White, WrapContents = false };
        legend.Controls.Add(MakeLegend("Còn chỗ", Color.FromArgb(15, 167, 122)));
        legend.Controls.Add(MakeLegend("Đã đầy", Color.FromArgb(226, 61, 94)));
        legend.Controls.Add(MakeLegend("Đang sửa chữa", Color.FromArgb(217, 138, 11)));
        mapFooter.Controls.Add(legend);
        roomLayout.Controls.Add(roomHeader, 0, 0); roomLayout.Controls.Add(lblSectionSub, 0, 1); roomLayout.Controls.Add(mapHost, 0, 2); roomLayout.Controls.Add(mapFooter, 0, 3);
        roomCard.Controls.Add(roomLayout);
        tlpLower.Controls.Add(roomCard, 0, 0);

        var right = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, BackColor = Color.FromArgb(242, 246, 252), Margin = new Padding(0, 0, 0, 0) };
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 38)); right.RowStyles.Add(new RowStyle(SizeType.Percent, 27)); right.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
        right.Controls.Add(BuildIncidentCard(), 0, 0); right.Controls.Add(BuildUtilityCard(), 0, 1); right.Controls.Add(BuildQuickCard(), 0, 2);
        tlpLower.Controls.Add(right, 1, 0);
        tlpLower.ResumeLayout(true);
    }

    private Panel BuildIncidentCard()
    {
        var card = MakeDashboardCard(new Padding(18, 12, 18, 8)); card.Margin = new Padding(0, 0, 0, 12);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Color.White };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(MakeDashboardTitle("Sự cố đang mở"), 0, 0);
        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _incidentRows.Dock = DockStyle.Fill; _incidentRows.FlowDirection = FlowDirection.TopDown; _incidentRows.WrapContents = false; _incidentRows.AutoScroll = true; _incidentRows.BackColor = Color.White;
        _incidentEmpty.Text = "Chưa có sự cố cần theo dõi"; _incidentEmpty.Dock = DockStyle.Fill; _incidentEmpty.TextAlign = ContentAlignment.MiddleCenter; _incidentEmpty.ForeColor = Color.FromArgb(91, 107, 133); _incidentEmpty.Font = new Font("Segoe UI", 9F);
        host.Controls.Add(_incidentRows); host.Controls.Add(_incidentEmpty); layout.Controls.Add(host, 0, 1); card.Controls.Add(layout); return card;
    }

    private Panel BuildUtilityCard()
    {
        var card = MakeDashboardCard(new Padding(18, 12, 18, 10)); card.Margin = new Padding(0, 0, 0, 12);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, BackColor = Color.White };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 10)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _utilityTitle = MakeDashboardTitle("Điện nước"); layout.Controls.Add(_utilityTitle, 0, 0);
        _utilitySummary.Dock = DockStyle.Fill; _utilitySummary.Font = new Font("Segoe UI", 8.5F); _utilitySummary.ForeColor = Color.FromArgb(91, 107, 133);
        _utilityProgress.Dock = DockStyle.Fill; _utilityProgress.Minimum = 0; _utilityProgress.Maximum = 100; _utilityProgress.Style = ProgressBarStyle.Continuous;
        _utilityHint.Dock = DockStyle.Fill; _utilityHint.Font = new Font("Segoe UI", 8F); _utilityHint.ForeColor = Color.FromArgb(91, 107, 133); _utilityHint.TextAlign = ContentAlignment.MiddleLeft;
        layout.Controls.Add(_utilitySummary, 0, 1); layout.Controls.Add(_utilityProgress, 0, 2); layout.Controls.Add(_utilityHint, 0, 3); card.Controls.Add(layout); return card;
    }

    private Panel BuildQuickCard()
    {
        var card = MakeDashboardCard(new Padding(18, 12, 18, 12));
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Color.White };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(MakeDashboardTitle("Làm nhanh"), 0, 0);
        var actions = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, BackColor = Color.White, Padding = new Padding(0, 2, 0, 0) };
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); actions.RowStyles.Add(new RowStyle(SizeType.Percent, 50)); actions.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        actions.Controls.Add(MakeQuickAction("＋  Xếp phòng", () => ShowRooms()), 0, 0);
        actions.Controls.Add(MakeQuickAction("ϟ  Nhập điện nước", () => ShowComingSoon("Điện nước")), 1, 0);
        actions.Controls.Add(MakeQuickAction("▤  Lập hóa đơn", () => ShowComingSoon("Hóa đơn")), 0, 1);
        actions.Controls.Add(MakeQuickAction("⇩  Xuất Excel", () => ShowComingSoon("Xuất Excel")), 1, 1);
        layout.Controls.Add(actions, 0, 1); card.Controls.Add(layout); return card;
    }

    private static Panel MakeDashboardCard(Padding padding) => new RoundedDashboardPanel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = padding, Margin = Padding.Empty };
    private static Label MakeDashboardTitle(string text) => new() { Text = text, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(14, 34, 64), TextAlign = ContentAlignment.MiddleLeft };
    private static Control MakeLegend(string text, Color color)
    {
        var label = new Label { Text = "■  " + text, AutoSize = true, Font = new Font("Segoe UI", 8F), ForeColor = Color.FromArgb(91, 107, 133), Margin = new Padding(0, 2, 12, 0) };
        label.ForeColor = color; return label;
    }
    private static Button MakeQuickAction(string text, Action action)
    {
        var button = new RoundedActionButton { Text = text, Dock = DockStyle.Fill, Margin = new Padding(5), BackColor = Color.FromArgb(242, 246, 252), ForeColor = Color.FromArgb(14, 34, 64), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 8.5F, FontStyle.Bold), Cursor = Cursors.Hand };
        button.FlatAppearance.BorderColor = Color.FromArgb(227, 234, 245); button.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 255); button.Click += (_, _) => action(); return button;
    }

    private void PopulateIncidents(System.Collections.Generic.IReadOnlyList<QuanLyKyTucXa.Infrastructure.SuCo> incidents)
    {
        _incidentRows.SuspendLayout(); _incidentRows.Controls.Clear();
        if (incidents.Count == 0) { _incidentEmpty.Visible = true; _incidentRows.Visible = false; _incidentRows.ResumeLayout(); return; }
        _incidentEmpty.Visible = false; _incidentRows.Visible = true;
        foreach (var incident in incidents)
        {
            var row = new Panel { Width = Math.Max(260, _incidentRows.ClientSize.Width - 25), Height = 48, BackColor = Color.White, Margin = new Padding(0, 0, 0, 2) };
            row.Paint += (_, e) => { using var pen = new Pen(Color.FromArgb(227, 234, 245)); e.Graphics.DrawLine(pen, 0, row.Height - 1, row.Width, row.Height - 1); };
            var priority = incident.MucDoUuTien.Contains("cao", StringComparison.OrdinalIgnoreCase) ? Color.FromArgb(255, 240, 211) : Color.FromArgb(232, 241, 255);
            row.Controls.Add(new Label { Text = "⌁", Location = new Point(1, 8), Size = new Size(34, 32), TextAlign = ContentAlignment.MiddleCenter, BackColor = priority, ForeColor = Color.FromArgb(31, 111, 235), Font = new Font("Segoe UI", 12, FontStyle.Bold) });
            row.Controls.Add(new Label { Text = incident.MaPhong, Location = new Point(44, 5), AutoSize = true, ForeColor = Color.FromArgb(14, 34, 64), Font = new Font("Segoe UI", 8.5F, FontStyle.Bold) });
            row.Controls.Add(new Label { Text = incident.TieuDe, Location = new Point(44, 23), AutoSize = true, ForeColor = Color.FromArgb(91, 107, 133), Font = new Font("Segoe UI", 7.5F) });
            _incidentRows.Controls.Add(row);
        }
        _incidentRows.ResumeLayout();
    }

    private void UpdateUtilities(int readings, int rooms)
    {
        var percentage = rooms == 0 ? 0m : Math.Round(readings * 100m / rooms, 2, MidpointRounding.AwayFromZero);
        _utilitySummary.Text = rooms == 0 ? "Chưa có phòng trong database"
            : $"{readings}/{rooms} phòng đã ghi chỉ số • {percentage.ToString("0.##", CultureInfo.GetCultureInfo("vi-VN"))}%";
        _utilityProgress.Visible = rooms > 0;
        _utilityProgress.Value = (int)Math.Clamp(Math.Round(percentage), 0m, 100m);
        _utilityHint.Text = rooms == 0 ? "Chưa có dữ liệu để tính tỷ lệ."
            : $"Còn {Math.Max(0, rooms - readings)} phòng chưa ghi chỉ số trong tháng.";
    }


    private void SelectNav(Button button)
    {
        if (_selectedNav is not null)
        {
            _selectedNav.BackColor = Color.FromArgb(14, 34, 64);
            _selectedNav.ForeColor = Color.FromArgb(201, 216, 242);
            _selectedNav.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        }
        _selectedNav = button;
        button.BackColor = Color.White;
        button.ForeColor = Color.FromArgb(14, 34, 64);
        button.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
    }

    private async void ShowHome()
    {
        if (_session?.IsStudent == true) { ShowPersonal(); return; }
        if (!RequireUi(AppPermission.Staff)) return;
        tabWorkspace.SelectedTab = tabDashboard;
        _activeChild = null;
        pnlHome.Visible = true;
        pnlHome.BringToFront();
        lblPageTitle.Text = "Chào mừng đến với KTX Home";
        lblDate.Text = "Hệ thống quản lý nội trú";
        SelectNav(btnNavHome);
        await RefreshDashboardAsync();

    }

    private async Task RefreshDashboardAsync()
    {
        if (_session?.IsStaff != true || !_session.IsActive || _refreshingDashboard || IsDisposed || Disposing) return;
        _refreshingDashboard = true;
        var now = DateTime.Now;
        var vi = CultureInfo.GetCultureInfo("vi-VN");
        lblDate.Text = now.ToString("dddd, dd MMMM yyyy", vi);
        try
        {
            await using var db = AppDbContextFactory.Create(_connectionString);
            await AuthPolicy.RequireAsync(db, _session, AppPermission.Staff);
            // Match sp_ThongKeDashboard: residents currently staying and beds actually in use.
            var allStudents = await db.SinhViens.CountAsync();
            var students = await db.SinhViens.CountAsync(s => s.TrangThai == "Đang ở");
            var freeBeds = await db.Giuongs.CountAsync(g => g.TrangThai == "Trống");
            var occupiedBeds = await db.Giuongs.CountAsync(g => g.TrangThai == "Đang sử dụng");
            var totalBeds = await db.Giuongs.CountAsync();
            // Match vw_CongNoHoaDon: exclude cancelled bills and compare charges with payments.
            var unpaidBills = await db.HoaDons.CountAsync(h => h.TrangThai != "Hủy" &&
                h.TongTien > (h.ThanhToans.Sum(t => (decimal?)t.SoTien) ?? 0m));
            var pending = await db.DangKyKtxes.CountAsync(d => d.TrangThai == "Chờ duyệt");
            var openIncidents = db.SuCos.AsNoTracking()
                .Where(s => s.TrangThai == "Mới tiếp nhận" || s.TrangThai == "Đang xử lý");
            var openIssues = await openIncidents.CountAsync();
            var latestIncidents = await openIncidents.OrderByDescending(s => s.NgayBao).Take(3).ToListAsync();
            var roomDetails = await db.Phongs.AsNoTracking().Include(p => p.Giuongs)
                .OrderBy(p => p.MaToa).ThenBy(p => p.Tang).ThenBy(p => p.MaPhong).ToListAsync();
            var rooms = roomDetails.Count;
            var monthlyReadings = await db.ChiSoDienNuocs
                .Where(x => x.Thang == now.Month && x.Nam == now.Year)
                .Select(x => x.MaPhong).Distinct().CountAsync();
            var occupancy = totalBeds == 0 ? (decimal?)null :
                Math.Round(occupiedBeds * 100m / totalBeds, 2, MidpointRounding.AwayFromZero);
            if (IsDisposed || Disposing) return;

            PopulateRoomMap(roomDetails);
            _incidentEmpty.Text = "Không có sự cố đang mở.";
            PopulateIncidents(latestIncidents);
            if (_utilityTitle is not null) _utilityTitle.Text = $"Điện nước tháng {now.Month:D2}/{now.Year}";
            UpdateUtilities(monthlyReadings, rooms);
            _heroMetrics[0].Text = $"{students:N0}\r\nSV đang ở";
            _heroMetrics[1].Text = $"{totalBeds:N0}\r\nGiường";
            _heroMetrics[2].Text = $"{rooms:N0}\r\nPhòng quản lý";
            pnlHero.OccupancyPercent = occupancy;
            statValues[0].Text = students.ToString("N0");
            statCaptions[0].Text = "Sinh viên đang ở";
            statValues[1].Text = freeBeds.ToString("N0");
            statCaptions[1].Text = "Giường trống";
            statValues[2].Text = unpaidBills.ToString("N0");
            statCaptions[2].Text = "Hóa đơn còn nợ";
            statValues[3].Text = openIssues.ToString("N0");
            statCaptions[3].Text = "Sự cố đang mở";
            lblHeroText.Text = $"Có {pending:N0} đơn chờ xét duyệt, {unpaidBills:N0} hóa đơn còn nợ và {openIssues:N0} sự cố đang mở.";
            lblQuickSummary.Text = $"{pending:N0} đơn chờ duyệt\r\n{unpaidBills:N0} hóa đơn còn nợ\r\n{openIssues:N0} sự cố đang mở";
            _dashboardStatsToolTip ??= new ToolTip(components!);
            var studentInfo = $"{students} sinh viên có trạng thái Đang ở / {allStudents} hồ sơ trong database.";
            var bedInfo = $"{totalBeds} giường đã khai báo: {occupiedBeds} đang sử dụng, {freeBeds} trống; còn lại hỏng hoặc bảo trì.";
            _dashboardStatsToolTip.SetToolTip(_heroMetrics[0], studentInfo);
            _dashboardStatsToolTip.SetToolTip(statCards[0], studentInfo);
            _dashboardStatsToolTip.SetToolTip(statValues[0], studentInfo);
            _dashboardStatsToolTip.SetToolTip(_heroMetrics[1], bedInfo);
            _dashboardStatsToolTip.SetToolTip(statValues[1], bedInfo);
            _dashboardStatsToolTip.SetToolTip(pnlHero, totalBeds == 0 ? "Chưa khai báo giường để tính tỷ lệ lấp đầy."
                : $"Tỷ lệ lấp đầy = {occupiedBeds}/{totalBeds} giường đang sử dụng = {occupancy?.ToString("0.##", vi)}%.");
            _dashboardStatsToolTip.SetToolTip(statValues[2], "Hóa đơn không hủy và tổng tiền lớn hơn tổng thanh toán đã ghi nhận.");
            _dashboardStatsToolTip.SetToolTip(statValues[3], "Chỉ tính trạng thái Mới tiếp nhận hoặc Đang xử lý.");
            SetStatus($"Dữ liệu cập nhật từ CSDL lúc {DateTime.Now:HH:mm:ss}");
        }
        catch
        {
            if (IsDisposed || Disposing) return;
            PopulateRoomMap(Array.Empty<QuanLyKyTucXa.Infrastructure.Phong>());
            _roomEmptyState.Text = "Không tải được sơ đồ phòng từ database. Bấm Làm mới để thử lại.";
            PopulateIncidents(Array.Empty<QuanLyKyTucXa.Infrastructure.SuCo>());
            _incidentEmpty.Text = "Không tải được dữ liệu sự cố.";
            foreach (var value in statValues) value.Text = "—";
            lblHeroText.Text = "Không tải được số liệu từ database. Bấm Làm mới để thử lại.";
            lblQuickSummary.Text = "Chưa tải được dữ liệu.";
            _utilitySummary.Text = "Không tải được dữ liệu điện nước.";
            _utilityHint.Text = "Bấm Làm mới để thử lại.";
            _utilityProgress.Visible = false;
            foreach (var metric in _heroMetrics) metric.Text = "—\r\nChưa có dữ liệu";
            pnlHero.OccupancyPercent = null;
            SetStatus("Không tải được số liệu từ cơ sở dữ liệu");
        }
        finally { _refreshingDashboard = false; }
    }


    private void PopulateRoomMap(System.Collections.Generic.IReadOnlyList<QuanLyKyTucXa.Infrastructure.Phong> rooms)
    {
        const string allTowers = "Tất cả";
        _roomMapToolTip ??= new ToolTip(components!);
        var towers = rooms.Select(room => room.MaToa).Distinct().OrderBy(code => code).ToList();
        if (_selectedTower is null || (_selectedTower != allTowers && !towers.Contains(_selectedTower)))
            _selectedTower = allTowers;

        _towerTabs.SuspendLayout();
        foreach (var control in _towerTabs.Controls.Cast<Control>().ToArray()) control.Dispose();
        _towerTabs.Controls.Clear();
        _towerTabs.Visible = rooms.Count > 0;
        foreach (var tower in new[] { allTowers }.Concat(towers))
        {
            var selected = tower == _selectedTower;
            var tab = new Button
            {
                Text = tower == allTowers ? allTowers : "Tòa " + tower,
                AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new Size(76, 32), Padding = new Padding(10, 3, 10, 3),
                TextAlign = ContentAlignment.MiddleCenter, FlatStyle = FlatStyle.Flat,
                BackColor = selected ? Color.White : Color.FromArgb(242, 246, 252),
                ForeColor = selected ? Color.FromArgb(14, 34, 64) : Color.FromArgb(91, 107, 133),
                Font = new Font("Segoe UI", 8F, selected ? FontStyle.Bold : FontStyle.Regular),
                Margin = new Padding(1, 0, 1, 0), Cursor = Cursors.Hand
            };
            tab.FlatAppearance.BorderSize = 0;
            tab.Click += (_, _) => { _selectedTower = tower; PopulateRoomMap(rooms); };
            _towerTabs.Controls.Add(tab);
        }
        _towerTabs.ResumeLayout();
        flpRoomMap.SuspendLayout();
        foreach (var control in flpRoomMap.Controls.Cast<Control>().ToArray()) control.Dispose();
        flpRoomMap.Controls.Clear();
        _roomEmptyState.Text = "Chưa có dữ liệu phòng trong database.";
        _roomEmptyState.Visible = rooms.Count == 0;
        flpRoomMap.Visible = rooms.Count > 0;
        var visibleRooms = rooms.Where(room => _selectedTower == allTowers || room.MaToa == _selectedTower).ToList();
        lblSectionSub.Text = $"Hiển thị {visibleRooms.Count}/{rooms.Count} phòng • Bấm vào phòng để quản lý.";
        foreach (var floor in visibleRooms.GroupBy(room => new { room.MaToa, room.Tang })
                     .OrderBy(group => group.Key.MaToa).ThenByDescending(group => group.Key.Tang))
        {
            var row = new Panel
            {
                Width = Math.Max(320, flpRoomMap.ClientSize.Width - 24), Height = 82,
                BackColor = Color.White, Margin = new Padding(0, 0, 0, 4)
            };
            var heading = new Label
            {
                Text = $"Tòa {floor.Key.MaToa}\r\nTầng {floor.Key.Tang}",
                Location = Point.Empty, Size = new Size(70, 72),
                TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(91, 107, 133)
            };
            var tiles = new FlowLayoutPanel
            {
                Location = new Point(70, 0), Size = new Size(Math.Max(120, row.Width - 70), 78),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                WrapContents = false, AutoScroll = true, BackColor = Color.White, Padding = Padding.Empty
            };
            foreach (var room in floor.OrderBy(room => room.MaPhong))
            {
                var free = room.Giuongs.Count(bed => bed.TrangThai == "Trống");
                var total = room.Giuongs.Count;
                var maintenance = room.TrangThai.Contains("sửa", StringComparison.OrdinalIgnoreCase) ||
                                  room.TrangThai.Contains("bảo trì", StringComparison.OrdinalIgnoreCase);
                var color = maintenance
                    ? (Back: Color.FromArgb(255, 240, 211), Fore: Color.FromArgb(170, 105, 0))
                    : room.TrangThai == "Đã đầy"
                        ? (Back: Color.FromArgb(255, 228, 234), Fore: Color.FromArgb(190, 45, 76))
                        : (Back: Color.FromArgb(221, 246, 236), Fore: Color.FromArgb(15, 130, 96));
                var bedSummary = total == 0 ? "Chưa có giường" : $"{free}/{total} trống";
                var tile = new Button
                {
                    Text = room.MaPhong + "\r\n" + bedSummary, Size = new Size(106, 68),
                    Margin = new Padding(0, 0, 6, 2), FlatStyle = FlatStyle.Flat,
                    BackColor = color.Back, ForeColor = color.Fore,
                    Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand
                };
                tile.FlatAppearance.BorderSize = 0;
                tile.Click += async (_, _) =>
                {
                    ShowRooms();
                    if (_activeChild is FrmPhong roomsForm) await roomsForm.SelectRoomAsync(room.MaPhong);
                };
                _roomMapToolTip.SetToolTip(tile,
                    $"{room.MaPhong} • {room.TrangThai}\r\n{free}/{total} giường trống đã khai báo • Sức chứa: {room.SoChoToiDa}");
                tiles.Controls.Add(tile);
            }
            row.Controls.Add(tiles);
            row.Controls.Add(heading);
            flpRoomMap.Controls.Add(row);
        }
        flpRoomMap.ResumeLayout();
    }


    private void ShowRooms()
    {
        if (_session?.IsStudent == true) { ShowPersonal(); return; }
        if (!RequireUi(AppPermission.ManageRooms)) return;
        var roomForm = new FrmPhong(_connectionString, _session);
        roomForm.ReturnHomeRequested += (_, _) => CloseCurrentTab();
        roomForm.RecordCountChanged += (_, _) => { if (_activeChild == roomForm) SetStatus($"Sẵn sàng - {roomForm.VisibleRoomCount} phòng"); };
        ShowChild(roomForm, "Phòng", "Quản lý phòng ở");
        SelectNav(btnNavRooms);
        if (_activeChild is FrmPhong activeRooms) SetStatus($"Sẵn sàng - {activeRooms.VisibleRoomCount} phòng");
    }

    private void ShowRegistration()
    {
        if (!RequireUi(AppPermission.Register)) return;
        var form = new FrmDangKyKtx(_connectionString, _session);
        form.ReturnHomeRequested += (_, _) => CloseCurrentTab();
        ShowChild(form, "Đăng ký KTX", "Tiếp nhận yêu cầu lưu trú");
        SelectNav(btnNavRegistration);
        SetStatus("Sẵn sàng - Đăng ký KTX");
    }

    private void ShowLookup()
    {
        if (!RequireUi(AppPermission.ViewApplications)) return;
        var form = new FrmTraCuuDangKy(_connectionString, _session);
        form.ReturnHomeRequested += (_, _) => CloseCurrentTab();
        ShowChild(form, "Tra cứu đơn", "Theo dõi tình trạng đăng ký");
        SelectNav(btnNavLookup);
        SetStatus("Sẵn sàng - Tra cứu đơn");
    }

    private void ShowApprovals()
    {
        if (!RequireUi(AppPermission.ApproveApplications)) return;
        var form = new FrmDuyetDon(_connectionString, _session);
        form.ReturnHomeRequested += (_, _) => CloseCurrentTab();
        ShowChild(form, "Duyệt đơn", "Xét duyệt hồ sơ sinh viên");
        SelectNav(btnNavApprovals);
        SetStatus("Sẵn sàng - Duyệt đơn");
    }

    private void ShowStudents()
    {
        if (!RequireUi(AppPermission.ManageResidents)) return;
        var studentForm = new FrmSinhVien(_connectionString, _session);
        studentForm.ReturnHomeRequested += (_, _) => CloseCurrentTab();
        studentForm.RecordCountChanged += (_, _) =>
        {
            if (_activeChild == studentForm) SetStatus($"Sẵn sàng - {studentForm.VisibleStudentCount} sinh viên");
        };
        ShowChild(studentForm, "Sinh viên", "Hồ sơ sinh viên nội trú");
        SelectNav(btnNavStudents);
        if (_activeChild is FrmSinhVien activeStudents)
            SetStatus($"Sẵn sàng - {activeStudents.VisibleStudentCount} sinh viên");
    }


    private void ShowChild(Form child, string title, string subtitle)
    {
        var page = title == "Phòng" ? tabRooms : title == "Sinh viên" ? tabStudents
            : tabWorkspace.TabPages.Cast<TabPage>().FirstOrDefault(p => p.Text == title);
        page ??= new TabPage(title) { BackColor = Color.FromArgb(246, 246, 250) };
        if (!tabWorkspace.TabPages.Contains(page)) tabWorkspace.TabPages.Add(page);
        var existing = page.Controls.OfType<Form>().FirstOrDefault();
        if (existing is not null)
        {
            child.Dispose();
            _activeChild = existing;
            tabWorkspace.SelectedTab = page;
            return;
        }
        child.TopLevel = false;
        child.FormBorderStyle = FormBorderStyle.None;
        child.Dock = DockStyle.Fill;
        _activeChild = child;
        page.Controls.Add(child);
        tabWorkspace.SelectedTab = page;
        child.Show();
    }

    private void CloseActiveChild()
    {
        foreach (var page in tabWorkspace.TabPages.Cast<TabPage>().ToArray())
        {
            foreach (var child in page.Controls.OfType<Form>().ToArray())
            {
                page.Controls.Remove(child);
                child.Close();
                child.Dispose();
            }
        }
        _activeChild = null;
    }


    private void ShowComingSoon(string feature)
    {
        if (_session?.IsStudent == true && feature is "Hóa đơn" or "Sự cố") { ShowPersonal(feature); return; }
        if (!RequireUi(AppPermission.Staff)) return;
        MessageBox.Show(this, $"Phân hệ {feature} chưa được triển khai trong project hiện tại.", feature, MessageBoxButtons.OK, MessageBoxIcon.Information);
        SetStatus($"Phân hệ {feature} chưa sẵn sàng");
    }

    private void SetStatus(string message) => classicStatusText.Text = message;

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        CloseActiveChild();
        base.OnFormClosed(e);
    }
}

internal sealed class RoundedDashboardPanel : Panel
{
    public RoundedDashboardPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        UpdateRoundedRegion();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateRoundedRegion();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = ClientRectangle;
        rect.Width -= 1; rect.Height -= 1;
        using var path = CreateRoundedPath(rect, 18);
        using var brush = new SolidBrush(BackColor);
        using var pen = new Pen(Color.FromArgb(227, 234, 245));
        e.Graphics.FillPath(brush, path);
        e.Graphics.DrawPath(pen, path);
    }

    private void UpdateRoundedRegion()
    {
        if (!IsHandleCreated || Width <= 0 || Height <= 0) return;
        using var path = CreateRoundedPath(new Rectangle(0, 0, Math.Max(1, Width - 1), Math.Max(1, Height - 1)), 18);
        var next = new Region(path);
        var previous = Region;
        Region = next;
        previous?.Dispose();
    }

    private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath(); var d = radius * 2;
        path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
        path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
        path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
        path.CloseFigure(); return path;
    }
}

internal sealed class RoundedActionButton : Button
{
    public RoundedActionButton()
    {
        SetStyle(ControlStyles.ResizeRedraw, true);
        Resize += (_, _) => RoundedCorners.ApplyNow(this, 9);
        HandleCreated += (_, _) => RoundedCorners.ApplyNow(this, 9);
    }

    protected override bool ShowFocusCues => false;
}

internal sealed class RoundedSearchPanel : Panel
{
    public RoundedSearchPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        BackColor = Color.White;
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var bounds = ClientRectangle;
        bounds.Width -= 1;
        bounds.Height -= 1;
        using var path = RoundedCorners.CreatePath(bounds, 10);
        using var fill = new SolidBrush(Color.White);
        using var border = new Pen(Color.FromArgb(220, 229, 242));
        e.Graphics.FillPath(fill, path);
        e.Graphics.DrawPath(border, path);
    }
}

internal static class RoundedCorners
{
    public static void Apply(Control control, int radius)
    {
        void UpdateRegion(object? _, EventArgs __) => ApplyNow(control, radius);
        control.Resize += UpdateRegion;
        control.HandleCreated += UpdateRegion;
        control.Disposed += (_, _) => control.Region?.Dispose();
        ApplyNow(control, radius);
    }

    public static void ApplyNow(Control control, int radius)
    {
        if (control.Width <= 0 || control.Height <= 0) return;
        var old = control.Region;
        using var path = CreatePath(new Rectangle(0, 0, control.Width, control.Height), radius);
        control.Region = new Region(path);
        old?.Dispose();
    }

    public static GraphicsPath CreatePath(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        var diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
        if (diameter < 2) { path.AddRectangle(bounds); return path; }
        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}









