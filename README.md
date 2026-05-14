# 🛍️ Hair Care E-Commerce Management System

> **Dự án:** Website Thương mại Điện tử Bán Sản phẩm Chăm sóc và Làm đẹp Tóc.

Một nền tảng web ứng dụng mô hình MVC cung cấp trải nghiệm mua sắm trực tuyến các sản phẩm chăm sóc tóc, đồng thời cung cấp cho Quản trị viên (Admin) và Nhân viên các công cụ quản lý bán hàng, theo dõi đơn hàng và thống kê doanh thu hiệu quả.

---

## 🚀 Công nghệ sử dụng
* **Backend:** C#, ASP.NET MVC (.NET 8.0)
* **ORM:** Entity Framework Core
* **Database:** Microsoft SQL Server
* **Frontend:** HTML, CSS, JavaScript
* **Bảo mật & Phân quyền:** ASP.NET Core Identity
* **Tích hợp bên thứ 3:** VNPAY API (Thanh toán), SMTP (Gửi Email tự động)

---

## ✨ Tính năng nổi bật

### 👤 Dành cho Khách hàng (Customer)
* **Xác thực người dùng:** Đăng nhập, Đăng ký, và Quên mật khẩu an toàn với tính năng xác thực qua Email tự động.
* **Mua sắm:** Tìm kiếm sản phẩm, lọc theo giá/danh mục, và xem chi tiết đánh giá sản phẩm.
* **Giỏ hàng & Thanh toán:** Quản lý giỏ hàng, đặt hàng trực tuyến với cổng thanh toán **VNPAY** hoặc Thanh toán khi nhận hàng (COD).
* **Quản lý tài khoản:** Theo dõi trạng thái đơn hàng, xem lịch sử mua hàng, cập nhật thông tin cá nhân và đánh giá sản phẩm.

### 🛡️ Dành cho Quản trị viên & Nhân viên (Admin/Staff)
* **Dashboard Thống kê:** Theo dõi số lượng đơn hàng, sản phẩm bán chạy, và biểu đồ thống kê doanh thu theo ngày.
* **Quản lý danh mục & Sản phẩm (CRUD):** Thêm, sửa, xóa, và cập nhật trạng thái các món hàng.
* **Quản lý Đơn hàng:** Theo dõi, phê duyệt và xử lý các đơn đặt hàng từ khách.
* **Quản lý Người dùng & Phân quyền:** Quản trị viên có quyền kiểm soát tài khoản khách hàng, nhân viên và phân quyền linh hoạt (Admin, Nhân viên, Khách hàng).
* **Quản lý Đánh giá:** Quản lý bình luận/đánh giá từ phía người dùng.

---

## 🛠️ Hướng dẫn cài đặt (Local Setup)

Để chạy dự án này trên môi trường local của bạn, vui lòng làm theo các bước sau:

**1. Clone repository**
```bash
git clone https://github.com/Username_cua_ban/Ten_Repository_cua_ban.git
```

**2. Mở dự án bằng Visual Studio**
* Yêu cầu đã cài đặt **.NET 8.0 SDK** và **SQL Server**.
* Mở file `.sln` bằng Visual Studio.

**3. Cập nhật Chuỗi kết nối (Connection String)**
* Mở file `appsettings.json`.
* Tìm cấu hình `DefaultConnection` và thay đổi chuỗi kết nối, địa chỉ mail và key vnpay sao cho phù hợp với SQL Server local của bạn.

**4. Khởi tạo Cơ sở dữ liệu (Migration)**
Mở **Package Manager Console** trong Visual Studio và chạy lệnh sau:
```bash
Update-Database
```

**5. Chạy dự án**
* `Run` trên Visual Studio để khởi động ứng dụng.

---

## 👤 Tác giả
* **Trương Công Huy** - *Full-stack Developer*
