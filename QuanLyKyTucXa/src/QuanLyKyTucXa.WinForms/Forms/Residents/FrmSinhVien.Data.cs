using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class FrmSinhVien
{
    private readonly string _connectionString;
    private bool _loadingData;
    private bool _updatingFilters;
    private List<StudentListRow> _students = new();
    public event EventHandler? RecordCountChanged;
    public int VisibleStudentCount => dgvSinhVien.Rows.Count;
    private sealed record StudentListRow(string MaSv, string HoTen, DateOnly NgaySinh,
        string GioiTinh, string Cccd, string SoDienThoai, string Lop, string Khoa, string TrangThai);

    private void ConfigureDatabaseEvents()
    {
        dgvSinhVien.AutoGenerateColumns = false;
        dgvSinhVien.Columns[2].DefaultCellStyle.Format = "dd/MM/yyyy";
        cboKhoa.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLop.DropDownStyle = ComboBoxStyle.DropDownList;
        cboKhoa.SelectedIndex = 0;
        cboLop.SelectedIndex = 0;
        Load += async (_, _) => await RefreshViewAsync();
        btnTimKiem.Click += (_, _) => ApplyStudentFilters();
        cboKhoa.SelectedIndexChanged += (_, _) =>
        {
            if (_updatingFilters) return;
            PopulateClasses();
            ApplyStudentFilters();
        };
        cboLop.SelectedIndexChanged += (_, _) => ApplyStudentFilters();
        txtTimKiem.KeyDown += (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            ApplyStudentFilters();
        };
        btnXuatExcel.Click += ExportStudents_Click;
        btnThem.Click += AddStudent_Click;
        btnSua.Click += EditStudent_Click;
        btnXoa.Click += DeleteStudent_Click;
    }

    public async Task RefreshViewAsync()
    {
        if (_loadingData) return;
        _loadingData = true;
        UseWaitCursor = true;
        btnTimKiem.Enabled = false;
        lblRecordCount.Text = "Đang tải...";
        try
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("Chưa có chuỗi kết nối database.");
            await using var db = AppDbContextFactory.Create(_connectionString);
            var students = await db.SinhViens.AsNoTracking().OrderBy(s => s.MaSv)
                .Select(s => new StudentListRow(s.MaSv, s.HoTen, s.NgaySinh, s.GioiTinh,
                    s.Cccd, s.SoDienThoai, s.Lop, s.Khoa, s.TrangThai))
                .ToListAsync();
            if (IsDisposed || Disposing) return;
            _students = students;
            _updatingFilters = true;
            try
            {
                cboKhoa.Items.Clear();
                cboKhoa.Items.Add("Tất cả khoa");
                cboKhoa.Items.AddRange(_students.Select(s => s.Khoa).Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct().OrderBy(x => x).Cast<object>().ToArray());
                cboKhoa.SelectedIndex = 0;
                txtTimKiem.Clear();
            }
            finally { _updatingFilters = false; }
            PopulateClasses();
            ApplyStudentFilters();
        }
        catch (Exception ex)
        {
            if (IsDisposed || Disposing) return;
            _students.Clear();
            dgvSinhVien.DataSource = null;
            lblRecordCount.Text = "Lỗi tải dữ liệu";
            RecordCountChanged?.Invoke(this, EventArgs.Empty);
            MessageBox.Show(this, "Không tải được danh sách sinh viên từ database.\r\n" + ex.Message,
                "Sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _loadingData = false;
            if (!IsDisposed && !Disposing)
            {
                UseWaitCursor = false;
                btnTimKiem.Enabled = true;
            }
        }
    }

    private void PopulateClasses()
    {
        var previous = cboLop.SelectedItem?.ToString();
        var faculty = cboKhoa.SelectedIndex > 0 ? cboKhoa.SelectedItem?.ToString() : null;
        _updatingFilters = true;
        try
        {
            cboLop.Items.Clear();
            cboLop.Items.Add("Tất cả lớp");
            cboLop.Items.AddRange(_students.Where(s => faculty is null || s.Khoa == faculty)
                .Select(s => s.Lop).Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct().OrderBy(x => x).Cast<object>().ToArray());
            cboLop.SelectedIndex = previous is not null && cboLop.Items.Contains(previous)
                ? cboLop.Items.IndexOf(previous) : 0;
        }
        finally { _updatingFilters = false; }
    }

    private void ApplyStudentFilters()
    {
        if (_updatingFilters) return;
        var keyword = txtTimKiem.Text.Trim();
        var faculty = cboKhoa.SelectedIndex > 0 ? cboKhoa.SelectedItem?.ToString() : null;
        var className = cboLop.SelectedIndex > 0 ? cboLop.SelectedItem?.ToString() : null;
        bool Matches(string? value) => value?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true;
        var result = _students.Where(s =>
            (faculty is null || s.Khoa == faculty) &&
            (className is null || s.Lop == className) &&
            (keyword.Length == 0 || Matches(s.MaSv) || Matches(s.HoTen) || Matches(s.Cccd) || Matches(s.SoDienThoai)))
            .ToList();
        dgvSinhVien.DataSource = result;
        lblRecordCount.Text = $"{result.Count} sinh viên";
        RecordCountChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ExportStudents_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Xuất danh sách sinh viên",
            Filter = "Excel Workbook (*.xlsx)|*.xlsx",
            FileName = $"SinhVien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
            AddExtension = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try { GridExcelExporter.Export(dgvSinhVien, dialog.FileName); }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Không thể xuất Excel. " + ex.Message, "Xuất Excel",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

