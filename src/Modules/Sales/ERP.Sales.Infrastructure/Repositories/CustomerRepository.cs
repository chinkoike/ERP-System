using ERP.Sales.Application.Repositories;
using ERP.Sales.Domain;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Sales.Infrastructure.Repositories;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        var customers = await FindAsync(c => c.Email == email, cancellationToken);
        return customers.FirstOrDefault();
    }

    public async Task<IEnumerable<Customer>> GetCustomersWithOrdersAsync(CancellationToken cancellationToken = default)
    {
        // Note: This would require a more specific query with includes
        // For now, return all customers - in a real implementation you'd use
        // the specific DbContext to include related orders
        return await GetAllAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        return await ExistsAsync(c => c.Email == email, cancellationToken);
    }
}