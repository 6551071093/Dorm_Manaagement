# Email thật và OTP Gmail

## Trạng thái

Đăng ký yêu cầu email thật người dùng đang sử dụng; chấp nhận Gmail và các email hợp lệ khác, không ép đuôi trường. Tên đăng nhập, họ tên, email và mật khẩu được kiểm tra ở cả giao diện đăng ký và service.

- Tên đăng nhập: 3–50 ký tự; bắt đầu/kết thúc bằng chữ hoặc số; chỉ chữ không dấu, số và `. _ @ -`; không có khoảng trắng hoặc hai dấu chấm liền nhau.
- Họ tên: 2–100 ký tự chữ có dấu, khoảng trắng, dấu nối/dấu nháy; không có số và ký tự lạ. Chuẩn hóa khoảng trắng và Unicode.
- Mật khẩu mới: 8–128 ký tự, có chữ hoa, chữ thường, số, ký tự đặc biệt; không có khoảng trắng. Xác nhận mật khẩu phải trùng. Áp dụng khi đăng ký, đổi, quên mật khẩu và Admin đặt mật khẩu.
- Email: tối đa 100 ký tự, đúng cú pháp, không có khoảng trắng/dấu chấm ngay trước @, không trùng tài khoản khác. Đúng cú pháp không chứng minh hộp thư tồn tại; chỉ khi người dùng nhận và nhập OTP mới chứng minh được việc truy cập hộp thư.

Email liên hệ ở hồ sơ Sinh viên và email đăng nhập/OTP ở Tài khoản là hai trường riêng. Nhân viên sửa hồ sơ không tự đổi email bảo mật của tài khoản.

## Cấu hình Gmail trên máy triển khai

File riêng máy: `src/QuanLyKyTucXa.WinForms/appsettings.local.json`, đã được Git bỏ qua. Phần Email đã được chuẩn bị theo địa chỉ gửi do người quản lý cung cấp. **Không commit file này hoặc mật khẩu ứng dụng.**

```json
{
  "Email": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "DIA_CHI_GMAIL_GUI",
    "FromAddress": "DIA_CHI_GMAIL_GUI",
    "FromName": "KTX Home",
    "Password": "MAT_KHAU_UNG_DUNG_GOOGLE"
  }
}
```

