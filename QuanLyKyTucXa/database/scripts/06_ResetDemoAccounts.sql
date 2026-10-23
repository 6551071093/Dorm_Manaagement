-- TÙY CHỌN: ĐẶT LẠI TÀI KHOẢN DEMO. KHÔNG CHẠY MỖI LẦN KHỞI ĐỘNG.
-- Chạy 05_UpdateSchema_Catalog.sql trước. Không xóa dữ liệu nghiệp vụ.
-- Phần A: admin / Admin@123. Phần B: nhanvien01 / NhanVien@123; sv2026001-003 / SinhVien@123.
-- Chỉ chạy khi chấp nhận đặt lại mật khẩu và mã khôi phục của các tài khoản demo.

-- PHẦN A: ADMIN
-- DÙNG CHO DATABASE DEMO CỦA NHÓM.
-- Chạy thủ công file này sẽ tạo/đặt lại admin / Admin@123, mở khóa Admin
-- và thay mã khôi phục. Không xóa dữ liệu sinh viên/phòng/hợp đồng.
-- Chạy 05_UpdateSchema_Catalog.sql trước. Ứng dụng không tự chạy script này.
USE [QuanLyKTXDB];
GO
SET XACT_ABORT ON;
SET NOCOUNT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.NguoiDung', N'U') IS NULL
        THROW 51000, N'Chưa có bảng NguoiDung. Hãy khởi tạo cấu trúc database trước.', 1;
    IF OBJECT_ID(N'dbo.TaiKhoanKhoiPhuc', N'U') IS NULL
        THROW 51001, N'Hãy chạy 05_UpdateSchema_Catalog.sql trước.', 1;
    DECLARE @Id int, @Role nvarchar(20);
    SELECT @Id = MaNguoiDung, @Role = VaiTro FROM dbo.NguoiDung WITH (UPDLOCK, HOLDLOCK) WHERE TenDangNhap = N'admin';
    IF @Id IS NOT NULL AND @Role <> N'Admin'
        THROW 51002, N'Tên admin đang thuộc tài khoản không phải Admin. Dừng để tránh chiếm tài khoản.', 1;
    IF @Id IS NULL
    BEGIN
        INSERT dbo.NguoiDung (TenDangNhap, MatKhauHash, HoTen, VaiTro, TrangThai, NgayTao)
        VALUES (N'admin', N'PBKDF2-SHA256$600000$gltA27sVEvlyhPpy64Hi8w==$8bczWZFRkePj1TjvhwETvGrLTzToYSYxjOlYJr3PcWk=', N'Quản trị viên Hệ thống', N'Admin', 1, GETDATE());
        SET @Id = CONVERT(int, SCOPE_IDENTITY());
    END
    ELSE
        UPDATE dbo.NguoiDung SET MatKhauHash = N'PBKDF2-SHA256$600000$gltA27sVEvlyhPpy64Hi8w==$8bczWZFRkePj1TjvhwETvGrLTzToYSYxjOlYJr3PcWk=', TrangThai = 1 WHERE MaNguoiDung = @Id;
    DECLARE @Recovery varchar(16) = CONVERT(varchar(16), CRYPT_GEN_RANDOM(8), 2);
    DECLARE @RecoveryHash char(64) = CONVERT(char(64), HASHBYTES('SHA2_256', @Recovery), 2);
    DELETE dbo.TaiKhoanKhoiPhuc WHERE MaNguoiDung = @Id;
    INSERT dbo.TaiKhoanKhoiPhuc (MaNguoiDung, MaKhoiPhucHash) VALUES (@Id, @RecoveryHash);
    IF OBJECT_ID(N'dbo.TaiKhoanDangNhap', N'U') IS NOT NULL
        EXEC sp_executesql N'DELETE dbo.TaiKhoanDangNhap WHERE MaNguoiDung = @UserId', N'@UserId int', @UserId = @Id;
    COMMIT TRANSACTION;
    SELECT N'admin' AS TenDangNhap, N'Admin@123' AS MatKhauDemo, @Recovery AS MaKhoiPhucMoi;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
GO
-- PHẦN B: NHÂN VIÊN VÀ SINH VIÊN DEMO
-- ĐỔI MẬT KHẨU 4 TÀI KHOẢN DEMO; KHÔNG XÓA DỮ LIỆU NGHIỆP VỤ.
-- Chạy trên database ứng dụng đang chọn. Không chạy lại sau khi đã tự đổi mật khẩu.
-- nhanvien01 / NhanVien@123; sv2026001, sv2026002, sv2026003 / SinhVien@123.
SET XACT_ABORT ON;
BEGIN TRY
BEGIN TRANSACTION;
IF (SELECT COUNT(*) FROM dbo.NguoiDung WHERE (TenDangNhap=N'nhanvien01' AND VaiTro=N'NhanVien') OR (TenDangNhap IN(N'sv2026001',N'sv2026002',N'sv2026003') AND VaiTro=N'SinhVien')) <> 4
    THROW 51101,N'Thiếu tài khoản demo hoặc vai trò đã thay đổi. Không cập nhật.',1;
UPDATE dbo.NguoiDung SET MatKhauHash=N'PBKDF2-SHA256$600000$hS/mw0kVued1j+WKngc8gQ==$fheghY73BSNeN8tWyd43oLnfV4ZldZArrWOcPG/UiNc=', TrangThai=1 WHERE TenDangNhap=N'nhanvien01' AND VaiTro=N'NhanVien';
UPDATE dbo.NguoiDung SET MatKhauHash=N'PBKDF2-SHA256$600000$FNBbltqSiSFnr1c/bJcRSg==$fcDxXG0hwj2LkbBTbOVvPRRSExC+JNQiYvkJ1iJFnXg=', TrangThai=1 WHERE TenDangNhap IN(N'sv2026001',N'sv2026002',N'sv2026003') AND VaiTro=N'SinhVien';
IF OBJECT_ID(N'dbo.TaiKhoanDangNhap',N'U') IS NOT NULL
    EXEC(N'DELETE FROM dbo.TaiKhoanDangNhap WHERE MaNguoiDung IN(SELECT MaNguoiDung FROM dbo.NguoiDung WHERE TenDangNhap IN(N''nhanvien01'',N''sv2026001'',N''sv2026002'',N''sv2026003''))');
IF OBJECT_ID(N'dbo.TaiKhoanKhoiPhuc',N'U') IS NOT NULL
    EXEC(N'DELETE FROM dbo.TaiKhoanKhoiPhuc WHERE MaNguoiDung IN(SELECT MaNguoiDung FROM dbo.NguoiDung WHERE TenDangNhap IN(N''nhanvien01'',N''sv2026001'',N''sv2026002'',N''sv2026003''))');
COMMIT TRANSACTION;
END TRY BEGIN CATCH IF @@TRANCOUNT>0 ROLLBACK TRANSACTION; THROW; END CATCH;