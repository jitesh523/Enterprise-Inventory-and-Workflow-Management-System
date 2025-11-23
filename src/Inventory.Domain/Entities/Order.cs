using Inventory.Domain.Enums;
using Inventory.Domain.Events;
using Inventory.Domain.Exceptions;

namespace Inventory.Domain.Entities;

/// <summary>
/// Represents a sales order with workflow state machine
/// </summary>
public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public int CustomerId { get; set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public int? WarehouseId { get; set; }

    // Navigation properties
    public Customer? Customer { get; set; }
    public Warehouse? Warehouse { get; set; }
    public ICollection<OrderLine> Lines { get; private set; } = new List<OrderLine>();

    /// <summary>
    /// Confirms the order (Draft -> Confirmed)
    /// </summary>
    public void Confirm()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("Only draft orders can be confirmed.");

        if (!Lines.Any())
            throw new DomainException("Cannot confirm an empty order.");

        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderConfirmedEvent(this));
    }

    /// <summary>
    /// Marks the order as allocated (Confirmed -> Allocated)
    /// </summary>
    public void Allocate()
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainException("Only confirmed orders can be allocated.");

        Status = OrderStatus.Allocated;
        AddDomainEvent(new OrderAllocatedEvent(this));
    }

    /// <summary>
    /// Marks the order as picked (Allocated -> Picked)
    /// </summary>
    public void Pick()
    {
        if (Status != OrderStatus.Allocated)
            throw new DomainException("Only allocated orders can be picked.");

        Status = OrderStatus.Picked;
    }

    /// <summary>
    /// Marks the order as packed (Picked -> Packed)
    /// </summary>
    public void Pack()
    {
        if (Status != OrderStatus.Picked)
            throw new DomainException("Only picked orders can be packed.");

        Status = OrderStatus.Packed;
    }

    /// <summary>
    /// Ships the order (Packed -> Shipped)
    /// </summary>
    public void Ship()
    {
        if (Status != OrderStatus.Packed)
            throw new DomainException("Order must be packed before shipping.");

        Status = OrderStatus.Shipped;
        AddDomainEvent(new OrderShippedEvent(this));
    }

    /// <summary>
    /// Marks the order as invoiced (Shipped -> Invoiced)
    /// </summary>
    public void Invoice()
    {
        if (Status != OrderStatus.Shipped)
            throw new DomainException("Only shipped orders can be invoiced.");

        Status = OrderStatus.Invoiced;
    }

    /// <summary>
    /// Cancels the order
    /// </summary>
    public void Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Invoiced)
            throw new DomainException("Cannot cancel shipped or invoiced orders.");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(this));
    }

    /// <summary>
    /// Adds a line item to the order
    /// </summary>
    public void AddLine(int productVariantId, decimal quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("Can only add lines to draft orders.");

        var line = new OrderLine
        {
            ProductVariantId = productVariantId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            LineTotal = quantity * unitPrice
        };

        Lines.Add(line);
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        TotalAmount = Lines.Sum(l => l.LineTotal);
    }
}

/// <summary>
/// Represents a line item in an order
/// </summary>
public class OrderLine : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductVariantId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Order? Order { get; set; }
    public ProductVariant? ProductVariant { get; set; }
}

/// <summary>
/// Represents a customer
/// </summary>
public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? TaxId { get; set; }

    // Navigation properties
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
