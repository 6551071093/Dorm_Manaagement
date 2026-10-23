using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Rooms;

public partial class RoomEditDialog : Form
{
    private Phong? _value;
    private PhongGiuongService? _service;
    private bool _isNew;
    private bool _saving;
    public string SavedId { get; private set; } = "";

    // Designer opens this constructor without loading the database.
    public RoomEditDialog()
    {
        InitializeComponent();
        QuanLyKyTucXa.WinForms.Helpers.DialogLayout.ConfigureCompactEditor(this, tlpRoot, pnlScroll, tlpFields, flpActions, lblError, 9);
        components ??= new System.ComponentModel.Container();
    }

    public RoomEditDialog(Phong value, bool isNew, List<ToaNha> towers, PhongGiuongService service)
        : this()
    {
        _value = value;
        _service = service;
        _isNew = isNew;
        Text = isNew ? "Thêm phòng" : "Sửa phòng";
        txtMaPhong.Text = value.MaPhong ?? "";
        txtMaPhong.ReadOnly = !isNew;
        txtTenPhong.Text = value.TenPhong ?? "";
        cboToa.Items.AddRange(towers.Select(x => x.MaToa).Cast<object>().ToArray());
        if (value.MaToa is not null && cboToa.Items.Contains(value.MaToa)) cboToa.SelectedItem = value.MaToa;
        else if (cboToa.Items.Count > 0) cboToa.SelectedIndex = 0;
        nudTang.Value = Math.Clamp(value.Tang == 0 ? 1 : value.Tang, nudTang.Minimum, nudTang.Maximum);
        txtLoaiPhong.Text = value.LoaiPhong ?? "Phòng 4 người";
        nudSucChua.Value = Math.Clamp(value.SoChoToiDa == 0 ? 4 : value.SoChoToiDa, nudSucChua.Minimum, nudSucChua.Maximum);
        nudGiaPhong.Value = Math.Clamp(value.GiaPhong, nudGiaPhong.Minimum, nudGiaPhong.Maximum);
        chkSuaChua.Checked = value.TrangThai == "Đang sửa chữa";
        txtGhiChu.Text = value.GhiChu ?? "";
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
            _value.MaPhong = txtMaPhong.Text.Trim();
            _value.TenPhong = txtTenPhong.Text;
            _value.MaToa = cboToa.Text;
            _value.Tang = (int)nudTang.Value;
            _value.LoaiPhong = txtLoaiPhong.Text;
            _value.SoChoToiDa = (int)nudSucChua.Value;
            _value.GiaPhong = nudGiaPhong.Value;
            _value.TrangThai = chkSuaChua.Checked ? "Đang sửa chữa" : "Còn chỗ";
            _value.GhiChu = txtGhiChu.Text;
            await _service.SaveAsync(_value, _isNew);
            SavedId = _value.MaPhong;
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
