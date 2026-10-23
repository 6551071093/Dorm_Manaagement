using QuanLyKyTucXa.Infrastructure.ImportExport;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class FrmSinhVien
{
    private bool _excelBusy;

    public async Task ExportStudentsAsync()
    {
        if (_excelBusy || _studentActionBusy || _loadingData) return;
        var ids = dgvSinhVien.Rows.Cast<DataGridViewRow>()
            .Select(r => ((StudentListRow)r.DataBoundItem!).MaSv).ToArray();
        if (ids.Length == 0) { MessageBox.Show(this, "Không có sinh viên trong bộ lọc để xuất."); return; }
        using var dialog = new SaveFileDialog {
            Title = "Xuất hồ sơ sinh viên theo bộ lọc", Filter = "Excel Workbook (*.xlsx)|*.xlsx",
            FileName = $"SinhVien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx", AddExtension = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        await RunExcelAsync(async progress =>
        {
            var students = await StudentService.GetForExportAsync(ids);
            if (students.Count != ids.Length) throw new InvalidOperationException("Danh sách đã thay đổi. Hãy làm mới trước khi xuất.");
            prgExcel.Style = ProgressBarStyle.Continuous;
            await Task.Run(() => SinhVienExcelService.Write(dialog.FileName, students, progress: progress));
            MessageBox.Show(this, $"Đã xuất {students.Count:N0} hồ sơ.", "Xuất Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
        });
    }

    private async void ImportStudents_Click(object? sender, EventArgs e)
    {
        if (_excelBusy || _studentActionBusy || _loadingData) return;
        using var dialog = new OpenFileDialog { Title = "Nhập hồ sơ sinh viên", Filter = "Excel Workbook (*.xlsx)|*.xlsx" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        var imported = false;
        await RunExcelAsync(async progress =>
        {
            prgExcel.Style = ProgressBarStyle.Continuous;
            var preview = await Task.Run(() => SinhVienExcelService.Read(dialog.FileName, progress));
            prgExcel.Style = ProgressBarStyle.Marquee;
            if (preview.Students.Count > 0)
                preview.Errors.AddRange(await StudentService.GetImportErrorsAsync(preview.Students));
            prgExcel.Style = ProgressBarStyle.Continuous;
            using var confirm = new StudentImportPreviewDialog(preview.Students, preview.Errors);
            if (confirm.ShowDialog(this) != DialogResult.OK) return;
            prgExcel.Style = ProgressBarStyle.Marquee;
            var count = await StudentService.ImportAsync(preview.Students);
            imported = true;
            MessageBox.Show(this, $"Đã nhập {count:N0} sinh viên. Các hồ sơ đang chờ xếp phòng.", "Nhập Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
        });
        if (imported) await RefreshViewAsync();
    }

    private async void DownloadTemplate_Click(object? sender, EventArgs e)
    {
        if (_excelBusy || _studentActionBusy || _loadingData) return;
        using var dialog = new SaveFileDialog {
            Title = "Lưu mẫu nhập hồ sơ sinh viên", Filter = "Excel Workbook (*.xlsx)|*.xlsx",
            FileName = "Mau_Nhap_SinhVien.xlsx", AddExtension = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        await RunExcelAsync(async progress =>
        {
            await Task.Run(() => SinhVienExcelService.Write(dialog.FileName, [], template: true, progress: progress));
            MessageBox.Show(this, "Đã tạo mẫu Excel. Xem sheet HuongDan trước khi nhập.", "Mẫu Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
        });
    }

    private async Task RunExcelAsync(Func<IProgress<int>, Task> action)
    {
        if (_excelBusy || _studentActionBusy || _loadingData) return;
        _excelBusy = true;
        grpFilters.Enabled = false;
        btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = btnDong.Enabled = false;
        prgExcel.Visible = true;
        prgExcel.Style = ProgressBarStyle.Marquee;
        prgExcel.Value = 0;
        var progress = new Progress<int>(value =>
        {
            if (!IsDisposed && !Disposing && _excelBusy) prgExcel.Value = Math.Clamp(value, 0, 100);
        });
        try { await action(progress); }
        catch (InvalidOperationException ex) { MessageBox.Show(this, ex.Message, "Excel sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        catch (Exception) { MessageBox.Show(this, "Không xử lý được file hoặc database. Hãy đóng file Excel nếu đang mở, kiểm tra định dạng và kết nối rồi thử lại.", "Excel sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally
        {
            _excelBusy = false;
            if (!IsDisposed && !Disposing)
            {
                grpFilters.Enabled = true;
                btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = btnDong.Enabled = true;
                prgExcel.Visible = false;
            }
        }
    }
}
