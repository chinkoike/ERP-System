using ERP.Inventory.Application.DTOs;

namespace ERP.Inventory.Application.Services.Interfaces;

public interface IInventoryService
{
    // --- Product Read Operations ---
    Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);

    // ใช้ CreateProductDto เพื่อรับข้อมูลที่จำเป็นตอนสร้างเท่านั้น
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, CancellationToken cancellationToken = default);

    // ใช้ ProductDto สำหรับการอัปเดตข้อมูลทั่วไป (ชื่อ, ราคา, รายละเอียด)
    Task<bool> UpdateProductAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default);

    // ใช้ UpdateProductStockDto สำหรับการจัดการสต็อกโดยเฉพาะ (Stock In/Out)
    Task<bool> UpdateProductStockAsync(Guid id, UpdateProductStockDto updateStockDto, CancellationToken cancellationToken = default);

    Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);

    // --- Category Read Operations ---
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryDto>> GetCategoriesWithProductsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByCategoryNameAsync(string name, CancellationToken cancellationToken = default);
    Task<int> GetProductCountByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    // --- Category Command Operations ---
    Task<Guid> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken ct = default);
    Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default);
    Task<bool> DeleteCategoryAsync(Guid id, CancellationToken ct = default);
}