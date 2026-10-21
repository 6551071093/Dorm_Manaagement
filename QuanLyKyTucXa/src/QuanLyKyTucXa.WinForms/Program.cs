using Microsoft.Extensions.Configuration;
using QuanLyKyTucXa.Infrastructure.Data.Context;

namespace QuanLyKyTucXa.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("KtxDb");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                MessageBox.Show(
                    "Không tìm thấy ConnectionStrings:KtxDb trong appsettings.json.",
                    "Cấu hình database",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                using var db = AppDbContextFactory.Create(connectionString);
                if (!db.Database.CanConnect())
                {
                    MessageBox.Show(
                        "Không kết nối được QuanLyKTXDB. Kiểm tra LocalDB và bảo đảm đã chạy các script SQL tạo database.",
                        "Không kết nối database",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không kết nối được database QuanLyKTXDB.{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                    "Không kết nối database",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            Application.Run(new FrmMain(connectionString));
        }
    }
}

