using ERP.Identity.Application.Repositories;
using ERP.Identity.Domain;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Identity.Infrastructure.Repositories;

public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(ur => ur.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(ur => ur.RoleId == roleId, cancellationToken);
    }

    public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow
        };
        await AddAsync(userRole, cancellationToken);
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRoles = await FindAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
        var userRole = userRoles.FirstOrDefault();
        if (userRole != null)
        {
            Remove(userRole);
        }
    }
}