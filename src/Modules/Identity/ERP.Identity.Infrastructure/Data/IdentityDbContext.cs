using Microsoft.EntityFrameworkCore;

namespace ERP.Identity.Infrastructure.Data;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    // TODO: Add DbSets for Identity entities (User, Role, etc.)
    // public DbSet<User> Users => Set<User>();
    // public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: Configure entity relationships and constraints
    }
}