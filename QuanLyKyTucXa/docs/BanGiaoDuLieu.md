# Bàn giao giao diện và dữ liệu qua Git

## Người tải code sẽ thấy gì?

- Giao diện, sự kiện và chức năng đi theo mã nguồn. Người nhận pull code mới rồi build/chạy `QuanLyKyTucXa.WinForms`.
- Hồ sơ sinh viên, phòng, đơn đã duyệt và hóa đơn nằm trong SQL Server. Pull Git không tự lấy dữ liệu SQL trên máy Bổn.
- `database/snapshots/LatestData.sql` là bản dữ liệu đã xuất để người nhận nhập vào database demo của họ. Đây là dữ liệu tại thời điểm xuất, không phải đồng bộ trực tiếp.

## Bổn cập nhật dữ liệu cho nhóm

1. Thêm/sửa/duyệt trong ứng dụng, chờ thông báo lưu thành công.
2. Đăng nhập Admin, chọn **Hệ thống > Lưu dữ liệu bàn giao**.
3. Lưu vào `database/snapshots/LatestData.sql` trong repository hiện tại, thay bản bàn giao cũ.
4. Xem lại nội dung trước khi chia sẻ; commit/push bản SQL này cùng các file code đã sửa. Không đưa thư mục bin/obj hoặc mật khẩu kết nối lên Git.

**Sau mỗi đợt thay đổi dữ liệu phải xuất và push lại.** Dữ liệu mới sau lần xuất cuối không nằm trong Git.

## Người nhận nhập bản dữ liệu

### Máy chưa có database

1. Cài SQL Server và kết nối SSMS bằng instance trên máy nhận.
2. Chỉ trên database mới, chạy các script trong `database/scripts`: **01 → 02 → 03 → 05 → 07**. Không cần 04 vì sẽ dùng LatestData.
3. Trong SSMS mở `database/snapshots/LatestData.sql`, chọn **QuanLyKTXDB** ở danh sách database trên thanh công cụ rồi Execute.
4. Cấu hình instance cục bộ bằng `appsettings.local.json` như ví dụ dưới; build và chạy WinForms. Đăng nhập **admin / Admin@123** nếu tài khoản admin đang hoạt động trong bản nguồn.

### Máy đã có database

Sao lưu database của máy nhận trước. Chạy bổ sung **05_UpdateSchema_Catalog.sql** và **07_UpdateSchema_RoomAssetsHandover.sql** nếu thiếu, rồi chọn database demo cần thay và Execute LatestData.

**LatestData thay thế dữ liệu của database đang chọn, không ghép dữ liệu hai máy.** Script dùng transaction để hoàn tác khi nhập lỗi. Không chạy trên database chứa dữ liệu riêng cần giữ. Không chạy lại 01 hoặc 04 trên database đang sử dụng: chúng xóa/tạo hoặc nạp lại dữ liệu.

### Cấu hình riêng máy nhận

Tạo `src/QuanLyKyTucXa.WinForms/appsettings.local.json` (đã được Git bỏ qua):

```json
{
  "ConnectionStrings": {
    "KtxDb": "Server=.\\SQLEXPRESS;Database=QuanLyKTXDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Thay `.\\SQLEXPRESS` theo instance thật. File này được copy khi build. Biến môi trường `ConnectionStrings__KtxDb`, nếu có, được ưu tiên hơn cả hai file JSON. Người dùng ứng dụng không cần nhập cấu hình SQL trên màn hình Login.

## Phạm vi bản bàn giao

Xuất 16 bảng nghiệp vụ/danh mục; giữ mã hồ sơ, tên, khoa/lớp, phòng, giường, đơn đăng ký, hợp đồng, điện nước, hóa đơn, thanh toán và trạng thái. Danh mục hiện có 4 khoa và 144 lớp mô phỏng; mã lớp mô phỏng không phải danh sách lớp chính thức UTC2.

Để bàn giao demo, CCCD, số điện thoại, email, quê quán và ngày sinh được thay bằng dữ liệu demo. Mật khẩu gốc và mã khôi phục không xuất. Mật khẩu trong bản nhận được đổi theo vai trò:

| Vai trò | Mật khẩu demo |
|---|---|
| Admin | `Admin@123` |
| Nhân viên | `NhanVien@123` |
| Sinh viên | `SinhVien@123` |

Tên tài khoản và trạng thái hoạt động giữ nguyên. Bộ đếm sai đăng nhập và mã khôi phục được xóa khi nhập; không chia sẻ mã khôi phục gốc. Bản này không dùng mật khẩu `123456`. Đổi mật khẩu demo trước khi triển khai thực tế.

Họ tên và nội dung ghi chú/mô tả vẫn giữ nguyên: cần xem lại trước khi đưa lên repository công khai. Đây không phải bản ẩn danh hoàn toàn, cũng không phải bản sao lưu phục hồi đầy đủ. Database nguồn và mật khẩu nguồn không bị thay bởi thao tác xuất.

## Xuất bằng công cụ dòng lệnh (tùy chọn cho người phát triển)

Chạy từ thư mục project có `src`, `database`, `tools`:

```powershell
dotnet run --project tools/QuanLyKyTucXa.DataHandoff -- --project-root . --output database/snapshots/LatestData.sql
```

Công cụ hỏi tài khoản Admin và mật khẩu, không hiển thị mật khẩu khi nhập. Dùng cùng appsettings/local và biến môi trường như WinForms. Không đặt mật khẩu trong tham số lệnh hoặc file đưa lên Git.

## Nếu muốn mọi người thấy thay đổi ngay

Cần một database chung có máy chủ online hoặc VPN. Hiện nhóm chưa có máy chủ/VPN nên bản bàn giao qua Git cần bước xuất/nhập trên. Xem `DungChungDatabase_QuaVPN.md` khi nhóm chuẩn bị được hạ tầng. Chưa publish/đóng gói EXE trong thay đổi này.
