using Microsoft.AspNetCore.Mvc;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Inventory.Domain;

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
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _inventoryService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }

    [HttpGet("by-sku/{sku}")]
    public async Task<IActionResult> GetBySku(string sku)
    {
        var product = await _inventoryService.GetProductBySkuAsync(sku);
        if (product == null)
            return NotFound();
        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _inventoryService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("by-category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(Guid categoryId)
    {
        var products = await _inventoryService.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 10)
    {
        var products = await _inventoryService.GetLowStockProductsAsync(threshold);
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        await _inventoryService.CreateProductAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Product product)
    {
        if (id != product.Id)
            return BadRequest();

        await _inventoryService.UpdateProductAsync(product);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _inventoryService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();

        await _inventoryService.DeleteProductAsync(product);
        return NoContent();
    }

    [HttpPatch("{id}/stock")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] int newStock)
    {
        await _inventoryService.UpdateProductStockAsync(id, newStock);
        return NoContent();
    }

    [HttpGet("exists/sku/{sku}")]
    public async Task<IActionResult> ExistsBySku(string sku)
    {
        var exists = await _inventoryService.ExistsBySkuAsync(sku);
        return Ok(new { exists });
    }
}