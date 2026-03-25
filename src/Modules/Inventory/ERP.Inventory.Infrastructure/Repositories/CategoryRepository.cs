using ERP.Inventory.Application.Repositories;
using ERP.Inventory.Domain;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Inventory.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be null or empty", nameof(name));

        var categories = await FindAsync(c => c.Name == name, cancellationToken);
        return categories.FirstOrDefault();
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithProductsAsync(CancellationToken cancellationToken = default)
    {
        // Note: This would require a more specific query with includes
        // For now, return all categories - in a real implementation you'd use
        // the specific DbContext to include related products
        return await GetAllAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be null or empty", nameof(name));

        return await ExistsAsync(c => c.Name == name, cancellationToken);
    }

    public Task<int> GetProductCountAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        // Note: This would require a more specific implementation
        // For now, return 0 - in a real implementation you'd query the products
        return Task.FromResult(0);
    }
}