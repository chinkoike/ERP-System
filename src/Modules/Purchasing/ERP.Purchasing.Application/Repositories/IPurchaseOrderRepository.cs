using ERP.Purchasing.Domain.Entities;
using ERP.Shared;

namespace ERP.Purchasing.Application.Repositories;

public interface IPurchaseOrderRepository : IGenericRepository<PurchaseOrder>
{
    Task<IEnumerable<PurchaseOrder>> GetBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PurchaseOrderItem>> GetItemsByPurchaseOrderIdAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<bool> SupplierHasOrdersAsync(Guid supplierId, CancellationToken cancellationToken = default);
}
