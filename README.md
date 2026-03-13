📦 Mini Inventory Management System (Fullstack)
Dự án quản lý kho bãi mini được thiết kế tập trung vào tính chính xác tuyệt đối của dữ liệu và quy trình nghiệp vụ tự động hóa, giúp loại bỏ các sai sót thủ công trong việc kiểm kê hàng hóa.

🚀 Technical Achievements (Thành quả kỹ thuật)
Gửi anh Tùng Leader, dựa trên những góp ý sát sao của anh về cấu trúc hệ thống, em đã thực hiện đợt Refactoring lớn để chuyển đổi dự án từ mức "chạy được" sang mức "chuẩn doanh nghiệp":

🏗️ Backend Architecture (C# .NET 8 & EF Core)
Service Layer Pattern: Đã triệt để tách Business Logic ra khỏi Controllers. Toàn bộ logic cộng/trừ tồn kho và kiểm tra nghiệp vụ hiện được xử lý tại InventoryService và ProductService.

Dependency Injection (DI): Áp dụng DI chuẩn mực thông qua Interface, giúp hệ thống lỏng lẻo về liên kết (Loose coupling) và dễ dàng viết Unit Test.

Atomic Transactions: Sử dụng BeginTransactionAsync để đảm bảo tính nhất quán dữ liệu (Data Consistency). Nếu một chi tiết hàng hóa lỗi, toàn bộ phiếu nhập/xuất sẽ được Rollback, tránh tình trạng sai lệch tồn kho.

SOLID Compliance: Tuân thủ nguyên lý Single Responsibility. Controllers hiện tại chỉ đóng vai trò "Lễ tân" điều hướng request.

Researching BaseService: Em đã nghiên cứu cấu trúc BaseService và BaseController như anh gợi ý để tối ưu hóa code và xử lý các sự kiện lặp lại trong tương lai.

🎨 Frontend Excellence (Angular 18+)
Real-time Dashboard: Tích hợp bộ 3 chỉ số quan trọng: KPI tổng quát, Báo cáo hàng sắp hết (Low-stock), và Lịch sử 10 giao dịch gần nhất (thời gian thực).

State Management: Sử dụng ChangeDetectorRef để đảm bảo UI luôn cập nhật chính xác ngay khi dữ liệu Backend thay đổi.

Smart Validation: Chặn xuất hàng âm ngay từ tầng giao diện và được bảo vệ lần 2 bằng Exception ở tầng Service.

🛠 Core Functions (Chức năng cốt lõi)
1. Xác thực & Bảo mật (Authentication)
Hệ thống bảo mật bằng JWT Token.

Tự động điều hướng (Guard) và quản lý phiên đăng nhập tại LocalStorage.

2. Quản lý Danh mục & Sản phẩm (Master Data)
Quản lý Categories và Products với quan hệ chặt chẽ.

Ràng buộc nghiệp vụ: Tuyệt đối không cho phép sửa tay số lượng tồn kho tại trang Sản phẩm để đảm bảo tính minh bạch.

3. Nghiệp vụ Kho (Transactions)
Nhập kho (Inbound): Tự động cộng dồn tồn kho và lưu vết lịch sử.

Xuất kho (Outbound): Kiểm tra tồn kho tức thời, đảm bảo không bao giờ xuất quá số lượng hiện có.

📂 Cấu trúc dự án
/MiniInventory.API: Mã nguồn Backend (Cleaned up & Service Layered).

/mini-inventory-ui: Mã nguồn Frontend Angular (Responsive & Interactive).

📝 Lời kết từ Developer
Dự án này là kết quả của việc bám sát yêu cầu nghiệp vụ thực tế và áp dụng các nguyên lý lập trình hiện đại. Em rất mong nhận được thêm những đánh giá chuyên môn từ anh Tùng để có thể hoàn thiện hơn nữa phần BaseService trong giai đoạn tiếp theo.
