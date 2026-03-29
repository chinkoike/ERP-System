using ERP.Shared;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Inventory.Domain;
using ERP.Inventory.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ERP.Inventory.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // --- Product Operations ---

    public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id, cancellationToken);
        return product == null ? null : MapToProductDto(product);
    }

    public async Task<ProductDto?> GetProductBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Repository<Product>().FindAsync(p => p.SKU == sku, cancellationToken);
        var product = products.FirstOrDefault();
        return product == null ? null : MapToProductDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Repository<Product>().GetAllAsync(cancellationToken);
        return products.Select(MapToProductDto);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Repository<Product>().FindAsync(p => p.CategoryId == categoryId, cancellationToken);
        return products.Select(MapToProductDto);
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Repository<Product>().FindAsync(p => p.CurrentStock <= threshold, cancellationToken);
        return products.Select(MapToProductDto);
    }

    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<Product>().ExistsAsync(p => p.SKU == sku, cancellationToken);
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
            CurrentStock = dto.InitialStock,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await _unitOfWork.Repository<Product>().AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToProductDto(product);
    }

    public async Task UpdateProductAsync(ProductDto dto, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<Product>();
        var product = await repo.GetByIdAsync(dto.Id, cancellationToken);

        if (product != null)
        {
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.BasePrice = dto.BasePrice;
            product.CategoryId = dto.CategoryId;
            product.LastModifiedAt = DateTime.UtcNow;
            product.LastModifiedBy = dto.UpdatedBy;

            repo.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> UpdateProductStockAsync(UpdateProductStockDto dto, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<Product>();
        var product = await repo.GetByIdAsync(dto.ProductId, cancellationToken);

        if (product == null) return false;

        int updatedStock = product.CurrentStock + dto.QuantityChange;

        if (updatedStock < 0)
        {
            throw new InvalidOperationException("Stock quantity cannot be less than zero.");
        }

        product.CurrentStock = updatedStock;
        product.LastModifiedAt = DateTime.UtcNow;

        repo.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<Product>();
        var product = await repo.GetByIdAsync(id, cancellationToken);
        if (product != null)
        {
            repo.Remove(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    // --- Category Operations ---

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id, cancellationToken);
        return category == null ? null : MapToCategoryDto(category);
    }

    public async Task<CategoryDto?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Repository<Category>().FindAsync(c => c.Name == name, cancellationToken);
        var category = categories.FirstOrDefault();
        return category == null ? null : MapToCategoryDto(category);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync(cancellationToken);
        return categories.Select(MapToCategoryDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesWithProductsAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync(cancellationToken);
        return categories.Select(MapToCategoryDto);
    }

    public async Task<bool> ExistsByCategoryNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<Category>().ExistsAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<int> GetProductCountByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Repository<Product>().FindAsync(p => p.CategoryId == categoryId, cancellationToken);
        return products.Count();
    }

    public async Task<Guid> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken ct)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Repository<Category>().AddAsync(category, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return category.Id;
    }

    public async Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Category>();
        var category = await repo.GetByIdAsync(id, ct);

        if (category == null) return false;

        category.Name = dto.Name ?? category.Name;
        category.Description = dto.Description ?? category.Description;
        category.LastModifiedAt = DateTime.UtcNow;

        repo.Update(category);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Category>();
        var category = await repo.GetByIdAsync(id, ct);

        if (category == null) return false;

        repo.Remove(category);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    // --- Helper Mapping Methods ---

    private static ProductDto MapToProductDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        SKU = p.SKU,
        ImageUrl = p.ImageUrl,
        Description = p.Description,
        BasePrice = p.BasePrice,
        CurrentStock = p.CurrentStock,
        CategoryId = p.CategoryId,
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