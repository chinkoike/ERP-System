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
    public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken ct)
    {
        var customer = await _salesService.GetCustomerByIdAsync(id, ct);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll(CancellationToken ct)
    {
        var customers = await _salesService.GetAllCustomersAsync(ct);
        return Ok(customers);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto, CancellationToken ct)
    {
        // Validation: ต้องมีทั้งชื่อและ Email (ตามที่เราแก้ใน Entity)
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("First Name and Email are required.");

        var customerId = await _salesService.CreateCustomerAsync(dto, ct);

        // ดึงข้อมูลที่สร้างเสร็จแล้วมาแสดง (หรือจะ Return แค่ ID ก็ได้)
        return CreatedAtAction(nameof(GetById), new { id = customerId }, new { id = customerId, dto.FirstName, dto.LastName });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerDto dto, CancellationToken ct)
    {
        // Service ตอนนี้รับ (Guid id, UpdateCustomerDto dto)
        try
        {
            await _salesService.UpdateCustomerAsync(id, dto, ct);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        // ดึงข้อมูลมาเช็คก่อนลบ
        var customer = await _salesService.GetCustomerByIdAsync(id, ct);
        if (customer == null) return NotFound();


        await _salesService.DeleteCustomerAsync(id, ct);
        return NoContent();
    }

    [HttpGet("exists/email/{email}")]
    public async Task<IActionResult> ExistsByEmail(string email, CancellationToken ct)
    {
        var exists = await _salesService.ExistsByEmailAsync(email, ct);
        return Ok(new { exists });
    }
}