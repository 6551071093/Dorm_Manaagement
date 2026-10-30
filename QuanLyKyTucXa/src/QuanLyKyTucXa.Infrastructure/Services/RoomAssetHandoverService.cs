using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;

namespace QuanLyKyTucXa.Infrastructure.Services;

public sealed record RoomAssetRow(int AssetId, string RoomId, string Name, int Quantity, string Condition, string? Note);
public sealed record HandoverAssetLine(string Name, int QuantityGiven, int QuantityNow, string ConditionGiven, string ConditionNow, decimal Compensation, string? Note);
public sealed record HandoverChecklist(string ContractId, string RoomId, string BedId, bool HasSavedBaseline, DateTime? BaselineDate, IReadOnlyList<HandoverAssetLine> Items);
public sealed record HandoverHistoryRow(int MinuteId, string Type, DateTime Date, string RoomId, string BedId, string Recorder, string? MinuteNote,
    string? AssetName, int? QuantityGiven, int? QuantityNow, string? ConditionGiven, string? ConditionNow, decimal? Compensation, string? ItemNote);

public sealed class RoomAssetHandoverService(Func<AppDbContext> createDb, AuthSession? session)
{
    public async Task<List<RoomAssetRow>> GetRoomAssetsAsync(string roomId)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageRooms);
        return await RoomAssetHandoverStore.GetRoomAssetsAsync(db, roomId);
    }

    public async Task SaveRoomAssetAsync(RoomAssetRow value, bool isNew)
    {
        var roomId = value.RoomId.Trim();
        var name = value.Name.Trim();
        var condition = value.Condition.Trim();
        if (roomId.Length is < 1 or > 20 || name.Length is < 1 or > 100)
            throw new InvalidOperationException("Mã phòng và tên tài sản không được để trống; tên tối đa 100 ký tự.");
        if (value.Quantity is < 1 or > 10000) throw new InvalidOperationException("Số lượng phải từ 1 đến 10.000.");
        if (condition is not ("Tốt" or "Hư hỏng" or "Bảo trì")) throw new InvalidOperationException("Tình trạng tài sản không hợp lệ.");
        if (value.Note?.Length > 255) throw new InvalidOperationException("Ghi chú tối đa 255 ký tự.");

        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ConfigureRooms);
        await RoomAssetHandoverStore.EnsureSchemaAsync(db);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var roomExists = await db.Phongs.AnyAsync(x => x.MaPhong == roomId);
        if (!roomExists) throw new InvalidOperationException("Phòng không còn tồn tại.");
        await using var command = await RoomAssetHandoverStore.CreateCommandAsync(db,
            isNew
                ? "INSERT dbo.TaiSanPhong(MaPhong,TenTaiSan,SoLuong,TinhTrang,GhiChu) VALUES(@room,@name,@qty,@condition,@note);"
                : "UPDATE dbo.TaiSanPhong SET TenTaiSan=@name,SoLuong=@qty,TinhTrang=@condition,GhiChu=@note WHERE MaTaiSan=@id AND MaPhong=@room;");
        RoomAssetHandoverStore.AddParameter(command, "@room", roomId);
        RoomAssetHandoverStore.AddParameter(command, "@name", name);
        RoomAssetHandoverStore.AddParameter(command, "@qty", value.Quantity);
        RoomAssetHandoverStore.AddParameter(command, "@condition", condition);
        RoomAssetHandoverStore.AddParameter(command, "@note", value.Note?.Trim());
        if (!isNew) RoomAssetHandoverStore.AddParameter(command, "@id", value.AssetId);
        var affected = await command.ExecuteNonQueryAsync();
        if (!isNew && affected == 0) throw new InvalidOperationException("Tài sản đã bị xóa hoặc không thuộc phòng này. Hãy tải lại danh sách.");
        await tx.CommitAsync();
    }

    public async Task DeleteRoomAssetAsync(int assetId, string roomId)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.DeleteRooms);
        await RoomAssetHandoverStore.EnsureSchemaAsync(db);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await using var command = await RoomAssetHandoverStore.CreateCommandAsync(db,
            "DELETE dbo.TaiSanPhong WHERE MaTaiSan=@id AND MaPhong=@room;");
        RoomAssetHandoverStore.AddParameter(command, "@id", assetId);
        RoomAssetHandoverStore.AddParameter(command, "@room", roomId);
        if (await command.ExecuteNonQueryAsync() == 0) throw new InvalidOperationException("Tài sản đã bị xóa. Hãy tải lại danh sách.");
        await tx.CommitAsync();
    }

    public async Task<HandoverChecklist> GetChecklistAsync(string contractId)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageRooms);
        var contract = await db.HopDongs.AsNoTracking()
            .Where(x => x.MaHopDong == contractId)
            .Select(x => new { x.MaHopDong, x.MaGiuong, RoomId = x.MaGiuongNavigation.MaPhong })
            .SingleOrDefaultAsync() ?? throw new InvalidOperationException("Không tìm thấy hợp đồng.");
        return await RoomAssetHandoverStore.GetChecklistAsync(db, contract.MaHopDong, contract.RoomId, contract.MaGiuong);
    }

    public async Task<List<HandoverHistoryRow>> GetHistoryAsync(string contractId)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageRooms);
        if (!await db.HopDongs.AnyAsync(x => x.MaHopDong == contractId)) throw new InvalidOperationException("Không tìm thấy hợp đồng.");
        return await RoomAssetHandoverStore.GetHistoryAsync(db, contractId);
    }
}

