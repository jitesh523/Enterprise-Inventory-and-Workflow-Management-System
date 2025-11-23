using Inventory.Domain.Enums;

namespace Inventory.Domain.Entities;

/// <summary>
/// Represents a physical warehouse location
/// </summary>
public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsNettable { get; set; } = true;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Location> Locations { get; set; } = new List<Location>();
    public ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();
}

/// <summary>
/// Represents a specific bin location within a warehouse
/// </summary>
public class Location : BaseEntity
{
    public int WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public ZoneType ZoneType { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Warehouse? Warehouse { get; set; }
    public ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();
}

/// <summary>
/// Represents the current inventory stock snapshot
/// </summary>
public class InventoryStock : BaseEntity
{
    public int ProductVariantId { get; set; }
    public int LocationId { get; set; }
    public int WarehouseId { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityAllocated { get; set; }
    
    /// <summary>
    /// Computed: QuantityOnHand - QuantityAllocated
    /// </summary>
    public decimal QuantityAvailable => QuantityOnHand - QuantityAllocated;

    // Navigation properties
    public ProductVariant? ProductVariant { get; set; }
    public Location? Location { get; set; }
    public Warehouse? Warehouse { get; set; }
}

/// <summary>
/// Represents an immutable inventory transaction (audit trail)
/// </summary>
public class InventoryTransaction : BaseEntity
{
    public DateTime TransactionDate { get; set; }
    public TransactionType Type { get; set; }
    public int ProductVariantId { get; set; }
    public int LocationId { get; set; }
    public decimal QuantityChange { get; set; }
    public decimal UnitCost { get; set; }
    public string? ReferenceDocType { get; set; }
    public int? ReferenceDocId { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public ProductVariant? ProductVariant { get; set; }
    public Location? Location { get; set; }
}

/// <summary>
/// Represents a stock adjustment
/// </summary>
public class StockAdjustment : BaseEntity
{
    public DateTime AdjustmentDate { get; set; }
    public int ProductVariantId { get; set; }
    public int LocationId { get; set; }
    public decimal QuantityAdjusted { get; set; }
    public AdjustmentReasonCode ReasonCode { get; set; }
    public string? Notes { get; set; }
    public string AdjustedBy { get; set; } = string.Empty;

    // Navigation properties
    public ProductVariant? ProductVariant { get; set; }
    public Location? Location { get; set; }
}

/// <summary>
/// Represents a transfer order between warehouses
/// </summary>
public class TransferOrder : BaseEntity
{
    public string TransferNumber { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public string Status { get; set; } = "Draft";
    public string? Notes { get; set; }

    // Navigation properties
    public Warehouse? FromWarehouse { get; set; }
    public Warehouse? ToWarehouse { get; set; }
    public ICollection<TransferOrderLine> Lines { get; set; } = new List<TransferOrderLine>();
}

/// <summary>
/// Represents a line item in a transfer order
/// </summary>
public class TransferOrderLine : BaseEntity
{
    public int TransferOrderId { get; set; }
    public int ProductVariantId { get; set; }
    public decimal Quantity { get; set; }

    // Navigation properties
    public TransferOrder? TransferOrder { get; set; }
    public ProductVariant? ProductVariant { get; set; }
}
