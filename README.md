# ERP System

A full-stack Enterprise Resource Planning (ERP) web application built with **ASP.NET Core 9** and **Vue 3**. This project demonstrates modular clean architecture, JWT authentication, and multi-domain business logic across five core business modules.

---

## Features

- **Identity & Access Control** — manage user accounts, assign roles (Admin, Manager, User), and handle secure authentication with JWT and Refresh Tokens
- **Sales** — manage customers, create and track orders, update order status
- **Inventory** — manage products and categories, track stock levels
- **Purchasing** — manage suppliers, create and approve purchase orders
- **Finance** — manage accounts, create invoices, record payments
- **Report & Dashboard** — financial summary, sales overview, inventory status with chart data
- **Quality Assurance** — 50+ unit tests covering core business logic using xUnit and Moq

---

## Architecture

This project follows **Modular Monolith** with **Clean Architecture** principles. Each business domain is structured as an independent module with its own layers:

```
backend/src/Modules/{Module}/
├── {Module}.Domain          → Entities, enums (no dependencies)
├── {Module}.Application     → DTOs, service interfaces, repository interfaces
├── {Module}.Infrastructure  → DbContext, EF Core migrations, repository implementations
```

**Shared infrastructure** (GenericRepository, UnitOfWork, exception middleware) lives in `ERP.Shared.*` and is reused across all modules.

**Presentation layer** (`ERP.Api`) is a single ASP.NET Core project that wires all modules together via dependency injection in `Program.cs`.

### Design Patterns Used

- Repository Pattern + Unit of Work
- Dependency Injection (built-in .NET DI)
- DTO-based data transfer (no entity exposure)
- Generic Repository with typed constraints
- Custom Exception Middleware for consistent error responses

---

## Tech Stack

### Backend

|          |                            |
| -------- | -------------------------- |
| Runtime  | .NET 9 / ASP.NET Core      |
| ORM      | Entity Framework Core 9    |
| Database | SQL Server                 |
| Auth     | JWT Bearer + Refresh Token |
| API Docs | Swagger / OpenAPI          |

### Frontend

|                  |                         |
| ---------------- | ----------------------- |
| Framework        | Vue 3 (Composition API) |
| Language         | TypeScript              |
| State Management | Pinia                   |
| Routing          | Vue Router              |
| Styling          | Tailwind CSS v4         |
| HTTP Client      | Axios                   |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) v20 or v22+
- SQL Server (local or Docker)

### 1. Clone the repository

```bash
git clone https://github.com/chinkoike/erp-system.git
cd erp-system
```

### 2. Configure the database

Update the connection string in `backend/src/Presentation/ERP.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ERPSystem;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Run database migrations

```bash
cd backend
dotnet ef database update --project src/Modules/Identity/ERP.Identity.Infrastructure --startup-project src/Presentation/ERP.Api
dotnet ef database update --project src/Modules/Inventory/ERP.Inventory.Infrastructure --startup-project src/Presentation/ERP.Api
dotnet ef database update --project src/Modules/Sales/ERP.Sales.Infrastructure --startup-project src/Presentation/ERP.Api
dotnet ef database update --project src/Modules/Purchasing/ERP.Purchasing.Infrastructure --startup-project src/Presentation/ERP.Api
dotnet ef database update --project src/Modules/Finance/ERP.Finance.Infrastructure --startup-project src/Presentation/ERP.Api
```

### 4. Run the backend

```bash
cd backend
dotnet run --project src/Presentation/ERP.Api
```

API will be available at `http://localhost:5049`  
Swagger UI: `http://localhost:5049/swagger`

### 5. Run the frontend

```bash
cd frontend
npm install
npm run dev
```

Frontend will be available at `http://localhost:5173`

## Testing

This project uses **xUnit** for unit testing and **Moq** for dependency mocking. The tests cover business logic in the Domain and Application layers across all modules.

To run the tests, navigate to the backend directory and execute:

```bash
cd backend/ERP.Tests
dotnet test
```

---

## API Overview

All endpoints are documented via Swagger UI at `/swagger`. Authentication uses **JWT Bearer tokens**.

| Module     | Base Route                         |
| ---------- | ---------------------------------- |
| Identity   | `/api/users`, `/api/roles`         |
| Sales      | `/api/orders`, `/api/customers`    |
| Inventory  | `/api/products`, `/api/categories` |
| Purchasing | `/api/purchasing`                  |
| Finance    | `/api/finance`                     |
| Report     | `/api/report`                      |
| Dashboard  | `/api/dashboard`                   |

---

## Project Structure

```

erp-system/
├── backend/
├──── tests/
│ └── src/
│ ├── Modules/
│ │ ├── Finance/
│ │ ├── Identity/
│ │ ├── Inventory/
│ │ ├── Purchasing/
│ │ ├── Report/
│ │ └── Sales/
│ ├── Presentation/
│ │ └── ERP.Api/ ← entry point
│ └── Shared/
│ ├── ERP.Shared/
│ └── ERP.Shared.Infrastructure/
└── frontend/
└── src/
├── views/ ← page components
├── stores/ ← Pinia state
├── services/ ← API clients
└── types/ ← TypeScript interfaces

```

```

```
