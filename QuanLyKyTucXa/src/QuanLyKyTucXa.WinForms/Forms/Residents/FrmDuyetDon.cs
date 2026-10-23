using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public sealed partial class FrmDuyetDon : Form
{
    public event EventHandler? ReturnHomeRequested;

    private void Close_Click(object? sender, EventArgs e)
    {
        if (_operationBusy) return;
        if (ReturnHomeRequested is null) Close();
        else ReturnHomeRequested.Invoke(this, EventArgs.Empty);
    }

    protected override void OnLoad(EventArgs e)
    {
        ApplyClassicLayoutScale();
        base.OnLoad(e);
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        if (tlpRoot is not null) ApplyClassicLayoutScale();
    }
    private void ApplyClassicLayoutScale()
    {
        int Px(int value) => (int)Math.Round(value * DeviceDpi / 96.0);
        Padding Space(int l, int t, int r, int b) => new(Px(l), Px(t), Px(r), Px(b));
        tlpRoot.SuspendLayout();
        flpFilters.SuspendLayout();
        try
        {
            tlpRoot.Padding = Space(20, 14, 20, 12);
            tlpRoot.RowStyles[0].Height = Px(58);
            tlpRoot.RowStyles[1].Height = Px(94);
            tlpRoot.RowStyles[3].Height = Px(56);
            lblTitle.Margin = Space(0, 0, 0, 6);
            grpFilters.Margin = Space(0, 0, 0, 14);
            grpFilters.Padding = Space(14, 8, 12, 10);
            flpFilters.Padding = Space(0, 3, 0, 0);
            lblKeyword.MinimumSize = new Size(Px(72), Px(32));
            txtSearch.Width = Px(360);
            txtSearch.Margin = Space(0, 3, 12, 0);
            btnSearch.Size = new Size(Px(88), Px(34));
            btnSearch.Margin = Space(0, 0, 8, 0);
            pnlActions.Padding = Space(0, 12, 0, 0);
            btnClose.Size = new Size(Px(84), Px(36));
            btnClose.Margin = Space(9, 0, 0, 0);
            btnReload.Size = new Size(Px(104), Px(34));
            btnApprove.Size = new Size(Px(176), Px(36));
            btnReject.Size = new Size(Px(90), Px(36));
            btnApprove.Margin = btnReject.Margin = Space(9, 0, 0, 0);
        }
        finally { flpFilters.ResumeLayout(true); tlpRoot.ResumeLayout(true); }
    }
    private readonly DangKyKtxService _service = null!;
    private readonly System.Windows.Forms.Timer _searchTimer;
    private bool _operationBusy;
    private bool _loading;
    private int _searchVersion;

    public FrmDuyetDon()
    {
        InitializeComponent();
        components ??= new System.ComponentModel.Container();
        _searchTimer = new System.Windows.Forms.Timer(components) { Interval = 350 };
        _searchTimer.Tick += async (_, _) => { _searchTimer.Stop(); await LoadPendingAsync(); };
        txtSearch.TextChanged += (_, _) => {
            ++_searchVersion;
            _searchTimer.Stop();
            _searchTimer.Start();
        };
        Load += async (_, _) => await RefreshViewAsync();
        btnSearch.Click += async (_, _) => await RefreshViewAsync();
        txtSearch.KeyDown += async (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await RefreshViewAsync(); } };
        btnReload.Click += async (_, _) => await RefreshViewAsync();
        btnApprove.Click += async (_, _) => await RunOperationAsync(ApproveAsync);
        btnReject.Click += async (_, _) => await RunOperationAsync(RejectAsync);
        _grid.Columns["NgayDangKy"]!.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
    }
    public FrmDuyetDon(string connectionString, AuthSession? session = null) : this(new DangKyKtxService(() => Infrastructure.Data.Context.AppDbContextFactory.Create(connectionString), session)) { }
    public FrmDuyetDon(DangKyKtxService service) : this() => _service = service;

    public Task RefreshViewAsync() => _operationBusy ? Task.CompletedTask : LoadPendingAsync();

    private async Task LoadPendingAsync()
    {
        if (_service is null) return;
        _searchTimer.Stop();
        var version = ++_searchVersion;
        _loading = true;
        UpdateButtons();
        try
        {
            var rows = await _service.SearchAsync(txtSearch.Text, "Chờ duyệt");
            if (IsDisposed || Disposing || version != _searchVersion) return;
            _grid.DataSource = rows;
            lblSubtitle.Text = $"{rows.Count:N0} đơn đang chờ duyệt";
        }
        catch (Exception)
        {
            if (IsDisposed || Disposing || version != _searchVersion) return;
            _grid.DataSource = null;
            lblSubtitle.Text = "Không tải được đơn. Kiểm tra kết nối và bấm Làm mới.";
        }
        finally
        {
            if (!IsDisposed && !Disposing && version == _searchVersion) { _loading = false; UpdateButtons(); }
        }
    }

    private DangKyRow Selected() => _grid.CurrentRow?.DataBoundItem as DangKyRow
        ?? throw new InvalidOperationException("Chọn đơn cần xử lý.");

    private async Task ApproveAsync()
    {
        var row = Selected();
        var beds = await _service.GetAvailableBedsAsync(row.MaDangKy);
        if (beds.Count == 0) throw new InvalidOperationException("Không còn giường trống phù hợp. Đơn vẫn chờ duyệt.");
        using var dialog = new ApprovalDialog(row, beds);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        await _service.ApproveAsync(row.MaDangKy, dialog.Bed!.MaGiuong,
            DateOnly.FromDateTime(dialog.Start), DateOnly.FromDateTime(dialog.End), dialog.Deposit, null);
        MessageBox.Show(this, "Đã duyệt, lập hợp đồng và cập nhật trạng thái sinh viên, giường, phòng.",
            "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        await LoadPendingAsync();
    }

    private async Task RejectAsync()
    {
        var row = Selected();
        using var dialog = new ReasonDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        await _service.RejectAsync(row.MaDangKy, dialog.Reason, null);
        MessageBox.Show(this, "Đã từ chối đơn và lưu lý do.", "Duyệt đơn", MessageBoxButtons.OK, MessageBoxIcon.Information);
        await LoadPendingAsync();
    }

    private async Task RunOperationAsync(Func<Task> action)
    {
        if (_operationBusy || _loading || _service is null) return;
        _searchTimer.Stop();
        _operationBusy = true;
        UpdateButtons();
        txtSearch.Enabled = _grid.Enabled = false;
        try { await action(); }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException) { MessageBox.Show(this, ex.Message, "Duyệt đơn", MessageBoxButtons.OK, MessageBoxIcon.Warning); await LoadPendingAsync(); }
        catch (Exception) { MessageBox.Show(this, "Thao tác chưa hoàn tất. Kiểm tra kết nối và tải lại danh sách trước khi thử lại.", "Duyệt đơn", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        finally
        {
            _operationBusy = false;
            if (!IsDisposed && !Disposing) { txtSearch.Enabled = _grid.Enabled = true; UpdateButtons(); }
        }
    }

    private void UpdateButtons()
    {
        btnApprove.Enabled = btnReject.Enabled = !_operationBusy && !_loading;
        btnReload.Enabled = btnSearch.Enabled = btnClose.Enabled = !_operationBusy;
    }
}


