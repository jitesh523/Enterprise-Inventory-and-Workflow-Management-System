# Enterprise Inventory and Workflow Management System

## 🎯 Project Overview

A production-ready enterprise inventory and workflow management system built with ASP.NET Core MVC and Microsoft SQL Server, implementing Clean Architecture principles with comprehensive business logic, security, and reporting capabilities.

## ✨ Key Features

### Core Functionality
- **Multi-Warehouse Inventory Management** with bin-level tracking
- **Order Management** with complete fulfillment workflow
- **Procurement System** with vendor management and GRN processing
- **Dynamic RBAC** with 31 granular permissions
- **Real-time Dashboards** with Chart.js visualizations
- **Comprehensive Reporting** with 6 optimized SQL views

### Technical Highlights
- ✅ Clean Architecture (5 layers)
- ✅ Domain-Driven Design with rich domain models
- ✅ State machines for workflow enforcement
- ✅ Pessimistic locking for concurrency control
- ✅ Moving weighted average cost calculation
- ✅ Audit trails for all data changes
- ✅ Repository + Unit of Work pattern
- ✅ Dapper for high-performance queries

## 🏗️ Architecture

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

## 📊 Database Schema

- **27 normalized tables** (3NF)
- **40+ performance indexes**
- **2 concurrency-safe stored procedures**
- **6 reporting views**

### Key Tables
- Products & ProductVariants (Master-Variant pattern)
- InventoryStock & InventoryTransactions
- Orders & OrderLines
- PurchaseOrders & GoodsReceiptNotes
- Vendors with rating system
- Permissions & RolePermissions (Dynamic RBAC)

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Microsoft SQL Server 2019+ or SQL Server Express
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/jitesh523/Enterprise-Inventory-and-Workflow-Management-System.git
   cd Enterprise-Inventory-and-Workflow-Management-System
   ```

2. **Install .NET SDK** (if not already installed)
   ```bash
   # macOS
   brew install --cask dotnet-sdk
   
   # Windows - Download from https://dotnet.microsoft.com/download
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

5. **Create database and run scripts**
   ```bash
   # Execute SQL scripts in order:
   # 1. Database/Scripts/01_CreateTables.sql
   # 2. Database/Scripts/02_CreateIndexes.sql
   # 3. Database/Scripts/03_CreateSecuritySchema.sql
   # 4. Database/StoredProcedures/sp_AllocateInventory.sql
   # 5. Database/StoredProcedures/sp_ProcessGoodsReceipt.sql
   # 6. Database/Views/ReportingViews.sql
   ```

6. **Run the application**
   ```bash
   cd src/Inventory.Web
   dotnet run
   ```

7. **Access the application**
   
   Open your browser: `https://localhost:5001`

## 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test --filter Category=Unit

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 📁 Project Structure

```
├── src/
│   ├── Inventory.Domain/          # Core domain entities and business rules
│   ├── Inventory.Application/     # Application business logic
│   ├── Inventory.Infrastructure/  # External concerns (DB, services)
│   └── Inventory.Web/             # ASP.NET Core MVC
├── tests/
│   └── Inventory.Tests/           # Unit and integration tests
└── Database/
    ├── Scripts/                   # SQL schema scripts
    ├── StoredProcedures/          # Stored procedures
    └── Views/                     # Reporting views
```

## 🔐 Security

- **Authentication**: ASP.NET Core Identity
- **Authorization**: Dynamic RBAC with 31 permissions
- **Audit Trails**: Automatic logging of all changes
- **SQL Injection**: Protected via parameterized queries

## 📈 Performance Optimizations

- **Indexes**: 40+ strategic indexes on all major query paths
- **Stored Procedures**: Critical operations use optimized SQL
- **Dapper**: High-performance queries for reporting
- **Materialized Views**: Pre-aggregated data for dashboards
- **Eager Loading**: Optimized entity loading strategies

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core 8.0
- **Micro-ORM**: Dapper 2.1
- **Frontend**: Razor Views, jQuery, Chart.js
- **Testing**: xUnit, Moq, FluentAssertions
- **Validation**: FluentValidation
- **Patterns**: CQRS (MediatR ready)

## 📝 Implementation Status

**Completed Phases**: 10/13 (77%)

✅ Phase 1: Project Setup & Architecture  
✅ Phase 2: Database Engineering  
✅ Phase 3: Core Domain Logic  
✅ Phase 4: Data Access & Concurrency  
✅ Phase 5: Security Architecture (RBAC)  
✅ Phase 6: Vendor & Procurement Module  
✅ Phase 7: Order Management Module  
✅ Phase 8: Inventory Control Module  
✅ Phase 9: Reporting & Analytics  
✅ Phase 10: Infrastructure & Best Practices  
✅ Phase 11: User Interface Development  
🔄 Phase 12: Testing (In Progress)  
⏳ Phase 13: Deployment & Documentation  

## 🤝 Contributing

This is an enterprise project following strict architectural guidelines. Please ensure:
- All changes maintain Clean Architecture principles
- Domain layer remains dependency-free
- Unit tests cover all business logic
- Integration tests verify database operations

## 📄 License

[Add your license here]

## 👥 Authors

[Add author information]

---

**Built with Clean Architecture principles for maintainability, testability, and scalability.**

**Total**: 35+ files | ~4,500 lines of code | 27 tables | 40+ indexes | 6 views | 2 stored procedures
