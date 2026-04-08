using Moq;
using ERP.Purchasing.Application.DTOs;
using ERP.Purchasing.Application.Repositories;
using ERP.Purchasing.Application.Services;
using ERP.Purchasing.Domain.Entities;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Inventory.Application.DTOs;
using ERP.Shared;
using ERP.Shared.Exceptions;
using Xunit;

namespace ERP.Tests.Purchasing;

public class PurchasingServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IInventoryService> _inventoryServiceMock = new();
    private readonly Mock<ISupplierRepository> _supplierRepoMock = new();
    private readonly Mock<IPurchaseOrderRepository> _poRepoMock = new();
    private readonly PurchasingService _service;

    public PurchasingServiceTests()
    {
        _service = new PurchasingService(
            _uowMock.Object,
            _inventoryServiceMock.Object,
            _supplierRepoMock.Object,
            _poRepoMock.Object);
    }

    // ─── Supplier ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllSuppliersAsync_WhenSuppliersExist_ReturnsMappedDtos()
    {
        // Arrange
        var suppliers = new List<Supplier>
        {
            new() { Id = Guid.NewGuid(), Name = "Supplier A", Email = "a@supplier.com" },
            new() { Id = Guid.NewGuid(), Name = "Supplier B", Email = "b@supplier.com" },
        };
        _supplierRepoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(suppliers);

        // Act
        var result = await _service.GetAllSuppliersAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, s => s.Name == "Supplier A");
    }

    [Fact]
    public async Task GetSupplierByIdAsync_WhenFound_ReturnsDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var supplier = new Supplier { Id = id, Name = "Tech Parts Co.", Email = "info@tech.com" };
        _supplierRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(supplier);

        // Act
        var result = await _service.GetSupplierByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Tech Parts Co.", result.Name);
    }

    [Fact]
    public async Task GetSupplierByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _supplierRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                         .ReturnsAsync((Supplier?)null);

        // Act
        var result = await _service.GetSupplierByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateSupplierAsync_WithValidData_ReturnsNewId()
    {
        // Arrange
        var dto = new CreateSupplierDto
        {
            Name = "New Supplier",
            ContactName = "John",
            Email = "new@supplier.com",
            Phone = "0812345678"
        };

        // Act
        var result = await _service.CreateSupplierAsync(dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _supplierRepoMock.Verify(r => r.AddAsync(It.IsAny<Supplier>(), default), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteSupplierAsync_WhenSupplierHasOrders_ThrowsBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        _supplierRepoMock.Setup(r => r.GetByIdAsync(id, default))
                         .ReturnsAsync(new Supplier { Id = id, Name = "Busy Supplier" });
        _poRepoMock.Setup(r => r.SupplierHasOrdersAsync(id, default))
                   .ReturnsAsync(true); // มี PO อยู่

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.DeleteSupplierAsync(id));
    }

    [Fact]
    public async Task DeleteSupplierAsync_WhenNoOrders_DeletesAndReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var supplier = new Supplier { Id = id, Name = "Free Supplier" };
        _supplierRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(supplier);
        _poRepoMock.Setup(r => r.SupplierHasOrdersAsync(id, default)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteSupplierAsync(id);

        // Assert
        Assert.True(result);
        _supplierRepoMock.Verify(r => r.Remove(supplier), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    // ─── Purchase Order ────────────────────────────────────────────────────────

    [Fact]
    public async Task CreatePurchaseOrderAsync_WithValidData_ReturnsNewId()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        _supplierRepoMock.Setup(r => r.GetByIdAsync(supplierId, default))
                         .ReturnsAsync(new Supplier { Id = supplierId, Name = "Supplier X" });

        var dto = new CreatePurchaseOrderDto
        {
            SupplierId = supplierId,
            Items = new List<PurchaseOrderItemDto>
            {
                new() { ProductId = Guid.NewGuid(), QuantityOrdered = 10, UnitPrice = 50 }
            }
        };

        // Act
        var result = await _service.CreatePurchaseOrderAsync(dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _poRepoMock.Verify(r => r.AddAsync(It.IsAny<PurchaseOrder>(), default), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreatePurchaseOrderAsync_WhenSupplierNotFound_ThrowsNotFound()
    {
        // Arrange
        _supplierRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                         .ReturnsAsync((Supplier?)null);

        var dto = new CreatePurchaseOrderDto
        {
            SupplierId = Guid.NewGuid(),
            Items = new List<PurchaseOrderItemDto>
            {
                new() { ProductId = Guid.NewGuid(), QuantityOrdered = 1, UnitPrice = 100 }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreatePurchaseOrderAsync(dto));
    }

    [Fact]
    public async Task CreatePurchaseOrderAsync_WithNoItems_ThrowsBadRequest()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        _supplierRepoMock.Setup(r => r.GetByIdAsync(supplierId, default))
                         .ReturnsAsync(new Supplier { Id = supplierId, Name = "บริษัท ซัพพลายเออร์ จำกัด" });

        var dto = new CreatePurchaseOrderDto
        {
            SupplierId = supplierId,
            Items = new List<PurchaseOrderItemDto>() // ไม่มี item เลย
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreatePurchaseOrderAsync(dto));
    }

    [Fact]
    public async Task CancelPurchaseOrderAsync_WhenOrderIsCompleted_ThrowsBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        _poRepoMock.Setup(r => r.GetByIdAsync(id, default))
                   .ReturnsAsync(new PurchaseOrder { Id = id, PurchaseOrderNumber = "PO-001", Status = PurchaseOrderStatus.Completed });

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CancelPurchaseOrderAsync(id));
    }

    [Fact]
    public async Task CancelPurchaseOrderAsync_WhenOrderIsOrdered_CancelsSuccessfully()
    {
        // Arrange
        var id = Guid.NewGuid();
        var order = new PurchaseOrder { Id = id, PurchaseOrderNumber = "PO-001", Status = PurchaseOrderStatus.Ordered };
        _poRepoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(order);

        // Act
        var result = await _service.CancelPurchaseOrderAsync(id);

        // Assert
        Assert.True(result);
        Assert.Equal(PurchaseOrderStatus.Cancelled, order.Status);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CancelPurchaseOrderAsync_WhenAlreadyCancelled_ReturnsTrueWithoutSaving()
    {
        // Arrange
        var id = Guid.NewGuid();
        _poRepoMock.Setup(r => r.GetByIdAsync(id, default))
                   .ReturnsAsync(new PurchaseOrder { Id = id, PurchaseOrderNumber = "PO-001", Status = PurchaseOrderStatus.Cancelled });

        // Act
        var result = await _service.CancelPurchaseOrderAsync(id);

        // Assert
        Assert.True(result);
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never); // ไม่ต้อง save ซ้ำ
    }

    [Fact]
    public async Task ReceivePurchaseOrderAsync_WhenOrderIsCancelled_ThrowsBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        _poRepoMock.Setup(r => r.GetByIdAsync(id, default))
                   .ReturnsAsync(new PurchaseOrder { Id = id, PurchaseOrderNumber = "PO-001", Status = PurchaseOrderStatus.Cancelled });

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.ReceivePurchaseOrderAsync(id, new List<PurchaseOrderItemDto>()));
    }
}
