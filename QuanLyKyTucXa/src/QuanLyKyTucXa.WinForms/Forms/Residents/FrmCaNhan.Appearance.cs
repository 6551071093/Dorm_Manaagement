namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class FrmCaNhan
{
    protected override void OnLoad(EventArgs e)
    {
        ApplyPersonalLayout();
        base.OnLoad(e);
    }
    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        if (tlpRoot is not null) ApplyPersonalLayout();
    }
    private void ApplyPersonalLayout()
    {
        int Px(int value) => (int)Math.Round(value * DeviceDpi / 96.0);
        tlpRoot.Padding = new Padding(Px(20), Px(14), Px(20), Px(12));
        tlpRoot.RowStyles[0].Height = Px(58);
        tlpRoot.RowStyles[1].Height = Px(94);
        tlpRoot.RowStyles[3].Height = Px(56);
        grpProfile.Margin = new Padding(0, 0, 0, Px(14));
        grpProfile.Padding = new Padding(Px(14), Px(8), Px(12), Px(10));
        tabs.Margin = Padding.Empty;
        tabs.ItemSize = new Size(Px(180), Px(32));
        foreach (var page in new[] { tabRooms, tabBills, tabIncidents })
        {
            page.BackColor = BackColor;
            page.Padding = new Padding(Px(1));
        }
        foreach (var grid in new[] { gridRooms, gridBills, gridIncidents })
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Color.FromArgb(246, 246, 250);
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.GridColor = Color.FromArgb(216, 221, 230);
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = Px(34);
            grid.RowTemplate.Height = Px(35);
            foreach (DataGridViewRow row in grid.Rows) row.Height = Px(35);
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToResizeRows = false;
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle {
                BackColor = Color.FromArgb(231,237,248), ForeColor = Color.FromArgb(35,40,48),
                SelectionBackColor = Color.FromArgb(231,237,248), SelectionForeColor = Color.FromArgb(35,40,48),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold), Padding = new Padding(Px(10),0,0,0), WrapMode = DataGridViewTriState.False
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle {
                BackColor = Color.White, ForeColor = Color.FromArgb(45,51,59),
                SelectionBackColor = Color.FromArgb(189,221,248), SelectionForeColor = Color.FromArgb(35,60,80),
                Padding = new Padding(Px(10),0,0,0), WrapMode = DataGridViewTriState.False
            };
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249,249,253);
        }
        pnlFooter.Padding = new Padding(0,Px(12),0,0);
        lblMessage.ForeColor = Color.FromArgb(100,116,139);
        foreach (var button in new[] { btnRefresh, btnReport })
        {
            button.Size = new Size(Px(button == btnReport ? 140 : 104),Px(36));
            button.Margin = new Padding(Px(9),0,0,0);
            button.FlatAppearance.BorderColor = Color.FromArgb(166,175,190);
        }
        btnReport.FlatAppearance.BorderSize = 0;
        btnRefresh.BackColor = BackColor;
    }
    private void PersonalStatusFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (sender is not DataGridView grid || e.RowIndex < 0 || grid.Columns[e.ColumnIndex].DataPropertyName != "TrangThai") return;
        var status = Convert.ToString(e.Value) ?? "";
        var done = status is "Hiệu lực" or "Đã thanh toán" or "Đã xử lý" or "Đã thu";
        var warning = status is "Chưa thanh toán" or "Chưa thu" or "Đang xử lý" or "Mới" or "Mới tiếp nhận" or "Đang mở" or "Thanh toán một phần";
        e.CellStyle.BackColor = done ? Color.FromArgb(232,246,234) : warning ? Color.FromArgb(255,244,224) : Color.FromArgb(237,240,245);
        e.CellStyle.ForeColor = done ? Color.FromArgb(53,104,63) : warning ? Color.FromArgb(170,104,25) : Color.FromArgb(100,112,128);
        e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
        e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
    }
    private void UpdateSectionCount()
    {
        var grid = tabs.SelectedTab == tabBills ? gridBills : tabs.SelectedTab == tabIncidents ? gridIncidents : gridRooms;
        var unit = tabs.SelectedTab == tabBills ? "hóa đơn" : tabs.SelectedTab == tabIncidents ? "báo hỏng" : "hợp đồng";
        lblMessage.Text = $"{grid.Rows.Count} {unit} • Chỉ hiển thị dữ liệu của bạn";
    }
}
