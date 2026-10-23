using QuanLyKyTucXa.Infrastructure;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class StudentImportPreviewDialog : Form
{
    public StudentImportPreviewDialog() { InitializeComponent(); QuanLyKyTucXa.WinForms.Helpers.DialogLayout.Configure(this, tlpRoot, flpActions); }

    public StudentImportPreviewDialog(IReadOnlyList<SinhVien> students, IReadOnlyList<string> errors) : this()
    {
        lblSummary.Text = $"{students.Count:N0} hồ sơ đọc được • {errors.Count:N0} lỗi. Chỉ thêm hồ sơ mới; trạng thái Chờ xếp phòng.";
        gridPreview.DataSource = students.Select(s => new {
            MaSV = s.MaSv, HoTen = s.HoTen, NgaySinh = s.NgaySinh.ToString("dd/MM/yyyy"),
            GioiTinh = s.GioiTinh, CCCD = s.Cccd, SoDienThoai = s.SoDienThoai, Email = s.Email,
            Lop = s.Lop, Khoa = s.Khoa, QueQuan = s.QueQuan
        }).ToList();
        var headers = new[] { "Mã SV", "Họ và tên", "Ngày sinh", "Giới tính", "CCCD", "Số điện thoại", "Email", "Lớp", "Khoa", "Quê quán" };
        for (var i = 0; i < gridPreview.Columns.Count; i++) gridPreview.Columns[i].HeaderText = headers[i];
        txtErrors.Text = errors.Count == 0
            ? "Không phát hiện lỗi. Bấm Xác nhận nhập để lưu toàn bộ hồ sơ mới vào database."
            : string.Join(Environment.NewLine, errors.Take(500)) + (errors.Count > 500 ? Environment.NewLine + "Còn lỗi khác. Hãy sửa file rồi nhập lại." : "");
        btnConfirm.Enabled = students.Count > 0 && errors.Count == 0;
    }
}
