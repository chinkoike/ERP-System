using ERP.Finance.Domain.Entities;
using ERP.Shared;

namespace ERP.Finance.Application.Repositories;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<Account?> GetByAccountCodeAsync(string accountCode, CancellationToken cancellationToken = default);
}