internal static class RoomAssetHandoverStore
{
    internal static async Task<List<RoomAssetRow>> GetRoomAssetsAsync(AppDbContext db, string roomId)
    {
        await EnsureSchemaAsync(db);
        await using var command = await CreateCommandAsync(db,
            "SELECT MaTaiSan,MaPhong,TenTaiSan,SoLuong,TinhTrang,GhiChu FROM dbo.TaiSanPhong WHERE MaPhong=@room ORDER BY TenTaiSan;");
        AddParameter(command, "@room", roomId);
        await using var reader = await command.ExecuteReaderAsync();
        var rows = new List<RoomAssetRow>();
        while (await reader.ReadAsync())
            rows.Add(new RoomAssetRow(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetString(4), reader.IsDBNull(5) ? null : reader.GetString(5)));
        return rows;
    }

    internal static async Task<HandoverChecklist> GetChecklistAsync(AppDbContext db, string contractId, string roomId, string bedId)
    {
        await EnsureSchemaAsync(db);
        await using var command = await CreateCommandAsync(db, """
            SELECT TOP (1) MaBienBan,NgayLap
            FROM dbo.BienBanBanGiao
            WHERE MaHopDong=@contract AND MaPhong=@room AND LoaiBienBan IN (N'Nhận phòng',N'Chuyển phòng')
            ORDER BY NgayLap DESC,MaBienBan DESC;
            """);
        AddParameter(command, "@contract", contractId);
        AddParameter(command, "@room", roomId);
        int? minuteId = null;
        DateTime? date = null;
        await using (var reader = await command.ExecuteReaderAsync())
            if (await reader.ReadAsync()) { minuteId = reader.GetInt32(0); date = reader.GetDateTime(1); }

        if (minuteId is null)
        {
            var current = await GetRoomAssetsAsync(db, roomId);
            return new HandoverChecklist(contractId, roomId, bedId, false, null,
                current.Select(x => new HandoverAssetLine(x.Name, x.Quantity, x.Quantity, x.Condition, x.Condition, 0, x.Note)).ToList());
        }

        await using var details = await CreateCommandAsync(db, """
            SELECT TenTaiSan,SoLuongBanGiao,SoLuongBanGiao,TinhTrangBanGiao,TinhTrangBanGiao,TienBoiThuong,GhiChu
            FROM dbo.ChiTietBienBanBanGiao WHERE MaBienBan=@minute ORDER BY MaChiTiet;
            """);
        AddParameter(details, "@minute", minuteId.Value);
        await using var detailReader = await details.ExecuteReaderAsync();
        var items = new List<HandoverAssetLine>();
        while (await detailReader.ReadAsync())
            items.Add(new HandoverAssetLine(detailReader.GetString(0), detailReader.GetInt32(1), detailReader.GetInt32(2),
                detailReader.GetString(3), detailReader.GetString(4), detailReader.GetDecimal(5), detailReader.IsDBNull(6) ? null : detailReader.GetString(6)));
        return new HandoverChecklist(contractId, roomId, bedId, true, date, items);
    }

    internal static async Task<List<HandoverHistoryRow>> GetHistoryAsync(AppDbContext db, string contractId)
    {
        await EnsureSchemaAsync(db);
        await using var command = await CreateCommandAsync(db, """
            SELECT b.MaBienBan,b.LoaiBienBan,b.NgayLap,b.MaPhong,b.MaGiuong,b.NguoiLap,b.GhiChu,
                   d.TenTaiSan,d.SoLuongBanGiao,d.SoLuongThucTe,d.TinhTrangBanGiao,d.TinhTrangThucTe,d.TienBoiThuong,d.GhiChu
            FROM dbo.BienBanBanGiao b
            LEFT JOIN dbo.ChiTietBienBanBanGiao d ON d.MaBienBan=b.MaBienBan
            WHERE b.MaHopDong=@contract
            ORDER BY b.NgayLap,b.MaBienBan,d.MaChiTiet;
            """);
        AddParameter(command, "@contract", contractId);
        await using var reader = await command.ExecuteReaderAsync();
        var rows = new List<HandoverHistoryRow>();
        while (await reader.ReadAsync())
            rows.Add(new HandoverHistoryRow(reader.GetInt32(0),reader.GetString(1),reader.GetDateTime(2),reader.GetString(3),reader.GetString(4),
                reader.GetString(5),reader.IsDBNull(6)?null:reader.GetString(6),reader.IsDBNull(7)?null:reader.GetString(7),
                reader.IsDBNull(8)?null:reader.GetInt32(8),reader.IsDBNull(9)?null:reader.GetInt32(9),reader.IsDBNull(10)?null:reader.GetString(10),
                reader.IsDBNull(11)?null:reader.GetString(11),reader.IsDBNull(12)?null:reader.GetDecimal(12),reader.IsDBNull(13)?null:reader.GetString(13)));
        return rows;
    }

