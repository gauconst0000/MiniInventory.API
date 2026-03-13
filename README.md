# 📦 Mini Inventory Management System (Fullstack)

> **Hệ thống quản lý kho bãi mini** tập trung vào tính chính xác tuyệt đối của dữ liệu và quy trình nghiệp vụ tự động hóa, giúp loại bỏ các sai sót thủ công trong việc kiểm kê hàng hóa.

---

## 🚀 Technical Achievements (Thành quả kỹ thuật)
*Gửi anh Tùng Leader, dựa trên những góp ý sát sao của anh về cấu trúc hệ thống, em đã thực hiện đợt Refactoring lớn để chuyển đổi dự án từ mức "chạy được" sang mức "chuẩn doanh nghiệp":*

### 🏗️ Backend Architecture (.NET 8 & EF Core)
* **Service Layer Pattern:** Triệt để tách Business Logic ra khỏi Controllers. Toàn bộ logic tính toán tồn kho được xử lý tại `InventoryService` và `ProductService`.
* **Dependency Injection (DI):** Áp dụng DI chuẩn mực thông qua Interface, giúp hệ thống lỏng lẻo về liên kết (Loose coupling) và dễ dàng viết Unit Test.
* **Atomic Transactions:** Sử dụng `BeginTransactionAsync` để đảm bảo tính nhất quán dữ liệu. Nếu một chi tiết hàng hóa lỗi, toàn bộ phiếu nhập/xuất sẽ được Rollback.
* **SOLID Compliance:** Tuân thủ nguyên lý *Single Responsibility*. Controllers hiện tại chỉ đóng vai trò "Lễ tân" điều hướng request.
* **Researching BaseService:** Đã nghiên cứu cấu trúc `BaseService` để tối ưu hóa code và xử lý các sự kiện lặp lại trong tương lai.

### 🎨 Frontend Excellence (Angular 18+)
* **Real-time Dashboard:** Tích hợp bộ 3 chỉ số quan trọng: KPI tổng quát, Báo cáo hàng sắp hết (Low-stock), và Lịch sử 10 giao dịch gần nhất.
* **State Management:** Sử dụng `ChangeDetectorRef` để đảm bảo UI luôn cập nhật chính xác ngay khi dữ liệu Backend thay đổi.

---

## 🛠 Tech Stack & Tools
| Thành phần | Công nghệ sử dụng |
| :--- | :--- |
| **Backend** | .NET 8, Web API, Entity Framework Core, SQL Server |
| **Frontend** | Angular 18+, Bootstrap 5, RxJS |
| **Bảo mật** | JWT (JSON Web Token) |
| **Công cụ** | Swagger, Git, VS Code, Visual Studio 2022 |

---

## 📂 Cấu trúc dự án
```text
MiniInventory-Fullstack/
├── MiniInventory.API/   # Source code Backend (Clean Architecture)
└── mini-inventory-ui/   # Source code Frontend (Reactive Forms)
