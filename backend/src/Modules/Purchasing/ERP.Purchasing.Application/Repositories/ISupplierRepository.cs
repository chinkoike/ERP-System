using ERP.Purchasing.Domain.Entities;
using ERP.Shared;

namespace ERP.Purchasing.Application.Repositories;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    Task<Supplier?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> HasPurchaseOrdersAsync(Guid supplierId, CancellationToken cancellationToken = default);
}
