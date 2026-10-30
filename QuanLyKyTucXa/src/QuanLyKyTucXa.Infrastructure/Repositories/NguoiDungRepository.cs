using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QuanLyKyTucXa.Infrastructure.Data.Context;

namespace QuanLyKyTucXa.Infrastructure.Repositories;

public sealed record LoginState(int FailedAttempts, bool Locked);

public sealed class NguoiDungRepository(Func<AppDbContext> createDb)
{
    public async Task<NguoiDung?> FindByUsernameAsync(string username)
    {
        await using var db = createDb();
        return await db.NguoiDungs.AsNoTracking().SingleOrDefaultAsync(x => x.TenDangNhap == username);
    }
    public async Task<List<string>> GetStudentLinksAsync(int userId)
    {
        await using var db = createDb();
        return await db.SinhViens.AsNoTracking().Where(x => x.MaNguoiDung == userId).Select(x => x.MaSv).ToListAsync();
    }

    private static async Task<LoginState> LockStateAsync(AppDbContext db, int userId)
    {
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.Transaction = db.Database.CurrentTransaction!.GetDbTransaction();
        command.CommandText = "SELECT SoLanSai, BiKhoa FROM dbo.TaiKhoanDangNhap WITH (UPDLOCK, HOLDLOCK) WHERE MaNguoiDung = @id";
        var parameter = command.CreateParameter(); parameter.ParameterName = "@id"; parameter.Value = userId;
        command.Parameters.Add(parameter);
        await using (var reader = await command.ExecuteReaderAsync())
            if (await reader.ReadAsync()) return new LoginState(reader.GetInt32(0), reader.GetBoolean(1));
        await db.Database.ExecuteSqlInterpolatedAsync($"INSERT dbo.TaiKhoanDangNhap (MaNguoiDung, SoLanSai, BiKhoa) VALUES ({userId}, 0, 0)");
        return new LoginState(0, false);
    }

    public async Task<LoginState> RecordFailureAsync(NguoiDung snapshot)
    {
        await using var db = createDb();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var state = await LockStateAsync(db, snapshot.MaNguoiDung);
        // Không ghi nhận lỗi lên một tài khoản đã được đổi mật khẩu/khóa trong lúc xác minh.
        var current = await db.NguoiDungs.AsNoTracking().SingleOrDefaultAsync(x => x.MaNguoiDung == snapshot.MaNguoiDung);
        if (current is null || !current.TrangThai || current.MatKhauHash != snapshot.MatKhauHash || state.Locked)
            return state;
        var count = Math.Min(5, state.FailedAttempts + 1);
        var locked = count >= 5;
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.TaiKhoanDangNhap SET SoLanSai = {count}, BiKhoa = {locked}, LanSaiCuoi = SYSUTCDATETIME() WHERE MaNguoiDung = {snapshot.MaNguoiDung}");
        if (locked)
            await db.NguoiDungs.Where(x => x.MaNguoiDung == snapshot.MaNguoiDung).ExecuteUpdateAsync(set => set.SetProperty(x => x.TrangThai, false));
        await tx.CommitAsync();
        return new LoginState(count, locked);
    }

    public async Task<bool> CompleteLoginAsync(NguoiDung snapshot, string? upgradedHash)
    {
        await using var db = createDb();
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var state = await LockStateAsync(db, snapshot.MaNguoiDung);
        if (state.Locked || state.FailedAttempts >= 5) return false;
        var query = db.NguoiDungs.Where(x => x.MaNguoiDung == snapshot.MaNguoiDung && x.TrangThai
            && x.VaiTro == snapshot.VaiTro && x.MatKhauHash == snapshot.MatKhauHash);
        var now = DateTime.Now;
        var count = upgradedHash is null
            ? await query.ExecuteUpdateAsync(set => set.SetProperty(x => x.LanDangNhapCuoi, now))
            : await query.ExecuteUpdateAsync(set => set.SetProperty(x => x.LanDangNhapCuoi, now).SetProperty(x => x.MatKhauHash, upgradedHash));
        if (count != 1) return false;
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.TaiKhoanDangNhap SET SoLanSai = 0, BiKhoa = 0, LanSaiCuoi = NULL WHERE MaNguoiDung = {snapshot.MaNguoiDung}");
        await tx.CommitAsync();
        return true;
    }
}
