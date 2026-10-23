using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public sealed partial class ApprovalDialog : Form
{
    public BedChoice? Bed => _beds.SelectedItem as BedChoice;
    public DateTime Start => _start.Value.Date;
    public DateTime End => _end.Value.Date;
    public decimal Deposit => _deposit.Value;

    public ApprovalDialog()
    {
        InitializeComponent();
        QuanLyKyTucXa.WinForms.Helpers.DialogLayout.ConfigureFixed(this, btnApprove, btnCancel);
        _start.Value = DateTime.Today;
        _end.Value = DateTime.Today.AddMonths(6);
        _beds.FormattingEnabled = true;
        _beds.Format += (_, e) => { if (e.ListItem is BedChoice bed) e.Value = $"{bed.MaPhong} — {bed.TenGiuong} ({bed.MaGiuong})"; };
        btnApprove.DialogResult = DialogResult.None;
        btnApprove.Click += (_, _) => {
            if (Bed is null) { MessageBox.Show(this, "Vui lòng chọn giường."); return; }
            if (End <= Start) { MessageBox.Show(this, "Ngày kết thúc phải sau ngày bắt đầu."); _end.Focus(); return; }
            if (End < DateTime.Today) { MessageBox.Show(this, "Hợp đồng không được đã kết thúc."); return; }
            DialogResult = DialogResult.OK;
        };
    }

    public ApprovalDialog(DangKyRow row, List<BedChoice> beds) : this()
    {
        Text = $"Duyệt đơn #{row.MaDangKy}";
        lblStudent.Text = $"{row.MaSv} — {row.HoTen}";
        _beds.DataSource = beds;
    }
}
