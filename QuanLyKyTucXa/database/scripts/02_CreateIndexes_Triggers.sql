-- ============================================================================
-- DỰ ÁN: QUẢN LÝ KÝ TÚC XÁ (QuanLyKyTucXa)
-- HỌC PHẦN: LẬP TRÌNH TRỰC QUAN (LTTQ) - .NET 9 & C# 12/13
-- FILE: 02_CreateIndexes_Triggers.sql
-- MỤC ĐÍCH: Tạo các Index tối ưu hóa truy vấn, Filtered Unique Index và Triggers toàn vẹn
-- ============================================================================

USE QuanLyKTXDB;
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- ============================================================================
-- PHẦN 1: INDEXES & FILTERED UNIQUE INDEXES
-- ============================================================================

-- 1. Index tìm kiếm sinh viên
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.SinhVien') AND name = 'IX_SinhVien_HoTen')
    CREATE NONCLUSTERED INDEX IX_SinhVien_HoTen ON dbo.SinhVien (HoTen);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.SinhVien') AND name = 'IX_SinhVien_Lop_Khoa')
    CREATE NONCLUSTERED INDEX IX_SinhVien_Lop_Khoa ON dbo.SinhVien (Lop, Khoa);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.SinhVien') AND name = 'IX_SinhVien_TrangThai')
    CREATE NONCLUSTERED INDEX IX_SinhVien_TrangThai ON dbo.SinhVien (TrangThai);

-- 2. Index quản lý phòng và tòa nhà
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Phong') AND name = 'IX_Phong_MaToa_Tang')
    CREATE NONCLUSTERED INDEX IX_Phong_MaToa_Tang ON dbo.Phong (MaToa, Tang);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Phong') AND name = 'IX_Phong_TrangThai')
    CREATE NONCLUSTERED INDEX IX_Phong_TrangThai ON dbo.Phong (TrangThai);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Giuong') AND name = 'IX_Giuong_TrangThai')
    CREATE NONCLUSTERED INDEX IX_Giuong_TrangThai ON dbo.Giuong (TrangThai);

-- 3. Filtered Unique Indexes cho Hợp đồng (Chặn xung đột đặt trùng giường & trùng hợp đồng)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.HopDong') AND name = 'UQ_HopDong_Giuong_HieuLuc')
    CREATE UNIQUE NONCLUSTERED INDEX UQ_HopDong_Giuong_HieuLuc 
    ON dbo.HopDong (MaGiuong) 
    WHERE TrangThai = N'Hiệu lực';

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.HopDong') AND name = 'UQ_HopDong_SinhVien_HieuLuc')
    CREATE UNIQUE NONCLUSTERED INDEX UQ_HopDong_SinhVien_HieuLuc 
    ON dbo.HopDong (MaSV) 
    WHERE TrangThai = N'Hiệu lực';

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.HopDong') AND name = 'IX_HopDong_MaSV')
    CREATE NONCLUSTERED INDEX IX_HopDong_MaSV ON dbo.HopDong (MaSV);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.HopDong') AND name = 'IX_HopDong_TrangThai_NgayKetThuc')
    CREATE NONCLUSTERED INDEX IX_HopDong_TrangThai_NgayKetThuc ON dbo.HopDong (TrangThai, NgayKetThuc);

-- 4. Index hóa đơn & chi tiết hóa đơn
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.HoaDon') AND name = 'IX_HoaDon_MaSV_TrangThai')
    CREATE NONCLUSTERED INDEX IX_HoaDon_MaSV_TrangThai ON dbo.HoaDon (MaSV, TrangThai);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.HoaDon') AND name = 'IX_HoaDon_MaPhong_Thang_Nam')
    CREATE NONCLUSTERED INDEX IX_HoaDon_MaPhong_Thang_Nam ON dbo.HoaDon (MaPhong, Thang, Nam);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.HoaDon') AND name = 'IX_HoaDon_TrangThai_HanThanhToan')
    CREATE NONCLUSTERED INDEX IX_HoaDon_TrangThai_HanThanhToan ON dbo.HoaDon (TrangThai, HanThanhToan);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.ChiTietHoaDon') AND name = 'IX_ChiTietHoaDon_MaHoaDon')
    CREATE NONCLUSTERED INDEX IX_ChiTietHoaDon_MaHoaDon ON dbo.ChiTietHoaDon (MaHoaDon);

