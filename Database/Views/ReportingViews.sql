-- =============================================
-- Enterprise Inventory Management System
-- Reporting Views
-- Phase 4: Performance-Optimized Reporting
-- =============================================

USE InventoryDB;
GO

-- =============================================
-- View: v_MonthlySales
-- Purpose: Pre-aggregated monthly sales data for dashboards
-- =============================================
CREATE OR ALTER VIEW dbo.v_MonthlySales
WITH SCHEMABINDING
AS
SELECT
    YEAR(OrderDate) AS Year,
    MONTH(OrderDate) AS Month,
    SUM(TotalAmount) AS Revenue,
    COUNT_BIG(*) AS OrderCount
FROM dbo.Orders
WHERE IsDeleted = 0 AND Status >= 5 -- Shipped or Invoiced
GROUP BY YEAR(OrderDate), MONTH(OrderDate);
GO

-- Create clustered index on the view for materialization
CREATE UNIQUE CLUSTERED INDEX IX_v_MonthlySales 
ON dbo.v_MonthlySales (Year, Month);
GO

-- =============================================
-- View: v_InventoryValuation
-- Purpose: Current inventory value by product
-- =============================================
CREATE OR ALTER VIEW dbo.v_InventoryValuation
AS
SELECT
    pv.Id AS ProductVariantId,
    pv.SKU,
    p.Name AS ProductName,
    b.Name AS BrandName,
    c.Name AS CategoryName,
    SUM(i.QuantityOnHand) AS TotalQuantityOnHand,
    SUM(i.QuantityAllocated) AS TotalQuantityAllocated,
    SUM(i.QuantityOnHand - i.QuantityAllocated) AS TotalQuantityAvailable,
    pv.CostPrice,
    SUM(i.QuantityOnHand * pv.CostPrice) AS TotalInventoryValue
FROM dbo.InventoryStock i
JOIN dbo.ProductVariants pv ON i.ProductVariantId = pv.Id
JOIN dbo.Products p ON pv.ProductId = p.Id
LEFT JOIN dbo.Brands b ON p.BrandId = b.Id
LEFT JOIN dbo.Categories c ON p.CategoryId = c.Id
WHERE i.IsDeleted = 0 AND pv.IsDeleted = 0 AND p.IsDeleted = 0
GROUP BY 
    pv.Id, pv.SKU, p.Name, b.Name, c.Name, pv.CostPrice;
GO

-- =============================================
-- View: v_VendorPerformance
-- Purpose: Vendor ratings and performance metrics
-- =============================================
CREATE OR ALTER VIEW dbo.v_VendorPerformance
AS
SELECT
    v.Id AS VendorId,
    v.Name AS VendorName,
    v.Rating,
    COUNT(DISTINCT po.Id) AS TotalPurchaseOrders,
    SUM(po.TotalAmount) AS TotalPurchaseValue,
    COUNT(DISTINCT grn.Id) AS TotalGoodsReceipts,
    AVG(DATEDIFF(DAY, po.OrderDate, grn.ReceivedDate)) AS AvgDeliveryDays,
    SUM(CASE WHEN po.Status = 5 THEN 1 ELSE 0 END) AS ClosedOrders,
    SUM(CASE WHEN po.Status = 99 THEN 1 ELSE 0 END) AS CancelledOrders
FROM dbo.Vendors v
LEFT JOIN dbo.PurchaseOrders po ON v.Id = po.VendorId AND po.IsDeleted = 0
LEFT JOIN dbo.GoodsReceiptNotes grn ON po.Id = grn.PurchaseOrderId AND grn.IsDeleted = 0
WHERE v.IsDeleted = 0
GROUP BY v.Id, v.Name, v.Rating;
GO

