-- ============================================================================
-- DỰ ÁN: QUẢN LÝ KÝ TÚC XÁ (QuanLyKyTucXa)
-- HỌC PHẦN: LẬP TRÌNH TRỰC QUAN (LTTQ) - .NET 9 & C# 12/13
-- FILE: 01_CreateDatabase_Tables.sql
-- MỤC ĐÍCH: Tạo Database QuanLyKTXDB và 13 bảng thực thể chuẩn hóa (Database-First)
-- KIẾN TRÚC: Hóa đơn cá nhân sinh viên + Chia sẻ tiền điện nước phòng (Phương án A)
-- ============================================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyKTXDB')
BEGIN
    CREATE DATABASE QuanLyKTXDB;
END
GO

USE QuanLyKTXDB;
GO

-- XÓA BẢNG CŨ THEO THỨ TỰ PHỤ THUỘC (NẾU CÓ)
IF OBJECT_ID('dbo.ChiTietHoaDon', 'U') IS NOT NULL DROP TABLE dbo.ChiTietHoaDon;
IF OBJECT_ID('dbo.ThanhToan', 'U') IS NOT NULL DROP TABLE dbo.ThanhToan;
IF OBJECT_ID('dbo.HoaDon', 'U') IS NOT NULL DROP TABLE dbo.HoaDon;
IF OBJECT_ID('dbo.ChiSoDienNuoc', 'U') IS NOT NULL DROP TABLE dbo.ChiSoDienNuoc;
IF OBJECT_ID('dbo.BangGia', 'U') IS NOT NULL DROP TABLE dbo.BangGia;
IF OBJECT_ID('dbo.SuCo', 'U') IS NOT NULL DROP TABLE dbo.SuCo;
IF OBJECT_ID('dbo.HopDong', 'U') IS NOT NULL DROP TABLE dbo.HopDong;
IF OBJECT_ID('dbo.DangKyKtx', 'U') IS NOT NULL DROP TABLE dbo.DangKyKtx;
IF OBJECT_ID('dbo.Giuong', 'U') IS NOT NULL DROP TABLE dbo.Giuong;
IF OBJECT_ID('dbo.Phong', 'U') IS NOT NULL DROP TABLE dbo.Phong;
IF OBJECT_ID('dbo.ToaNha', 'U') IS NOT NULL DROP TABLE dbo.ToaNha;
IF OBJECT_ID('dbo.SinhVien', 'U') IS NOT NULL DROP TABLE dbo.SinhVien;
IF OBJECT_ID('dbo.NguoiDung', 'U') IS NOT NULL DROP TABLE dbo.NguoiDung;
GO

-- ============================================================================
-- 1. BẢNG NguoiDung: Tài khoản đăng nhập hệ thống
-- ============================================================================
CREATE TABLE dbo.NguoiDung (
    MaNguoiDung INT IDENTITY(1,1) NOT NULL,
    TenDangNhap NVARCHAR(50) NOT NULL,
    MatKhauHash NVARCHAR(255) NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NULL,
    SoDienThoai NVARCHAR(20) NULL,
    VaiTro NVARCHAR(20) NOT NULL DEFAULT N'NhanVien',
    TrangThai BIT NOT NULL DEFAULT 1,                  -- 1: Hoạt động, 0: Khóa
    NgayTao DATETIME2 NOT NULL DEFAULT GETDATE(),
    LanDangNhapCuoi DATETIME2 NULL,
    CONSTRAINT PK_NguoiDung PRIMARY KEY CLUSTERED (MaNguoiDung),
    CONSTRAINT UQ_NguoiDung_TenDangNhap UNIQUE (TenDangNhap),
    CONSTRAINT CK_NguoiDung_VaiTro CHECK (VaiTro IN (N'Admin', N'NhanVien', N'SinhVien'))
);
GO

