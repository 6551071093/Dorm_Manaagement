namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class FrmCaNhan
{
    private int Px(int value) => (int)Math.Round(value * DeviceDpi / 96.0);
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
        tlpRoot.Padding = new Padding(Px(24), Px(14), Px(24), Px(12));
        tlpRoot.RowStyles[0].Height = Px(44);
        tlpRoot.RowStyles[1].Height = Px(96);
        tlpRoot.RowStyles[2].Height = 0;
        tlpRoot.RowStyles[4].Height = Px(56);
        profileCard.Margin = new Padding(0, 0, 0, Px(12));
        profileCard.Padding = Padding.Empty;
        profileText.Padding = Padding.Empty;
        avatar.Dock = profileStatus.Dock = profileText.Dock = DockStyle.None;
        profileCard.SizeChanged -= ProfileCard_SizeChanged;
        profileCard.SizeChanged += ProfileCard_SizeChanged;
        LayoutProfileCard();
        navigation.Margin = new Padding(0, 0, 0, Px(12));
        foreach (var button in new[] { navRooms, navBills, navIncidents })
        {
            button.Font = new Font("Segoe UI", 10F);
            button.Size = new Size(Px(button == navRooms ? 160 : 150), Px(36));
            button.Margin = Padding.Empty;
        }
        foreach (var grid in new[] { gridRooms, gridBills, gridIncidents })
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Color.FromArgb(230, 237, 248);
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = Px(38);
            grid.RowTemplate.Height = Px(38);
            foreach (DataGridViewRow row in grid.Rows) row.Height = Px(38);
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToResizeRows = false;
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle {
                BackColor = Color.FromArgb(233,241,253), ForeColor = Color.FromArgb(30,74,128),
                SelectionBackColor = Color.FromArgb(233,241,253), SelectionForeColor = Color.FromArgb(30,74,128),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Padding = new Padding(Px(14),0,0,0), WrapMode = DataGridViewTriState.False
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle {
                BackColor = Color.White, ForeColor = Color.FromArgb(30,60,96),
                SelectionBackColor = Color.FromArgb(215,233,254), SelectionForeColor = Color.FromArgb(20,60,106),
                Font = new Font("Segoe UI", 10F), Padding = new Padding(Px(14),0,0,0), WrapMode = DataGridViewTriState.False
            };
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248,251,255);
            foreach (DataGridViewColumn column in grid.Columns)
            {
                var property = column.DataPropertyName;
                column.MinimumWidth = Px(property is "Thang" or "Nam" ? 68 : property == "TienPhong" ? 160 : 115);
                column.FillWeight = property is "TieuDe" or "TenToa" ? 160 : property is "Thang" or "Nam" ? 60 : 100;
                if (property is "BatDau" or "KetThuc" or "HanThanhToan" or "NgayBao") column.DefaultCellStyle.Format = "dd/MM/yyyy";
                if (property is "TienPhong" or "TongTien" or "DaThu" or "ConNo") column.DefaultCellStyle.Format = "N0";
            }
        }
        pnlFooter.Padding = new Padding(0,Px(12),0,0);
        foreach (var button in new[] { btnRefresh, btnReport })
        {
            button.Size = new Size(Px(button == btnReport ? 140 : 104),Px(36));
            button.Margin = new Padding(Px(8),0,0,0);
        }
        LayoutEmptyState();
    }
    private void ProfileCard_SizeChanged(object? sender, EventArgs e) => LayoutProfileCard();
    private void LayoutProfileCard()
    {
        var inset = Px(18);
        avatar.SetBounds(inset, Math.Max(0,(profileCard.Height-Px(52))/2), Px(52), Px(52));
        profileStatus.SetBounds(Math.Max(inset,profileCard.Width-inset-Px(132)), Math.Max(0,(profileCard.Height-Px(26))/2), Px(132), Px(26));
        var x = avatar.Right + Px(14);
        profileText.SetBounds(x,Math.Max(0,(profileCard.Height-Px(52))/2),Math.Max(0,profileStatus.Left-Px(12)-x),Px(52));
        lblName.Height = Px(28);
    }
    private void LayoutEmptyState()
    {
        if (emptyPanel is null || gridRooms is null || emptySymbol is null) return;
        var header = ActiveGrid.ColumnHeadersHeight + 1;
        var scrollbar = ActiveGrid.Controls.OfType<HScrollBar>().FirstOrDefault(s => s.Visible);
        emptyPanel.SetBounds(1,header,Math.Max(0,tableCard.ClientSize.Width-2),Math.Max(0,tableCard.ClientSize.Height-header-1-(scrollbar?.Height??0)));
        var y = Math.Max(Px(8),(emptyPanel.Height-Px(118))/2);
        emptySymbol.SetBounds(Math.Max(0,(emptyPanel.Width-Px(48))/2),y,Px(48),Px(48));
        lblEmptyTitle.SetBounds(Px(12),y+Px(56),Math.Max(0,emptyPanel.Width-Px(24)),Px(28));
        lblEmptySubtitle.SetBounds(Px(16),y+Px(86),Math.Max(0,emptyPanel.Width-Px(32)),Px(44));
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
}
