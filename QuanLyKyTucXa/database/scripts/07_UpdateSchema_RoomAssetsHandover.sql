-- Bổ sung danh mục tài sản theo phòng và biên bản bàn giao nhận/trả/chuyển phòng.
-- Chạy trên database hiện có; có thể chạy lại, không xóa hoặc sửa dữ liệu nghiệp vụ cũ.
USE [QuanLyKTXDB];
GO

SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'dbo.TaiSanPhong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaiSanPhong (
        MaTaiSan INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TaiSanPhong PRIMARY KEY,
        MaPhong NVARCHAR(20) NOT NULL,
        TenTaiSan NVARCHAR(100) NOT NULL,
        SoLuong INT NOT NULL,
        TinhTrang NVARCHAR(30) NOT NULL CONSTRAINT DF_TaiSanPhong_TinhTrang DEFAULT N'Tốt',
        GhiChu NVARCHAR(255) NULL,
        CONSTRAINT FK_TaiSanPhong_Phong FOREIGN KEY (MaPhong)
            REFERENCES dbo.Phong(MaPhong) ON DELETE CASCADE,
        CONSTRAINT CK_TaiSanPhong_SoLuong CHECK (SoLuong > 0),
        CONSTRAINT CK_TaiSanPhong_TinhTrang CHECK (TinhTrang IN (N'Tốt', N'Hư hỏng', N'Bảo trì'))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.TaiSanPhong') AND name = N'UQ_TaiSanPhong_Phong_Ten')
    CREATE UNIQUE INDEX UQ_TaiSanPhong_Phong_Ten ON dbo.TaiSanPhong(MaPhong, TenTaiSan);
GO

IF OBJECT_ID(N'dbo.BienBanBanGiao', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BienBanBanGiao (
        MaBienBan INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_BienBanBanGiao PRIMARY KEY,
        MaHopDong NVARCHAR(30) NOT NULL,
        MaPhong NVARCHAR(20) NOT NULL,
        MaGiuong NVARCHAR(30) NOT NULL,
        LoaiBienBan NVARCHAR(20) NOT NULL,
        NgayLap DATETIME2 NOT NULL CONSTRAINT DF_BienBanBanGiao_NgayLap DEFAULT SYSDATETIME(),
        NguoiLap NVARCHAR(100) NOT NULL,
        GhiChu NVARCHAR(500) NULL,
        CONSTRAINT FK_BienBanBanGiao_HopDong FOREIGN KEY (MaHopDong)
            REFERENCES dbo.HopDong(MaHopDong),
        CONSTRAINT FK_BienBanBanGiao_Phong FOREIGN KEY (MaPhong)
            REFERENCES dbo.Phong(MaPhong),
        CONSTRAINT FK_BienBanBanGiao_Giuong FOREIGN KEY (MaGiuong)
            REFERENCES dbo.Giuong(MaGiuong),
        CONSTRAINT CK_BienBanBanGiao_Loai CHECK (LoaiBienBan IN (N'Nhận phòng', N'Trả phòng', N'Chuyển phòng'))
    );
END;
GO

IF OBJECT_ID(N'dbo.ChiTietBienBanBanGiao', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChiTietBienBanBanGiao (
        MaChiTiet INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChiTietBienBanBanGiao PRIMARY KEY,
        MaBienBan INT NOT NULL,
        TenTaiSan NVARCHAR(100) NOT NULL,
        SoLuongBanGiao INT NOT NULL,
        SoLuongThucTe INT NOT NULL,
        TinhTrangBanGiao NVARCHAR(30) NOT NULL,
        TinhTrangThucTe NVARCHAR(30) NOT NULL,
        TienBoiThuong DECIMAL(18,2) NOT NULL CONSTRAINT DF_ChiTietBienBan_TienBoiThuong DEFAULT 0,
        GhiChu NVARCHAR(255) NULL,
        CONSTRAINT FK_ChiTietBienBan_BienBan FOREIGN KEY (MaBienBan)
            REFERENCES dbo.BienBanBanGiao(MaBienBan) ON DELETE CASCADE,
        CONSTRAINT CK_ChiTietBienBan_SoLuong CHECK (SoLuongBanGiao >= 0 AND SoLuongThucTe >= 0),
        CONSTRAINT CK_ChiTietBienBan_Tien CHECK (TienBoiThuong >= 0),
        CONSTRAINT CK_ChiTietBienBan_TinhTrangBanGiao CHECK (TinhTrangBanGiao IN (N'Tốt', N'Hư hỏng', N'Mất', N'Bảo trì')),
        CONSTRAINT CK_ChiTietBienBan_TinhTrangThucTe CHECK (TinhTrangThucTe IN (N'Tốt', N'Hư hỏng', N'Mất', N'Bảo trì'))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.BienBanBanGiao') AND name = N'IX_BienBanBanGiao_HopDong_Ngay')
    CREATE INDEX IX_BienBanBanGiao_HopDong_Ngay ON dbo.BienBanBanGiao(MaHopDong, NgayLap DESC);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ChiTietBienBanBanGiao') AND name = N'IX_ChiTietBienBan_MaBienBan')
    CREATE INDEX IX_ChiTietBienBan_MaBienBan ON dbo.ChiTietBienBanBanGiao(MaBienBan);
GO
