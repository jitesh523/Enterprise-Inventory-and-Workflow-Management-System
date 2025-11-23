-- =============================================
-- Enterprise Inventory Management System
-- Database Schema Creation Script
-- Phase 2: Normalized Database Design (3NF)
-- =============================================

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'InventoryDB')
BEGIN
    CREATE DATABASE InventoryDB;
END
GO

USE InventoryDB;
GO

-- =============================================
-- SECTION 1: Product Management Tables
-- =============================================

-- Categories Table (Hierarchical)
CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ParentCategoryId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Categories_ParentCategory FOREIGN KEY (ParentCategoryId) REFERENCES Categories(Id)
);

-- Brands Table
CREATE TABLE Brands (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- Units of Measure Table
CREATE TABLE UnitsOfMeasure (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(20) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- Tax Groups Table
CREATE TABLE TaxGroups (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    TaxRate DECIMAL(5,2) NOT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- Products Table (Master Product)
CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CategoryId INT NOT NULL,
    BrandId INT NULL,
    BaseUOMId INT NOT NULL,
    TaxGroupId INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Products_Category FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    CONSTRAINT FK_Products_Brand FOREIGN KEY (BrandId) REFERENCES Brands(Id),
    CONSTRAINT FK_Products_BaseUOM FOREIGN KEY (BaseUOMId) REFERENCES UnitsOfMeasure(Id),
    CONSTRAINT FK_Products_TaxGroup FOREIGN KEY (TaxGroupId) REFERENCES TaxGroups(Id)
);

-- Product Variants Table (Sellable SKUs)
CREATE TABLE ProductVariants (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    SKU NVARCHAR(50) NOT NULL UNIQUE,
    Barcode NVARCHAR(100) NULL,
    CostPrice DECIMAL(18,4) NOT NULL DEFAULT 0,
    SalesPrice DECIMAL(18,4) NOT NULL DEFAULT 0,
    ReorderPoint INT NOT NULL DEFAULT 0,
    ReorderQuantity INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_ProductVariants_Product FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- Product Attributes Table (Dynamic Attributes like Color, Size)
CREATE TABLE ProductAttributes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- Product Attribute Values Table
CREATE TABLE ProductAttributeValues (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductVariantId INT NOT NULL,
    AttributeId INT NOT NULL,
    Value NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_ProductAttributeValues_ProductVariant FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id),
    CONSTRAINT FK_ProductAttributeValues_Attribute FOREIGN KEY (AttributeId) REFERENCES ProductAttributes(Id)
);

-- =============================================
-- SECTION 2: Warehouse and Inventory Tables
-- =============================================

-- Warehouses Table
CREATE TABLE Warehouses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Code NVARCHAR(20) NOT NULL UNIQUE,
    Address NVARCHAR(500) NOT NULL,
    IsNettable BIT NOT NULL DEFAULT 1,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- Locations Table (Bin Management)
CREATE TABLE Locations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    WarehouseId INT NOT NULL,
    Code NVARCHAR(20) NOT NULL,
    ZoneType INT NOT NULL, -- 1=Receiving, 2=BulkStorage, 3=Picking, 4=Packing, 5=Shipping, 6=Quarantine
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Locations_Warehouse FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id),
    CONSTRAINT UQ_Location_Code UNIQUE (WarehouseId, Code)
);

-- Inventory Stock Table (Current Snapshot)
CREATE TABLE InventoryStock (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductVariantId INT NOT NULL,
    LocationId INT NOT NULL,
    WarehouseId INT NOT NULL,
    QuantityOnHand DECIMAL(18,2) NOT NULL DEFAULT 0,
    QuantityAllocated DECIMAL(18,2) NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_InventoryStock_ProductVariant FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id),
    CONSTRAINT FK_InventoryStock_Location FOREIGN KEY (LocationId) REFERENCES Locations(Id),
    CONSTRAINT FK_InventoryStock_Warehouse FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id),
    CONSTRAINT UQ_InventoryStock UNIQUE (ProductVariantId, LocationId)
);

-- Inventory Transactions Table (Immutable Audit Trail)
CREATE TABLE InventoryTransactions (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    TransactionDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Type INT NOT NULL, -- 1=Purchase, 2=Sale, 3=Adjustment, 4=Transfer, 5=Allocation, 6=Return
    ProductVariantId INT NOT NULL,
    LocationId INT NOT NULL,
    QuantityChange DECIMAL(18,2) NOT NULL,
    UnitCost DECIMAL(18,4) NOT NULL,
    ReferenceDocType NVARCHAR(50) NULL,
    ReferenceDocId INT NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_InventoryTransactions_ProductVariant FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id),
    CONSTRAINT FK_InventoryTransactions_Location FOREIGN KEY (LocationId) REFERENCES Locations(Id)
);

-- Stock Adjustments Table
CREATE TABLE StockAdjustments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AdjustmentDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ProductVariantId INT NOT NULL,
    LocationId INT NOT NULL,
    QuantityAdjusted DECIMAL(18,2) NOT NULL,
    ReasonCode INT NOT NULL, -- 1=Damaged, 2=Expired, 3=Lost, 4=Found, 5=CycleCountCorrection, 99=Other
    Notes NVARCHAR(MAX) NULL,
    AdjustedBy NVARCHAR(256) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_StockAdjustments_ProductVariant FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id),
    CONSTRAINT FK_StockAdjustments_Location FOREIGN KEY (LocationId) REFERENCES Locations(Id)
);

