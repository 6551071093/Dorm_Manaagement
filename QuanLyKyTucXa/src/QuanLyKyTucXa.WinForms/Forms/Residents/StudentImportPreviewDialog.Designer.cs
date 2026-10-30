#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Residents;

partial class StudentImportPreviewDialog
{
    private System.ComponentModel.IContainer? components;
    private TableLayoutPanel tlpRoot = null!;
    private Label lblSummary = null!;
    private DataGridView gridPreview = null!;
    private TextBox txtErrors = null!;
    private FlowLayoutPanel flpActions = null!;
    private Button btnConfirm = null!;
    private Button btnCancel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        tlpRoot = new TableLayoutPanel();
        lblSummary = new Label();
        gridPreview = new DataGridView();
        txtErrors = new TextBox();
        flpActions = new FlowLayoutPanel();
        btnConfirm = new Button();
        btnCancel = new Button();
        tlpRoot.SuspendLayout();
        flpActions.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)gridPreview).BeginInit();
        SuspendLayout();
        tlpRoot.Dock = DockStyle.Fill;
        tlpRoot.Padding = new Padding(16);
        tlpRoot.ColumnCount = 1;
        tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tlpRoot.RowCount = 4;
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tlpRoot.Controls.Add(lblSummary, 0, 0);
        tlpRoot.Controls.Add(gridPreview, 0, 1);
        tlpRoot.Controls.Add(txtErrors, 0, 2);
        tlpRoot.Controls.Add(flpActions, 0, 3);
        lblSummary.Dock = DockStyle.Fill;
        lblSummary.TextAlign = ContentAlignment.MiddleLeft;
        gridPreview.Name = "gridPreview";
        gridPreview.Dock = DockStyle.Fill;
        gridPreview.ReadOnly = true;
        gridPreview.AllowUserToAddRows = false;
        gridPreview.AllowUserToDeleteRows = false;
        gridPreview.RowHeadersVisible = false;
        gridPreview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        gridPreview.BackgroundColor = Color.White;
        gridPreview.RowTemplate.Height = 30;
        txtErrors.Name = "txtErrors";
        txtErrors.Dock = DockStyle.Fill;
        txtErrors.Multiline = true;
        txtErrors.ReadOnly = true;
        txtErrors.ScrollBars = ScrollBars.Vertical;
        flpActions.AutoSize = true; flpActions.AutoSizeMode = AutoSizeMode.GrowAndShrink; flpActions.Dock = DockStyle.Fill;
        flpActions.FlowDirection = FlowDirection.RightToLeft;
        flpActions.WrapContents = false;
        flpActions.Padding = new Padding(0, 6, 0, 0);
        flpActions.Controls.Add(btnCancel);
        flpActions.Controls.Add(btnConfirm);
        btnCancel.Text = "Hủy";
        btnCancel.Size = new Size(90, 36);
        btnCancel.DialogResult = DialogResult.Cancel;
        btnConfirm.Text = "Xác nhận nhập";
        btnConfirm.Size = new Size(150, 36);
        btnConfirm.BackColor = Color.FromArgb(23, 101, 183);
        btnConfirm.ForeColor = Color.White;
        btnConfirm.FlatStyle = FlatStyle.Flat;
        btnConfirm.DialogResult = DialogResult.OK;
        AcceptButton = btnConfirm;
        CancelButton = btnCancel;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 10F);
        ClientSize = new Size(1000, 650);
        MinimumSize = new Size(640, 450);
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        Name = "StudentImportPreviewDialog";
        Text = "Xem trước nhập Excel sinh viên";
        Controls.Add(tlpRoot);
        ((System.ComponentModel.ISupportInitialize)gridPreview).EndInit();
        flpActions.ResumeLayout(false);
        tlpRoot.ResumeLayout(false);
        tlpRoot.PerformLayout();
        ResumeLayout(false);
    }
}
