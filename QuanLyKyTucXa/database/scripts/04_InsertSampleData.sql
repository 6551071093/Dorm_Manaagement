-- ============================================================================
-- DỰ ÁN: QUẢN LÝ KÝ TÚC XÁ (QuanLyKyTucXa)
-- HỌC PHẦN: LẬP TRÌNH TRỰC QUAN (LTTQ) - .NET 9 & C# 12/13
-- FILE: 04_InsertSampleData.sql
-- MỤC ĐÍCH: Nạp dữ liệu mẫu phong phú và chuẩn hóa để kiểm thử hệ thống
-- ============================================================================

USE QuanLyKTXDB;
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- CẢNH BÁO: FILE NÀY NẠP LẠI DỮ LIỆU MẪU VÀ XÓA DỮ LIỆU CŨ.
-- Chỉ tạo/reset Admin: dùng 06_ResetDemoAccounts.sql.
-- XÓA DỮ LIỆU CŨ THEO THỨ TỰ RÀNG BUỘC
IF OBJECT_ID(N'dbo.TaiKhoanKhoiPhuc', N'U') IS NOT NULL
    EXEC(N'DELETE FROM dbo.TaiKhoanKhoiPhuc');
DELETE FROM dbo.ChiTietHoaDon;
DELETE FROM dbo.ThanhToan;
DELETE FROM dbo.HoaDon;
DELETE FROM dbo.ChiSoDienNuoc;
DELETE FROM dbo.BangGia;
DELETE FROM dbo.SuCo;
DELETE FROM dbo.HopDong;
DELETE FROM dbo.DangKyKtx;
DELETE FROM dbo.Giuong;
DELETE FROM dbo.Phong;
DELETE FROM dbo.ToaNha;
DELETE FROM dbo.SinhVien;
IF OBJECT_ID(N'dbo.TaiKhoanDangNhap', N'U') IS NOT NULL
    EXEC(N'DELETE FROM dbo.TaiKhoanDangNhap');
DELETE FROM dbo.NguoiDung;
GO

-- 1. TÀI KHOẢN NGƯỜI DÙNG (NguoiDung)
-- Admin: Admin@123; Nhân viên: NhanVien@123; Sinh viên: SinhVien@123 (PBKDF2-SHA256).
SET IDENTITY_INSERT dbo.NguoiDung ON;
INSERT INTO dbo.NguoiDung (MaNguoiDung, TenDangNhap, MatKhauHash, HoTen, Email, SoDienThoai, VaiTro, TrangThai)
VALUES 
(1, N'admin', N'PBKDF2-SHA256$600000$gltA27sVEvlyhPpy64Hi8w==$8bczWZFRkePj1TjvhwETvGrLTzToYSYxjOlYJr3PcWk=', N'Quản trị viên Hệ thống', N'admin@ktx.edu.vn', N'0901234567', N'Admin', 1),
(2, N'nhanvien01', N'PBKDF2-SHA256$600000$hS/mw0kVued1j+WKngc8gQ==$fheghY73BSNeN8tWyd43oLnfV4ZldZArrWOcPG/UiNc=', N'Nguyễn Văn Quản Lý', N'quanly@ktx.edu.vn', N'0912345678', N'NhanVien', 1),
(3, N'sv2026001', N'PBKDF2-SHA256$600000$FNBbltqSiSFnr1c/bJcRSg==$fcDxXG0hwj2LkbBTbOVvPRRSExC+JNQiYvkJ1iJFnXg=', N'Nguyễn Văn An', N'an.nv@sinhvien.edu.vn', N'0987654321', N'SinhVien', 1),
(4, N'sv2026002', N'PBKDF2-SHA256$600000$FNBbltqSiSFnr1c/bJcRSg==$fcDxXG0hwj2LkbBTbOVvPRRSExC+JNQiYvkJ1iJFnXg=', N'Trần Thị Bình', N'binh.tt@sinhvien.edu.vn', N'0987654322', N'SinhVien', 1),
(5, N'sv2026003', N'PBKDF2-SHA256$600000$FNBbltqSiSFnr1c/bJcRSg==$fcDxXG0hwj2LkbBTbOVvPRRSExC+JNQiYvkJ1iJFnXg=', N'Lê Hoàng Nam', N'nam.lh@sinhvien.edu.vn', N'0987654323', N'SinhVien', 1);
SET IDENTITY_INSERT dbo.NguoiDung OFF;
GO