-- ============================================================================
-- 2. BẢNG SinhVien: Hồ sơ sinh viên nội trú
-- ============================================================================
CREATE TABLE dbo.SinhVien (
    MaSV NVARCHAR(20) NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE NOT NULL,
    GioiTinh NVARCHAR(10) NOT NULL,                    -- Nam, Nữ
    CCCD NVARCHAR(20) NOT NULL,
    SoDienThoai NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NULL,
    Lop NVARCHAR(50) NOT NULL,
    Khoa NVARCHAR(100) NOT NULL,
    QueQuan NVARCHAR(200) NULL,
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Chờ xếp phòng', -- Đang ở, Đã rời KTX, Chờ xếp phòng
    MaNguoiDung INT NULL,
    CONSTRAINT PK_SinhVien PRIMARY KEY CLUSTERED (MaSV),
    CONSTRAINT UQ_SinhVien_CCCD UNIQUE (CCCD),
    CONSTRAINT FK_SinhVien_NguoiDung FOREIGN KEY (MaNguoiDung) 
        REFERENCES dbo.NguoiDung (MaNguoiDung) ON DELETE SET NULL,
    CONSTRAINT CK_SinhVien_GioiTinh CHECK (GioiTinh IN (N'Nam', N'Nữ')),
    CONSTRAINT CK_SinhVien_TrangThai CHECK (TrangThai IN (N'Đang ở', N'Đã rời KTX', N'Chờ xếp phòng'))
);
GO

-- ============================================================================
-- 3. BẢNG ToaNha: Danh mục khối nhà ký túc xá
-- ============================================================================
CREATE TABLE dbo.ToaNha (
    MaToa NVARCHAR(10) NOT NULL,                       -- A1, B2, C1...
    TenToa NVARCHAR(100) NOT NULL,
    LoaiToa NVARCHAR(20) NOT NULL DEFAULT N'Hỗn hợp',  -- Nam, Nữ, Hỗn hợp
    SoTang INT NOT NULL DEFAULT 5,
    SoPhong INT NOT NULL DEFAULT 0,
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Hoạt động', -- Hoạt động, Bảo trì, Dừng hoạt động
    GhiChu NVARCHAR(255) NULL,
    CONSTRAINT PK_ToaNha PRIMARY KEY CLUSTERED (MaToa),
    CONSTRAINT CK_ToaNha_LoaiToa CHECK (LoaiToa IN (N'Nam', N'Nữ', N'Hỗn hợp')),
    CONSTRAINT CK_ToaNha_SoTang CHECK (SoTang > 0),
    CONSTRAINT CK_ToaNha_SoPhong CHECK (SoPhong >= 0),
    CONSTRAINT CK_ToaNha_TrangThai CHECK (TrangThai IN (N'Hoạt động', N'Bảo trì', N'Dừng hoạt động'))
);
GO

-- ============================================================================
-- 4. BẢNG Phong: Danh mục phòng ở
-- ============================================================================
CREATE TABLE dbo.Phong (
    MaPhong NVARCHAR(20) NOT NULL,                     -- A1-101, B2-205...
    MaToa NVARCHAR(10) NOT NULL,
    TenPhong NVARCHAR(50) NOT NULL,
    Tang INT NOT NULL DEFAULT 1,
    LoaiPhong NVARCHAR(50) NOT NULL DEFAULT N'Phòng 4 người',
    SoChoToiDa INT NOT NULL DEFAULT 4,
    SoNguoiHienTai INT NOT NULL DEFAULT 0,
    GiaPhong DECIMAL(18,2) NOT NULL DEFAULT 500000,
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Còn chỗ', -- Còn chỗ, Đã đầy, Đang sửa chữa
    GhiChu NVARCHAR(255) NULL,
    CONSTRAINT PK_Phong PRIMARY KEY CLUSTERED (MaPhong),
    CONSTRAINT FK_Phong_ToaNha FOREIGN KEY (MaToa) 
        REFERENCES dbo.ToaNha (MaToa) ON DELETE CASCADE,
    CONSTRAINT CK_Phong_Tang CHECK (Tang > 0),
    CONSTRAINT CK_Phong_SoChoToiDa CHECK (SoChoToiDa > 0),
    CONSTRAINT CK_Phong_SoNguoiHienTai CHECK (SoNguoiHienTai >= 0 AND SoNguoiHienTai <= SoChoToiDa),
    CONSTRAINT CK_Phong_GiaPhong CHECK (GiaPhong >= 0),
    CONSTRAINT CK_Phong_TrangThai CHECK (TrangThai IN (N'Còn chỗ', N'Đã đầy', N'Đang sửa chữa'))
);
GO

