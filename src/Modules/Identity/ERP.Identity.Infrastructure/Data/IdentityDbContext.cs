using Microsoft.EntityFrameworkCore;
using ERP.Identity.Domain;

namespace ERP.Identity.Infrastructure.Data;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure User entity
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Configure Role entity
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();

        // Configure UserRole many-to-many relationship
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed default roles
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Admin",
                Description = "Administrator with full access",
                IsActive = true,
                CreatedAt = new DateTime(2026, 3, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new Role
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Manager",
                Description = "Manager with operational access",
                IsActive = true,
                CreatedAt = new DateTime(2026, 3, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new Role
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "User",
                Description = "Regular user with limited access",
                IsActive = true,
                CreatedAt = new DateTime(2026, 3, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            }
        );
    }
}