-- 2. HỒ SƠ SINH VIÊN (SinhVien)
INSERT INTO dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, CCCD, SoDienThoai, Email, Lop, Khoa, QueQuan, TrangThai, MaNguoiDung)
VALUES 
(N'SV2026001', N'Nguyễn Văn An', '2005-03-15', N'Nam', N'001205012345', N'0987654321', N'an.nv@sinhvien.edu.vn', N'CNTT1-K17', N'Công nghệ thông tin', N'Hà Nội', N'Chờ xếp phòng', 3),
(N'SV2026002', N'Trần Thị Bình', '2005-08-20', N'Nữ', N'001205012346', N'0987654322', N'binh.tt@sinhvien.edu.vn', N'KT1-K17', N'Kinh tế', N'Nam Định', N'Chờ xếp phòng', 4),
(N'SV2026003', N'Lê Hoàng Nam', '2005-11-05', N'Nam', N'001205012347', N'0987654323', N'nam.lh@sinhvien.edu.vn', N'KTPM2-K17', N'Công nghệ thông tin', N'Hải Phòng', N'Chờ xếp phòng', 5),
(N'SV2026004', N'Phạm Minh Đức', '2005-01-12', N'Nam', N'001205012348', N'0987654324', N'duc.pm@sinhvien.edu.vn', N'HTTT1-K17', N'Công nghệ thông tin', N'Bắc Ninh', N'Chờ xếp phòng', NULL),
(N'SV2026005', N'Vũ Thu Trang', '2005-05-25', N'Nữ', N'001205012349', N'0987654325', N'trang.vt@sinhvien.edu.vn', N'MKT2-K17', N'Quản trị kinh doanh', N'Thái Bình', N'Chờ xếp phòng', NULL),
(N'SV2026006', N'Đặng Gia Huy', '2006-09-18', N'Nam', N'001206012350', N'0987654326', N'huy.dg@sinhvien.edu.vn', N'KHMT1-K18', N'Công nghệ thông tin', N'Quảng Ninh', N'Chờ xếp phòng', NULL);
GO

-- 3. TÒA NHÀ (ToaNha)
INSERT INTO dbo.ToaNha (MaToa, TenToa, LoaiToa, SoTang, SoPhong, TrangThai, GhiChu)
VALUES 
(N'A1', N'Tòa nhà A1 - Nam', N'Nam', 5, 20, N'Hoạt động', N'Dành cho sinh viên nam khối kỹ thuật'),
(N'B2', N'Tòa nhà B2 - Nữ', N'Nữ', 5, 20, N'Hoạt động', N'Dành cho sinh viên nữ các khoa'),
(N'C1', N'Tòa nhà C1 - Chất lượng cao', N'Hỗn hợp', 4, 16, N'Hoạt động', N'Phòng điều hòa khép kín');
GO

-- 4. PHÒNG Ở (Phong)
INSERT INTO dbo.Phong (MaPhong, MaToa, TenPhong, Tang, LoaiPhong, SoChoToiDa, SoNguoiHienTai, GiaPhong, TrangThai)
VALUES 
(N'A1-101', N'A1', N'Phòng 101', 1, N'Phòng 4 người', 4, 0, 600000, N'Còn chỗ'),
(N'A1-102', N'A1', N'Phòng 102', 1, N'Phòng 4 người', 4, 0, 600000, N'Còn chỗ'),
(N'A1-201', N'A1', N'Phòng 201', 2, N'Phòng 6 người', 6, 0, 450000, N'Còn chỗ'),
(N'B2-101', N'B2', N'Phòng 101', 1, N'Phòng 4 người', 4, 0, 600000, N'Còn chỗ'),
(N'B2-102', N'B2', N'Phòng 102', 1, N'Phòng 4 người', 4, 0, 600000, N'Còn chỗ');
GO

