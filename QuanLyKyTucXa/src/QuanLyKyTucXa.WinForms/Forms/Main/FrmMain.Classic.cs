using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.WinForms.Forms.Users;
using System.Globalization;
using System.Windows.Forms;
using QuanLyKyTucXa.WinForms.Forms.Residents;
using QuanLyKyTucXa.WinForms.Forms.Rooms;

namespace QuanLyKyTucXa.WinForms.Forms.Main;

public partial class FrmMain
{
    private bool _classicReady;

    private void ConfigureClassicShell()
    {
        tabWorkspace.TabPages.Remove(tabRooms);
        tabWorkspace.TabPages.Remove(tabStudents);
        tabDashboard.Controls.Add(pnlHome);
        pnlHome.Dock = DockStyle.Fill;
        classicAccountText.Text = $"Tài khoản: {lblProfileName.Text} (Quản trị viên) | {DateTime.Today:dd/MM/yyyy}";
        var buttons = new[] { tsStudents, tsRooms, tsContracts, tsInvoices, tsIncidents, tsRefresh, tsExport };
        var kinds = new[] { "students", "rooms", "document", "invoice", "tools", "refresh", "export" };
        for (var i = 0; i < buttons.Length; i++) buttons[i].Image = ClassicIcons.Create(kinds[i]);
        _classicReady = true;
    }

    private async void Main_Load(object? sender, EventArgs e)
    {
        ShowHome();
    }

    private void Home_Click(object? sender, EventArgs e) => ShowHome();
    private void Students_Click(object? sender, EventArgs e) => ShowStudents();
    private void Rooms_Click(object? sender, EventArgs e) => ShowRooms();
    private void Register_Click(object? sender, EventArgs e) => ShowRegistration();
    private void Lookup_Click(object? sender, EventArgs e) => ShowLookup();
    private void Approve_Click(object? sender, EventArgs e) => ShowApprovals();
    private void Invoices_Click(object? sender, EventArgs e) => ShowComingSoon("Hóa đơn");
    private void Incidents_Click(object? sender, EventArgs e) => ShowComingSoon("Sự cố");
    private void Utilities_Click(object? sender, EventArgs e) => ShowComingSoon("Điện nước");
    private void Reports_Click(object? sender, EventArgs e) => ShowComingSoon("Thống kê");
    private void Towers_Click(object? sender, EventArgs e) => ShowComingSoon("Tòa nhà");
    private void Beds_Click(object? sender, EventArgs e) => ShowComingSoon("Giường");
    private void Prices_Click(object? sender, EventArgs e) => ShowComingSoon("Bảng giá và dịch vụ");
    private void Contracts_Click(object? sender, EventArgs e) => ShowComingSoon("Hợp đồng");
    private void Exit_Click(object? sender, EventArgs e) => Close();
    private void CloseTab_Click(object? sender, EventArgs e) => CloseCurrentTab();
    private void About_Click(object? sender, EventArgs e) => MessageBox.Show(this,
        "Quản lý ký túc xá\r\nQuản lý sinh viên, phòng ở và các nghiệp vụ nội trú.",
        "Giới thiệu", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private async void Refresh_Click(object? sender, EventArgs e)
    {
        if (!await EnsureSessionAsync()) return;
        if (_activeChild is FrmNguoiDung users) { await users.RefreshViewAsync(); return; }
        if (_activeChild is FrmCaNhan personal) { await personal.RefreshViewAsync(); return; }
        if (tabWorkspace.SelectedTab == tabDashboard)
        {
            await RefreshDashboardAsync();
            return;
        }
        if (_activeChild is FrmPhong rooms)
        {
            await rooms.RefreshViewAsync();
            return;
        }
        if (_activeChild is FrmSinhVien students)
        {
            await students.RefreshViewAsync();
            return;
        }
        if (_activeChild is FrmDangKyKtx registration) { await registration.RefreshViewAsync(); return; }
        if (_activeChild is FrmTraCuuDangKy lookup) { await lookup.RefreshViewAsync(); return; }
        if (_activeChild is FrmDuyetDon approvals) { await approvals.RefreshViewAsync(); return; }
        var refresh = FindControls<Button>(_activeChild).FirstOrDefault(b => b.Name == "btnLamMoi");
        if (refresh is not null) refresh.PerformClick();
        else SetStatus("Sẵn sàng");
    }

    private async void Export_Click(object? sender, EventArgs e)
    {
        if (_activeChild is FrmSinhVien students)
        {
            await students.ExportStudentsAsync();
            return;
        }
        var grid = FindControls<DataGridView>(_activeChild).FirstOrDefault(g => g.Visible);
        if (grid is null)
        {
            MessageBox.Show(this, "Chọn tab có bảng dữ liệu để xuất Excel.", "Xuất Excel",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using var dialog = new SaveFileDialog
        {
            Title = "Xuất Excel",
            Filter = "Excel Workbook (*.xlsx)|*.xlsx",
            FileName = $"KTX_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
            AddExtension = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            GridExcelExporter.Export(grid, dialog.FileName);
            SetStatus("Đã xuất Excel: " + Path.GetFileName(dialog.FileName));
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Không thể xuất Excel. " + ex.Message, "Xuất Excel",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static IEnumerable<T> FindControls<T>(Control? root) where T : Control
    {
        if (root is null) yield break;
        foreach (Control child in root.Controls)
        {
            if (child is T match) yield return match;
            foreach (var descendant in FindControls<T>(child)) yield return descendant;
        }
    }

    private async void Workspace_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_classicReady) return;
        var page = tabWorkspace.SelectedTab;
        if (page == tabDashboard) { if (_session?.IsStudent == true) { ShowPersonal(); return; } _activeChild = null; await RefreshDashboardAsync(); return; }
        if (page == tabStudents && page.Controls.Count == 0) { ShowStudents(); return; }
        if (page == tabRooms && page.Controls.Count == 0) { ShowRooms(); return; }
        _activeChild = page?.Controls.OfType<Form>().FirstOrDefault();
        if (_activeChild is FrmPhong rooms) SetStatus($"Sẵn sàng - {rooms.VisibleRoomCount} phòng");
        else if (_activeChild is FrmSinhVien students) SetStatus($"Sẵn sàng - {students.VisibleStudentCount} sinh viên");
        else SetStatus(page == tabDashboard ? "Sẵn sàng - Dashboard" : "Sẵn sàng - " + page?.Text);
    }

    private void CloseCurrentTab()
    {
        var page = tabWorkspace.SelectedTab;
        if (page is null || page == tabDashboard) return;
        var child = page.Controls.OfType<Form>().FirstOrDefault();
        if (child is not null) { page.Controls.Remove(child); child.Close(); child.Dispose(); }
        tabWorkspace.TabPages.Remove(page);
        if (page != tabRooms && page != tabStudents) page.Dispose();
        _activeChild = null;
        tabWorkspace.SelectedTab = tabDashboard;
    }
}


