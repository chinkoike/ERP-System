using Microsoft.AspNetCore.Mvc;
using ERP.Report.Application.DTOs;
using ERP.Report.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
namespace ERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("sales")]
    public async Task<ActionResult<SalesReportDto>> GetSalesReport(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var report = await _reportService.GetSalesReportAsync(startDate, endDate, cancellationToken);
        return Ok(report);
    }

    [HttpGet("inventory")]
    public async Task<ActionResult<IEnumerable<InventoryStatusDto>>> GetInventoryStatus(CancellationToken cancellationToken = default)
    {
        var status = await _reportService.GetInventoryStatusAsync(cancellationToken);
        return Ok(status);
    }

    [HttpGet("financial")]
    public async Task<ActionResult<FinancialSummaryDto>> GetFinancialSummary(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var summary = await _reportService.GetFinancialSummaryAsync(startDate, endDate, cancellationToken);
        return Ok(summary);
    }
}
