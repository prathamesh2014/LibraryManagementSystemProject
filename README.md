# 📚 Library Management System

A full-stack **Library Management System** built with **ASP.NET Core MVC**, **Entity Framework Core**, and **SQL Server**, following **Clean Architecture** principles. This project demonstrates role-based authentication, complete CRUD operations, a book issue/return workflow with automatic fine calculation, soft delete, and search — built as a fresher-level, interview-ready .NET Full Stack project.

---

## 🚀 Features

- 🔐 **Authentication & Authorization** — Cookie-based login with hashed passwords, role-based access control (Admin / Student)
- 📖 **Book Management** — Full CRUD with cover image upload, Category & Author dropdowns, Soft Delete with restore
- 🏷️ **Category & Author Management** — Full CRUD with delete protection (prevents deleting records still in use)
- 👥 **Member Management** — Full CRUD with active/inactive status
- 🔄 **Issue & Return Books** — Stock validation on issue, automatic fine calculation (₹10/day) on late return
- 🔍 **Search** — Filter books by Title, Category, and Author (combinable)
- 📊 **Admin Dashboard** — Live summary cards (Total Books, Members, Pending, Returned) and recent activity table
- ✅ **Validation** — Server-side and client-side validation using Data Annotations
- 🗑️ **Soft Delete** — Books are archived, not permanently removed, preserving borrowing history

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Language | C# |
| Framework | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core (Code First) |
| Database | SQL Server |
| Views | Razor Views (.cshtml) |
| Styling | Bootstrap 5, custom CSS |
| Authentication | Cookie-based Authentication |
| Validation | Data Annotations |
| Tools | Visual Studio 2022, SQL Server Management Studio (SSMS) |

---

## 🏗️ Architecture

This project follows **Clean Architecture** with four separate projects. Dependencies only ever point inward.

```
LibraryManagementSystem
│
├── LibraryManagement.Domain          → Entities (Book, Author, Category, Member, User, Role, IssueBook)
│
├── LibraryManagement.Infrastructure  → DbContext, Repository Interfaces & Classes, Migrations
│
├── LibraryManagement.Application     → Service Interfaces & Classes (business logic)
│
└── LibraryManagement.UI              → Controllers, Views, ViewModels, wwwroot, Program.cs
```

**Request Flow:**

```
Controller → Service Interface → Service → Repository Interface → Repository → DbContext → SQL Server
```

The Controller never accesses the DbContext or a Repository directly — only Services. This keeps business logic centralized, testable, and out of both the Controllers and the data-access layer.

---

## 🗄️ Database Schema

| Table | Description |
|---|---|
| `Roles` | Admin / Student roles |
| `Users` | Login accounts (email, hashed password, role) |
| `Authors` | Book authors |
| `Categories` | Book categories/genres |
| `Books` | Book catalog (ISBN, copies, soft-delete flag, links to Author & Category) |
| `Members` | Library patrons (separate from login Users) |
| `IssueBooks` | Borrow transactions (issue date, due date, return date, fine amount, status) |

All foreign keys use `DeleteBehavior.Restrict` to prevent accidental data loss — deleting an Author/Category/Member/Book still referenced elsewhere is blocked with a friendly error message instead of corrupting related records.

---

## ⚙️ Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) (Express edition is fine)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code
- SQL Server Management Studio (SSMS) — optional, for inspecting the database

### 1. Clone the repository

```bash
git clone https://github.com/<your-username>/LibraryManagementSystem.git
cd LibraryManagementSystem
```

### 2. Configure the database connection

Open `LibraryManagement.UI/appsettings.json` and update the connection string to match your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=LibraryManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Apply migrations to create the database

Open **Package Manager Console** in Visual Studio, set the **Default Project** dropdown to `LibraryManagement.Infrastructure`, then run:

```powershell
Update-Database -StartupProject LibraryManagement.UI
```

Or via the .NET CLI:

```bash
dotnet ef database update --project LibraryManagement.Infrastructure --startup-project LibraryManagement.UI
```

### 4. Seed the initial roles

Run this in SSMS against the newly created `LibraryManagementDb` database:

```sql
INSERT INTO Roles (RoleName, CreatedDate) VALUES ('Admin', GETDATE());
INSERT INTO Roles (RoleName, CreatedDate) VALUES ('Student', GETDATE());
```

### 5. Run the project

```bash
dotnet run --project LibraryManagement.UI
```

Or press **F5** in Visual Studio.

### 6. Create your first Admin account

1. Register a new account through the app (`/Account/Register`) — new registrations default to the **Student** role.
2. In SSMS, promote that account to Admin:

```sql
UPDATE Users SET RoleId = (SELECT RoleId FROM Roles WHERE RoleName = 'Admin')
WHERE Email = 'your-registered-email@example.com';
```

3. Log out and log back in — you'll now have Admin access to the Dashboard, Books, Categories, Authors, Members, and Issued Books.

---

## 📁 Project Structure

```
UI
├── Controllers/        → AccountController, BookController, CategoryController,
│                          AuthorController, MemberController, IssueBookController,
│                          ReturnBookController, DashboardController
├── Models/              → ViewModels (BookViewModel, IssueBookViewModel, DashboardViewModel)
├── Views/                → Razor views organized by controller
├── wwwroot/              → CSS, JS, uploaded book cover images
└── Program.cs            → DI registration, middleware pipeline, routing

Application
├── IServices/           → Service contracts
└── Services/             → Business logic implementations

Infrastructure
├── Data/                 → LibraryDbContext
├── IRepository/          → Repository contracts
├── Repository/           → EF Core data access implementations
└── Migrations/           → EF Core Code-First migration history

Domain
└── Entities/              → Book, Author, Category, Member, User, Role, IssueBook, BaseEntity
```

---

## 🔑 Key Design Decisions

- **Repository + Service split** — Repositories only perform data access; Services hold business rules (stock checks, fine calculation, default values). This keeps logic testable and independent of EF Core.
- **Soft Delete on Books** — Deleted books are hidden (`IsDeleted = true`) rather than removed, preserving issue/return history permanently. An Admin can view and restore archived books.
- **Flat ViewModels** — `BookViewModel` uses plain fields instead of binding the `Book` entity directly, avoiding circular-reference issues from EF Core navigation properties during model binding.
- **Cookie-based Authentication** — Chosen over JWT since this is a server-rendered MVC app, not a separate API/SPA. Passwords are hashed with `PasswordHasher<T>` and never stored in plain text.
- **Post-Redirect-Get pattern** — Every Create/Edit/Delete/Issue/Return POST action redirects afterward, preventing duplicate form submissions on page refresh.

---

## 🧪 Default Business Rules

| Rule | Value |
|---|---|
| Loan period | 14 days from issue date |
| Late fine | ₹10 per day overdue |
| New book copies | `AvailableCopies` initialized equal to `TotalCopies` |
| Stock check | Books with 0 available copies cannot be issued |

---

## 📌 Possible Future Improvements

- Pagination on Book / Member / IssueBook listing pages
- Background job to auto-flag overdue books instead of calculating fines only at return time
- Unit tests for the Service layer (fine calculation, stock validation)
- Email notifications for due-date reminders

---

## 📄 License

This project was built for educational purposes as part of a .NET Full Stack learning path.