-- ============================================================================
-- 5. BẢNG Giuong: Chi tiết vị trí giường trong phòng
-- ============================================================================
CREATE TABLE dbo.Giuong (
    MaGiuong NVARCHAR(30) NOT NULL,                    -- A1-101-G1...
    MaPhong NVARCHAR(20) NOT NULL,
    TenGiuong NVARCHAR(50) NOT NULL,                   -- Giường 1, Giường 2...
    ViTri NVARCHAR(30) NULL,                           -- Tầng dưới, Tầng trên
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Trống',  -- Trống, Đang sử dụng, Hỏng, Bảo trì
    CONSTRAINT PK_Giuong PRIMARY KEY CLUSTERED (MaGiuong),
    CONSTRAINT FK_Giuong_Phong FOREIGN KEY (MaPhong) 
        REFERENCES dbo.Phong (MaPhong) ON DELETE CASCADE,
    CONSTRAINT CK_Giuong_TrangThai CHECK (TrangThai IN (N'Trống', N'Đang sử dụng', N'Hỏng', N'Bảo trì'))
);
GO

-- ============================================================================
-- 6. BẢNG DangKyKtx: Đơn tiếp nhận sinh viên đăng ký ở KTX
-- ============================================================================
CREATE TABLE dbo.DangKyKtx (
    MaDangKy INT IDENTITY(1,1) NOT NULL,
    MaSV NVARCHAR(20) NOT NULL,
    MaPhongDeXuat NVARCHAR(20) NULL,
    HocKy INT NOT NULL DEFAULT 1,
    NamHoc NVARCHAR(20) NOT NULL,                      -- VD: 2026-2027
    NgayDangKy DATETIME2 NOT NULL DEFAULT GETDATE(),
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Chờ duyệt', -- Chờ duyệt, Đã duyệt, Từ chối, Đã hủy
    NguoiDuyet INT NULL,
    NgayDuyet DATETIME2 NULL,
    GhiChu NVARCHAR(255) NULL,
    CONSTRAINT PK_DangKyKtx PRIMARY KEY CLUSTERED (MaDangKy),
    CONSTRAINT FK_DangKyKtx_SinhVien FOREIGN KEY (MaSV) 
        REFERENCES dbo.SinhVien (MaSV) ON DELETE CASCADE,
    CONSTRAINT FK_DangKyKtx_Phong FOREIGN KEY (MaPhongDeXuat) 
        REFERENCES dbo.Phong (MaPhong) ON DELETE SET NULL,
    CONSTRAINT FK_DangKyKtx_NguoiDuyet FOREIGN KEY (NguoiDuyet) 
        REFERENCES dbo.NguoiDung (MaNguoiDung) ON DELETE NO ACTION,
    CONSTRAINT CK_DangKyKtx_HocKy CHECK (HocKy IN (1, 2, 3)),
    CONSTRAINT CK_DangKyKtx_TrangThai CHECK (TrangThai IN (N'Chờ duyệt', N'Đã duyệt', N'Từ chối', N'Đã hủy'))
);
GO

