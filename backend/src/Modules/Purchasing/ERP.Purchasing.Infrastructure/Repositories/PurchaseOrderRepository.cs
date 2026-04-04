using ERP.Purchasing.Application.Repositories;
using ERP.Purchasing.Domain.Entities;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Purchasing.Infrastructure.Repositories;

public class PurchaseOrderRepository : GenericRepository<PurchaseOrder>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<PurchaseOrder>> GetBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(po => po.SupplierId == supplierId, cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrderItem>> GetItemsByPurchaseOrderIdAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<PurchaseOrderItem>().Where(i => i.PurchaseOrderId == purchaseOrderId).ToListAsync(cancellationToken);
    }

    public async Task<bool> SupplierHasOrdersAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(po => po.SupplierId == supplierId, cancellationToken);
    }
}
