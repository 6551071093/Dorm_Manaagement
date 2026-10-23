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
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
            var connectionString = configuration.GetConnectionString("KtxDb");
            if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException("Thiếu cấu hình kết nối KtxDb.");
            var auth = new AuthService(() => AppDbContextFactory.Create(connectionString));
            while (true)
            {
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
        catch (Exception)
        {
            MessageBox.Show("Không khởi động được ứng dụng. Kiểm tra file appsettings.json và cấu hình KtxDb.", "KTX Home", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}