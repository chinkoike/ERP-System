using Microsoft.AspNetCore.Mvc;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Inventory.Domain;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public CategoriesController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _inventoryService.GetCategoryByIdAsync(id);
        if (category == null)
            return NotFound();
        return Ok(category);
    }

    [HttpGet("by-name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var category = await _inventoryService.GetCategoryByNameAsync(name);
        if (category == null)
            return NotFound();
        return Ok(category);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _inventoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("with-products")]
    public async Task<IActionResult> GetWithProducts()
    {
        var categories = await _inventoryService.GetCategoriesWithProductsAsync();
        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Category category)
    {
        await _inventoryService.CreateCategoryAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Category category)
    {
        if (id != category.Id)
            return BadRequest();

        await _inventoryService.UpdateCategoryAsync(category);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _inventoryService.GetCategoryByIdAsync(id);
        if (category == null)
            return NotFound();

        await _inventoryService.DeleteCategoryAsync(category);
        return NoContent();
    }

    [HttpGet("{id}/product-count")]
    public async Task<IActionResult> GetProductCount(Guid id)
    {
        var count = await _inventoryService.GetProductCountByCategoryAsync(id);
        return Ok(new { categoryId = id, productCount = count });
    }

    [HttpGet("exists/name/{name}")]
    public async Task<IActionResult> ExistsByName(string name)
    {
        var exists = await _inventoryService.ExistsByCategoryNameAsync(name);
        return Ok(new { exists });
    }
}