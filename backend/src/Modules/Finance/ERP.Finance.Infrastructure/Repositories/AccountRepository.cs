using ERP.Finance.Application.Repositories;
using ERP.Finance.Domain.Entities;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Finance.Infrastructure.Repositories;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Account?> GetByAccountCodeAsync(string accountCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accountCode))
            throw new ArgumentException("accountCode cannot be null or empty", nameof(accountCode));

        var accounts = await FindAsync(a => a.AccountCode == accountCode, cancellationToken);
        return accounts.FirstOrDefault();
    }
}
