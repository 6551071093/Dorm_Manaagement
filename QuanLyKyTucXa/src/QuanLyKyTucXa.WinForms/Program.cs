using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuanLyKyTucXa.Application.Interfaces;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;
using QuanLyKyTucXa.Infrastructure.Services;
using QuanLyKyTucXa.WinForms.Forms.Auth;
using QuanLyKyTucXa.WinForms.Forms.Main;
using QuanLyKyTucXa.Helpers;
using QuanLyKyTucXa.WinForms.Helpers;
using Serilog;
using WinFormsApplication = System.Windows.Forms.Application;

namespace QuanLyKyTucXa.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        WinFormsApplication.EnableVisualStyles();
        WinFormsApplication.SetCompatibleTextRenderingDefault(false);
        WinFormsApplication.SetHighDpiMode(HighDpiMode.SystemAware);

        // 1. Áp dụng Theme UI toàn cục
        UITheme.Initialize();

        // 2. Cấu hình Serilog quay vòng ghi log hàng ngày
        var logsDir = Path.Combine(AppContext.BaseDirectory, "Logs");
        Directory.CreateDirectory(logsDir);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                path: Path.Combine(logsDir, "ktx-.log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        // Bắt lỗi toàn cục
        WinFormsApplication.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        WinFormsApplication.ThreadException += (_, e) =>
        {
            Log.Error(e.Exception, "Lỗi UI Thread chưa xử lý");
            MessageBox.Show("Đã xảy ra lỗi giao diện: " + e.Exception.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
        };
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                Log.Fatal(ex, "Lỗi nghiêm trọng (AppDomain Unhandled)");
            }
        };

        try
        {
            Log.Information("Ứng dụng Quản lý Ký túc xá đang khởi động...");

            // 3. Đọc cấu hình
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("KtxDb");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Thiếu cấu hình chuỗi kết nối 'KtxDb' trong appsettings.json.");
            }

            // 4. Xây dựng DI Container
            var services = new ServiceCollection();
            ConfigureServices(services, configuration, connectionString);
            using var serviceProvider = services.BuildServiceProvider();

            // 5. Vòng lặp Đăng nhập -> FrmMain -> Đăng xuất
            while (true)
            {
                using var scope = serviceProvider.CreateScope();
                var login = scope.ServiceProvider.GetRequiredService<FrmLogin>();
                WinFormsApplication.Run(login);

                var session = login.Session;
                if (session is null)
                {
                    Log.Information("Người dùng đóng cửa sổ đăng nhập. Thoát ứng dụng.");
                    break;
                }

                Log.Information("Đăng nhập thành công: {Username} ({Role})", session.Username, session.Role);

                var mainFactory = scope.ServiceProvider.GetRequiredService<Func<AuthSession, FrmMain>>();
                using var main = mainFactory(session);
                WinFormsApplication.Run(main);

                session.SignOut();
                Log.Information("Kết thúc phiên làm việc của: {Username}", session.Username);

                if (!main.LogoutRequested)
                {
                    break;
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            Log.Error(ex, "Lỗi cấu hình khởi động");
            MessageBox.Show(ex.Message, "KTX Home - Lỗi cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Không thể khởi động ứng dụng");
            MessageBox.Show("Không khởi động được ứng dụng. Kiểm tra cấu hình kết nối database hoặc xem file log.", "KTX Home - Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.Information("Ứng dụng dừng hoạt động.");
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration, string connectionString)
    {
        services.AddSingleton(configuration);
        services.AddSingleton(connectionString);
        services.AddSingleton<Func<AppDbContext>>(() => AppDbContextFactory.Create(connectionString));

        // Mail OTP Sender
        services.AddSingleton<IOtpEmailSender>(_ => new SmtpOtpEmailSender(new SmtpOtpOptions(
            configuration["Email:Host"] ?? "smtp.gmail.com",
            int.TryParse(configuration["Email:Port"], out var port) ? port : 587,
            configuration["Email:Username"] ?? "",
            configuration["Email:Password"] ?? "",
            configuration["Email:FromAddress"] ?? "",
            configuration["Email:FromName"] ?? "KTX Home")));

        // Authentication & User Management Services
        services.AddScoped<AuthService>();
        services.AddScoped<IAuthService>(sp => sp.GetRequiredService<AuthService>());

        // WinForms Forms
        services.AddTransient<FrmLogin>();
        services.AddTransient<Func<AuthSession, FrmMain>>(sp => session =>
            new FrmMain(sp.GetRequiredService<string>(), session));
    }
}
