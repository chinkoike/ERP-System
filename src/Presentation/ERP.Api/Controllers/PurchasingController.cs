using Microsoft.AspNetCore.Mvc;
using ERP.Purchasing.Application.Services.Interfaces;
using ERP.Purchasing.Application.DTOs;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasingController : ControllerBase
{
    private readonly IPurchasingService _purchasingService;

    public PurchasingController(IPurchasingService purchasingService)
    {
        _purchasingService = purchasingService;
    }

    [HttpGet("suppliers")]
    public async Task<IActionResult> GetSuppliers(CancellationToken ct = default)
    {
        var suppliers = await _purchasingService.GetAllSuppliersAsync(ct);
        return Ok(suppliers);
    }

    [HttpGet("suppliers/{id}")]
    public async Task<IActionResult> GetSupplier(Guid id, CancellationToken ct = default)
    {
        var supplier = await _purchasingService.GetSupplierByIdAsync(id, ct);
        if (supplier is null) return NotFound();
        return Ok(supplier);
    }

    [HttpPost("suppliers")]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto dto, CancellationToken ct = default)
    {
        var id = await _purchasingService.CreateSupplierAsync(dto, ct);
        return CreatedAtAction(nameof(GetSupplier), new { id }, new { id });
    }

    [HttpPut("suppliers/{id}")]
    public async Task<IActionResult> UpdateSupplier(Guid id, [FromBody] UpdateSupplierDto dto, CancellationToken ct = default)
    {
        var updated = await _purchasingService.UpdateSupplierAsync(id, dto, ct);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("suppliers/{id}")]
    public async Task<IActionResult> DeleteSupplier(Guid id, CancellationToken ct = default)
    {
        var deleted = await _purchasingService.DeleteSupplierAsync(id, ct);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("purchase-orders")]
    public async Task<IActionResult> GetPurchaseOrders(CancellationToken ct = default)
    {
        var pos = await _purchasingService.GetAllPurchaseOrdersAsync(ct);
        return Ok(pos);
    }

    [HttpGet("purchase-orders/{id}")]
    public async Task<IActionResult> GetPurchaseOrder(Guid id, CancellationToken ct = default)
    {
        var po = await _purchasingService.GetPurchaseOrderByIdAsync(id, ct);
        if (po is null) return NotFound();
        return Ok(po);
    }

    [HttpPost("purchase-orders")]
    public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderDto dto, CancellationToken ct = default)
    {
        var id = await _purchasingService.CreatePurchaseOrderAsync(dto, ct);
        return CreatedAtAction(nameof(GetPurchaseOrder), new { id }, new { id });
    }

    [HttpPost("purchase-orders/{id}/receive")]
    public async Task<IActionResult> ReceivePurchaseOrder(Guid id, [FromBody] List<PurchaseOrderItemDto> items, CancellationToken ct = default)
    {
        var ok = await _purchasingService.ReceivePurchaseOrderAsync(id, items, ct);
        if (!ok) return BadRequest();
        return NoContent();
    }
    [HttpPatch("purchase-orders/{id}/cancel")]
    public async Task<IActionResult> CancelPurchaseOrder(Guid id, CancellationToken ct = default)
    {
        var ok = await _purchasingService.CancelPurchaseOrderAsync(id, ct);

        if (!ok)
            return BadRequest("ไม่พบใบสั่งซื้อ หรือรายการไม่สามารถยกเลิกได้");

        return NoContent();
    }
}
