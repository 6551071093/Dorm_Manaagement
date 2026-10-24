using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public sealed partial class FrmDangKyKtx : Form
{
    public event EventHandler? ReturnHomeRequested;

    private void Close_Click(object? sender, EventArgs e)
    {
        if (_submitting) return;
        if (ReturnHomeRequested is null) Close();
        else ReturnHomeRequested.Invoke(this, EventArgs.Empty);
    }

    protected override void OnLoad(EventArgs e)
    {

        base.OnLoad(e);
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);

    }
    private void ApplyClassicLayoutScale()
    {
        int Px(int value) => (int)Math.Round(value * DeviceDpi / 96.0);
        Padding Space(int l, int t, int r, int b) => new(Px(l), Px(t), Px(r), Px(b));
        tlpRoot.SuspendLayout();
        tlpFields.SuspendLayout();
        try
        {
            tlpRoot.Padding = Space(20, 14, 20, 12);
            tlpRoot.RowStyles[0].Height = Px(58);
            tlpRoot.RowStyles[1].Height = Px(52);
            tlpRoot.RowStyles[3].Height = Px(56);
            lblTitle.Margin = Space(0, 0, 0, 6);
            lblNotice.Margin = Space(0, 0, 0, 8);
            lblNotice.Padding = Space(12, 0, 12, 0);
            grpRegistration.Padding = Space(14, 8, 12, 10);
            tlpFields.Padding = Space(0, 8, 12, 8);
            tlpFields.MaximumSize = new Size(Px(1000), 0);
            tlpFields.ColumnStyles[0].Width = Px(160);
            for (var row = 0; row < 4; row++) tlpFields.RowStyles[row].Height = Px(48);
            tlpFields.RowStyles[4].Height = Px(112);
            foreach (Control control in new Control[] { cboSinhVien, cboPhong, cboHocKy, txtNamHoc, txtGhiChu })
                control.Margin = Space(0, 5, 0, 8);
            foreach (Control label in new Control[] { lblSinhVien, lblPhong, lblHocKy, lblNamHoc, lblGhiChu })
                label.Margin = Space(0, 0, 10, 0);
            lblGhiChu.Padding = Space(0, 7, 0, 0);
            cboHocKy.MaximumSize = new Size(Px(160), 0);
            txtNamHoc.MaximumSize = new Size(Px(200), 0);
            pnlFooter.Padding = Space(0, 12, 0, 0);
            btnGuiDon.Size = new Size(Px(152), Px(36));
            btnClose.Size = new Size(Px(84), Px(36));
            btnGuiDon.Margin = btnClose.Margin = Space(9, 0, 0, 0);
        }
        finally { tlpFields.ResumeLayout(true); tlpRoot.ResumeLayout(true); }
    }
    private readonly DangKyKtxService _service = null!;
    private Task? _refreshTask;
    private bool _loadingOptions;
    private bool _loadingRooms;
    private bool _submitting;
    private int _roomRequestVersion;

    public FrmDangKyKtx()
    {
        InitializeComponent();
        QuanLyKyTucXa.WinForms.Helpers.ModernDialog.Install(this, tlpFields, btnGuiDon, btnClose, lblMessage, lblNotice);
        cboSinhVien.FormattingEnabled = true;
        cboSinhVien.DropDownStyle = ComboBoxStyle.DropDownList;
        cboSinhVien.Format += (_, e) => { if (e.ListItem is SinhVien s) e.Value = $"{s.MaSv} — {s.HoTen}"; };
        cboPhong.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPhong.FormattingEnabled = true;
        cboPhong.Format += (_, e) => { if (e.ListItem is RoomChoice r) e.Value = r.MaPhong.Length == 0 ? "Không chọn phòng đề xuất" : $"{r.MaPhong} — {r.TenToa} (còn {r.ChoTrong} giường)"; };
        cboHocKy.Items.AddRange(new object[] { "1", "2", "3" });
        cboHocKy.SelectedIndex = 0;
        var year = DateTime.Today.Month >= 8 ? DateTime.Today.Year : DateTime.Today.Year - 1;
        txtNamHoc.Text = $"{year}-{year + 1}";
        txtNamHoc.MaxLength = 9;
        txtGhiChu.MaxLength = 255;
        cboSinhVien.SelectedIndexChanged += async (_, _) => { if (!_loadingOptions) await LoadRoomsAsync(); };
        btnGuiDon.Click += async (_, _) => await SubmitAsync();
        Load += async (_, _) => await RefreshViewAsync();
    }

    public FrmDangKyKtx(string connectionString, AuthSession? session = null) : this(new DangKyKtxService(() => Infrastructure.Data.Context.AppDbContextFactory.Create(connectionString), session)) { }
    public FrmDangKyKtx(DangKyKtxService service) : this() => _service = service;

    public Task RefreshViewAsync()
    {
        if (_service is null || _submitting) return Task.CompletedTask;
        if (_refreshTask is { IsCompleted: false }) return _refreshTask;
        return _refreshTask = LoadOptionsAsync();
    }

    private async Task LoadOptionsAsync()
    {
        _loadingOptions = true;
        tlpFields.Enabled = false;
        UpdateSubmitButton();
        var previousId = (cboSinhVien.SelectedItem as SinhVien)?.MaSv;
        try
        {
            var students = await Task.Run(() => _service.GetEligibleStudentsAsync());
            if (IsDisposed || Disposing) return;
            cboSinhVien.DisplayMember = nameof(SinhVien.HoTen);
            cboSinhVien.DataSource = students;
            var previous = students.FindIndex(s => s.MaSv == previousId);
            if (previous >= 0) cboSinhVien.SelectedIndex = previous;
            await LoadRoomsAsync();
            if (students.Count == 0) lblMessage.Text = "Chưa có sinh viên đủ điều kiện đăng ký.";
        }
        catch (Exception ex) { ShowFailure(ex, "Không tải được dữ liệu đăng ký."); }
        finally
        {
            _loadingOptions = false;
            if (!IsDisposed && !Disposing) { tlpFields.Enabled = !_submitting; UpdateSubmitButton(); }
        }
    }

    private async Task LoadRoomsAsync()
    {
        if (_service is null) return;
        var version = ++_roomRequestVersion;
        _loadingRooms = true;
        cboPhong.Enabled = false;
        UpdateSubmitButton();
        var studentId = (cboSinhVien.SelectedItem as SinhVien)?.MaSv;
        try
        {
            var rooms = studentId is null ? new List<RoomChoice>() : await Task.Run(() => _service.GetRoomsAsync(studentId));
            if (IsDisposed || Disposing || version != _roomRequestVersion) return;
            rooms.Insert(0, new RoomChoice("", "", 0, 0));
            cboPhong.DataSource = rooms;
        }
        catch (Exception ex)
        {
            if (version == _roomRequestVersion && !IsDisposed && !Disposing)
            {
                cboPhong.DataSource = null;
                ShowFailure(ex, "Không tải được danh sách phòng.");
            }
        }
        finally
        {
            if (version == _roomRequestVersion && !IsDisposed && !Disposing)
            {
                _loadingRooms = false;
                cboPhong.Enabled = true;
                UpdateSubmitButton();
            }
        }
    }

    private void UpdateSubmitButton()
    {
        btnGuiDon.Enabled = !_loadingOptions && !_loadingRooms && !_submitting && cboSinhVien.SelectedItem is SinhVien;
        btnClose.Enabled = !_submitting;
    }

    private async Task SubmitAsync()
    {
        if (_submitting || _loadingOptions || _loadingRooms || _service is null) return;
        if (cboSinhVien.SelectedItem is not SinhVien student) { MessageBox.Show(this, "Không có sinh viên đủ điều kiện đăng ký."); return; }
        _submitting = true;
        tlpFields.Enabled = false;
        UpdateSubmitButton();
        try
        {
            var room = cboPhong.SelectedItem as RoomChoice;
            var id = await _service.SubmitAsync(student.MaSv, room?.MaPhong,
                int.Parse(cboHocKy.SelectedItem!.ToString()!), txtNamHoc.Text, txtGhiChu.Text);
            lblMessage.Text = $"Đã lưu đơn #{id} — Chờ duyệt";
            txtGhiChu.Clear();
            MessageBox.Show(this, $"Đơn #{id} đang chờ xét duyệt.", "Gửi đơn thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadOptionsAsync();
        }
        catch (Exception ex) { ShowFailure(ex, "Không gửi được đơn. Hãy kiểm tra kết nối rồi thử lại."); }
        finally
        {
            _submitting = false;
            if (!IsDisposed && !Disposing) { tlpFields.Enabled = true; UpdateSubmitButton(); }
        }
    }

    private void ShowFailure(Exception ex, string fallback)
    {
        if (IsDisposed || Disposing) return;
        lblMessage.Text = "Thao tác chưa hoàn tất.";
        MessageBox.Show(this, ex is InvalidOperationException or UnauthorizedAccessException ? ex.Message : fallback,
            "Đăng ký KTX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
