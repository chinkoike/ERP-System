using ERP.Sales.Domain;
using ERP.Shared;

namespace ERP.Sales.Application.Repositories;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetCustomersWithOrdersAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}