-- Transfer Orders Table
CREATE TABLE TransferOrders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TransferNumber NVARCHAR(50) NOT NULL UNIQUE,
    TransferDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FromWarehouseId INT NOT NULL,
    ToWarehouseId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Draft',
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_TransferOrders_FromWarehouse FOREIGN KEY (FromWarehouseId) REFERENCES Warehouses(Id),
    CONSTRAINT FK_TransferOrders_ToWarehouse FOREIGN KEY (ToWarehouseId) REFERENCES Warehouses(Id)
);

-- Transfer Order Lines Table
CREATE TABLE TransferOrderLines (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TransferOrderId INT NOT NULL,
    ProductVariantId INT NOT NULL,
    Quantity DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_TransferOrderLines_TransferOrder FOREIGN KEY (TransferOrderId) REFERENCES TransferOrders(Id),
    CONSTRAINT FK_TransferOrderLines_ProductVariant FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id)
);

-- =============================================
-- SECTION 3: Customer and Order Management Tables
-- =============================================

-- Customers Table
CREATE TABLE Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Phone NVARCHAR(20) NULL,
    Address NVARCHAR(500) NULL,
    TaxId NVARCHAR(50) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- Orders Table
CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderNumber NVARCHAR(50) NOT NULL UNIQUE,
    OrderDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CustomerId INT NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- 0=Draft, 1=Confirmed, 2=Allocated, 3=Picked, 4=Packed, 5=Shipped, 6=Invoiced, 99=Cancelled
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Notes NVARCHAR(MAX) NULL,
    WarehouseId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Orders_Customer FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    CONSTRAINT FK_Orders_Warehouse FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id)
);

-- Order Lines Table
CREATE TABLE OrderLines (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductVariantId INT NOT NULL,
    Quantity DECIMAL(18,2) NOT NULL,
    UnitPrice DECIMAL(18,4) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_OrderLines_Order FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    CONSTRAINT FK_OrderLines_ProductVariant FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id)
);

-- =============================================
-- SECTION 4: Vendor and Procurement Tables
-- =============================================

-- Vendors Table
CREATE TABLE Vendors (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    TaxId NVARCHAR(50) NULL,
    Email NVARCHAR(256) NOT NULL,
    Phone NVARCHAR(20) NULL,
    Address NVARCHAR(500) NULL,
    PaymentTermsDays INT NOT NULL DEFAULT 30,
    Rating INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- Purchase Orders Table
CREATE TABLE PurchaseOrders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PONumber NVARCHAR(50) NOT NULL UNIQUE,
    OrderDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    VendorId INT NOT NULL,
    Status INT NOT NULL DEFAULT 0, -- 0=Draft, 1=Submitted, 2=PendingApproval, 3=Approved, 4=PartiallyReceived, 5=Closed, 99=Cancelled
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    ExpectedDeliveryDate DATETIME2 NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_PurchaseOrders_Vendor FOREIGN KEY (VendorId) REFERENCES Vendors(Id)
);

-- Purchase Order Lines Table
CREATE TABLE PurchaseOrderLines (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PurchaseOrderId INT NOT NULL,
    ProductVariantId INT NOT NULL,
    QuantityOrdered DECIMAL(18,2) NOT NULL,
    QuantityReceived DECIMAL(18,2) NOT NULL DEFAULT 0,
    UnitPrice DECIMAL(18,4) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_PurchaseOrderLines_PurchaseOrder FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrders(Id),
    CONSTRAINT FK_PurchaseOrderLines_ProductVariant FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id)
);

-- Goods Receipt Notes Table
CREATE TABLE GoodsReceiptNotes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GRNNumber NVARCHAR(50) NOT NULL UNIQUE,
    PurchaseOrderId INT NOT NULL,
    ReceivedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DeliveryNoteNumber NVARCHAR(50) NULL,
    ReceivedBy NVARCHAR(256) NOT NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_GoodsReceiptNotes_PurchaseOrder FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrders(Id)
);

-- Goods Receipt Lines Table
CREATE TABLE GoodsReceiptLines (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GoodsReceiptNoteId INT NOT NULL,
    PurchaseOrderLineId INT NOT NULL,
    ProductVariantId INT NOT NULL,
    QuantityReceived DECIMAL(18,2) NOT NULL,
    LocationId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_GoodsReceiptLines_GoodsReceiptNote FOREIGN KEY (GoodsReceiptNoteId) REFERENCES GoodsReceiptNotes(Id),
    CONSTRAINT FK_GoodsReceiptLines_PurchaseOrderLine FOREIGN KEY (PurchaseOrderLineId) REFERENCES PurchaseOrderLines(Id),
    CONSTRAINT FK_GoodsReceiptLines_ProductVariant FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id),
    CONSTRAINT FK_GoodsReceiptLines_Location FOREIGN KEY (LocationId) REFERENCES Locations(Id)
);

GO

PRINT 'Database schema created successfully!';
PRINT 'Total tables created: 27';
GO
