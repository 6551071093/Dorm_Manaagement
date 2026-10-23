using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Main;

public partial class FrmMain
{
    private bool _exportingHandoff;

    private async Task SaveDataHandoffAsync(ToolStripMenuItem item)
    {
        if (_exportingHandoff || !await EnsureSessionAsync() || !RequireUi(AppPermission.ManageUsers) || _session is null) return;
        using var dialog = new SaveFileDialog {
            Title = "Lưu dữ liệu bàn giao cho nhóm", Filter = "Dữ liệu bàn giao (*.sql)|*.sql",
            FileName = "LatestData.sql", DefaultExt = "sql", AddExtension = true, OverwritePrompt = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        _exportingHandoff = true;
        item.Enabled = false;
        try
        {
            var result = await new DataHandoffService(() => AppDbContextFactory.Create(_connectionString), _session).ExportAsync(dialog.FileName);
            MessageBox.Show(this, $"Đã lưu {result.Rows} bản ghi trong {result.Tables} bảng.\n\nĐưa file này vào database/snapshots/LatestData.sql và commit/push cùng code. Máy nhận nhập file vào database demo để xem dữ liệu đã bàn giao.\n\nCCCD, thông tin liên hệ, ngày sinh và mật khẩu gốc đã được thay bằng dữ liệu demo. Nội dung ghi chú vẫn giữ nguyên; hãy xem lại trước khi chia sẻ.", "Bàn giao dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            var message = ex is UnauthorizedAccessException ? ex.Message : "Chưa lưu được bản bàn giao. Kiểm tra kết nối, quyền ghi file và cấu trúc database (script 05_UpdateSchema_Catalog.sql).";
            MessageBox.Show(this, message, "Bàn giao dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally { _exportingHandoff = false; if (!IsDisposed) item.Enabled = true; }
    }
}
