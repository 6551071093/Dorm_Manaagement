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

Mật khẩu mặc định cho toàn bộ tài khoản thử nghiệm: **`123456`**

| Tên đăng nhập | Vai trò | Người dùng | Ghi chú |
|---|---|---|---|
| `admin` | Admin | Quản trị viên Hệ thống | Toàn quyền quản trị hệ thống |
| `nhanvien01` | NhanVien | Nguyễn Văn Quản Lý | Quản lý sinh viên, phòng, hóa đơn |
| `sv2026001` | SinhVien | Nguyễn Văn An | Sinh viên phòng A1-101 |
| `sv2026002` | SinhVien | Trần Thị Bình | Sinh viên phòng B2-101 |
| `sv2026003` | SinhVien | Lê Hoàng Nam | Sinh viên phòng A1-101 |