-- 5. GIƯỜNG Ở (Giuong)
INSERT INTO dbo.Giuong (MaGiuong, MaPhong, TenGiuong, ViTri, TrangThai)
VALUES 
-- Phòng A1-101
(N'A1-101-G1', N'A1-101', N'Giường 1 (Tầng dưới)', N'Tầng dưới', N'Trống'),
(N'A1-101-G2', N'A1-101', N'Giường 2 (Tầng trên)', N'Tầng trên', N'Trống'),
(N'A1-101-G3', N'A1-101', N'Giường 3 (Tầng dưới)', N'Tầng dưới', N'Trống'),
(N'A1-101-G4', N'A1-101', N'Giường 4 (Tầng trên)', N'Tầng trên', N'Trống'),
-- Phòng A1-102
(N'A1-102-G1', N'A1-102', N'Giường 1', N'Tầng dưới', N'Trống'),
(N'A1-102-G2', N'A1-102', N'Giường 2', N'Tầng trên', N'Trống'),
(N'A1-102-G3', N'A1-102', N'Giường 3', N'Tầng dưới', N'Trống'),
(N'A1-102-G4', N'A1-102', N'Giường 4', N'Tầng trên', N'Trống'),
-- Phòng B2-101
(N'B2-101-G1', N'B2-101', N'Giường 1', N'Tầng dưới', N'Trống'),
(N'B2-101-G2', N'B2-101', N'Giường 2', N'Tầng trên', N'Trống'),
(N'B2-101-G3', N'B2-101', N'Giường 3', N'Tầng dưới', N'Trống'),
(N'B2-101-G4', N'B2-101', N'Giường 4', N'Tầng trên', N'Trống');
GO

-- 6. BẢNG GIÁ DỊCH VỤ (BangGia)
INSERT INTO dbo.BangGia (TenDichVu, DonViTinh, DonGia, NgayApDung, TrangThai)
VALUES 
(N'Tiền điện sinh hoạt', N'kWh', 3500, '2026-01-01', 1),
(N'Tiền nước sinh hoạt', N'm3', 15000, '2026-01-01', 1),
(N'Internet tốc độ cao', N'Tháng', 50000, '2026-01-01', 1),
(N'Phí vệ sinh KTX', N'Tháng', 30000, '2026-01-01', 1),
(N'Phí gửi xe máy', N'Tháng', 80000, '2026-01-01', 1);
GO

-- 7. ĐƠN ĐĂNG KÝ KTX (DangKyKtx)
INSERT INTO dbo.DangKyKtx (MaSV, MaPhongDeXuat, HocKy, NamHoc, TrangThai, NguoiDuyet, NgayDuyet, GhiChu)
VALUES 
(N'SV2026001', N'A1-101', 1, N'2026-2027', N'Đã duyệt', 2, '2026-08-25', N'Ưu tiên diện vùng sâu vùng xa'),
(N'SV2026002', N'B2-101', 1, N'2026-2027', N'Đã duyệt', 2, '2026-08-25', N'Đủ điều kiện xét duyệt'),
(N'SV2026003', N'A1-101', 1, N'2026-2027', N'Đã duyệt', 2, '2026-08-26', N'Đủ điều kiện xét duyệt'),
(N'SV2026004', N'A1-101', 1, N'2026-2027', N'Đã duyệt', 2, '2026-08-26', N'Đủ điều kiện xét duyệt'),
(N'SV2026005', N'B2-101', 1, N'2026-2027', N'Đã duyệt', 2, '2026-08-27', N'Đủ điều kiện xét duyệt'),
(N'SV2026006', N'A1-102', 1, N'2026-2027', N'Chờ duyệt', NULL, NULL, N'Đơn nộp đợt 2');
GO

