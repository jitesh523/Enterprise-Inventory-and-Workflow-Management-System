namespace Inventory.Domain.Enums;

/// <summary>
/// Represents the lifecycle states of an order
/// </summary>
public enum OrderStatus
{
    Draft = 0,
    Confirmed = 1,
    Allocated = 2,
    Picked = 3,
    Packed = 4,
    Shipped = 5,
    Invoiced = 6,
    Cancelled = 99
}

/// <summary>
/// Represents the lifecycle states of a purchase order
/// </summary>
public enum PurchaseOrderStatus
{
    Draft = 0,
    Submitted = 1,
    PendingApproval = 2,
    Approved = 3,
    PartiallyReceived = 4,
    Closed = 5,
    Cancelled = 99
}

/// <summary>
/// Types of inventory transactions
/// </summary>
public enum TransactionType
{
    Purchase = 1,
    Sale = 2,
    Adjustment = 3,
    Transfer = 4,
    Allocation = 5,
    Return = 6
}

/// <summary>
/// Zone types within a warehouse
/// </summary>
public enum ZoneType
{
    Receiving = 1,
    BulkStorage = 2,
    Picking = 3,
    Packing = 4,
    Shipping = 5,
    Quarantine = 6
}

/// <summary>
/// Reason codes for stock adjustments
/// </summary>
public enum AdjustmentReasonCode
{
    Damaged = 1,
    Expired = 2,
    Lost = 3,
    Found = 4,
    CycleCountCorrection = 5,
    Other = 99
}
