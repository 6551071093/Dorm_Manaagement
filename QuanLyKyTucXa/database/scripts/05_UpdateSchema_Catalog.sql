-- CẬP NHẬT BỔ SUNG: KHÔI PHỤC, KHÓA ĐĂNG NHẬP VÀ KHOA/NGÀNH/LỚP.
-- Chạy sau 01/02/03; chạy lại được, không đặt lại mật khẩu, không xóa dữ liệu nghiệp vụ.

GO
USE [QuanLyKTXDB];
GO
-- Chạy trên database KTX đang cấu hình. Không đổi bảng NguoiDung hiện có.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.TaiKhoanKhoiPhuc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaiKhoanKhoiPhuc (
        MaNguoiDung int NOT NULL CONSTRAINT PK_TaiKhoanKhoiPhuc PRIMARY KEY,
        MaKhoiPhucHash char(64) NOT NULL,
        NgayCap datetime2 NOT NULL CONSTRAINT DF_TaiKhoanKhoiPhuc_NgayCap DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_TaiKhoanKhoiPhuc_NguoiDung FOREIGN KEY (MaNguoiDung) REFERENCES dbo.NguoiDung(MaNguoiDung)
    );
END;
COMMIT TRANSACTION;

GO
-- Bộ đếm đăng nhập sai được lưu riêng để không sửa Entities sinh từ Scaffold.
USE [QuanLyKTXDB];
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.TaiKhoanDangNhap', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaiKhoanDangNhap (
        MaNguoiDung int NOT NULL CONSTRAINT PK_TaiKhoanDangNhap PRIMARY KEY,
        SoLanSai int NOT NULL CONSTRAINT DF_TaiKhoanDangNhap_SoLanSai DEFAULT 0,
        BiKhoa bit NOT NULL CONSTRAINT DF_TaiKhoanDangNhap_BiKhoa DEFAULT 0,
        LanSaiCuoi datetime2 NULL,
        CONSTRAINT CK_TaiKhoanDangNhap_SoLanSai CHECK (SoLanSai BETWEEN 0 AND 5),
        CONSTRAINT FK_TaiKhoanDangNhap_NguoiDung FOREIGN KEY (MaNguoiDung) REFERENCES dbo.NguoiDung(MaNguoiDung)
    );
END;
COMMIT TRANSACTION;
GO

GO
-- Nguồn tên khoa: https://utc2.edu.vn/gioi-thieu/cac-khoa-bo-mon
-- Nguồn tên ngành: https://utc2.edu.vn/chuong-trinh-dao-tao
-- Tham khảo ngày 23/10/2026. Phân khoa/ngành phục vụ đồ án.
-- Mã lớp K64-K67 bên dưới là MÔ PHỎNG, không phải mã lớp chính thức UTC2.
-- Không thêm sinh viên và không sửa khoa/lớp của hồ sơ đã có.
USE [QuanLyKTXDB];
GO
SET XACT_ABORT ON;
BEGIN TRY
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.DanhMucKhoa',N'U') IS NULL
 CREATE TABLE dbo.DanhMucKhoa(MaKhoa nvarchar(20) NOT NULL PRIMARY KEY,TenKhoa nvarchar(100) NOT NULL UNIQUE,Nguon nvarchar(300) NOT NULL);
IF OBJECT_ID(N'dbo.DanhMucNganh',N'U') IS NULL
 CREATE TABLE dbo.DanhMucNganh(MaNganh nvarchar(20) NOT NULL PRIMARY KEY,TenNganh nvarchar(150) NOT NULL,MaKhoa nvarchar(20) NOT NULL REFERENCES dbo.DanhMucKhoa(MaKhoa));
IF OBJECT_ID(N'dbo.DanhMucLop',N'U') IS NULL
 CREATE TABLE dbo.DanhMucLop(MaLop nvarchar(50) NOT NULL PRIMARY KEY,MaNganh nvarchar(20) NOT NULL REFERENCES dbo.DanhMucNganh(MaNganh),KhoaHoc int NOT NULL,LaLopMoPhong bit NOT NULL DEFAULT 1);
INSERT dbo.DanhMucKhoa(MaKhoa,TenKhoa,Nguon)
SELECT v.MaKhoa,v.TenKhoa,N'https://utc2.edu.vn/gioi-thieu/cac-khoa-bo-mon'
FROM (VALUES(N'CT',N'Công trình'),(N'KTVT',N'Kinh tế - Vận tải'),(N'KTCN',N'Kỹ thuật Công nghệ'),(N'KHCB',N'Khoa học cơ bản')) v(MaKhoa,TenKhoa)
WHERE NOT EXISTS(SELECT 1 FROM dbo.DanhMucKhoa k WHERE k.MaKhoa=v.MaKhoa);
DECLARE @Nganh TABLE(Ma nvarchar(20),Ten nvarchar(150),Khoa nvarchar(20));
INSERT @Nganh(Ma,Ten,Khoa) VALUES
(N'KTXD',N'Kinh tế xây dựng',N'KTVT'),
(N'KTXDCT',N'Kỹ thuật xây dựng',N'CT'),
(N'XDGT',N'Kỹ thuật xây dựng công trình giao thông',N'CT'),
(N'KTRUC',N'Kiến trúc',N'CT'),
(N'QLXD',N'Quản lý xây dựng',N'CT'),
(N'KTVT',N'Khai thác vận tải',N'KTVT'),
(N'QTKD',N'Quản trị kinh doanh',N'KTVT'),
(N'KTOAN',N'Kế toán',N'KTVT'),
(N'TCNH',N'Tài chính - Ngân hàng',N'KTVT'),
(N'QTDL',N'Quản trị dịch vụ du lịch và lữ hành',N'KTVT'),
(N'LOG',N'Logistics và quản lý chuỗi cung ứng',N'KTVT'),
(N'CNTT',N'Công nghệ thông tin',N'KTCN'),
(N'DTVT',N'Kỹ thuật điện tử - viễn thông',N'KTCN'),
(N'CDT',N'Kỹ thuật cơ điện tử',N'KTCN'),
(N'CKDL',N'Kỹ thuật cơ khí động lực',N'KTCN'),
(N'OTO',N'Kỹ thuật ô tô',N'KTCN'),
(N'DIEN',N'Kỹ thuật điện',N'KTCN'),
(N'TDH',N'Kỹ thuật điều khiển và tự động hóa',N'KTCN')
;
INSERT dbo.DanhMucNganh(MaNganh,TenNganh,MaKhoa)
SELECT n.Ma,n.Ten,n.Khoa FROM @Nganh n WHERE NOT EXISTS(SELECT 1 FROM dbo.DanhMucNganh x WHERE x.MaNganh=n.Ma);
INSERT dbo.DanhMucLop(MaLop,MaNganh,KhoaHoc,LaLopMoPhong)
SELECT CONCAT(n.Ma,s.So,N'-K',k.Khoa),n.Ma,k.Khoa,1
FROM @Nganh n CROSS JOIN (VALUES(64),(65),(66),(67)) k(Khoa) CROSS JOIN(VALUES(1),(2))s(So)
WHERE NOT EXISTS(SELECT 1 FROM dbo.DanhMucLop l WHERE l.MaLop=CONCAT(n.Ma,s.So,N'-K',k.Khoa));
COMMIT TRANSACTION;
END TRY
BEGIN CATCH
 IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
GO
