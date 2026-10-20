-- ============================================================================
-- DỰ ÁN: QUẢN LÝ KÝ TÚC XÁ (QuanLyKyTucXa)
-- HỌC PHẦN: LẬP TRÌNH TRỰC QUAN (LTTQ) - .NET 9 & C# 12/13
-- FILE: 03_StoredProcedures_Views.sql
-- MỤC ĐÍCH: Views & Stored Procedures phục vụ Báo cáo, ADO.NET thuần (Chương 7)
-- ============================================================================

USE QuanLyKTXDB;
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ============================================================================
-- PHẦN 1: VIEWS TỔNG HỢP & BÁO CÁO
-- ============================================================================

-- View 1: Danh sách sinh viên đang nội trú kèm thông tin phòng giường
IF OBJECT_ID('dbo.vw_DanhSachSinhVienNoTru', 'V') IS NOT NULL DROP VIEW dbo.vw_DanhSachSinhVienNoTru;
GO
CREATE VIEW dbo.vw_DanhSachSinhVienNoTru
AS
SELECT 
    sv.MaSV,
    sv.HoTen,
    sv.NgaySinh,
    sv.GioiTinh,
    sv.CCCD,
    sv.SoDienThoai,
    sv.Email,
    sv.Lop,
    sv.Khoa,
    sv.QueQuan,
    hd.MaHopDong,
    hd.NgayBatDau,
    hd.NgayKetThuc,
    g.MaGiuong,
    g.TenGiuong,
    p.MaPhong,
    p.TenPhong,
    p.Tang,
    tn.MaToa,
    tn.TenToa
FROM dbo.SinhVien sv
INNER JOIN dbo.HopDong hd ON sv.MaSV = hd.MaSV AND hd.TrangThai = N'Hiệu lực'
INNER JOIN dbo.Giuong g ON hd.MaGiuong = g.MaGiuong
INNER JOIN dbo.Phong p ON g.MaPhong = p.MaPhong
INNER JOIN dbo.ToaNha tn ON p.MaToa = tn.MaToa;
GO

-- View 2: Tình trạng phòng và tỷ lệ lấp đầy theo tòa nhà
IF OBJECT_ID('dbo.vw_TinhTrangPhongKTX', 'V') IS NOT NULL DROP VIEW dbo.vw_TinhTrangPhongKTX;
GO
CREATE VIEW dbo.vw_TinhTrangPhongKTX
AS
SELECT 
    tn.MaToa,
    tn.TenToa,
    p.MaPhong,
    p.TenPhong,
    p.Tang,
    p.LoaiPhong,
    p.SoChoToiDa,
    p.SoNguoiHienTai,
    (p.SoChoToiDa - p.SoNguoiHienTai) AS SoChoTrong,
    p.GiaPhong,
    p.TrangThai,
    CASE 
        WHEN p.SoChoToiDa > 0 THEN CAST((CAST(p.SoNguoiHienTai AS FLOAT) / p.SoChoToiDa) * 100 AS DECIMAL(5,2))
        ELSE 0 
    END AS TyLeLapDay
FROM dbo.Phong p
INNER JOIN dbo.ToaNha tn ON p.MaToa = tn.MaToa;
GO

-- View 3: Tổng hợp điện nước và tiêu thụ theo phòng
IF OBJECT_ID('dbo.vw_BaoCaoDienNuoc', 'V') IS NOT NULL DROP VIEW dbo.vw_BaoCaoDienNuoc;
GO
CREATE VIEW dbo.vw_BaoCaoDienNuoc
AS
SELECT 
    cs.MaChiSo,
    cs.MaPhong,
    p.TenPhong,
    p.MaToa,
    cs.Thang,
    cs.Nam,
    cs.ChiSoDienDau,
    cs.ChiSoDienCuoi,
    (cs.ChiSoDienCuoi - cs.ChiSoDienDau) AS SoDienTieuThu,
    cs.ChiSoNuocDau,
    cs.ChiSoNuocCuoi,
    (cs.ChiSoNuocCuoi - cs.ChiSoNuocDau) AS SoNuocTieuThu,
    cs.NgayGhi,
    cs.NguoiGhi
FROM dbo.ChiSoDienNuoc cs
INNER JOIN dbo.Phong p ON cs.MaPhong = p.MaPhong;
GO

