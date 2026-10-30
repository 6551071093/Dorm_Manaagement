using QuanLyKyTucXa.Infrastructure.Security;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;

namespace QuanLyKyTucXa.Infrastructure.Services;

public sealed record DangKyRow(int MaDangKy, string MaSv, string HoTen, string? MaPhong, int HocKy, string NamHoc, DateTime NgayDangKy, string TrangThai, string? GhiChu);
public sealed record RoomChoice(string MaPhong, string TenToa, int ChoTrong, decimal GiaPhong);
public sealed record BedChoice(string MaGiuong, string MaPhong, string TenGiuong, decimal GiaPhong);

public sealed class DangKyKtxService(Func<AppDbContext> createDb, AuthSession? session = null)
{
    public async Task<List<RoomChoice>> GetRoomsAsync(string? maSv = null)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.Register);
        if (session!.IsStudent && !string.IsNullOrWhiteSpace(maSv) && maSv != session.StudentId) throw new UnauthorizedAccessException("Chỉ được đăng ký cho chính mình.");
        if (session!.IsStudent) maSv = session.StudentId;
        var sex = string.IsNullOrWhiteSpace(maSv) ? null :
            await db.SinhViens.Where(s => s.MaSv == maSv).Select(s => s.GioiTinh).SingleOrDefaultAsync();
        return await db.Phongs.AsNoTracking()
            .Where(p => p.TrangThai != "Đang sửa chữa" && p.MaToaNavigation.TrangThai == "Hoạt động"
                && p.SoNguoiHienTai < p.SoChoToiDa
                && (sex == null || p.MaToaNavigation.LoaiToa == "Hỗn hợp" || p.MaToaNavigation.LoaiToa == sex)
                && p.Giuongs.Any(g => g.TrangThai == "Trống" && !db.HopDongs.Any(h => h.MaGiuong == g.MaGiuong && h.TrangThai == "Hiệu lực")))
            .OrderBy(p => p.MaPhong)
            .Select(p => new RoomChoice(p.MaPhong, p.MaToaNavigation.TenToa,
                p.Giuongs.Count(g => g.TrangThai == "Trống" && !db.HopDongs.Any(h => h.MaGiuong == g.MaGiuong && h.TrangThai == "Hiệu lực")),
                p.GiaPhong)).ToListAsync();
    }

    public async Task<List<SinhVien>> GetEligibleStudentsAsync()
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.Register);
        return await db.SinhViens.AsNoTracking()
            .Where(s => (!session!.IsStudent || s.MaSv == session.StudentId) && !db.HopDongs.Any(h => h.MaSv == s.MaSv && h.TrangThai == "Hiệu lực"))
            .OrderBy(s => s.MaSv).ToListAsync();
    }

    public async Task<int> SubmitAsync(string maSv, string? maPhong, int hocKy, string namHoc, string? ghiChu)
    {
        maSv = (maSv ?? "").Trim();
        maPhong = string.IsNullOrWhiteSpace(maPhong) ? null : maPhong.Trim();
        namHoc = (namHoc ?? "").Trim();
        if (hocKy is < 1 or > 3) throw new InvalidOperationException("Học kỳ phải là 1, 2 hoặc 3.");
        var match = Regex.Match(namHoc, @"^([0-9]{4})-([0-9]{4})$");
        if (!match.Success || int.Parse(match.Groups[2].Value) != int.Parse(match.Groups[1].Value) + 1)
            throw new InvalidOperationException("Năm học phải có dạng 2026-2027, gồm hai năm liên tiếp.");
        if (ghiChu?.Trim().Length > 255) throw new InvalidOperationException("Ghi chú tối đa 255 ký tự.");
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.Register);
        if (session!.IsStudent && maSv != session.StudentId) throw new UnauthorizedAccessException("Chỉ được đăng ký cho chính mình.");
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var student = await db.SinhViens.SingleOrDefaultAsync(s => s.MaSv == maSv)
            ?? throw new InvalidOperationException("Không tìm thấy sinh viên.");
        if (await db.HopDongs.AnyAsync(h => h.MaSv == maSv && h.TrangThai == "Hiệu lực"))
            throw new InvalidOperationException("Sinh viên đang có hợp đồng hiệu lực, không thể tạo đơn mới.");
        if (await db.DangKyKtxes.AnyAsync(x => x.MaSv == maSv && x.HocKy == hocKy && x.NamHoc == namHoc
            && (x.TrangThai == "Chờ duyệt" || x.TrangThai == "Đã duyệt")))
            throw new InvalidOperationException("Sinh viên đã có đơn chờ duyệt/đã duyệt cho học kỳ và năm học này.");
        if (maPhong is not null)
        {
            var room = await db.Phongs.Include(p => p.MaToaNavigation).SingleOrDefaultAsync(p => p.MaPhong == maPhong)
                ?? throw new InvalidOperationException("Phòng đề xuất không tồn tại.");
            CheckRoom(room, student.GioiTinh);
            if (!await db.Giuongs.AnyAsync(g => g.MaPhong == maPhong && g.TrangThai == "Trống"
                && !db.HopDongs.Any(h => h.MaGiuong == g.MaGiuong && h.TrangThai == "Hiệu lực")))
                throw new InvalidOperationException("Phòng không còn giường trống.");
        }
        var application = new DangKyKtx {
            MaSv = maSv, MaPhongDeXuat = maPhong, HocKy = hocKy, NamHoc = namHoc,
            NgayDangKy = DateTime.Now, TrangThai = "Chờ duyệt",
            GhiChu = string.IsNullOrWhiteSpace(ghiChu) ? null : ghiChu.Trim()
        };
        db.DangKyKtxes.Add(application);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return application.MaDangKy;
    }

    public async Task<List<DangKyRow>> SearchAsync(string? keyword, string? status)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ViewApplications);
        var q = db.DangKyKtxes.AsNoTracking().AsQueryable();
        if (session!.IsStudent) q = q.Where(x => x.MaSv == session.StudentId);
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var key = keyword.Trim();
            q = q.Where(x => x.MaSv.Contains(key) || x.MaSvNavigation.HoTen.Contains(key)
                || (x.MaPhongDeXuat != null && x.MaPhongDeXuat.Contains(key)));
        }
        if (!string.IsNullOrWhiteSpace(status) && status != "Tất cả trạng thái") q = q.Where(x => x.TrangThai == status);
        return await q.OrderByDescending(x => x.NgayDangKy).ThenByDescending(x => x.MaDangKy)
            .Select(x => new DangKyRow(x.MaDangKy, x.MaSv, x.MaSvNavigation.HoTen, x.MaPhongDeXuat,
                x.HocKy, x.NamHoc, x.NgayDangKy, x.TrangThai, x.GhiChu)).ToListAsync();
    }

    public async Task<List<BedChoice>> GetAvailableBedsAsync(int maDangKy)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ApproveApplications);
        var app = await db.DangKyKtxes.AsNoTracking().Include(x => x.MaSvNavigation)
            .SingleOrDefaultAsync(x => x.MaDangKy == maDangKy) ?? throw new InvalidOperationException("Không tìm thấy đơn.");
        if (app.TrangThai != "Chờ duyệt") throw new InvalidOperationException("Đơn đã được xử lý.");
        if (await db.HopDongs.AnyAsync(h => h.MaSv == app.MaSv && h.TrangThai == "Hiệu lực"))
            throw new InvalidOperationException("Sinh viên đã có hợp đồng hiệu lực.");
        var sex = app.MaSvNavigation.GioiTinh;
        var q = db.Giuongs.AsNoTracking().Where(g => g.TrangThai == "Trống"
            && g.MaPhongNavigation.TrangThai != "Đang sửa chữa"
            && g.MaPhongNavigation.MaToaNavigation.TrangThai == "Hoạt động"
            && g.MaPhongNavigation.SoNguoiHienTai < g.MaPhongNavigation.SoChoToiDa
            && (g.MaPhongNavigation.MaToaNavigation.LoaiToa == "Hỗn hợp" || g.MaPhongNavigation.MaToaNavigation.LoaiToa == sex)
            && !db.HopDongs.Any(h => h.MaGiuong == g.MaGiuong && h.TrangThai == "Hiệu lực"));
        if (!string.IsNullOrWhiteSpace(app.MaPhongDeXuat)) q = q.Where(g => g.MaPhong == app.MaPhongDeXuat);
        return await q.OrderBy(g => g.MaPhong).ThenBy(g => g.MaGiuong)
            .Select(g => new BedChoice(g.MaGiuong, g.MaPhong, g.TenGiuong, g.MaPhongNavigation.GiaPhong)).ToListAsync();
    }

    public async Task ApproveAsync(int maDangKy, string maGiuong, DateOnly start, DateOnly end, decimal deposit, int? reviewerId)
    {
        if (end <= start) throw new InvalidOperationException("Ngày kết thúc phải sau ngày bắt đầu.");
        if (start < new DateOnly(1900, 1, 1) || end < DateOnly.FromDateTime(DateTime.Today))
            throw new InvalidOperationException("Thời hạn hợp đồng không hợp lệ hoặc đã kết thúc.");
        if (deposit < 0 || deposit > 9999999999999999.99m) throw new InvalidOperationException("Tiền cọc không hợp lệ.");
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ApproveApplications);
        reviewerId = session!.UserId;
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var app = await db.DangKyKtxes.Include(x => x.MaSvNavigation).SingleOrDefaultAsync(x => x.MaDangKy == maDangKy)
            ?? throw new InvalidOperationException("Không tìm thấy đơn.");
        if (app.TrangThai != "Chờ duyệt") throw new InvalidOperationException("Đơn đã được xử lý trước đó.");
        var bed = await db.Giuongs.Include(g => g.MaPhongNavigation).ThenInclude(p => p.MaToaNavigation)
            .SingleOrDefaultAsync(g => g.MaGiuong == maGiuong && g.TrangThai == "Trống")
            ?? throw new InvalidOperationException("Giường không còn trống. Hãy tải lại danh sách.");
        CheckRoom(bed.MaPhongNavigation, app.MaSvNavigation.GioiTinh);
        if (!string.IsNullOrWhiteSpace(app.MaPhongDeXuat) && bed.MaPhong != app.MaPhongDeXuat)
            throw new InvalidOperationException("Giường được chọn không thuộc phòng sinh viên đề xuất.");
        if (await db.HopDongs.AnyAsync(x => x.MaSv == app.MaSv && x.TrangThai == "Hiệu lực"))
            throw new InvalidOperationException("Sinh viên đã có hợp đồng hiệu lực.");
        if (await db.HopDongs.AnyAsync(x => x.MaGiuong == maGiuong && x.TrangThai == "Hiệu lực"))
            throw new InvalidOperationException("Giường đã được cấp hợp đồng hiệu lực.");
        var liveOccupancy = await db.HopDongs.CountAsync(x => x.MaGiuongNavigation.MaPhong == bed.MaPhong && x.TrangThai == "Hiệu lực");
        if (liveOccupancy >= bed.MaPhongNavigation.SoChoToiDa) throw new InvalidOperationException("Phòng đã đủ sức chứa.");
        if (reviewerId is not null && !await db.NguoiDungs.AnyAsync(x => x.MaNguoiDung == reviewerId && x.TrangThai))
            throw new InvalidOperationException("Tài khoản xét duyệt không hợp lệ.");
        app.TrangThai = "Đã duyệt";
        app.NguoiDuyet = reviewerId;
        app.NgayDuyet = DateTime.Now;
        var contract = new HopDong {
            MaHopDong = $"HD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..16].ToUpperInvariant()}",
            MaSv = app.MaSv, MaGiuong = bed.MaGiuong, NgayBatDau = start, NgayKetThuc = end,
            TienCoc = deposit, TienPhongThang = bed.MaPhongNavigation.GiaPhong,
            TrangThai = "Hiệu lực", NgayTao = DateTime.Now, GhiChu = $"Tạo từ đơn đăng ký #{maDangKy}"
        };
        db.HopDongs.Add(contract);
        // The database trigger synchronizes student, bed and room occupancy in this transaction.
        await db.SaveChangesAsync();
        await RoomAssetHandoverStore.CreateEntryMinuteAsync(db, contract.MaHopDong, bed.MaPhong, bed.MaGiuong,
            session!.FullName, "Biên bản nhận phòng được tạo từ danh mục tài sản tại thời điểm ký hợp đồng.");
        await tx.CommitAsync();
    }

    public async Task RejectAsync(int maDangKy, string reason, int? reviewerId)
    {
        reason = (reason ?? "").Trim();
        if (reason.Length is < 1 or > 200) throw new InvalidOperationException("Lý do từ chối phải có từ 1 đến 200 ký tự.");
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ApproveApplications);
        reviewerId = session!.UserId;
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var app = await db.DangKyKtxes.SingleOrDefaultAsync(x => x.MaDangKy == maDangKy)
            ?? throw new InvalidOperationException("Không tìm thấy đơn.");
        if (app.TrangThai != "Chờ duyệt") throw new InvalidOperationException("Đơn đã được xử lý trước đó.");
        if (reviewerId is not null && !await db.NguoiDungs.AnyAsync(x => x.MaNguoiDung == reviewerId && x.TrangThai))
            throw new InvalidOperationException("Tài khoản xét duyệt không hợp lệ.");
        app.TrangThai = "Từ chối";
        app.NguoiDuyet = reviewerId;
        app.NgayDuyet = DateTime.Now;
        var note = $"Lý do từ chối: {reason}";
        var previous = app.GhiChu?.Trim();
        app.GhiChu = string.IsNullOrEmpty(previous) ? note
            : previous[..Math.Min(previous.Length, 255 - note.Length - 3)] + " | " + note;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    private static void CheckRoom(Phong room, string sex)
    {
        if (room.TrangThai == "Đang sửa chữa" || room.MaToaNavigation.TrangThai != "Hoạt động")
            throw new InvalidOperationException("Phòng hoặc tòa nhà đang ngừng sử dụng/bảo trì.");
        if (room.SoNguoiHienTai >= room.SoChoToiDa) throw new InvalidOperationException("Phòng đã đủ sức chứa.");
        if (room.MaToaNavigation.LoaiToa != "Hỗn hợp" && room.MaToaNavigation.LoaiToa != sex)
            throw new InvalidOperationException("Giới tính sinh viên không phù hợp với tòa nhà.");
    }
}
