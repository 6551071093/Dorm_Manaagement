using QuanLyKyTucXa.Infrastructure.Services;
using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.WinForms.Helpers;

namespace QuanLyKyTucXa.WinForms.Forms.Auth;

public partial class ForgotPasswordDialog : Form
{
    private enum ResetStep {Email,Otp,Password}
    private readonly AuthService? _service;
    private ResetStep _step;
    private bool _busy;
    private int _cooldown;
    private string _email="";
    private string _grant="";
    public ForgotPasswordDialog()
    {
        InitializeComponent();
        ModernDialog.InstallAuth(this,fields,actions,btnSubmit,btnCancel,lblMessage,lblNotice,chkShowPassword,
            new(labelEmail,txtEmail,true),new(label1,otpEntry,true),new(label0,txtUsername,true),new(label2,txtPassword,true),new(label3,txtConfirm,true));
        btnSendOtp.Click+=ResendOtp_Click;
        txtOtp.KeyPress+=(_,e)=>{if(!char.IsControl(e.KeyChar) && !char.IsAsciiDigit(e.KeyChar))e.Handled=true;};
        components ??= new System.ComponentModel.Container();
        var timer=new System.Windows.Forms.Timer(components){Interval=1000};
        timer.Tick+=(_,_)=>{if(_cooldown>0)_cooldown--;UpdateActions();};
        timer.Start();
        Load+=(_,_)=>ShowStep(ResetStep.Email);
        Shown+=(_,_)=>txtEmail.Focus();
        FormClosed+=(_,_)=>{_grant="";txtOtp.Clear();txtPassword.Clear();txtConfirm.Clear();};
    }
    public ForgotPasswordDialog(AuthService service,string username):this(){_service=service;txtUsername.Text=username;}
    private void ShowPassword_Changed(object? sender,EventArgs e)=>txtPassword.UseSystemPasswordChar=txtConfirm.UseSystemPasswordChar=!chkShowPassword.Checked;
    private void UpdateActions()
    {
        btnSubmit.Text=_step switch {ResetStep.Email=>_cooldown>0?$"Gửi OTP ({_cooldown}s)":"Gửi OTP",ResetStep.Otp=>"Xác nhận OTP",_=>"Lưu mật khẩu"};
        btnCancel.Text=_step==ResetStep.Email?"Quay lại":"Nhập lại";
        btnSubmit.Enabled=!_busy && (_step!=ResetStep.Email || _cooldown==0);
        btnSendOtp.Text=_cooldown>0?$"Gửi lại ({_cooldown}s)":"Gửi lại OTP";
        btnSendOtp.Enabled=!_busy && _cooldown==0 && _step==ResetStep.Otp;
    }
    private void SetBusy(bool busy)
    {
        _busy=busy;fields.Enabled=actions.Enabled=!busy;UseWaitCursor=busy;UpdateActions();
        if(!busy && !IsDisposed)
        {
            if(_step==ResetStep.Email)txtEmail.Focus();else if(_step==ResetStep.Otp)txtOtp.Focus();else txtUsername.Focus();
        }
    }
    private void ShowStep(ResetStep step)
    {
        _step=step;lblMessage.Text="";
        ModernDialog.SetFieldVisible(txtEmail,step==ResetStep.Email);
        ModernDialog.SetFieldVisible(otpEntry,step==ResetStep.Otp);
        ModernDialog.SetFieldVisible(txtUsername,step==ResetStep.Password);
        ModernDialog.SetFieldVisible(txtPassword,step==ResetStep.Password);
        ModernDialog.SetFieldVisible(txtConfirm,step==ResetStep.Password);
        chkShowPassword.Visible=step==ResetStep.Password;
        lblNotice.Text=step switch {
            ResetStep.Email=>"Bước 1/3 • Nhập email đã lưu trong tài khoản. Nếu email khớp, OTP sẽ được gửi đến hộp thư đó.",
            ResetStep.Otp=>$"Bước 2/3 • Kiểm tra Hộp thư/Spam của {_email} và nhập OTP 6 chữ số. Mã có hạn 5 phút, chỉ dùng một lần.",
            _=>"Bước 3/3 • Email đã xác nhận. Nhập đúng tài khoản gắn với email đó, mật khẩu mới và xác nhận. Mật khẩu chỉ đổi khi bạn bấm Lưu."};
        UpdateActions();ModernDialog.ResizeToContent(this);
        if(step==ResetStep.Email)txtEmail.Focus();else if(step==ResetStep.Otp)txtOtp.Focus();else txtUsername.Focus();
    }
    private async void Submit_Click(object? sender,EventArgs e)
    {
        if(_busy || _service is null)return;
        SetBusy(true);
        try
        {
            if(_step==ResetStep.Email)
            {
                var email=AccountValidation.Email(txtEmail.Text);
                lblMessage.Text="Đang yêu cầu OTP...";
                await RequestOtpWithCooldownAsync(email);
                _email=email;_grant="";_cooldown=60;txtOtp.Clear();ShowStep(ResetStep.Otp);
            }
            else if(_step==ResetStep.Otp)
            {
                lblMessage.Text="Đang xác nhận OTP...";
                _grant=await _service.VerifyPasswordOtpAsync(_email,txtOtp.Text);
                txtOtp.Clear();txtPassword.Clear();txtConfirm.Clear();ShowStep(ResetStep.Password);
            }
            else
            {
                AccountValidation.Username(txtUsername.Text);PasswordHash.ValidateNew(txtPassword.Text);
                if(txtPassword.Text!=txtConfirm.Text)throw new InvalidOperationException("Mật khẩu xác nhận không trùng khớp.");
                lblMessage.Text="Đang lưu mật khẩu mới...";
                await _service.ResetPasswordWithGrantAsync(txtUsername.Text,_email,_grant,txtPassword.Text);
                _grant="";txtPassword.Clear();txtConfirm.Clear();
                MessageBox.Show(this,"Đã đặt lại mật khẩu thành công.","KTX Home",MessageBoxButtons.OK,MessageBoxIcon.Information);
                SetBusy(false);DialogResult=DialogResult.OK;Close();
            }
        }
        catch(Exception ex)
        {
            lblMessage.Text=ex is InvalidOperationException?ex.Message:"Không hoàn tất được thao tác. Kiểm tra kết nối hoặc liên hệ Admin.";

        }
        finally{if(!IsDisposed)SetBusy(false);}
    }
    private async Task RequestOtpWithCooldownAsync(string email)
    {
        // Lỗi gửi SMTP cũng có thể đã ghi nhận lượt yêu cầu trong SQL.
        // Chặn bấm lại ngay để tránh chuyển bước do yêu cầu bị giới hạn gửi.
        var started=Environment.TickCount64;
        try { await _service!.RequestPasswordOtpAsync(email); }
        catch
        {
            _cooldown=Math.Max(_cooldown,Math.Max(0,60-(int)((Environment.TickCount64-started)/1000)));
            throw;
        }
    }
    private async void ResendOtp_Click(object? sender,EventArgs e)
    {
        if(_busy || _cooldown>0 || _service is null || _step!=ResetStep.Otp)return;
        SetBusy(true);lblMessage.Text="Đang yêu cầu gửi lại OTP...";
        try
        {
            await RequestOtpWithCooldownAsync(_email);_cooldown=60;_grant="";txtOtp.Clear();
            lblMessage.Text="Đã gửi yêu cầu. Nếu email khớp tài khoản và còn lượt gửi, kiểm tra Hộp thư/Spam để nhận mã mới.";
        }
        catch(Exception ex){lblMessage.Text=ex is InvalidOperationException?ex.Message:"Không gửi được OTP. Vui lòng thử lại.";}
        finally{SetBusy(false);}
    }
    private void Cancel_Click(object? sender,EventArgs e)
    {
        if(_busy)return;
        if(_step==ResetStep.Email){Close();return;}
        _grant="";txtOtp.Clear();txtPassword.Clear();txtConfirm.Clear();ShowStep(ResetStep.Email);
    }
    private void Busy_FormClosing(object? sender,FormClosingEventArgs e){if(_busy)e.Cancel=true;}
}
