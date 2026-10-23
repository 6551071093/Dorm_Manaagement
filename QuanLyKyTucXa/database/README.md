# Database QuanLyKTXDB

Ứng dụng dùng **một database QuanLyKTXDB**. Thư mục scripts chỉ còn **6 file SQL**; không cần chạy tất cả mỗi lần mở ứng dụng.

## Các file được giữ

| File trong scripts | Khi nào dùng |
|---|---|
| 01_CreateDatabase_Tables.sql | Tạo cấu trúc trên database mới; có xóa bảng cũ, không chạy lại trên dữ liệu đang dùng |
| 02_CreateIndexes_Triggers.sql | Tạo chỉ mục và trigger sau 01 |
| 03_StoredProcedures_Views.sql | Tạo view và stored procedure sau 02 |
| 04_InsertSampleData.sql | Tùy chọn nạp lại dữ liệu mẫu, đã có đủ giường; xóa dữ liệu cũ |
| 05_UpdateSchema_Catalog.sql | Bổ sung khôi phục tài khoản, khóa đăng nhập và danh mục khoa/ngành/lớp; chạy lại được, không reset mật khẩu |
| 06_ResetDemoAccounts.sql | Tùy chọn đặt lại Admin và bốn tài khoản demo; chỉ chạy khi muốn reset mật khẩu/mã khôi phục |

Các file cập nhật rời trước đây đã được gộp. Mẫu cấp login SQL cho máy chủ chung chưa dùng đã bỏ. Việc dọn file không xóa database hoặc dữ liệu đang lưu trên máy.

## Người mới tải code: muốn thấy dữ liệu Bổn đã sửa

1. Kết nối SQL Server của máy mình bằng SSMS.
2. Trên **database mới**, chạy **01 → 02 → 03 → 05**.
3. Mở **[snapshots/LatestData.sql](snapshots/LatestData.sql)**, chọn đúng **QuanLyKTXDB** rồi Execute.
4. Cấu hình SQL instance của máy nhận trong appsettings.local.json; chọn QuanLyKyTucXa.WinForms làm Startup Project và chạy.

Không cần chạy 04 hoặc 06 khi dùng bản LatestData mới. LatestData thay thế dữ liệu của database đang chọn; sao lưu trước nếu database đã có dữ liệu cần giữ.

## Máy đang có database và dữ liệu

- Chỉ cần chạy **05_UpdateSchema_Catalog.sql** nếu chưa cập nhật phần tài khoản/danh mục.
- Đóng/mở WinForms và pull code không xóa dữ liệu SQL.
- Không chạy lại 01 hoặc 04 để sửa lỗi đăng nhập.
- Nếu muốn nhận dữ liệu mới từ Bổn: sao lưu database máy nhận rồi nhập LatestData như trên. Đây là thay thế, không tự ghép dữ liệu hai máy.
- **06_ResetDemoAccounts.sql** dành riêng cho reset demo. Chạy toàn file sẽ reset Admin cùng bốn tài khoản Nhân viên/Sinh viên. Có thể chọn riêng phần A (Admin) hoặc B (bốn tài khoản còn lại) trong SSMS. Phần B yêu cầu đã có đủ bốn tài khoản và đúng vai trò.

## Tài khoản demo

| Tài khoản | Mật khẩu | Vai trò |
|---|---|---|
| admin | Admin@123 | Admin |
| nhanvien01 | NhanVien@123 | Nhân viên |
| sv2026001, sv2026002, sv2026003 | SinhVien@123 | Sinh viên |

Script 04 và LatestData dùng các mật khẩu trên. Database cũ chỉ đổi khi chạy reset demo hoặc Admin chủ động đặt mật khẩu. Ứng dụng không tự reset mật khẩu khi mở.

## Bàn giao lần tiếp theo

Admin chọn **Hệ thống → Lưu dữ liệu bàn giao**, lưu đè `database/snapshots/LatestData.sql`, xem lại rồi commit/push cùng code. Người nhận pull và nhập bản này vào database demo. Chỉ pull Git không tự đồng bộ dữ liệu SQL.

Chi tiết: **[Hướng dẫn bàn giao](../docs/BanGiaoDuLieu.md)** và **[Phân quyền](../docs/PhanQuyen_KhoaDangNhap.md)**.
