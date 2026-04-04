namespace ERP.Report.Application.DTOs;

public class FinancialSummaryDto
{
    public decimal TotalInvoiced { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalReceivable { get; set; }
    public IEnumerable<ReportGraphDataPointDto> MonthlyRevenue { get; set; } = Array.Empty<ReportGraphDataPointDto>();
    public IEnumerable<ReportGraphDataPointDto> MonthlyInvoiceAmount { get; set; } = Array.Empty<ReportGraphDataPointDto>();
    public IEnumerable<AccountBalanceDto> TopAccounts { get; set; } = Array.Empty<AccountBalanceDto>();
}

public class AccountBalanceDto
{
    public Guid AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
