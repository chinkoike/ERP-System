using ERP.Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERP.Finance.Infrastructure.Data;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
    }

    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.Payments)
            .WithOne(p => p.Invoice!)
            .HasForeignKey(p => p.InvoiceId);

        modelBuilder.Entity<Invoice>().Property(i => i.InvoiceNumber).IsRequired();
        modelBuilder.Entity<Account>().Property(a => a.AccountCode).IsRequired();

        modelBuilder.Entity<Account>()
        .Property(a => a.Balance)
        .HasPrecision(18, 2);

        // สำหรับตาราง Invoice (TotalAmount และ AmountDue)
        modelBuilder.Entity<Invoice>()
            .Property(i => i.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Invoice>()
            .Property(i => i.AmountDue)
            .HasPrecision(18, 2);

        // สำหรับตาราง Payment (AmountPaid)
        modelBuilder.Entity<Payment>()
            .Property(p => p.AmountPaid)
            .HasPrecision(18, 2);
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }
}
