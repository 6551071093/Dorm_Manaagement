using System.Data;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa.Infrastructure.Data.Context;
using QuanLyKyTucXa.Infrastructure.Security;

namespace QuanLyKyTucXa.Infrastructure.Services;

public sealed record ContractRoomRow(string ContractId, string StudentId, string StudentName, string RoomId, string BedId, DateOnly Start, DateOnly End, decimal RoomPrice, string Status, decimal Outstanding, string? History);
public sealed record AvailableBedRow(string BedId, string RoomId, string TowerName, decimal RoomPrice);
public sealed record BedManagementRow(string BedId, string RoomId, string TowerName, string BedName, string? Position, string Status);

public sealed class RoomOperationsService(Func<AppDbContext> createDb, AuthSession? session)
{
    private static string AppendHistory(string? current, string entry)
    {
        const int maxLength = 255;
        if (string.IsNullOrWhiteSpace(current))
            return entry.Length <= maxLength ? entry : entry[^maxLength..];
        var separator = " | ";
        var maxEntryLength = maxLength - separator.Length;
        if (entry.Length > maxEntryLength) entry = entry[^maxEntryLength..];
        var keep = maxLength - separator.Length - entry.Length;
        // Keep the newest part of the previous history when the column is full.
        var prefix = current.Length > keep ? current.Substring(current.Length - keep) : current;
        return prefix + separator + entry;
    }

