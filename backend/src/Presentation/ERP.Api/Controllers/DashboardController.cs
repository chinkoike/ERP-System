using Microsoft.AspNetCore.Mvc;
using ERP.Sales.Application.Services.Interfaces;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Sales.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
namespace ERP.Api.Controllers;

[Authorize(Roles = "Admin,Manager")]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ISalesService _salesService;
    private readonly IInventoryService _inventoryService;

    public DashboardController(
        ISalesService salesService,
        IInventoryService inventoryService)
    {
        _salesService = salesService;
        _inventoryService = inventoryService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        var totalSales = await _salesService.GetTotalSalesAsync(startOfMonth, now, ct);
        var pendingCount = await _salesService.GetPendingOrdersCountAsync(ct);
        var recentOrders = await _salesService.GetRecentOrdersAsync(5, ct);
        var lowStockProducts = await _inventoryService.GetLowStockProductsAsync(10, ct);

        var summary = new DashboardSummaryDto
        {
            TotalSales = totalSales,
            PendingOrdersCount = pendingCount,
            RecentOrders = recentOrders,
            LowStockProductsCount = lowStockProducts.Count(),
            TotalCustomers = (await _salesService.GetAllCustomersAsync(ct)).Count()
        };

        return Ok(summary);
    }
}