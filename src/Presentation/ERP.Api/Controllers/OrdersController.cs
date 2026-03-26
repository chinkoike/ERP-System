using Microsoft.AspNetCore.Mvc;
using ERP.Sales.Application.Services.Interfaces;
using ERP.Sales.Domain;

namespace ERP.Api.Controllers;

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
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _salesService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();
        return Ok(order);
    }

    [HttpGet("by-number/{orderNumber}")]
    public async Task<IActionResult> GetByOrderNumber(string orderNumber)
    {
        var order = await _salesService.GetOrderByOrderNumberAsync(orderNumber);
        if (order == null)
            return NotFound();
        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _salesService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("by-customer/{customerId}")]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var orders = await _salesService.GetOrdersByCustomerAsync(customerId);
        return Ok(orders);
    }

    [HttpGet("by-date-range")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var orders = await _salesService.GetOrdersByDateRangeAsync(startDate, endDate);
        return Ok(orders);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var orders = await _salesService.GetPendingOrdersAsync();
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Order order)
    {
        await _salesService.CreateOrderAsync(order);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Order order)
    {
        if (id != order.Id)
            return BadRequest();

        await _salesService.UpdateOrderAsync(order);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var order = await _salesService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();

        await _salesService.DeleteOrderAsync(order);
        return NoContent();
    }

    [HttpGet("total-sales")]
    public async Task<IActionResult> GetTotalSales([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var total = await _salesService.GetTotalSalesAsync(startDate, endDate);
        return Ok(new { totalSales = total });
    }

    [HttpGet("exists/order-number/{orderNumber}")]
    public async Task<IActionResult> ExistsByOrderNumber(string orderNumber)
    {
        var exists = await _salesService.ExistsByOrderNumberAsync(orderNumber);
        return Ok(new { exists });
    }

    [HttpPost("create-with-user")]
    public async Task<IActionResult> CreateOrderWithUser([FromBody] CreateOrderWithUserRequest request)
    {
        await _salesService.CreateOrderWithUserAsync(request.Username, request.Email, request.ProductSku, request.Quantity);
        return Ok(new { message = "Order created successfully with new user" });
    }

    [HttpGet("{id}/summary")]
    public async Task<IActionResult> GetOrderSummary(Guid id)
    {
        var summary = await _salesService.GetOrderSummaryAsync(id);
        return Ok(summary);
    }
}

public class CreateOrderWithUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}