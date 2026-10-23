# Phân quyền và khóa đăng nhập

## Quyền theo vai trò

| Chức năng đã triển khai | Admin | Nhân viên | Sinh viên |
|---|---|---|---|
| Dashboard toàn ký túc xá | Có | Có | Không |
| Xem, thêm, sửa hồ sơ sinh viên | Có | Có | Không |
| Xem/tìm phòng | Có | Có | Chỉ phòng của mình |
| Thêm/sửa cấu hình phòng (giá, sức chứa, giường...) | Có | Không | Không |
| Lưu dữ liệu bàn giao | Có | Không | Không |
| Xóa sinh viên/phòng (có kiểm tra lịch sử) | Có | Không | Không |
| Nhập Excel hàng loạt, tải mẫu nhập | Có | Không | Không |
| Xuất Excel dữ liệu được xem | Có | Có | Chỉ dữ liệu cá nhân |
| Gửi đăng ký, tra cứu đơn | Toàn bộ | Toàn bộ | Chỉ chính mình |
| Duyệt/từ chối đơn, chọn giường | Có | Có | Không |
| Quản lý tài khoản, cấp vai trò, khóa/mở khóa, reset mật khẩu | Có | Không | Không |
| Xem hồ sơ, phòng, hóa đơn và sự cố cá nhân | Qua nghiệp vụ quản lý | Qua nghiệp vụ quản lý | Chỉ chính mình |
| Gửi báo hỏng cá nhân | Không áp dụng | Không áp dụng | Phòng đang ở |
| Đổi mật khẩu của mình, đăng xuất | Có | Có | Có |

Menu Thống kê chỉ dành Admin; phân hệ báo cáo tổng hợp, quản lý hợp đồng/hóa đơn/điện nước/sự cố toàn hệ thống vẫn là các phần chưa triển khai ở mã hiện tại. Phân quyền không tự tạo thêm những nghiệp vụ đó.

## Đăng nhập
1. Tìm tài khoản; không tồn tại hoặc mật khẩu sai thì thông báo lỗi.
2. Tài khoản đang khóa/chưa kích hoạt không được vào.
3. Với tài khoản hoạt động: mỗi lần mật khẩu sai tăng bộ đếm trong database. Lần sai thứ 5 đặt BiKhoa=1 và NguoiDung.TrangThai=0.
4. Đóng mở ứng dụng không xóa bộ đếm. Đăng nhập thành công đưa số lần sai về 0.
5. Admin mở Quản lý tài khoản > chọn tài khoản bị khóa > Mở khóa (Sinh viên phải có hồ sơ liên kết). Thao tác này xóa bộ đếm và trạng thái khóa do đăng nhập.
6. Khôi phục mật khẩu không tự mở khóa. Admin không được tự khóa/hạ quyền, luôn phải giữ ít nhất một Admin hoạt động khi quản lý tài khoản.
7. Sau khi đăng nhập xác định đúng vai trò; giới hạn dữ liệu Sinh viên áp dụng cả ở service. Vai trò/tài khoản/mật khẩu thay đổi thì phiên cũ bị từ chối ở lần kiểm tra tiếp theo.

## Database First
Chạy **database/scripts/05_UpdateSchema_Catalog.sql** trên database đã có để bổ sung bảng khôi phục, bộ đếm khóa và danh mục khoa/lớp. Script này không reset mật khẩu hoặc xóa hồ sơ.
Chỉ khi muốn đặt lại tài khoản demo mới chạy **06_ResetDemoAccounts.sql**. File có phần A cho Admin và B cho nhanvien01/sv2026001–003. Chạy toàn file đặt lại cả hai phần. Không chạy lại 04 vì 04 xóa/nạp lại dữ liệu mẫu.
## File liên quan
- Infrastructure/Security/AuthSession.cs
- Infrastructure/Repositories/NguoiDungRepository.cs
- Infrastructure/Services/AuthService.cs, NguoiDungService.cs, SinhVienService.cs, PhongGiuongService.cs
- WinForms/Forms/Main/FrmMain.Auth.cs, FrmMain.cs
- WinForms/Forms/Residents/FrmSinhVien.cs, FrmSinhVien.Excel.cs
- WinForms/Forms/Rooms/FrmPhong.cs
- WinForms/Forms/Users/FrmNguoiDung.cs
- database/scripts/05_UpdateSchema_Catalog.sql và 06_ResetDemoAccounts.sql
## Mật khẩu demo đã thống nhất

| Tài khoản | Mật khẩu |
|---|---|
| admin | Admin@123 |
| nhanvien01 | NhanVien@123 |
| sv2026001, sv2026002, sv2026003 | SinhVien@123 |

Máy đang có database cũ: chọn đúng QuanLyKTXDB trong SSMS và chạy phần B của **06_ResetDemoAccounts.sql** nếu chỉ muốn đổi mật khẩu, kích hoạt và xóa khóa/mã khôi phục cũ của bốn tài khoản nhanvien01/sv2026001–003. Chạy phần A nếu cần reset riêng Admin. Chạy toàn file reset cả năm tài khoản demo. Giữ vai trò Nhân viên/Sinh viên và dữ liệu nghiệp vụ; không chạy lại 04 để sửa mật khẩu.

Không reset mật khẩu tự động khi khởi động. Khi quản trị viên đã đặt mật khẩu riêng thì không chạy lại script demo. Người nhận LatestData.sql mới đã có mật khẩu mới, không cần chạy reset demo.

Kiểm tra quyền ở AuthPolicy và service lưu dữ liệu, đồng thời ẩn nút không được phép ở form. Quyền tài khoản trong database thay đổi sẽ làm phiên cũ bị từ chối ở lần kiểm tra tiếp theo. Hệ thống > Quyền hạn của tôi hiển thị mô tả theo vai trò.