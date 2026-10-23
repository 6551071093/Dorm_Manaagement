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