    public async Task<List<BedManagementRow>> GetBedsAsync(string? keyword = null)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageRooms);
        var query = db.Giuongs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var key = keyword.Trim();
            query = query.Where(x => x.MaGiuong.Contains(key) || x.MaPhong.Contains(key) || x.MaPhongNavigation.MaToaNavigation.TenToa.Contains(key));
        }
        return await query.OrderBy(x => x.MaPhong).ThenBy(x => x.MaGiuong)
            .Select(x => new BedManagementRow(x.MaGiuong,x.MaPhong,x.MaPhongNavigation.MaToaNavigation.TenToa,x.TenGiuong,x.ViTri,x.TrangThai)).ToListAsync();
    }

    public async Task SetBedConditionAsync(string bedId, string status)
    {
        if (status is not ("Trống" or "Hỏng" or "Bảo trì")) throw new InvalidOperationException("Chỉ chọn Trống, Hỏng hoặc Bảo trì; trạng thái Đang sử dụng được cập nhật theo hợp đồng.");
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ConfigureRooms);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var bed = await db.Giuongs.SingleOrDefaultAsync(x => x.MaGiuong == bedId)
            ?? throw new InvalidOperationException("Giường không còn tồn tại.");
        if (await db.HopDongs.AnyAsync(x => x.MaGiuong == bedId && x.TrangThai == "Hiệu lực"))
            throw new InvalidOperationException("Giường đang có hợp đồng hiệu lực, không thể đổi trạng thái.");
        bed.TrangThai = status;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task AddBedAsync(string roomId)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ConfigureRooms);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var room = await db.Phongs.Include(x => x.MaToaNavigation).SingleOrDefaultAsync(x => x.MaPhong == roomId)
            ?? throw new InvalidOperationException("Phòng không còn tồn tại.");
        if (room.TrangThai == "Đang sửa chữa" || room.MaToaNavigation.TrangThai != "Hoạt động")
            throw new InvalidOperationException("Không thể thêm giường vào phòng hoặc tòa đang bảo trì.");
        var bedCount = await db.Giuongs.CountAsync(x => x.MaPhong == roomId);
        if (bedCount != room.SoChoToiDa) throw new InvalidOperationException("Số giường hiện có không khớp sức chứa. Hãy mở sửa phòng để đồng bộ trước.");
        if (room.SoChoToiDa >= 1000) throw new InvalidOperationException("Sức chứa phòng đã đạt giới hạn.");
        var usedIds = (await db.Giuongs.Where(x => x.MaGiuong.StartsWith(roomId + "-G")).Select(x => x.MaGiuong).ToListAsync())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var index = 1;
        while (!usedIds.Add($"{roomId}-G{index}")) index++;
        room.SoChoToiDa++;
        room.TrangThai = room.SoNguoiHienTai >= room.SoChoToiDa ? "Đã đầy" : "Còn chỗ";
        db.Giuongs.Add(new Giuong { MaGiuong = $"{roomId}-G{index}", MaPhong = roomId, TenGiuong = $"Giường {index}", TrangThai = "Trống" });
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task DeleteBedAsync(string bedId)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.DeleteRooms);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var bed = await db.Giuongs.Include(x => x.MaPhongNavigation).SingleOrDefaultAsync(x => x.MaGiuong == bedId)
            ?? throw new InvalidOperationException("Giường không còn tồn tại.");
        var room = bed.MaPhongNavigation;
        if (bed.TrangThai != "Trống" || await db.HopDongs.AnyAsync(x => x.MaGiuong == bedId))
            throw new InvalidOperationException("Chỉ xóa được giường trống chưa có lịch sử hợp đồng.");
        var bedCount = await db.Giuongs.CountAsync(x => x.MaPhong == room.MaPhong);
        if (bedCount != room.SoChoToiDa) throw new InvalidOperationException("Số giường hiện có không khớp sức chứa. Hãy mở sửa phòng để đồng bộ trước.");
        if (bedCount <= 1 || room.SoNguoiHienTai > bedCount - 1)
            throw new InvalidOperationException("Không thể xóa giường cuối cùng hoặc làm sức chứa nhỏ hơn số người đang ở.");
        db.Giuongs.Remove(bed);
        room.SoChoToiDa--;
        room.TrangThai = room.TrangThai == "Đang sửa chữa" ? room.TrangThai
            : room.SoNguoiHienTai >= room.SoChoToiDa ? "Đã đầy" : "Còn chỗ";
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task<List<ToaNha>> GetTowersAsync()
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageRooms);
        return await db.ToaNhas.AsNoTracking().OrderBy(x => x.MaToa).ToListAsync();
    }

    public async Task SaveTowerAsync(ToaNha value, bool isNew)
    {
        value.MaToa = (value.MaToa ?? "").Trim();
        value.TenToa = (value.TenToa ?? "").Trim();
        value.LoaiToa = (value.LoaiToa ?? "").Trim();
        if (value.MaToa.Length is < 1 or > 10 || value.TenToa.Length is < 1 or > 100)
            throw new InvalidOperationException("Mã tòa tối đa 10 ký tự và tên tòa tối đa 100 ký tự.");
        if (value.SoTang is < 1 or > 100) throw new InvalidOperationException("Số tầng phải từ 1 đến 100.");
        if (value.LoaiToa is not ("Nam" or "Nữ" or "Hỗn hợp")) throw new InvalidOperationException("Loại tòa không hợp lệ.");
        if (value.TrangThai is not ("Hoạt động" or "Bảo trì" or "Dừng hoạt động")) throw new InvalidOperationException("Trạng thái tòa không hợp lệ.");
        if (value.GhiChu?.Length > 255) throw new InvalidOperationException("Ghi chú tối đa 255 ký tự.");
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ConfigureRooms);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var row = await db.ToaNhas.SingleOrDefaultAsync(x => x.MaToa == value.MaToa);
        if (isNew && row is not null) throw new InvalidOperationException("Mã tòa đã tồn tại.");
        if (!isNew && row is null) throw new InvalidOperationException("Tòa nhà không còn tồn tại.");
        if (row is null) { row = new ToaNha { MaToa = value.MaToa }; db.ToaNhas.Add(row); }
        var hasResidents = await db.HopDongs.AnyAsync(x => x.TrangThai == "Hiệu lực" && x.MaGiuongNavigation.MaPhongNavigation.MaToa == value.MaToa);
        if (hasResidents && row.LoaiToa != value.LoaiToa)
            throw new InvalidOperationException("Không đổi loại tòa khi đang có sinh viên lưu trú.");
        if (hasResidents && value.TrangThai != "Hoạt động")
            throw new InvalidOperationException("Không thể dừng/bảo trì tòa khi còn sinh viên đang lưu trú.");
        if (await db.Phongs.AnyAsync(x => x.MaToa == value.MaToa && x.Tang > value.SoTang))
            throw new InvalidOperationException("Số tầng mới không được thấp hơn tầng cao nhất đang có phòng.");
        row.TenToa = value.TenToa; row.LoaiToa = value.LoaiToa; row.SoTang = value.SoTang;
        row.TrangThai = value.TrangThai; row.GhiChu = string.IsNullOrWhiteSpace(value.GhiChu) ? null : value.GhiChu.Trim();
        row.SoPhong = await db.Phongs.CountAsync(x => x.MaToa == value.MaToa);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task DeleteTowerAsync(string id)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.DeleteRooms);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var tower = await db.ToaNhas.Include(x => x.Phongs).SingleOrDefaultAsync(x => x.MaToa == id)
            ?? throw new InvalidOperationException("Tòa nhà không còn tồn tại.");
        if (tower.Phongs.Count > 0) throw new InvalidOperationException("Không thể xóa tòa nhà còn phòng. Hãy xóa hoặc chuyển phòng trước.");
        db.ToaNhas.Remove(tower);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task<List<ContractRoomRow>> GetContractsAsync(string? keyword = null)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageRooms);
        var query = db.HopDongs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var key = keyword.Trim();
            query = query.Where(x => x.MaHopDong.Contains(key) || x.MaSv.Contains(key) || x.MaSvNavigation.HoTen.Contains(key) || x.MaGiuongNavigation.MaPhong.Contains(key));
        }
        return await query.OrderByDescending(x => x.NgayTao).Select(x => new ContractRoomRow(
            x.MaHopDong, x.MaSv, x.MaSvNavigation.HoTen, x.MaGiuongNavigation.MaPhong, x.MaGiuong,
            x.NgayBatDau, x.NgayKetThuc, x.TienPhongThang, x.TrangThai,
            (x.HoaDons.Where(h => h.TrangThai != "Hủy").Sum(h => (decimal?)h.TongTien) ?? 0m) -
            (x.HoaDons.Where(h => h.TrangThai != "Hủy").SelectMany(h => h.ThanhToans).Sum(p => (decimal?)p.SoTien) ?? 0m),
            x.GhiChu)).ToListAsync();
    }

    public async Task<List<AvailableBedRow>> GetAvailableBedsAsync(string contractId)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.ManageRooms);
        var contract = await db.HopDongs.AsNoTracking().SingleOrDefaultAsync(x => x.MaHopDong == contractId)
            ?? throw new InvalidOperationException("Không tìm thấy hợp đồng.");
        var sex = await db.SinhViens.Where(x => x.MaSv == contract.MaSv).Select(x => x.GioiTinh).SingleAsync();
        return await db.Giuongs.AsNoTracking().Where(x => x.TrangThai == "Trống" && x.MaGiuong != contract.MaGiuong
            && x.MaPhongNavigation.TrangThai != "Đang sửa chữa" && x.MaPhongNavigation.MaToaNavigation.TrangThai == "Hoạt động"
            && (x.MaPhongNavigation.MaToaNavigation.LoaiToa == "Hỗn hợp" || x.MaPhongNavigation.MaToaNavigation.LoaiToa == sex)
            && x.MaPhongNavigation.SoNguoiHienTai < x.MaPhongNavigation.SoChoToiDa)
            .OrderBy(x => x.MaPhong).ThenBy(x => x.MaGiuong)
            .Select(x => new AvailableBedRow(x.MaGiuong, x.MaPhong, x.MaPhongNavigation.MaToaNavigation.TenToa, x.MaPhongNavigation.GiaPhong)).ToListAsync();
    }

    public async Task TransferAsync(string contractId, string newBedId)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.Staff);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var contract = await db.HopDongs.Include(x => x.MaGiuongNavigation).Include(x => x.MaSvNavigation).SingleOrDefaultAsync(x => x.MaHopDong == contractId)
            ?? throw new InvalidOperationException("Không tìm thấy hợp đồng.");
        if (contract.TrangThai != "Hiệu lực") throw new InvalidOperationException("Chỉ chuyển phòng được với hợp đồng đang hiệu lực.");
        var bed = await db.Giuongs.Include(x => x.MaPhongNavigation).ThenInclude(x => x.MaToaNavigation).SingleOrDefaultAsync(x => x.MaGiuong == newBedId && x.TrangThai == "Trống")
            ?? throw new InvalidOperationException("Giường mới không còn trống. Hãy tải lại danh sách.");
        if (bed.MaPhongNavigation.MaToaNavigation.TrangThai != "Hoạt động")
            throw new InvalidOperationException("Tòa nhà mới đang bảo trì hoặc đã dừng hoạt động.");
        if (bed.MaPhongNavigation.TrangThai == "Đang sửa chữa" || bed.MaPhongNavigation.SoNguoiHienTai >= bed.MaPhongNavigation.SoChoToiDa)
            throw new InvalidOperationException("Phòng mới đang bảo trì hoặc đã đủ sức chứa.");
        if (bed.MaPhongNavigation.MaToaNavigation.LoaiToa is not ("Hỗn hợp") && bed.MaPhongNavigation.MaToaNavigation.LoaiToa != contract.MaSvNavigation.GioiTinh)
            throw new InvalidOperationException("Tòa nhà mới không phù hợp với giới tính sinh viên.");
        if (await db.HoaDons.AnyAsync(h => h.MaHopDong == contractId && h.TrangThai != "Hủy" && h.TongTien > (h.ThanhToans.Sum(p => (decimal?)p.SoTien) ?? 0m)))
            throw new InvalidOperationException("Cần thanh toán hết công nợ trước khi chuyển phòng.");
        var oldBedId = contract.MaGiuong;
        var oldRoomId = contract.MaGiuongNavigation.MaPhong;
        var oldPrice = contract.TienPhongThang;
        contract.MaGiuong = bed.MaGiuong;
        contract.TienPhongThang = bed.MaPhongNavigation.GiaPhong;
        contract.GhiChu = AppendHistory(contract.GhiChu,
            $"Chuyển {oldRoomId}/{oldBedId} → {bed.MaPhong}/{bed.MaGiuong} ngày {DateTime.Now:dd/MM/yyyy}; giá {oldPrice:N0} → {contract.TienPhongThang:N0} đ/tháng.");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task RenewAsync(string contractId, DateOnly newEndDate)
    {
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.Staff);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var contract = await db.HopDongs.SingleOrDefaultAsync(x => x.MaHopDong == contractId)
            ?? throw new InvalidOperationException("Không tìm thấy hợp đồng.");
        if (contract.TrangThai != "Hiệu lực") throw new InvalidOperationException("Chỉ gia hạn được hợp đồng đang hiệu lực.");
        if (newEndDate <= contract.NgayKetThuc || newEndDate <= DateOnly.FromDateTime(DateTime.Today))
            throw new InvalidOperationException("Ngày kết thúc mới phải sau ngày kết thúc hiện tại và sau hôm nay.");
        if (await db.HoaDons.AnyAsync(h => h.MaHopDong == contractId && h.TrangThai != "Hủy" && h.TongTien > (h.ThanhToans.Sum(p => (decimal?)p.SoTien) ?? 0m)))
            throw new InvalidOperationException("Cần thanh toán hết công nợ hóa đơn trước khi gia hạn.");
        var oldEndDate = contract.NgayKetThuc;
        contract.NgayKetThuc = newEndDate;
        contract.GhiChu = AppendHistory(contract.GhiChu, $"Gia hạn đến {newEndDate:dd/MM/yyyy} (trước đó {oldEndDate:dd/MM/yyyy}).");
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task CheckoutAsync(string contractId, string? handoverNote, bool assetsInspected, bool damageReported, bool compensationCollected, decimal compensationAmount, string paymentMethod)
    {
        handoverNote = string.IsNullOrWhiteSpace(handoverNote) ? null : handoverNote.Trim();
        if (handoverNote?.Length > 180) throw new InvalidOperationException("Ghi chú bàn giao tối đa 180 ký tự.");
        if (!assetsInspected) throw new InvalidOperationException("Cần kiểm tra tài sản và thiết bị phòng trước khi trả phòng.");
        if (compensationAmount < 0 || compensationAmount > 9999999999999999.99m) throw new InvalidOperationException("Tiền bồi thường không hợp lệ.");
        if (damageReported && (compensationAmount <= 0 || handoverNote is null)) throw new InvalidOperationException("Hư hỏng/mất mát phải có mô tả và khoản bồi thường lớn hơn 0.");
        if (damageReported && !compensationCollected) throw new InvalidOperationException("Cần xác nhận đã lập biên bản và thực thu khoản bồi thường.");
        if (!damageReported && compensationAmount > 0) throw new InvalidOperationException("Chỉ ghi nhận khoản thu khi đã chọn có hư hỏng/mất mát.");
        if (compensationAmount > 0 && paymentMethod is not ("Tiền mặt" or "Chuyển khoản VietQR" or "Chuyển khoản ngân hàng"))
            throw new InvalidOperationException("Vui lòng chọn phương thức thu bồi thường hợp lệ.");
        await using var db = createDb();
        await AuthPolicy.RequireAsync(db, session, AppPermission.Staff);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var contract = await db.HopDongs.SingleOrDefaultAsync(x => x.MaHopDong == contractId)
            ?? throw new InvalidOperationException("Không tìm thấy hợp đồng.");
        if (contract.TrangThai != "Hiệu lực") throw new InvalidOperationException("Hợp đồng này đã được kết thúc.");
        if (await db.HoaDons.AnyAsync(h => h.MaHopDong == contractId && h.TrangThai != "Hủy" && h.TongTien > (h.ThanhToans.Sum(p => (decimal?)p.SoTien) ?? 0m)))
            throw new InvalidOperationException("Cần thanh toán hết công nợ hóa đơn trước khi trả phòng.");
        if (compensationAmount > 0)
        {
            var bed = await db.Giuongs.Include(x => x.MaPhongNavigation).SingleAsync(x => x.MaGiuong == contract.MaGiuong);
            var today = DateOnly.FromDateTime(DateTime.Today);
            var bill = await db.HoaDons.Where(x => x.MaHopDong == contractId && x.Thang == today.Month && x.Nam == today.Year && x.TrangThai != "Hủy")
                .SingleOrDefaultAsync();
            if (bill is null)
            {
                var usedPeriods = await db.HoaDons.Where(x => x.MaHopDong == contractId).Select(x => new { x.Thang, x.Nam }).ToListAsync();
                var year = today.Year; var month = today.Month;
                for (var i = 0; i < 120 && usedPeriods.Any(x => x.Thang == month && x.Nam == year); i++)
                { month++; if (month > 12) { month = 1; year++; } }
                if (usedPeriods.Any(x => x.Thang == month && x.Nam == year)) throw new InvalidOperationException("Không tìm được kỳ hóa đơn còn trống để ghi nhận bồi thường.");
                bill = new HoaDon
                {
                    MaHoaDon = $"BT-{DateTime.Now:yyMMdd}-{Guid.NewGuid().ToString("N")[..15].ToUpperInvariant()}",
                    MaHopDong = contract.MaHopDong, MaSv = contract.MaSv, MaPhong = bed.MaPhong,
                    Thang = month, Nam = year, TongTien = 0, HanThanhToan = today,
                    TrangThai = "Chưa thanh toán", NgayLap = DateTime.Now, GhiChu = "Hóa đơn bồi thường khi bàn giao phòng."
                };
                db.HoaDons.Add(bill);
                await db.SaveChangesAsync();
            }
            db.ChiTietHoaDons.Add(new ChiTietHoaDon
            {
                MaHoaDon = bill.MaHoaDon, LoaiTien = "Bồi thường tài sản phòng", SoLuong = 1,
                DonGia = compensationAmount, ThanhTien = compensationAmount,
                GhiChu = string.IsNullOrWhiteSpace(handoverNote) ? "Bồi thường khi bàn giao phòng." : handoverNote
            });
            db.ThanhToans.Add(new ThanhToan
            {
                MaHoaDon = bill.MaHoaDon, NgayThanhToan = DateTime.Now, SoTien = compensationAmount,
                PhuongThuc = paymentMethod, NguoiThu = session!.FullName,
                GhiChu = "Đã thu tiền bồi thường tài sản/thiết bị khi trả phòng."
            });
        }
        contract.TrangThai = "Đã thanh lý";
        var handover = damageReported
            ? $"Bàn giao: đã kiểm tra; đã lập biên bản, thực thu {compensationAmount:N0} đ ({paymentMethod}). {handoverNote}"
            : handoverNote is null ? "Bàn giao: tài sản đã kiểm tra, không ghi nhận hư hỏng/mất mát."
                : $"Bàn giao: đã kiểm tra; {handoverNote}";
        contract.GhiChu = AppendHistory(contract.GhiChu, handover);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
