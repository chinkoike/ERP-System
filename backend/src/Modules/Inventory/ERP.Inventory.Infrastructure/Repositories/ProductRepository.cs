using ERP.Inventory.Application.DTOs;
using ERP.Inventory.Application.Repositories;
using ERP.Inventory.Domain;
using ERP.Shared;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Inventory.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be null or empty", nameof(sku));

        var products = await FindAsync(p => p.SKU == sku, cancellationToken);
        return products.FirstOrDefault();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(p => p.CategoryId == categoryId, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
    {
        return await FindAsync(p => p.CurrentStock <= threshold, cancellationToken);
    }

    public async Task<PagedResult<Product>> SearchProductsAsync(ProductFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = DbContext.Set<Product>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.SKU.ToLower().Contains(term) ||
                (p.Description != null && p.Description.ToLower().Contains(term)));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
        }

        if (filter.MinStock.HasValue)
        {
            query = query.Where(p => p.CurrentStock >= filter.MinStock.Value);
        }

        if (filter.MaxStock.HasValue)
        {
            query = query.Where(p => p.CurrentStock <= filter.MaxStock.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(filter.PageNumber, 1);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be null or empty", nameof(sku));

        return await ExistsAsync(p => p.SKU == sku, cancellationToken);
    }

    public async Task UpdateStockAsync(Guid productId, int newStock, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(productId, cancellationToken);
        if (product != null)
        {
            product.CurrentStock = newStock;
            product.LastModifiedAt = DateTime.UtcNow;
            Update(product);
        }
    }
}