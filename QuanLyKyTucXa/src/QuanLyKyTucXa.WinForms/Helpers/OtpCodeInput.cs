namespace QuanLyKyTucXa.WinForms.Helpers;

public class OtpCodeInput : Panel
{
    public OtpCodeInput(){BackColor=Color.White;}
    public override Size GetPreferredSize(Size proposedSize)=>new(0,(int)Math.Ceiling(28*DeviceDpi/96.0));
}
