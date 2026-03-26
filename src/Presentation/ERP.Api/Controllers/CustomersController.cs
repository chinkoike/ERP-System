using Microsoft.AspNetCore.Mvc;
using ERP.Sales.Application.Services.Interfaces;
using ERP.Sales.Application.DTOs;

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
    public async Task<ActionResult<CustomerDto>> GetById(Guid id)
    {
        var customer = await _salesService.GetCustomerByIdAsync(id);
        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    [HttpGet("by-email/{email}")]
    public async Task<ActionResult<CustomerDto>> GetByEmail(string email)
    {
        var customer = await _salesService.GetCustomerByEmailAsync(email);
        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _salesService.GetAllCustomersAsync();
        return Ok(customers);
    }

    [HttpGet("with-orders")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetWithOrders()
    {
        var customers = await _salesService.GetCustomersWithOrdersAsync();
        return Ok(customers);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerDto customerDto)
    {
        // Validation เบื้องต้น
        if (string.IsNullOrWhiteSpace(customerDto.Email))
            return BadRequest("Customer email is required.");

        await _salesService.CreateCustomerAsync(customerDto);

        return CreatedAtAction(nameof(GetById), new { id = customerDto.Id }, customerDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CustomerDto customerDto)
    {
        if (id != customerDto.Id)
            return BadRequest("ID mismatch");

        await _salesService.UpdateCustomerAsync(customerDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // เช็คว่ามีตัวตนอยู่จริงไหมก่อนสั่งลบ
        var exists = await _salesService.GetCustomerByIdAsync(id);
        if (exists == null)
            return NotFound();

        // แก้ไขให้ส่งแค่ Guid id ตามที่ Refactor ใน Service ไว้
        await _salesService.DeleteCustomerAsync(id);
        return NoContent();
    }

    [HttpGet("exists/email/{email}")]
    public async Task<IActionResult> ExistsByEmail(string email)
    {
        var exists = await _salesService.ExistsByEmailAsync(email);
        return Ok(new { exists });
    }
}