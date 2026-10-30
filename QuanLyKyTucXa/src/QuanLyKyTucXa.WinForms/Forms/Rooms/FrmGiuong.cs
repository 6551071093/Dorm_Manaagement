using System.Drawing;
using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Rooms;

public sealed class FrmGiuong : Form
{
    private readonly string _connectionString;
    private readonly AuthSession? _session;
    private readonly DataGridView _grid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AutoGenerateColumns = true,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false
    };
    private readonly FlowLayoutPanel _roomMap = new()
    {
        Dock = DockStyle.Top,
        Height = 250,
        AutoScroll = true,
        WrapContents = true,
        Padding = new Padding(8),
        BackColor = Color.FromArgb(246, 248, 251)
    };
    private readonly TextBox _search = new() { Width = 220, PlaceholderText = "Mã giường, phòng, tòa" };
    private readonly ComboBox _status = new() { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Button _save = new() { Text = "Cập nhật trạng thái", AutoSize = true };
    private readonly Button _add = new() { Text = "Thêm giường", AutoSize = true };
    private readonly Button _delete = new() { Text = "Xóa giường", AutoSize = true };
    private readonly ToolTip _bedToolTip = new();

    public event EventHandler? ReturnHomeRequested;

    private RoomOperationsService Service => new(
        () => QuanLyKyTucXa.Infrastructure.Data.Context.AppDbContextFactory.Create(_connectionString), _session);

    public FrmGiuong(string connectionString, AuthSession? session)
    {
        _connectionString = connectionString;
        _session = session;
        Text = "Quản lý giường";
        Width = 1100;
        Height = 720;
        StartPosition = FormStartPosition.CenterParent;
        _status.Items.AddRange(["Trống", "Hỏng", "Bảo trì"]);
        _status.SelectedIndex = 0;
        BuildUi();
        Load += async (_, _) => await ReloadAsync();
    }

    public Task RefreshViewAsync() => ReloadAsync();

    private void BuildUi()
    {
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(8) };
        var find = Button("Tìm", async () => await ReloadAsync());
        _search.KeyDown += async (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            await ReloadAsync();
        };
        _save.Click += async (_, _) => await SaveAsync();
        _save.Visible = AuthPolicy.Can(_session, AppPermission.ConfigureRooms);
        _add.Click += async (_, _) => await AddAsync();
        _add.Visible = AuthPolicy.Can(_session, AppPermission.ConfigureRooms);
        _delete.Click += async (_, _) => await DeleteAsync();
        _delete.Visible = AuthPolicy.Can(_session, AppPermission.DeleteRooms);
        _grid.SelectionChanged += (_, _) => UpdateEditor();
        _grid.DataBindingComplete += (_, _) => SetGridHeaders();
        var refresh = Button("Làm mới", async () => await ReloadAsync());
        var close = Button("Đóng", () => ReturnHomeRequested?.Invoke(this, EventArgs.Empty));
        toolbar.Controls.AddRange([_search, find, _status, _save, _add, _delete, refresh, close]);

        var legend = new Label
        {
            Dock = DockStyle.Top,
            Height = 30,
            Padding = new Padding(10, 7, 0, 0),
            Text = "Sơ đồ giường  •  Xanh: trống     Đỏ: đang sử dụng     Cam: hỏng     Xám: bảo trì"
        };
        Controls.Add(_grid);
        Controls.Add(_roomMap);
        Controls.Add(legend);
        Controls.Add(toolbar);
    }

    private static Button Button(string text, Action action)
    {
        var button = new Button { Text = text, AutoSize = true };
        button.Click += (_, _) => action();
        return button;
    }

    private void SetGridHeaders()
    {
        SetHeader("BedId", "Mã giường");
        SetHeader("RoomId", "Mã phòng");
        SetHeader("TowerName", "Tòa nhà");
        SetHeader("BedName", "Tên giường");
        SetHeader("Position", "Vị trí");
        SetHeader("Status", "Trạng thái");
    }

    private void SetHeader(string property, string title)
    {
        if (_grid.Columns[property] is { } column) column.HeaderText = title;
    }

    private async Task ReloadAsync()
    {
        try
        {
            var beds = await Service.GetBedsAsync(_search.Text);
            _grid.DataSource = beds;
            RenderRoomMap(beds);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Giường", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void RenderRoomMap(List<BedManagementRow> beds)
    {
        _roomMap.SuspendLayout();
        try
        {
            foreach (Control control in _roomMap.Controls) control.Dispose();
            _roomMap.Controls.Clear();
            foreach (var room in beds.GroupBy(x => new { x.RoomId, x.TowerName })
                         .OrderBy(x => x.Key.TowerName).ThenBy(x => x.Key.RoomId))
            {
                var card = new GroupBox
                {
                    Text = $"Phòng {room.Key.RoomId}  •  {room.Key.TowerName}",
                    Width = 350,
                    Height = 215,
                    Margin = new Padding(5),
                    Padding = new Padding(7)
                };
                var bedsPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    WrapContents = true,
                    Padding = new Padding(2)
                };
                foreach (var bed in room.OrderBy(x => x.BedId))
                {
                    var color = bed.Status switch
                    {
                        "Trống" => Color.FromArgb(211, 239, 218),
                        "Đang sử dụng" => Color.FromArgb(250, 214, 216),
                        "Hỏng" => Color.FromArgb(255, 226, 194),
                        "Bảo trì" => Color.FromArgb(224, 228, 233),
                        _ => Color.White
                    };
                    var tile = new Button
                    {
                        Text = $"{bed.BedName}\n{bed.Status}",
                        Tag = bed.BedId,
                        Width = 132,
                        Height = 48,
                        Margin = new Padding(3),
                        BackColor = color,
                        UseVisualStyleBackColor = false,
                        FlatStyle = FlatStyle.Flat,
                        AccessibleName = $"{bed.RoomId}, {bed.BedName}, {bed.Status}"
                    };
                    tile.FlatAppearance.BorderColor = Color.FromArgb(185, 194, 205);
                    tile.Click += (_, _) => SelectBed(bed.BedId);
                    _bedToolTip.SetToolTip(tile, $"{bed.BedId} — {bed.RoomId} — {bed.Status}");
                    bedsPanel.Controls.Add(tile);
                }
                card.Controls.Add(bedsPanel);
                _roomMap.Controls.Add(card);
            }
        }
        finally
        {
            _roomMap.ResumeLayout(true);
        }
    }

    private void SelectBed(string bedId)
    {
        foreach (DataGridViewRow row in _grid.Rows)
        {
            if (row.DataBoundItem is not BedManagementRow bed || bed.BedId != bedId) continue;
            _grid.ClearSelection();
            _grid.CurrentCell = row.Cells[0];
            row.Selected = true;
            _grid.FirstDisplayedScrollingRowIndex = row.Index;
            _grid.Focus();
            return;
        }
    }

    private void UpdateEditor()
    {
        var bed = _grid.CurrentRow?.DataBoundItem as BedManagementRow;
        _save.Enabled = bed is not null && bed.Status != "Đang sử dụng";
        _add.Enabled = bed is not null;
        _delete.Enabled = bed?.Status == "Trống";
        if (bed is not null && _status.Items.Contains(bed.Status)) _status.SelectedItem = bed.Status;
    }

    private async Task AddAsync()
    {
        if (_grid.CurrentRow?.DataBoundItem is not BedManagementRow bed) return;
        try
        {
            await Service.AddBedAsync(bed.RoomId);
            await ReloadAsync();
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task DeleteAsync()
    {
        if (_grid.CurrentRow?.DataBoundItem is not BedManagementRow bed) return;
        if (MessageBox.Show(this, $"Xóa giường {bed.BedId}? Chỉ xóa được giường trống chưa có lịch sử hợp đồng.",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
        try
        {
            await Service.DeleteBedAsync(bed.BedId);
            await ReloadAsync();
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task SaveAsync()
    {
        if (_grid.CurrentRow?.DataBoundItem is not BedManagementRow bed) return;
        try
        {
            await Service.SetBedConditionAsync(bed.BedId, _status.Text);
            await ReloadAsync();
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void ShowError(Exception ex) => MessageBox.Show(this, ex.Message, "Giường", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
