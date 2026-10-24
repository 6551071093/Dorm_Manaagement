using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;

namespace QuanLyKyTucXa.Infrastructure.Services;

public sealed record DataHandoffResult(int Tables, int Rows);

public sealed class DataHandoffService(Func<AppDbContext> createDb, AuthSession session)
{
    // Thứ tự chèn theo khóa ngoại. Xóa theo thứ tự ngược lại.
    private static readonly string[] Tables = ["NguoiDung", "ToaNha", "SinhVien", "Phong", "Giuong", "DangKyKtx", "HopDong", "BangGia", "ChiSoDienNuoc", "SuCo", "HoaDon", "ChiTietHoaDon", "ThanhToan", "DanhMucKhoa", "DanhMucNganh", "DanhMucLop"];
    private sealed record Column(string Name, bool Identity, bool PrimaryKey);
    private static string Quote(string name) => "[" + name.Replace("]", "]]") + "]";

    public async Task<DataHandoffResult> ExportAsync(string path)
    {
        if (!Path.GetExtension(path).Equals(".sql", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Bản bàn giao phải là file .sql.");
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageUsers);
        var passwords = new Dictionary<string, string> {
            ["Admin"] = await Task.Run(() => PasswordHash.Create("Admin@123")),
            ["NhanVien"] = await Task.Run(() => PasswordHash.Create("NhanVien@123")),
            ["SinhVien"] = await Task.Run(() => PasswordHash.Create("SinhVien@123"))
        };
        var fullPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        var temporary = fullPath + ".pending";
        var rows = 0;
        try
        {
            await using (var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable))
            await using (var writer = new StreamWriter(temporary, false, new UTF8Encoding(true)))
            {
                await writer.WriteLineAsync("-- BẢN DỮ LIỆU BÀN GIAO KTX HOME - KHÔNG PHẢI BACKUP DATABASE GỐC");
                await writer.WriteLineAsync("-- Được tạo: " + DateTimeOffset.UtcNow.ToString("O"));
                await writer.WriteLineAsync("-- Nhập sẽ THAY THẾ dữ liệu trên database đang chọn. Dùng database demo cục bộ.");
                await writer.WriteLineAsync("-- Mã SV, tên, lớp, phòng, đơn, hợp đồng và trạng thái giữ nguyên.");
                await writer.WriteLineAsync("-- CCCD/SĐT/email/địa chỉ/ngày sinh được thay bằng dữ liệu demo; mật khẩu và mã khôi phục gốc KHÔNG được xuất.");
                await writer.WriteLineAsync("-- Tài khoản demo: Admin@123 cho Admin; NhanVien@123 cho Nhân viên; SinhVien@123 cho Sinh viên.");
                await writer.WriteLineAsync("SET XACT_ABORT ON; SET NOCOUNT ON; DECLARE @SnapshotIdentity nvarchar(300) = NULL;");
                await writer.WriteLineAsync("BEGIN TRY BEGIN TRANSACTION;");
                foreach (var table in Tables)
                    await writer.WriteLineAsync($"IF OBJECT_ID(N'dbo.{table}', N'U') IS NULL THROW 51100, N'Thiếu bảng {table}. Hãy chạy cấu trúc database và script 05_UpdateSchema_Catalog.sql trước.', 1;");
                foreach (var table in Tables)
                    await writer.WriteLineAsync($"DISABLE TRIGGER ALL ON dbo.{Quote(table)};");
                await writer.WriteLineAsync("IF OBJECT_ID(N'dbo.TaiKhoanKhoiPhuc',N'U') IS NOT NULL EXEC(N'DELETE dbo.TaiKhoanKhoiPhuc');");
                await writer.WriteLineAsync("IF OBJECT_ID(N'dbo.TaiKhoanDangNhap',N'U') IS NOT NULL EXEC(N'DELETE dbo.TaiKhoanDangNhap');");
                await writer.WriteLineAsync("IF OBJECT_ID(N'dbo.TaiKhoanOtp',N'U') IS NOT NULL EXEC(N'DELETE dbo.TaiKhoanOtp');");
                foreach (var table in Tables.Reverse()) await writer.WriteLineAsync($"DELETE FROM dbo.{Quote(table)};");
                foreach (var table in Tables)
                {
                    var columns = await ReadColumnsAsync(db, table);
                    if (columns.Count == 0) throw new InvalidOperationException($"Chưa có bảng {table}. Chạy script cấu trúc/danh mục trước.");
                    var identity = columns.Any(c => c.Identity);
                    var tableName = "dbo." + Quote(table);
                    if (identity) await writer.WriteLineAsync($"SET @SnapshotIdentity = N'{tableName}'; SET IDENTITY_INSERT {tableName} ON;");
                    var columnList = string.Join(",", columns.Select(c => Quote(c.Name)));
                    await using var command = db.Database.GetDbConnection().CreateCommand();
                    command.Transaction = tx.GetDbTransaction(); command.CommandTimeout = 120;
                    var keys = columns.Where(c => c.PrimaryKey).Select(c => Quote(c.Name)).ToArray();
                    command.CommandText = $"SELECT {columnList} FROM {tableName}" + (keys.Length == 0 ? "" : " ORDER BY " + string.Join(",", keys));
                    var ordinal = 0;
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ordinal++; rows++;
                            var record = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                            for (var i = 0; i < columns.Count; i++) record[columns[i].Name] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            Sanitize(table, record, ordinal, passwords);
                            await writer.WriteLineAsync($"INSERT INTO {tableName} ({columnList}) VALUES ({string.Join(",", columns.Select(c => Literal(record[c.Name])))});");
                        }
                    }
                    if (identity) await writer.WriteLineAsync($"SET IDENTITY_INSERT {tableName} OFF; SET @SnapshotIdentity = NULL;");
                    await writer.WriteLineAsync($"-- {table}: {ordinal} bản ghi.");
                }
                foreach (var table in Tables)
                {
                    await writer.WriteLineAsync($"ALTER TABLE dbo.{Quote(table)} WITH CHECK CHECK CONSTRAINT ALL;");
                    await writer.WriteLineAsync($"ENABLE TRIGGER ALL ON dbo.{Quote(table)};");
                }
                await writer.WriteLineAsync("COMMIT TRANSACTION; END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK TRANSACTION; IF @SnapshotIdentity IS NOT NULL BEGIN DECLARE @Off nvarchar(max)=N'SET IDENTITY_INSERT '+@SnapshotIdentity+N' OFF'; EXEC(@Off); END; THROW; END CATCH;");
                await writer.WriteLineAsync($"-- Tổng cộng {Tables.Length} bảng / {rows} bản ghi.");
                await tx.CommitAsync();
            }
            File.Move(temporary, fullPath, overwrite: true);
            return new DataHandoffResult(Tables.Length, rows);
        }
        catch
        {
            if (File.Exists(temporary)) File.Delete(temporary);
            throw;
        }
    }

    private static async Task<List<Column>> ReadColumnsAsync(AppDbContext db, string table)
    {
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.Transaction = db.Database.CurrentTransaction!.GetDbTransaction();
        command.CommandText = "SELECT c.name,c.is_identity,CONVERT(bit,CASE WHEN EXISTS(SELECT 1 FROM sys.indexes i JOIN sys.index_columns ic ON ic.object_id=i.object_id AND ic.index_id=i.index_id WHERE i.object_id=c.object_id AND i.is_primary_key=1 AND ic.column_id=c.column_id) THEN 1 ELSE 0 END) FROM sys.columns c WHERE c.object_id=OBJECT_ID(@table) AND c.is_computed=0 AND c.system_type_id<>189 ORDER BY c.column_id";
        var parameter = command.CreateParameter(); parameter.ParameterName = "@table"; parameter.Value = "dbo." + table;
        command.Parameters.Add(parameter);
        var result = new List<Column>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) result.Add(new Column(reader.GetString(0), reader.GetBoolean(1), reader.GetBoolean(2)));
        return result;
    }

    private static void Sanitize(string table, Dictionary<string, object?> values, int index, Dictionary<string, string> passwords)
    {
        if (table == "NguoiDung")
        {
            values["MatKhauHash"] = passwords[Convert.ToString(values["VaiTro"])!];
            values["LanDangNhapCuoi"] = null;
            foreach (var field in new[] { "Email", "SoDienThoai" }) if (values.ContainsKey(field)) values[field] = null;
        }
        if (table == "SinhVien")
        {
            values["CCCD"] = (100000000000L + index).ToString(CultureInfo.InvariantCulture);
            values["SoDienThoai"] = "09" + index.ToString("D8", CultureInfo.InvariantCulture);
            values["Email"] = "demo-" + index + "@example.invalid";
            values["QueQuan"] = "Dữ liệu bàn giao";
            values["NgaySinh"] = new DateTime(2005, 1, 1);
        }
    }

    private static string Literal(object? value) => value switch
    {
        null or DBNull => "NULL",
        string s => "N'" + s.Replace("'", "''") + "'",
        bool b => b ? "1" : "0",
        DateTime d => "'" + d.ToString("yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture) + "'",
        DateTimeOffset d => "'" + d.ToString("O", CultureInfo.InvariantCulture) + "'",
        DateOnly d => "'" + d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "'",
        TimeSpan t => "'" + t.ToString("c", CultureInfo.InvariantCulture) + "'",
        Guid g => "'" + g + "'",
        byte[] bytes => "0x" + Convert.ToHexString(bytes),
        IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
        _ => throw new InvalidOperationException("Kiểu dữ liệu chưa hỗ trợ bàn giao: " + value.GetType().Name)
    };
}