-- 8. HỢP ĐỒNG LƯU TRÚ (HopDong)
-- Khi lệnh này chạy, Trigger TR_HopDong_DongBoPhongGiuong sẽ tự động:
--  - Chuyển Giường sang 'Đang sử dụng'
--  - Tăng Sĩ số Phòng A1-101 lên 3, B2-101 lên 2
--  - Chuyển Sinh viên sang 'Đang ở'
INSERT INTO dbo.HopDong (MaHopDong, MaSV, MaGiuong, NgayBatDau, NgayKetThuc, TienCoc, TienPhongThang, TrangThai, GhiChu)
VALUES 
(N'HD-2026-001', N'SV2026001', N'A1-101-G1', '2026-09-01', '2027-06-30', 500000, 600000, N'Hiệu lực', N'Hợp đồng năm học 2026-2027'),
(N'HD-2026-002', N'SV2026002', N'B2-101-G1', '2026-09-01', '2027-06-30', 500000, 600000, N'Hiệu lực', N'Hợp đồng năm học 2026-2027'),
(N'HD-2026-003', N'SV2026003', N'A1-101-G2', '2026-09-01', '2027-06-30', 500000, 600000, N'Hiệu lực', N'Hợp đồng năm học 2026-2027'),
(N'HD-2026-004', N'SV2026004', N'A1-101-G3', '2026-09-01', '2027-06-30', 500000, 600000, N'Hiệu lực', N'Hợp đồng năm học 2026-2027'),
(N'HD-2026-005', N'SV2026005', N'B2-101-G2', '2026-09-01', '2027-06-30', 500000, 600000, N'Hiệu lực', N'Hợp đồng năm học 2026-2027');
GO

-- 9. CHỈ SỐ ĐIỆN NƯỚC (ChiSoDienNuoc)
INSERT INTO dbo.ChiSoDienNuoc (MaPhong, Thang, Nam, ChiSoDienDau, ChiSoDienCuoi, ChiSoNuocDau, ChiSoNuocCuoi, NgayGhi, NguoiGhi)
VALUES 
(N'A1-101', 9, 2026, 1200, 1350, 45, 57, '2026-09-30', N'Nguyễn Văn Quản Lý'), -- Dùng 150 kWh, 12 m3
(N'B2-101', 9, 2026, 800, 890, 30, 38, '2026-09-30', N'Nguyễn Văn Quản Lý'),   -- Dùng 90 kWh, 8 m3
(N'A1-101', 10, 2026, 1350, 1530, 57, 72, '2026-10-15', N'Nguyễn Văn Quản Lý'), -- Dùng 180 kWh, 15 m3
(N'B2-101', 10, 2026, 890, 995, 38, 48, '2026-10-15', N'Nguyễn Văn Quản Lý');   -- Dùng 105 kWh, 10 m3
GO

-- 10. HÓA ĐƠN CÁ NHÂN (HoaDon & ChiTietHoaDon)
-- Tháng 9/2026: Đã thanh toán đầy đủ
-- Phòng A1-101 (3 SV): Điện = 150 * 3500 = 525,000 / 3 = 175,000đ; Nước = 12 * 15000 = 180,000 / 3 = 60,000đ.
INSERT INTO dbo.HoaDon (MaHoaDon, MaHopDong, MaSV, MaPhong, Thang, Nam, TongTien, HanThanhToan, TrangThai, GhiChu)
VALUES 
(N'HD-202609-SV2026001', N'HD-2026-001', N'SV2026001', N'A1-101', 9, 2026, 0, '2026-10-10', N'Chưa thanh toán', N'Hóa đơn tháng 9/2026'),
(N'HD-202609-SV2026003', N'HD-2026-003', N'SV2026003', N'A1-101', 9, 2026, 0, '2026-10-10', N'Chưa thanh toán', N'Hóa đơn tháng 9/2026'),
(N'HD-202609-SV2026004', N'HD-2026-004', N'SV2026004', N'A1-101', 9, 2026, 0, '2026-10-10', N'Chưa thanh toán', N'Hóa đơn tháng 9/2026');

