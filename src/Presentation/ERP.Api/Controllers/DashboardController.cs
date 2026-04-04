using Microsoft.AspNetCore.Mvc;
using ERP.Sales.Application.Services.Interfaces;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Sales.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
namespace ERP.Api.Controllers;

[Authorize]
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
        // ดึงข้อมูลจาก Sales Module
        var totalSales = await _salesService.GetTotalSalesAsync(null, null, ct);
        var pendingCount = await _salesService.GetPendingOrdersCountAsync(ct);
        var recentOrders = await _salesService.GetRecentOrdersAsync(5, ct);

        // ดึงข้อมูลจาก Inventory Module (สมมติว่าคุณมี Method เช็คของใกล้หมดสต็อก)
        // var lowStockCount = await _inventoryService.GetLowStockCountAsync(10, ct); 

        var summary = new DashboardSummaryDto
        {
            TotalSales = totalSales,
            PendingOrdersCount = pendingCount,
            RecentOrders = recentOrders,
            LowStockProductsCount = 0, // รอเติม Logic จาก InventoryService
            TotalCustomers = (await _salesService.GetAllCustomersAsync(ct)).Count()
        };

        return Ok(summary);
    }
}