namespace QuanLyKyTucXa.WinForms.Helpers;

// Shared native WinForms editor: validation errors keep the dialog and entered values open.
internal class RecordEditorDialog : Form
{
    private readonly TableLayoutPanel fields;
    private readonly Button save;
    private readonly Button cancel;
    private readonly Label error;
    private bool saving;
    protected Func<Task>? SaveAction { get; set; }

    protected RecordEditorDialog(string title)
    {
        Text = title;
        Font = new Font("Segoe UI", 10F);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96, 96);
        ClientSize = new Size(560, 610);
        MinimumSize = new Size(460, 380);
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        BackColor = Color.FromArgb(246, 246, 250);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(18), ColumnCount = 1, RowCount = 3 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        fields = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Padding = new Padding(0, 0, 12, 0) };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        scroll.Controls.Add(fields);
        error = new Label { Dock = DockStyle.Fill, ForeColor = Color.Firebrick, AutoEllipsis = true, TextAlign = ContentAlignment.MiddleLeft };
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        cancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, Size = new Size(92, 36) };
        save = new Button { Text = "Lưu", BackColor = Color.FromArgb(23, 101, 183), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(92, 36) };
        save.Click += Save_Click;
        actions.Controls.Add(cancel);
        actions.Controls.Add(save);
        layout.Controls.Add(scroll, 0, 0);
        layout.Controls.Add(error, 0, 1);
        layout.Controls.Add(actions, 0, 2);
        Controls.Add(layout);
        AcceptButton = save;
        CancelButton = cancel;
        FormClosing += (_, e) => { if (saving) e.Cancel = true; };
    }

    protected T Field<T>(string label, T control) where T : Control
    {
        var row = fields.RowCount++;
        fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var caption = new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 8, 10) };
        control.Dock = DockStyle.Top;
        control.Margin = new Padding(0, 5, 0, 8);
        fields.Controls.Add(caption, 0, row);
        fields.Controls.Add(control, 1, row);
        return control;
    }
    protected TextBox TextField(string label, string? value, int max, bool readOnly = false) =>
        Field(label, new TextBox { Text = value ?? "", MaxLength = max, ReadOnly = readOnly });
    protected ComboBox Choice(string label, IEnumerable<string> values, string? selected)
    {
        var box = Field(label, new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList });
        box.Items.AddRange(values.Cast<object>().ToArray());
        if (selected is not null && box.Items.Contains(selected)) box.SelectedItem = selected;
        else if (box.Items.Count > 0) box.SelectedIndex = 0;
        return box;
    }
    protected NumericUpDown Number(string label, decimal value, decimal min, decimal max, int decimals = 0) =>
        Field(label, new NumericUpDown { Minimum = min, Maximum = max, Value = Math.Clamp(value, min, max), DecimalPlaces = decimals, ThousandsSeparator = true });

    private async void Save_Click(object? sender, EventArgs e)
    {
        if (saving || SaveAction is null) return;
        saving = true;
        save.Enabled = cancel.Enabled = fields.Enabled = false;
        error.Text = "";
        UseWaitCursor = true;
        var succeeded = false;
        try { await SaveAction(); succeeded = true; }
        catch (InvalidOperationException ex) { error.Text = ex.Message; }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException) { error.Text = "Không lưu được: dữ liệu trùng, đã thay đổi hoặc còn liên kết. Hãy kiểm tra và thử lại."; }
        catch (Exception) { error.Text = "Không kết nối/lưu được database. Kiểm tra kết nối rồi thử lại."; }
        finally
        {
            saving = false;
            save.Enabled = cancel.Enabled = fields.Enabled = true;
            UseWaitCursor = false;
        }
        if (succeeded) { DialogResult = DialogResult.OK; Close(); }
    }
}
