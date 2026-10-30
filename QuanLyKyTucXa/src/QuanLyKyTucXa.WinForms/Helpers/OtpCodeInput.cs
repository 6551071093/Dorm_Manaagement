namespace QuanLyKyTucXa.WinForms.Helpers;

public class OtpCodeInput : Panel
{
    private bool _aligning;
    public OtpCodeInput() { BackColor=Color.White; }
    public override Size GetPreferredSize(Size proposedSize) => new(0, Scale(36));
    private int Scale(int value) => (int)Math.Round(value*DeviceDpi/96.0);
    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        if(_aligning)return;
        var text=Controls.OfType<TextBox>().FirstOrDefault();
        var button=Controls.OfType<Button>().FirstOrDefault();
        if(text is null || button is null)return;
        _aligning=true;
        try
        {
            text.Dock=button.Dock=DockStyle.None;
            var width=Math.Max(Scale(144),TextRenderer.MeasureText(button.Text,button.Font).Width+Scale(24));
            width=Math.Min(width,Math.Max(1,ClientSize.Width-Scale(80)));
            var height=Math.Min(Scale(36),ClientSize.Height);
            button.SetBounds(Math.Max(0,ClientSize.Width-width),(ClientSize.Height-height)/2,width,height);
            var textHeight=text.PreferredHeight;
            text.SetBounds(0,Math.Max(0,(ClientSize.Height-textHeight)/2),Math.Max(1,button.Left-Scale(12)),textHeight);
        }
        finally { _aligning=false; }
    }
    protected override void OnControlAdded(ControlEventArgs e)
    {
        base.OnControlAdded(e);
        if(e.Control is not null)e.Control.TextChanged+=ChildTextChanged;
        PerformLayout();
    }
    protected override void OnControlRemoved(ControlEventArgs e)
    {
        if(e.Control is not null)e.Control.TextChanged-=ChildTextChanged;
        base.OnControlRemoved(e);
    }
    private void ChildTextChanged(object? sender,EventArgs e) => PerformLayout();
}