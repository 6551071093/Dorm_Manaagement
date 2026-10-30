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
    private readonly Button _assets = new() { Text = "Tài sản phòng", AutoSize = true };
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
        _assets.Click += async (_, _) => await ManageAssetsAsync();
        _assets.Visible = AuthPolicy.Can(_session, AppPermission.ManageRooms);
        _grid.SelectionChanged += (_, _) => UpdateEditor();
        _grid.DataBindingComplete += (_, _) => SetGridHeaders();
        var refresh = Button("Làm mới", async () => await ReloadAsync());
        var close = Button("Đóng", () => ReturnHomeRequested?.Invoke(this, EventArgs.Empty));
        toolbar.Controls.AddRange([_search, find, _status, _save, _add, _delete, _assets, refresh, close]);

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

    private async Task ManageAssetsAsync()
    {
        if (_grid.CurrentRow?.DataBoundItem is not BedManagementRow bed) return;
        using var dialog = new RoomAssetsDialog(_connectionString, _session, bed.RoomId);
        dialog.ShowDialog(this);
    }

    private void ShowError(Exception ex) => MessageBox.Show(this, ex.Message, "Giường", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}

internal sealed class RoomAssetsDialog : Form
{
    private readonly string _connectionString;
    private readonly AuthSession? _session;
    private readonly string _roomId;
    private int? _selectedAssetId;
    private readonly DataGridView _grid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AutoGenerateColumns = true,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false
    };
    private readonly TextBox _name = new() { Width = 210, PlaceholderText = "Tên tài sản" };
    private readonly NumericUpDown _quantity = new() { Width = 90, Minimum = 1, Maximum = 10000, Value = 1, ThousandsSeparator = true };
    private readonly ComboBox _condition = new() { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _note = new() { Width = 230, MaxLength = 255, PlaceholderText = "Ghi chú" };
    private RoomAssetHandoverService Service => new(() => QuanLyKyTucXa.Infrastructure.Data.Context.AppDbContextFactory.Create(_connectionString), _session);

    public RoomAssetsDialog(string connectionString, AuthSession? session, string roomId)
    {
        _connectionString = connectionString;
        _session = session;
        _roomId = roomId;
        Text = $"Tài sản phòng {_roomId}";
        Width = 950;
        Height = 560;
        StartPosition = FormStartPosition.CenterParent;
        _condition.Items.AddRange(["Tốt", "Hư hỏng", "Bảo trì"]);
        _condition.SelectedIndex = 0;
        _grid.DataBindingComplete += (_, _) => SetHeaders();
        _grid.SelectionChanged += (_, _) => SelectCurrent();

        var fields = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(8) };
        fields.Controls.AddRange([new Label { Text = "Tên", AutoSize = true, Padding = new Padding(0, 7, 0, 0) }, _name,
            new Label { Text = "Số lượng", AutoSize = true, Padding = new Padding(0, 7, 0, 0) }, _quantity, _condition, _note]);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, Padding = new Padding(8) };
        var add = Button("Thêm mới", ResetFields);
        var save = Button("Lưu", async () => await SaveAsync());
        var delete = Button("Xóa", async () => await DeleteAsync());
        var refresh = Button("Làm mới", async () => await ReloadAsync());
        var close = Button("Đóng", Close);
        add.Visible = save.Visible = AuthPolicy.Can(_session, AppPermission.ConfigureRooms);
        delete.Visible = AuthPolicy.Can(_session, AppPermission.DeleteRooms);
        actions.Controls.AddRange([add, save, delete, refresh, close]);
        Controls.Add(_grid);
        Controls.Add(fields);
        Controls.Add(actions);
        Load += async (_, _) => await ReloadAsync();
    }

    private static Button Button(string text, Action action)
    {
        var button = new Button { Text = text, AutoSize = true };
        button.Click += (_, _) => action();
        return button;
    }

    private void SetHeaders()
    {
        Header("AssetId", "Mã"); Header("RoomId", "Phòng"); Header("Name", "Tên tài sản");
        Header("Quantity", "Số lượng"); Header("Condition", "Tình trạng"); Header("Note", "Ghi chú");
    }

    private void Header(string key, string value)
    {
        if (_grid.Columns[key] is { } column) column.HeaderText = value;
    }

    private async Task ReloadAsync()
    {
        try { _grid.DataSource = await Service.GetRoomAssetsAsync(_roomId); }
        catch (Exception ex) { ShowError(ex); }
    }

    private void SelectCurrent()
    {
        if (_grid.CurrentRow?.DataBoundItem is not RoomAssetRow row) return;
        _selectedAssetId = row.AssetId;
        _name.Text = row.Name;
        _quantity.Value = row.Quantity;
        _condition.SelectedItem = row.Condition;
        _note.Text = row.Note ?? "";
    }

    private void ResetFields()
    {
        _selectedAssetId = null;
        _grid.ClearSelection();
        _name.Clear();
        _quantity.Value = 1;
        _condition.SelectedItem = "Tốt";
        _note.Clear();
        _name.Focus();
    }

    private async Task SaveAsync()
    {
        try
        {
            await Service.SaveRoomAssetAsync(new RoomAssetRow(_selectedAssetId ?? 0, _roomId, _name.Text,
                (int)_quantity.Value, _condition.Text, _note.Text), _selectedAssetId is null);
            await ReloadAsync();
            ResetFields();
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task DeleteAsync()
    {
        if (_selectedAssetId is not int id) return;
        if (MessageBox.Show(this, $"Xóa tài sản '{_name.Text}' khỏi phòng {_roomId}? Biên bản cũ vẫn được lưu.",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
        try { await Service.DeleteRoomAssetAsync(id, _roomId); await ReloadAsync(); ResetFields(); }
        catch (Exception ex) { ShowError(ex); }
    }

    private void ShowError(Exception ex) => MessageBox.Show(this, ex.Message, "Tài sản phòng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
