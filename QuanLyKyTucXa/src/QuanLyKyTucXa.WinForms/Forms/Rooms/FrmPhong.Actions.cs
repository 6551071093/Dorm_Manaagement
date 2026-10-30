using QuanLyKyTucXa.Infrastructure;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Rooms;

public partial class FrmPhong
{
    private bool _roomActionBusy;
    private PhongGiuongService RoomService => new(() => AppDbContextFactory.Create(_connectionString), _session);
    private string? SelectedRoomId()
    {
        if (dgvPhong.SelectedRows.Count > 0)
            return Convert.ToString(dgvPhong.SelectedRows[0].Cells["MaPhong"].Value);
        MessageBox.Show(this, "Vui lòng chọn phòng trong danh sách.", "Phòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return null;
    }
    private async void Add_Click(object? sender, EventArgs e) => await RunRoomActionAsync(() => EditRoomAsync(null));
    private async void Edit_Click(object? sender, EventArgs e)
    {
        var id = SelectedRoomId();
        if (id is not null) await RunRoomActionAsync(() => EditRoomAsync(id));
    }
    private async void Delete_Click(object? sender, EventArgs e)
    {
        var id = SelectedRoomId();
        if (id is null) return;
        if (MessageBox.Show(this, $"Xóa phòng {id} và các giường trống thuộc phòng? Phòng có lịch sử liên quan sẽ không được xóa.",
            "Xác nhận xóa phòng", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
        await RunRoomActionAsync(async () =>
        {
            await RoomService.DeleteAsync(id);
            await RefreshViewAsync();
        });
    }
    private async Task EditRoomAsync(string? id)
    {
        if (!QuanLyKyTucXa.Infrastructure.Security.AuthPolicy.Can(_session, QuanLyKyTucXa.Infrastructure.Security.AppPermission.ConfigureRooms))
            throw new UnauthorizedAccessException("Chỉ Admin được thêm hoặc sửa cấu hình phòng.");
        var service = RoomService;
        using var dialog = new RoomEditDialog(service, id);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        await RefreshViewAsync();
        foreach (DataGridViewRow row in dgvPhong.Rows)
        {
            if (!string.Equals(Convert.ToString(row.Cells["MaPhong"].Value), dialog.SavedId, StringComparison.OrdinalIgnoreCase)) continue;
            dgvPhong.ClearSelection();
            dgvPhong.CurrentCell = row.Cells["MaPhong"];
            row.Selected = true;
            dgvPhong.FirstDisplayedScrollingRowIndex = row.Index;
            break;
        }
    }
    private async Task RunRoomActionAsync(Func<Task> action)
    {
        if (_roomActionBusy || _loadTask is { IsCompleted: false }) return;
        _roomActionBusy = true;
        btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = false;
        try { await action(); }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException) { MessageBox.Show(this, ex.Message, "Phòng", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        catch (Exception) { MessageBox.Show(this, "Không thể thực hiện thao tác. Kiểm tra kết nối hoặc làm mới dữ liệu rồi thử lại.", "Phòng", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally
        {
            _roomActionBusy = false;
            if (!IsDisposed && !Disposing) btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = true;
        }
    }
}
