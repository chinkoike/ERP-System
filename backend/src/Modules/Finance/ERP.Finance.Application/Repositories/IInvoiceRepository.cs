using ERP.Finance.Application.DTOs;
using ERP.Finance.Domain.Entities;
using ERP.Shared;

namespace ERP.Finance.Application.Repositories;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<Invoice>> SearchInvoicesAsync(InvoiceFilterDto filter, CancellationToken cancellationToken = default);
}