-- View 4: Công nợ và hóa đơn (Khắc phục hoàn toàn số dư âm & gắn rõ sinh viên)
IF OBJECT_ID('dbo.vw_CongNoHoaDon', 'V') IS NOT NULL DROP VIEW dbo.vw_CongNoHoaDon;
GO
CREATE VIEW dbo.vw_CongNoHoaDon
AS
SELECT 
    hd.MaHoaDon,
    hd.MaHopDong,
    hd.MaSV,
    sv.HoTen AS TenSinhVien,
    sv.SoDienThoai,
    hd.MaPhong,
    p.TenPhong,
    p.MaToa,
    hd.Thang,
    hd.Nam,
    hd.TongTien,
    ISNULL(tt.DaThanhToan, 0) AS DaThanhToan,
    CASE 
        WHEN hd.TongTien > ISNULL(tt.DaThanhToan, 0) THEN (hd.TongTien - ISNULL(tt.DaThanhToan, 0))
        ELSE 0
    END AS ConNo,
    hd.HanThanhToan,
    hd.TrangThai,
    CASE 
        WHEN hd.TrangThai IN (N'Chưa thanh toán', N'Quá hạn') AND hd.HanThanhToan < CAST(GETDATE() AS DATE) THEN 1
        ELSE 0
    END AS DaQuaHan
FROM dbo.HoaDon hd
INNER JOIN dbo.SinhVien sv ON hd.MaSV = sv.MaSV
INNER JOIN dbo.Phong p ON hd.MaPhong = p.MaPhong
LEFT JOIN (
    SELECT MaHoaDon, SUM(SoTien) AS DaThanhToan
    FROM dbo.ThanhToan
    GROUP BY MaHoaDon
) tt ON hd.MaHoaDon = tt.MaHoaDon;
GO

-- ============================================================================
-- PHẦN 2: STORED PROCEDURES (MINH CHỨNG ADO.NET THUẦN - CHƯƠNG 7)
-- ============================================================================

