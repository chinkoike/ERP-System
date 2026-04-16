using Microsoft.AspNetCore.Mvc;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Inventory.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using ERP.Shared;
namespace ERP.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public CategoriesController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<CategoryDto>>> GetPaged([FromQuery] CategoryFilterDto filter)
    {
        var result = await _inventoryService.GetCategoriesPagedAsync(filter);
        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken ct)
    {
        var category = await _inventoryService.GetCategoryByIdAsync(id, ct);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken ct)
    {
        var categories = await _inventoryService.GetAllCategoriesAsync(ct);
        return Ok(categories);
    }

    // แนะนำให้ใช้ CreateCategoryDto แทน CategoryDto เพื่อความปลอดภัย
    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        // ตรวจสอบความถูกต้องเบื้องต้น (Data Annotation ใน DTO จะช่วยเช็คให้อีกแรง)
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Category name is required.");

        var categoryId = await _inventoryService.CreateCategoryAsync(dto, ct);

        // ดึงข้อมูลที่เพิ่งสร้างเพื่อส่งกลับไป (หรือจะสร้าง DTO ใหม่เพื่อ return ก็ได้)
        return CreatedAtAction(nameof(GetById), new { id = categoryId }, new { id = categoryId, name = dto.Name });
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
    {
        // เรียกใช้ Service สำหรับการ Update
        var result = await _inventoryService.UpdateCategoryAsync(id, dto, ct);
        if (!result) return NotFound();

        return NoContent();
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        // ไม่ต้องดึง GetById มาเช็คซ้ำที่นี่ (ไปเช็คใน Service แล้ว throw exception หรือ return false จะคลีนกว่า)
        var result = await _inventoryService.DeleteCategoryAsync(id, ct);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpGet("{id}/product-count")]
    public async Task<IActionResult> GetProductCount(Guid id, CancellationToken ct)
    {
        var count = await _inventoryService.GetProductCountByCategoryAsync(id, ct);
        return Ok(new { categoryId = id, productCount = count });
    }
}