-- =============================================
-- Enterprise Inventory Management System
-- Stored Procedure: sp_ProcessGoodsReceipt
-- Purpose: Process goods receipt and update inventory with moving average cost
-- =============================================

USE InventoryDB;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ProcessGoodsReceipt]
    @GoodsReceiptNoteId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Update InventoryStock: Increase QuantityOnHand
        UPDATE i
        SET i.QuantityOnHand = i.QuantityOnHand + grl.QuantityReceived,
            i.UpdatedAt = GETUTCDATE()
        FROM InventoryStock i
        JOIN GoodsReceiptLines grl ON i.ProductVariantId = grl.ProductVariantId 
                                   AND i.LocationId = grl.LocationId
        WHERE grl.GoodsReceiptNoteId = @GoodsReceiptNoteId;

        -- Update PurchaseOrderLines: Increase QuantityReceived
        UPDATE pol
        SET pol.QuantityReceived = pol.QuantityReceived + grl.QuantityReceived,
            pol.UpdatedAt = GETUTCDATE()
        FROM PurchaseOrderLines pol
        JOIN GoodsReceiptLines grl ON pol.Id = grl.PurchaseOrderLineId
        WHERE grl.GoodsReceiptNoteId = @GoodsReceiptNoteId;

        -- Update ProductVariant CostPrice using Moving Weighted Average
        UPDATE pv
        SET pv.CostPrice = 
            CASE 
                WHEN (i.QuantityOnHand - grl.QuantityReceived) <= 0 THEN pol.UnitPrice
                ELSE (
                    ((i.QuantityOnHand - grl.QuantityReceived) * pv.CostPrice) + 
                    (grl.QuantityReceived * pol.UnitPrice)
                ) / i.QuantityOnHand
            END,
            pv.UpdatedAt = GETUTCDATE()
        FROM ProductVariants pv
        JOIN GoodsReceiptLines grl ON pv.Id = grl.ProductVariantId
        JOIN PurchaseOrderLines pol ON grl.PurchaseOrderLineId = pol.Id
        JOIN InventoryStock i ON pv.Id = i.ProductVariantId AND grl.LocationId = i.LocationId
        WHERE grl.GoodsReceiptNoteId = @GoodsReceiptNoteId;

        -- Insert InventoryTransactions for audit trail
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
            1, -- Type = Purchase
            grl.ProductVariantId,
            grl.LocationId,
            grl.QuantityReceived,
            pol.UnitPrice,
            'GRN',
            @GoodsReceiptNoteId,
            GETUTCDATE()
        FROM GoodsReceiptLines grl
        JOIN PurchaseOrderLines pol ON grl.PurchaseOrderLineId = pol.Id
        WHERE grl.GoodsReceiptNoteId = @GoodsReceiptNoteId;

        -- Update PurchaseOrder Status
        -- If all lines are fully received, set status to Closed (5)
        -- Otherwise, set to PartiallyReceived (4)
        DECLARE @PurchaseOrderId INT;
        SELECT @PurchaseOrderId = PurchaseOrderId 
        FROM GoodsReceiptNotes 
        WHERE Id = @GoodsReceiptNoteId;

        DECLARE @AllReceived BIT = 0;
        IF NOT EXISTS (
            SELECT 1 
            FROM PurchaseOrderLines 
            WHERE PurchaseOrderId = @PurchaseOrderId 
              AND QuantityReceived < QuantityOrdered
        )
        BEGIN
            SET @AllReceived = 1;
        END

        UPDATE PurchaseOrders
        SET Status = CASE WHEN @AllReceived = 1 THEN 5 ELSE 4 END, -- 5=Closed, 4=PartiallyReceived
            UpdatedAt = GETUTCDATE()
        WHERE Id = @PurchaseOrderId;

        COMMIT TRANSACTION;

        SELECT 1 AS Success, 'Goods receipt processed successfully' AS Message;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        SELECT 0 AS Success, @ErrorMessage AS Message;
        
        THROW;
    END CATCH
END
GO

PRINT 'Stored procedure sp_ProcessGoodsReceipt created successfully!';
GO
