using ERP.Identity.Domain;
using ERP.Shared;

namespace ERP.Identity.Application.Repositories;

public interface IUserRoleRepository : IGenericRepository<UserRole>
{
    Task<IEnumerable<UserRole>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRole>> GetRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}