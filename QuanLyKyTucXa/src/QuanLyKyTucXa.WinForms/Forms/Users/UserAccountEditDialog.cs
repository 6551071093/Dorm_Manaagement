using QuanLyKyTucXa.Infrastructure.Services;

namespace QuanLyKyTucXa.WinForms.Forms.Users;

public partial class UserAccountEditDialog : Form
{
    private UserAccountEdit? _value;
    private NguoiDungService? _service;
    private bool _busy;
    public UserAccountEditDialog() => InitializeComponent();
    public UserAccountEditDialog(UserAccountEdit value, List<StudentAccountChoice> choices, NguoiDungService service) : this()
    {
        _value = value; _service = service;
        Text = value.Id is null ? "Thêm tài khoản" : "Sửa tài khoản";
        lblTitle.Text = Text;
        txtUsername.Text = value.Username; txtUsername.ReadOnly = value.Id is not null;
        txtFullName.Text = value.FullName;
        cboRole.FormattingEnabled = true;
        cboRole.Items.AddRange(new object[] { "Admin", "NhanVien", "SinhVien" });
        cboRole.Format += (_, e) => e.Value = e.ListItem?.ToString() switch {
            "Admin" => "Admin — Quản trị viên", "NhanVien" => "NhanVien — Nhân viên", "SinhVien" => "SinhVien — Sinh viên", _ => e.ListItem
        };
        cboRole.SelectedItem = value.Role;
        cboStudent.FormattingEnabled = true;
        cboStudent.DisplayMember = nameof(StudentAccountChoice.Name);
        cboStudent.ValueMember = nameof(StudentAccountChoice.Id);
        cboStudent.Format += (_, e) => { if (e.ListItem is StudentAccountChoice s) e.Value = s.Id.Length == 0 ? s.Name : $"{s.Id} — {s.Name}"; };
        var all = new List<StudentAccountChoice> { new("", "Chọn hồ sơ sinh viên") }; all.AddRange(choices);
        cboStudent.DataSource = all;
        if (value.StudentId is not null) cboStudent.SelectedValue = value.StudentId;
        chkEnabled.Checked = value.Enabled;
        cboRole.SelectedIndexChanged += (_, _) => UpdateStudentChoice();
        UpdateStudentChoice();
        lblMessage.Text = value.Id is null ? "Mật khẩu mới: 8–128 ký tự." : "Để trống mật khẩu mới nếu muốn giữ mật khẩu hiện tại.";
    }
    private void UpdateStudentChoice() => cboStudent.Enabled = Convert.ToString(cboRole.SelectedItem) == "SinhVien";
    private async void Save_Click(object? sender, EventArgs e)
    {
        if (_busy || _value is null || _service is null) return;
        _value.Username = txtUsername.Text.Trim(); _value.FullName = txtFullName.Text;
        _value.Role = Convert.ToString(cboRole.SelectedItem) ?? "";
        _value.Enabled = chkEnabled.Checked;
        _value.StudentId = (cboStudent.SelectedItem as StudentAccountChoice)?.Id;
        _value.NewPassword = txtNewPassword.Text.Length == 0 ? null : txtNewPassword.Text;
        _busy = true;
        tlpFields.Enabled = btnSave.Enabled = btnCancel.Enabled = false;
        lblMessage.Text = "Đang lưu...";
        var ok = false;
        try { await _service.SaveAsync(_value); ok = true; }
        catch (Exception ex) { lblMessage.Text = ex is InvalidOperationException or UnauthorizedAccessException ? ex.Message : "Không lưu được tài khoản. Kiểm tra kết nối và thử lại."; }
        finally { _busy = false; tlpFields.Enabled = btnSave.Enabled = btnCancel.Enabled = true; }
        if (ok) { DialogResult = DialogResult.OK; Close(); }
    }
    private void Cancel_Click(object? sender, EventArgs e) { if (!_busy) Close(); }
    private void Auth_FormClosing(object? sender, FormClosingEventArgs e) { if (_busy) e.Cancel = true; }
}