using Microsoft.EntityFrameworkCore;
using ERP.Sales.Domain;
using ERP.Inventory.Domain;
namespace ERP.Sales.Infrastructure.Data;

public class SalesDbContext(DbContextOptions<SalesDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>();
        // 1. กำหนดความละเอียดของเงิน (Decimal Precision)
        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);

        // อย่าลืมตัวนี้ครับ! เพราะเป็นฟิลด์ใหม่ที่เราเพิ่งเพิ่มเข้าไป
        modelBuilder.Entity<OrderItem>().Property(i => i.TotalPrice).HasPrecision(18, 2);

        // 2. Configure relationships
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade); // เมื่อลบ Order ให้ลบ Item ทิ้งด้วยอัตโนมัติ
    }
}