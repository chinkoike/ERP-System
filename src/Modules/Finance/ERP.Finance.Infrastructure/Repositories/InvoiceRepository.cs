using ERP.Finance.Application.Repositories;
using ERP.Finance.Domain.Entities;
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
}
