# Danh mục khoa/ngành/lớp tham khảo UTC2

Nguồn đã đối chiếu ngày 23/10/2026:
- https://utc2.edu.vn/chuong-trinh-dao-tao
- https://utc2.edu.vn/gioi-thieu/cac-khoa-bo-mon

## Dữ liệu bổ sung
- 4 khoa: Công trình; Kinh tế - Vận tải; Kỹ thuật Công nghệ; Khoa học cơ bản.
- 18 ngành theo trang chương trình đào tạo.
- 144 mã lớp mẫu: 18 ngành × 4 khóa K64–K67 × 2 lớp/khóa.
- Các mã lớp và cách gán ngành vào khoa trong đồ án phục vụ mô phỏng, không khẳng định là danh sách lớp/phân công ngành chính thức của UTC2.
- Khoa Khoa học cơ bản có trong danh sách khoa nhưng không được gán ngành/lớp đào tạo chuyên ngành khi nguồn không cung cấp.

Script `database/scripts/05_UpdateSchema_Catalog.sql` chạy lại được và không thay đổi hồ sơ sinh viên hiện có.
Các khoa/lớp cũ của hồ sơ vẫn hiển thị trong bộ lọc bên cạnh danh mục tham khảo.
Danh mục hiển thị ngay cả khi chưa có sinh viên thuộc khoa/lớp đó. Chọn danh mục chưa có hồ sơ sẽ trả về 0 sinh viên.

## Form sinh viên
- Bộ lọc khoa/lớp lấy cả danh mục lẫn hồ sơ hiện có.
- Thêm/sửa hồ sơ: Khoa và Lớp chuyển thành ComboBox có gợi ý. Vẫn cho nhập dữ liệu riêng để không hạn chế lớp thực tế của nhóm.
- Dữ liệu được lưu SQL Server như trước. Không tự thêm sinh viên, CCCD hay số điện thoại mô phỏng.