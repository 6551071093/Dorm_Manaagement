using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class FrmSinhVien
{
    private readonly string _connectionString;
    private AcademicCatalog _academicCatalog = new([], []);
    private bool _loadingData;
    private bool _updatingFilters;
    private List<StudentListRow> _students = new();
    public event EventHandler? RecordCountChanged;
    public int VisibleStudentCount => dgvSinhVien.Rows.Count;
    private sealed record StudentListRow(string MaSv, string HoTen, DateOnly NgaySinh,
        string GioiTinh, string Cccd, string SoDienThoai, string Lop, string Khoa, string TrangThai);

    private void ConfigureDatabaseEvents()
    {
        EnsureStudentColumns();
        dgvSinhVien.AutoGenerateColumns = false;
        dgvSinhVien.Columns[2].DefaultCellStyle.Format = "dd/MM/yyyy";
        cboKhoa.DropDownStyle = ComboBoxStyle.DropDownList;
        cboKhoa.DropDownWidth = 330; cboLop.DropDownWidth = 250;
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
        btnTaiMau.Click += DownloadTemplate_Click;
        btnThem.Click += AddStudent_Click;
        btnSua.Click += EditStudent_Click;
        btnXoa.Click += DeleteStudent_Click;
    }

    // Restore columns even if a Designer save drops their serialized definitions.
    private void EnsureStudentColumns()
    {
        if (dgvSinhVien.Columns.Count == 9) return;
        dgvSinhVien.Columns.Clear();
        dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn { Name = "dataGridViewTextBoxColumn1", DataPropertyName = "MaSv", HeaderText = "Mã SV", FillWeight = 11, MinimumWidth = 95 };
        dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn { Name = "dataGridViewTextBoxColumn2", DataPropertyName = "HoTen", HeaderText = "Họ và tên", FillWeight = 17, MinimumWidth = 145 };
        dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn { Name = "dataGridViewTextBoxColumn3", DataPropertyName = "NgaySinh", HeaderText = "Ngày sinh", FillWeight = 11, MinimumWidth = 100 };
        dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn { Name = "dataGridViewTextBoxColumn4", DataPropertyName = "GioiTinh", HeaderText = "Giới tính", FillWeight = 8, MinimumWidth = 100 };
        dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn { Name = "dataGridViewTextBoxColumn5", DataPropertyName = "Cccd", HeaderText = "CCCD", FillWeight = 13, MinimumWidth = 125 };
        dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn { Name = "dataGridViewTextBoxColumn6", DataPropertyName = "SoDienThoai", HeaderText = "Số điện thoại", FillWeight = 12, MinimumWidth = 120 };
        dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn { Name = "dataGridViewTextBoxColumn7", DataPropertyName = "Lop", HeaderText = "Lớp", FillWeight = 11, MinimumWidth = 105 };
        dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn { Name = "dataGridViewTextBoxColumn8", DataPropertyName = "Khoa", HeaderText = "Khoa", FillWeight = 17, MinimumWidth = 155 };
        dataGridViewTextBoxColumn9 = new DataGridViewTextBoxColumn { Name = "dataGridViewTextBoxColumn9", DataPropertyName = "TrangThai", HeaderText = "Trạng thái", FillWeight = 13, MinimumWidth = 125 };
        dgvSinhVien.Columns.AddRange(dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3,
            dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5, dataGridViewTextBoxColumn6,
            dataGridViewTextBoxColumn7, dataGridViewTextBoxColumn8, dataGridViewTextBoxColumn9);
        foreach (DataGridViewColumn col in dgvSinhVien.Columns) col.SortMode = DataGridViewColumnSortMode.NotSortable;
    }
    public async Task RefreshViewAsync()
    {
        if (_loadingData || _excelBusy) return;
        _loadingData = true;
        UseWaitCursor = true;
        btnTimKiem.Enabled = false;
        lblRecordCount.Text = "Đang tải...";
        try
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("Chưa có chuỗi kết nối database.");
            await using var db = AppDbContextFactory.Create(_connectionString);
            await AuthPolicy.RequireAsync(db, _session, AppPermission.ManageResidents);
            var students = await db.SinhViens.AsNoTracking().OrderBy(s => s.MaSv)
                .Select(s => new StudentListRow(s.MaSv, s.HoTen, s.NgaySinh, s.GioiTinh,
                    s.Cccd, s.SoDienThoai, s.Lop, s.Khoa, s.TrangThai))
                .ToListAsync();
            if (IsDisposed || Disposing) return;
            _students = students;
            _academicCatalog = await StudentService.GetAcademicCatalogAsync();
            _updatingFilters = true;
            try
            {
                cboKhoa.Items.Clear();
                cboKhoa.Items.Add("Tất cả khoa");
                cboKhoa.Items.AddRange(_academicCatalog.Faculties.Concat(_students.Select(s => s.Khoa)).Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct().OrderBy(x => x).Cast<object>().ToArray());
                cboKhoa.SelectedIndex = 0;
                cboLop.SelectedIndex = 0;
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
                .Select(s => s.Lop).Concat(_academicCatalog.Classes.Where(x => faculty is null || x.Faculty == faculty).Select(x => x.Code)).Where(x => !string.IsNullOrWhiteSpace(x))
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

    private async void ExportStudents_Click(object? sender, EventArgs e) => await ExportStudentsAsync();
}



