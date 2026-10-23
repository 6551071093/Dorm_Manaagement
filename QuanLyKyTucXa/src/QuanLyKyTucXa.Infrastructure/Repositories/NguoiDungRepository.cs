using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;

namespace QuanLyKyTucXa.Infrastructure.Repositories;

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
    public async Task<bool> CompleteLoginAsync(NguoiDung snapshot, string? upgradedHash)
    {
        await using var db = createDb();
        var query = db.NguoiDungs.Where(x => x.MaNguoiDung == snapshot.MaNguoiDung && x.TrangThai
            && x.VaiTro == snapshot.VaiTro && x.MatKhauHash == snapshot.MatKhauHash);
        var now = DateTime.Now;
        var count = upgradedHash is null
            ? await query.ExecuteUpdateAsync(set => set.SetProperty(x => x.LanDangNhapCuoi, now))
            : await query.ExecuteUpdateAsync(set => set.SetProperty(x => x.LanDangNhapCuoi, now).SetProperty(x => x.MatKhauHash, upgradedHash));
        return count == 1;
    }
}