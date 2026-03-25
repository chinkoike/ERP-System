using ERP.Identity.Application.Repositories;
using ERP.Identity.Domain;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Identity.Infrastructure.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be null or empty", nameof(name));

        var roles = await FindAsync(r => r.Name == name, cancellationToken);
        return roles.FirstOrDefault();
    }

    public async Task<IEnumerable<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
    {
        return await FindAsync(r => r.IsActive, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be null or empty", nameof(name));

        return await ExistsAsync(r => r.Name == name, cancellationToken);
    }
}