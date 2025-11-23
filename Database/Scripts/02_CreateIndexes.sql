-- =============================================
-- Enterprise Inventory Management System
-- Database Indexes and Constraints
-- Phase 2: Performance Optimization
-- =============================================

USE InventoryDB;
GO

-- =============================================
-- SECTION 1: Product Management Indexes
-- =============================================

-- Products indexes
CREATE NONCLUSTERED INDEX IX_Products_CategoryId ON Products(CategoryId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Products_BrandId ON Products(BrandId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Products_IsActive ON Products(IsActive) WHERE IsDeleted = 0;

-- ProductVariants indexes
CREATE NONCLUSTERED INDEX IX_ProductVariants_ProductId ON ProductVariants(ProductId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_ProductVariants_SKU ON ProductVariants(SKU) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_ProductVariants_Barcode ON ProductVariants(Barcode) WHERE IsDeleted = 0 AND Barcode IS NOT NULL;

-- ProductAttributeValues indexes
CREATE NONCLUSTERED INDEX IX_ProductAttributeValues_ProductVariantId ON ProductAttributeValues(ProductVariantId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_ProductAttributeValues_AttributeId ON ProductAttributeValues(AttributeId) WHERE IsDeleted = 0;

-- =============================================
-- SECTION 2: Inventory and Warehouse Indexes
-- =============================================

-- Warehouses indexes
CREATE NONCLUSTERED INDEX IX_Warehouses_Code ON Warehouses(Code) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Warehouses_IsActive ON Warehouses(IsActive) WHERE IsDeleted = 0;

-- Locations indexes
CREATE NONCLUSTERED INDEX IX_Locations_WarehouseId ON Locations(WarehouseId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Locations_ZoneType ON Locations(ZoneType) WHERE IsDeleted = 0;

-- InventoryStock indexes (CRITICAL for performance)
CREATE NONCLUSTERED INDEX IX_InventoryStock_ProductVariantId ON InventoryStock(ProductVariantId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_InventoryStock_LocationId ON InventoryStock(LocationId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_InventoryStock_WarehouseId ON InventoryStock(WarehouseId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_InventoryStock_QuantityOnHand ON InventoryStock(QuantityOnHand) WHERE IsDeleted = 0;

-- InventoryTransactions indexes (for audit queries)
CREATE NONCLUSTERED INDEX IX_InventoryTransactions_ProductVariantId ON InventoryTransactions(ProductVariantId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_InventoryTransactions_TransactionDate ON InventoryTransactions(TransactionDate DESC) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_InventoryTransactions_Type ON InventoryTransactions(Type) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_InventoryTransactions_ReferenceDoc ON InventoryTransactions(ReferenceDocType, ReferenceDocId) WHERE IsDeleted = 0;

-- StockAdjustments indexes
CREATE NONCLUSTERED INDEX IX_StockAdjustments_ProductVariantId ON StockAdjustments(ProductVariantId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_StockAdjustments_AdjustmentDate ON StockAdjustments(AdjustmentDate DESC) WHERE IsDeleted = 0;

-- TransferOrders indexes
CREATE NONCLUSTERED INDEX IX_TransferOrders_TransferNumber ON TransferOrders(TransferNumber) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_TransferOrders_FromWarehouseId ON TransferOrders(FromWarehouseId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_TransferOrders_ToWarehouseId ON TransferOrders(ToWarehouseId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_TransferOrders_Status ON TransferOrders(Status) WHERE IsDeleted = 0;

-- =============================================
-- SECTION 3: Order Management Indexes
-- =============================================

-- Customers indexes
CREATE NONCLUSTERED INDEX IX_Customers_Email ON Customers(Email) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Customers_Name ON Customers(Name) WHERE IsDeleted = 0;

-- Orders indexes (CRITICAL for performance)
CREATE NONCLUSTERED INDEX IX_Orders_OrderNumber ON Orders(OrderNumber) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Orders_CustomerId ON Orders(CustomerId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Orders_Status ON Orders(Status) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Orders_OrderDate ON Orders(OrderDate DESC) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Orders_WarehouseId ON Orders(WarehouseId) WHERE IsDeleted = 0;

-- OrderLines indexes
CREATE NONCLUSTERED INDEX IX_OrderLines_OrderId ON OrderLines(OrderId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_OrderLines_ProductVariantId ON OrderLines(ProductVariantId) WHERE IsDeleted = 0;

-- =============================================
-- SECTION 4: Procurement Indexes
-- =============================================

-- Vendors indexes
CREATE NONCLUSTERED INDEX IX_Vendors_Name ON Vendors(Name) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Vendors_IsActive ON Vendors(IsActive) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Vendors_Rating ON Vendors(Rating DESC) WHERE IsDeleted = 0;

-- PurchaseOrders indexes
CREATE NONCLUSTERED INDEX IX_PurchaseOrders_PONumber ON PurchaseOrders(PONumber) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_PurchaseOrders_VendorId ON PurchaseOrders(VendorId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_PurchaseOrders_Status ON PurchaseOrders(Status) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_PurchaseOrders_OrderDate ON PurchaseOrders(OrderDate DESC) WHERE IsDeleted = 0;

-- PurchaseOrderLines indexes
CREATE NONCLUSTERED INDEX IX_PurchaseOrderLines_PurchaseOrderId ON PurchaseOrderLines(PurchaseOrderId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_PurchaseOrderLines_ProductVariantId ON PurchaseOrderLines(ProductVariantId) WHERE IsDeleted = 0;

-- GoodsReceiptNotes indexes
CREATE NONCLUSTERED INDEX IX_GoodsReceiptNotes_GRNNumber ON GoodsReceiptNotes(GRNNumber) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_GoodsReceiptNotes_PurchaseOrderId ON GoodsReceiptNotes(PurchaseOrderId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_GoodsReceiptNotes_ReceivedDate ON GoodsReceiptNotes(ReceivedDate DESC) WHERE IsDeleted = 0;

-- GoodsReceiptLines indexes
CREATE NONCLUSTERED INDEX IX_GoodsReceiptLines_GoodsReceiptNoteId ON GoodsReceiptLines(GoodsReceiptNoteId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_GoodsReceiptLines_ProductVariantId ON GoodsReceiptLines(ProductVariantId) WHERE IsDeleted = 0;

GO

PRINT 'Database indexes created successfully!';
PRINT 'Total indexes created: 40+';
GO
