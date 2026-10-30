using System.Drawing.Drawing2D;

namespace QuanLyKyTucXa.WinForms.Forms.Auth;

internal static class LoginDrawing
{
    internal static GraphicsPath Rounded(RectangleF bounds, float radius)
    {
        var path = new GraphicsPath();
        var d = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
        path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
        path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
        path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
        path.CloseFigure(); return path;
    }
}

public class LoginInputPanel : Panel
{
    public LoginInputPanel()
    {
        DoubleBuffered = true; BackColor = Color.White;
        Padding = new Padding(44, 12, 14, 10); TabStop = false;
    }
    protected override void OnControlAdded(ControlEventArgs e)
    {
        base.OnControlAdded(e);
        if (e.Control is null) return;
        e.Control.Enter += ChildFocusChanged; e.Control.Leave += ChildFocusChanged;
    }
    private void ChildFocusChanged(object? sender, EventArgs e) => Invalidate();
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var scale = DeviceDpi / 96F;
        using var outline = LoginDrawing.Rounded(new RectangleF(3 * scale, 3 * scale, Width - 6 * scale - 1, Height - 6 * scale - 1), 9 * scale);
        if (ContainsFocus)
        {
            using var halo = new Pen(Color.FromArgb(220, 235, 255), 6 * scale);
            e.Graphics.DrawPath(halo, outline);
        }
        using var border = new Pen(ContainsFocus ? Color.FromArgb(23, 101, 210) : Color.FromArgb(199, 214, 234), (ContainsFocus ? 1.7F : 1.3F) * scale);
        e.Graphics.DrawPath(border, outline);
    }
}

public class LoginActionButton : Button
{
    private bool _hover;
    [System.ComponentModel.DefaultValue(false)]
    public bool Primary { get; set; }
    public LoginActionButton()
    {
        DoubleBuffered = true; FlatStyle = FlatStyle.Flat; FlatAppearance.BorderSize = 0;
        BackColor = Color.White; Cursor = Cursors.Hand; UseVisualStyleBackColor = false;
    }
    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
    protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(Parent?.BackColor ?? Color.White);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var scale = DeviceDpi / 96F;
        using var shape = LoginDrawing.Rounded(new RectangleF(1, 1, Width - 3, Height - 3), 9 * scale);
        var fill = !Enabled ? Color.FromArgb(226, 232, 241) : Primary
            ? (_hover ? Color.FromArgb(42, 122, 215) : Color.FromArgb(23, 101, 194))
            : (_hover ? Color.FromArgb(239, 246, 255) : Color.White);
        using var brush = new SolidBrush(fill); e.Graphics.FillPath(brush, shape);
        if (!Primary) { using var pen = new Pen(Color.FromArgb(199, 214, 234), 1.3F * scale); e.Graphics.DrawPath(pen, shape); }
        TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, !Enabled ? Color.Gray : Primary ? Color.White : Color.FromArgb(14, 34, 64), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        if (Focused && ShowFocusCues) ControlPaint.DrawFocusRectangle(e.Graphics, Rectangle.Inflate(ClientRectangle, -7, -7), Primary ? Color.White : ForeColor, fill);
    }
}

public enum LoginGlyphKind { Home, User, Lock, Key }
public class LoginGlyph : Control
{
    [System.ComponentModel.DefaultValue(LoginGlyphKind.Home)]
    public LoginGlyphKind Kind { get; set; }
    public LoginGlyph() { DoubleBuffered = true; SetStyle(ControlStyles.SupportsTransparentBackColor, true); BackColor = Color.Transparent; TabStop = false; }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e); var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
        if (Kind == LoginGlyphKind.Home)
        {
            using var shape = LoginDrawing.Rounded(new RectangleF(0, 0, Width - 1, Height - 1), Width * .25F);
            using var brush = new SolidBrush(Color.White); g.FillPath(brush, shape);
        }
        var saved = g.Save();
        var unit = Math.Min(Width, Height) / 24F;
        g.TranslateTransform((Width - 24 * unit) / 2, (Height - 24 * unit) / 2); g.ScaleTransform(unit, unit);
        using var pen = new Pen(ForeColor, Kind == LoginGlyphKind.Home ? .85F : 1.6F) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
        if (Kind == LoginGlyphKind.Home)
        {
            g.DrawLines(pen, new PointF[] { new(7,17), new(7,10), new(12,6.5F), new(17,10), new(17,17) });
            g.DrawLines(pen, new PointF[] { new(10,17), new(10,13), new(14,13), new(14,17) }); g.DrawLine(pen,10,10.5F,14,10.5F);
        }
        else if (Kind == LoginGlyphKind.User)
        {
            g.DrawEllipse(pen, 8, 3.5F, 8, 8); g.DrawArc(pen, 4.5F, 13, 15, 13, 180, 180);
        }
        else if (Kind == LoginGlyphKind.Key)
        {
            g.DrawEllipse(pen, 3, 3, 9, 9);
            g.DrawLine(pen, 10.5F, 10.5F, 21, 21);
            g.DrawLine(pen, 17, 17, 20, 14);
            g.DrawLine(pen, 19, 19, 22, 16);
        }        else
        {
            using var box = LoginDrawing.Rounded(new RectangleF(5, 10, 14, 11), 2);
            g.DrawPath(pen, box); g.DrawArc(pen, 8, 2, 8, 13, 180, 180);
            g.DrawLine(pen, 8, 8, 8, 10); g.DrawLine(pen, 16, 8, 16, 10);
        }
        g.Restore(saved);
    }
}
