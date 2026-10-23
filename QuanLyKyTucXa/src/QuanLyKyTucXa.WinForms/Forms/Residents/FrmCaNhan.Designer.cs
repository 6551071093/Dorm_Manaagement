#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Residents;
partial class FrmCaNhan
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel tlpRoot = null!;
    private Label lblTitle = null!;
    private Label lblProfile = null!;
    private GroupBox grpProfile = null!;
    private TabControl tabs = null!;
    private TabPage tabRooms = null!;
    private TabPage tabBills = null!;
    private TabPage tabIncidents = null!;
    private DataGridView gridRooms = null!;
    private DataGridView gridBills = null!;
    private DataGridView gridIncidents = null!;
    private Panel pnlFooter = null!;
    private Label lblMessage = null!;
    private FlowLayoutPanel flpActions = null!;
    private Button btnRefresh = null!;
    private Button btnReport = null!;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        tlpRoot = new TableLayoutPanel(); lblTitle = new Label(); lblProfile = new Label(); grpProfile = new GroupBox();
        tabs = new TabControl(); tabRooms = new TabPage(); tabBills = new TabPage(); tabIncidents = new TabPage();
        gridRooms = new DataGridView(); gridBills = new DataGridView(); gridIncidents = new DataGridView();
        pnlFooter = new Panel(); lblMessage = new Label(); flpActions = new FlowLayoutPanel(); btnRefresh = new Button(); btnReport = new Button();
        tlpRoot.SuspendLayout(); tabs.SuspendLayout(); pnlFooter.SuspendLayout(); flpActions.SuspendLayout(); SuspendLayout();
        tlpRoot.Dock = DockStyle.Fill; tlpRoot.Padding = new Padding(20, 14, 20, 12);
        tlpRoot.ColumnCount = 1; tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tlpRoot.RowCount = 4; tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F)); tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 76F));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        tlpRoot.Controls.Add(lblTitle, 0, 0); tlpRoot.Controls.Add(grpProfile, 0, 1); tlpRoot.Controls.Add(tabs, 0, 2); tlpRoot.Controls.Add(pnlFooter, 0, 3);
        lblTitle.Text = "Thông tin cá nhân"; lblTitle.Dock = DockStyle.Fill; lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold); lblTitle.TextAlign = ContentAlignment.MiddleLeft;
        grpProfile.Text = "Hồ sơ sinh viên"; grpProfile.Dock = DockStyle.Fill; grpProfile.Controls.Add(lblProfile);
        lblProfile.Dock = DockStyle.Fill; lblProfile.TextAlign = ContentAlignment.MiddleLeft; lblProfile.Text = "Đang tải hồ sơ...";
        tabs.Dock = DockStyle.Fill; tabs.Controls.Add(tabRooms); tabs.Controls.Add(tabBills); tabs.Controls.Add(tabIncidents);
        tabRooms.Text = "Phòng / Hợp đồng"; tabRooms.Controls.Add(gridRooms);
        tabBills.Text = "Hóa đơn của tôi"; tabBills.Controls.Add(gridBills);
        tabIncidents.Text = "Báo hỏng của tôi"; tabIncidents.Controls.Add(gridIncidents);
        gridRooms.Dock = DockStyle.Fill; gridRooms.ReadOnly = true; gridRooms.AllowUserToAddRows = false; gridRooms.AllowUserToDeleteRows = false; gridRooms.RowHeadersVisible = false; gridRooms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; gridRooms.RowTemplate.Height = 40; gridRooms.ColumnHeadersHeight = 42; gridRooms.BackgroundColor = Color.White;
        gridBills.Dock = DockStyle.Fill; gridBills.ReadOnly = true; gridBills.AllowUserToAddRows = false; gridBills.AllowUserToDeleteRows = false; gridBills.RowHeadersVisible = false; gridBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; gridBills.RowTemplate.Height = 40; gridBills.ColumnHeadersHeight = 42; gridBills.BackgroundColor = Color.White;
        gridIncidents.Dock = DockStyle.Fill; gridIncidents.ReadOnly = true; gridIncidents.AllowUserToAddRows = false; gridIncidents.AllowUserToDeleteRows = false; gridIncidents.RowHeadersVisible = false; gridIncidents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; gridIncidents.RowTemplate.Height = 40; gridIncidents.ColumnHeadersHeight = 42; gridIncidents.BackgroundColor = Color.White;
        pnlFooter.Dock = DockStyle.Fill; pnlFooter.Padding = new Padding(0, 12, 0, 0); pnlFooter.Controls.Add(lblMessage); pnlFooter.Controls.Add(flpActions);
        lblMessage.Dock = DockStyle.Fill; lblMessage.TextAlign = ContentAlignment.MiddleLeft;
        flpActions.AutoSize = true; flpActions.Dock = DockStyle.Right; flpActions.FlowDirection = FlowDirection.RightToLeft; flpActions.WrapContents = false;
        flpActions.Controls.Add(btnReport); flpActions.Controls.Add(btnRefresh);
        btnRefresh.Text = "Làm mới"; btnRefresh.Size = new Size(104, 36); btnRefresh.FlatStyle = FlatStyle.Flat; btnRefresh.Click += Refresh_Click;
        btnReport.Text = "Gửi báo hỏng"; btnReport.Size = new Size(140, 36); btnReport.BackColor = Color.FromArgb(23, 101, 183); btnReport.ForeColor = Color.White; btnReport.FlatStyle = FlatStyle.Flat; btnReport.Click += Report_Click;
        AutoScaleDimensions = new SizeF(96F, 96F); AutoScaleMode = AutoScaleMode.Dpi; Font = new Font("Segoe UI", 11F);
        BackColor = Color.FromArgb(246, 246, 250); ClientSize = new Size(1100, 620); Name = "FrmCaNhan"; Text = "Thông tin cá nhân"; Controls.Add(tlpRoot);
        flpActions.ResumeLayout(false); pnlFooter.ResumeLayout(false); pnlFooter.PerformLayout(); tabs.ResumeLayout(false); tlpRoot.ResumeLayout(false); ResumeLayout(false);
    }
}