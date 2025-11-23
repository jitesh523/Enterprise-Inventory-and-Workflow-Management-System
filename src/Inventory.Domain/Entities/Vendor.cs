using Inventory.Domain.Enums;

namespace Inventory.Domain.Entities;

/// <summary>
/// Represents a vendor/supplier
/// </summary>
public class Vendor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
    public int Rating { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}

/// <summary>
/// Represents a purchase order
/// </summary>
public class PurchaseOrder : BaseEntity
{
    public string PONumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public int VendorId { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Vendor? Vendor { get; set; }
    public ICollection<PurchaseOrderLine> Lines { get; set; } = new List<PurchaseOrderLine>();
    public ICollection<GoodsReceiptNote> GoodsReceipts { get; set; } = new List<GoodsReceiptNote>();
}

/// <summary>
/// Represents a line item in a purchase order
/// </summary>
public class PurchaseOrderLine : BaseEntity
{
    public int PurchaseOrderId { get; set; }
    public int ProductVariantId { get; set; }
    public decimal QuantityOrdered { get; set; }
    public decimal QuantityReceived { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    // Navigation properties
    public PurchaseOrder? PurchaseOrder { get; set; }
    public ProductVariant? ProductVariant { get; set; }
}

/// <summary>
/// Represents a goods receipt note (physical arrival of goods)
/// </summary>
public class GoodsReceiptNote : BaseEntity
{
    public string GRNNumber { get; set; } = string.Empty;
    public int PurchaseOrderId { get; set; }
    public DateTime ReceivedDate { get; set; }
    public string? DeliveryNoteNumber { get; set; }
    public string ReceivedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Navigation properties
    public PurchaseOrder? PurchaseOrder { get; set; }
    public ICollection<GoodsReceiptLine> Lines { get; set; } = new List<GoodsReceiptLine>();
}

/// <summary>
/// Represents a line item in a goods receipt note
/// </summary>
public class GoodsReceiptLine : BaseEntity
{
    public int GoodsReceiptNoteId { get; set; }
    public int PurchaseOrderLineId { get; set; }
    public int ProductVariantId { get; set; }
    public decimal QuantityReceived { get; set; }
    public int LocationId { get; set; }

    // Navigation properties
    public GoodsReceiptNote? GoodsReceiptNote { get; set; }
    public PurchaseOrderLine? PurchaseOrderLine { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public Location? Location { get; set; }
}
