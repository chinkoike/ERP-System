namespace ERP.Report.Application.DTOs;

public class SalesReportDto
{
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public IEnumerable<ReportGraphDataPointDto> MonthlySales { get; set; } = Array.Empty<ReportGraphDataPointDto>();
    public IEnumerable<ReportGraphDataPointDto> MonthlyOrderCount { get; set; } = Array.Empty<ReportGraphDataPointDto>();
}
