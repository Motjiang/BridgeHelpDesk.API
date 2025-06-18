# 🛠️ Mini Helpdesk System (Backend)

A backend-focused IT Helpdesk Management System built with **ASP.NET Core 9 Web API**, using **C#**, **Entity Framework Core**, **MediatR**, **SignalR**, and **Microsoft SQL Server**. It enables employees to log IT-related tickets (software/hardware), and technicians to view, manage, and resolve them. Real-time updates and audit trails are supported. Email communication is used for password recovery only via **Mailjet**.

---

## 🚀 Features

### 🔐 Authentication & Security
- JWT-based authentication
- Role-based access control (Employee / Technician)
- Password reset via email (Forgot Password feature)

### 🎫 Ticket Management
- Employees log tickets related to IT issues (e.g., software or hardware)
- Technicians view and manage logged tickets
- Tickets can be resolved with status updates

### 🔔 Real-time Notifications (SignalR)
- Technicians receive real-time notifications when a new ticket is logged
- Employees are notified in real-time when their ticket is resolved

### 📝 Audit Trail
- Tracks all ticket-related activities:
  - Ticket creation
  - Ticket resolution
- Captures:
  - User ID
  - Action performed
  - Timestamp

### 📧 Email (Password Reset Only)
- Email sent with reset link for:
  - "Forgot Password" functionality
- Email service handled via **Mailjet API**

---

## 🧱 Tech Stack

| Technology           | Purpose                                 |
|----------------------|------------------------------------------|
| ASP.NET Core 9       | Web API backend                          |
| Entity Framework Core| ORM for data access                      |
| MediatR              | Implements CQRS for clean structure      |
| SignalR              | Real-time communication                  |
| MS SQL Server        | Database backend                         |
| Mailjet              | Email API for password reset             |
| Microsoft Azure      | Optional deployment & integration        |
| xUnit                | Unit tests using FakeItEasy and FluentAssertions        |
| Postman              | API testing and debugging                |

---
