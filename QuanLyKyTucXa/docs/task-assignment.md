# HƯỚNG DẪN TRIỂN KHAI ĐỒ ÁN - QUẢN LÝ KÝ TÚC XÁ
## Quy trình Database-First • Phân công 5 thành viên • GitHub & Jira • Kế hoạch 14 ngày (17/10/2026 – 30/10/2026)
**Nhóm thực hiện:** Tú (Lead) • Bổn • Bảo • Quy • Đạt  
**Học phần:** Lập trình trực quan (.NET 9 / C# 12-13 / Windows Forms / SQL Server / EF Core Database First)

---

> [!IMPORTANT]
> **QUYẾT ĐỊNH KỸ THUẬT QUAN TRỌNG:**  
> Dự án chính thức áp dụng mô hình **Entity Framework Core Database-First**. SQL Server là cội nguồn của mọi dữ liệu (*Single Source of Truth*). Thực thể C# và `AppDbContext` được tự động sinh bằng lệnh `Scaffold-DbContext`, tuyệt đối không tạo Entity thủ công hoặc viết migration tay. Cấu trúc mã nguồn đã được đưa về chung thư mục `src/` (loại bỏ lồng ghép `apps/desktop/`) để tối ưu đường dẫn và đồng bộ hoàn hảo với Visual Studio 2022+.

---

## 1. Mục tiêu và Nguyên tắc Vận hành 14 Ngày

* **Học phần:** Lập trình trực quan (LTTQ) – Nền tảng Windows Forms (.NET 9 / C# 12-13 / SQL Server / EF Core & ADO.NET).
* **Mục tiêu cốt lõi:** Xây dựng hoàn chỉnh một ứng dụng Desktop chuyên nghiệp đạt chuẩn điểm tối đa (10/10), có khả năng build độc lập, chạy mượt mà, phân quyền chặt chẽ và dữ liệu được bảo toàn tính toàn vẹn cao nhất.

### Các nguyên tắc làm việc sống còn của nhóm:
1. **Database-First là kim chỉ nam:** Mọi thay đổi bảng/cột, kiểu dữ liệu, ràng buộc phải thực hiện trong các script SQL (`database/scripts/`) và chạy `Scaffold-DbContext` để cập nhật Entities. Tuyệt đối không sửa tay trong thư mục `Domain/Entities`.
2. **Bảo toàn mã nguồn qua Partial Classes:** Các thuộc tính tính toán, logic bổ trợ cho Form phải viết trong `Domain/Partials/` dưới dạng `partial class` để không bị mất khi chạy lại Scaffold.
3. **Quy chế nhánh Git nghiêm ngặt:** Nhánh `main` là nhánh phát hành ổn định, luôn luôn build được (0 Error). Mỗi thành viên chỉ làm việc trên nhánh chức năng (`feature/*`), nộp code qua Pull Request (PR) và phải có ít nhất 1 thành viên khác duyệt.
4. **Minh bạch trên Jira Board:** Mỗi công việc được quản lý bởi 1 Jira Task (`https://studentdorm.atlassian.net/jira/software/projects/KAN/boards/2`). Chỉ chuyển trạng thái 'Done' khi chức năng đã chạy thực tế với CSDL; giao diện tĩnh không được tính là hoàn thành.
5. **Báo cáo vướng mắc tức thì:** Nếu gặp lỗi khó kéo dài quá 45 phút, phải thông báo ngay lên nhóm kèm ảnh chụp lỗi, tên file và thao tác tái hiện để các thành viên khác hỗ trợ.

---

## 2. Cấu trúc Dự án Clean Architecture Chuẩn hóa

Cấu trúc mã nguồn được phân chia thành 4 project .NET độc lập trong thư mục `src/`:

| Project .NET | Trách nhiệm & Nội dung | Người phụ trách chính |
|---|---|---|
| **QuanLyKyTucXa.WinForms** *(Presentation Layer)* | Giao diện người dùng: `MainForm` điều hướng MenuStrip, Forms chức năng theo module, UserControls, DataGridView binding, ViewModels, Helpers, Icons. | Cả nhóm *(Chia theo Form)* |
| **QuanLyKyTucXa.Application** *(Business Logic Layer)* | Xử lý logic nghiệp vụ, Services (`Auth`, `SinhVien`, `PhongGiuong`, `HopDong`, `DienNuoc`, `HoaDon`, `SuCo`, `ThongKe`), DTOs trao đổi dữ liệu, Validators, Reports. | Cả nhóm *(Chia theo Service)* |
| **QuanLyKyTucXa.Infrastructure** *(Data Access Layer)* | Triển khai truy cập CSDL: `AppDbContext` (Scaffold), Repositories, thư mục `AdoNet/` (Chương 7), Import/Export ClosedXML, Logging Serilog, VietQR API. | Tú (Lead) + Đạt (AdoNet) |
| **QuanLyKyTucXa.Domain** *(Domain Core Layer)* | Entities sinh tự động từ SQL Server (thư mục `Entities/` - KHÔNG sửa tay); thư mục `Partials/` chứa logic mở rộng; Enums ánh xạ trạng thái. | Tú (Scaffold) + Tất cả (Partials) |

---

## 3. Ma trận Phân công 5 Thành viên & Đầu ra Bắt buộc

| Thành viên | Phân hệ đảm nhiệm | Mã nguồn bàn giao cụ thể | Tiêu chuẩn nghiệm thu (DoD) |
|---|---|---|---|
| **TÚ** *(Trưởng nhóm)* | Kiến trúc, CSDL & Phân quyền | • `QuanLyKTXDB` (4 file script SQL chuẩn hóa)<br>• Chạy Scaffold EF Core & cấu hình `AppDbContext`<br>• `AuthService`, `NguoiDungRepository`<br>• `LoginForm`, `MainForm`, `FrmNguoiDung`<br>• Điều hướng MenuStrip phân quyền Admin/Nhân viên | Solution build 0 error; đăng nhập đúng 2 role; phân quyền menu ẩn/hiện chuẩn xác; chạy được trên máy khác. |
| **BỔN** | Quản lý Sinh viên & Đăng ký KTX | • `SinhVienService`, `DangKyKtxService`<br>• `FrmSinhVien` (CRUD, tra cứu, lọc khoa/lớp)<br>• `FrmDangKyKtx` (tiếp nhận xét duyệt đơn)<br>• Import/Export Excel hồ sơ SV (ClosedXML) | CRUD sinh viên mượt mà; validation CCCD/SĐT không trùng; duyệt đơn đăng ký chuyển trạng thái chuẩn xác; xuất Excel không đơ UI. |
| **BẢO** | Phòng, Giường & Hợp đồng lưu trú | • `PhongGiuongService`, `HopDongService`<br>• `FrmToaNha`, `FrmPhong`, `FrmGiuong` (sơ đồ giường)<br>• `FrmHopDong` (ký hợp đồng, gia hạn, trả phòng)<br>• Nghiệp vụ chuyển phòng/đổi giường | Sơ đồ trực quan phòng/giường; chặn xếp vượt quá sức chứa; khi ký hoặc đổi giường tự động cập nhật trạng thái giường và sĩ số phòng. |
| **QUY** | Biểu phí, Điện nước, Hóa đơn & Thu tiền | • `DienNuocService`, `HoaDonService`<br>• `FrmBangGia`, `FrmChiSoDienNuoc` (nhập công tơ)<br>• `FrmHoaDon` (tự động chia điện nước theo phòng)<br>• `FrmThanhToan` (thu tiền, VietQR API) | Chốt chỉ số điện nước chuẩn; tự động tạo hóa đơn cá nhân chia sẻ điện nước phòng; thanh toán cập nhật công nợ tức thì; có mã VietQR. |
| **ĐẠT** | Sự cố, Thống kê Dashboard & Báo cáo | • `SuCoService`, `ThongKeService`<br>• `FrmSuCo` (tiếp nhận phản ánh, chi phí sửa)<br>• `DashboardForm` (LiveCharts2/GDI+ từ CSDL thật)<br>• `FrmBaoCao` & In ấn (`PrintPreviewDialog`)<br>• Phối hợp minh chứng ADO.NET thuần | Biểu đồ tỷ lệ lấp đầy & doanh thu 12 tháng phản ánh dữ liệu thật từ SQL Server; in ấn hóa đơn có xem trước; hoàn thành code ADO.NET thuần. |

---

## 4. Quy trình Vận hành Kỹ thuật Chuẩn Database-First

1. **Bước 1: Thiết kế & Cập nhật CSDL trong SSMS:** Tú chịu trách nhiệm quản lý 4 file script trong `database/scripts/`. Bổ sung đầy đủ khóa chính (PK), khóa ngoại (FK), chỉ mục (Indexes), Filtered Unique Indexes và các ràng buộc `CHECK`. Chạy kiểm thử trong SSMS hoặc dòng lệnh `sqlcmd -f 65001`.
2. **Bước 2: Tự động hóa Scaffolding EF Core:** Khi CSDL có thay đổi, chạy lệnh `Scaffold-DbContext` (hoặc script `scripts/dev/run-scaffold.ps1`). EF Core sẽ quét schema CSDL và sinh đè toàn bộ các Entity vào `src/QuanLyKyTucXa.Domain/Entities` và `AppDbContext` trong `src/QuanLyKyTucXa.Infrastructure/Data/Context`.
3. **Bước 3: Mở rộng thực thể an toàn bằng Partial Class:** Các thành viên tuyệt đối không viết thêm code vào các file Entity tự sinh. Mọi thuộc tính tính toán (như `HoTen`, `SoChoTrong`) phải được đặt trong `src/QuanLyKyTucXa.Domain/Partials/` dưới dạng `public partial class` để vĩnh viễn không bị mất khi Scaffold lại.
4. **Bước 4: Xây dựng Service (BLL) & Repository (DAL):** Viết Interface và Service trong `QuanLyKyTucXa.Application`. Dữ liệu trao đổi với WinForms được đóng gói qua các class DTO (Data Transfer Object). Các thao tác dữ liệu được thực hiện qua LINQ to Entities với cơ chế bất đồng bộ (`async/await`, `ToListAsync()`, `SaveChangesAsync()`).
5. **Bước 5: Thiết kế Giao diện WinForms & Data Binding:** Thiết kế Form trong `QuanLyKyTucXa.WinForms/Forms/<Module>/`. Đăng ký Form và Service trong `Program.cs` qua Dependency Injection. Gọi Service bất đồng bộ, nạp dữ liệu lên DataGridView, xử lý sự kiện qua `ErrorProvider` và thông báo người dùng thân thiện.

---

## 5. Quy chuẩn GitHub & Jira Board

* **Jira Board:** [https://studentdorm.atlassian.net/jira/software/projects/KAN/boards/2](https://studentdorm.atlassian.net/jira/software/projects/KAN/boards/2)

| Hạng mục | Quy ước bắt buộc | Ví dụ thực tế |
|---|---|---|
| **Nhánh chính (main)** | Chỉ chứa mã nguồn ổn định, đã qua kiểm thử và build thành công. | Nhánh gốc bảo vệ, cấm push trực tiếp. |
| **Nhánh tính năng (feature)** | Tạo từ main mới nhất, đặt tên theo cấu trúc: `feature/<thành-viên>-<tên-chức-năng>` | `feature/bon-student-crud`<br>`feature/bao-room-contract`<br>`feature/quy-billing-invoice`<br>`feature/dat-incident-dashboard` |
| **Nhánh sửa lỗi (fix)** | Dành cho sửa lỗi phát sinh trong quá trình tích hợp: `fix/<mô-tả-lỗi>` | `fix/invoice-negative-debt`<br>`fix/contract-room-sync` |
| **Quy chuẩn Commit** | Tuân thủ Conventional Commits: `<type>(scope): <mô tả ngắn>` | `feat(resident): add export excel for students`<br>`fix(billing): handle room utility calculation` |
| **Pull Request (PR)** | Mô tả rõ: Mục tiêu, các file đã thay đổi, kịch bản đã test và link Task Jira liên quan. | Tiêu đề PR: `[KAN-12] Hoàn thiện CRUD Sinh viên & Export Excel`<br>Cần ít nhất Tú hoặc 1 peer review trước khi merge. |

---

## 6. Kế hoạch Hành động Chi tiết 14 Ngày (17/10 – 30/10/2026)

### Giai đoạn A (17/10 – 20/10): Nền tảng CSDL & Kiến trúc
* **17/10 (Tất cả):** Họp chốt yêu cầu, tài liệu Database First, phân chia Jira Board và thống nhất quy ước Git.
* **18/10 (Tú + Nhóm):** Tú khởi tạo Solution 4 project, thiết lập DI, Serilog, và `MainForm` khung điều hướng. Các bạn tạo branch cá nhân.
* **19/10 (Tú):** Hoàn thiện 4 script SQL (`01`..`04`), nạp dữ liệu mẫu và chạy thử nghiệm `Scaffold-DbContext` sinh Entities.
* **20/10 (Cả nhóm):** Pull nhánh `main` đã có Entities và DbContext về máy, mở Solution bấm `Ctrl+Shift+B` đảm bảo 0 error.

### Giai đoạn B (21/10 – 26/10): Phát triển Độc lập 5 Phân hệ Song song
* **21/10 (Bổn, Bảo):** Bổn dựng `FrmSinhVien`; Bảo dựng `FrmToaNha`, `FrmPhong`. Bắt đầu viết Service CRUD cơ bản.
* **22/10 (Quy, Đạt):** Quy dựng `FrmBangGia`, `FrmChiSoDienNuoc`; Đạt dựng `FrmSuCo` và khung `DashboardForm`.
* **23/10 (Tú):** Hoàn thiện `AuthService`, mã hóa mật khẩu, phân quyền MenuStrip và quản lý tài khoản người dùng.
* **24/10 (Bảo, Quy):** Bảo làm `FrmHopDong` (chuyển/trả phòng); Quy hoàn thiện tính năng tự động chia điện nước và lập hóa đơn cá nhân.
* **25/10 (Bổn, Đạt):** Bổn làm tiếp nhận xét duyệt Đăng ký KTX; Đạt hoàn thiện biểu đồ tỷ lệ lấp đầy phòng và doanh thu 12 tháng.
* **26/10 (Cả nhóm):** Gửi toàn bộ Pull Request của giai đoạn B lên GitHub để review chéo và chuẩn bị tích hợp.

### Giai đoạn C (27/10 – 30/10): Tích hợp Toàn hệ thống, Nghiệm thu & Đóng gói
* **27/10 (Tất cả):** Tích hợp toàn diện: Chạy luồng xuyên suốt từ Sinh viên $\rightarrow$ Đăng ký $\rightarrow$ Xếp phòng $\rightarrow$ Hợp đồng $\rightarrow$ Điện nước $\rightarrow$ Hóa đơn $\rightarrow$ Thanh toán.
* **28/10 (Đạt, Quy):** Đạt hoàn thiện module Báo cáo In ấn (`PrintPreviewDialog`) và chức năng ADO.NET thuần Chương 7; Quy hoàn thiện VietQR API.
* **29/10 (Cả nhóm):** Đóng băng tính năng (*Feature Freeze*). Chỉ sửa bug giao diện, kiểm thử dữ liệu biên, đóng gói publish self-contained.
* **30/10 (Cả nhóm):** Tổng duyệt kịch bản demo 3-5 phút, phân công thuyết trình, xuất file backup `QuanLyKTX.bak` và hoàn thiện báo cáo Word.

---

## 7. Bảng Đối chiếu Điểm Tối đa (Rubric 10/10 - Chương 1 đến 9)

| Chương | Nội dung kỹ thuật yêu cầu | Điểm | Vị trí minh chứng thực tế trong Repository |
|---|---|:---:|---|
| **Ch. 1-2** | Kiến trúc .NET 9, Clean Architecture, OOP, LINQ, Async/Await | 1.0 | Phân tầng rõ rệt 4 project; Service, Repository, DTO; LINQ Where, GroupBy; gọi async ToListAsync(), SaveChangesAsync(). |
| **Ch. 3** | Controls, DataGridView, Điều hướng Desktop MDI/MenuStrip, DPI | 1.0 | MainForm điều hướng MenuStrip/ToolStrip; hơn 8 loại controls khác nhau; DataGridView binding CSDL thật; hỗ trợ DPI. |
| **Ch. 4** | Sự kiện, Validation dữ liệu, ErrorProvider, Phím tắt | 1.0 | Bắt lỗi nhập liệu chặt chẽ bằng ErrorProvider; phím tắt thao tác nhanh (Ctrl+S, F5); tìm kiếm realtime có debounce. |
| **Ch. 5** | I/O, Excel ClosedXML, Tác vụ nền Async & ProgressBar | 1.0 | Xuất/Nhập danh sách sinh viên ra Excel bằng ClosedXML chạy nền (Task.Run), hiển thị tiến độ ProgressBar không đơ UI. |
| **Ch. 6** | Trực quan hóa dữ liệu, GDI+ / LiveCharts2 Dashboard | 0.5 | DashboardForm hiển thị biểu đồ tỷ lệ lấp đầy phòng và doanh thu 12 tháng lấy từ CSDL thật (sp_ThongKeDashboard). |
| **Ch. 7** | CSDL: EF Core Database First & Minh chứng ADO.NET thuần | 2.0 | EF Core CRUD toàn diện 13 bảng; thư mục Infrastructure/AdoNet/ chạy Stored Procedure độc lập để đối chiếu kỹ thuật. |
| **Ch. 8** | Báo cáo, Xuất file & Xem trước khi in (PrintPreviewDialog) | 1.5 | Xuất hóa đơn tiền phòng/điện nước và hợp đồng ra file PDF/Excel theo bộ lọc; xem trước khi in bằng PrintPreviewDialog. |
| **Ch. 9** | Đóng gói Publish Self-contained, Logging Serilog & REST API | 1.0 | Publish bản chạy độc lập .exe không cần cài .NET SDK; Serilog ghi log lỗi; tích hợp VietQR API tạo mã thanh toán động. |
| **Bảo vệ** | Hồ sơ đồ án, Báo cáo Word, Video demo & Trả lời phản biện | 1.0 | Báo cáo Word chỉn chu, ảnh chụp minh họa thực tế; phân vai demo 3-5 phút mạch lạc; giải thích rõ quyết định thiết kế. |
| **TỔNG** | **Điểm tối đa** | **10.0** | Dùng checklist nghiệm thu và kịch bản demo để rà soát trước khi bảo vệ |

---

## 8. Kịch bản Demo 3–5 Phút Khi Báo cáo Trước Giảng Viên

1. **Phút 1: Khởi động & Phân quyền người dùng (Tú trình bày):**
   * Đăng nhập bằng tài khoản Sinh viên: Chỉ thấy các chức năng cá nhân (xem phòng, hóa đơn của mình, gửi báo hỏng).
   * Đăng xuất và đăng nhập bằng tài khoản Quản trị viên (`admin`): MenuStrip tự động mở rộng đầy đủ các phân hệ quản lý.
2. **Phút 2: Dashboard thống kê thời gian thực & Hồ sơ sinh viên (Đạt & Bổn):**
   * Mở `DashboardForm`: Thuyết minh số liệu thật từ SQL Server (tỷ lệ lấp đầy phòng, doanh thu tháng, biểu đồ cột 12 tháng).
   * Mở `FrmSinhVien`: Thực hiện tìm kiếm nhanh theo MSSV/Khoa, demo tính năng xuất danh sách ra file Excel có ProgressBar.
3. **Phút 3: Tiếp nhận đăng ký, Xếp giường & Ký hợp đồng lưu trú (Bổn & Bảo):**
   * Bổn mở `FrmDangKyKtx` duyệt đơn đăng ký của sinh viên mới.
   * Bảo mở sơ đồ phòng `FrmGiuong`: Chọn giường trống (`A1-101-G4`) và bấm 'Ký hợp đồng'. Cho giảng viên thấy giường lập tức chuyển sang màu đỏ ('Đang sử dụng') và sĩ số phòng tăng lên tự động nhờ Trigger DB.
4. **Phút 4: Ghi điện nước, Tự động lập hóa đơn & Thanh toán VietQR (Quy):**
   * Quy mở `FrmChiSoDienNuoc`: Nhập số điện và nước tiêu thụ của phòng A1-101.
   * Bấm 'Tự động tính hóa đơn': Hệ thống tự động lấy tổng điện nước chia cho số sinh viên đang ở và tạo hóa đơn cá nhân.
   * Mở `FrmThanhToan`: Hiện mã QR thanh toán động VietQR và xác nhận thu tiền, công nợ lập tức về 0.
5. **Phút 5: Xử lý sự cố, Đối chiếu ADO.NET vs EF Core & Kết luận (Đạt & Tú):**
   * Đạt mở `FrmSuCo`: Xem yêu cầu báo hỏng vòi sen, cập nhật thợ xử lý và chi phí sửa chữa.
   * Tú mở code minh họa sự khác biệt giữa EF Core (CRUD hàng ngày) và ADO.NET thuần (thực thi Stored Procedure báo cáo Chương 7).
   * Mở thư mục Publish cho thấy ứng dụng chạy độc lập (Self-contained) và nhật ký Serilog hoạt động chuẩn mực.
