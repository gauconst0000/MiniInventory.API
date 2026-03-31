# 📦 Mini Inventory Management System

## 🚀 Bản cập nhật tính năng Nâng cao

Bản cập nhật này bổ sung các tiện ích quản trị và giám sát cho hệ thống kho hàng. Toàn bộ kiến trúc được triển khai theo hướng mở rộng độc lập, đảm bảo **bảo toàn 100% tính toàn vẹn và sự ổn định của các tính năng CRUD** đã được hoàn thiện từ trước.

### ✨ Các tính năng mới tích hợp:

* **📝 Ghi log hệ thống (System Logging)**
    * **Công nghệ:** `Serilog`
    * **Mô tả:** Hệ thống tự động ghi nhận mọi luồng hoạt động, cảnh báo lỗi và lịch sử truy xuất API. Dữ liệu log được xuất tự động ra file `.txt` và phân loại theo từng ngày tại thư mục `Logs/`, giúp dễ dàng truy vết mà không ảnh hưởng đến hiệu năng.
* **📊 Xuất dữ liệu báo cáo (Excel Export)**
    * **Công nghệ:** `ClosedXML`
    * **Mô tả:** Xây dựng Endpoint API chuyên biệt (`GET /api/Products/export`) cho phép kết xuất toàn bộ danh sách sản phẩm hiện tại trong Database thành định dạng file `DanhSachSanPham.xlsx` chuẩn, phục vụ nhu cầu báo cáo thống kê nhanh chóng.
* **⚙️ Tác vụ chạy ngầm (Cronjob / Background Tasks)**
    * **Công nghệ:** `.NET BackgroundService`
    * **Mô tả:** Triển khai luồng dịch vụ chạy ngầm độc lập với tiến trình Web chính. Hệ thống tự động khởi chạy chu trình tuần tra, kiểm tra trạng thái kho hàng theo lịch trình định sẵn (hiện tại thiết lập chu kỳ demo là 10 giây/lần), các hoạt động đều được ghi nhận trực tiếp vào hệ thống Log chung.
