using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Users;

public partial class FrmNguoiDung : Form
{
    private readonly NguoiDungService? _service;
    private List<UserAccountRow> _users = [];
    private bool _busy;
    public event EventHandler? ReturnHomeRequested;
    public FrmNguoiDung()
    {
        InitializeComponent();
        cboRole.Items.AddRange(new object[] { "Tất cả vai trò", "Admin", "NhanVien", "SinhVien" });
        cboRole.SelectedIndex = 0;
        txtSearch.TextChanged += (_, _) => ApplyFilter();
        cboRole.SelectedIndexChanged += (_, _) => ApplyFilter();
        dgvUsers.SelectionChanged += (_, _) => UpdateToggle();
        dgvUsers.CellFormatting += (_, e) => {
            if (e.RowIndex < 0) return;
            var property = dgvUsers.Columns[e.ColumnIndex].DataPropertyName;
            if (property == "Role") e.Value = e.Value?.ToString() switch { "Admin" => "Quản trị viên", "NhanVien" => "Nhân viên", "SinhVien" => "Sinh viên", _ => e.Value };
            if (property == "Enabled" && e.Value is bool enabled) e.Value = enabled ? "Hoạt động" : "Đã khóa";
        };
        Load += async (_, _) => await RefreshViewAsync();
    }
    public FrmNguoiDung(string connectionString, AuthSession session) : this() =>
        _service = new NguoiDungService(() => AppDbContextFactory.Create(connectionString), session);

    public async Task RefreshViewAsync()
    {
        if (_busy || _service is null) return;
        await RunAsync(async () => { _users = await _service.ListAsync(); ApplyFilter(); });
    }
    private void ApplyFilter()
    {
        var key = txtSearch.Text.Trim();
        dgvUsers.DataSource = _users.Where(x => (cboRole.SelectedIndex <= 0 || x.Role == cboRole.Text)
            && (key.Length == 0 || x.Username.Contains(key, StringComparison.OrdinalIgnoreCase)
                || x.FullName.Contains(key, StringComparison.OrdinalIgnoreCase))).ToList();
        lblCount.Text = $"{dgvUsers.Rows.Count:N0} tài khoản";
    }
    private UserAccountRow Selected() => dgvUsers.CurrentRow?.DataBoundItem as UserAccountRow
        ?? throw new InvalidOperationException("Chọn tài khoản trong danh sách.");
    private void UpdateToggle() => btnToggle.Text = dgvUsers.CurrentRow?.DataBoundItem is UserAccountRow { Enabled: false } ? "Mở khóa" : "Khóa";
    private async void Add_Click(object? sender, EventArgs e) => await EditAsync(null);
    private async void Edit_Click(object? sender, EventArgs e)
    {
        if (_busy) return;
        try { await EditAsync(Selected().Id); } catch (InvalidOperationException ex) { MessageBox.Show(this, ex.Message); }
    }
    private async Task EditAsync(int? id)
    {
        if (_busy || _service is null) return;
        await RunAsync(async () => {
            var value = id is null ? new UserAccountEdit() : await _service.GetAsync(id.Value);
            var choices = await _service.StudentChoicesAsync(id);
            using var dialog = new UserAccountEditDialog(value, choices, _service);
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            _users = await _service.ListAsync(); ApplyFilter();
        });
    }
    private async void Toggle_Click(object? sender, EventArgs e)
    {
        if (_busy || _service is null) return;
        await RunAsync(async () => {
            var selected = Selected();
            if (MessageBox.Show(this, $"{(selected.Enabled ? "Khóa" : "Mở khóa")} tài khoản {selected.Username}?",
                "Quản lý tài khoản", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
            var value = await _service.GetAsync(selected.Id);
            value.Enabled = !value.Enabled;
            await _service.SaveAsync(value);
            _users = await _service.ListAsync(); ApplyFilter();
        });
    }
    private async void Refresh_Click(object? sender, EventArgs e) => await RefreshViewAsync();
    private async Task RunAsync(Func<Task> action)
    {
        if (_busy) return;
        _busy = true; grpFilters.Enabled = dgvUsers.Enabled = flpActions.Enabled = false;
        try { await action(); }
        catch (Exception ex) { MessageBox.Show(this, ex is InvalidOperationException or UnauthorizedAccessException ? ex.Message : "Không xử lý được tài khoản. Kiểm tra kết nối và thử lại.", "Tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        finally { _busy = false; if (!IsDisposed) grpFilters.Enabled = dgvUsers.Enabled = flpActions.Enabled = true; }
    }
    private void Close_Click(object? sender, EventArgs e) { if (_busy) return; if (ReturnHomeRequested is null) Close(); else ReturnHomeRequested.Invoke(this, EventArgs.Empty); }
}