-- 5. Index quản lý sự cố
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.SuCo') AND name = 'IX_SuCo_TrangThai')
    CREATE NONCLUSTERED INDEX IX_SuCo_TrangThai ON dbo.SuCo (TrangThai);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.SuCo') AND name = 'IX_SuCo_MaPhong')
    CREATE NONCLUSTERED INDEX IX_SuCo_MaPhong ON dbo.SuCo (MaPhong);
GO

-- ============================================================================
-- PHẦN 2: TRIGGERS BẢO TOÀN VÀ ĐỒNG BỘ DỮ LIỆU TOÀN DIỆN
-- ============================================================================

-- Trigger 1: Tự động cộng tổng tiền hóa đơn & cập nhật lại trạng thái hóa đơn
IF OBJECT_ID('dbo.TR_ChiTietHoaDon_CapNhatTongTien', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_ChiTietHoaDon_CapNhatTongTien;
GO

CREATE TRIGGER dbo.TR_ChiTietHoaDon_CapNhatTongTien
ON dbo.ChiTietHoaDon
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AffectedInvoices TABLE (MaHoaDon NVARCHAR(30));
    
    INSERT INTO @AffectedInvoices
    SELECT DISTINCT MaHoaDon FROM inserted
    UNION
    SELECT DISTINCT MaHoaDon FROM deleted;

    -- 1. Cập nhật lại TongTien trong HoaDon
    UPDATE h
    SET h.TongTien = ISNULL((
        SELECT SUM(c.ThanhTien)
        FROM dbo.ChiTietHoaDon c
        WHERE c.MaHoaDon = h.MaHoaDon
    ), 0)
    FROM dbo.HoaDon h
    INNER JOIN @AffectedInvoices a ON h.MaHoaDon = a.MaHoaDon;

    -- 2. Tái tính toán trạng thái hóa đơn dựa trên tổng thanh toán hiện có
    UPDATE h
    SET h.TrangThai = CASE 
        WHEN ISNULL(p.TotalPaid, 0) >= h.TongTien AND h.TongTien > 0 THEN N'Đã thanh toán'
        WHEN h.HanThanhToan < CAST(GETDATE() AS DATE) THEN N'Quá hạn'
        ELSE N'Chưa thanh toán'
    END
    FROM dbo.HoaDon h
    INNER JOIN @AffectedInvoices a ON h.MaHoaDon = a.MaHoaDon
    LEFT JOIN (
        SELECT MaHoaDon, SUM(SoTien) AS TotalPaid
        FROM dbo.ThanhToan
        GROUP BY MaHoaDon
    ) p ON h.MaHoaDon = p.MaHoaDon;
END;
GO

-- Trigger 2: Tự động cập nhật trạng thái Hóa đơn khi có phát sinh Thanh toán
IF OBJECT_ID('dbo.TR_ThanhToan_CapNhatTrangThaiHoaDon', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_ThanhToan_CapNhatTrangThaiHoaDon;
GO

CREATE TRIGGER dbo.TR_ThanhToan_CapNhatTrangThaiHoaDon
ON dbo.ThanhToan
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AffectedInvoices TABLE (MaHoaDon NVARCHAR(30));
    
    INSERT INTO @AffectedInvoices
    SELECT DISTINCT MaHoaDon FROM inserted
    UNION
    SELECT DISTINCT MaHoaDon FROM deleted;

    UPDATE h
    SET h.TrangThai = CASE 
        WHEN ISNULL(p.TotalPaid, 0) >= h.TongTien AND h.TongTien > 0 THEN N'Đã thanh toán'
        WHEN h.HanThanhToan < CAST(GETDATE() AS DATE) THEN N'Quá hạn'
        ELSE N'Chưa thanh toán'
    END
    FROM dbo.HoaDon h
    INNER JOIN @AffectedInvoices a ON h.MaHoaDon = a.MaHoaDon
    LEFT JOIN (
        SELECT MaHoaDon, SUM(SoTien) AS TotalPaid
        FROM dbo.ThanhToan
        GROUP BY MaHoaDon
    ) p ON h.MaHoaDon = p.MaHoaDon;
END;
GO

-- Trigger 3: Đồng bộ trạng thái Giường, Sĩ số Phòng và Trạng thái Sinh viên khi thay đổi Hợp đồng
-- Xử lý đầy đủ: Thêm hợp đồng, Hủy/Thanh lý, Chuyển giường (Update MaGiuong), Xóa hợp đồng
IF OBJECT_ID('dbo.TR_HopDong_DongBoPhongGiuong', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_HopDong_DongBoPhongGiuong;
GO

CREATE TRIGGER dbo.TR_HopDong_DongBoPhongGiuong
ON dbo.HopDong
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Tập hợp toàn bộ giường bị ảnh hưởng (cả giường mới từ inserted và giường cũ từ deleted)
    DECLARE @AffectedBeds TABLE (MaGiuong NVARCHAR(30));
    INSERT INTO @AffectedBeds
    SELECT DISTINCT MaGiuong FROM inserted WHERE MaGiuong IS NOT NULL
    UNION
    SELECT DISTINCT MaGiuong FROM deleted WHERE MaGiuong IS NOT NULL;

    -- Tập hợp toàn bộ sinh viên bị ảnh hưởng
    DECLARE @AffectedStudents TABLE (MaSV NVARCHAR(20));
    INSERT INTO @AffectedStudents
    SELECT DISTINCT MaSV FROM inserted WHERE MaSV IS NOT NULL
    UNION
    SELECT DISTINCT MaSV FROM deleted WHERE MaSV IS NOT NULL;

    -- 1. Đồng bộ trạng thái Giường
    -- Nếu có hợp đồng 'Hiệu lực' -> Đang sử dụng; Ngược lại -> Trống (nếu không bị Hỏng/Bảo trì)
    UPDATE g
    SET g.TrangThai = CASE 
        WHEN EXISTS (
            SELECT 1 FROM dbo.HopDong hd 
            WHERE hd.MaGiuong = g.MaGiuong AND hd.TrangThai = N'Hiệu lực'
        ) THEN N'Đang sử dụng'
        WHEN g.TrangThai IN (N'Hỏng', N'Bảo trì') THEN g.TrangThai
        ELSE N'Trống'
    END
    FROM dbo.Giuong g
    INNER JOIN @AffectedBeds b ON g.MaGiuong = b.MaGiuong;

    -- 2. Đồng bộ Sĩ số và Trạng thái Phòng
    DECLARE @AffectedRooms TABLE (MaPhong NVARCHAR(20));
    INSERT INTO @AffectedRooms
    SELECT DISTINCT g.MaPhong 
    FROM dbo.Giuong g
    INNER JOIN @AffectedBeds b ON g.MaGiuong = b.MaGiuong;

    UPDATE p
    SET p.SoNguoiHienTai = ISNULL((
        SELECT COUNT(DISTINCT hd.MaSV)
        FROM dbo.HopDong hd
        INNER JOIN dbo.Giuong g ON hd.MaGiuong = g.MaGiuong
        WHERE g.MaPhong = p.MaPhong AND hd.TrangThai = N'Hiệu lực'
    ), 0)
    FROM dbo.Phong p
    INNER JOIN @AffectedRooms r ON p.MaPhong = r.MaPhong;

    UPDATE p
    SET p.TrangThai = CASE 
        WHEN p.TrangThai = N'Đang sửa chữa' THEN N'Đang sửa chữa'
        WHEN p.SoNguoiHienTai >= p.SoChoToiDa THEN N'Đã đầy'
        ELSE N'Còn chỗ'
    END
    FROM dbo.Phong p
    INNER JOIN @AffectedRooms r ON p.MaPhong = r.MaPhong;

    -- 3. Đồng bộ trạng thái Sinh viên (Đang ở hoặc Đã rời KTX)
    UPDATE sv
    SET sv.TrangThai = CASE 
        WHEN EXISTS (
            SELECT 1 FROM dbo.HopDong hd 
            WHERE hd.MaSV = sv.MaSV AND hd.TrangThai = N'Hiệu lực'
        ) THEN N'Đang ở'
        ELSE N'Đã rời KTX'
    END
    FROM dbo.SinhVien sv
    INNER JOIN @AffectedStudents s ON sv.MaSV = s.MaSV;
END;
GO
