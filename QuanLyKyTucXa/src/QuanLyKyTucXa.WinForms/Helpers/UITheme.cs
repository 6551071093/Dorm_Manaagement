using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QuanLyKyTucXa.Helpers;   // đổi cho khớp namespace gốc của project

/// <summary>Nhóm trạng thái dùng để chọn cặp màu hiển thị.</summary>
public enum StatusKind { Success, Warning, Danger, Info, Neutral }

/// <summary>
/// Nguồn duy nhất cho màu, font, kích thước và style dùng chung của toàn bộ ứng dụng.
/// Các form KHÔNG tự gõ mã màu, luôn lấy từ UITheme.
///
/// Cách dùng:
///   1. Program.Main:      UITheme.Initialize();          // một lần, trước Application.Run
///   2. Constructor form:  InitializeComponent(); UITheme.Apply(this);
///   3. Nút trong Designer: đặt Tag = "primary" (Thêm, Lưu, Tìm) hoặc "danger" (Xóa); để trống = nút thường.
///      Nhãn tiêu đề trang: Tag = "title"; nhãn phụ: Tag = "muted".
///   4. Cột trạng thái:    dgv.CellFormatting += (s, e) => UITheme.ApplyStatusColor(dgv, e, "TrangThai");
/// </summary>
public static class UITheme
{
    private static Color Hex(string hex) => ColorTranslator.FromHtml(hex);

    // ===== MÀU CHÍNH =====
    public static readonly Color Primary = Hex("#1565C0");   // nút chính, StatusStrip
    public static readonly Color PrimaryHover = Hex("#1E88E5");
    public static readonly Color PrimaryDark = Hex("#0D47A1");
    public static readonly Color Danger = Hex("#C62828");   // chỉ dùng cho nút Xóa
    public static readonly Color DangerSoft = Hex("#FFEBEE");   // nền hover của nút Xóa

    // ===== MÀU KHUNG ỨNG DỤNG =====
    public static readonly Color MenuBar = Hex("#F5F7FA");   // MenuStrip
    public static readonly Color ToolBar = Hex("#EEF2F7");   // ToolStrip
    public static readonly Color MenuHover = Hex("#D6E6F8");   // mục menu hover/chọn
    public static readonly Color MenuBorder = Hex("#90B8E8");
    public static readonly Color Workspace = Hex("#DDE3EC");   // vùng MDI phía sau form con
    public static readonly Color Separator = Hex("#C5CEDB");

    // ===== MÀU FORM CON =====
    public static readonly Color FormBack = Hex("#F4F6FA");   // nền form con
    public static readonly Color Surface = Color.White;      // lưới, ô nhập, panel
    public static readonly Color Border = Hex("#CBD5E1");
    public static readonly Color BorderLight = Hex("#E0E5EC");
    public static readonly Color ButtonBorder = Hex("#A8B3C4");
    public static readonly Color Text = Hex("#1F2937");
    public static readonly Color TextMuted = Hex("#6B7280");

    // ===== MÀU LƯỚI =====
    public static readonly Color GridHeader = Hex("#E8EEF7");
    public static readonly Color GridAltRow = Hex("#F8FAFC");
    public static readonly Color GridSelect = Hex("#BBDEFB");

    // ===== FONT =====
    public static readonly Font Base = new("Segoe UI", 9F);
    public static readonly Font Bold = new("Segoe UI", 9F, FontStyle.Bold);
    public static readonly Font Title = new("Segoe UI", 14F, FontStyle.Bold);

    // ===== KÍCH THƯỚC / KHOẢNG CÁCH =====
    public const int Pad = 12;                      // lề trong form con
    public const int Gap = 8;                       // khoảng cách giữa các control/nút
    public const int RowHeight = 32;                // chiều cao dòng lưới
    public static readonly Size ButtonSize = new(90, 30);

    // =====================================================================
    //  KHỞI TẠO
    // =====================================================================

