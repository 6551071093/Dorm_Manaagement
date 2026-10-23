namespace QuanLyKyTucXa.WinForms.Helpers;

internal static class DialogLayout
{
    // Footer nằm ngoài vùng cuộn; không dùng chiều cao tuyệt đối cho hàng nút.
    public static void Configure(Form form, TableLayoutPanel root, Control actions, TableLayoutPanel? fields = null)
    {
        var row = root.GetRow(actions);
        if (row >= 0 && row < root.RowStyles.Count)
            root.RowStyles[row].SizeType = SizeType.AutoSize;
        actions.AutoSize = true;
        actions.Margin = Padding.Empty;
        if (actions is FlowLayoutPanel flow)
        {
            flow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flow.WrapContents = true;
        }
        if (fields?.Parent == root)
        {
            var position = root.GetPositionFromControl(fields);
            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Margin = fields.Margin, TabIndex = fields.TabIndex };
            root.Controls.Remove(fields);
            fields.Margin = Padding.Empty;
            fields.Dock = DockStyle.Top;
            fields.AutoSize = true;
            scroll.Controls.Add(fields);
            root.Controls.Add(scroll, position.Column, position.Row);
        }
        void ReserveSpace()
        {
            int Px(int n) => (int)Math.Ceiling(n * form.DeviceDpi / 96.0);
            if (actions is Button single)
            {
                single.MinimumSize = new Size(Px(128), Px(38));
                return;
            }
            actions.Padding = new Padding(0, Px(6), 0, Px(6));
            foreach (var button in actions.Controls.OfType<Button>())
            {
                button.Margin = new Padding(Px(8), 0, 0, 0);
                button.MinimumSize = new Size(Px(84), Px(36));
                button.Height = Math.Max(Px(36), button.PreferredSize.Height);
            }
        }
        form.Load += (_, _) => { ReserveSpace(); FitToScreen(form); };
        form.DpiChanged += (_, _) => { ReserveSpace(); FitToScreen(form); };
    }

    public static void ConfigureAuth(Form form, Panel root, Panel fields, Panel actions, Label message)
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Margin = Padding.Empty };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Margin = Padding.Empty };
        root.Controls.Remove(fields);
        fields.Controls.Remove(actions);
        fields.Controls.Remove(message);
        fields.Location = Point.Empty;
        fields.Height = fields.Controls.Cast<Control>().Max(c => c.Bottom) + 16;
        scroll.Controls.Add(fields);
        actions.Dock = DockStyle.Fill;
        actions.Margin = Padding.Empty;
        message.Dock = DockStyle.Fill;
        message.AutoSize = true;
        message.MinimumSize = new Size(0, 28);
        message.Margin = new Padding(0, 8, 0, 4);
        layout.Controls.Add(scroll, 0, 0);
        layout.Controls.Add(message, 0, 1);
        layout.Controls.Add(actions, 0, 2);
        root.Controls.Add(layout);
        var buttons = actions.Controls.OfType<Button>().OrderBy(b => b.Left).ToArray();
        void Arrange()
        {
            int Px(int n) => (int)Math.Ceiling(n * form.DeviceDpi / 96.0);
            root.Padding = new Padding(Px(40), Px(12), Px(40), Px(16));
            var height = Px(48);
            actions.MinimumSize = new Size(0, height + Px(8));
            if (buttons.Length == 2)
            {
                var secondary = Math.Min(Px(138), actions.ClientSize.Width / 3);
                buttons[0].SetBounds(0, Px(4), Math.Max(1, actions.ClientSize.Width - secondary - Px(12)), height);
                buttons[1].SetBounds(Math.Max(0, actions.ClientSize.Width - secondary), Px(4), secondary, height);
            }
        }
        actions.SizeChanged += (_, _) => Arrange();
        form.Load += (_, _) => { Arrange(); FitToScreen(form); };
        form.DpiChanged += (_, _) => { Arrange(); FitToScreen(form); };
    }

    public static void ConfigureFixed(Form form, params Button[] buttons)
    {
        var content = form.Controls.Cast<Control>().Except(buttons).ToArray();
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(12) };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Margin = Padding.Empty };
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft, WrapContents = true };
        foreach (var control in content)
        {
            var bounds = control.Bounds;
            control.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            scroll.Controls.Add(control);
            control.Bounds = bounds;
        }
        foreach (var button in buttons.OrderByDescending(b => b.Left)) actions.Controls.Add(button);
        root.Controls.Add(scroll, 0, 0);
        root.Controls.Add(actions, 0, 1);
        form.Controls.Add(root);
        Configure(form, root, actions);
    }

    public static void ConfigureCompactEditor(Form form, TableLayoutPanel root, Panel scroll, TableLayoutPanel fields, FlowLayoutPanel actions, Label error, int fieldRows)
    {
        Configure(form, root, actions);
        var errorRow = root.GetRow(error);
        root.RowStyles[errorRow].SizeType = SizeType.AutoSize;
        error.AutoEllipsis = false;
        error.AutoSize = true;
        error.Dock = DockStyle.Top;
        var arranging = false;
        void Arrange()
        {
            if (arranging || form.IsDisposed) return;
            arranging = true;
            try
            {
                int Px(int n) => (int)Math.Ceiling(n * form.DeviceDpi / 96.0);
                root.Padding = new Padding(Px(20), Px(16), Px(20), Px(12));
                form.ClientSize = new Size(Px(620), form.ClientSize.Height);
                fields.ColumnStyles[0].Width = Px(150);
                fields.Padding = new Padding(0, 0, Px(8), 0);
                fields.Margin = scroll.Margin = Padding.Empty;
                for (var row = 0; row < fields.RowCount; row++)
                {
                    if (row < fields.RowStyles.Count)
                    {
                        fields.RowStyles[row].SizeType = row < fieldRows ? SizeType.Absolute : SizeType.AutoSize;
                        if (fields.RowStyles[row].SizeType == SizeType.Absolute) fields.RowStyles[row].Height = Px(40);
                    }
                }
                foreach (Control control in fields.Controls)
                {
                    if (control is Label && fields.GetColumn(control) == 0)
                    {
                        control.Anchor = AnchorStyles.Left;
                        control.Margin = new Padding(0, 0, Px(8), 0);
                    }
                    else control.Margin = new Padding(0, Px(5), 0, Px(5));
                }
                error.Visible = !string.IsNullOrWhiteSpace(error.Text);
                error.Margin = new Padding(0, Px(6), 0, Px(4));
                root.PerformLayout();
                var width = Math.Max(Px(200), root.ClientSize.Width - root.Padding.Horizontal);
                error.MaximumSize = new Size(width, 0);
                var contentHeight = fields.GetPreferredSize(new Size(width, 0)).Height;
                var footerHeight = actions.GetPreferredSize(new Size(width, 0)).Height;
                var errorHeight = error.Visible ? error.GetPreferredSize(new Size(width, 0)).Height + error.Margin.Vertical : 0;
                form.ClientSize = new Size(form.ClientSize.Width, root.Padding.Vertical + contentHeight + footerHeight + errorHeight + Px(4));
                FitToScreen(form);
            }
            finally { arranging = false; }
        }
        form.Load += (_, _) => Arrange();
        form.DpiChanged += (_, _) => Arrange();
        error.TextChanged += (_, _) => { if (form.IsHandleCreated) Arrange(); };
    }


    private static void FitToScreen(Form form)
    {
        var area = Screen.FromControl(form.Owner ?? form).WorkingArea;
        var inset = Math.Max(8, (int)Math.Ceiling(12 * form.DeviceDpi / 96.0));
        area.Inflate(-inset, -inset);
        // MinimumSize cũ có thể lớn hơn màn hình nhỏ hoặc màn hình đang dùng DPI cao.
        form.MinimumSize = new Size(Math.Min(form.MinimumSize.Width, area.Width), Math.Min(form.MinimumSize.Height, area.Height));
        form.Size = new Size(Math.Min(form.Width, area.Width), Math.Min(form.Height, area.Height));
        var x = form.Owner is null ? area.Left + (area.Width - form.Width) / 2 : form.Owner.Left + (form.Owner.Width - form.Width) / 2;
        var y = form.Owner is null ? area.Top + (area.Height - form.Height) / 2 : form.Owner.Top + (form.Owner.Height - form.Height) / 2;
        form.StartPosition = FormStartPosition.Manual;
        form.Location = new Point(Math.Clamp(x, area.Left, area.Right - form.Width), Math.Clamp(y, area.Top, area.Bottom - form.Height));
    }
}
