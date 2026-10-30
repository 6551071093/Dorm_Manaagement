using QuanLyKyTucXa.Infrastructure.Services;
namespace QuanLyKyTucXa.WinForms.Forms.Residents;
public partial class ReportIncidentDialog : Form
{
    private readonly PersonalService? _service;
    private bool _busy;
    public ReportIncidentDialog() { InitializeComponent(); QuanLyKyTucXa.WinForms.Helpers.ModernDialog.InstallCustom(this, layout, btnSave, btnCancel, lblError, null, new(lblTitle, txtTitle, true), new(lblDescription, txtDescription, true)); }
    public ReportIncidentDialog(PersonalService service) : this() => _service = service;
    private async void Save_Click(object? sender, EventArgs e)
    {
        if (_busy || _service is null) return;
        _busy = true; btnSave.Enabled = btnCancel.Enabled = false;
        var ok = false;
        try { await _service.ReportIncidentAsync(txtTitle.Text, txtDescription.Text); ok = true; }
        catch (Exception ex) { lblError.Text = ex is InvalidOperationException or UnauthorizedAccessException ? ex.Message : "Không gửi được báo hỏng. Kiểm tra kết nối rồi thử lại."; }
        finally { _busy = false; btnSave.Enabled = btnCancel.Enabled = true; }
        if (ok) { DialogResult = DialogResult.OK; Close(); }
    }
    private void Cancel_Click(object? sender, EventArgs e) { if (!_busy) Close(); }
    private void Editor_Closing(object? sender, FormClosingEventArgs e) { if (_busy) e.Cancel = true; }
}