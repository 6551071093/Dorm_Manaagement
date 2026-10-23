-- DÙNG CHO DATABASE DEMO CỦA NHÓM.
-- Chạy thủ công file này sẽ tạo/đặt lại admin / Admin@123, mở khóa Admin
-- và thay mã khôi phục. Không xóa dữ liệu sinh viên/phòng/hợp đồng.
-- Chạy 06_AccountRecovery.sql trước. Ứng dụng không tự chạy script này.
USE [QuanLyKTXDB];
GO
SET XACT_ABORT ON;
SET NOCOUNT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.NguoiDung', N'U') IS NULL
        THROW 51000, N'Chưa có bảng NguoiDung. Hãy khởi tạo cấu trúc database trước.', 1;
    IF OBJECT_ID(N'dbo.TaiKhoanKhoiPhuc', N'U') IS NULL
        THROW 51001, N'Hãy chạy 06_AccountRecovery.sql trước.', 1;
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
    COMMIT TRANSACTION;
    SELECT N'admin' AS TenDangNhap, N'Admin@123' AS MatKhauDemo, @Recovery AS MaKhoiPhucMoi;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO