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
    public string SavedId { get; private set; } = "";

    // Visual Studio can create the form without database access.
    public StudentEditDialog()
    {
        InitializeComponent();
    }

    public StudentEditDialog(SinhVien value, bool isNew, SinhVienService service) : this()
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
    }

    private async void Save_Click(object? sender, EventArgs e)
    {
        if (_saving || _value is null || _service is null) return;
        _saving = true;
        btnLuu.Enabled = btnHuy.Enabled = tlpFields.Enabled = false;
        lblError.Text = "";
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
        catch (InvalidOperationException ex) { lblError.Text = ex.Message; }
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

    private void Editor_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_saving) e.Cancel = true;
    }
}
