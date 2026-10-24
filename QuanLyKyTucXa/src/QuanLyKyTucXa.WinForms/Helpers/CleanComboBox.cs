using System.Drawing.Drawing2D;

namespace QuanLyKyTucXa.WinForms.Helpers;

// Giữ nguyên ComboBox gốc (binding, bàn phím, autocomplete và Enabled).
// Khung bo góc do LoginInputPanel bên ngoài đảm nhiệm.
public class CleanComboBox : ComboBox
{
    public CleanComboBox()
    {
        FlatStyle=FlatStyle.Flat;
        DrawMode=DrawMode.OwnerDrawFixed;
        BackColor=Color.White;
        ForeColor=Color.FromArgb(25,50,78);
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        ItemHeight=Math.Max(Font.Height+6,(int)Math.Round(24*DeviceDpi/96.0));
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        var selected=(e.State&DrawItemState.Selected)!=0 && (e.State&DrawItemState.ComboBoxEdit)==0;
        var back=selected?Color.FromArgb(231,240,254):Color.White;
        var fore=Enabled?ForeColor:Color.FromArgb(133,147,166);
        using var brush=new SolidBrush(back);
        e.Graphics.FillRectangle(brush,e.Bounds);
        var value=e.Index>=0 && e.Index<Items.Count?GetItemText(Items[e.Index]):Text;
        var bounds=Rectangle.Inflate(e.Bounds,-4,0);
        TextRenderer.DrawText(e.Graphics,value,Font,bounds,fore,back,
            TextFormatFlags.VerticalCenter|TextFormatFlags.Left|TextFormatFlags.EndEllipsis|TextFormatFlags.NoPrefix);
        base.OnDrawItem(e);
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if(m.Msg!=0x000F || !IsHandleCreated || IsDisposed || ClientSize.Width<4 || ClientSize.Height<4)return;
        using var graphics=Graphics.FromHwnd(Handle);
        var arrowWidth=(int)Math.Round(24*DeviceDpi/96.0);
        var arrow=new Rectangle(Math.Max(0,ClientSize.Width-arrowWidth),0,arrowWidth,ClientSize.Height);
        var fore=Enabled?ForeColor:Color.FromArgb(133,147,166);
        if(DropDownStyle==ComboBoxStyle.DropDownList || !Enabled)
        {
            graphics.Clear(Color.White);
            var value=SelectedItem is null?Text:GetItemText(SelectedItem);
            var text=new Rectangle(1,0,Math.Max(1,arrow.Left-3),ClientSize.Height);
            TextRenderer.DrawText(graphics,value,Font,text,fore,Color.White,
                TextFormatFlags.VerticalCenter|TextFormatFlags.Left|TextFormatFlags.EndEllipsis|TextFormatFlags.NoPrefix);
        }
        // Che viền vuông và nền nút xám do Windows vẽ; vùng nhập vẫn hoạt động như cũ.
        using var white=new SolidBrush(Color.White);
        graphics.FillRectangle(white,arrow);
        using(var edge=new Pen(Color.White,3))graphics.DrawRectangle(edge,0,0,ClientSize.Width-1,ClientSize.Height-1);
        graphics.SmoothingMode=SmoothingMode.AntiAlias;
        var x=arrow.Left+arrow.Width/2f;
        var y=ClientSize.Height/2f;
        var half=3.5f*DeviceDpi/96f;
        using var pen=new Pen(fore,1.2f*DeviceDpi/96f){StartCap=LineCap.Round,EndCap=LineCap.Round,LineJoin=LineJoin.Round};
        graphics.DrawLines(pen,[new PointF(x-half,y-half/2),new PointF(x,y+half/2),new PointF(x+half,y-half/2)]);
    }
}
