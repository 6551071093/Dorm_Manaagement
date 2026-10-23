# Hướng dẫn khởi tạo và cập nhật CSDL QuanLyKTXDB

Hệ thống cơ sở dữ liệu **QuanLyKTXDB** được thiết kế theo phương pháp **Database-First** phục vụ ứng dụng Windows Forms (.NET 9) với Entity Framework Core.

---

## 1. Danh sách file script (Thứ tự thực thi bắt buộc)

| Thứ tự | Tên File Script | Mục đích |
|---|---|---|
| **1** | `01_CreateDatabase_Tables.sql` | Khởi tạo CSDL, 13 bảng thực thể, khóa chính, khóa ngoại và toàn bộ ràng buộc `CHECK`. |
| **2** | `02_CreateIndexes_Triggers.sql` | Tạo chỉ mục tìm kiếm, Filtered Unique Index chống trùng giường/hợp đồng và 3 Trigger toàn vẹn dữ liệu. |
| **3** | `03_StoredProcedures_Views.sql` | Tạo 4 Views báo cáo và 4 Stored Procedures (kèm procedure chốt điện nước & chia đều hóa đơn theo phòng). |
| **4** | `04_InsertSampleData.sql` | Nạp dữ liệu mẫu phong phú (tài khoản demo, sinh viên, phòng, chỉ số điện nước, hóa đơn, thanh toán). |

---

## 2. Cách thực thi trên SQL Server Management Studio (SSMS)

1. Mở SSMS và kết nối vào instance SQL Server của bạn (ví dụ: `.` hoặc `localhost` hoặc `(localdb)\MSSQLLocalDB`).
2. Mở lần lượt các file script từ `01` đến `04` theo thứ tự trên.
3. Bấm **Execute** (`F5`) cho từng file. Đảm bảo mỗi file đều hoàn thành với thông báo thành công.

---

## 3. Cách thực thi tự động qua PowerShell / Command Prompt

Sử dụng lệnh `sqlcmd` với cờ `-f 65001` (bắt buộc để đảm bảo mã hóa tiếng Việt UTF-8 không bị lỗi dấu):

```powershell
# Chạy trên LocalDB mặc định:
sqlcmd -f 65001 -S "(localdb)\MSSQLLocalDB" -i database\scripts\01_CreateDatabase_Tables.sql
sqlcmd -f 65001 -S "(localdb)\MSSQLLocalDB" -i database\scripts\02_CreateIndexes_Triggers.sql
sqlcmd -f 65001 -S "(localdb)\MSSQLLocalDB" -i database\scripts\03_StoredProcedures_Views.sql
sqlcmd -f 65001 -S "(localdb)\MSSQLLocalDB" -i database\scripts\04_InsertSampleData.sql
```

---

## 4. Tài khoản đăng nhập mẫu

Tài khoản **admin / Admin@123**. Các tài khoản thử nghiệm khác có mật khẩu **123456**.

| Tên đăng nhập | Vai trò | Người dùng | Ghi chú |
|---|---|---|---|
| `admin` | Admin | Quản trị viên Hệ thống | Toàn quyền quản trị hệ thống |
| `nhanvien01` | NhanVien | Nguyễn Văn Quản Lý | Quản lý sinh viên, phòng, hóa đơn |
| `sv2026001` | SinhVien | Nguyễn Văn An | Sinh viên phòng A1-101 |
| `sv2026002` | SinhVien | Trần Thị Bình | Sinh viên phòng B2-101 |
| `sv2026003` | SinhVien | Lê Hoàng Nam | Sinh viên phòng A1-101 |

## Đăng nhập Admin sau khi pull code

Git không mang theo dữ liệu SQL Server của máy thành viên khác.

### Máy đã có database QuanLyKTXDB
1. Mở SSMS, kết nối đúng SQL Server được khai báo tại `ConnectionStrings:KtxDb` trong appsettings.json.
2. Chạy `database/scripts/06_AccountRecovery.sql` (chạy lại được).
3. Chạy `database/scripts/07_SetupDemoAdmin.sql`. Script tạo/reset Admin demo, giữ nguyên dữ liệu nghiệp vụ. Lưu mã khôi phục mới trả về trong Results.
4. Chạy project WinForms, đăng nhập **admin / Admin@123**.

### Máy chưa có database
Khởi tạo cấu trúc bằng 01, 02, 03. Nếu cần dữ liệu mẫu, chạy 04 rồi 05. Tiếp theo chạy 06 và 07.
File 04 xóa/nạp lại dữ liệu mẫu; không dùng 04 chỉ để sửa mật khẩu.

Chỉnh tên SQL Server theo máy của mình; `MSI\SQLEXPRESS` chỉ là tên instance trên máy của Bổn. Đặt Database=QuanLyKTXDB và dùng Windows Authentication nếu dùng tài khoản Windows.
Nếu có biến môi trường `ConnectionStrings__KtxDb`, biến đó có thể ghi đè cấu hình appsettings.json.

`07_SetupDemoAdmin.sql` dành cho bàn giao/demo nhóm, chỉ chạy khi cần tạo/reset Admin. Mỗi lần chạy sẽ đặt lại mật khẩu Admin và mã khôi phục. Ứng dụng không reset mật khẩu khi khởi động.