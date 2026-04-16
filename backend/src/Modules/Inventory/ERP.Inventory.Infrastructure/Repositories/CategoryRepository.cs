using ERP.Inventory.Application.Repositories;
using ERP.Inventory.Domain;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using ERP.Inventory.Application.DTOs;
using ERP.Shared;

namespace ERP.Inventory.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PagedResult<Category>> SearchCategoriesAsync(CategoryFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = Query(); // ใช้ method จาก GenericRepository

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(term) ||
                                    (c.Description != null && c.Description.ToLower().Contains(term)));
        }

        if (filter.CreatedAfter.HasValue)
        {
            query = query.Where(c => c.CreatedAt >= filter.CreatedAfter.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.Name) // Default sort
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Category>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
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