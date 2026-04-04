using ERP.Inventory.Domain;
using ERP.Shared;

namespace ERP.Inventory.Application.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetCategoriesWithProductsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<int> GetProductCountAsync(Guid categoryId, CancellationToken cancellationToken = default);
}