-- ============================================================================
-- 7. BẢNG HopDong: Hợp đồng lưu trú ký kết giữa KTX và sinh viên
-- ============================================================================
CREATE TABLE dbo.HopDong (
    MaHopDong NVARCHAR(30) NOT NULL,                   -- HD-2026-001...
    MaSV NVARCHAR(20) NOT NULL,
    MaGiuong NVARCHAR(30) NOT NULL,
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    TienCoc DECIMAL(18,2) NOT NULL DEFAULT 500000,
    TienPhongThang DECIMAL(18,2) NOT NULL,
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Hiệu lực', -- Hiệu lực, Đã thanh lý, Hết hạn, Đã hủy
    NgayTao DATETIME2 NOT NULL DEFAULT GETDATE(),
    GhiChu NVARCHAR(255) NULL,
    CONSTRAINT PK_HopDong PRIMARY KEY CLUSTERED (MaHopDong),
    CONSTRAINT FK_HopDong_SinhVien FOREIGN KEY (MaSV) 
        REFERENCES dbo.SinhVien (MaSV) ON DELETE CASCADE,
    CONSTRAINT FK_HopDong_Giuong FOREIGN KEY (MaGiuong) 
        REFERENCES dbo.Giuong (MaGiuong) ON DELETE NO ACTION,
    CONSTRAINT CK_HopDong_Ngay CHECK (NgayKetThuc > NgayBatDau),
    CONSTRAINT CK_HopDong_TienCoc CHECK (TienCoc >= 0),
    CONSTRAINT CK_HopDong_TienPhongThang CHECK (TienPhongThang >= 0),
    CONSTRAINT CK_HopDong_TrangThai CHECK (TrangThai IN (N'Hiệu lực', N'Đã thanh lý', N'Hết hạn', N'Đã hủy'))
);
GO

-- ============================================================================
-- 8. BẢNG BangGia: Biểu phí dịch vụ định mức (Điện, Nước, Xe, Vệ sinh...)
-- ============================================================================
CREATE TABLE dbo.BangGia (
    MaBangGia INT IDENTITY(1,1) NOT NULL,
    TenDichVu NVARCHAR(100) NOT NULL,                  -- Tiền điện, Tiền nước, Internet...
    DonViTinh NVARCHAR(30) NOT NULL,                   -- kWh, m3, Tháng...
    DonGia DECIMAL(18,2) NOT NULL,
    NgayApDung DATE NOT NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_BangGia PRIMARY KEY CLUSTERED (MaBangGia),
    CONSTRAINT CK_BangGia_DonGia CHECK (DonGia >= 0)
);
GO

-- ============================================================================
-- 9. BẢNG ChiSoDienNuoc: Chốt chỉ số công tơ điện nước theo phòng hàng tháng
-- ============================================================================
CREATE TABLE dbo.ChiSoDienNuoc (
    MaChiSo INT IDENTITY(1,1) NOT NULL,
    MaPhong NVARCHAR(20) NOT NULL,
    Thang INT NOT NULL,
    Nam INT NOT NULL,
    ChiSoDienDau INT NOT NULL DEFAULT 0,
    ChiSoDienCuoi INT NOT NULL DEFAULT 0,
    ChiSoNuocDau INT NOT NULL DEFAULT 0,
    ChiSoNuocCuoi INT NOT NULL DEFAULT 0,
    NgayGhi DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    NguoiGhi NVARCHAR(100) NULL,
    CONSTRAINT PK_ChiSoDienNuoc PRIMARY KEY CLUSTERED (MaChiSo),
    CONSTRAINT FK_ChiSoDienNuoc_Phong FOREIGN KEY (MaPhong) 
        REFERENCES dbo.Phong (MaPhong) ON DELETE CASCADE,
    CONSTRAINT UQ_ChiSoDienNuoc_Phong_Thang_Nam UNIQUE (MaPhong, Thang, Nam),
    CONSTRAINT CK_ChiSoDienNuoc_Thang CHECK (Thang BETWEEN 1 AND 12),
    CONSTRAINT CK_ChiSoDienNuoc_Nam CHECK (Nam >= 2020),
    CONSTRAINT CK_ChiSoDienNuoc_Dien CHECK (ChiSoDienDau >= 0 AND ChiSoDienCuoi >= ChiSoDienDau),
    CONSTRAINT CK_ChiSoDienNuoc_Nuoc CHECK (ChiSoNuocDau >= 0 AND ChiSoNuocCuoi >= ChiSoNuocDau)
);
GO

