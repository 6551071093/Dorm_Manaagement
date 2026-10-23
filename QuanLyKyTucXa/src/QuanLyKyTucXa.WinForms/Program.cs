using Microsoft.Extensions.Configuration;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Services;
using QuanLyKyTucXa.WinForms.Forms.Auth;
using QuanLyKyTucXa.WinForms.Forms.Main;


namespace QuanLyKyTucXa.WinForms;
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        try
        {
            var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json").AddJsonFile("appsettings.local.json", optional: true).AddEnvironmentVariables().Build();
            var fallback = configuration.GetConnectionString("KtxDb");
            if (string.IsNullOrWhiteSpace(fallback)) throw new InvalidOperationException("Thiếu cấu hình KtxDb.");
            while (true)
            {
                var connectionString = fallback;
                var auth = new AuthService(() => AppDbContextFactory.Create(connectionString));
                using var login = new FrmLogin(auth);
                Application.Run(login);

                var session = login.Session;
                if (session is null) break;
                using var main = new FrmMain(connectionString, session);
                Application.Run(main);
                session.SignOut();
                if (!main.LogoutRequested) break;
            }
        }
        catch (InvalidOperationException ex) { MessageBox.Show(ex.Message, "KTX Home", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        catch (Exception) { MessageBox.Show("Không khởi động được ứng dụng. Kiểm tra cấu hình kết nối database.", "KTX Home", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}