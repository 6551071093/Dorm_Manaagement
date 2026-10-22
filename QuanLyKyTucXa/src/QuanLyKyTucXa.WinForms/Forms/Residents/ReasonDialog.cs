using System.Windows.Forms;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

internal sealed partial class ReasonDialog : Form
{
    public string Reason => txtReason.Text.Trim();
    public ReasonDialog() => InitializeComponent();
}
