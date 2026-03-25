using ERP.Inventory.Domain;
using ERP.Shared;

namespace ERP.Inventory.Application.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task UpdateStockAsync(Guid productId, int newStock, CancellationToken cancellationToken = default);
}