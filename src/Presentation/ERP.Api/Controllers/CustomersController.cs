using Microsoft.AspNetCore.Mvc;
using ERP.Sales.Application.Services.Interfaces;
using ERP.Sales.Domain;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ISalesService _salesService;

    public CustomersController(ISalesService salesService)
    {
        _salesService = salesService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _salesService.GetCustomerByIdAsync(id);
        if (customer == null)
            return NotFound();
        return Ok(customer);
    }

    [HttpGet("by-email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var customer = await _salesService.GetCustomerByEmailAsync(email);
        if (customer == null)
            return NotFound();
        return Ok(customer);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _salesService.GetAllCustomersAsync();
        return Ok(customers);
    }

    [HttpGet("with-orders")]
    public async Task<IActionResult> GetWithOrders()
    {
        var customers = await _salesService.GetCustomersWithOrdersAsync();
        return Ok(customers);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Customer customer)
    {
        await _salesService.CreateCustomerAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Customer customer)
    {
        if (id != customer.Id)
            return BadRequest();

        await _salesService.UpdateCustomerAsync(customer);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _salesService.GetCustomerByIdAsync(id);
        if (customer == null)
            return NotFound();

        await _salesService.DeleteCustomerAsync(customer);
        return NoContent();
    }

    [HttpGet("exists/email/{email}")]
    public async Task<IActionResult> ExistsByEmail(string email)
    {
        var exists = await _salesService.ExistsByEmailAsync(email);
        return Ok(new { exists });
    }
}