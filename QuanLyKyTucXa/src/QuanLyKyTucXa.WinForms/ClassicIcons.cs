using System.Drawing.Drawing2D;
namespace QuanLyKyTucXa.WinForms;

internal static class ClassicIcons
{
    public static Bitmap Create(string kind)
    {
        var image = new Bitmap(24, 24);
        using var g = Graphics.FromImage(image);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.FromArgb(29, 112, 168), 1.8F)
        {
            StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round
        };
        switch (kind)
        {
            case "students":
                g.DrawEllipse(pen, 5, 2, 6, 6);
                g.DrawArc(pen, 15, 3, 5, 5, 270, 180);
                g.DrawArc(pen, 2, 11, 13, 14, 180, 180);
                g.DrawLine(pen, 2, 18, 2, 22);
                g.DrawLine(pen, 15, 18, 15, 22);
                g.DrawArc(pen, 13, 11, 8, 10, 270, 90);
                g.DrawLine(pen, 21, 16, 21, 21);
                break;
            case "rooms":
                g.DrawLine(pen, 2, 7, 2, 22); g.DrawLine(pen, 22, 11, 22, 22);
                g.DrawRectangle(pen, 3, 11, 18, 7); g.DrawRectangle(pen, 5, 7, 5, 4);
                g.DrawLine(pen, 12, 8, 22, 8); g.DrawLine(pen, 22, 8, 22, 11);
                break;
            case "refresh":
                g.DrawArc(pen, 3, 3, 18, 18, 30, 300);
                g.DrawLines(pen, new[] { new Point(15, 2), new Point(21, 6), new Point(15, 7) });
                break;
            case "export":
                g.DrawLine(pen, 12, 2, 12, 16);
                g.DrawLines(pen, new[] { new Point(7, 11), new Point(12, 16), new Point(17, 11) });
                g.DrawLines(pen, new[] { new Point(3, 17), new Point(3, 22), new Point(21, 22), new Point(21, 17) });
                break;
            case "tools":
                g.DrawArc(pen, 2, 2, 11, 11, 20, 270);
                g.DrawLines(pen, new[] { new Point(5, 3), new Point(8, 7), new Point(12, 5) });
                g.DrawLines(pen, new[] { new Point(12, 10), new Point(21, 18), new Point(18, 21), new Point(9, 12) });
                break;
            default:
                g.DrawLines(pen, new[] { new Point(5, 2), new Point(14, 2), new Point(20, 8), new Point(20, 22), new Point(5, 22), new Point(5, 2) });
                g.DrawLines(pen, new[] { new Point(14, 2), new Point(14, 8), new Point(20, 8) });
                g.DrawLine(pen, 8, 12, 16, 12); g.DrawLine(pen, 8, 16, 16, 16);
                break;
        }
        return image;
    }
}
