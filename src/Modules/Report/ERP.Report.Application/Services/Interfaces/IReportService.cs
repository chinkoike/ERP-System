using ERP.Report.Application.DTOs;

namespace ERP.Report.Application.Services.Interfaces;

public interface IReportService
{
    Task<SalesReportDto> GetSalesReportAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryStatusDto>> GetInventoryStatusAsync(CancellationToken cancellationToken = default);
    Task<FinancialSummaryDto> GetFinancialSummaryAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
}