1. Chủ tài khoản Gmail tự bật Xác minh hai bước và tạo mật khẩu ứng dụng. Không dùng mật khẩu Gmail thông thường. Xem [Google: mật khẩu ứng dụng](https://support.google.com/accounts/answer/185833?hl=vi).
2. Chủ tài khoản tự nhập mật khẩu ứng dụng vào Password trong file local. Không gửi mật khẩu vào chat, ảnh hoặc Git.
3. Đóng ứng dụng, build và chạy lại để copy file local vào thư mục chạy. Có thể dùng biến môi trường `Email__Host`, `Email__Port`, `Email__Username`, `Email__Password`, `Email__FromAddress`, `Email__FromName` thay thế.
4. Kết nối Gmail dùng TLS bắt buộc, cổng 587. Xem [Gmail SMTP](https://developers.google.com/workspace/gmail/imap/imap-smtp).

Nếu chưa cấu hình hoặc gửi SMTP thất bại, ứng dụng báo lỗi và không cấp OTP hợp lệ. Không có OTP giả hay mã dự phòng hiển thị trên màn hình. SMTP chấp nhận gửi không đảm bảo email đã vào Inbox; người nhận kiểm tra Spam hoặc chờ rồi gửi lại.

File local là cấu hình riêng cho môi trường phát triển, không mã hóa trên đĩa. Không chia sẻ file chứa mật khẩu cho người tải code. Khi triển khai cho người dùng cuối, dịch vụ gửi OTP và thông tin SMTP nên nằm trên máy chủ/API được quản trị; không phân phối tài khoản Gmail gửi cho từng người dùng.

## Database và tài khoản cũ

- Máy hiện tại đã được bổ sung bảng TaiKhoanOtp, không nạp lại dữ liệu.
- Máy thành viên chạy lại **05_UpdateSchema_Catalog.sql** để thêm bảng OTP; script không reset mật khẩu hay xóa hồ sơ.
- Admin mở Hệ thống > Quản lý tài khoản > Sửa, nhập email thật cho tài khoản cần dùng OTP. Không tự gán Gmail gửi thư vào bất kỳ tài khoản người dùng nào.
- Tài khoản cũ chưa có email vẫn đăng nhập được; cần Admin cập nhật email trước khi dùng Quên mật khẩu. Không thể dùng email chưa lưu để nhận mã cho tài khoản khác.
- Bản LatestData loại thông tin email cá nhân; máy nhận bản demo phải cập nhật email thật cho tài khoản muốn dùng OTP. Không xuất OTP hoặc cấu hình SMTP trong bản bàn giao.
- Luồng mã khôi phục cũ đã bị gỡ khỏi ứng dụng. Bảng cũ giữ lại để tương thích script/backup, các mã cũ không thể đặt lại mật khẩu nữa.

## Luồng Quên mật khẩu — ba bước

1. Bấm Quên mật khẩu: chỉ hiện **Email**. Nhập email đã lưu và bấm **Gửi OTP**. Nếu email khớp đúng một tài khoản, hệ thống gửi mã đến email đó rồi chuyển sang bước OTP. Thông báo dùng câu chung để không tiết lộ email có tồn tại hay không.
2. Chỉ hiện **OTP 6 chữ số** và nút gửi lại. Nhập mã từ Hộp thư/Spam rồi bấm **Xác nhận OTP**. Mã sai/hết hạn không được chuyển bước. Mỗi OTP có hạn 5 phút, sai tối đa 5 lần, dùng một lần. Giới hạn gửi lại 60 giây và tối đa 5 lượt/tài khoản/giờ vẫn lưu trong SQL.
3. OTP đúng mới hiện **Tài khoản, mật khẩu mới, xác nhận mật khẩu**. Tên tài khoản phải gắn với email đã xác nhận. Chỉ bấm **Lưu mật khẩu** thành công ở bước này mới cập nhật mật khẩu; hai bước trước không đổi mật khẩu.

Sau khi xác nhận OTP, service cấp phiên ngẫu nhiên 256 bit có hạn 5 phút, chỉ dùng một lần và gắn với đúng tài khoản/email/mật khẩu hiện tại. Chỉ lưu bản băm trong SQL, không hiển thị phiên trong form hay log. Không thể dùng một cờ trên giao diện để bỏ qua bước OTP. OTP đã xác nhận không thể dùng lại; gửi OTP mới hoặc đổi mật khẩu/email sẽ vô hiệu hóa phiên cũ.

Nút Nhập lại quay về bước Email, bỏ phiên đang giữ và xóa mật khẩu trong form. Đóng cửa sổ không lưu mật khẩu. Đặt lại không tự mở khóa tài khoản hoặc đổi vai trò.

Các ô mật khẩu và xác nhận dài toàn hàng, chỉ các ô của bước hiện tại hiển thị. Khung tự thu gọn theo nội dung và giữ hàng nút phía dưới.

Database mới/của thành viên chạy lại **05_UpdateSchema_Catalog.sql** để bổ sung cột Stage cho TaiKhoanOtp. Database máy hiện tại đã được cập nhật cột này. Không chạy lại 01/04 hoặc LatestData chỉ để cập nhật luồng OTP.

Gửi thư thật cần mật khẩu ứng dụng Gmail hợp lệ trong cấu hình cục bộ. Lượt chỉnh luồng ba bước không tự gửi thư, không tự đặt lại tài khoản và không đọc/hiển thị mật khẩu SMTP.
## Nút gửi lại và chẩn đoán Gmail

- Nút Gửi lại dùng control bo góc, tự bố trí theo DPI và đủ rộng cho bộ đếm 60 giây; OTP hiển thị dạng số để người dùng kiểm tra trước khi xác nhận.
- Gmail SMTP yêu cầu App Password 16 ký tự (bỏ khoảng trắng phân nhóm). Cấu hình sai sẽ báo rõ trước khi gửi; không thay bằng mật khẩu đăng nhập Gmail thông thường.
- Gửi SMTP dùng TLS cổng 587 và hủy sau 25 giây; báo riêng lỗi xác thực, mạng, địa chỉ nhận hoặc quá thời gian chờ. Phản hồi SMTP gốc không hiển thị lên form.
- Sau một lần gửi lỗi, chờ 60 giây trước khi thử lại vì yêu cầu có thể đã được ghi nhận trong SQL. Giới hạn 5 yêu cầu/giờ vẫn áp dụng.
- Email nhận phải là email bảo mật đã lưu trong Tài khoản, không phải chỉ email liên hệ trong hồ sơ Sinh viên. Địa chỉ Gmail gửi chỉ là người gửi, không tự nhận OTP thay người dùng.