    /// <summary>Gọi một lần trong Program.Main: áp màu cho MenuStrip, ToolStrip, StatusStrip toàn ứng dụng.</summary>
    public static void Initialize()
    {
        ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new AppColorTable())
        {
            RoundedEdges = false
        };
    }

    // =====================================================================
    //  ÁP THEME CHO FORM / CONTROL
    // =====================================================================

    /// <summary>Áp theme cho một form hoặc UserControl và toàn bộ control con.</summary>
    public static void Apply(Control root)
    {
        if (root is Form form)
        {
            form.BackColor = FormBack;
            form.ForeColor = Text;
            form.Font = Base;
        }

        StyleControl(root);
        foreach (Control child in root.Controls)
            Apply(child);
    }

    private static void StyleControl(Control c)
    {
        // Thứ tự quan trọng: StatusStrip và MenuStrip đều kế thừa ToolStrip.
        switch (c)
        {
            case DataGridView g: StyleGrid(g); break;
            case Button b: StyleButton(b); break;
            case GroupBox gb: gb.ForeColor = Text; break;
            case Label l: StyleLabel(l); break;
            case TextBox tb: tb.BorderStyle = BorderStyle.FixedSingle; break;
            case StatusStrip ss: StyleStatusStrip(ss); break;
            case MenuStrip ms: ms.BackColor = MenuBar; ms.ForeColor = Text; break;
            case ToolStrip ts: ts.BackColor = ToolBar; ts.ForeColor = Text; ts.GripStyle = ToolStripGripStyle.Hidden; break;
            case MdiClient mdi: mdi.BackColor = Workspace; break;
        }
    }

    // ----- Nút -----

    public static void StyleButton(Button b)
    {
        switch ((b.Tag as string)?.ToLowerInvariant())
        {
            case "primary": StylePrimaryButton(b); break;
            case "danger": StyleDangerButton(b); break;
            default: StyleSecondaryButton(b); break;
        }
    }

    public static void StylePrimaryButton(Button b)
    {
        BaseButton(b);
        b.BackColor = Primary;
        b.ForeColor = Color.White;
        b.FlatAppearance.BorderColor = PrimaryDark;
        b.FlatAppearance.MouseOverBackColor = PrimaryHover;
        b.FlatAppearance.MouseDownBackColor = PrimaryDark;
    }

    public static void StyleSecondaryButton(Button b)
    {
        BaseButton(b);
        b.BackColor = MenuBar;
        b.ForeColor = Text;
        b.FlatAppearance.BorderColor = ButtonBorder;
        b.FlatAppearance.MouseOverBackColor = MenuHover;
        b.FlatAppearance.MouseDownBackColor = GridSelect;
    }

    public static void StyleDangerButton(Button b)
    {
        BaseButton(b);
        b.BackColor = Color.White;
        b.ForeColor = Danger;
        b.FlatAppearance.BorderColor = Danger;
        b.FlatAppearance.MouseOverBackColor = DangerSoft;
        b.FlatAppearance.MouseDownBackColor = DangerSoft;
    }

    private static void BaseButton(Button b)
    {
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize = 1;
        b.UseVisualStyleBackColor = false;
        b.Cursor = Cursors.Hand;
    }

    // ----- Nhãn -----

    private static void StyleLabel(Label l)
    {
        switch ((l.Tag as string)?.ToLowerInvariant())
        {
            case "title": l.Font = Title; l.ForeColor = Text; break;
            case "muted": l.ForeColor = TextMuted; break;
        }
    }

    // ----- DataGridView -----

    public static void StyleGrid(DataGridView g)
    {
        g.BackgroundColor = Surface;
        g.BorderStyle = BorderStyle.FixedSingle;
        g.GridColor = BorderLight;
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        g.RowHeadersVisible = false;
        g.EnableHeadersVisualStyles = false;

        g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        g.ColumnHeadersHeight = RowHeight;
        var header = g.ColumnHeadersDefaultCellStyle;
        header.BackColor = GridHeader;
        header.ForeColor = Text;
        header.Font = Bold;
        header.SelectionBackColor = GridHeader;
        header.SelectionForeColor = Text;
        header.Alignment = DataGridViewContentAlignment.MiddleLeft;
        header.Padding = new Padding(6, 0, 6, 0);

        var cell = g.DefaultCellStyle;
        cell.BackColor = Surface;
        cell.ForeColor = Text;
        cell.SelectionBackColor = GridSelect;
        cell.SelectionForeColor = Text;
        cell.Padding = new Padding(6, 0, 6, 0);
        g.AlternatingRowsDefaultCellStyle.BackColor = GridAltRow;
        g.RowTemplate.Height = RowHeight;

        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        g.MultiSelect = false;
        g.ReadOnly = true;
        g.AllowUserToAddRows = false;
        g.AllowUserToDeleteRows = false;
        g.AllowUserToResizeRows = false;
        g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    }

    // ----- StatusStrip -----

    public static void StyleStatusStrip(StatusStrip ss)
    {
        ss.BackColor = Primary;
        ss.ForeColor = Color.White;
        foreach (ToolStripItem item in ss.Items)
            item.ForeColor = Color.White;
    }

    // =====================================================================
    //  MÀU TRẠNG THÁI
    // =====================================================================

    // Tên trạng thái phải khớp với giá trị trong CHECK constraint của CSDL.
    // Tên nào không có trong danh sách sẽ hiển thị màu trung tính.
    private static readonly Dictionary<string, StatusKind> StatusMap = BuildStatusMap();

    private static Dictionary<string, StatusKind> BuildStatusMap()
    {
        var map = new Dictionary<string, StatusKind>(StringComparer.OrdinalIgnoreCase);
        void Add(StatusKind kind, params string[] names)
        {
            foreach (var n in names) map[Norm(n)] = kind;
        }

        Add(StatusKind.Success,
            "Còn chỗ", "Trống", "Đang ở", "Đã duyệt", "Hiệu lực",
            "Đã thanh toán", "Đã giải quyết", "Hoạt động");
        Add(StatusKind.Warning,
            "Chờ duyệt", "Chờ xếp phòng", "Chưa thanh toán",
            "Đang xử lý", "Bảo trì", "Đang sửa chữa");
        Add(StatusKind.Danger,
            "Đã đầy", "Đang sử dụng", "Hỏng", "Từ chối", "Quá hạn");
        Add(StatusKind.Info,
            "Mới tiếp nhận");
        Add(StatusKind.Neutral,
            "Đã hủy", "Hủy", "Đã thanh lý", "Hết hạn", "Đã rời KTX", "Dừng hoạt động");
        return map;
    }

    private static string Norm(string s) => s.Trim().Normalize(NormalizationForm.FormC);

    public static StatusKind GetStatusKind(string status)
    {
        if (string.IsNullOrWhiteSpace(status)) return StatusKind.Neutral;
        return StatusMap.TryGetValue(Norm(status), out var kind) ? kind : StatusKind.Neutral;
    }

    /// <summary>Cặp màu (nền, chữ) cho một nhóm trạng thái.</summary>
    public static (Color Back, Color Fore) StatusColors(StatusKind kind) => kind switch
    {
        StatusKind.Success => (Hex("#E8F5E9"), Hex("#1B5E20")),
        StatusKind.Warning => (Hex("#FFF3E0"), Hex("#E65100")),
        StatusKind.Danger => (Hex("#FFEBEE"), Hex("#B71C1C")),
        StatusKind.Info => (Hex("#E3F2FD"), Hex("#0D47A1")),
        _ => (Hex("#ECEFF1"), Hex("#455A64")),
    };

    /// <summary>Gọi trong sự kiện CellFormatting để tô màu ô trạng thái theo tên cột.</summary>
    public static void ApplyStatusColor(DataGridView grid, DataGridViewCellFormattingEventArgs e, string columnName)
    {
        if (e.RowIndex < 0 || e.Value == null) return;
        if (grid.Columns[e.ColumnIndex].Name != columnName) return;

        var (back, fore) = StatusColors(GetStatusKind(e.Value?.ToString() ?? string.Empty));
        e.CellStyle.BackColor = back;
        e.CellStyle.ForeColor = fore;
        e.CellStyle.SelectionBackColor = back;   // giữ màu trạng thái khi dòng đang chọn
        e.CellStyle.SelectionForeColor = fore;
        e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
    }

    // =====================================================================
    //  TIỆN ÍCH
    // =====================================================================

    /// <summary>Tên hiển thị của vai trò (giá trị VaiTro trong CSDL) cho StatusStrip, tiêu đề cửa sổ.</summary>
    public static string RoleDisplayName(string vaiTro) => vaiTro switch
    {
        "Admin" => "Quản trị viên",
        "NhanVien" => "Nhân viên",
        "SinhVien" => "Sinh viên",
        _ => vaiTro ?? string.Empty
    };
}

