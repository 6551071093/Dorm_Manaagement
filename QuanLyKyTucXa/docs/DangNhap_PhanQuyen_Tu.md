# Đăng nhập và phân quyền phần Tú

## Cách sử dụng

Khi chạy ứng dụng, FrmLogin mở trước. Đăng nhập bằng tài khoản trong bảng NguoiDung.
Các tài khoản mẫu của project được ghi ở database/README.md.
Ứng dụng không tạo hoặc đặt lại mật khẩu của tài khoản đang có.

Sau khi đăng nhập, tên tài khoản và vai trò thật xuất hiện ở thanh trạng thái.
Hệ thống → Đổi mật khẩu dùng để đổi mật khẩu của chính mình.
Hệ thống → Đăng xuất đóng phiên hiện tại và trở lại màn hình đăng nhập.

## Quyền của từng vai trò

| Vai trò | Chức năng |
| --- | --- |
| Admin | Dashboard, các nghiệp vụ quản lý và Hệ thống → Quản lý tài khoản |
| NhanVien | Dashboard và các nghiệp vụ quản lý; không quản lý tài khoản hay cấp quyền |
| SinhVien | Thông tin cá nhân, phòng/hợp đồng và hóa đơn của mình, báo hỏng của mình, gửi/tra cứu đăng ký của mình |

Sinh viên không thấy danh sách hồ sơ, sơ đồ quản lý toàn bộ phòng, màn hình duyệt đơn hoặc Dashboard tổng hợp.
Phòng, hợp đồng, hóa đơn, sự cố của Sinh viên đều lọc theo hồ sơ liên kết với MaNguoiDung đang đăng nhập.

## Quản lý tài khoản

Admin có thể thêm tài khoản, sửa họ tên, vai trò, trạng thái và đặt mật khẩu mới.
Tên đăng nhập không thay đổi sau khi tạo.
Vai trò SinhVien bắt buộc liên kết với đúng một hồ sơ sinh viên chưa thuộc tài khoản khác.
Không thể khóa hoặc đổi vai trò của chính tài khoản Admin đang đăng nhập.
Không khóa hoặc hạ quyền Admin cuối cùng đang hoạt động.
Dùng khóa/mở khóa để giữ lịch sử tài khoản; không xóa tài khoản và lịch sử nghiệp vụ.

## Tích hợp mã

- Program.cs chạy FrmLogin rồi truyền AuthSession vào FrmMain.
- AuthService xác thực qua NguoiDungRepository.
- Security/AuthSession.cs chứa AuthPolicy và quyền theo vai trò.
- Các service SinhVienService, PhongGiuongService, DangKyKtxService nhận AuthSession ở constructor.
- Các form quản lý nhận connectionString và AuthSession từ Main.
- Mỗi truy cập database kiểm tra trạng thái tài khoản, vai trò và phiên; không chỉ ẩn menu.
- Duyệt/từ chối đơn ghi NguoiDuyet bằng ID tài khoản thật.
- Khi khóa, đổi vai trò hoặc đặt lại mật khẩu, phiên cũ bị từ chối ở lần kiểm tra tiếp theo.
- FrmCaNhan và PersonalService cung cấp khu vực Sinh viên có giới hạn theo tài khoản.
- FrmNguoiDung và NguoiDungService chỉ cho Admin sử dụng.

Không tạo entity hay migration mới; tiếp tục dùng schema database-first hiện có.
Các phân hệ quản lý hợp đồng/hóa đơn/sự cố còn chưa triển khai trước đó vẫn thuộc thành viên phụ trách.
Màn hình cá nhân chỉ đọc dữ liệu sẵn có; thao tác ghi mới chỉ gồm báo hỏng và đăng ký của chính sinh viên.

## Mật khẩu

Mật khẩu mới lưu bằng PBKDF2-HMAC-SHA256, 600000 vòng, salt ngẫu nhiên riêng.
Hash SHA256 của dữ liệu mẫu vẫn được xác minh và tự nâng cấp sau khi đăng nhập thành công.
Mật khẩu mới dài 8–128 ký tự. Mật khẩu không ghi vào log và không hiển thị trong bảng quản lý.
Nguồn thuật toán: https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
