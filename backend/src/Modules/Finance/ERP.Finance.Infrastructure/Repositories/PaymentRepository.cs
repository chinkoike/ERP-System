using ERP.Finance.Application.Repositories;
using ERP.Finance.Domain.Entities;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Finance.Infrastructure.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Payment>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(p => p.InvoiceId == invoiceId, cancellationToken);
    }
}
