#nullable enable
namespace QuanLyKyTucXa.WinForms.Forms.Auth;
partial class RegisterDialog
{
    private System.ComponentModel.IContainer? components;
    private Panel root = null!, fields = null!, heading = null!, footer = null!, actions = null!;
    private Label lblTitle = null!, lblSubtitle = null!, lblNotice = null!, lblMessage = null!, lblFooter = null!;
    private LoginActionButton btnSubmit = null!, btnCancel = null!;
    private CheckBox chkShowPassword = null!;
    private LoginGlyph homeGlyph = null!;
    private TextBox txtUsername = null!; private Label label0 = null!; private LoginInputPanel box0 = null!; private LoginGlyph glyph0 = null!;
private TextBox txtFullName = null!; private Label label1 = null!; private LoginInputPanel box1 = null!; private LoginGlyph glyph1 = null!;
private TextBox txtPassword = null!; private Label label2 = null!; private LoginInputPanel box2 = null!; private LoginGlyph glyph2 = null!;
private TextBox txtConfirm = null!; private Label label3 = null!; private LoginInputPanel box3 = null!; private LoginGlyph glyph3 = null!;
    protected override void Dispose(bool disposing) { if (disposing) components?.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        root = new Panel(); fields = new Panel(); heading = new Panel(); footer = new Panel(); actions = new Panel();
        lblTitle = new Label(); lblSubtitle = new Label(); lblNotice = new Label(); lblMessage = new Label(); lblFooter = new Label();
        btnSubmit = new LoginActionButton(); btnCancel = new LoginActionButton(); chkShowPassword = new CheckBox(); homeGlyph = new LoginGlyph();
        txtUsername = new TextBox(); label0 = new Label(); box0 = new LoginInputPanel(); glyph0 = new LoginGlyph();
txtFullName = new TextBox(); label1 = new Label(); box1 = new LoginInputPanel(); glyph1 = new LoginGlyph();
txtPassword = new TextBox(); label2 = new Label(); box2 = new LoginInputPanel(); glyph2 = new LoginGlyph();
txtConfirm = new TextBox(); label3 = new Label(); box3 = new LoginInputPanel(); glyph3 = new LoginGlyph();
        SuspendLayout(); root.SuspendLayout(); fields.SuspendLayout(); heading.SuspendLayout(); footer.SuspendLayout(); actions.SuspendLayout();
        heading.Dock = DockStyle.Top; heading.Height = 130; heading.BackColor = Color.FromArgb(23, 101, 194);
        homeGlyph.Kind = LoginGlyphKind.Home; homeGlyph.ForeColor = Color.FromArgb(23, 101, 194);
        homeGlyph.Location = new Point(54, 35); homeGlyph.Size = new Size(60, 60);
        lblTitle.Text = "Đăng ký tài khoản"; lblTitle.Font = new Font("Segoe UI", 23F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White; lblTitle.Location = new Point(132, 30); lblTitle.Size = new Size(530, 46);
        lblSubtitle.Text = "Tạo tài khoản sinh viên KTX Home"; lblSubtitle.Font = new Font("Segoe UI", 10.5F);
        lblSubtitle.ForeColor = Color.White; lblSubtitle.Location = new Point(134, 77); lblSubtitle.Size = new Size(500, 25);
        heading.Controls.Add(homeGlyph); heading.Controls.Add(lblTitle); heading.Controls.Add(lblSubtitle);
        footer.Dock = DockStyle.Bottom; footer.Height = 64; footer.BackColor = Color.FromArgb(243, 247, 254);
        lblFooter.Text = "Đã có tài khoản? Chọn Quay lại để đăng nhập."; lblFooter.Font = new Font("Segoe UI", 9.5F);
        lblFooter.ForeColor = Color.FromArgb(85, 111, 143); lblFooter.Dock = DockStyle.Fill; lblFooter.TextAlign = ContentAlignment.MiddleCenter;
        footer.Controls.Add(lblFooter);
        root.Dock = DockStyle.Fill; root.BackColor = Color.White;
        fields.Location = new Point(52, 16); fields.Size = new Size(606, 526);
        lblNotice.Text = "Tài khoản mới cần Admin liên kết hồ sơ sinh viên và kích hoạt trước khi sử dụng."; lblNotice.Location = new Point(2, 0); lblNotice.Size = new Size(602, 52);
        lblNotice.Padding = new Padding(12, 9, 12, 5); lblNotice.BackColor = Color.FromArgb(239, 246, 255);
        lblNotice.ForeColor = Color.FromArgb(54, 91, 137); lblNotice.Font = new Font("Segoe UI", 9.5F);
        fields.Controls.Add(lblNotice);
        label0.Text = "Tên đăng nhập"; label0.Font = new Font("Segoe UI", 10F, FontStyle.Bold); label0.Location = new Point(2, 64); label0.Size = new Size(602, 24);
box0.Location = new Point(0, 88); box0.Size = new Size(606, 52); box0.TabIndex = 0;
txtUsername.Name = "txtUsername"; txtUsername.BorderStyle = BorderStyle.None; txtUsername.Location = new Point(48, 15); txtUsername.Size = new Size(538, 24); txtUsername.Font = new Font("Segoe UI", 11F); txtUsername.TabIndex = 0; txtUsername.MaxLength = 50; txtUsername.PlaceholderText = "Chọn tên đăng nhập";
glyph0.Kind = LoginGlyphKind.User; glyph0.ForeColor = Color.FromArgb(109, 128, 151); glyph0.Location = new Point(21, 16); glyph0.Size = new Size(18, 18);
box0.Controls.Add(txtUsername); box0.Controls.Add(glyph0); fields.Controls.Add(label0); fields.Controls.Add(box0);
label1.Text = "Họ và tên"; label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold); label1.Location = new Point(2, 148); label1.Size = new Size(602, 24);
box1.Location = new Point(0, 172); box1.Size = new Size(606, 52); box1.TabIndex = 1;
txtFullName.Name = "txtFullName"; txtFullName.BorderStyle = BorderStyle.None; txtFullName.Location = new Point(48, 15); txtFullName.Size = new Size(538, 24); txtFullName.Font = new Font("Segoe UI", 11F); txtFullName.TabIndex = 0; txtFullName.MaxLength = 100; txtFullName.PlaceholderText = "Nhập họ và tên đầy đủ";
glyph1.Kind = LoginGlyphKind.User; glyph1.ForeColor = Color.FromArgb(109, 128, 151); glyph1.Location = new Point(21, 16); glyph1.Size = new Size(18, 18);
box1.Controls.Add(txtFullName); box1.Controls.Add(glyph1); fields.Controls.Add(label1); fields.Controls.Add(box1);
label2.Text = "Mật khẩu"; label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold); label2.Location = new Point(2, 232); label2.Size = new Size(602, 24);
box2.Location = new Point(0, 256); box2.Size = new Size(606, 52); box2.TabIndex = 2;
txtPassword.Name = "txtPassword"; txtPassword.BorderStyle = BorderStyle.None; txtPassword.Location = new Point(48, 15); txtPassword.Size = new Size(538, 24); txtPassword.Font = new Font("Segoe UI", 11F); txtPassword.TabIndex = 0; txtPassword.MaxLength = 128; txtPassword.PlaceholderText = "Tạo mật khẩu từ 8 đến 128 ký tự";
txtPassword.UseSystemPasswordChar = true;
glyph2.Kind = LoginGlyphKind.Lock; glyph2.ForeColor = Color.FromArgb(109, 128, 151); glyph2.Location = new Point(21, 16); glyph2.Size = new Size(18, 18);
box2.Controls.Add(txtPassword); box2.Controls.Add(glyph2); fields.Controls.Add(label2); fields.Controls.Add(box2);
label3.Text = "Xác nhận mật khẩu"; label3.Font = new Font("Segoe UI", 10F, FontStyle.Bold); label3.Location = new Point(2, 316); label3.Size = new Size(602, 24);
box3.Location = new Point(0, 340); box3.Size = new Size(606, 52); box3.TabIndex = 3;
txtConfirm.Name = "txtConfirm"; txtConfirm.BorderStyle = BorderStyle.None; txtConfirm.Location = new Point(48, 15); txtConfirm.Size = new Size(538, 24); txtConfirm.Font = new Font("Segoe UI", 11F); txtConfirm.TabIndex = 0; txtConfirm.MaxLength = 128; txtConfirm.PlaceholderText = "Nhập lại mật khẩu";
txtConfirm.UseSystemPasswordChar = true;
glyph3.Kind = LoginGlyphKind.Lock; glyph3.ForeColor = Color.FromArgb(109, 128, 151); glyph3.Location = new Point(21, 16); glyph3.Size = new Size(18, 18);
box3.Controls.Add(txtConfirm); box3.Controls.Add(glyph3); fields.Controls.Add(label3); fields.Controls.Add(box3);
        chkShowPassword.Text = "Hiện mật khẩu"; chkShowPassword.AutoSize = true; chkShowPassword.Font = new Font("Segoe UI", 10F);
        chkShowPassword.Location = new Point(2, 401); chkShowPassword.TabIndex = 4; chkShowPassword.CheckedChanged += ShowPassword_Changed;
        actions.Location = new Point(0, 436); actions.Size = new Size(606, 50);
        btnSubmit.Primary = true; btnSubmit.Text = "Đăng ký tài khoản"; btnSubmit.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        btnSubmit.Location = new Point(0, 0); btnSubmit.Size = new Size(458, 49); btnSubmit.TabIndex = 5; btnSubmit.Click += Submit_Click;
        btnCancel.Text = "Quay lại"; btnCancel.Font = new Font("Segoe UI", 10.5F);
        btnCancel.Location = new Point(468, 0); btnCancel.Size = new Size(138, 49); btnCancel.TabIndex = 6; btnCancel.Click += Cancel_Click;
        actions.Controls.Add(btnSubmit); actions.Controls.Add(btnCancel);
        lblMessage.Location = new Point(2, 492); lblMessage.Size = new Size(602, 32); lblMessage.ForeColor = Color.Firebrick;
        lblMessage.Font = new Font("Segoe UI", 9F);
        fields.Controls.Add(chkShowPassword); fields.Controls.Add(actions); fields.Controls.Add(lblMessage); root.Controls.Add(fields);
        Controls.Add(root); Controls.Add(footer); Controls.Add(heading);
        Font = new Font("Segoe UI", 10.5F); BackColor = Color.White; ForeColor = Color.FromArgb(14, 34, 64);
        AutoScaleDimensions = new SizeF(96F, 96F); AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(710, 740); StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog; MinimizeBox = false; MaximizeBox = false; ShowIcon = false;
        AcceptButton = btnSubmit; CancelButton = btnCancel; Text = "KTX Home – Đăng ký tài khoản"; Name = "RegisterDialog";
        FormClosing += Busy_FormClosing;
        actions.ResumeLayout(false); heading.ResumeLayout(false); footer.ResumeLayout(false);
        fields.ResumeLayout(false); fields.PerformLayout(); root.ResumeLayout(false); ResumeLayout(false);
    }
}
