using QuanLyKyTucXa.Infrastructure.Security;
using System.Data;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;

namespace QuanLyKyTucXa.Infrastructure.Services;

public sealed class PhongGiuongService(Func<AppDbContext> createDb, AuthSession? session = null)
{
    private static string Required(string text, string label, int max)
    {
        text = text.Trim();
        if (text.Length == 0 || text.Length > max)
            throw new InvalidOperationException($"{label} phải có từ 1 đến {max} ký tự.");
        return text;
    }
    public async Task<List<ToaNha>> GetTowersAsync()
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageRooms);
        return await db.ToaNhas.AsNoTracking().OrderBy(x => x.MaToa).ToListAsync();
    }
    public async Task<Phong> GetAsync(string id)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageRooms);
        return await db.Phongs.AsNoTracking().SingleOrDefaultAsync(x => x.MaPhong == id)
            ?? throw new InvalidOperationException("Phòng không còn tồn tại. Hãy làm mới danh sách.");
    }

    public async Task SaveAsync(Phong value, bool isNew)
    {
        value.MaPhong = Required(value.MaPhong, "Mã phòng", 20);
        value.TenPhong = Required(value.TenPhong, "Tên phòng", 50);
        value.LoaiPhong = Required(value.LoaiPhong, "Loại phòng", 50);
        if (value.SoChoToiDa is < 1 or > 1000) throw new InvalidOperationException("Sức chứa phải từ 1 đến 1000.");
        if (value.GiaPhong < 0 || value.GiaPhong > 9999999999999999.99m) throw new InvalidOperationException("Giá phòng không hợp lệ.");
        if (value.GhiChu?.Length > 255) throw new InvalidOperationException("Ghi chú tối đa 255 ký tự.");
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageRooms);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var tower = await db.ToaNhas.SingleOrDefaultAsync(x => x.MaToa == value.MaToa)
            ?? throw new InvalidOperationException("Vui lòng chọn tòa nhà hợp lệ.");
        if (value.Tang < 1 || value.Tang > tower.SoTang) throw new InvalidOperationException($"Tầng phải từ 1 đến {tower.SoTang} của tòa {tower.MaToa}.");
        var entity = await db.Phongs.Include(x => x.Giuongs).SingleOrDefaultAsync(x => x.MaPhong == value.MaPhong);
        if (isNew && entity is not null) throw new InvalidOperationException("Mã phòng đã tồn tại.");
        if (!isNew && entity is null) throw new InvalidOperationException("Phòng không còn tồn tại.");
        if (isNew)
        {
            entity = new Phong { MaPhong = value.MaPhong, MaToa = value.MaToa };
            db.Phongs.Add(entity);
        }
        var oldTower = entity!.MaToa;
        if (!isNew && (oldTower != value.MaToa || entity.Tang != value.Tang)
            && (entity.SoNguoiHienTai > 0 || await db.HopDongs.AnyAsync(x => x.MaGiuongNavigation.MaPhong == value.MaPhong)))
            throw new InvalidOperationException("Phòng có người ở hoặc lịch sử hợp đồng không thể chuyển tòa/tầng.");
        var beds = entity.Giuongs.ToList();
        var occupied = beds.Count(x => x.TrangThai == "Đang sử dụng");
        if (value.SoChoToiDa < Math.Max(entity.SoNguoiHienTai, occupied))
            throw new InvalidOperationException("Sức chứa không được nhỏ hơn số người đang ở.");
        if (beds.Count > value.SoChoToiDa)
        {
            var historicalBeds = await db.HopDongs.Where(x => x.MaGiuongNavigation.MaPhong == value.MaPhong)
                .Select(x => x.MaGiuong).ToListAsync();
            var removable = beds.Where(x => x.TrangThai == "Trống" && !historicalBeds.Contains(x.MaGiuong))
                .OrderByDescending(x => x.MaGiuong).Take(beds.Count - value.SoChoToiDa).ToList();
            if (removable.Count != beds.Count - value.SoChoToiDa)
                throw new InvalidOperationException("Không thể giảm sức chứa: có giường đang dùng, hỏng/bảo trì hoặc đã có lịch sử hợp đồng.");
            db.Giuongs.RemoveRange(removable);
            beds.RemoveAll(x => removable.Contains(x));
        }
        var usedIds = (await db.Giuongs.Where(x => x.MaGiuong.StartsWith(value.MaPhong + "-G"))
            .Select(x => x.MaGiuong).ToListAsync()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        for (var index = 1; beds.Count < value.SoChoToiDa; index++)
        {
            var id = $"{value.MaPhong}-G{index}";
            if (!usedIds.Add(id)) continue;
            var bed = new Giuong { MaGiuong = id, MaPhong = value.MaPhong, TenGiuong = $"Giường {index}", TrangThai = "Trống" };
            entity.Giuongs.Add(bed);
            beds.Add(bed);
        }
        entity.MaToa = value.MaToa;
        entity.TenPhong = value.TenPhong;
        entity.Tang = value.Tang;
        entity.LoaiPhong = value.LoaiPhong;
        entity.SoChoToiDa = value.SoChoToiDa;
        entity.GiaPhong = value.GiaPhong;
        entity.GhiChu = value.GhiChu?.Trim();
        entity.TrangThai = value.TrangThai == "Đang sửa chữa" ? "Đang sửa chữa"
            : entity.SoNguoiHienTai >= entity.SoChoToiDa ? "Đã đầy" : "Còn chỗ";
        await db.SaveChangesAsync();
        foreach (var id in new[] { oldTower, value.MaToa }.Distinct())
        {
            var t = await db.ToaNhas.SingleAsync(x => x.MaToa == id);
            t.SoPhong = await db.Phongs.CountAsync(x => x.MaToa == id);
        }
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task DeleteAsync(string id)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageRooms);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var entity = await db.Phongs.Include(x => x.Giuongs).SingleOrDefaultAsync(x => x.MaPhong == id)
            ?? throw new InvalidOperationException("Phòng không còn tồn tại.");
        if (entity.SoNguoiHienTai > 0 || entity.Giuongs.Any(x => x.TrangThai != "Trống")
            || await db.HopDongs.AnyAsync(x => x.MaGiuongNavigation.MaPhong == id)
            || await db.DangKyKtxes.AnyAsync(x => x.MaPhongDeXuat == id)
            || await db.ChiSoDienNuocs.AnyAsync(x => x.MaPhong == id)
            || await db.HoaDons.AnyAsync(x => x.MaPhong == id) || await db.SuCos.AnyAsync(x => x.MaPhong == id))
            throw new InvalidOperationException("Không thể xóa phòng có người ở, giường chưa trống hoặc đã có lịch sử đăng ký, hợp đồng, điện nước, hóa đơn, sự cố.");
        db.Giuongs.RemoveRange(entity.Giuongs);
        db.Phongs.Remove(entity);
        await db.SaveChangesAsync();
        var tower = await db.ToaNhas.SingleAsync(x => x.MaToa == entity.MaToa);
        tower.SoPhong = await db.Phongs.CountAsync(x => x.MaToa == entity.MaToa);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}

