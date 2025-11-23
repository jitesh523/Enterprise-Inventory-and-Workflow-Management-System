# Enterprise Inventory and Workflow Management System

A comprehensive enterprise-grade inventory and workflow management system built with ASP.NET Core MVC and Microsoft SQL Server, following Clean Architecture principles.

## 🏗️ Architecture

This project implements **Clean Architecture** (also known as Onion or Hexagonal Architecture) with strict layer separation:

```
┌─────────────────────────────────────────┐
│           Inventory.Web (MVC)           │
│         Controllers, Views, UI          │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│       Inventory.Infrastructure          │
│   EF Core, Repositories, External APIs  │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│        Inventory.Application            │
│   Business Logic, DTOs, Validation      │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│          Inventory.Domain               │
│   Entities, Value Objects, Events       │
│        (Zero Dependencies)              │
└─────────────────────────────────────────┘
```

## 📋 Features

### Core Modules
- **Product Management**: Master-Variant pattern with dynamic attributes
- **Inventory Control**: Multi-warehouse, bin-level tracking
- **Order Management**: Complete fulfillment workflow with state machine
- **Procurement**: Vendor management, PO workflow, GRN processing
- **Reporting & Analytics**: Real-time dashboards with Chart.js

### Technical Highlights
- ✅ Clean Architecture with dependency inversion
- ✅ Domain-Driven Design (DDD) with rich domain models
- ✅ CQRS pattern with MediatR
- ✅ Pessimistic locking for concurrency control
- ✅ Dynamic RBAC (Role-Based Access Control)
- ✅ Audit trails with EF Core interceptors
- ✅ Comprehensive unit and integration tests

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Microsoft SQL Server 2019+ or SQL Server Express
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Enterprise-Inventory-and-Workflow-Management-System
   ```

2. **Install .NET SDK** (if not already installed)
   ```bash
   # macOS
   brew install --cask dotnet-sdk
   
   # Windows
   # Download from https://dotnet.microsoft.com/download
   ```

3. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

4. **Update database connection string**
   
   Edit `src/Inventory.Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=InventoryDB;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```

5. **Run database migrations**
   ```bash
   cd src/Inventory.Web
   dotnet ef database update
   ```

6. **Run the application**
   ```bash
   dotnet run
   ```

7. **Access the application**
   
   Open your browser and navigate to: `https://localhost:5001`

## 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test --filter Category=Unit

# Run integration tests
dotnet test --filter Category=Integration
```

## 📁 Project Structure

```
├── src/
│   ├── Inventory.Domain/          # Core domain entities and business rules
│   │   ├── Entities/              # Domain entities
│   │   ├── Events/                # Domain events
│   │   ├── Exceptions/            # Domain exceptions
│   │   ├── Interfaces/            # Repository interfaces
│   │   └── ValueObjects/          # Value objects
│   │
│   ├── Inventory.Application/     # Application business logic
│   │   ├── Commands/              # CQRS commands
│   │   ├── Queries/               # CQRS queries
│   │   ├── DTOs/                  # Data transfer objects
│   │   └── Services/              # Application services
│   │
│   ├── Inventory.Infrastructure/  # External concerns
│   │   ├── Data/                  # EF Core DbContext
│   │   ├── Repositories/          # Repository implementations
│   │   └── Services/              # External services
│   │
│   └── Inventory.Web/             # ASP.NET Core MVC
│       ├── Controllers/           # MVC controllers
│       ├── Views/                 # Razor views
│       └── wwwroot/               # Static files
│
├── tests/
│   └── Inventory.Tests/           # Unit and integration tests
│
└── Database/
    └── Scripts/                   # SQL scripts and stored procedures
```

## 🔐 Security

- **Authentication**: ASP.NET Core Identity
- **Authorization**: Dynamic RBAC with database-driven permissions
- **Audit Trails**: Automatic logging of all data changes
- **SQL Injection**: Protected via parameterized queries and EF Core

## 📊 Database Schema

The system uses a normalized database schema (3NF) with the following key tables:

- **Products & Variants**: Master-variant pattern for product management
- **Inventory**: Multi-warehouse stock tracking with bin locations
- **Orders**: Sales order workflow management
- **Purchase Orders**: Procurement and vendor management
- **Transactions**: Immutable audit trail of all inventory movements

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core 8.0
- **Micro-ORM**: Dapper (for performance-critical queries)
- **Frontend**: Razor Views, jQuery, Chart.js
- **Testing**: xUnit, Moq, FluentAssertions
- **Validation**: FluentValidation
- **Messaging**: MediatR (CQRS pattern)

## 📖 Documentation

- [Implementation Plan](/.gemini/antigravity/brain/a666dffe-251a-45f6-97bd-8ee38dab42c0/implementation_plan.md)
- [Task Breakdown](/.gemini/antigravity/brain/a666dffe-251a-45f6-97bd-8ee38dab42c0/task.md)
- [Original Documentation](/Building%20Inventory%20System%20From%20Scratch.pdf)

## 🤝 Contributing

This is an enterprise project following strict architectural guidelines. Please ensure:
- All changes maintain Clean Architecture principles
- Domain layer remains dependency-free
- Unit tests cover all business logic
- Integration tests verify database operations

## 📝 License

[Add your license here]

## 👥 Authors

[Add author information]

---

**Status**: Phase 1 Complete ✅ - Foundation and Core Domain Entities

**Next Phase**: Phase 2 - Database Engineering and Schema Design
