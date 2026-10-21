using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuanLyKyTucXa.WinForms;

internal sealed class GradientPanel : Panel
{
    private int _occupancyPercent;

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public int OccupancyPercent
    {
        get => _occupancyPercent;
        set { _occupancyPercent = Math.Clamp(value, 0, 100); Invalidate(); }
    }

    public GradientPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        BackColor = Color.FromArgb(14, 34, 64);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;
        using var gradient = new LinearGradientBrush(ClientRectangle,
            Color.FromArgb(14, 34, 64), Color.FromArgb(31, 111, 235), 0F);
        e.Graphics.FillRectangle(gradient, ClientRectangle);
        using var glow = new SolidBrush(Color.FromArgb(28, Color.White));
        e.Graphics.FillEllipse(glow, ClientSize.Width - 250, -100, 310, 310);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;
        PaintOccupancyRing(e.Graphics);
    }

    private void PaintOccupancyRing(Graphics graphics)
    {
        const int preferredSize = 116;
        var size = Math.Min(preferredSize, Math.Min(ClientSize.Height - 12, ClientSize.Width - 24));
        if (size < 64) return;
        var scale = size / (float)preferredSize;
        var left = ClientSize.Width - size - Math.Max(12, (int)Math.Round(22 * scale));
        var top = (ClientSize.Height - size) / 2;
        var inset = Math.Max(6, (int)Math.Round(10 * scale));
        var bounds = new Rectangle(left + inset, top + inset, size - inset * 2, size - inset * 2);
        var penWidth = Math.Max(4F, 8F * scale);

        var oldSmoothing = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var track = new Pen(Color.FromArgb(145, 130, 230, 220), penWidth);
        using var progress = new Pen(Color.FromArgb(230, 93, 231, 196), penWidth)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };
        graphics.DrawArc(track, bounds, -90, 359);
        if (_occupancyPercent > 0)
            graphics.DrawArc(progress, bounds, -90, 360F * _occupancyPercent / 100F);

        using var large = new Font("Segoe UI", 17F * scale, FontStyle.Bold);
        using var small = new Font("Segoe UI", 7F * scale, FontStyle.Regular);
        using var white = new SolidBrush(Color.White);
        using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        graphics.DrawString($"{_occupancyPercent}%", large, white,
            new Rectangle(left, top + (int)Math.Round(38 * scale), size, (int)Math.Round(25 * scale)), format);
        graphics.DrawString("đã lấp đầy", small, white,
            new Rectangle(left, top + (int)Math.Round(63 * scale), size, (int)Math.Round(15 * scale)), format);
        graphics.SmoothingMode = oldSmoothing;
    }
}