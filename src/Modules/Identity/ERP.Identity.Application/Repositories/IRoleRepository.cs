using ERP.Identity.Domain;
using ERP.Shared;

namespace ERP.Identity.Application.Repositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}