using ERP.Purchasing.Application.DTOs;
using ERP.Shared;

namespace ERP.Purchasing.Application.Services.Interfaces;

public interface IPurchasingService
{
    // Supplier
    Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync(CancellationToken cancellationToken = default);
    Task<SupplierDto?> GetSupplierByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateSupplierAsync(CreateSupplierDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateSupplierAsync(Guid id, UpdateSupplierDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteSupplierAsync(Guid id, CancellationToken cancellationToken = default);

    // Purchase Order
    Task<IEnumerable<PurchaseOrderDto>> GetAllPurchaseOrdersAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<PurchaseOrderDto>> SearchPurchaseOrdersAsync(PurchaseOrderFilterDto filter, CancellationToken cancellationToken = default);
    Task<PurchaseOrderDto?> GetPurchaseOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto, CancellationToken cancellationToken = default);
    Task<bool> ReceivePurchaseOrderAsync(Guid purchaseOrderId, List<PurchaseOrderItemDto> receiveItems, CancellationToken cancellationToken = default);
    Task<bool> CancelPurchaseOrderAsync(Guid id, CancellationToken cancellationToken = default);
}