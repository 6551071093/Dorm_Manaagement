# Dùng chung dữ liệu khi các thành viên ở khác mạng

## Trạng thái hiện tại
Ứng dụng đọc cấu hình triển khai từ appsettings.json, appsettings.local.json hoặc biến môi trường. Màn hình cấu hình SQL trên Login đã được xóa. Database hiện tại vẫn ở SQL Express trên máy Bổn; chưa có VPN hoặc địa chỉ máy chủ online nên các máy bên ngoài chưa kết nối được.
Git chỉ đồng bộ mã nguồn. Thêm sinh viên, duyệt đơn, sửa phòng... ghi vào SQL Server bằng transaction; muốn mọi người đọc cùng dữ liệu, phải dùng CHUNG một máy chủ và cùng tên database.

## 1. Tạo đường kết nối VPN
Lựa chọn tham khảo: Tailscale, theo tài liệu chính thức:
- Cài Windows: https://tailscale.com/docs/install/windows
- Kết nối thiết bị, quản lý mạng: https://tailscale.com/docs/how-to/quickstart

Người quản lý tự tạo tài khoản và đăng nhập. Mời đúng các thành viên vào mạng VPN, mỗi người đăng nhập bằng tài khoản của mình. Không chia sẻ tài khoản đăng nhập VPN.
Ghi lại địa chỉ VPN 100.x.y.z của máy SQL Server. Máy này cần bật, SQL Server và VPN phải đang chạy khi nhóm sử dụng.

## 2. SQL Server trên máy chủ
Dùng SQL Server Express SQLEXPRESS đang chứa QuanLyKTXDB (không dùng LocalDB làm máy chủ từ xa).
Trong SQL Server Configuration Manager:
- SQL Server Network Configuration > Protocols for SQLEXPRESS > bật TCP/IP.
- TCP/IP Properties > IP Addresses > IPAll: bỏ giá trị TCP Dynamic Ports, đặt TCP Port = 14330 (chọn cổng chưa dùng).
- Khởi động lại dịch vụ SQL Server (SQLEXPRESS) vào lúc nhóm không ghi dữ liệu.

Tài liệu: https://learn.microsoft.com/en-us/sql/relational-databases/lesson-2-connecting-from-another-computer?view=sql-server-ver17

## 3. Firewall và phạm vi truy cập
Chỉ cho phép TCP 14330 từ các IP VPN cụ thể của thành viên tới IP VPN của máy SQL Server. Dùng Windows Defender Firewall with Advanced Security > Inbound Rules > New Rule > Port > TCP 14330; trong Scope giới hạn Local IP là IP VPN máy chủ và Remote IP là từng IP VPN đã được duyệt.
Không mở cổng SQL trên router ra Internet; không dùng Remote Address=Any.
Hướng dẫn Microsoft: https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/configure-a-windows-firewall-for-database-engine-access?view=sql-server-ver17

## 4. Chuẩn bị database và tài khoản SQL
Trên database máy chủ đang có dữ liệu: chạy script 05_UpdateSchema_Catalog.sql nếu chưa có; không chạy lại 01/04 vì đó là các script dựng/nạp lại dữ liệu.
Dữ liệu đang có nằm trên máy chủ và không phải sao chép cho từng máy thành viên.
Nếu dùng SQL Authentication cho các máy ngoài Windows domain:
- Trong SSMS > Server Properties > Security, người quản lý cấu hình SQL Server and Windows Authentication mode rồi khởi động lại SQL Server nếu cần.
- Người quản trị máy chủ cấp riêng tài khoản SQL cho ứng dụng theo quyền cần thiết. Chưa cấu hình máy chủ chung trong bản hiện tại.
- Tài khoản ktx_shared_app dùng để kết nối database; admin/nhanvien01/... là tài khoản bên trong ứng dụng. Hai loại này khác nhau.

Tài khoản SQL được cấp DML để EF Core hoạt động, không được cấp quản trị server hoặc sửa cấu trúc. Phân quyền Admin/Nhân viên/Sinh viên trong bản đồ án nằm ở tầng ứng dụng; người có thông tin đăng nhập SQL có thể truy cập trực tiếp dữ liệu ngoài ứng dụng. Nếu cần cô lập quyền chống truy cập SQL trực tiếp, triển khai API trung gian hoặc quyền SQL riêng cho từng người.

## 5. Cấu hình triển khai cho từng máy
Người triển khai cấu hình trước khi người dùng mở ứng dụng; Login chỉ phục vụ đăng nhập tài khoản KTX.

1. Pull mã mới, bật VPN của nhóm.
2. Tạo `src/QuanLyKyTucXa.WinForms/appsettings.local.json` với `ConnectionStrings:KtxDb` trỏ tới `tcp:IP_VPN_MAY_CHU,14330`, database `QuanLyKTXDB`. Dùng tài khoản SQL riêng do người quản lý cấp nếu các máy không thuộc Windows domain.
3. Dùng chứng chỉ máy chủ hợp lệ. Chỉ tin chứng chỉ tự ký khi máy chủ VPN đã được nhóm xác minh.
4. Build/chạy WinForms. File local được copy vào thư mục chạy và đã được .gitignore loại trừ. Không đưa mật khẩu SQL lên Git hoặc gửi trong ảnh.
5. Có thể cấp biến môi trường `ConnectionStrings__KtxDb` thay cho file local; biến này có ưu tiên cao nhất.
6. Đăng nhập tài khoản ứng dụng. Bấm Làm mới/F5 để tải dữ liệu thay đổi bởi thành viên khác.

Không còn dùng file cấu hình DPAPI DatabaseConnection.dat của phiên bản cũ. Ứng dụng không tự chuyển database khi kết nối lỗi.

Hiện nhóm chưa có máy chủ/VPN. Cách bàn giao qua Git đang dùng là xuất/nhập `database/snapshots/LatestData.sql`, xem **BanGiaoDuLieu.md**. Cách này không đồng bộ trực tiếp.
## Dữ liệu lưu như thế nào?
- Chỉ sau khi ứng dụng báo lưu/duyệt thành công mới coi giao dịch đã commit.
- Đóng/mở ứng dụng không xóa dữ liệu SQL Server.
- Pull code không reset dữ liệu trên máy chủ, trừ khi người dùng chủ động chạy các script xóa/nạp lại.
- Máy chủ mất mạng/tắt thì không có chế độ ghi offline và không giả báo thành công.
- Người quản lý cần sao lưu SQL Server định kỳ; Git không thay thế backup database.