-- ============================================================================
-- 10. BẢNG HoaDon: Hóa đơn cá nhân sinh viên hàng tháng (Phương án A)
-- Gồm: Tiền phòng cá nhân + Phần bổ đều tiền điện/nước của phòng
-- ============================================================================
CREATE TABLE dbo.HoaDon (
    MaHoaDon NVARCHAR(30) NOT NULL,                    -- HD-202610-SV01...
    MaHopDong NVARCHAR(30) NOT NULL,                   -- Hóa đơn gắn chặt với hợp đồng sinh viên
    MaSV NVARCHAR(20) NOT NULL,                        -- Sinh viên chịu trách nhiệm thanh toán
    MaPhong NVARCHAR(20) NOT NULL,                     -- Phòng sinh viên đang lưu trú tại kỳ hóa đơn
    Thang INT NOT NULL,
    Nam INT NOT NULL,
    TongTien DECIMAL(18,2) NOT NULL DEFAULT 0,
    HanThanhToan DATE NOT NULL,
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Chưa thanh toán', -- Chưa thanh toán, Đã thanh toán, Quá hạn, Hủy
    NgayLap DATETIME2 NOT NULL DEFAULT GETDATE(),
    GhiChu NVARCHAR(255) NULL,
    CONSTRAINT PK_HoaDon PRIMARY KEY CLUSTERED (MaHoaDon),
    CONSTRAINT FK_HoaDon_HopDong FOREIGN KEY (MaHopDong) 
        REFERENCES dbo.HopDong (MaHopDong) ON DELETE NO ACTION,
    CONSTRAINT FK_HoaDon_SinhVien FOREIGN KEY (MaSV) 
        REFERENCES dbo.SinhVien (MaSV) ON DELETE NO ACTION,
    CONSTRAINT FK_HoaDon_Phong FOREIGN KEY (MaPhong) 
        REFERENCES dbo.Phong (MaPhong) ON DELETE NO ACTION,
    CONSTRAINT UQ_HoaDon_HopDong_Thang_Nam UNIQUE (MaHopDong, Thang, Nam),
    CONSTRAINT CK_HoaDon_Thang CHECK (Thang BETWEEN 1 AND 12),
    CONSTRAINT CK_HoaDon_Nam CHECK (Nam >= 2020),
    CONSTRAINT CK_HoaDon_TongTien CHECK (TongTien >= 0),
    CONSTRAINT CK_HoaDon_TrangThai CHECK (TrangThai IN (N'Chưa thanh toán', N'Đã thanh toán', N'Quá hạn', N'Hủy'))
);
GO

-- ============================================================================
-- 11. BẢNG ChiTietHoaDon: Phân rã mục thu trong hóa đơn cá nhân
-- (VD: Tiền phòng, Tiền điện phòng bổ đầu người, Tiền nước bổ đầu người...)
-- ============================================================================
CREATE TABLE dbo.ChiTietHoaDon (
    MaChiTiet INT IDENTITY(1,1) NOT NULL,
    MaHoaDon NVARCHAR(30) NOT NULL,
    LoaiTien NVARCHAR(50) NOT NULL,                    -- Tiền phòng, Tiền điện, Tiền nước, Phí gửi xe...
    SoLuong DECIMAL(10,2) NOT NULL DEFAULT 1,
    DonGia DECIMAL(18,2) NOT NULL DEFAULT 0,
    ThanhTien DECIMAL(18,2) NOT NULL DEFAULT 0,
    GhiChu NVARCHAR(200) NULL,
    CONSTRAINT PK_ChiTietHoaDon PRIMARY KEY CLUSTERED (MaChiTiet),
    CONSTRAINT FK_ChiTietHoaDon_HoaDon FOREIGN KEY (MaHoaDon) 
        REFERENCES dbo.HoaDon (MaHoaDon) ON DELETE CASCADE,
    CONSTRAINT CK_ChiTietHoaDon_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CK_ChiTietHoaDon_DonGia CHECK (DonGia >= 0),
    CONSTRAINT CK_ChiTietHoaDon_ThanhTien CHECK (ThanhTien >= 0)
);
GO

