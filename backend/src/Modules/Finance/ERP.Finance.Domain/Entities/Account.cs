using ERP.Shared;

namespace ERP.Finance.Domain.Entities;

public class Account : BaseEntity
{
    public string AccountName { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
