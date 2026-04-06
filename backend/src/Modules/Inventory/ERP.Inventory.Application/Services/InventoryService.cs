using ERP.Shared;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Inventory.Application.Repositories;
using ERP.Inventory.Domain;
using ERP.Inventory.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ERP.Inventory.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public InventoryService(
        IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    // --- Product Operations ---

    public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null) return null;
        var categoryName = (await _categoryRepository.GetByIdAsync(product.CategoryId, cancellationToken))?.Name;
        return MapToProductDto(product, categoryName);
    }

    public async Task<ProductDto?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetBySkuAsync(sku, cancellationToken);
        if (product == null) return null;
        var categoryName = (await _categoryRepository.GetByIdAsync(product.CategoryId, cancellationToken))?.Name;
        return MapToProductDto(product, categoryName);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        var categoryMap = (await _categoryRepository.GetAllAsync(cancellationToken)).ToDictionary(c => c.Id, c => c.Name);
        return products.Select(p => MapToProductDto(p, categoryMap.TryGetValue(p.CategoryId, out var name) ? name : null));
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId, cancellationToken);
        var categoryName = (await _categoryRepository.GetByIdAsync(categoryId, cancellationToken))?.Name;
        return products.Select(p => MapToProductDto(p, categoryName));
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetLowStockProductsAsync(threshold, cancellationToken);
        var categoryMap = (await _categoryRepository.GetAllAsync(cancellationToken)).ToDictionary(c => c.Id, c => c.Name);
        return products.Select(p => MapToProductDto(p, categoryMap.TryGetValue(p.CategoryId, out var name) ? name : null));
    }

    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _productRepository.ExistsBySkuAsync(sku, cancellationToken);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Name = dto.Name,
            SKU = dto.SKU,
            ImageUrl = dto.ImageUrl,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            Price = dto.BasePrice,
            CurrentStock = dto.InitialStock,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToProductDto(product);
    }

    public async Task<bool> UpdateProductAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _productRepository.GetByIdAsync(id, ct);

        if (product == null) return false;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.BasePrice = dto.BasePrice;
        product.CategoryId = dto.CategoryId;
        product.LastModifiedAt = DateTime.UtcNow;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }
    public async Task<bool> UpdateProductStockAsync(Guid id, UpdateProductStockDto dto, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(id, ct);
        if (product == null) return false;

        product.CurrentStock += dto.QuantityChange;
        product.LastModifiedAt = DateTime.UtcNow;
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteProductAsync(Guid id, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(id, ct);
        if (product == null) return false;

        _productRepository.Remove(product);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }
    // --- Category Operations ---

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        return category == null ? null : MapToCategoryDto(category);
    }

    public async Task<CategoryDto?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByNameAsync(name, cancellationToken);
        return category == null ? null : MapToCategoryDto(category);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(MapToCategoryDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesWithProductsAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetCategoriesWithProductsAsync(cancellationToken);
        return categories.Select(MapToCategoryDto);
    }

    public async Task<bool> ExistsByCategoryNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.ExistsByNameAsync(name, cancellationToken);
    }

    public async Task<int> GetProductCountByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.GetProductCountAsync(categoryId, cancellationToken);
    }

    public async Task<Guid> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken ct)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };
        await _categoryRepository.AddAsync(category, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return category.Id;
    }

    public async Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, ct);

        if (category == null) return false;

        category.Name = dto.Name ?? category.Name;
        category.Description = dto.Description ?? category.Description;
        category.LastModifiedAt = DateTime.UtcNow;

        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, ct);

        if (category == null) return false;

        _categoryRepository.Remove(category);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    // --- Helper Mapping Methods ---

    private static ProductDto MapToProductDto(Product p, string? categoryName = null) => new()
    {
        Id = p.Id,
        Name = p.Name,
        SKU = p.SKU,
        Price = p.Price,
        ImageUrl = p.ImageUrl,
        Description = p.Description,
        BasePrice = p.BasePrice,
        CurrentStock = p.CurrentStock,
        CategoryId = p.CategoryId,
        CategoryName = categoryName ?? p.Category?.Name ?? string.Empty,
        CreatedAt = p.CreatedAt,
        CreatedBy = p.CreatedBy ?? "System",
        UpdatedAt = p.LastModifiedAt,
        UpdatedBy = p.LastModifiedBy
    };

    private static CategoryDto MapToCategoryDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Description = c.Description,
        CreatedAt = c.CreatedAt,
        CreatedBy = c.CreatedBy ?? "System",
        UpdatedAt = c.LastModifiedAt,
        UpdatedBy = c.LastModifiedBy
    };
}