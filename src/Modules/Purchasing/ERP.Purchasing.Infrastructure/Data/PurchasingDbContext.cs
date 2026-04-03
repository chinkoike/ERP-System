using Microsoft.EntityFrameworkCore;
using ERP.Purchasing.Domain.Entities;

namespace ERP.Purchasing.Infrastructure.Data;

public class PurchasingDbContext : DbContext
{
    public PurchasingDbContext(DbContextOptions<PurchasingDbContext> options) : base(options)
    {
    }

    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PurchaseOrder>().Property(po => po.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<PurchaseOrderItem>().Property(poi => poi.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<PurchaseOrderItem>().Property(poi => poi.TotalPrice).HasPrecision(18, 2);

        modelBuilder.Entity<PurchaseOrder>()
            .HasMany(po => po.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(po => po.Supplier)
            .WithMany(s => s.PurchaseOrders)
            .HasForeignKey(po => po.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
