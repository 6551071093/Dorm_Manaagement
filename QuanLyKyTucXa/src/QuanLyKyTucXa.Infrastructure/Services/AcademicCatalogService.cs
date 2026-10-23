using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;

namespace QuanLyKyTucXa.Infrastructure.Services;

public sealed record AcademicClass(string Code, string Faculty, string Program, bool IsExample);
public sealed record AcademicCatalog(List<string> Faculties, List<AcademicClass> Classes);

public sealed class AcademicCatalogService(Func<AppDbContext> createDb, AuthSession? session)
{
    public async Task<AcademicCatalog> LoadAsync()
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageResidents);
        await db.Database.OpenConnectionAsync();
        var faculties = new List<string>(); var classes = new List<AcademicClass>();
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT TenKhoa FROM dbo.DanhMucKhoa ORDER BY TenKhoa; SELECT l.MaLop, k.TenKhoa, n.TenNganh, l.LaLopMoPhong FROM dbo.DanhMucLop l JOIN dbo.DanhMucNganh n ON n.MaNganh=l.MaNganh JOIN dbo.DanhMucKhoa k ON k.MaKhoa=n.MaKhoa ORDER BY l.MaLop;";
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) faculties.Add(reader.GetString(0));
        await reader.NextResultAsync();
        while (await reader.ReadAsync()) classes.Add(new AcademicClass(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetBoolean(3)));
        return new AcademicCatalog(faculties, classes);
    }
}
