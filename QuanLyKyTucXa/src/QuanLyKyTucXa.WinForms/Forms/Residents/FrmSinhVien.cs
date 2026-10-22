using System.Drawing;
using System.Windows.Forms;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public partial class FrmSinhVien : Form
{
    public event EventHandler? ReturnHomeRequested;
    public FrmSinhVien() : this(string.Empty) { }

    public FrmSinhVien(string connectionString)
    {
        _connectionString = connectionString;
        InitializeComponent();
        ConfigureDatabaseEvents();
    }

    private void Close_Click(object? sender, EventArgs e) =>
        ReturnHomeRequested?.Invoke(this, EventArgs.Empty);

    private void FormatStudentStatusCell(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || dgvSinhVien.Columns[e.ColumnIndex].DataPropertyName != "TrangThai") return;
        var status = Convert.ToString(e.Value);
        e.CellStyle.BackColor = status switch
        {
            "Đang ở" => Color.FromArgb(232, 246, 234),
            "Chờ xếp phòng" => Color.FromArgb(255, 244, 224),
            "Đã rời KTX" => Color.FromArgb(237, 240, 245),
            _ => Color.White
        };
        e.CellStyle.ForeColor = status switch
        {
            "Đang ở" => Color.FromArgb(53, 104, 63),
            "Chờ xếp phòng" => Color.FromArgb(170, 104, 25),
            _ => Color.FromArgb(100, 112, 128)
        };
        e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
        e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
    }
}
