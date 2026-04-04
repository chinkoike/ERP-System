using ERP.Finance.Domain.Entities;
using ERP.Shared;

namespace ERP.Finance.Application.Repositories;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}
