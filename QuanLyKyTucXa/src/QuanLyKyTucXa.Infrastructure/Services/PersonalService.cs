using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;

namespace QuanLyKyTucXa.Infrastructure.Services;

public sealed record PersonalProfile(string MaSv, string HoTen, string Lop, string Khoa, string TrangThai);
public sealed record PersonalRoom(string MaPhong, string TenToa, string TenGiuong, DateOnly BatDau, DateOnly KetThuc, decimal TienPhong, string TrangThai);
public sealed record PersonalBill(string MaHoaDon, int Thang, int Nam, decimal TongTien, decimal DaThu, DateOnly HanThanhToan, string TrangThai);
public sealed record PersonalIncident(int MaSuCo, string MaPhong, string TieuDe, string TrangThai, DateTime NgayBao);
public sealed record PersonalData(PersonalProfile Profile, List<PersonalRoom> Rooms, List<PersonalBill> Bills, List<PersonalIncident> Incidents);

public sealed class PersonalService(Func<AppDbContext> createDb, AuthSession session)
{
    public async Task<PersonalData> LoadAsync()
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.Personal);
        var id = session.StudentId!;
        var profile = await db.SinhViens.Where(x => x.MaSv == id && x.MaNguoiDung == session.UserId)
            .Select(x => new PersonalProfile(x.MaSv, x.HoTen, x.Lop, x.Khoa, x.TrangThai)).SingleAsync();
        var rooms = await db.HopDongs.AsNoTracking().Where(x => x.MaSv == id).OrderByDescending(x => x.NgayBatDau)
            .Select(x => new PersonalRoom(x.MaGiuongNavigation.MaPhong, x.MaGiuongNavigation.MaPhongNavigation.MaToaNavigation.TenToa,
                x.MaGiuongNavigation.TenGiuong, x.NgayBatDau, x.NgayKetThuc, x.TienPhongThang, x.TrangThai)).ToListAsync();
        var bills = await db.HoaDons.AsNoTracking().Where(x => x.MaSv == id).OrderByDescending(x => x.Nam).ThenByDescending(x => x.Thang)
            .Select(x => new PersonalBill(x.MaHoaDon, x.Thang, x.Nam, x.TongTien, x.ThanhToans.Sum(t => (decimal?)t.SoTien) ?? 0m,
                x.HanThanhToan, x.TrangThai)).ToListAsync();
        var incidents = await db.SuCos.AsNoTracking().Where(x => x.MaSv == id).OrderByDescending(x => x.NgayBao)
            .Select(x => new PersonalIncident(x.MaSuCo, x.MaPhong, x.TieuDe, x.TrangThai, x.NgayBao)).ToListAsync();
        return new PersonalData(profile, rooms, bills, incidents);
    }
    public async Task ReportIncidentAsync(string title, string description)
    {
        title = (title ?? "").Trim(); description = (description ?? "").Trim();
        if (title.Length is < 1 or > 200 || description.Length is < 1 or > 2000)
            throw new InvalidOperationException("Tiêu đề phải từ 1–200 ký tự và mô tả từ 1–2000 ký tự.");
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.Personal);
        var room = await db.HopDongs.Where(x => x.MaSv == session.StudentId && x.TrangThai == "Hiệu lực")
            .Select(x => x.MaGiuongNavigation.MaPhong).SingleOrDefaultAsync()
            ?? throw new InvalidOperationException("Bạn chưa có phòng đang ở để gửi báo hỏng.");
        db.SuCos.Add(new SuCo { MaPhong = room, MaSv = session.StudentId, TieuDe = title, MoTa = description,
            MucDoUuTien = "Bình thường", NgayBao = DateTime.Now, TrangThai = "Mới tiếp nhận", ChiPhiSuaChua = 0 });
        await db.SaveChangesAsync();
    }
}