using ERP.Inventory.Application.DTOs; // ต้องเพิ่ม using DTOs เข้ามา

namespace ERP.Inventory.Application.Services.Interfaces;

public interface IInventoryService
{
    // Product operations (เปลี่ยนจาก Product เป็น ProductDto)
    Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);

    Task CreateProductAsync(ProductDto productDto, CancellationToken cancellationToken = default);
    Task UpdateProductAsync(ProductDto productDto, CancellationToken cancellationToken = default);

    Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task UpdateProductStockAsync(Guid productId, int newStock, CancellationToken cancellationToken = default);

    // Category operations (เปลี่ยนจาก Category เป็น CategoryDto)
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryDto>> GetCategoriesWithProductsAsync(CancellationToken cancellationToken = default);

    Task CreateCategoryAsync(CategoryDto categoryDto, CancellationToken cancellationToken = default);
    Task UpdateCategoryAsync(CategoryDto categoryDto, CancellationToken cancellationToken = default);

    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCategoryNameAsync(string name, CancellationToken cancellationToken = default);
    Task<int> GetProductCountByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
}