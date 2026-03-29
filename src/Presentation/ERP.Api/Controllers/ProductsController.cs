using Microsoft.AspNetCore.Mvc;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Inventory.Application.DTOs;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public ProductsController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken ct)
    {
        var product = await _inventoryService.GetProductByIdAsync(id, ct);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet("by-sku/{sku}")]
    public async Task<ActionResult<ProductDto>> GetBySku(string sku, CancellationToken ct)
    {
        var product = await _inventoryService.GetProductBySkuAsync(sku, ct);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken ct)
    {
        var products = await _inventoryService.GetAllProductsAsync(ct);
        return Ok(products);
    }

    [HttpGet("by-category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(Guid categoryId, CancellationToken ct)
    {
        var products = await _inventoryService.GetProductsByCategoryAsync(categoryId, ct);
        return Ok(products);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStock([FromQuery] int threshold = 10, CancellationToken ct = default)
    {
        var products = await _inventoryService.GetLowStockProductsAsync(threshold, ct);
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.SKU))
            return BadRequest("SKU is required.");

        var productId = await _inventoryService.CreateProductAsync(dto, ct);

        return CreatedAtAction(nameof(GetById), new { id = productId }, new { id = productId, dto.Name, dto.SKU });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken ct)
    {
        // Service ต้องรับ (Guid id, UpdateProductDto dto, ...)
        var result = await _inventoryService.UpdateProductAsync(id, dto, ct);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        // เปลี่ยนจากส่ง Entity เป็นส่งแค่ Guid ID
        var result = await _inventoryService.DeleteProductAsync(id, ct);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpPatch("{id}/stock")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateProductStockDto dto, CancellationToken ct)
    {
        if (id != dto.ProductId) return BadRequest("Product ID mismatch");

        var result = await _inventoryService.UpdateProductStockAsync(id, dto, ct);

        if (!result) return NotFound();
        return NoContent();
    }

    [HttpGet("exists/sku/{sku}")]
    public async Task<IActionResult> ExistsBySku(string sku, CancellationToken ct)
    {
        var exists = await _inventoryService.ExistsBySkuAsync(sku, ct);
        return Ok(new { exists });
    }
}