-- Chi tiết hóa đơn tháng 9 (Trigger sẽ tự động cộng TongTien = 835,000đ)
INSERT INTO dbo.ChiTietHoaDon (MaHoaDon, LoaiTien, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(N'HD-202609-SV2026001', N'Tiền phòng', 1, 600000, 600000, N'Tiền phòng tháng 9'),
(N'HD-202609-SV2026001', N'Tiền điện', 50, 3500, 175000, N'150 kWh chia 3'),
(N'HD-202609-SV2026001', N'Tiền nước', 4, 15000, 60000, N'12 m3 chia 3'),

(N'HD-202609-SV2026003', N'Tiền phòng', 1, 600000, 600000, N'Tiền phòng tháng 9'),
(N'HD-202609-SV2026003', N'Tiền điện', 50, 3500, 175000, N'150 kWh chia 3'),
(N'HD-202609-SV2026003', N'Tiền nước', 4, 15000, 60000, N'12 m3 chia 3'),

(N'HD-202609-SV2026004', N'Tiền phòng', 1, 600000, 600000, N'Tiền phòng tháng 9'),
(N'HD-202609-SV2026004', N'Tiền điện', 50, 3500, 175000, N'150 kWh chia 3'),
(N'HD-202609-SV2026004', N'Tiền nước', 4, 15000, 60000, N'12 m3 chia 3');

-- Tháng 10/2026: Hóa đơn chưa thanh toán & Quá hạn để kiểm thử báo cáo
INSERT INTO dbo.HoaDon (MaHoaDon, MaHopDong, MaSV, MaPhong, Thang, Nam, TongTien, HanThanhToan, TrangThai, GhiChu)
VALUES 
(N'HD-202610-SV2026001', N'HD-2026-001', N'SV2026001', N'A1-101', 10, 2026, 0, '2026-10-25', N'Chưa thanh toán', N'Hóa đơn tháng 10/2026'),
(N'HD-202610-SV2026003', N'HD-2026-003', N'SV2026003', N'A1-101', 10, 2026, 0, '2026-10-05', N'Quá hạn', N'Hóa đơn quá hạn');

INSERT INTO dbo.ChiTietHoaDon (MaHoaDon, LoaiTien, SoLuong, DonGia, ThanhTien, GhiChu)
VALUES 
(N'HD-202610-SV2026001', N'Tiền phòng', 1, 600000, 600000, N'Tiền phòng tháng 10'),
(N'HD-202610-SV2026001', N'Tiền điện', 60, 3500, 210000, N'180 kWh chia 3'),
(N'HD-202610-SV2026001', N'Tiền nước', 5, 15000, 75000, N'15 m3 chia 3'),

(N'HD-202610-SV2026003', N'Tiền phòng', 1, 600000, 600000, N'Tiền phòng tháng 10'),
(N'HD-202610-SV2026003', N'Tiền điện', 60, 3500, 210000, N'180 kWh chia 3'),
(N'HD-202610-SV2026003', N'Tiền nước', 5, 15000, 75000, N'15 m3 chia 3');
GO

-- 11. LỊCH SỬ THANH TOÁN (ThanhToan)
-- Thanh toán hóa đơn tháng 9 (Trigger sẽ tự động chuyển trạng thái HoaDon sang 'Đã thanh toán')
INSERT INTO dbo.ThanhToan (MaHoaDon, NgayThanhToan, SoTien, PhuongThuc, MaGiaoDich, NguoiThu, GhiChu)
VALUES 
(N'HD-202609-SV2026001', '2026-10-02 08:30:00', 835000, N'Chuyển khoản VietQR', N'VQR9823412', N'Hệ thống VietQR', N'Sinh viên chuyển khoản qua VietQR'),
(N'HD-202609-SV2026003', '2026-10-03 14:15:00', 835000, N'Tiền mặt', NULL, N'Nguyễn Văn Quản Lý', N'Nộp tiền mặt tại văn phòng KTX'),
(N'HD-202609-SV2026004', '2026-10-05 09:00:00', 835000, N'Chuyển khoản VietQR', N'VQR9823455', N'Hệ thống VietQR', N'Thanh toán qua app');
GO

-- 12. PHẢN ÁNH SỰ CỐ (SuCo)
INSERT INTO dbo.SuCo (MaPhong, MaSV, TieuDe, MoTa, MucDoUuTien, NgayBao, TrangThai, NguoiXuLy, ChiPhiSuaChua)
VALUES 
(N'A1-101', N'SV2026001', N'Vòi nước nhà tắm bị rò rỉ', N'Vòi nước bồn rửa mặt bị rỉ nước liên tục gây lãng phí nước', N'Bình thường', '2026-10-12 10:00:00', N'Đang xử lý', N'Thợ điện nước Tuấn', 50000),
(N'B2-101', N'SV2026002', N'Cháy bóng đèn tuýp LED', N'Bóng đèn chính phòng ngủ bị nhấp nháy rồi tắt hẳn', N'Khẩn cấp', '2026-10-10 19:30:00', N'Đã giải quyết', N'Thợ điện nước Tuấn', 85000);
GO

GO
-- Bổ sung đủ giường cho các phòng trong dữ liệu mẫu.
-- Complete missing bed records for the room capacities already declared.
-- Safe to rerun: existing beds, contracts and occupancy are not overwritten.
USE QuanLyKTXDB; -- Nhớ đổi tên database trong giống của mình
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @Expected TABLE (MaPhong nvarchar(20) PRIMARY KEY, Capacity int);
    INSERT INTO @Expected VALUES (N'A1-201', 6), (N'B2-102', 4);
    IF (SELECT COUNT(*) FROM dbo.Phong WITH (UPDLOCK, HOLDLOCK)
        WHERE MaPhong IN (N'A1-201', N'B2-102')) <> 2
        THROW 50001, N'Không tìm thấy đủ hai phòng cần bổ sung giường.', 1;
    IF EXISTS (SELECT 1 FROM @Expected e JOIN dbo.Phong p ON p.MaPhong=e.MaPhong
        WHERE p.SoChoToiDa<>e.Capacity)
        THROW 50002, N'Sức chứa phòng đã thay đổi. Cần kiểm tra trước khi bổ sung giường.', 1;

    DECLARE @Beds TABLE (MaGiuong nvarchar(30), MaPhong nvarchar(20),
        TenGiuong nvarchar(50), ViTri nvarchar(30), TrangThai nvarchar(30));
    INSERT INTO @Beds VALUES
(N'A1-201-G1', N'A1-201', N'Giường 1', NULL, N'Trống'),
(N'A1-201-G2', N'A1-201', N'Giường 2', NULL, N'Trống'),
(N'A1-201-G3', N'A1-201', N'Giường 3', NULL, N'Trống'),
(N'A1-201-G4', N'A1-201', N'Giường 4', NULL, N'Trống'),
(N'A1-201-G5', N'A1-201', N'Giường 5', NULL, N'Trống'),
(N'A1-201-G6', N'A1-201', N'Giường 6', NULL, N'Trống'),
(N'B2-102-G1', N'B2-102', N'Giường 1', NULL, N'Trống'),
(N'B2-102-G2', N'B2-102', N'Giường 2', NULL, N'Trống'),
(N'B2-102-G3', N'B2-102', N'Giường 3', NULL, N'Trống'),
(N'B2-102-G4', N'B2-102', N'Giường 4', NULL, N'Trống');

    INSERT INTO dbo.Giuong (MaGiuong, MaPhong, TenGiuong, ViTri, TrangThai)
    SELECT b.MaGiuong,b.MaPhong,b.TenGiuong,b.ViTri,b.TrangThai
    FROM @Beds b
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Giuong g WITH (UPDLOCK, HOLDLOCK)
        WHERE g.MaGiuong=b.MaGiuong);
    DECLARE @Added int=@@ROWCOUNT;
    IF EXISTS (SELECT 1 FROM @Expected e
        WHERE (SELECT COUNT(*) FROM dbo.Giuong g WHERE g.MaPhong=e.MaPhong)<>e.Capacity)
        THROW 50003, N'Số giường không khớp sức chứa; đã hủy toàn bộ lần bổ sung này.', 1;
    COMMIT TRANSACTION;
    SELECT @Added AS GiuongDaBoSung;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
SELECT p.MaPhong,p.SoChoToiDa AS SucChua,COUNT(g.MaGiuong) AS SoGiuong,
    SUM(CASE WHEN g.TrangThai=N'Trống' THEN 1 ELSE 0 END) AS GiuongTrong
FROM dbo.Phong p LEFT JOIN dbo.Giuong g ON g.MaPhong=p.MaPhong
GROUP BY p.MaPhong,p.SoChoToiDa ORDER BY p.MaPhong;
SELECT COUNT(*) AS TongGiuong,
    SUM(CASE WHEN TrangThai=N'Đang sử dụng' THEN 1 ELSE 0 END) AS DangSuDung,
    SUM(CASE WHEN TrangThai=N'Trống' THEN 1 ELSE 0 END) AS GiuongTrong,
    CAST(100.0*SUM(CASE WHEN TrangThai=N'Đang sử dụng' THEN 1 ELSE 0 END)/
        NULLIF(COUNT(*),0) AS decimal(5,2)) AS TyLeLapDay
FROM dbo.Giuong;
GO
