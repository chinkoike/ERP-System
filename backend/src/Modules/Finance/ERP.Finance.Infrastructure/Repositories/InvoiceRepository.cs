using ERP.Finance.Application.DTOs;
using ERP.Finance.Application.Repositories;
using ERP.Finance.Domain.Entities;
using ERP.Shared;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Finance.Infrastructure.Repositories;

public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new ArgumentException("invoiceNumber cannot be null or empty", nameof(invoiceNumber));

        var results = await FindAsync(i => i.InvoiceNumber == invoiceNumber, cancellationToken);
        return results.FirstOrDefault();
    }

    public async Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default)
    {
        return await FindAsync(i => i.DueDate < DateTime.UtcNow && i.Status != InvoiceStatus.Paid, cancellationToken);
    }

    public async Task<PagedResult<Invoice>> SearchInvoicesAsync(InvoiceFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = DbContext.Set<Invoice>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim().ToLower();
            query = query.Where(i =>
                i.InvoiceNumber.ToLower().Contains(term) ||
                (i.Description != null && i.Description.ToLower().Contains(term)));
        }

        if (filter.CustomerId.HasValue)
        {
            query = query.Where(i => i.CustomerId == filter.CustomerId.Value);
        }

        if (filter.SupplierId.HasValue)
        {
            query = query.Where(i => i.SupplierId == filter.SupplierId.Value);
        }

        if (filter.AccountId.HasValue)
        {
            query = query.Where(i => i.AccountId == filter.AccountId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(i => i.Status == filter.Status.Value);
        }

        if (filter.StartDate.HasValue)
        {
            query = query.Where(i => i.InvoiceDate >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            query = query.Where(i => i.InvoiceDate <= filter.EndDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(filter.PageNumber, 1);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var items = await query
            .OrderByDescending(i => i.InvoiceDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Invoice>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