/// <summary>Bảng màu cho MenuStrip, ToolStrip, StatusStrip (được đăng ký trong UITheme.Initialize).</summary>
internal sealed class AppColorTable : ProfessionalColorTable
{
    public AppColorTable() => UseSystemColors = false;

    // MenuStrip
    public override Color MenuStripGradientBegin => UITheme.MenuBar;
    public override Color MenuStripGradientEnd => UITheme.MenuBar;
    public override Color MenuBorder => UITheme.MenuBorder;
    public override Color MenuItemBorder => UITheme.MenuBorder;
    public override Color MenuItemSelected => UITheme.MenuHover;
    public override Color MenuItemSelectedGradientBegin => UITheme.MenuHover;
    public override Color MenuItemSelectedGradientEnd => UITheme.MenuHover;
    public override Color MenuItemPressedGradientBegin => UITheme.MenuHover;
    public override Color MenuItemPressedGradientMiddle => UITheme.MenuHover;
    public override Color MenuItemPressedGradientEnd => UITheme.MenuHover;

    // Menu thả xuống
    public override Color ToolStripDropDownBackground => UITheme.Surface;
    public override Color ImageMarginGradientBegin => UITheme.Surface;
    public override Color ImageMarginGradientMiddle => UITheme.Surface;
    public override Color ImageMarginGradientEnd => UITheme.Surface;
    public override Color SeparatorDark => UITheme.Separator;
    public override Color SeparatorLight => UITheme.Surface;

