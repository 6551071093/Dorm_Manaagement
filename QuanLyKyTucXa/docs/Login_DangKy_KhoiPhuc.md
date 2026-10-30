# Đăng ký, email và đặt lại mật khẩu

Luồng mã khôi phục cũ đã được thay bằng **OTP gửi qua email thật**. Xem [hướng dẫn Email/OTP Gmail](EmailOtp_Gmail.md) để cấu hình và sử dụng.

- Đăng ký: tên đăng nhập, họ tên, email, mật khẩu, xác nhận. Email không trùng tài khoản khác. Tài khoản mới là Sinh viên, chờ Admin liên kết hồ sơ và kích hoạt.
- Quên mật khẩu: email đã lưu → Gửi OTP → xác nhận 6 chữ số → nhập tài khoản, mật khẩu mới và xác nhận → Lưu mật khẩu. Không còn nhận hoặc dùng mã khôi phục.
- OTP có hạn 5 phút, sai tối đa 5 lần, dùng một lần; gửi lại cách 60 giây, tối đa 5 lần/tài khoản/giờ. Mã lưu dạng băm trong TaiKhoanOtp; hạn dùng/bộ đếm được lưu SQL, không mất khi đóng ứng dụng.
- Đặt lại không mở khóa tài khoản hoặc đổi vai trò. Đổi mật khẩu/email sẽ làm OTP trước đó mất hiệu lực.
- Admin có thể sửa email nhận OTP trong Quản lý tài khoản. Người dùng cũ chưa có email cần cập nhật trước khi dùng Quên mật khẩu.
- Ràng buộc tên đăng nhập/họ tên/email nằm ở AccountValidation; ràng buộc mật khẩu mới nằm ở PasswordHash.ValidateNew. Không tự đổi mật khẩu tài khoản cũ khi mở ứng dụng.
- Chạy script 05_UpdateSchema_Catalog.sql trên máy nhận để thêm bảng OTP. Không chạy lại 01/04 chỉ để bổ sung OTP.
- Máy gửi cần appsettings.local.json hoặc biến môi trường Email__*. File mật khẩu cục bộ đã được Git bỏ qua. Không đưa thông tin Gmail gửi vào bản code công khai hoặc bản bàn giao SQL.

Code mới: AuthService.Enrollment.cs, SmtpOtpEmailSender.cs, AccountValidation.cs, RegisterDialog, ForgotPasswordDialog, UserAccountEditDialog. RecoveryCodeDialog đã được gỡ.