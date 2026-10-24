using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace QuanLyKyTucXa.WinForms.Helpers;

// Viền ngoài do LoginInputPanel vẽ. Vẫn dùng lịch, bàn phím và validation gốc.
public class CleanDateTimePicker : DateTimePicker
{
    public CleanDateTimePicker()
    {
        BackColor=Color.White;
        ForeColor=Color.FromArgb(25,50,78);
        CalendarMonthBackground=Color.White;
        CalendarTitleBackColor=Color.FromArgb(23,101,194);
        CalendarTitleForeColor=Color.White;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var parameters=base.CreateParams;
            parameters.Style&=~0x00800000; // WS_BORDER
            parameters.ExStyle&=~0x00000200; // WS_EX_CLIENTEDGE
            return parameters;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRectangle { public int Left, Top, Right, Bottom; }

    [StructLayout(LayoutKind.Sequential)]
    private struct PickerInfo
    {
        public uint Size;
        public NativeRectangle Check;
        public uint CheckState;
        public NativeRectangle Button;
        public uint ButtonState;
        public nint EditHandle, UpDownHandle, CalendarHandle;
    }

    [DllImport("user32.dll", EntryPoint="SendMessageW", ExactSpelling=true)]
    private static extern nint GetPickerInfo(nint handle, uint message, nint parameter, ref PickerInfo info);

    private Rectangle GetCalendarButtonBounds(float scale)
    {
        var info=new PickerInfo {Size=(uint)Marshal.SizeOf<PickerInfo>()};
        GetPickerInfo(Handle,0x100E,0,ref info); // DTM_GETDATETIMEPICKERINFO
        var button=Rectangle.Intersect(ClientRectangle,Rectangle.FromLTRB(info.Button.Left,info.Button.Top,info.Button.Right,info.Button.Bottom));
        if(button.Width<=0 || button.Height<=0)
        {
            var width=(int)Math.Ceiling(32*scale);
            button=new Rectangle(Math.Max(0,ClientSize.Width-width),0,Math.Min(width,ClientSize.Width),ClientSize.Height);
        }
        return button;
    }

    protected override void OnGotFocus(EventArgs e) {base.OnGotFocus(e);Invalidate();}
    protected override void OnLostFocus(EventArgs e) {base.OnLostFocus(e);Invalidate();}
    protected override void OnValueChanged(EventArgs e) {base.OnValueChanged(e);Invalidate();}
    protected override void OnDropDown(EventArgs e) {base.OnDropDown(e);Invalidate();}
    protected override void OnCloseUp(EventArgs e) {base.OnCloseUp(e);Invalidate();}
    protected override void OnEnabledChanged(EventArgs e) {base.OnEnabledChanged(e);Invalidate();}

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if(m.Msg!=0x000F || !IsHandleCreated || IsDisposed || ClientSize.Width<8 || ClientSize.Height<8)return;
        using var graphics=Graphics.FromHwnd(Handle);
        var scale=DeviceDpi/96f;
        var button=GetCalendarButtonBounds(scale);
        var fore=Enabled?Color.FromArgb(74,107,148):Color.FromArgb(133,147,166);
        if(!Enabled && !ShowCheckBox)
        {
            graphics.Clear(Color.White);
            var text=new Rectangle(2,0,Math.Max(1,button.Left-4),ClientSize.Height);
            TextRenderer.DrawText(graphics,Text,Font,text,fore,Color.White,
                TextFormatFlags.Left|TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis|TextFormatFlags.NoPrefix);
        }
        using(var edge=new Pen(Color.White,3*scale))
            graphics.DrawRectangle(edge,0,0,ClientSize.Width-1,ClientSize.Height-1);
        if(ShowUpDown)return;
        using var background=new SolidBrush(Color.White);
        // Xóa toàn bộ nút Windows, kể cả đường viền/focus sát mép trái.
        var clearLeft=Math.Max(0,button.Left-(int)Math.Ceiling(3*scale));
        graphics.FillRectangle(background,clearLeft,0,ClientSize.Width-clearLeft,ClientSize.Height);
        graphics.SmoothingMode=SmoothingMode.AntiAlias;
        var width=12*scale;
        var height=11*scale;
        var x=button.Left+(button.Width-width)/2;
        var y=(ClientSize.Height-height)/2;
        using var pen=new Pen(fore,1.1f*scale){StartCap=LineCap.Round,EndCap=LineCap.Round,LineJoin=LineJoin.Round};
        graphics.DrawRectangle(pen,x,y,width,height);
        graphics.DrawLine(pen,x,y+3*scale,x+width,y+3*scale);
        graphics.DrawLine(pen,x+3*scale,y-1.5f*scale,x+3*scale,y+1.5f*scale);
        graphics.DrawLine(pen,x+9*scale,y-1.5f*scale,x+9*scale,y+1.5f*scale);
        using var dot=new SolidBrush(fore);
        for(var row=0;row<2;row++)
            for(var column=0;column<3;column++)
                graphics.FillRectangle(dot,x+(2+column*3.5f)*scale,y+(5+row*3)*scale,1.3f*scale,1.3f*scale);
    }
}
