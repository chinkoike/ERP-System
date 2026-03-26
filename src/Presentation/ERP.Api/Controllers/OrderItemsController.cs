using Microsoft.AspNetCore.Mvc;
using ERP.Sales.Application.Services.Interfaces;
using ERP.Sales.Domain;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly ISalesService _salesService;

    public OrderItemsController(ISalesService salesService)
    {
        _salesService = salesService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var orderItem = await _salesService.GetOrderItemByIdAsync(id);
        if (orderItem == null)
            return NotFound();
        return Ok(orderItem);
    }

    [HttpGet("by-order/{orderId}")]
    public async Task<IActionResult> GetByOrderId(Guid orderId)
    {
        var orderItems = await _salesService.GetOrderItemsByOrderIdAsync(orderId);
        return Ok(orderItems);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orderItems = await _salesService.GetAllOrderItemsAsync();
        return Ok(orderItems);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderItem orderItem)
    {
        await _salesService.CreateOrderItemAsync(orderItem);
        return CreatedAtAction(nameof(GetById), new { id = orderItem.Id }, orderItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] OrderItem orderItem)
    {
        if (id != orderItem.Id)
            return BadRequest();

        await _salesService.UpdateOrderItemAsync(orderItem);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var orderItem = await _salesService.GetOrderItemByIdAsync(id);
        if (orderItem == null)
            return NotFound();

        await _salesService.DeleteOrderItemAsync(orderItem);
        return NoContent();
    }

    [HttpPost("range")]
    public async Task<IActionResult> AddRange([FromBody] IEnumerable<OrderItem> orderItems)
    {
        await _salesService.AddOrderItemsRangeAsync(orderItems);
        return Ok();
    }

    [HttpGet("total/{orderId}")]
    public async Task<IActionResult> GetTotalByOrderId(Guid orderId)
    {
        var total = await _salesService.GetTotalByOrderIdAsync(orderId);
        return Ok(new { orderId, total });
    }
}