    internal static async Task CreateEntryMinuteAsync(AppDbContext db, string contractId, string roomId, string bedId, string recorder, string? note)
    {
        await EnsureSchemaAsync(db);
        var minuteId = await InsertMinuteAsync(db, contractId, roomId, bedId, "Nhận phòng", recorder, note);
        await using var command = await CreateCommandAsync(db, """
            INSERT dbo.ChiTietBienBanBanGiao(MaBienBan,TenTaiSan,SoLuongBanGiao,SoLuongThucTe,TinhTrangBanGiao,TinhTrangThucTe,TienBoiThuong,GhiChu)
            SELECT @minute,TenTaiSan,SoLuong,SoLuong,TinhTrang,TinhTrang,0,GhiChu
            FROM dbo.TaiSanPhong WHERE MaPhong=@room;
            """);
        AddParameter(command, "@minute", minuteId);
        AddParameter(command, "@room", roomId);
        await command.ExecuteNonQueryAsync();
    }

    internal static async Task SaveMinuteAsync(AppDbContext db, string contractId, string roomId, string bedId,
        string type, string recorder, string? note, IReadOnlyCollection<HandoverAssetLine> items)
    {
        await EnsureSchemaAsync(db);
        var minuteId = await InsertMinuteAsync(db, contractId, roomId, bedId, type, recorder, note);
        foreach (var item in items)
        {
            await using var command = await CreateCommandAsync(db, """
                INSERT dbo.ChiTietBienBanBanGiao(MaBienBan,TenTaiSan,SoLuongBanGiao,SoLuongThucTe,TinhTrangBanGiao,TinhTrangThucTe,TienBoiThuong,GhiChu)
                VALUES(@minute,@name,@given,@actual,@givenCondition,@actualCondition,@compensation,@note);
                """);
            AddParameter(command, "@minute", minuteId);
            AddParameter(command, "@name", item.Name);
            AddParameter(command, "@given", item.QuantityGiven);
            AddParameter(command, "@actual", item.QuantityNow);
            AddParameter(command, "@givenCondition", item.ConditionGiven);
            AddParameter(command, "@actualCondition", item.ConditionNow);
            AddParameter(command, "@compensation", item.Compensation);
            AddParameter(command, "@note", item.Note);
            await command.ExecuteNonQueryAsync();
        }
    }

    private static async Task<int> InsertMinuteAsync(AppDbContext db, string contractId, string roomId, string bedId,
        string type, string recorder, string? note)
    {
        await using var command = await CreateCommandAsync(db, """
            INSERT dbo.BienBanBanGiao(MaHopDong,MaPhong,MaGiuong,LoaiBienBan,NguoiLap,GhiChu)
            VALUES(@contract,@room,@bed,@type,@recorder,@note);
            SELECT CONVERT(int,SCOPE_IDENTITY());
            """);
        AddParameter(command, "@contract", contractId);
        AddParameter(command, "@room", roomId);
        AddParameter(command, "@bed", bedId);
        AddParameter(command, "@type", type);
        AddParameter(command, "@recorder", recorder);
        AddParameter(command, "@note", note);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    internal static async Task EnsureSchemaAsync(AppDbContext db)
    {
        await using var command = await CreateCommandAsync(db, """
            SELECT CASE WHEN OBJECT_ID(N'dbo.TaiSanPhong',N'U') IS NOT NULL
                          AND OBJECT_ID(N'dbo.BienBanBanGiao',N'U') IS NOT NULL
                          AND OBJECT_ID(N'dbo.ChiTietBienBanBanGiao',N'U') IS NOT NULL THEN 1 ELSE 0 END;
            """);
        var exists = Convert.ToInt32(await command.ExecuteScalarAsync()) == 1;
        if (!exists) throw new InvalidOperationException("Database chưa có bảng tài sản/biên bản. Chọn đúng QuanLyKTXDB trong SSMS và chạy database/scripts/07_UpdateSchema_RoomAssetsHandover.sql.");
    }

    internal static async Task<DbCommand> CreateCommandAsync(AppDbContext db, string sql)
    {
        await db.Database.OpenConnectionAsync();
        var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;
        if (db.Database.CurrentTransaction is { } transaction) command.Transaction = transaction.GetDbTransaction();
        return command;
    }

    internal static void AddParameter(DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}
