using System.Globalization;
using ClosedXML.Excel;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.Infrastructure.ImportExport;

public sealed record StudentImportPreview(List<SinhVien> Students, List<string> Errors);

public static class SinhVienExcelService
{
    private static readonly string[] Headers = [
        "Mã SV", "Họ và tên", "Ngày sinh", "Giới tính", "CCCD", "Số điện thoại",
        "Email", "Lớp", "Khoa", "Quê quán", "Trạng thái"
    ];

    public static StudentImportPreview Read(string path, IProgress<int>? progress = null)
    {
        if (new FileInfo(path).Length > 20 * 1024 * 1024) throw new InvalidOperationException("File Excel tối đa 20 MB.");
        using var workbook = new XLWorkbook(path);
        var sheet = workbook.Worksheets.FirstOrDefault(x => x.Name == "SinhVien") ?? workbook.Worksheets.First();
        var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var c = 1; c <= (sheet.LastColumnUsed()?.ColumnNumber() ?? 0); c++)
        {
            var header = sheet.Cell(1, c).GetString().Trim();
            if (header.Length > 0 && !columns.TryAdd(header, c)) throw new InvalidOperationException($"Tiêu đề cột '{header}' bị lặp.");
        }
        foreach (var header in Headers.Take(10).Where(x => x is not ("Email" or "Quê quán")))
            if (!columns.ContainsKey(header)) throw new InvalidOperationException($"Thiếu cột '{header}'. Hãy dùng nút Tải mẫu Excel.");
        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 1;
        if (lastRow > 5001) throw new InvalidOperationException("Mỗi file tối đa 5000 dòng sinh viên.");
        var students = new List<SinhVien>();
        var errors = new List<string>();
        for (var r = 2; r <= lastRow; r++)
        {
            var row = r;
            string ReadCell(string header)
            {
                if (!columns.TryGetValue(header, out var col)) return "";
                var cell = sheet.Cell(row, col);
                if (cell.HasFormula) throw new InvalidOperationException($"Cột '{header}' phải chứa dữ liệu, không dùng công thức.");
                return cell.DataType == XLDataType.Number ? cell.GetFormattedString(CultureInfo.InvariantCulture).Trim() : cell.GetString().Trim();
            }
            if (columns.Values.All(c => sheet.Cell(r, c).IsEmpty())) continue;
            try
            {
                var dateCell = sheet.Cell(r, columns["Ngày sinh"]);
                if (dateCell.HasFormula) throw new InvalidOperationException("Ngày sinh không được dùng công thức.");
                DateOnly birth;
                if (dateCell.DataType == XLDataType.DateTime) birth = DateOnly.FromDateTime(dateCell.GetDateTime());
                else if (!DateOnly.TryParseExact(ReadCell("Ngày sinh"), ["dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd"],
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out birth))
                    throw new InvalidOperationException("Ngày sinh phải theo dd/MM/yyyy hoặc yyyy-MM-dd.");
                var s = new SinhVien {
                    MaSv = ReadCell("Mã SV"), HoTen = ReadCell("Họ và tên"), NgaySinh = birth,
                    GioiTinh = ReadCell("Giới tính"), Cccd = ReadCell("CCCD"), SoDienThoai = ReadCell("Số điện thoại"),
                    Email = ReadCell("Email"), Lop = ReadCell("Lớp"), Khoa = ReadCell("Khoa"), QueQuan = ReadCell("Quê quán"),
                    TrangThai = "Chờ xếp phòng"
                };
                SinhVienService.ValidateAndNormalize(s);
                students.Add(s);
            }
            catch (InvalidOperationException ex) { errors.Add($"Dòng Excel {r}: {ex.Message}"); }
            if (r % 25 == 0 || r == lastRow) progress?.Report((int)(r * 100L / Math.Max(2, lastRow)));
        }
        if (students.Count == 0 && errors.Count == 0) errors.Add("File chưa có dữ liệu sinh viên.");
        progress?.Report(100);
        return new StudentImportPreview(students, errors);
    }

    public static void Write(string path, IReadOnlyList<SinhVien> students, bool template = false, IProgress<int>? progress = null)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("SinhVien");
        for (var c = 0; c < Headers.Length; c++) sheet.Cell(1, c + 1).Value = Headers[c];
        sheet.Range(1, 1, 1, Headers.Length).Style.Font.Bold = true;
        sheet.Range(1, 1, 1, Headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#DCE8F6");
        sheet.SheetView.FreezeRows(1);
        sheet.Columns(1, Headers.Length).Width = 20;
        sheet.Column(2).Width = 28;
        sheet.Column(9).Width = 30;
        sheet.Column(10).Width = 30;
        foreach (var col in new[] { 1, 5, 6 }) sheet.Column(col).Style.NumberFormat.Format = "@";
        for (var i = 0; i < students.Count; i++)
        {
            var s = students[i]; var row = i + 2;
            sheet.Cell(row, 1).Value = s.MaSv;
            sheet.Cell(row, 2).Value = s.HoTen;
            sheet.Cell(row, 3).Value = s.NgaySinh.ToDateTime(TimeOnly.MinValue);
            sheet.Cell(row, 3).Style.DateFormat.Format = "dd/MM/yyyy";
            sheet.Cell(row, 4).Value = s.GioiTinh;
            sheet.Cell(row, 5).Value = s.Cccd;
            sheet.Cell(row, 6).Value = s.SoDienThoai;
            sheet.Cell(row, 7).Value = s.Email ?? "";
            sheet.Cell(row, 8).Value = s.Lop;
            sheet.Cell(row, 9).Value = s.Khoa;
            sheet.Cell(row, 10).Value = s.QueQuan ?? "";
            sheet.Cell(row, 11).Value = s.TrangThai;
            if (i % 25 == 0) progress?.Report((int)((i + 1) * 90L / Math.Max(1, students.Count)));
        }
        if (students.Count > 0) sheet.Range(1, 1, students.Count + 1, Headers.Length).SetAutoFilter();
        if (template)
        {
            var notes = workbook.Worksheets.Add("HuongDan");
            string[] lines = [
                "Nhập sinh viên mới vào sheet SinhVien, giữ nguyên tiêu đề ở dòng 1.",
                "Bắt buộc: Mã SV, Họ và tên, Ngày sinh, Giới tính, CCCD, Số điện thoại, Lớp, Khoa.",
                "Ngày sinh: dd/MM/yyyy; giới tính: Nam hoặc Nữ.",
                "CCCD: 12 chữ số; số điện thoại: 10 chữ số bắt đầu bằng 0. Giữ các cột này ở dạng Text.",
                "Không lặp mã SV, CCCD hoặc số điện thoại trong file hay database.",
                "Chỉ thêm hồ sơ mới. Không ghi đè hồ sơ đã có.",
                "Trạng thái hồ sơ nhập luôn là Chờ xếp phòng; trạng thái đang ở được quản lý qua hợp đồng.",
                "Tối đa 5000 sinh viên/file; xem trước và sửa hết lỗi trước khi xác nhận nhập."
            ];
            for (var i = 0; i < lines.Length; i++) notes.Cell(i + 1, 1).Value = lines[i];
            notes.Column(1).Width = 115;
        }
        // Save completely before replacing the chosen destination file.
        var destination = Path.GetFullPath(path);
        var temp = Path.Combine(Path.GetDirectoryName(destination)!, "." + Guid.NewGuid().ToString("N") + ".xlsx");
        try { workbook.SaveAs(temp); File.Move(temp, destination, true); }
        finally { if (File.Exists(temp)) File.Delete(temp); }
        progress?.Report(100);
    }
}
