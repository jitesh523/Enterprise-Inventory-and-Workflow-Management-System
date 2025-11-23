using Inventory.Domain.Entities;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

/// <summary>
/// Specialized repository for Purchase Order operations
/// </summary>
public class PurchaseOrderRepository : Repository<PurchaseOrder>
{
    private readonly DapperContext _dapperContext;

    public PurchaseOrderRepository(ApplicationDbContext context, DapperContext dapperContext) 
        : base(context)
    {
        _dapperContext = dapperContext;
    }

    /// <summary>
    /// Get purchase order with all related data
    /// </summary>
    public async Task<PurchaseOrder?> GetPurchaseOrderWithDetailsAsync(
        int purchaseOrderId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(po => po.Vendor)
            .Include(po => po.Lines)
                .ThenInclude(l => l.ProductVariant)
                    .ThenInclude(pv => pv!.Product)
            .Include(po => po.GoodsReceipts)
            .FirstOrDefaultAsync(po => po.Id == purchaseOrderId, cancellationToken);
    }

    /// <summary>
    /// Get purchase orders by status
    /// </summary>
    public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(
        Domain.Enums.PurchaseOrderStatus status, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(po => po.Vendor)
            .Include(po => po.Lines)
            .Where(po => po.Status == status)
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get purchase orders by vendor
    /// </summary>
    public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersByVendorAsync(
        int vendorId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(po => po.Lines)
            .Where(po => po.VendorId == vendorId)
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Process goods receipt using stored procedure
    /// </summary>
    public async Task<bool> ProcessGoodsReceiptAsync(
        int goodsReceiptNoteId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _dapperContext.QuerySingleOrDefaultStoredProcedureAsync<ProcessResult>(
                "sp_ProcessGoodsReceipt",
                new { GoodsReceiptNoteId = goodsReceiptNoteId },
                cancellationToken);

            return result?.Success == 1;
        }
        catch
        {
            return false;
        }
    }

    private class ProcessResult
    {
        public int Success { get; set; }
        public string? Message { get; set; }
    }
}
