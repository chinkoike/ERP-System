using ERP.Shared;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Inventory.Domain;

namespace ERP.Inventory.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // Product operations
    public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var productRepository = _unitOfWork.Repository<Product>();
        return await productRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Product?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var productRepository = _unitOfWork.Repository<Product>();
        var products = await productRepository.FindAsync(p => p.SKU == sku, cancellationToken);
        return products.FirstOrDefault();
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var productRepository = _unitOfWork.Repository<Product>();
        return await productRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var productRepository = _unitOfWork.Repository<Product>();
        return await productRepository.FindAsync(p => p.CategoryId == categoryId, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
    {
        var productRepository = _unitOfWork.Repository<Product>();
        return await productRepository.FindAsync(p => p.CurrentStock <= threshold, cancellationToken);
    }

    public async Task CreateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        var productRepository = _unitOfWork.Repository<Product>();
        await productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        var productRepository = _unitOfWork.Repository<Product>();
        productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        var productRepository = _unitOfWork.Repository<Product>();
        productRepository.Remove(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var productRepository = _unitOfWork.Repository<Product>();
        return await productRepository.ExistsAsync(p => p.SKU == sku, cancellationToken);
    }

    public async Task UpdateProductStockAsync(Guid productId, int newStock, CancellationToken cancellationToken = default)
    {
        var productRepository = _unitOfWork.Repository<Product>();
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        if (product != null)
        {
            product.CurrentStock = newStock;
            product.LastModifiedAt = DateTime.UtcNow;
            productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    // Category operations
    public async Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var categoryRepository = _unitOfWork.Repository<Category>();
        return await categoryRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Category?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var categoryRepository = _unitOfWork.Repository<Category>();
        var categories = await categoryRepository.FindAsync(c => c.Name == name, cancellationToken);
        return categories.FirstOrDefault();
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categoryRepository = _unitOfWork.Repository<Category>();
        return await categoryRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithProductsAsync(CancellationToken cancellationToken = default)
    {
        var categoryRepository = _unitOfWork.Repository<Category>();
        return await categoryRepository.GetAllAsync(cancellationToken);
    }

    public async Task CreateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        var categoryRepository = _unitOfWork.Repository<Category>();
        await categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        var categoryRepository = _unitOfWork.Repository<Category>();
        categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        var categoryRepository = _unitOfWork.Repository<Category>();
        categoryRepository.Remove(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCategoryNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var categoryRepository = _unitOfWork.Repository<Category>();
        return await categoryRepository.ExistsAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<int> GetProductCountByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var productRepository = _unitOfWork.Repository<Product>();
        var products = await productRepository.FindAsync(p => p.CategoryId == categoryId, cancellationToken);
        return products.Count();
    }
}