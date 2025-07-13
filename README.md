# Bridge HelpDesk System

**Bridge HelpDesk System** is a robust backend solution for managing IT support tickets within an organization. Built with modern ASP.NET Core 9 RESTful Web API architecture, this system streamlines ticket logging, technician assignment, and real-time notifications — empowering support teams to resolve issues efficiently.

---

## 🚀 Project Overview

This backend project provides a Help Desk Management System where:

- **Employees** can log support tickets describing their IT issues.
- **Technicians** receive and resolve tickets.
- All **active technicians** receive real-time notifications when new tickets are logged, leveraging **SignalR**.
- The system maintains an **audit trail** for all activities to ensure security and accountability.
- Employees can **reset their password** via a secure email link sent through **Mailjet**.
- The system uses **JWT (JSON Web Tokens)** for secure, stateless authentication.
- Designed to be scalable, secure, and maintainable using best practices and modern design patterns.

---

## 🛠️ Technologies & Tools Used

- **ASP.NET Core 9** - RESTful Web API framework  
- **C#** - Primary programming language  
- **MediatR** - Implements CQRS (Command Query Responsibility Segregation) for clean separation of commands and queries  
- **SignalR** - Real-time communication to notify technicians instantly of new tickets  
- **xUnit** - Unit testing framework  
- **Postman** - API testing and documentation  
- **Azure App Service** - Cloud deployment platform  
- **Mailjet** - Email delivery service for password reset emails  
- **JWT Authentication** - Secure, stateless user authentication and authorization  
- **Audit Trail** - Tracks user and system actions for enhanced security  

---

## 📂 Key Features

- **Ticket Management**: Create, update, view, and delete support tickets  
- **User Roles**: Employee and Technician with role-based authorization  
- **Real-Time Notifications**: Active technicians receive instant alerts on ticket updates  
- **Audit Logging**: Records all critical actions for security and compliance  
- **Password Reset**: Employees can request password resets; emails with secure reset links are sent via Mailjet  
- **JWT Authentication**: Secure login with token-based stateless authentication  
- **RESTful API**: Clean, maintainable API endpoints following REST principles  

---

## 🧪 Testing

- Comprehensive unit tests implemented using **xUnit** to ensure robust and bug-free code  
- Postman collections available for manual and automated API testing  

---

## ☁️ Deployment

- The API will soon be deployed on **Azure App Service** for scalable and reliable cloud hosting  

---
