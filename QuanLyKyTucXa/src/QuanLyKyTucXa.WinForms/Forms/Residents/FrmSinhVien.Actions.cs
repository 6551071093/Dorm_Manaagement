using QuanLyKyTucXa.Infrastructure;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class FrmSinhVien
{
    private bool _studentActionBusy;
    private SinhVienService StudentService => new(() => AppDbContextFactory.Create(_connectionString), _session);

    private string? SelectedStudentId()
    {
        if (dgvSinhVien.SelectedRows.Count > 0 && dgvSinhVien.SelectedRows[0].DataBoundItem is StudentListRow row) return row.MaSv;
        MessageBox.Show(this, "Vui lòng chọn sinh viên trong danh sách.", "Sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return null;
    }

    private async void AddStudent_Click(object? sender, EventArgs e) => await RunStudentActionAsync(() => EditStudentAsync(null));
    private async void EditStudent_Click(object? sender, EventArgs e)
    {
        var id = SelectedStudentId();
        if (id is not null) await RunStudentActionAsync(() => EditStudentAsync(id));
    }
    private async void DeleteStudent_Click(object? sender, EventArgs e)
    {
        var id = SelectedStudentId();
        if (id is null) return;
        if (MessageBox.Show(this, $"Xóa hồ sơ sinh viên {id}? Chỉ xóa được hồ sơ chưa có lịch sử liên quan.",
            "Xác nhận xóa sinh viên", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
        await RunStudentActionAsync(async () =>
        {
            await StudentService.DeleteAsync(id);
            await RefreshViewAsync();
        });
    }
    private async Task EditStudentAsync(string? id)
    {
        var service = StudentService;
        using var dialog = id is null ? new StudentEditDialog(new SinhVien(), true, service) : new StudentEditDialog(service, id);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        await RefreshViewAsync();
        foreach (DataGridViewRow row in dgvSinhVien.Rows)
        {
            if (row.DataBoundItem is not StudentListRow record || record.MaSv != dialog.SavedId) continue;
            dgvSinhVien.ClearSelection();
            dgvSinhVien.CurrentCell = row.Cells[0];
            row.Selected = true;
            dgvSinhVien.FirstDisplayedScrollingRowIndex = row.Index;
            break;
        }
    }
    private async Task RunStudentActionAsync(Func<Task> action)
    {
        if (_studentActionBusy || _loadingData || _excelBusy) return;
        _studentActionBusy = true;
        btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = false;
        try { await action(); }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException) { MessageBox.Show(this, ex.Message, "Sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        catch (Exception) { MessageBox.Show(this, "Không thể thực hiện thao tác. Kiểm tra kết nối hoặc làm mới dữ liệu rồi thử lại.", "Sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally
        {
            _studentActionBusy = false;
            if (!IsDisposed && !Disposing) btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = true;
        }
    }
}