-- SP 1: Thống kê số liệu Dashboard thời gian thực
IF OBJECT_ID('dbo.sp_ThongKeDashboard', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_ThongKeDashboard;
GO
CREATE PROCEDURE dbo.sp_ThongKeDashboard
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TongSinhVien INT, @TongPhong INT, @TongGiuong INT, @GiuongDangO INT;
    DECLARE @DoanhThuThang DECIMAL(18,2), @SuCoChuaXuLy INT;
    DECLARE @CurrentMonth INT = MONTH(GETDATE()), @CurrentYear INT = YEAR(GETDATE());

    SELECT @TongSinhVien = COUNT(1) FROM dbo.SinhVien WHERE TrangThai = N'Đang ở';
    SELECT @TongPhong = COUNT(1) FROM dbo.Phong;
    SELECT @TongGiuong = COUNT(1) FROM dbo.Giuong;
    SELECT @GiuongDangO = COUNT(1) FROM dbo.Giuong WHERE TrangThai = N'Đang sử dụng';

    SELECT @DoanhThuThang = ISNULL(SUM(SoTien), 0)
    FROM dbo.ThanhToan
    WHERE MONTH(NgayThanhToan) = @CurrentMonth AND YEAR(NgayThanhToan) = @CurrentYear;

    SELECT @SuCoChuaXuLy = COUNT(1) FROM dbo.SuCo WHERE TrangThai IN (N'Mới tiếp nhận', N'Đang xử lý');

    SELECT 
        @TongSinhVien AS TongSinhVien,
        @TongPhong AS TongPhong,
        @TongGiuong AS TongGiuong,
        @GiuongDangO AS GiuongDangO,
        CASE WHEN @TongGiuong > 0 THEN CAST((CAST(@GiuongDangO AS FLOAT) / @TongGiuong) * 100 AS DECIMAL(5,2)) ELSE 0 END AS TyLeLapDay,
        @DoanhThuThang AS DoanhThuThangHienTai,
        @SuCoChuaXuLy AS SuCoChuaXuLy;
END;
GO

-- SP 2: Thống kê doanh thu 12 tháng theo năm (Phục vụ vẽ biểu đồ GDI+ / LiveCharts2)
IF OBJECT_ID('dbo.sp_ThongKeDoanhThuTheoThang', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_ThongKeDoanhThuTheoThang;
GO
CREATE PROCEDURE dbo.sp_ThongKeDoanhThuTheoThang
    @Nam INT
AS
BEGIN
    SET NOCOUNT ON;

    WITH Months AS (
        SELECT 1 AS Thang UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4
        UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7 UNION ALL SELECT 8
        UNION ALL SELECT 9 UNION ALL SELECT 10 UNION ALL SELECT 11 UNION ALL SELECT 12
    )
    SELECT 
        m.Thang,
        ISNULL(SUM(t.SoTien), 0) AS TongDoanhThu,
        COUNT(t.MaThanhToan) AS SoGiaoDich
    FROM Months m
    LEFT JOIN dbo.ThanhToan t ON MONTH(t.NgayThanhToan) = m.Thang AND YEAR(t.NgayThanhToan) = @Nam
    GROUP BY m.Thang
    ORDER BY m.Thang;
END;
GO

-- SP 3: Lấy danh sách hóa đơn quá hạn chưa thanh toán (Khắc phục triệt để lỗi nhân bản dòng)
IF OBJECT_ID('dbo.sp_GetSinhVienChuaThanhToanHoaDon', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetSinhVienChuaThanhToanHoaDon;
GO
CREATE PROCEDURE dbo.sp_GetSinhVienChuaThanhToanHoaDon
    @Thang INT = NULL,
    @Nam INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        hd.MaHoaDon,
        hd.MaHopDong,
        sv.MaSV,
        sv.HoTen AS TenSinhVien,
        sv.SoDienThoai,
        sv.Email,
        p.MaPhong,
        p.TenPhong,
        tn.TenToa,
        hd.Thang,
        hd.Nam,
        hd.TongTien,
        ISNULL(tt.DaThanhToan, 0) AS DaThanhToan,
        (hd.TongTien - ISNULL(tt.DaThanhToan, 0)) AS ConNo,
        hd.HanThanhToan,
        hd.TrangThai
    FROM dbo.HoaDon hd
    INNER JOIN dbo.SinhVien sv ON hd.MaSV = sv.MaSV
    INNER JOIN dbo.Phong p ON hd.MaPhong = p.MaPhong
    INNER JOIN dbo.ToaNha tn ON p.MaToa = tn.MaToa
    LEFT JOIN (
        SELECT MaHoaDon, SUM(SoTien) AS DaThanhToan
        FROM dbo.ThanhToan
        GROUP BY MaHoaDon
    ) tt ON hd.MaHoaDon = tt.MaHoaDon
    WHERE (hd.TrangThai = N'Quá hạn' OR (hd.TrangThai = N'Chưa thanh toán' AND hd.HanThanhToan < CAST(GETDATE() AS DATE)))
      AND (@Thang IS NULL OR hd.Thang = @Thang)
      AND (@Nam IS NULL OR hd.Nam = @Nam)
    ORDER BY hd.HanThanhToan ASC, p.MaPhong;
END;
GO

-- SP 4: Nghiệp vụ tự động lập hóa đơn hàng tháng cho phòng và chia đều điện nước cho các sinh viên
-- (Đặc sắc cho bài toán thực tế & minh chứng ADO.NET thuần Chương 7)
IF OBJECT_ID('dbo.sp_LapHoaDonHangThangChoPhong', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_LapHoaDonHangThangChoPhong;
GO
CREATE PROCEDURE dbo.sp_LapHoaDonHangThangChoPhong
    @MaPhong NVARCHAR(20),
    @Thang INT,
    @Nam INT,
    @HanThanhToan DATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Lấy chỉ số điện nước tháng tương ứng của phòng
        DECLARE @DienTieuThu INT, @NuocTieuThu INT;
        SELECT 
            @DienTieuThu = (ChiSoDienCuoi - ChiSoDienDau),
            @NuocTieuThu = (ChiSoNuocCuoi - ChiSoNuocDau)
        FROM dbo.ChiSoDienNuoc
        WHERE MaPhong = @MaPhong AND Thang = @Thang AND Nam = @Nam;

        IF @DienTieuThu IS NULL
        BEGIN
            RAISERROR(N'Chưa có chỉ số điện nước tháng %d/%d cho phòng %s!', 16, 1, @Thang, @Nam, @MaPhong);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Lấy đơn giá điện và nước hiện hành
        DECLARE @DonGiaDien DECIMAL(18,2) = 0, @DonGiaNuoc DECIMAL(18,2) = 0;
        SELECT TOP 1 @DonGiaDien = DonGia FROM dbo.BangGia WHERE TenDichVu LIKE N'%điện%' AND TrangThai = 1 ORDER BY NgayApDung DESC;
        SELECT TOP 1 @DonGiaNuoc = DonGia FROM dbo.BangGia WHERE TenDichVu LIKE N'%nước%' AND TrangThai = 1 ORDER BY NgayApDung DESC;
        IF @DonGiaDien = 0 SET @DonGiaDien = 3500;
        IF @DonGiaNuoc = 0 SET @DonGiaNuoc = 15000;

        DECLARE @TongTienDienPhong DECIMAL(18,2) = @DienTieuThu * @DonGiaDien;
        DECLARE @TongTienNuocPhong DECIMAL(18,2) = @NuocTieuThu * @DonGiaNuoc;

        -- 3. Lấy danh sách hợp đồng đang hiệu lực trong phòng này
        DECLARE @HopDongList TABLE (
            MaHopDong NVARCHAR(30),
            MaSV NVARCHAR(20),
            TienPhongThang DECIMAL(18,2)
        );

        INSERT INTO @HopDongList (MaHopDong, MaSV, TienPhongThang)
        SELECT hd.MaHopDong, hd.MaSV, hd.TienPhongThang
        FROM dbo.HopDong hd
        INNER JOIN dbo.Giuong g ON hd.MaGiuong = g.MaGiuong
        WHERE g.MaPhong = @MaPhong AND hd.TrangThai = N'Hiệu lực';

        DECLARE @SoNguoi INT = (SELECT COUNT(1) FROM @HopDongList);
        IF @SoNguoi = 0
        BEGIN
            RAISERROR(N'Phòng %s hiện không có sinh viên nào đang có hợp đồng hiệu lực!', 16, 1, @MaPhong);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4. Chia đều tiền điện nước cho mỗi sinh viên trong phòng
        DECLARE @TienDienMoiNguoi DECIMAL(18,2) = ROUND(@TongTienDienPhong / @SoNguoi, 0);
        DECLARE @TienNuocMoiNguoi DECIMAL(18,2) = ROUND(@TongTienNuocPhong / @SoNguoi, 0);

        -- 5. Duyệt từng sinh viên để tạo Hóa đơn cá nhân
        DECLARE @CurMaHopDong NVARCHAR(30), @CurMaSV NVARCHAR(20), @CurTienPhong DECIMAL(18,2);
        DECLARE @CurMaHoaDon NVARCHAR(30);

        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT MaHopDong, MaSV, TienPhongThang FROM @HopDongList;

        OPEN cur;
        FETCH NEXT FROM cur INTO @CurMaHopDong, @CurMaSV, @CurTienPhong;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @CurMaHoaDon = N'HD-' + CAST(@Nam AS NVARCHAR(4)) + RIGHT(N'00' + CAST(@Thang AS NVARCHAR(2)), 2) + N'-' + @CurMaSV;

            -- Xóa chi tiết cũ nếu đã tạo trước đó
            IF EXISTS (SELECT 1 FROM dbo.HoaDon WHERE MaHoaDon = @CurMaHoaDon)
            BEGIN
                DELETE FROM dbo.ChiTietHoaDon WHERE MaHoaDon = @CurMaHoaDon;
                DELETE FROM dbo.HoaDon WHERE MaHoaDon = @CurMaHoaDon;
            END

            -- Tạo vỏ hóa đơn
            INSERT INTO dbo.HoaDon (MaHoaDon, MaHopDong, MaSV, MaPhong, Thang, Nam, TongTien, HanThanhToan, TrangThai, GhiChu)
            VALUES (@CurMaHoaDon, @CurMaHopDong, @CurMaSV, @MaPhong, @Thang, @Nam, 0, @HanThanhToan, N'Chưa thanh toán', 
                    N'Hóa đơn tháng ' + CAST(@Thang AS NVARCHAR(2)) + N'/' + CAST(@Nam AS NVARCHAR(4)));

            -- Thêm chi tiết: Tiền phòng
            INSERT INTO dbo.ChiTietHoaDon (MaHoaDon, LoaiTien, SoLuong, DonGia, ThanhTien, GhiChu)
            VALUES (@CurMaHoaDon, N'Tiền phòng', 1, @CurTienPhong, @CurTienPhong, N'Tiền phòng theo hợp đồng');

            -- Thêm chi tiết: Phần tiền điện chia sẻ
            INSERT INTO dbo.ChiTietHoaDon (MaHoaDon, LoaiTien, SoLuong, DonGia, ThanhTien, GhiChu)
            VALUES (@CurMaHoaDon, N'Tiền điện', CAST(@DienTieuThu AS DECIMAL(10,2)) / @SoNguoi, @DonGiaDien, @TienDienMoiNguoi, 
                    N'Điện tiêu thụ phòng: ' + CAST(@DienTieuThu AS NVARCHAR(10)) + N' kWh chia ' + CAST(@SoNguoi AS NVARCHAR(5)) + N' người');

            -- Thêm chi tiết: Phần tiền nước chia sẻ
            INSERT INTO dbo.ChiTietHoaDon (MaHoaDon, LoaiTien, SoLuong, DonGia, ThanhTien, GhiChu)
            VALUES (@CurMaHoaDon, N'Tiền nước', CAST(@NuocTieuThu AS DECIMAL(10,2)) / @SoNguoi, @DonGiaNuoc, @TienNuocMoiNguoi, 
                    N'Nước tiêu thụ phòng: ' + CAST(@NuocTieuThu AS NVARCHAR(10)) + N' m3 chia ' + CAST(@SoNguoi AS NVARCHAR(5)) + N' người');

            FETCH NEXT FROM cur INTO @CurMaHopDong, @CurMaSV, @CurTienPhong;
        END

        CLOSE cur;
        DEALLOCATE cur;

        COMMIT TRANSACTION;
        SELECT @SoNguoi AS SoHoaDonDaTao;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
