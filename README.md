# 🍽️ Restaurant Management System - Backend API (RBE)

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?logo=nuget)](https://docs.microsoft.com/ef/core/)
[![MySQL](https://img.shields.io/badge/Database-MySQL-4479A1?logo=mysql&logoColor=white)](https://www.mysql.com/)
[![SignalR](https://img.shields.io/badge/Realtime-SignalR-blue)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A robust, enterprise-ready **Restaurant Management & Realtime Ordering API** built with **ASP.NET Core 8 Web API**, **Entity Framework Core**, and **SignalR**. 

This backend powers a complete restaurant operational suite: Customer QR Code Self-Ordering, Kitchen Display System (KDS), Waiter Call/Notification Hub, Cashier POS with **Automated VietQR Payment Verification**, and an Admin Management Dashboard.

---

## 🌟 Key Features

- **Real-Time Order Pipeline (SignalR Hub)**:
  - Push new orders instantly to the Kitchen Display System (KDS).
  - Broadcast real-time order status transitions: `Pending` ➔ `Cooking` ➔ `Ready` ➔ `Served` ➔ `Paid`.
  - Real-time waiter calls (`Assistance`, `Bill`) from physical table QR sessions.
- **Automated Banking Settlement (SePay Webhook)**:
  - Listens for banking transaction webhooks.
  - Automatically matches `OrderCode` inside transfer descriptions and validates transaction value against `TotalAmount`.
  - Automatically switches table status to `Available` and triggers voice/toast payment confirmations on the frontend.
- **Role-Based Access Control (RBAC)**:
  - Secure JWT authentication with BCrypt password hashing.
  - Granular access policy for roles: `Admin`, `Chef`, `Cashier`, `Waiter`.
- **Clean Architecture & Design Patterns**:
  - **Repository & Service Pattern**: Clear separation between database query logic and business rules.
  - **Soft Delete & Query Filters**: Global query filters (`HasQueryFilter`) ensure deleted records are hidden without data loss and can be restored from the Trash Bin.
  - **Reusable Pagination**: Generic `PagedList<T>` returning headers metadata (`X-Pagination`).
- **External Integrations**:
  - **Cloudinary**: Cloud-based storage and automated thumbnail transformations for food variant assets.
  - **SePay**: Automated bank account sync & webhook verification.

---

## 🏗️ Architecture & Project Structure

```text
RestaurantManagement.API/
├── Controllers/         # API Endpoints (Auth, Foods, Orders, Payments, Tables, Users, Dashboard)
├── Data/
│   ├── Migrations/      # EF Core Code-First migrations
│   └── RestaurantDbContext.cs # EF Core DbContext with Global Query Filters & Auditing
├── DTOs/                # Data Transfer Objects & API Request/Response models
├── Entities/            # Database entities (Food, Order, OrderDetail, Table, User, Feedback...)
├── ExternalServices/   # Third-party integrations (Cloudinary PhotoService)
├── Helpers/             # PagedList<T>, FoodParams, pagination helpers
├── Hubs/                # SignalR OrderHub for real-time WebSocket communication
├── Interfaces/          # Abstractions for Repositories and Services
├── Mappings/            # AutoMapper Profile configurations
├── Repositories/        # Database access implementations (EF Core)
├── Services/            # Core business workflows & webhook reconciliation
└── Program.cs           # Dependency Injection container & pipeline configuration
🔄 Automated Payment Flow (VietQR + Webhook)
hub
Diagram
Mermaid diagram
📡 SignalR Real-Time Contract (/orderHub)
Event Name	Direction	Payload	Description
ReceiveNewOrder	Server ➔ Clients	OrderDto	Phát khi có bàn đặt món mới
OrderStatusUpdated	Server ➔ Clients	orderId, status	Cập nhật tiến độ nấu/lên món
ReceiveTableCall	Server ➔ Clients	tableId, tableName, type	Khách gọi hỗ trợ hoặc gọi tính tiền
TableStatusUpdated	Server ➔ Clients	tableId, statusInt	Bàn chuyển trạng thái (Trống/Có khách)
PaymentReceivedAuto	Server ➔ Clients	{ orderId, tableName, amount }	Nhận tiền chuyển khoản thành công
🚀 Getting Started
1. Prerequisites
.NET 8.0 SDK
MySQL Server (v8.0+)
Cloudinary Account (cho tính năng upload ảnh)
2. Configuration (appsettings.json)
Cấu hình chuỗi kết nối và API keys trong RestaurantManagement.API/appsettings.json:
code
JSON
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=RestaurantDb;User=root;Password=your_password;"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyWithAtLeast32CharactersLong!",
    "Issuer": "RestaurantAPI",
    "Audience": "RestaurantClient"
  },
  "CloudinarySettings": {
    "CloudName": "your_cloud_name",
    "ApiKey": "your_api_key",
    "ApiSecret": "your_api_secret"
  },
  "SePay": {
    "ApiKey": "YOUR_SEPAY_API_KEY"
  }
}
3. Run Migrations & Start API
code
Bash
# Di chuyển vào thư mục API
cd RestaurantManagement.API

# Cập nhật Database Schema
dotnet ef database update

# Khởi chạy server
dotnet run
Mặc định Swagger UI sẽ chạy tại: https://localhost:7xxx/swagger hoặc http://localhost:5xxx/swagger.
4. Khởi tạo tài khoản Quản Trị (Admin)
Gửi request POST đến:
code
Http
POST /api/auth/setup-admin
Tài khoản mặc định được tạo:
Username: admin
Password: 123456
📋 Core API Endpoints
🔐 Authentication
POST /api/auth/login - Đăng nhập (trả về JWT token & vai trò)
POST /api/auth/setup-admin - Khởi tạo tài khoản admin đầu tiên
🍲 Foods Management
GET /api/foods - Lấy danh sách món (hỗ trợ phân trang, tìm kiếm, lọc theo Category, sắp xếp theo giá)
GET /api/foods/deleted - Xem danh sách món ăn trong thùng rác
PUT /api/foods/{id}/restore - Khôi phục món ăn từ thùng rác
POST /api/foods - Thêm món ăn mới (kèm file ảnh upload lên Cloudinary)
PUT /api/foods/{id} - Cập nhật thông tin món
DELETE /api/foods/{id} - Xóa mềm món ăn
🧾 Orders & Kitchen
GET /api/orders - Lịch sử toàn bộ đơn hàng
GET /api/orders/active - Danh sách đơn đang hoạt động (cho KDS & Waiter)
POST /api/orders - Tạo đơn đặt món mới từ bàn
PUT /api/orders/{id}/status - Chuyển trạng thái đơn (Cooking, Ready, Served, Cancelled)
PUT /api/orders/{id}/pay - Xác nhận thanh toán thủ công (Tiền mặt / Thẻ)
💳 Payments & Webhook
POST /api/payments/sepay-webhook - Endpoint nhận Webhook biến động số dư từ SePay/VietQR
📊 Analytics & Dashboard
GET /api/dashboard/summary?period={Day|Week|Month|Year|All} - Báo cáo tổng hợp doanh thu, lượng khách, món bán chạy, hiệu suất bếp
POST /api/dashboard/feedback - Lưu đánh giá chất lượng phục vụ của khách hàng
🛡️ License

This project is licensed under the **MIT License**.

```text
MIT License

Copyright (c) 2026 ShibaTatsuya0905

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
