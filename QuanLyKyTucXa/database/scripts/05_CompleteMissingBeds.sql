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
