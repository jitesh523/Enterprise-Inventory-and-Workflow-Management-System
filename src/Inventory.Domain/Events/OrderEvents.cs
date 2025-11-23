using Inventory.Domain.Entities;

namespace Inventory.Domain.Events;

/// <summary>
/// Event raised when an order is confirmed
/// </summary>
public class OrderConfirmedEvent : IDomainEvent
{
    public Order Order { get; }
    public DateTime OccurredOn { get; }

    public OrderConfirmedEvent(Order order)
    {
        Order = order;
        OccurredOn = DateTime.UtcNow;
    }
}

/// <summary>
/// Event raised when an order is allocated
/// </summary>
public class OrderAllocatedEvent : IDomainEvent
{
    public Order Order { get; }
    public DateTime OccurredOn { get; }

    public OrderAllocatedEvent(Order order)
    {
        Order = order;
        OccurredOn = DateTime.UtcNow;
    }
}

/// <summary>
/// Event raised when an order is shipped
/// </summary>
public class OrderShippedEvent : IDomainEvent
{
    public Order Order { get; }
    public DateTime OccurredOn { get; }

    public OrderShippedEvent(Order order)
    {
        Order = order;
        OccurredOn = DateTime.UtcNow;
    }
}

/// <summary>
/// Event raised when an order is cancelled
/// </summary>
public class OrderCancelledEvent : IDomainEvent
{
    public Order Order { get; }
    public DateTime OccurredOn { get; }

    public OrderCancelledEvent(Order order)
    {
        Order = order;
        OccurredOn = DateTime.UtcNow;
    }
}
