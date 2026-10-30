using System.Text.Json;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Services;

try
{
    if (args.Length != 4 || args[0] != "--project-root" || args[2] != "--output")
        throw new InvalidOperationException("Dùng: --project-root <thư mục project> --output <file.sql>");
    var root = Path.GetFullPath(args[1]);
    var configRoot = Path.Combine(root, "src", "QuanLyKyTucXa.WinForms");
    string? connection = null;
    foreach (var name in new[] { "appsettings.json", "appsettings.local.json" })
    {
        var file = Path.Combine(configRoot, name);
        if (!File.Exists(file)) continue;
        using var config = JsonDocument.Parse(File.ReadAllText(file), new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
        if (config.RootElement.TryGetProperty("ConnectionStrings", out var strings) && strings.TryGetProperty("KtxDb", out var value)) connection = value.GetString();
    }
    connection = Environment.GetEnvironmentVariable("ConnectionStrings__KtxDb") ?? connection;
    if (string.IsNullOrWhiteSpace(connection)) throw new InvalidOperationException("Thiếu cấu hình KtxDb.");
    var username = Environment.GetEnvironmentVariable("KTX_HANDOFF_USERNAME");
    var password = Environment.GetEnvironmentVariable("KTX_HANDOFF_PASSWORD");
    if (string.IsNullOrWhiteSpace(username)) { Console.Write("Tài khoản Admin: "); username = Console.ReadLine(); }
    if (password is null)
    {
        if (Console.IsInputRedirected) throw new InvalidOperationException("Cần phiên terminal để nhập mật khẩu.");
        Console.Write("Mật khẩu: ");
        password = "";
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter) break;
            if (key.Key == ConsoleKey.Backspace) { if (password.Length > 0) password = password[..^1]; }
            else if (!char.IsControl(key.KeyChar)) password += key.KeyChar;
        }
        Console.WriteLine();
    }
    var auth = new AuthService(() => AppDbContextFactory.Create(connection));
    var session = await auth.LoginAsync(username ?? "", password);
    try
    {
        var result = await new DataHandoffService(() => AppDbContextFactory.Create(connection), session).ExportAsync(args[3]);
        Console.WriteLine($"Đã xuất {result.Tables} bảng / {result.Rows} bản ghi: {Path.GetFullPath(args[3])}");
    }
    finally { session.SignOut(); }
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex is UnauthorizedAccessException or InvalidOperationException ? ex.Message : "Không xuất được dữ liệu. Kiểm tra database, quyền và đường dẫn file.");
    Environment.ExitCode = 1;
}
