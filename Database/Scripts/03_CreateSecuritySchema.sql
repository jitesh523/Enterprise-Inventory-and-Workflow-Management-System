-- =============================================
-- Enterprise Inventory Management System
-- Security Schema: Identity and Permissions
-- Phase 5: Dynamic RBAC
-- =============================================

USE InventoryDB;
GO

-- Permissions Table
CREATE TABLE Permissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClaimValue NVARCHAR(256) NOT NULL UNIQUE,
    GroupName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- RolePermissions Junction Table
CREATE TABLE RolePermissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleId NVARCHAR(450) NOT NULL,
    PermissionId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_RolePermissions_Role FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE,
    CONSTRAINT FK_RolePermissions_Permission FOREIGN KEY (PermissionId) REFERENCES Permissions(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_RolePermission UNIQUE (RoleId, PermissionId)
);

-- Indexes
CREATE NONCLUSTERED INDEX IX_Permissions_GroupName ON Permissions(GroupName) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_RolePermissions_RoleId ON RolePermissions(RoleId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_RolePermissions_PermissionId ON RolePermissions(PermissionId) WHERE IsDeleted = 0;

GO

-- Seed default permissions
INSERT INTO Permissions (ClaimValue, GroupName, Description) VALUES
-- Product Management
('Permissions.Products.View', 'Products', 'View products'),
('Permissions.Products.Create', 'Products', 'Create new products'),
('Permissions.Products.Edit', 'Products', 'Edit existing products'),
('Permissions.Products.Delete', 'Products', 'Delete products'),

-- Order Management
('Permissions.Orders.View', 'Orders', 'View orders'),
('Permissions.Orders.Create', 'Orders', 'Create new orders'),
('Permissions.Orders.Edit', 'Orders', 'Edit orders'),
('Permissions.Orders.Delete', 'Orders', 'Delete orders'),
('Permissions.Orders.Confirm', 'Orders', 'Confirm orders'),
('Permissions.Orders.Ship', 'Orders', 'Ship orders'),
('Permissions.Orders.Cancel', 'Orders', 'Cancel orders'),

-- Inventory Management
('Permissions.Inventory.View', 'Inventory', 'View inventory'),
('Permissions.Inventory.Adjust', 'Inventory', 'Adjust inventory'),
('Permissions.Inventory.Transfer', 'Inventory', 'Transfer inventory'),

-- Purchase Orders
('Permissions.PurchaseOrders.View', 'PurchaseOrders', 'View purchase orders'),
('Permissions.PurchaseOrders.Create', 'PurchaseOrders', 'Create purchase orders'),
('Permissions.PurchaseOrders.Edit', 'PurchaseOrders', 'Edit purchase orders'),
('Permissions.PurchaseOrders.Approve', 'PurchaseOrders', 'Approve purchase orders'),
('Permissions.PurchaseOrders.Delete', 'PurchaseOrders', 'Delete purchase orders'),

-- Vendors
('Permissions.Vendors.View', 'Vendors', 'View vendors'),
('Permissions.Vendors.Create', 'Vendors', 'Create vendors'),
('Permissions.Vendors.Edit', 'Vendors', 'Edit vendors'),
('Permissions.Vendors.Delete', 'Vendors', 'Delete vendors'),

-- Reports
('Permissions.Reports.View', 'Reports', 'View reports'),
('Permissions.Reports.Export', 'Reports', 'Export reports'),

-- Administration
('Permissions.Users.View', 'Administration', 'View users'),
('Permissions.Users.Create', 'Administration', 'Create users'),
('Permissions.Users.Edit', 'Administration', 'Edit users'),
('Permissions.Users.Delete', 'Administration', 'Delete users'),
('Permissions.Roles.Manage', 'Administration', 'Manage roles and permissions');

GO

PRINT 'Security schema and permissions created successfully!';
PRINT 'Total permissions seeded: 31';
GO
