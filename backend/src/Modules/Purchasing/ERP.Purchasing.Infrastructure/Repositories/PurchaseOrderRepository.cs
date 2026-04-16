using ERP.Purchasing.Application.DTOs;
using ERP.Purchasing.Application.Repositories;
using ERP.Purchasing.Domain.Entities;
using ERP.Shared;
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

    public async Task<IEnumerable<PurchaseOrder>> GetAllWithItemsAndSupplierAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<PurchaseOrder>()
            .Include(po => po.Items)
            .Include(po => po.Supplier)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<PurchaseOrder?> GetByIdWithItemsAndSupplierAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<PurchaseOrder>()
            .Include(po => po.Items)
            .Include(po => po.Supplier)
            .FirstOrDefaultAsync(po => po.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<PurchaseOrderItem>> GetItemsByPurchaseOrderIdAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<PurchaseOrderItem>().Where(i => i.PurchaseOrderId == purchaseOrderId).ToListAsync(cancellationToken);
    }

    public async Task<bool> SupplierHasOrdersAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(po => po.SupplierId == supplierId, cancellationToken);
    }

    public async Task<PagedResult<PurchaseOrder>> SearchPurchaseOrdersAsync(PurchaseOrderFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = DbContext.Set<PurchaseOrder>()
            .Include(po => po.Supplier)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(po =>
                po.PurchaseOrderNumber.Contains(term) ||
                (po.Supplier != null && po.Supplier.Name.Contains(term)));
        }

        if (filter.SupplierId.HasValue)
        {
            query = query.Where(po => po.SupplierId == filter.SupplierId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(po => po.Status == filter.Status.Value);
        }

        if (filter.StartDate.HasValue)
        {
            query = query.Where(po => po.OrderDate >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            query = query.Where(po => po.OrderDate <= filter.EndDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(filter.PageNumber, 1);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var items = await query
            .OrderByDescending(po => po.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<PurchaseOrder>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
