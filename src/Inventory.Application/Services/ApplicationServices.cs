using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class OrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IRepository<Order> orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Order> CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task ConfirmOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null) throw new InvalidOperationException("Order not found");
        
        order.Confirm(); // Domain logic with validation
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ShipOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null) throw new InvalidOperationException("Order not found");
        
        order.Ship(); // Domain logic with validation
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public class VendorService
{
    private readonly IRepository<Vendor> _vendorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VendorService(IRepository<Vendor> vendorRepository, IUnitOfWork unitOfWork)
    {
        _vendorRepository = vendorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Vendor> CreateVendorAsync(Vendor vendor, CancellationToken cancellationToken = default)
    {
        await _vendorRepository.AddAsync(vendor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return vendor;
    }
}

public class InventoryService
{
    private readonly IRepository<InventoryStock> _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(IRepository<InventoryStock> inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<InventoryStock>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _inventoryRepository.FindAsync(
            i => i.QuantityOnHand <= i.ProductVariant!.ReorderPoint,
            cancellationToken);
    }
}
