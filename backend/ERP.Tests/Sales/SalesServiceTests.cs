using Moq;
using ERP.Sales.Application.DTOs;
using ERP.Sales.Application.Repositories;
using ERP.Sales.Application.Services;
using ERP.Sales.Domain;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Inventory.Application.DTOs;
using ERP.Identity.Application.Services.Interfaces;
using ERP.Shared;
using Xunit;

namespace ERP.Tests.Sales;

public class SalesServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<IInventoryService> _inventoryServiceMock = new();
    private readonly Mock<ICustomerRepository> _customerRepoMock = new();
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly Mock<IOrderItemRepository> _orderItemRepoMock = new();
    private readonly SalesService _service;

    public SalesServiceTests()
    {
        _service = new SalesService(
            _uowMock.Object,
            _identityServiceMock.Object,
            _inventoryServiceMock.Object,
            _customerRepoMock.Object,
            _orderRepoMock.Object,
            _orderItemRepoMock.Object);
    }

    // ─── Customer ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllCustomersAsync_WhenCustomersExist_ReturnsMappedDtos()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john@example.com" },
            new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" },
        };
        _customerRepoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(customers);

        // Act
        var result = await _service.GetAllCustomersAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Email == "john@example.com");
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WhenCustomerExists_ReturnsDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var customer = new Customer
        {
            Id = id,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        _customerRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(customer);

        // Act
        var result = await _service.GetCustomerByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("john@example.com", result.Email);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _customerRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                         .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.GetCustomerByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCustomerAsync_WithUniqueEmail_ReturnsNewId()
    {
        // Arrange
        _customerRepoMock.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>(), default))
                         .ReturnsAsync(false); // email ยังไม่ถูกใช้

        var dto = new CreateCustomerDto
        {
            FirstName = "Alice",
            LastName = "Wonder",
            Email = "alice@example.com",
            Phone = "0812345678"
        };

        // Act
        var result = await _service.CreateCustomerAsync(dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _customerRepoMock.Verify(r => r.AddAsync(It.IsAny<Customer>(), default), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateCustomerAsync_WithDuplicateEmail_ThrowsException()
    {
        // Arrange
        _customerRepoMock.Setup(r => r.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>(), default))
                         .ReturnsAsync(true); // email ซ้ำแล้ว

        var dto = new CreateCustomerDto { Email = "duplicate@example.com" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateCustomerAsync(dto));
    }

    [Fact]
    public async Task DeleteCustomerAsync_WhenCustomerExists_RemovesAndReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var customer = new Customer { Id = id, FirstName = "To Delete", LastName = "Customer", Email = "todelete@example.com" };
        _customerRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(customer);

        // Act
        var result = await _service.DeleteCustomerAsync(id);

        // Assert
        Assert.True(result);
        _customerRepoMock.Verify(r => r.Remove(customer), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerAsync_WhenNotFound_ReturnsFalse()
    {
        // Arrange
        _customerRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                         .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.DeleteCustomerAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    // ─── Order ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task PlaceOrderAsync_WithValidItems_CreatesOrderAndDeductsStock()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        _inventoryServiceMock.Setup(s => s.GetProductByIdAsync(productId, default))
            .ReturnsAsync(new ProductDto { Id = productId, Name = "Widget", Price = 100, BasePrice = 100 });
        _inventoryServiceMock.Setup(s => s.UpdateProductStockAsync(productId, It.IsAny<UpdateProductStockDto>(), default))
            .ReturnsAsync(true);

        var dto = new CreateOrderDto
        {
            CustomerId = customerId,
            ShippingAddress = "123 Main St",
            Items = new List<OrderItemDto>
            {
                new() { ProductId = productId, Quantity = 2 }
            }
        };

        // Act
        var result = await _service.PlaceOrderAsync(dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _orderRepoMock.Verify(r => r.AddAsync(It.IsAny<Order>(), default), Times.Once);
        _inventoryServiceMock.Verify(
            s => s.UpdateProductStockAsync(productId, It.Is<UpdateProductStockDto>(d => d.QuantityChange == -2), default),
            Times.Once);
    }

    [Fact]
    public async Task PlaceOrderAsync_WhenProductNotFound_ThrowsException()
    {
        // Arrange
        _inventoryServiceMock.Setup(s => s.GetProductByIdAsync(It.IsAny<Guid>(), default))
                             .ReturnsAsync((ProductDto?)null);

        var dto = new CreateOrderDto
        {
            CustomerId = Guid.NewGuid(),
            Items = new List<OrderItemDto>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 1 }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.PlaceOrderAsync(dto));
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenOrderExists_UpdatesAndReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var order = new Order { Id = id, OrderNumber = "ORD-001", Status = OrderStatus.Pending };
        _orderRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(order);

        // Act
        var result = await _service.UpdateOrderStatusAsync(id, OrderStatus.Processing);

        // Assert
        Assert.True(result);
        Assert.Equal(OrderStatus.Processing, order.Status);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenOrderNotFound_ReturnsFalse()
    {
        // Arrange
        _orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                      .ReturnsAsync((Order?)null);

        // Act
        var result = await _service.UpdateOrderStatusAsync(Guid.NewGuid(), OrderStatus.Processing);

        // Assert
        Assert.False(result);
    }
}
