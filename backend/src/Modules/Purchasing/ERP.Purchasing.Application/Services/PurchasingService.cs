using ERP.Shared;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Purchasing.Application.Services.Interfaces;
using ERP.Purchasing.Application.Repositories;
using ERP.Purchasing.Application.DTOs;
using ERP.Purchasing.Domain.Entities;
using ERP.Shared.Exceptions;

namespace ERP.Purchasing.Application.Services;

public class PurchasingService : IPurchasingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInventoryService _inventoryService;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;

    public PurchasingService(
        IUnitOfWork unitOfWork,
        IInventoryService inventoryService,
        ISupplierRepository supplierRepository,
        IPurchaseOrderRepository purchaseOrderRepository)
    {
        _unitOfWork = unitOfWork;
        _inventoryService = inventoryService;
        _supplierRepository = supplierRepository;
        _purchaseOrderRepository = purchaseOrderRepository;
    }

    // Supplier
    public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync(CancellationToken cancellationToken = default)
    {
        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);
        return suppliers.Select(MapToSupplierDto);
    }

    public async Task<SupplierDto?> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);
        return supplier == null ? null : MapToSupplierDto(supplier);
    }

    public async Task<Guid> CreateSupplierAsync(CreateSupplierDto dto, CancellationToken cancellationToken = default)
    {
        var supplier = new Supplier
        {
            Name = dto.Name,
            ContactName = dto.ContactName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await _supplierRepository.AddAsync(supplier, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }

    public async Task<bool> UpdateSupplierAsync(Guid id, UpdateSupplierDto dto, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);
        if (supplier == null) return false;

        supplier.Name = dto.Name ?? supplier.Name;
        supplier.ContactName = dto.ContactName ?? supplier.ContactName;
        supplier.Email = dto.Email ?? supplier.Email;
        supplier.Phone = dto.Phone ?? supplier.Phone;
        supplier.Address = dto.Address ?? supplier.Address;
        supplier.LastModifiedAt = DateTime.UtcNow;

        _supplierRepository.Update(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id, cancellationToken);
        if (supplier == null) return false;

        var hasOrders = await _purchaseOrderRepository.SupplierHasOrdersAsync(id, cancellationToken);
        if (hasOrders)
        {
            throw new BadRequestException("ไม่สามารถลบ supplier ที่มี PO อยู่แล้ว");
        }

        _supplierRepository.Remove(supplier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Purchase Order
    public async Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _purchaseOrderRepository.GetAllWithItemsAndSupplierAsync(cancellationToken);
        return orders.Select(o => MapToPurchaseOrderDto(o, o.Supplier?.Name ?? string.Empty));
    }

    public async Task<PagedResult<PurchaseOrderDto>> SearchPurchaseOrdersAsync(PurchaseOrderFilterDto filter, CancellationToken cancellationToken = default)
    {
        var result = await _purchaseOrderRepository.SearchPurchaseOrdersAsync(filter, cancellationToken);
        return new PagedResult<PurchaseOrderDto>
        {
            Items = result.Items.Select(o => MapToPurchaseOrderDto(o, o.Supplier?.Name ?? string.Empty)).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<PurchaseOrderDto?> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _purchaseOrderRepository.GetByIdWithItemsAndSupplierAsync(id, cancellationToken);
        if (order == null) return null;

        return MapToPurchaseOrderDto(order, order.Supplier?.Name ?? string.Empty);
    }

    public async Task<Guid> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId, cancellationToken);
        if (supplier == null)
            throw new NotFoundException("Supplier not found.");

        if (dto.Items == null || !dto.Items.Any())
            throw new BadRequestException("Purchase order must contain at least one item.");

        var order = new PurchaseOrder
        {
            PurchaseOrderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            SupplierId = dto.SupplierId,
            OrderDate = DateTime.UtcNow,
            Status = PurchaseOrderStatus.Ordered,
            Items = new List<PurchaseOrderItem>(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        decimal totalAmount = 0;

        foreach (var itemDto in dto.Items)
        {
            if (itemDto.QuantityOrdered <= 0)
            {
                throw new BadRequestException("จำนวนสินค้าใน PO ต้องมากกว่า 0");
            }

            var lineTotal = itemDto.UnitPrice * itemDto.QuantityOrdered;
            totalAmount += lineTotal;

            order.Items.Add(new PurchaseOrderItem
            {
                ProductId = itemDto.ProductId,
                QuantityOrdered = itemDto.QuantityOrdered,
                QuantityReceived = 0,
                UnitPrice = itemDto.UnitPrice,
                TotalPrice = lineTotal
            });
        }

        order.TotalAmount = totalAmount;

        await _purchaseOrderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }

    public async Task<bool> ReceivePurchaseOrderAsync(Guid purchaseOrderId, List<PurchaseOrderItemDto> receiveItems, CancellationToken cancellationToken = default)
    {
        var order = await _purchaseOrderRepository.GetByIdAsync(purchaseOrderId, cancellationToken);
        if (order == null)
            throw new NotFoundException("Purchase order not found.");

        if (order.Status == PurchaseOrderStatus.Cancelled || order.Status == PurchaseOrderStatus.Completed)
            throw new BadRequestException("Purchase order ไม่สามารถรับสินค้าได้ในสถานะนี้");

        var existingItems = (await _purchaseOrderRepository.GetItemsByPurchaseOrderIdAsync(purchaseOrderId, cancellationToken)).ToList();
        if (!existingItems.Any())
            throw new BadRequestException("ไม่มีไอเท็มใน Purchase order นี้");

        foreach (var receive in receiveItems)
        {
            var line = existingItems.FirstOrDefault(x => x.ProductId == receive.ProductId);
            if (line == null)
                throw new NotFoundException($"Product {receive.ProductId} not found in PO");

            if (receive.QuantityReceived <= 0)
                throw new BadRequestException("จำนวนรับต้องมากกว่า 0");

            if (line.QuantityReceived + receive.QuantityReceived > line.QuantityOrdered)
                throw new BadRequestException($"จำนวนรับเกินจำนวนที่สั่งสำหรับ Product {receive.ProductId}");

            line.QuantityReceived += receive.QuantityReceived;
            line.TotalPrice = line.UnitPrice * line.QuantityOrdered;

            // เพิ่มสต็อกสินค้าใน Inventory
            await _inventoryService.UpdateProductStockAsync(line.ProductId, new ERP.Inventory.Application.DTOs.UpdateProductStockDto
            {
                QuantityChange = receive.QuantityReceived
            }, cancellationToken);
        }

        var allReceived = existingItems.All(i => i.QuantityReceived >= i.QuantityOrdered);
        order.Status = allReceived ? PurchaseOrderStatus.Completed : PurchaseOrderStatus.Receiving;
        order.LastModifiedAt = DateTime.UtcNow;

        _purchaseOrderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
    public async Task<bool> CancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _purchaseOrderRepository.GetByIdAsync(id, cancellationToken);

        if (order == null)
            throw new NotFoundException("ไม่พบใบสั่งซื้อที่ระบุ");

        if (order.Status == PurchaseOrderStatus.Completed || order.Status == PurchaseOrderStatus.Receiving)
        {
            throw new BadRequestException("ไม่สามารถยกเลิกใบสั่งซื้อได้ เนื่องจากมีการรับสินค้าไปบางส่วนหรือทั้งหมดแล้ว");
        }

        if (order.Status == PurchaseOrderStatus.Cancelled)
        {
            return true;
        }

        order.Status = PurchaseOrderStatus.Cancelled;
        order.LastModifiedAt = DateTime.UtcNow;
        order.LastModifiedBy = "System";

        _purchaseOrderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
    private static SupplierDto MapToSupplierDto(Supplier entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        ContactName = entity.ContactName,
        Email = entity.Email,
        Phone = entity.Phone,
        Address = entity.Address,
        CreatedAt = entity.CreatedAt,
        CreatedBy = entity.CreatedBy,
        UpdatedAt = entity.LastModifiedAt,
        UpdatedBy = entity.LastModifiedBy
    };

    private static PurchaseOrderDto MapToPurchaseOrderDto(PurchaseOrder entity, string? supplierName = null)
    {
        var items = entity.Items?.Select(i => new PurchaseOrderItemDto
        {
            ProductId = i.ProductId,
            QuantityOrdered = i.QuantityOrdered,
            QuantityReceived = i.QuantityReceived,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.TotalPrice
        }).ToList() ?? new List<PurchaseOrderItemDto>();

        return new PurchaseOrderDto
        {
            Id = entity.Id,
            PurchaseOrderNumber = entity.PurchaseOrderNumber,
            SupplierId = entity.SupplierId,
            SupplierName = supplierName ?? string.Empty,
            OrderDate = entity.OrderDate,
            Status = entity.Status.ToString(),
            TotalAmount = entity.TotalAmount,
            Items = items
        };
    }
}