-- ============================================================================
-- 12. BẢNG ThanhToan: Lịch sử giao dịch đóng tiền (Tiền mặt / VietQR)
-- ============================================================================
CREATE TABLE dbo.ThanhToan (
    MaThanhToan INT IDENTITY(1,1) NOT NULL,
    MaHoaDon NVARCHAR(30) NOT NULL,
    NgayThanhToan DATETIME2 NOT NULL DEFAULT GETDATE(),
    SoTien DECIMAL(18,2) NOT NULL,
    PhuongThuc NVARCHAR(50) NOT NULL DEFAULT N'Chuyển khoản VietQR', -- Tiền mặt, Chuyển khoản VietQR, Chuyển khoản ngân hàng
    MaGiaoDich NVARCHAR(100) NULL,
    NguoiThu NVARCHAR(100) NULL,
    GhiChu NVARCHAR(255) NULL,
    CONSTRAINT PK_ThanhToan PRIMARY KEY CLUSTERED (MaThanhToan),
    CONSTRAINT FK_ThanhToan_HoaDon FOREIGN KEY (MaHoaDon) 
        REFERENCES dbo.HoaDon (MaHoaDon) ON DELETE CASCADE,
    CONSTRAINT CK_ThanhToan_SoTien CHECK (SoTien > 0),
    CONSTRAINT CK_ThanhToan_PhuongThuc CHECK (PhuongThuc IN (N'Tiền mặt', N'Chuyển khoản VietQR', N'Chuyển khoản ngân hàng'))
);
GO

-- ============================================================================
-- 13. BẢNG SuCo: Tiếp nhận & xử lý phản ánh cơ sở vật chất
-- ============================================================================
CREATE TABLE dbo.SuCo (
    MaSuCo INT IDENTITY(1,1) NOT NULL,
    MaPhong NVARCHAR(20) NOT NULL,
    MaSV NVARCHAR(20) NULL,
    TieuDe NVARCHAR(200) NOT NULL,
    MoTa NVARCHAR(MAX) NOT NULL,
    MucDoUuTien NVARCHAR(20) NOT NULL DEFAULT N'Bình thường', -- Thấp, Bình thường, Khẩn cấp
    NgayBao DATETIME2 NOT NULL DEFAULT GETDATE(),
    NgayXuLy DATETIME2 NULL,
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Mới tiếp nhận', -- Mới tiếp nhận, Đang xử lý, Đã giải quyết, Hủy
    NguoiXuLy NVARCHAR(100) NULL,
    ChiPhiSuaChua DECIMAL(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT PK_SuCo PRIMARY KEY CLUSTERED (MaSuCo),
    CONSTRAINT FK_SuCo_Phong FOREIGN KEY (MaPhong) 
        REFERENCES dbo.Phong (MaPhong) ON DELETE CASCADE,
    CONSTRAINT FK_SuCo_SinhVien FOREIGN KEY (MaSV) 
        REFERENCES dbo.SinhVien (MaSV) ON DELETE SET NULL,
    CONSTRAINT CK_SuCo_MucDoUuTien CHECK (MucDoUuTien IN (N'Thấp', N'Bình thường', N'Khẩn cấp')),
    CONSTRAINT CK_SuCo_TrangThai CHECK (TrangThai IN (N'Mới tiếp nhận', N'Đang xử lý', N'Đã giải quyết', N'Hủy')),
    CONSTRAINT CK_SuCo_ChiPhi CHECK (ChiPhiSuaChua >= 0)
);
GO
