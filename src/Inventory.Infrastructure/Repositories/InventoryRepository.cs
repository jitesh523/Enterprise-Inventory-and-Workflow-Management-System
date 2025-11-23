using Inventory.Domain.Entities;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

/// <summary>
/// Specialized repository for Inventory operations
/// </summary>
public class InventoryRepository : Repository<InventoryStock>
{
    private readonly DapperContext _dapperContext;

    public InventoryRepository(ApplicationDbContext context, DapperContext dapperContext) 
        : base(context)
    {
        _dapperContext = dapperContext;
    }

    /// <summary>
    /// Get inventory stock for a product variant across all locations
    /// </summary>
    public async Task<IEnumerable<InventoryStock>> GetStockByProductVariantAsync(
        int productVariantId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Location)
                .ThenInclude(l => l!.Warehouse)
            .Include(i => i.ProductVariant)
            .Where(i => i.ProductVariantId == productVariantId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get inventory stock by warehouse
    /// </summary>
    public async Task<IEnumerable<InventoryStock>> GetStockByWarehouseAsync(
        int warehouseId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.ProductVariant)
                .ThenInclude(pv => pv!.Product)
            .Include(i => i.Location)
            .Where(i => i.WarehouseId == warehouseId)
            .OrderBy(i => i.ProductVariant!.SKU)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get low stock items (below reorder point)
    /// </summary>
    public async Task<IEnumerable<InventoryStock>> GetLowStockItemsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.ProductVariant)
                .ThenInclude(pv => pv!.Product)
            .Include(i => i.Warehouse)
            .Where(i => i.QuantityOnHand <= i.ProductVariant!.ReorderPoint)
            .OrderBy(i => i.QuantityOnHand)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Allocate inventory for an order using stored procedure (with pessimistic locking)
    /// </summary>
    public async Task<bool> AllocateInventoryAsync(
        int orderId, 
        int warehouseId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _dapperContext.QuerySingleOrDefaultStoredProcedureAsync<AllocationResult>(
                "sp_AllocateInventory",
                new { OrderId = orderId, WarehouseId = warehouseId },
                cancellationToken);

            return result?.Success == 1;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get total available quantity for a product variant
    /// </summary>
    public async Task<decimal> GetTotalAvailableQuantityAsync(
        int productVariantId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.ProductVariantId == productVariantId)
            .SumAsync(i => i.QuantityOnHand - i.QuantityAllocated, cancellationToken);
    }

    private class AllocationResult
    {
        public int Success { get; set; }
        public string? Message { get; set; }
    }
}