-- =============================================
-- View: v_LowStockItems
-- Purpose: Products below reorder point
-- =============================================
CREATE OR ALTER VIEW dbo.v_LowStockItems
AS
SELECT
    pv.Id AS ProductVariantId,
    pv.SKU,
    p.Name AS ProductName,
    w.Name AS WarehouseName,
    SUM(i.QuantityOnHand) AS CurrentStock,
    pv.ReorderPoint,
    pv.ReorderQuantity,
    (pv.ReorderPoint - SUM(i.QuantityOnHand)) AS ShortageQuantity
FROM dbo.InventoryStock i
JOIN dbo.ProductVariants pv ON i.ProductVariantId = pv.Id
JOIN dbo.Products p ON pv.ProductId = p.Id
JOIN dbo.Warehouses w ON i.WarehouseId = w.Id
WHERE i.IsDeleted = 0 
  AND pv.IsDeleted = 0 
  AND p.IsDeleted = 0
  AND pv.IsActive = 1
GROUP BY pv.Id, pv.SKU, p.Name, w.Name, pv.ReorderPoint, pv.ReorderQuantity
HAVING SUM(i.QuantityOnHand) <= pv.ReorderPoint;
GO

-- =============================================
-- View: v_OrderSummary
-- Purpose: Order summary with customer and status
-- =============================================
CREATE OR ALTER VIEW dbo.v_OrderSummary
AS
SELECT
    o.Id AS OrderId,
    o.OrderNumber,
    o.OrderDate,
    c.Name AS CustomerName,
    c.Email AS CustomerEmail,
    o.Status,
    CASE o.Status
        WHEN 0 THEN 'Draft'
        WHEN 1 THEN 'Confirmed'
        WHEN 2 THEN 'Allocated'
        WHEN 3 THEN 'Picked'
        WHEN 4 THEN 'Packed'
        WHEN 5 THEN 'Shipped'
        WHEN 6 THEN 'Invoiced'
        WHEN 99 THEN 'Cancelled'
    END AS StatusName,
    o.TotalAmount,
    COUNT(ol.Id) AS LineItemCount,
    w.Name AS WarehouseName
FROM dbo.Orders o
JOIN dbo.Customers c ON o.CustomerId = c.Id
LEFT JOIN dbo.OrderLines ol ON o.Id = ol.OrderId AND ol.IsDeleted = 0
LEFT JOIN dbo.Warehouses w ON o.WarehouseId = w.Id
WHERE o.IsDeleted = 0 AND c.IsDeleted = 0
GROUP BY 
    o.Id, o.OrderNumber, o.OrderDate, c.Name, c.Email, 
    o.Status, o.TotalAmount, w.Name;
GO

-- =============================================
-- View: v_TopSellingProducts
-- Purpose: Best selling products by quantity and revenue
-- =============================================
CREATE OR ALTER VIEW dbo.v_TopSellingProducts
AS
SELECT
    pv.Id AS ProductVariantId,
    pv.SKU,
    p.Name AS ProductName,
    b.Name AS BrandName,
    c.Name AS CategoryName,
    SUM(ol.Quantity) AS TotalQuantitySold,
    SUM(ol.LineTotal) AS TotalRevenue,
    COUNT(DISTINCT o.Id) AS NumberOfOrders,
    AVG(ol.UnitPrice) AS AverageSellingPrice
FROM dbo.OrderLines ol
JOIN dbo.Orders o ON ol.OrderId = o.Id
JOIN dbo.ProductVariants pv ON ol.ProductVariantId = pv.Id
JOIN dbo.Products p ON pv.ProductId = p.Id
LEFT JOIN dbo.Brands b ON p.BrandId = b.Id
LEFT JOIN dbo.Categories c ON p.CategoryId = c.Id
WHERE ol.IsDeleted = 0 
  AND o.IsDeleted = 0 
  AND o.Status >= 5 -- Shipped or Invoiced
  AND pv.IsDeleted = 0
GROUP BY pv.Id, pv.SKU, p.Name, b.Name, c.Name;
GO

PRINT 'Reporting views created successfully!';
PRINT 'Total views created: 6';
GO
