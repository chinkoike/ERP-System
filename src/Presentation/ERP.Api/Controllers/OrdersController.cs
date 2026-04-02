using Microsoft.AspNetCore.Mvc;
using ERP.Sales.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ERP.Sales.Application.DTOs;
using ERP.Sales.Application.DTOs.Requests;
using ERP.Identity.Application.DTOs;
using ERP.Sales.Domain;
namespace ERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ISalesService _salesService;

    public OrdersController(ISalesService salesService)
    {
        _salesService = salesService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderSummaryDto>> GetById(Guid id, CancellationToken ct)
    {
        var order = await _salesService.GetOrderSummaryAsync(id, ct);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetAll(CancellationToken ct)
    {
        // ใช้ DTO สรุปรายการเพื่อลดภาระ Database และ Network
        var orders = await _salesService.GetRecentOrdersAsync(50, ct);
        return Ok(orders);
    }

    [HttpGet("by-customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<OrderSummaryDto>>> GetByCustomer(Guid customerId, CancellationToken ct)
    {
        var orders = await _salesService.GetOrdersByCustomerAsync(customerId, ct);
        return Ok(orders);
    }

    // จุดสำคัญ: เปลี่ยนจากการรับ Order Entity เป็น CreateOrderDto
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        try
        {
            var orderId = await _salesService.PlaceOrderAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = orderId }, new { id = orderId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] OrderStatus newStatus, CancellationToken ct)
    {
        var result = await _salesService.UpdateOrderStatusAsync(id, newStatus, ct);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _salesService.CancelOrderAsync(id, ct);
            if (!result) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("total-sales")]
    public async Task<IActionResult> GetTotalSales([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken ct)
    {
        var total = await _salesService.GetTotalSalesAsync(startDate, endDate, ct);
        return Ok(new { totalSales = total });
    }

    [HttpPost("create-with-user")]
    public async Task<IActionResult> CreateOrderWithUser([FromBody] CreateOrderWithUserRequest request, CancellationToken ct)
    {
        try
        {
            var registerReq = new RegisterRequest
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = request.RoleId
            };

            // เรียกใช้งานใหม่ด้วย 3-4 arguments ตามที่ Interface กำหนด
            await _salesService.CreateOrderWithUserAsync(
                registerReq,
                request.ProductSku,
                request.Quantity,
                ct);

            return Ok(new { message = "Order created successfully with new user" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