    // ToolStrip
    public override Color ToolStripGradientBegin => UITheme.ToolBar;
    public override Color ToolStripGradientMiddle => UITheme.ToolBar;
    public override Color ToolStripGradientEnd => UITheme.ToolBar;
    public override Color ToolStripBorder => UITheme.BorderLight;
    public override Color OverflowButtonGradientBegin => UITheme.ToolBar;
    public override Color OverflowButtonGradientMiddle => UITheme.ToolBar;
    public override Color OverflowButtonGradientEnd => UITheme.ToolBar;
    public override Color GripDark => UITheme.Separator;
    public override Color GripLight => UITheme.Surface;

    // Nút trên ToolStrip
    public override Color ButtonSelectedHighlight => UITheme.MenuHover;
    public override Color ButtonSelectedGradientBegin => UITheme.MenuHover;
    public override Color ButtonSelectedGradientMiddle => UITheme.MenuHover;
    public override Color ButtonSelectedGradientEnd => UITheme.MenuHover;
    public override Color ButtonSelectedBorder => UITheme.MenuBorder;
    public override Color ButtonPressedHighlight => UITheme.GridSelect;
    public override Color ButtonPressedGradientBegin => UITheme.GridSelect;
    public override Color ButtonPressedGradientMiddle => UITheme.GridSelect;
    public override Color ButtonPressedGradientEnd => UITheme.GridSelect;
    public override Color ButtonPressedBorder => UITheme.MenuBorder;
    public override Color ButtonCheckedHighlight => UITheme.GridSelect;
    public override Color ButtonCheckedGradientBegin => UITheme.GridSelect;
    public override Color ButtonCheckedGradientMiddle => UITheme.GridSelect;
    public override Color ButtonCheckedGradientEnd => UITheme.GridSelect;

    // StatusStrip
    public override Color StatusStripGradientBegin => UITheme.Primary;
    public override Color StatusStripGradientEnd => UITheme.Primary;
}

