using Inventory.Domain.Enums;

namespace Inventory.Domain.Entities;

/// <summary>
/// Represents a master product (abstract product concept)
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int BaseUOMId { get; set; }
    public int? TaxGroupId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Category? Category { get; set; }
    public Brand? Brand { get; set; }
    public UnitOfMeasure? BaseUOM { get; set; }
    public TaxGroup? TaxGroup { get; set; }
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
}

/// <summary>
/// Represents a sellable product variant (specific SKU)
/// </summary>
public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalesPrice { get; set; }
    public int ReorderPoint { get; set; }
    public int ReorderQuantity { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Product? Product { get; set; }
    public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
    public ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();
}

/// <summary>
/// Represents a product category
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }

    // Navigation properties
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

/// <summary>
/// Represents a product brand
/// </summary>
public class Brand : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

/// <summary>
/// Represents a unit of measure
/// </summary>
public class UnitOfMeasure : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// Represents a tax group
/// </summary>
public class TaxGroup : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Represents a dynamic product attribute (e.g., Color, Size)
/// </summary>
public class ProductAttribute : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
}

/// <summary>
/// Represents a specific attribute value for a product variant
/// </summary>
public class ProductAttributeValue : BaseEntity
{
    public int ProductVariantId { get; set; }
    public int AttributeId { get; set; }
    public string Value { get; set; } = string.Empty;

    // Navigation properties
    public ProductVariant? ProductVariant { get; set; }
    public ProductAttribute? Attribute { get; set; }
}
