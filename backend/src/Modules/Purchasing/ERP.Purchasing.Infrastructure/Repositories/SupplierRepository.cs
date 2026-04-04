using ERP.Purchasing.Application.Repositories;
using ERP.Purchasing.Domain.Entities;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Purchasing.Infrastructure.Repositories;

public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    public SupplierRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Supplier?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name cannot be null or empty", nameof(name));

        var suppliers = await FindAsync(s => s.Name == name, cancellationToken);
        return suppliers.FirstOrDefault();
    }

    public async Task<bool> HasPurchaseOrdersAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(s => s.Id == supplierId && s.PurchaseOrders.Any(), cancellationToken);
    }
}
