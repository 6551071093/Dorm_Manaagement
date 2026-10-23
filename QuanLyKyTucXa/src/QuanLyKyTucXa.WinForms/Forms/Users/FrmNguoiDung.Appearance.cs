namespace QuanLyKyTucXa.WinForms.Forms.Users;

public partial class FrmNguoiDung
{
    protected override void OnLoad(EventArgs e)
    {
        ApplyAccountAppearance();
        base.OnLoad(e);
    }
    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        if (tlpRoot is not null) ApplyAccountAppearance();
    }
    private void ApplyAccountAppearance()
    {
        int Px(int n) => (int)Math.Round(n * DeviceDpi / 96.0);
        tlpRoot.Padding = new Padding(Px(20),Px(14),Px(20),Px(12));
        tlpRoot.RowStyles[0].Height = Px(58);
        tlpRoot.RowStyles[1].Height = Px(94);
        tlpRoot.RowStyles[3].Height = Px(56);
        grpFilters.Margin = new Padding(0,0,0,Px(14));
        grpFilters.Padding = new Padding(Px(14),Px(8),Px(12),Px(10));
        flpFilters.Padding = new Padding(0,Px(3),0,0);
        txtSearch.Width = Px(320);
        cboRole.Width = Px(180);
        txtSearch.Margin = cboRole.Margin = new Padding(0,Px(3),Px(12),0);
        btnRefresh.Size = new Size(Px(104),Px(34));
        btnRefresh.Margin = Padding.Empty;
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.BackColor = Color.FromArgb(23,101,183);
        btnRefresh.ForeColor = Color.White;
        dgvUsers.EnableHeadersVisualStyles = false;
        dgvUsers.BackgroundColor = Color.FromArgb(246,246,250);
        dgvUsers.GridColor = Color.FromArgb(216,221,230);
        dgvUsers.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvUsers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvUsers.ColumnHeadersHeight = Px(34);
        dgvUsers.RowTemplate.Height = Px(35);
        foreach (DataGridViewRow row in dgvUsers.Rows) row.Height = Px(35);
        dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvUsers.MultiSelect = false;
        dgvUsers.AllowUserToResizeRows = false;
        dgvUsers.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle {
            BackColor=Color.FromArgb(231,237,248), ForeColor=Color.FromArgb(35,40,48),
            SelectionBackColor=Color.FromArgb(231,237,248), SelectionForeColor=Color.FromArgb(35,40,48),
            Font=new Font("Segoe UI",11F,FontStyle.Bold), Padding=new Padding(Px(10),0,0,0), WrapMode=DataGridViewTriState.False
        };
        dgvUsers.DefaultCellStyle = new DataGridViewCellStyle {
            BackColor=Color.White, ForeColor=Color.FromArgb(45,51,59),
            SelectionBackColor=Color.FromArgb(189,221,248), SelectionForeColor=Color.FromArgb(35,60,80),
            Padding=new Padding(Px(10),0,0,0), WrapMode=DataGridViewTriState.False
        };
        colLastLogin.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
        dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249,249,253);
        pnlFooter.Padding = new Padding(0,Px(12),0,0);
        lblCount.ForeColor = Color.FromArgb(100,116,139);
        btnAdd.Size = new Size(Px(104),Px(36));
        btnEdit.Size = btnToggle.Size = new Size(Px(90),Px(36));
        btnClose.Size = new Size(Px(84),Px(36));
        btnEdit.Text = "✎  Sửa";
        foreach (var button in new[] {btnAdd,btnEdit,btnToggle,btnClose})
        {
            button.FlatStyle = FlatStyle.Flat;
            button.Margin = new Padding(Px(9),0,0,0);
            button.FlatAppearance.BorderColor = Color.FromArgb(166,175,190);
            if (button != btnAdd) { button.BackColor=BackColor; button.ForeColor=Color.FromArgb(35,40,48); }
        }
        btnAdd.FlatAppearance.BorderSize = 0;
        UpdateToggleAppearance();
    }
    private void UpdateToggleAppearance()
    {
        var unlock = dgvUsers.CurrentRow?.DataBoundItem is QuanLyKyTucXa.Infrastructure.Services.UserAccountRow {Enabled:false};
        btnToggle.ForeColor = unlock ? Color.FromArgb(53,104,63) : Color.FromArgb(211,45,47);
        btnToggle.FlatAppearance.BorderColor = btnToggle.ForeColor;
    }
    private void FormatAccountState(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || dgvUsers.Columns[e.ColumnIndex].DataPropertyName is not ("Enabled" or "LoginLocked")) return;
        if (dgvUsers.Rows[e.RowIndex].DataBoundItem is not QuanLyKyTucXa.Infrastructure.Services.UserAccountRow account) return;
        var problem = dgvUsers.Columns[e.ColumnIndex].DataPropertyName == "Enabled" ? !account.Enabled : account.LoginLocked;
        e.CellStyle.BackColor = problem ? Color.FromArgb(253,235,237) : Color.FromArgb(232,246,234);
        e.CellStyle.ForeColor = problem ? Color.FromArgb(170,55,62) : Color.FromArgb(53,104,63);
        e.CellStyle.SelectionBackColor=e.CellStyle.BackColor;
        e.CellStyle.SelectionForeColor=e.CellStyle.ForeColor;
    }
}
