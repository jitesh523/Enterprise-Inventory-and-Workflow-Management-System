using Inventory.Domain.Entities;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

/// <summary>
/// Specialized repository for Order entities with custom queries
/// </summary>
public class OrderRepository : Repository<Order>
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get order with all related data (lines, customer, product variants)
    /// </summary>
    public async Task<Order?> GetOrderWithDetailsAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Lines)
                .ThenInclude(l => l.ProductVariant)
                    .ThenInclude(pv => pv!.Product)
            .Include(o => o.Warehouse)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    /// <summary>
    /// Get orders by status
    /// </summary>
    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(
        Domain.Enums.OrderStatus status, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Lines)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get orders by customer
    /// </summary>
    public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(
        int customerId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Lines)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get recent orders with pagination
    /// </summary>
    public async Task<IEnumerable<Order>> GetRecentOrdersAsync(
        int pageNumber = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
