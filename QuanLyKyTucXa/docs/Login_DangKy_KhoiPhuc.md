# Login, Đăng ký và Khôi phục mật khẩu (23/10/2026)

## Sử dụng
- Login dùng nền #F6F6FA, chữ #0E2240, nút #1765B7 đồng bộ giao diện quản lý.
- Đăng ký: nhập tên tài khoản, họ tên và mật khẩu xác nhận. Tài khoản mới luôn có vai trò SinhVien, chưa hoạt động.
- Sau khi đăng ký, lưu mã khôi phục hiển thị trên màn hình. Mã chỉ lưu dạng SHA256 trong database; mật khẩu lưu PBKDF2 có salt.
- Admin vào Hệ thống > Quản lý tài khoản > chọn tài khoản > Sửa, chọn hồ sơ sinh viên và bật hoạt động. Có thể cấp vai trò Nhân viên hoặc Admin ở đây.
- Quên mật khẩu: nhập tên tài khoản, mã khôi phục và mật khẩu mới. Mã cũ dùng một lần, nhận mã mới sau khi đổi thành công.
- Khôi phục không mở khóa tài khoản hay thay đổi vai trò.
- Tài khoản cũ chưa được cấp mã, hoặc người dùng mất mã: Admin xác minh người dùng rồi đặt lại mật khẩu trong màn hình sửa tài khoản. Đặt lại bởi Admin thu hồi mã khôi phục cũ.
- Chức năng này dùng mã khôi phục, chưa gửi OTP qua email.

## Database First và triển khai
Chạy database/scripts/06_AccountRecovery.sql trên database đích trước khi sử dụng chức năng mới.
Script thêm bảng phụ dbo.TaiKhoanKhoiPhuc, không sửa Entities tự sinh hoặc dùng EF migrations. Truy cập bảng phụ bằng câu lệnh tham số hóa.
Database máy hiện tại đã được cập nhật. Tài khoản Admin mới admin.bon đã được tạo riêng trên máy hiện tại; thông tin mật khẩu nằm trong file riêng ngoài project, không đưa lên Git.
Không tạo tài khoản mặc định hoặc reset Admin tự động khi ứng dụng khởi động.

## File thay đổi
- Forms/Auth/FrmLogin.cs và FrmLogin.Designer.cs
- Forms/Auth/RegisterDialog.cs và RegisterDialog.Designer.cs
- Forms/Auth/ForgotPasswordDialog.cs và ForgotPasswordDialog.Designer.cs
- Forms/Auth/RecoveryCodeDialog.cs và RecoveryCodeDialog.Designer.cs
- Infrastructure/Services/AuthService.cs và AuthService.Enrollment.cs
- Infrastructure/Services/NguoiDungService.cs
- WinForms/QuanLyKyTucXa.WinForms.csproj
- database/scripts/06_AccountRecovery.sql

Admin quản lý vai trò bằng FrmNguoiDung/UserAccountEditDialog và AuthPolicy đã tích hợp ở lượt trước. Khi giao phần Tú lên Git cần kèm cả các file nền phân quyền đó.

## Cập nhật mã khôi phục
Mã mới gồm 16 ký tự hex ngẫu nhiên, không phân biệt hoa/thường. Mã 48 ký tự đã cấp vẫn được chấp nhận đến khi sử dụng/thay mới.
Giới hạn 5 lần thử khôi phục trong 15 phút theo tên tài khoản trong phiên chạy ứng dụng. Giới hạn này lưu trong bộ nhớ và sẽ mất khi ứng dụng khởi động lại.
Tên admin đã có sẵn với vai trò Admin nên tài khoản này được cập nhật theo yêu cầu người dùng; admin.bon được giữ nguyên. Mật khẩu/mã khôi phục được bàn giao riêng, không lưu trong repository.