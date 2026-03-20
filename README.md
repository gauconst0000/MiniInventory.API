# 📦 Mini Inventory Management System

Một hệ thống Quản lý Kho Tổng hợp Full-stack được thiết kế tối giản, hiệu quả và bảo mật. Dự án cung cấp giải pháp theo dõi hàng hóa, cảnh báo tồn kho và quản lý giao dịch xuất/nhập kho theo thời gian thực.

Trong quá trình phát triển và nâng cấp, hệ thống luôn đặt **tính toàn vẹn của các tính năng đã hoàn thiện** lên hàng đầu, đảm bảo mọi bản cập nhật mới không làm ảnh hưởng đến sự ổn định của core logic.

## 🚀 Công nghệ sử dụng

**Frontend (Giao diện người dùng):**
* Framework: Angular
* Thiết kế: Bootstrap 5 (Responsive, UI/UX thân thiện)
* Kiến trúc: Áp dụng Standalone Components và Shared Components để tối ưu hóa việc tái sử dụng code.
* Quản lý Form: Reactive Forms kết hợp Validation chặt chẽ.
* Cấu hình: Quản lý API qua `environment.ts`.

**Backend (Máy chủ & API):**
* Framework: C# .NET Core Web API
* Kiến trúc: Service Pattern (Bóc tách Business Logic khỏi Controller).
* Bảo mật: Xác thực bằng JWT Token & Mã hóa mật khẩu người dùng bằng thư viện BCrypt.

## ✨ Tính năng nổi bật

* **📊 Dashboard Thống kê:** Hiển thị tổng quan số lượng mặt hàng, tồn kho tổng, cảnh báo hàng sắp hết hạn/tồn kho thấp và lịch sử 10 giao dịch gần nhất.
* **🛍️ Quản lý Sản phẩm:** Thêm, sửa, xóa sản phẩm mượt mà thông qua Shared Component UI (Form Modal) dùng chung.
* **📥/🚀 Nhập & Xuất kho:** Quản lý luồng hàng hóa ra vào chặt chẽ.
* **🔐 Xác thực an toàn:** Đăng nhập/Đăng xuất bảo mật với Token.

## 🛠️ Hướng dẫn cài đặt và Chạy dự án

Dự án được cấu trúc theo dạng Monorepo, bao gồm cả Backend (`MiniInventory.API`) và Frontend (`mini-inventory-ui`).

### 1. Khởi động Backend (.NET)
1. Mở thư mục `MiniInventory.API` bằng Visual Studio.
2. Kiểm tra chuỗi kết nối cơ sở dữ liệu (Connection String) trong file `appsettings.json`.
3. Chạy lệnh `Update-Database` trong Package Manager Console (nếu dùng Entity Framework).
4. Nhấn `F5` hoặc nút Run để khởi động API Server.

### 2. Khởi động Frontend (Angular)
1. Mở Terminal, di chuyển vào thư mục Frontend:
   ```bash
   cd mini-inventory-ui
Cài đặt các thư viện cần thiết:

Bash
npm install
Khởi chạy Server phát triển:

Bash
ng serve
Truy cập ứng dụng tại: http://localhost:4200

📝 Nhật ký cập nhật (Changelog) gần nhất
Refactor Backend: Chuyển đổi toàn bộ xử lý nghiệp vụ sang Service Pattern.

Security: Nâng cấp thuật toán băm mật khẩu với BCrypt.

Refactor Frontend: Xây dựng app-product-form làm Shared Component. Tích hợp Reactive Forms để validate dữ liệu đầu vào. Tách cấu hình domain API vào file environment.
