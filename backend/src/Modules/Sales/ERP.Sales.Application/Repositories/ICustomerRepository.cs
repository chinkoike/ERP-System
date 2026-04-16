using ERP.Sales.Domain;
using ERP.Shared;
using ERP.Sales.Application.DTOs;
namespace ERP.Sales.Application.Repositories;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<PagedResult<Customer>> SearchCustomersAsync(CustomerFilterDto filter, CancellationToken ct = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetCustomersWithOrdersAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}