# Community Library Desk System

A simple internal system for community libraries to track books, members, and loans.

## Features
- 📚 Book management (CRUD)
- 👥 Member management (CRUD)
- 📋 Loan management with business rules
- 🔍 Search and filter books
- 👑 Admin role management
- 🔒 ASP.NET Core Identity authentication

## Tech Stack
- ASP.NET Core MVC
- Entity Framework Core
- SQLite
- xUnit for testing
- GitHub Actions CI/CD

## Setup Instructions
1. Clone the repository
2. Navigate to Library.MVC folder: `cd Library.MVC`
3. Update database: `dotnet ef database update`
4. Run the app: `dotnet run`

## Default Admin Credentials
- Email: Billie@library1.com
- Password: Billie@123

## Test Credentials
- Regular users can register themselves

## GitHub Actions
CI workflow automatically builds and tests on every push to main.
