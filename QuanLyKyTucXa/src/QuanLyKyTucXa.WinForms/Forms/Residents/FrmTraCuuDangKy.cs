using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public sealed partial class FrmTraCuuDangKy : Form
{
    private void FormatApplicationStatusCell(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0 || dgvDon.Columns[e.ColumnIndex].DataPropertyName != "TrangThai") return;
        var status = Convert.ToString(e.Value);
        e.CellStyle.BackColor = status switch {
            "Đã duyệt" => Color.FromArgb(232, 246, 234),
            "Chờ duyệt" => Color.FromArgb(255, 244, 224),
            "Từ chối" => Color.FromArgb(254, 236, 238),
            _ => Color.FromArgb(237, 240, 245)
        };
        e.CellStyle.ForeColor = status switch {
            "Đã duyệt" => Color.FromArgb(53, 104, 63),
            "Chờ duyệt" => Color.FromArgb(170, 104, 25),
            "Từ chối" => Color.FromArgb(169, 61, 65),
            _ => Color.FromArgb(100, 112, 128)
        };
        e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
        e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
    }

    public event EventHandler? ReturnHomeRequested;

    private void Close_Click(object? sender, EventArgs e)
    {

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
        tlpFilters.SuspendLayout();
        try
        {
            tlpRoot.Padding = Space(20, 14, 20, 12);
            tlpRoot.RowStyles[0].Height = Px(58);
            tlpRoot.RowStyles[1].Height = Px(94);
            tlpRoot.RowStyles[3].Height = Px(56);
            lblTitle.Margin = Space(0, 0, 0, 6);
            grpFilters.Margin = Space(0, 0, 0, 14);
            grpFilters.Padding = Space(14, 8, 12, 10);
            tlpFilters.Padding = Space(0, 3, 0, 0);
            lblKeyword.MinimumSize = new Size(Px(72), Px(32));
            txtSearch.Width = Px(360);
            txtSearch.Margin = Space(0, 3, 12, 0);
            btnSearch.Size = new Size(Px(88), Px(34));
            btnSearch.Margin = Space(0, 0, 8, 0);
            pnlFooter.Padding = Space(0, 12, 0, 0);
            btnClose.Size = new Size(Px(84), Px(36));
            btnClose.Margin = Space(9, 0, 0, 0);
            lblStatus.MinimumSize = new Size(Px(90), Px(32));
            cboStatus.Width = Px(180);
            cboStatus.Margin = Space(0, 3, 12, 0);
        }
        finally { tlpFilters.ResumeLayout(true); tlpRoot.ResumeLayout(true); }
    }
    private readonly DangKyKtxService _service = null!;
    private int _searchVersion;

    public FrmTraCuuDangKy()
    {
        InitializeComponent();
        cboStatus.Items.AddRange(new object[] { "Tất cả trạng thái", "Chờ duyệt", "Đã duyệt", "Từ chối", "Đã hủy" });
        cboStatus.SelectedIndex = 0;
        btnSearch.Click += async (_, _) => await RefreshViewAsync();
        cboStatus.SelectedIndexChanged += async (_, _) => await RefreshViewAsync();
        txtSearch.KeyDown += async (_, e) => {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await RefreshViewAsync(); }
        };
        dgvDon.CellFormatting += FormatApplicationStatusCell;
        dgvDon.Columns["NgayDangKy"]!.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
        Load += async (_, _) => await RefreshViewAsync();
    }
    public FrmTraCuuDangKy(string connectionString, AuthSession? session = null) : this(new DangKyKtxService(() => Infrastructure.Data.Context.AppDbContextFactory.Create(connectionString), session)) { }
    public FrmTraCuuDangKy(DangKyKtxService service) : this() => _service = service;

    public async Task RefreshViewAsync()
    {
        if (_service is null) return;
        var version = ++_searchVersion;
        var keyword = txtSearch.Text;
        var status = cboStatus.SelectedItem?.ToString();
        btnSearch.Enabled = false;
        lblCount.Text = "Đang tải...";
        try
        {
            var rows = await _service.SearchAsync(keyword, status);
            if (IsDisposed || Disposing || version != _searchVersion) return;
            dgvDon.DataSource = rows;
            lblCount.Text = $"Tìm thấy {rows.Count:N0} đơn đăng ký";
        }
        catch (Exception)
        {
            if (IsDisposed || Disposing || version != _searchVersion) return;
            dgvDon.DataSource = null;
            lblCount.Text = "Không tải được dữ liệu.";
            MessageBox.Show(this, "Không tra cứu được đơn. Kiểm tra kết nối rồi thử lại.", "Tra cứu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally { if (!IsDisposed && !Disposing && version == _searchVersion) btnSearch.Enabled = true; }
    }
}

