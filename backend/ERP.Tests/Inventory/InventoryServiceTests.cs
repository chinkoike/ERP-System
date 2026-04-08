using Moq;
using ERP.Inventory.Application.DTOs;
using ERP.Inventory.Application.Repositories;
using ERP.Inventory.Application.Services;
using ERP.Inventory.Domain;
using ERP.Shared;
using Xunit;
namespace ERP.Tests.Inventory;

public class InventoryServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IProductRepository> _productRepoMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        _service = new InventoryService(
            _uowMock.Object,
            _productRepoMock.Object,
            _categoryRepoMock.Object);
    }

    // ─── Product ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllProductsAsync_WhenProductsExist_ReturnsMappedDtos()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Widget A", SKU = "SKU-001",
                    BasePrice = 100, Price = 100, CurrentStock = 50, CategoryId = categoryId },
            new() { Id = Guid.NewGuid(), Name = "Widget B", SKU = "SKU-002",
                    BasePrice = 200, Price = 200, CurrentStock = 30, CategoryId = categoryId },
        };
        var categories = new List<Category>
        {
            new() { Id = categoryId, Name = "Electronics" }
        };

        _productRepoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(products);
        _categoryRepoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal("Electronics", p.CategoryName));
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenProductExists_ReturnsDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = new Product
        {
            Id = id,
            Name = "Widget A",
            SKU = "SKU-001",
            BasePrice = 100,
            Price = 100,
            CurrentStock = 10,
            CategoryId = categoryId
        };
        var category = new Category { Id = categoryId, Name = "Tools" };

        _productRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(product);
        _categoryRepoMock.Setup(r => r.GetByIdAsync(categoryId, default)).ReturnsAsync(category);

        // Act
        var result = await _service.GetProductByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Widget A", result.Name);
        Assert.Equal("Tools", result.CategoryName);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                        .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetProductByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidData_ReturnsDto()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "New Product",
            SKU = "SKU-NEW",
            BasePrice = 350,
            InitialStock = 100,
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = await _service.CreateProductAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        Assert.Equal(100, result.CurrentStock);
        _productRepoMock.Verify(r => r.AddAsync(It.IsAny<Product>(), default), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateProductStockAsync_WhenProductExists_AddsQuantityAndSaves()
    {
        // Arrange
        var id = Guid.NewGuid();
        var product = new Product { Id = id, SKU = "PROD-001", Name = "Test Product", CurrentStock = 50 };
        _productRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(product);

        var dto = new UpdateProductStockDto { QuantityChange = 25 };

        // Act
        var result = await _service.UpdateProductStockAsync(id, dto, default);

        // Assert
        Assert.True(result);
        Assert.Equal(75, product.CurrentStock);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateProductStockAsync_WhenProductNotFound_ReturnsFalse()
    {
        // Arrange
        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                        .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.UpdateProductStockAsync(Guid.NewGuid(), new UpdateProductStockDto(), default);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenProductExists_RemovesAndSaves()
    {
        // Arrange
        var id = Guid.NewGuid();
        var product = new Product { Id = id, SKU = "PROD-001", Name = "To Delete" };
        _productRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(product);

        // Act
        var result = await _service.DeleteProductAsync(id, default);

        // Assert
        Assert.True(result);
        _productRepoMock.Verify(r => r.Remove(product), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenNotFound_ReturnsFalse()
    {
        // Arrange
        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                        .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.DeleteProductAsync(Guid.NewGuid(), default);

        // Assert
        Assert.False(result);
    }

    // ─── Category ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllMappedDtos()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Electronics", Description = "Electronic items" },
            new() { Id = Guid.NewGuid(), Name = "Tools", Description = "Hand tools" },
        };
        _categoryRepoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllCategoriesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Name == "Electronics");
    }

    [Fact]
    public async Task CreateCategoryAsync_WithValidData_ReturnsNewId()
    {
        // Arrange
        var dto = new CreateCategoryDto { Name = "New Category", Description = "Desc" };

        // Act
        var result = await _service.CreateCategoryAsync(dto, default);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _categoryRepoMock.Verify(r => r.AddAsync(It.IsAny<Category>(), default), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WhenNotFound_ReturnsFalse()
    {
        // Arrange
        _categoryRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                         .ReturnsAsync((Category?)null);

        // Act
        var result = await _service.DeleteCategoryAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }
}
