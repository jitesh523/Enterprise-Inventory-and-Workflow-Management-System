-- =============================================
-- Enterprise Inventory Management System
-- Stored Procedure: sp_AllocateInventory
-- Purpose: Allocate inventory for an order with pessimistic locking
-- =============================================

USE InventoryDB;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_AllocateInventory]
    @OrderId INT,
    @WarehouseId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON; -- Ensures auto-rollback on error

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Declare variables
        DECLARE @ProductVariantId INT;
        DECLARE @Quantity DECIMAL(18,2);
        DECLARE @AvailableQty DECIMAL(18,2);

        -- PESSIMISTIC LOCK: Lock the inventory rows for products in this order
        -- CRITICAL: UPDLOCK holds lock until commit to prevent race conditions
        SELECT 
            i.ProductVariantId,
            i.QuantityOnHand - i.QuantityAllocated AS QuantityAvailable
        INTO #StockCheck
        FROM InventoryStock i WITH (UPDLOCK, ROWLOCK)
        JOIN OrderLines ol ON i.ProductVariantId = ol.ProductVariantId
        WHERE ol.OrderId = @OrderId 
          AND i.WarehouseId = @WarehouseId
          AND i.IsDeleted = 0;

        -- Validation: Check if any item lacks sufficient stock
        IF EXISTS (
            SELECT 1
            FROM OrderLines ol
            LEFT JOIN #StockCheck sc ON ol.ProductVariantId = sc.ProductVariantId
            WHERE ol.OrderId = @OrderId
              AND (sc.QuantityAvailable IS NULL OR sc.QuantityAvailable < ol.Quantity)
        )
        BEGIN
            -- Rollback and throw error
            ROLLBACK TRANSACTION;
            THROW 51000, 'Insufficient stock for one or more items.', 1;
        END

        -- Execute the allocation: Update QuantityAllocated
        UPDATE i
        SET i.QuantityAllocated = i.QuantityAllocated + ol.Quantity,
            i.UpdatedAt = GETUTCDATE()
        FROM InventoryStock i
        JOIN OrderLines ol ON i.ProductVariantId = ol.ProductVariantId
        WHERE ol.OrderId = @OrderId 
          AND i.WarehouseId = @WarehouseId;

        -- Insert Audit Log into InventoryTransactions
        INSERT INTO InventoryTransactions (
            TransactionDate, 
            Type, 
            ProductVariantId, 
            LocationId, 
            QuantityChange, 
            UnitCost, 
            ReferenceDocType, 
            ReferenceDocId,
            CreatedAt
        )
        SELECT 
            GETUTCDATE(),
            5, -- Type = Allocation
            ol.ProductVariantId,
            i.LocationId,
            0, -- No physical movement yet
            pv.CostPrice,
            'Order',
            @OrderId,
            GETUTCDATE()
        FROM OrderLines ol
        JOIN InventoryStock i ON ol.ProductVariantId = i.ProductVariantId
        JOIN ProductVariants pv ON ol.ProductVariantId = pv.Id
        WHERE ol.OrderId = @OrderId 
          AND i.WarehouseId = @WarehouseId;

        -- Update Order Status to Allocated (Status = 2)
        UPDATE Orders
        SET Status = 2, -- Allocated
            UpdatedAt = GETUTCDATE()
        WHERE Id = @OrderId;

        COMMIT TRANSACTION;

        -- Return success
        SELECT 1 AS Success, 'Inventory allocated successfully' AS Message;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Return error details
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        SELECT 0 AS Success, @ErrorMessage AS Message;
        
        THROW;
    END CATCH
END
GO

PRINT 'Stored procedure sp_AllocateInventory created successfully!';
GO
