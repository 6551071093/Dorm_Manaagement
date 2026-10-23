using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class FrmCaNhan : Form
{
    private readonly PersonalService? _service;
    private bool _busy;
    public FrmCaNhan()
    {
        InitializeComponent();
        Load += async (_, _) => await RefreshViewAsync();
    }
    public FrmCaNhan(string connectionString, AuthSession session) : this() =>
        _service = new PersonalService(() => AppDbContextFactory.Create(connectionString), session);
    public void SelectSection(string? section)
    {
        tabs.SelectedTab = section switch { "Hóa đơn" => tabBills, "Sự cố" => tabIncidents, _ => tabRooms };
    }
    public async Task RefreshViewAsync()
    {
        if (_busy || _service is null) return;
        _busy = true; btnRefresh.Enabled = btnReport.Enabled = false;
        try
        {
            var data = await _service.LoadAsync();
            if (IsDisposed || Disposing) return;
            var p = data.Profile;
            lblProfile.Text = $"{p.MaSv} — {p.HoTen}\n{p.Lop} • {p.Khoa} • {p.TrangThai}".Replace("\n", Environment.NewLine);
            gridRooms.DataSource = data.Rooms;
            gridBills.DataSource = data.Bills.Select(b => new { b.MaHoaDon, b.Thang, b.Nam, b.TongTien, b.DaThu, ConNo = Math.Max(0, b.TongTien - b.DaThu), b.HanThanhToan, b.TrangThai }).ToList();
            gridIncidents.DataSource = data.Incidents;
            ConfigureGrid(gridRooms, ["Mã phòng", "Tòa", "Giường", "Bắt đầu", "Kết thúc", "Tiền phòng/tháng", "Trạng thái"]);
            ConfigureGrid(gridBills, ["Mã hóa đơn", "Tháng", "Năm", "Tổng tiền", "Đã thu", "Còn nợ", "Hạn thanh toán", "Trạng thái"]);
            ConfigureGrid(gridIncidents, ["Mã sự cố", "Phòng", "Tiêu đề", "Trạng thái", "Ngày báo"]);
            lblMessage.Text = "Chỉ hiển thị dữ liệu gắn với tài khoản của bạn.";
        }
        catch (Exception ex) { lblProfile.Text = ""; gridRooms.DataSource = gridBills.DataSource = gridIncidents.DataSource = null; lblMessage.Text = ex is UnauthorizedAccessException ? ex.Message : "Không tải được dữ liệu cá nhân. Kiểm tra kết nối và thử lại."; }
        finally { _busy = false; if (!IsDisposed) btnRefresh.Enabled = btnReport.Enabled = true; }
    }
    private static void ConfigureGrid(DataGridView grid, string[] headers)
    {
        for (var i = 0; i < grid.Columns.Count; i++)
        {
            var c = grid.Columns[i]; c.HeaderText = headers[i]; c.SortMode = DataGridViewColumnSortMode.NotSortable;
            if (c.ValueType == typeof(decimal)) c.DefaultCellStyle.Format = "N0";
            if (c.ValueType == typeof(DateOnly) || c.ValueType == typeof(DateTime)) c.DefaultCellStyle.Format = "dd/MM/yyyy";
        }
    }
    private async void Refresh_Click(object? sender, EventArgs e) => await RefreshViewAsync();
    private async void Report_Click(object? sender, EventArgs e)
    {
        if (_busy || _service is null) return;
        using var dialog = new ReportIncidentDialog(_service);
        if (dialog.ShowDialog(this) == DialogResult.OK) { tabs.SelectedTab = tabIncidents; await RefreshViewAsync(); }
    }
}