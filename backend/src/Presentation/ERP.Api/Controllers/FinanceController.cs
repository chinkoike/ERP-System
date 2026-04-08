using Microsoft.AspNetCore.Mvc;
using ERP.Finance.Application.Services.Interfaces;
using ERP.Finance.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FinanceController : ControllerBase
{
    private readonly IFinanceService _financeService;

    public FinanceController(IFinanceService financeService)
    {
        _financeService = financeService;
    }

    [HttpGet("invoices")]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAllInvoices(CancellationToken ct)
    {
        var invoices = await _financeService.GetAllInvoicesAsync(ct);
        return Ok(invoices);
    }

    [HttpGet("invoices/{id}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoiceById(Guid id, CancellationToken ct)
    {
        var invoice = await _financeService.GetInvoiceByIdAsync(id, ct);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    [HttpPost("invoices")]
    public async Task<ActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto, CancellationToken ct)
    {
        var id = await _financeService.CreateInvoiceAsync(dto, ct);
        return CreatedAtAction(nameof(GetInvoiceById), new { id = id }, new { id = id });
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPatch("invoices/{id}/pay")]
    public async Task<ActionResult> SetInvoicePaid(Guid id, CancellationToken ct)
    {
        var result = await _financeService.SetInvoicePaidAsync(id, ct);
        if (!result) return NotFound();
        return NoContent();
    }
    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("invoices/{id}")]
    public async Task<ActionResult> UpdateInvoice(Guid id, [FromBody] UpdateInvoiceDto dto, CancellationToken ct)
    {
        var result = await _financeService.UpdateInvoiceAsync(id, dto, ct);

        if (!result) return NotFound();

        return NoContent(); // 204 Success แต่ไม่ต้องส่งข้อมูลกลับ
    }
    [HttpGet("payments/invoice/{invoiceId}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByInvoiceId(Guid invoiceId, CancellationToken ct)
    {
        var payments = await _financeService.GetPaymentsByInvoiceIdAsync(invoiceId, ct);
        return Ok(payments);
    }

    [HttpPost("payments")]
    public async Task<ActionResult> CreatePayment([FromBody] CreatePaymentDto dto, CancellationToken ct)
    {
        var id = await _financeService.CreatePaymentAsync(dto, ct);
        return CreatedAtAction(nameof(GetPaymentsByInvoiceId), new { invoiceId = dto.InvoiceId }, new { id = id });
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("accounts")]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccounts(CancellationToken ct)
    {
        var accounts = await _financeService.GetAllAccountsAsync(ct);
        return Ok(accounts);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("accounts")]
    public async Task<ActionResult> CreateAccount([FromBody] CreateAccountDto dto, CancellationToken ct)
    {
        var id = await _financeService.CreateAccountAsync(dto, ct);
        return CreatedAtAction(nameof(GetAccounts), new { id = id }, new { id = id });
    }
}
