using QuanLyKyTucXa.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyKyTucXa.WinForms.Forms.Rooms;

public partial class FrmPhong : Form
{
    private readonly AuthSession? _session;
    public event EventHandler? ReturnHomeRequested;
    public event EventHandler? RecordCountChanged;
    public int VisibleRoomCount => dgvPhong.Rows.Cast<DataGridViewRow>().Count(row => row.Visible);

    public FrmPhong() : this(string.Empty) { }

    public FrmPhong(string connectionString, AuthSession? session = null)
    {
        _connectionString = connectionString;
        _session = session;
        InitializeComponent();
        if (_session is not null)
        {
            btnXoa.Visible = AuthPolicy.Can(_session, AppPermission.DeleteRooms);
            btnThem.Visible = btnSua.Visible = AuthPolicy.Can(_session, AppPermission.ConfigureRooms);
        }
        cboToa.SelectedIndex = 0;
        cboTrangThai.SelectedIndex = 0;
        Load += async (_, _) => await RefreshViewAsync();
    }


    protected override void OnLoad(EventArgs e)
    {
        ApplyRoomLayoutScale();
        base.OnLoad(e);
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        if (tlpRoom is not null) ApplyRoomLayoutScale();
    }

    // Use the same 96-DPI layout as FrmSinhVien, including after a Designer save.
    // Assign absolute dimensions so repeated DPI events cannot accumulate scaling.
    private void ApplyRoomLayoutScale()
    {
        int Px(int value) => (int)Math.Round(value * DeviceDpi / 96.0);
        Padding Space(int left, int top, int right, int bottom) =>
            new Padding(Px(left), Px(top), Px(right), Px(bottom));

        tlpRoom.SuspendLayout();
        flpFilters.SuspendLayout();
        flpActions.SuspendLayout();
        try
        {
            tlpRoom.Padding = Space(20, 14, 20, 12);
            tlpRoom.RowStyles[0].Height = Px(58);
            tlpRoom.RowStyles[1].Height = Px(94);
            tlpRoom.RowStyles[3].Height = Px(56);
            lblTitle.Margin = Space(0, 0, 0, 6);
            grpFilters.Margin = Space(0, 0, 0, 14);
            grpFilters.Padding = Space(14, 8, 12, 10);
            flpFilters.Padding = Space(0, 3, 0, 0);
            lblToa.AutoSize = true;
            lblToa.MinimumSize = new Size(Px(44), Px(32));
            lblTrangThai.AutoSize = true;
            lblTrangThai.MinimumSize = new Size(Px(90), Px(32));
            cboToa.Width = Px(160);
            cboTrangThai.Width = Px(160);
            cboToa.Margin = cboTrangThai.Margin = Space(0, 3, 12, 0);
            txtMaPhong.Width = Px(200);
            txtMaPhong.Margin = Space(0, 3, 12, 0);
            btnTim.Size = new Size(Px(88), Px(34));
            pnlActions.Padding = Space(0, 12, 0, 0);
            btnThem.Size = new Size(Px(104), Px(36));
            btnSua.Size = btnXoa.Size = new Size(Px(90), Px(36));
            btnDong.Size = new Size(Px(84), Px(36));
            btnThem.Margin = btnSua.Margin = btnXoa.Margin = btnDong.Margin = Space(9, 0, 0, 0);
            lblRecordCount.Width = Px(160);
        }
        finally
        {
            flpActions.ResumeLayout(true);
            flpFilters.ResumeLayout(true);
            tlpRoom.ResumeLayout(true);
        }
    }
    private readonly string _connectionString;

    private bool _updatingFilters;
    private List<RoomListRow> _rooms = new();
    private sealed record RoomListRow(string MaPhong, string MaToa, int Tang, int ChoTrong, int TongCho, string TrangThai);

    private Task? _loadTask;
    private bool _loadSucceeded;
    private string? _exactRoomCode;
    private int _roomNavigationVersion;

    public Task RefreshViewAsync()
    {
        if (_loadTask is { IsCompleted: false }) return _loadTask;
        _loadTask = LoadRoomsAsync();
        return _loadTask;
    }

