namespace ERP.Finance.Application.DTOs;

public class AccountDto
{
    public Guid Id { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
