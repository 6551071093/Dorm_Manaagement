using QuanLyKyTucXa.Infrastructure.Security;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class FrmSinhVien : Form
{
    private readonly AuthSession? _session;
    public event EventHandler? ReturnHomeRequested;
    public FrmSinhVien() : this(string.Empty) { }

    public FrmSinhVien(string connectionString, AuthSession? session = null)
    {
        _connectionString = connectionString;
        _session = session;
        InitializeComponent();
        components ??= new System.ComponentModel.Container();
        ConfigureDatabaseEvents();
    }

    protected override void OnLoad(EventArgs e)
    {
        ApplyStudentLayoutScale();
        base.OnLoad(e);
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        if (tlpRoot is not null) ApplyStudentLayoutScale();
    }

    private void ApplyStudentLayoutScale()
    {
        int Px(int value) => (int)Math.Round(value * DeviceDpi / 96.0);
        Padding Space(int l, int t, int r, int b) => new(Px(l), Px(t), Px(r), Px(b));
        tlpRoot.SuspendLayout();
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
            lblKhoa.MinimumSize = new Size(Px(60), Px(32));
            lblLop.MinimumSize = new Size(Px(50), Px(32));
            cboKhoa.Width = Px(160);
            cboLop.Width = Px(120);
            cboKhoa.Margin = cboLop.Margin = txtTimKiem.Margin = Space(0, 3, 12, 0);
            txtTimKiem.Width = Px(200);
            btnTimKiem.Size = new Size(Px(88), Px(34));
            btnTimKiem.Margin = Space(0, 0, 8, 0);
            btnXuatExcel.Size = new Size(Px(96), Px(34));
            btnNhapExcel.Size = new Size(Px(104), Px(34));
            btnTaiMau.Size = new Size(Px(112), Px(34));
            btnNhapExcel.Margin = btnTaiMau.Margin = Space(8, 0, 0, 0);
            pnlActions.Padding = Space(0, 12, 0, 0);
            btnThem.Size = new Size(Px(104), Px(36));
            btnSua.Size = btnXoa.Size = new Size(Px(90), Px(36));
            btnDong.Size = new Size(Px(84), Px(36));
            btnThem.Margin = btnSua.Margin = btnXoa.Margin = btnDong.Margin = Space(9, 0, 0, 0);
            lblRecordCount.Width = Px(160);
        }
        finally { tlpRoot.ResumeLayout(true); }
    }
    private void Close_Click(object? sender, EventArgs e) =>
        ReturnHomeRequested?.Invoke(this, EventArgs.Empty);

    private void FormatStudentStatusCell(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || dgvSinhVien.Columns[e.ColumnIndex].DataPropertyName != "TrangThai") return;
        var status = Convert.ToString(e.Value);
        e.CellStyle.BackColor = status switch
        {
            "Đang ở" => Color.FromArgb(232, 246, 234),
            "Chờ xếp phòng" => Color.FromArgb(255, 244, 224),
            "Đã rời KTX" => Color.FromArgb(237, 240, 245),
            _ => Color.White
        };
        e.CellStyle.ForeColor = status switch
        {
            "Đang ở" => Color.FromArgb(53, 104, 63),
            "Chờ xếp phòng" => Color.FromArgb(170, 104, 25),
            _ => Color.FromArgb(100, 112, 128)
        };
        e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
        e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
    }
}

