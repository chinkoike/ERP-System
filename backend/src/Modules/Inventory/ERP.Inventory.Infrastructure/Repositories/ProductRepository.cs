using ERP.Inventory.Application.Repositories;
using ERP.Inventory.Domain;
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