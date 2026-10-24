using System.Drawing.Drawing2D;
using QuanLyKyTucXa.WinForms.Forms.Auth;

namespace QuanLyKyTucXa.WinForms.Helpers;

internal static class ModernDialog
{
    internal sealed record Field(Label Caption, Control Input, bool FullWidth = false);

    public static void Install(Form form, TableLayoutPanel fields, Button save, Button cancel, Label? message = null, Label? notice = null)
    {
        WhenCreating(form, () =>
        {
            var entries = fields.Controls.Cast<Control>()
                .Where(c => fields.GetColumn(c) == 0 && c is Label)
                .OrderBy(c => fields.GetRow(c))
                .Select(c => new Field((Label)c, fields.GetControlFromPosition(1, fields.GetRow(c))!))
                .Where(f => f.Input is not null).ToArray();
            Build(form, fields, entries, save, cancel, message, notice);
        });
    }

    public static void InstallCustom(Form form, Control surface, Button save, Button cancel, Label? message, Label? notice, params Field[] entries) =>
        WhenCreating(form, () => Build(form, surface, entries, save, cancel, message, notice));

    public static void InstallAuth(Form form, Panel fields, Panel actions, Button save, Button cancel, Label message, Label notice, CheckBox showPassword, params Field[] entries) =>
        WhenCreating(form, () => Build(form, fields, entries, save, cancel, message, notice, actions, showPassword));

