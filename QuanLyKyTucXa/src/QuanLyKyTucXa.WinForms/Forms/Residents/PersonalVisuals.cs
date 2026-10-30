using System.Drawing.Drawing2D;
using QuanLyKyTucXa.WinForms.Forms.Auth;

namespace QuanLyKyTucXa.WinForms.Forms.Residents;

public class PersonalSurface : Panel
{
    public PersonalSurface(){DoubleBuffered=true;BackColor=Color.White;}
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if(Width<3 || Height<3)return;
        using var path=LoginDrawing.Rounded(new RectangleF(0,0,Width,Height),12*DeviceDpi/96f);
        var old=Region;Region=new Region(path);old?.Dispose();Invalidate();
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
        using var path=LoginDrawing.Rounded(new RectangleF(.5f,.5f,Width-1,Height-1),12*DeviceDpi/96f);
        using var pen=new Pen(Color.FromArgb(222,232,246));e.Graphics.DrawPath(pen,path);
    }
}

public class PersonalBadge : Label
{
    [System.ComponentModel.DefaultValue(false)]
    public bool Circle {get;set;}
    public PersonalBadge(){DoubleBuffered=true;AutoSize=false;TextAlign=ContentAlignment.MiddleCenter;}
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(Parent?.BackColor??Color.White);e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
        using var brush=new SolidBrush(BackColor);
        if(Circle)e.Graphics.FillEllipse(brush,1,1,Width-2,Height-2);
        else {using var path=LoginDrawing.Rounded(new RectangleF(1,1,Width-2,Height-2),Height/2f);e.Graphics.FillPath(brush,path);}
        TextRenderer.DrawText(e.Graphics,Text,Font,ClientRectangle,ForeColor,TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis);
    }
}

public class PersonalSectionButton : Button
{
    private bool _selected;
    [System.ComponentModel.DefaultValue(false)]
    public bool Selected {get=>_selected;set{_selected=value;Invalidate();}}
    public PersonalSectionButton(){DoubleBuffered=true;FlatStyle=FlatStyle.Flat;FlatAppearance.BorderSize=0;Cursor=Cursors.Hand;}
    protected override void OnPaint(PaintEventArgs e)
    {
        var background=Parent?.BackColor??Color.FromArgb(244,247,253);
        e.Graphics.Clear(background);
        var color=Selected?Color.FromArgb(12,105,221):Color.FromArgb(70,96,132);
        TextRenderer.DrawText(e.Graphics,Text,Font,ClientRectangle,color,TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter);
        if(Selected){using var pen=new Pen(color,2*DeviceDpi/96f);e.Graphics.DrawLine(pen,0,Height-2,Width,Height-2);}
        if(Focused && ShowFocusCues)ControlPaint.DrawFocusRectangle(e.Graphics,Rectangle.Inflate(ClientRectangle,-5,-5));
    }
}

public class PersonalEmptySymbol : Control
{
    [System.ComponentModel.DefaultValue(0)]
    public int Section {get;set;}
    public PersonalEmptySymbol(){DoubleBuffered=true;}
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(Parent?.BackColor??Color.White);e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
        using var fill=new SolidBrush(Color.FromArgb(234,242,254));e.Graphics.FillEllipse(fill,0,0,Width-1,Height-1);
        var state=e.Graphics.Save();var unit=Math.Min(Width,Height)/48f;e.Graphics.ScaleTransform(unit,unit);
        using var pen=new Pen(Color.FromArgb(16,108,218),1.6f){StartCap=LineCap.Round,EndCap=LineCap.Round,LineJoin=LineJoin.Round};
        if(Section==0)
        {
            e.Graphics.DrawRectangle(pen,16,22,17,7);e.Graphics.DrawLine(pen,16,19,16,32);e.Graphics.DrawLine(pen,33,25,33,32);
            e.Graphics.DrawRectangle(pen,18,19,5,3);
        }
        else if(Section==1)
        {
            e.Graphics.DrawRectangle(pen,18,15,13,19);
            for(var y=20;y<30;y+=4)e.Graphics.DrawLine(pen,21,y,28,y);
        }
        else
        {
            e.Graphics.DrawEllipse(pen,16,15,9,9);e.Graphics.DrawLine(pen,23,23,33,33);e.Graphics.DrawLine(pen,20,25,29,34);
        }
        e.Graphics.Restore(state);
    }
}
