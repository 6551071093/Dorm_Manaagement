using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class StudentEditDialog : Form
{
    private SinhVien? _value;
    private SinhVienService? _service;
    private bool _isNew;
    private bool _saving;
    private bool _bindingAcademic;
    private AcademicCatalog _catalog = new([], []);
    public string SavedId { get; private set; } = "";

    // Visual Studio can create the form without database access.
    public StudentEditDialog()
    {
        InitializeComponent();
        QuanLyKyTucXa.WinForms.Helpers.ModernDialog.Install(this, tlpFields, btnLuu, btnHuy, lblError);
        components ??= new System.ComponentModel.Container();
        errorProvider1 ??= new ErrorProvider(components);
        errorProvider1.ContainerControl = this;
        errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
    }

    public StudentEditDialog(SinhVien value, bool isNew, SinhVienService service) : this()
    {
        BindValue(value, isNew, service);
        Shown += async (_, _) => await LoadAcademicCatalogAsync();
    }

    private void BindValue(SinhVien value, bool isNew, SinhVienService service)
    {
        _value = value;
        _isNew = isNew;
        _service = service;
        Text = isNew ? "Thêm sinh viên" : "Sửa sinh viên";
        txtMaSv.Text = value.MaSv ?? "";
        txtMaSv.ReadOnly = !isNew;
        txtHoTen.Text = value.HoTen ?? "";
        dtpNgaySinh.MaxDate = DateTime.Today;
        var birth = value.NgaySinh == default ? DateTime.Today.AddYears(-18) : value.NgaySinh.ToDateTime(TimeOnly.MinValue);
        dtpNgaySinh.Value = birth < dtpNgaySinh.MinDate ? dtpNgaySinh.MinDate :
            birth > dtpNgaySinh.MaxDate ? dtpNgaySinh.MaxDate : birth;
        cboGioiTinh.SelectedItem = value.GioiTinh == "Nữ" ? "Nữ" : "Nam";
        txtCccd.Text = value.Cccd ?? "";
        txtSoDienThoai.Text = value.SoDienThoai ?? "";
        txtEmail.Text = value.Email ?? "";
        txtLop.Text = value.Lop ?? "";
        txtKhoa.Text = value.Khoa ?? "";
        txtQueQuan.Text = value.QueQuan ?? "";
        txtTrangThai.Text = isNew ? "Chờ xếp phòng" : value.TrangThai;
        txtKhoa.AutoCompleteMode = txtLop.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        txtKhoa.AutoCompleteSource = txtLop.AutoCompleteSource = AutoCompleteSource.ListItems;
        txtKhoa.DropDownWidth = 330; txtLop.DropDownWidth = 250;
        txtKhoa.SelectedIndexChanged += (_, _) => PopulateAcademicClasses();
        txtKhoa.Leave += (_, _) => PopulateAcademicClasses();
        txtLop.SelectedIndexChanged += (_, _) => {
            if (_bindingAcademic) return;
            var match = _catalog.Classes.FirstOrDefault(x => x.Code == txtLop.Text);
            if (match is not null && txtKhoa.Text != match.Faculty) txtKhoa.Text = match.Faculty;
        };

    }


    private async Task LoadAcademicCatalogAsync()
    {
        if (_service is null) return;
        btnLuu.Enabled = false;
        try
        {
            _catalog = await _service.GetAcademicCatalogAsync();
            var selected = txtKhoa.Text;
            _bindingAcademic = true;
            txtKhoa.Items.Clear();
            txtKhoa.Items.AddRange(_catalog.Faculties.Append(selected).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).Cast<object>().ToArray());
            txtKhoa.Text = selected;
            _bindingAcademic = false;
            PopulateAcademicClasses();
            lblError.ForeColor = Color.Firebrick;
            lblError.Text = "";
        }
        catch (Exception) { lblError.Text = "Chưa tải được khoa/lớp. Kiểm tra kết nối và chạy script 05_UpdateSchema_Catalog.sql."; }
        finally { _bindingAcademic = false; if (!IsDisposed) btnLuu.Enabled = true; }
    }
    private void PopulateAcademicClasses()
    {
        if (_bindingAcademic) return;
        var selected = txtLop.Text;
        _bindingAcademic = true;
        try
        {
            txtLop.Items.Clear();
            txtLop.Items.AddRange(_catalog.Classes.Where(x => string.IsNullOrWhiteSpace(txtKhoa.Text) || x.Faculty == txtKhoa.Text)
                .Select(x => x.Code).Append(selected).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).Cast<object>().ToArray());
            txtLop.Text = selected;
        }
        finally { _bindingAcademic = false; }
    }
    private async void Save_Click(object? sender, EventArgs e)
    {
        if (_saving || _value is null || _service is null) return;
        _saving = true;
        btnLuu.Enabled = btnHuy.Enabled = tlpFields.Enabled = false;
        lblError.Text = "";
        lblError.ForeColor = Color.Firebrick;
        errorProvider1.Clear();
        UseWaitCursor = true;
        var succeeded = false;
        try
        {
            _value.MaSv = txtMaSv.Text.Trim();
            _value.HoTen = txtHoTen.Text;
            _value.NgaySinh = DateOnly.FromDateTime(dtpNgaySinh.Value);
            _value.GioiTinh = cboGioiTinh.Text;
            _value.Cccd = txtCccd.Text;
            _value.SoDienThoai = txtSoDienThoai.Text;
            _value.Email = txtEmail.Text;
            _value.Lop = txtLop.Text;
            _value.Khoa = txtKhoa.Text;
            _value.QueQuan = txtQueQuan.Text;
            await _service.SaveAsync(_value, _isNew);
            SavedId = _value.MaSv;
            succeeded = true;
        }
        catch (InvalidOperationException ex) { ShowFieldError(ex.Message); }
        catch (DbUpdateException) { lblError.Text = "Không lưu được: dữ liệu trùng, đã thay đổi hoặc còn liên kết. Hãy kiểm tra và thử lại."; }
        catch (Exception) { lblError.Text = "Không kết nối/lưu được database. Kiểm tra kết nối rồi thử lại."; }
        finally
        {
            _saving = false;
            btnLuu.Enabled = btnHuy.Enabled = tlpFields.Enabled = true;
            UseWaitCursor = false;
        }
        if (succeeded) { DialogResult = DialogResult.OK; Close(); }
    }

    private void ShowFieldError(string message)
    {
        lblError.Text = message;
        Control target =
            message.Contains("CCCD") ? txtCccd :
            message.Contains("điện thoại") ? txtSoDienThoai :
            message.Contains("Ngày sinh") ? dtpNgaySinh :
            message.Contains("Email") ? txtEmail :
            message.Contains("Họ tên") ? txtHoTen :
            message.Contains("Khoa") ? txtKhoa :
            message.Contains("Lớp") ? txtLop :
            message.Contains("Quê quán") ? txtQueQuan :
            message.Contains("giới tính") ? cboGioiTinh : txtMaSv;
        errorProvider1.SetError(target, message);
        if (message.Contains("CCCD") && message.Contains("điện thoại"))
            errorProvider1.SetError(txtSoDienThoai, message);
        pnlScroll.ScrollControlIntoView(target);
    }
    private void Editor_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_saving) e.Cancel = true;
    }
}

