using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext for the Inventory Management System
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Product Management
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
    public DbSet<TaxGroup> TaxGroups => Set<TaxGroup>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();

    // Warehouse and Inventory
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<InventoryStock> InventoryStocks => Set<InventoryStock>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<TransferOrder> TransferOrders => Set<TransferOrder>();
    public DbSet<TransferOrderLine> TransferOrderLines => Set<TransferOrderLine>();

    // Order Management
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    // Procurement
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();
    public DbSet<GoodsReceiptNote> GoodsReceiptNotes => Set<GoodsReceiptNote>();
    public DbSet<GoodsReceiptLine> GoodsReceiptLines => Set<GoodsReceiptLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)), parameter);
                
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }

        ConfigureProductManagement(modelBuilder);
        ConfigureInventoryManagement(modelBuilder);
        ConfigureOrderManagement(modelBuilder);
        ConfigureProcurement(modelBuilder);
    }

    private void ConfigureProductManagement(ModelBuilder modelBuilder)
    {
        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasColumnType("nvarchar(max)");
            
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(e => e.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Variants)
                .WithOne(v => v.Product)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ProductVariant configuration
        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.Property(e => e.SKU).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Barcode).HasMaxLength(100);
            entity.Property(e => e.CostPrice).HasPrecision(18, 4);
            entity.Property(e => e.SalesPrice).HasPrecision(18, 4);

            entity.HasIndex(e => e.SKU).IsUnique();
            entity.HasIndex(e => e.Barcode);
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            
            entity.HasOne(e => e.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(e => e.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Brand configuration
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
        });

        // UnitOfMeasure configuration
        modelBuilder.Entity<UnitOfMeasure>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // TaxGroup configuration
        modelBuilder.Entity<TaxGroup>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.TaxRate).HasPrecision(5, 2);
        });

        // ProductAttribute configuration
        modelBuilder.Entity<ProductAttribute>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        });

        // ProductAttributeValue configuration
        modelBuilder.Entity<ProductAttributeValue>(entity =>
        {
            entity.Property(e => e.Value).HasMaxLength(100).IsRequired();
        });
    }

    private void ConfigureInventoryManagement(ModelBuilder modelBuilder)
    {
        // Warehouse configuration
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(500).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Location configuration
        modelBuilder.Entity<Location>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
            entity.HasIndex(e => new { e.WarehouseId, e.Code }).IsUnique();
        });

        // InventoryStock configuration
        modelBuilder.Entity<InventoryStock>(entity =>
        {
            entity.Property(e => e.QuantityOnHand).HasPrecision(18, 2);
            entity.Property(e => e.QuantityAllocated).HasPrecision(18, 2);
            
            entity.Ignore(e => e.QuantityAvailable); // Computed property
            
            entity.HasIndex(e => new { e.ProductVariantId, e.LocationId }).IsUnique();
        });

        // InventoryTransaction configuration
        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.Property(e => e.QuantityChange).HasPrecision(18, 2);
            entity.Property(e => e.UnitCost).HasPrecision(18, 4);
            entity.Property(e => e.ReferenceDocType).HasMaxLength(50);
        });

        // StockAdjustment configuration
        modelBuilder.Entity<StockAdjustment>(entity =>
        {
            entity.Property(e => e.QuantityAdjusted).HasPrecision(18, 2);
            entity.Property(e => e.AdjustedBy).HasMaxLength(256).IsRequired();
        });

        // TransferOrder configuration
        modelBuilder.Entity<TransferOrder>(entity =>
        {
            entity.Property(e => e.TransferNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.HasIndex(e => e.TransferNumber).IsUnique();

            entity.HasOne(e => e.FromWarehouse)
                .WithMany()
                .HasForeignKey(e => e.FromWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ToWarehouse)
                .WithMany()
                .HasForeignKey(e => e.ToWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TransferOrderLine configuration
        modelBuilder.Entity<TransferOrderLine>(entity =>
        {
            entity.Property(e => e.Quantity).HasPrecision(18, 2);
        });
    }

    private void ConfigureOrderManagement(ModelBuilder modelBuilder)
    {
        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.TaxId).HasMaxLength(50);
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.HasIndex(e => e.OrderNumber).IsUnique();

            entity.HasMany(e => e.Lines)
                .WithOne(l => l.Order)
                .HasForeignKey(l => l.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // OrderLine configuration
        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.Property(e => e.Quantity).HasPrecision(18, 2);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.LineTotal).HasPrecision(18, 2);
        });
    }

    private void ConfigureProcurement(ModelBuilder modelBuilder)
    {
        // Vendor configuration
        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.TaxId).HasMaxLength(50);
        });

        // PurchaseOrder configuration
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.Property(e => e.PONumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.HasIndex(e => e.PONumber).IsUnique();

            entity.HasMany(e => e.Lines)
                .WithOne(l => l.PurchaseOrder)
                .HasForeignKey(l => l.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PurchaseOrderLine configuration
        modelBuilder.Entity<PurchaseOrderLine>(entity =>
        {
            entity.Property(e => e.QuantityOrdered).HasPrecision(18, 2);
            entity.Property(e => e.QuantityReceived).HasPrecision(18, 2);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 4);
            entity.Property(e => e.LineTotal).HasPrecision(18, 2);
        });

        // GoodsReceiptNote configuration
        modelBuilder.Entity<GoodsReceiptNote>(entity =>
        {
            entity.Property(e => e.GRNNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.DeliveryNoteNumber).HasMaxLength(50);
            entity.Property(e => e.ReceivedBy).HasMaxLength(256).IsRequired();
            entity.HasIndex(e => e.GRNNumber).IsUnique();

            entity.HasMany(e => e.Lines)
                .WithOne(l => l.GoodsReceiptNote)
                .HasForeignKey(l => l.GoodsReceiptNoteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // GoodsReceiptLine configuration
        modelBuilder.Entity<GoodsReceiptLine>(entity =>
        {
            entity.Property(e => e.QuantityReceived).HasPrecision(18, 2);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set audit fields before saving
        var entries = ChangeTracker.Entries<BaseEntity>();
        
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                // CreatedBy will be set by the audit interceptor
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                // UpdatedBy will be set by the audit interceptor
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