    public async Task SelectRoomAsync(string roomCode)
    {
        var version = ++_roomNavigationVersion;
        await RefreshViewAsync();
        if (IsDisposed || Disposing || !_loadSucceeded || version != _roomNavigationVersion) return;
        _updatingFilters = true;
        try
        {
            cboToa.SelectedIndex = 0;
            cboTrangThai.SelectedIndex = 0;
            _exactRoomCode = roomCode;
            txtMaPhong.Text = roomCode;
        }
        finally { _updatingFilters = false; }
        ApplyFilters();
        var selectedRow = dgvPhong.Rows.Cast<DataGridViewRow>()
            .FirstOrDefault(row => string.Equals(Convert.ToString(row.Cells["MaPhong"].Value),
                roomCode, StringComparison.OrdinalIgnoreCase));
        if (selectedRow is null)
        {
            MessageBox.Show(this, $"Phòng {roomCode} không còn trong database. Hãy làm mới sơ đồ phòng.",
                "Quản lý phòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        dgvPhong.ClearSelection();
        dgvPhong.CurrentCell = selectedRow.Cells["MaPhong"];
        selectedRow.Selected = true;
        dgvPhong.FirstDisplayedScrollingRowIndex = selectedRow.Index;
        dgvPhong.Focus();
    }


    private async Task LoadRoomsAsync()
    {

        _loadSucceeded = false;
        btnTim.Enabled = false;
        UseWaitCursor = true;
        lblRecordCount.Text = "Đang tải...";
        try
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("Chưa có chuỗi kết nối database.");
            await using var db = AppDbContextFactory.Create(_connectionString);
            await AuthPolicy.RequireAsync(db, _session, AppPermission.ManageRooms);
            var rooms = await db.Phongs.AsNoTracking()
                .OrderBy(p => p.MaToa).ThenBy(p => p.Tang).ThenBy(p => p.MaPhong)
                .Select(p => new RoomListRow(p.MaPhong, p.MaToa, p.Tang,
                    p.Giuongs.Count(g => g.TrangThai == "Trống"), p.SoChoToiDa, p.TrangThai))
                .ToListAsync();
            if (IsDisposed || Disposing) return;
            _rooms = rooms;
            _loadSucceeded = true;
            _updatingFilters = true;
            try
            {
                cboToa.Items.Clear();
                cboToa.Items.Add("Tất cả");
                cboToa.Items.AddRange(_rooms.Select(r => r.MaToa).Distinct().OrderBy(x => x).Cast<object>().ToArray());
                cboToa.SelectedIndex = 0;
                cboTrangThai.SelectedIndex = 0;
                txtMaPhong.Clear();
            }
            finally { _updatingFilters = false; }
            ApplyFilters();
        }
        catch (Exception ex)
        {
            if (IsDisposed || Disposing) return;
            _rooms.Clear();
            dgvPhong.Rows.Clear();
            UpdateCount();
            lblRecordCount.Text = "Lỗi tải dữ liệu";
            MessageBox.Show(this, "Không tải được danh sách phòng từ database.\r\n" + ex.Message,
                "Quản lý phòng", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {

            if (!IsDisposed && !Disposing) { btnTim.Enabled = true; UseWaitCursor = false; }
        }
    }


    private void Find_Click(object? sender, EventArgs e) => ApplyFilters();
    private void Filter_Changed(object? sender, EventArgs e) => ApplyFilters();
    private void Search_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        if (_updatingFilters) return;
        var search = txtMaPhong.Text.Trim();
        if (!string.Equals(search, _exactRoomCode, StringComparison.OrdinalIgnoreCase)) _exactRoomCode = null;
        var tower = cboToa.SelectedItem?.ToString();
        var status = cboTrangThai.SelectedItem?.ToString();
        var rows = _rooms.Where(r =>
            (search.Length == 0 || (_exactRoomCode is not null
                ? string.Equals(r.MaPhong, _exactRoomCode, StringComparison.OrdinalIgnoreCase)
                : r.MaPhong.Contains(search, StringComparison.OrdinalIgnoreCase))) &&
            (string.IsNullOrWhiteSpace(tower) || tower == "Tất cả" || r.MaToa == tower) &&
            (string.IsNullOrWhiteSpace(status) || status == "Tất cả" || r.TrangThai == status));
        dgvPhong.SuspendLayout();
        try
        {
            dgvPhong.Rows.Clear();
            foreach (var room in rows)
                dgvPhong.Rows.Add(room.MaPhong, room.MaToa, room.Tang,
                    $"{room.ChoTrong}/{room.TongCho}", room.TrangThai);
        }
        finally { dgvPhong.ResumeLayout(); }
        UpdateCount();
    }


    private void UpdateCount()
    {
        lblRecordCount.Text = $"{VisibleRoomCount} phòng";
        RecordCountChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Close_Click(object? sender, EventArgs e) => ReturnHomeRequested?.Invoke(this, EventArgs.Empty);
    private void FormatStatusCell(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || dgvPhong.Columns[e.ColumnIndex].Name != "TrangThai") return;
        var status = Convert.ToString(e.Value);
        e.CellStyle.BackColor = status switch
        {
            "Còn chỗ" => Color.FromArgb(232, 246, 234),
            "Đã đầy" => Color.FromArgb(254, 236, 238),
            "Đang sửa chữa" => Color.FromArgb(255, 244, 224),
            _ => Color.White
        };
        e.CellStyle.ForeColor = status switch
        {
            "Còn chỗ" => Color.FromArgb(53, 104, 63),
            "Đã đầy" => Color.FromArgb(169, 61, 65),
            "Đang sửa chữa" => Color.FromArgb(170, 104, 25),
            _ => Color.FromArgb(45, 51, 59)
        };
        e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
        e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
    }
}


