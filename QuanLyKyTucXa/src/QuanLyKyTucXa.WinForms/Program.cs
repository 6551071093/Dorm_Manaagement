using Microsoft.Extensions.Configuration;
using QuanLyKyTucXa.WinForms.Forms.Main;

namespace QuanLyKyTucXa.WinForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("KtxDb")
                ?? throw new InvalidOperationException("Thiếu cấu hình KtxDb.");
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmMain(connectionString));
        }
    }
}
