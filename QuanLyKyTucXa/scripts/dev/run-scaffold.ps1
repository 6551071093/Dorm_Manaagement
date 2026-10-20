# ============================================================================
# SCRIPT TỰ ĐỘNG CHẠY SCAFFOLD EF CORE DATABASE FIRST
# DỰ ÁN: QuanLyKyTucXa (.NET 9/10 & C# 12/13)
# ============================================================================

$connectionString = "Server=(localdb)\MSSQLLocalDB;Database=QuanLyKTXDB;Trusted_Connection=True;TrustServerCertificate=True;"

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host "BAT DAU CHAY SCAFFOLD-DBCONTEXT CHO QUANLYKTXDB..." -ForegroundColor Yellow
Write-Host "Connection String: $connectionString" -ForegroundColor Gray
Write-Host "=================================================================" -ForegroundColor Cyan

dotnet ef dbcontext scaffold $connectionString Microsoft.EntityFrameworkCore.SqlServer `
    --project src/QuanLyKyTucXa.Infrastructure `
    --startup-project src/QuanLyKyTucXa.WinForms `
    --output-dir ../QuanLyKyTucXa.Domain/Entities `
    --context-dir Data/Context `
    --context AppDbContext `
    --force

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n>>> SCAFFOLD THANH CONG! CAC ENTITY VA APPDBCONTEXT DA DUOC CAP NHAT. <<<" -ForegroundColor Green
} else {
    Write-Host "`n>>> CO LOI XAY RA KHI SCAFFOLD. VUI LONG KIEM TRA LAI CSDL! <<<" -ForegroundColor Red
}
