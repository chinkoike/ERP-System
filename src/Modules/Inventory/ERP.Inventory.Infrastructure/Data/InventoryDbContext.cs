using Microsoft.EntityFrameworkCore;
using ERP.Inventory.Domain;

namespace ERP.Inventory.Infrastructure.Data;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.BasePrice).HasPrecision(18, 2);
    }
}