    // Dựng giao diện một lần trước khi Windows tạo handle cho các control con.
    private static void WhenCreating(Form form, Action build)
    {
        var built = false;
        form.ShowIcon = false;
        form.HandleCreated += (_, _) => {
            if (built) return;
            built = true;
            build();
        };
    }
    private static void Build(Form form, Control surface, Field[] entries, Button save, Button cancel,
        Label? message, Label? notice, Panel? actionSurface = null, CheckBox? extra = null)
    {
        int Px(int value) => (int)Math.Round(value * form.DeviceDpi / 96.0);
        var blue = Color.FromArgb(23,101,194);
        var oldRoots = form.Controls.Cast<Control>().ToArray();
        form.SuspendLayout();
        surface.SuspendLayout();
        foreach (var old in oldRoots) if (old != surface) old.SuspendLayout();
        // Giữ chính control cũ để bảo toàn dữ liệu, validation, sự kiện và trạng thái Enabled.
        foreach (var field in entries) { field.Caption.Parent = null; field.Input.Parent = null; }
        if (message is not null) message.Parent = null;
        if (notice is not null) notice.Parent = null;
        if (extra is not null) extra.Parent = null;
        save.Parent = cancel.Parent = null;
        surface.Parent = null;
        actionSurface?.Parent?.Controls.Remove(actionSurface);
        surface.Controls.Clear();
        form.Controls.Clear();

        form.BackColor = Color.White;
        form.ForeColor = Color.FromArgb(25,50,78);
        form.ShowIcon = false;
        // Chừa sẵn vùng thông báo ngoài phần cuộn, tránh thay đổi bố cục khi nhập lỗi.
        var messageSpace = message is null ? 0 : 48;
        var shell = new TableLayoutPanel { Name="modernShell", Tag=messageSpace, Dock=DockStyle.Fill, ColumnCount=1, RowCount=message is null?3:4, Margin=Padding.Empty, Padding=Padding.Empty };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute,Px(78)));
        shell.RowStyles.Add(new RowStyle(SizeType.Percent,100));
        if(message is not null) shell.RowStyles.Add(new RowStyle(SizeType.Absolute,Px(messageSpace)));
        shell.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var header = new Panel {Dock=DockStyle.Fill,BackColor=blue,Margin=Padding.Empty};
        var icon = new LoginGlyph {Kind=LoginGlyphKind.Home, ForeColor=blue, BackColor=Color.Transparent, Location=new Point(Px(24),Px(20)), Size=new Size(Px(38),Px(38))};
        var titleText = form.Text.Replace("KTX Home – ","").Replace("KTX Home — ","");
        if (titleText == "Thêm phòng") titleText = "Thêm phòng mới";
        var title = new Label {Text=titleText, ForeColor=Color.White, Font=new Font("Segoe UI",17F,FontStyle.Bold), AutoEllipsis=true,
            Location=new Point(Px(76),Px(15)), Size=new Size(Px(510),Px(30)), Anchor=AnchorStyles.Left|AnchorStyles.Top|AnchorStyles.Right};
        var subtitle = new Label {Text="Các mục có dấu * là bắt buộc",ForeColor=Color.White,Font=new Font("Segoe UI",9F),
            Location=new Point(Px(78),Px(47)),Size=new Size(Px(480),Px(20)), Anchor=AnchorStyles.Left|AnchorStyles.Top|AnchorStyles.Right};
        header.Controls.Add(icon);header.Controls.Add(title);header.Controls.Add(subtitle);
        header.SizeChanged+=(_,_)=>{title.Width=Math.Max(1,header.Width-Px(96));subtitle.Width=title.Width;};
        var scroll = new Panel {Dock=DockStyle.Fill,AutoScroll=true,BackColor=Color.White,Margin=Padding.Empty,Padding=new Padding(Px(24),Px(16),Px(24),Px(12))};
        TableLayoutPanel grid;
        if (surface is TableLayoutPanel existing) grid=existing;
        else { grid=new TableLayoutPanel {Dock=DockStyle.Top,AutoSize=true};surface.Controls.Add(grid); }
        grid.SuspendLayout();
        grid.ColumnStyles.Clear();grid.RowStyles.Clear();grid.ColumnCount=2;grid.RowCount=0;
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));
        grid.MaximumSize=Size.Empty;grid.AutoSize=true;grid.AutoSizeMode=AutoSizeMode.GrowAndShrink;grid.Padding=Padding.Empty;grid.Margin=Padding.Empty;grid.BackColor=Color.White;
        grid.Name="modernFields";
        var row=0;var col=0;
        foreach (var field in entries)
        {
            var wide=field.FullWidth || field.Input is TextBox {UseSystemPasswordChar:true} || field.Input is TextBox {Multiline:true} || field.Caption.Text.Contains("Ghi chú") || (col==0 && field==entries[^1]);
            if(wide && col==1){row++;col=0;}
            while(grid.RowStyles.Count<=row) grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var card=new Panel {Dock=DockStyle.Fill,Height=Px(field.Input is TextBox {Multiline:true}?104:66),Margin=new Padding(col==0?0:Px(7),0,wide?0:col==0?Px(7):0,Px(4))};
            field.Caption.Dock=DockStyle.Top;field.Caption.AutoSize=false;field.Caption.Height=Px(20);field.Caption.Margin=Padding.Empty;
            field.Caption.Font=new Font("Segoe UI",9F,FontStyle.Bold);field.Caption.TextAlign=ContentAlignment.MiddleLeft;
            var input=new LoginInputPanel {Dock=DockStyle.Fill,Padding=new Padding(Px(12),Px(6),Px(12),Px(6)),Margin=Padding.Empty};
            field.Input.Dock=DockStyle.None;field.Input.Anchor=AnchorStyles.Left|AnchorStyles.Top|AnchorStyles.Right;field.Input.Margin=Padding.Empty;
            field.Input.Font=new Font("Segoe UI",10F);
            field.Input.MinimumSize=field.Input.MaximumSize=Size.Empty;
            if(field.Input is TextBox text){text.BackColor=Color.White;text.BorderStyle=BorderStyle.None;if(text.PlaceholderText.Length==0 && !text.ReadOnly)text.PlaceholderText="Nhập "+field.Caption.Text.Replace("*","").Trim().ToLowerInvariant();}
            if(field.Input is NumericUpDown number) {number.BackColor=Color.White;number.BorderStyle=BorderStyle.None;}
            if(field.Input is ComboBox combo) combo.FlatStyle=FlatStyle.Flat;
            if(field.Input is CheckBox check){check.AutoSize=false;check.TextAlign=ContentAlignment.MiddleLeft;}
            input.Controls.Add(field.Input);
            void AlignInput(object? sender,EventArgs e)
            {
                var multiline=field.Input is TextBox {Multiline:true};
                var height=multiline?input.ClientSize.Height-Px(14):Math.Max(field.Input.PreferredSize.Height,Px(21));
                field.Input.SetBounds(Px(12),Math.Max(Px(5),(input.ClientSize.Height-height)/2),Math.Max(1,input.ClientSize.Width-Px(24)),Math.Max(1,height));
            }
            input.SizeChanged+=AlignInput;AlignInput(null,EventArgs.Empty);
            card.Tag=field.Input;
            card.Controls.Add(input);card.Controls.Add(field.Caption);
            grid.Controls.Add(card,col,row);if(wide)grid.SetColumnSpan(card,2);
            if(wide||col==1){row++;col=0;}else col=1;
        }
        if(col==1)row++;
        void AddFull(Control control,int height)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));control.Dock=DockStyle.Fill;control.Margin=new Padding(0,Px(5),0,Px(5));
            control.MinimumSize=new Size(0,Px(height));grid.Controls.Add(control,0,row++);grid.SetColumnSpan(control,2);
        }
        if(extra is not null)AddFull(extra,28);
        if(notice is not null){notice.AutoSize=true;notice.BackColor=Color.FromArgb(234,242,254);notice.ForeColor=Color.FromArgb(44,93,153);notice.Font=new Font("Segoe UI",9F);notice.Padding=new Padding(Px(12),Px(9),Px(12),Px(9));AddFull(notice,40);}
        if(message is not null)
        {
            message.AutoSize=false;message.AutoEllipsis=false;
            message.ForeColor=Color.Firebrick;message.BackColor=Color.White;
            message.TextAlign=ContentAlignment.MiddleLeft;
            message.Padding=new Padding(Px(24),Px(4),Px(24),Px(4));
            message.Margin=Padding.Empty;message.MinimumSize=Size.Empty;
            message.Dock=DockStyle.Fill;message.Visible=true;
        }
        grid.RowCount=row;
        if(surface is Panel panel)panel.AutoSizeMode=AutoSizeMode.GrowAndShrink;
        surface.MaximumSize=Size.Empty;surface.Dock=DockStyle.Top;surface.AutoSize=true;surface.Padding=Padding.Empty;surface.Margin=Padding.Empty;
        scroll.Controls.Add(surface);
        var actions=actionSurface??new Panel();actions.Controls.Clear();actions.Dock=DockStyle.Fill;actions.Margin=Padding.Empty;actions.BackColor=Color.FromArgb(244,247,252);
        actions.MinimumSize=new Size(0,Px(64));
        Skin(save,true);Skin(cancel,false);
        actions.Controls.Add(save);actions.Controls.Add(cancel);
        void AlignButtons(object? sender,EventArgs e)
        {
            var saveWidth=Math.Max(Px(112),TextRenderer.MeasureText(save.Text,save.Font).Width+Px(28));
            save.SetBounds(Math.Max(0,actions.Width-Px(24)-saveWidth),Px(14),saveWidth,Px(36));
            cancel.SetBounds(Math.Max(0,save.Left-Px(10)-Px(92)),Px(14),Px(92),Px(36));
        }
        actions.SizeChanged+=AlignButtons;
        save.TextChanged+=AlignButtons;cancel.TextChanged+=AlignButtons;
        shell.Controls.Add(header,0,0);shell.Controls.Add(scroll,0,1);
        if(message is not null)shell.Controls.Add(message,0,2);
        shell.Controls.Add(actions,0,message is null?2:3);form.Controls.Add(shell);
        grid.ResumeLayout(true); surface.ResumeLayout(true);form.ResumeLayout(true);
        AlignButtons(null,EventArgs.Empty);
        if(form.TopLevel)
        {
            form.MinimumSize=Size.Empty;
            form.ClientSize=new Size(Px(620),Px(78+64+32+messageSpace)+grid.GetPreferredSize(new Size(Px(572),0)).Height);
            var work=Screen.FromControl(form.Owner??form).WorkingArea;work.Inflate(-Px(12),-Px(12));
            form.Size=new Size(Math.Min(form.Width,work.Width),Math.Min(form.Height,work.Height));
            form.StartPosition=FormStartPosition.Manual;
            form.Location=new Point(work.Left+(work.Width-form.Width)/2,work.Top+(work.Height-form.Height)/2);
        }
        form.Disposed+=(_,_)=>{foreach(var old in oldRoots)if(!old.IsDisposed && old.Parent is null)old.Dispose();};
    }

    public static void SetFieldVisible(Control input,bool visible)
    {
        input.Visible=visible;
        if(input.Parent is LoginInputPanel host && host.Parent is Panel card && ReferenceEquals(card.Tag,input))card.Visible=visible;
    }

    public static void ResizeToContent(Form form)
    {
        if(!form.TopLevel || form.IsDisposed)return;
        TableLayoutPanel? FindGrid(Control parent)
        {
            foreach(Control child in parent.Controls)
            {
                if(child is TableLayoutPanel table && table.Name=="modernFields")return table;
                var nested=FindGrid(child);if(nested is not null)return nested;
            }
            return null;
        }
        var grid=FindGrid(form);if(grid is null)return;
        int Px(int value)=>(int)Math.Round(value*form.DeviceDpi/96.0);
        grid.PerformLayout();grid.Parent?.PerformLayout();form.PerformLayout();
        var shell=form.Controls.OfType<TableLayoutPanel>().FirstOrDefault(c=>c.Name=="modernShell");
        var messageSpace=shell?.Tag is int space?space:0;
        var center=new Point(form.Left+form.Width/2,form.Top+form.Height/2);
        form.ClientSize=new Size(form.ClientSize.Width,Px(78+64+32+messageSpace)+grid.GetPreferredSize(new Size(Math.Max(Px(200),form.ClientSize.Width-Px(48)),0)).Height);
        var work=Screen.FromControl(form.Owner??form).WorkingArea;work.Inflate(-Px(12),-Px(12));
        form.Size=new Size(Math.Min(form.Width,work.Width),Math.Min(form.Height,work.Height));
        form.Location=new Point(Math.Clamp(center.X-form.Width/2,work.Left,work.Right-form.Width),Math.Clamp(center.Y-form.Height/2,work.Top,work.Bottom-form.Height));
    }

    private static void Skin(Button button,bool primary)
    {
        var blue=Color.FromArgb(23,101,194);
        if(button is LoginActionButton login)login.Primary=primary;
        button.AutoSize=false;button.FlatStyle=FlatStyle.Flat;button.FlatAppearance.BorderSize=0;button.UseVisualStyleBackColor=false;
        button.BackColor=primary?blue:Color.White;button.ForeColor=primary?Color.White:Color.FromArgb(32,63,99);
        button.Font=new Font("Segoe UI",10F,primary?FontStyle.Bold:FontStyle.Regular);button.Cursor=Cursors.Hand;
        if(button is LoginActionButton)return;
        button.Paint+=(_,e)=>
        {
            e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;e.Graphics.Clear(button.Parent?.BackColor??Color.White);
            var rect=new RectangleF(1,1,button.Width-3,button.Height-3);
            using var path=LoginDrawing.Rounded(rect,7*button.DeviceDpi/96F);
            using var brush=new SolidBrush(button.Enabled?button.BackColor:Color.FromArgb(225,231,239));e.Graphics.FillPath(brush,path);
            using var pen=new Pen(primary?blue:Color.FromArgb(197,213,233));e.Graphics.DrawPath(pen,path);
            TextRenderer.DrawText(e.Graphics,button.Text,button.Font,button.ClientRectangle,button.Enabled?button.ForeColor:Color.Gray,TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis);
        };
    }
}
