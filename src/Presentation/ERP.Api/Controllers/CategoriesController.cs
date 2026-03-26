using Microsoft.AspNetCore.Mvc;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Inventory.Application.DTOs;

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
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
    {
        var category = await _inventoryService.GetCategoryByIdAsync(id);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<CategoryDto>> GetByName(string name)
    {
        var category = await _inventoryService.GetCategoryByNameAsync(name);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _inventoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("with-products")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetWithProducts()
    {
        var categories = await _inventoryService.GetCategoriesWithProductsAsync();
        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryDto categoryDto)
    {
        if (string.IsNullOrWhiteSpace(categoryDto.Name))
            return BadRequest("Category name is required.");

        await _inventoryService.CreateCategoryAsync(categoryDto);

        // ส่งกลับ 201 Created พร้อมบอก Path ในการดูข้อมูลที่เพิ่งสร้าง
        return CreatedAtAction(nameof(GetById), new { id = categoryDto.Id }, categoryDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryDto categoryDto)
    {
        if (id != categoryDto.Id)
            return BadRequest("ID mismatch");

        await _inventoryService.UpdateCategoryAsync(categoryDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var exists = await _inventoryService.GetCategoryByIdAsync(id);
        if (exists == null)
            return NotFound();

        // เรียกใช้ Delete ผ่าน ID ตามที่เราแก้ใน Service
        await _inventoryService.DeleteCategoryAsync(id);
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