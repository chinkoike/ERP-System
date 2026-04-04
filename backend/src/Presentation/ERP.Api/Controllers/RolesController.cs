using Microsoft.AspNetCore.Mvc;
using ERP.Identity.Application.Services.Interfaces;
using ERP.Identity.Application.DTOs; // อย่าลืมใช้ DTOs
using Microsoft.AspNetCore.Authorization;
namespace ERP.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public RolesController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetById(Guid id, CancellationToken ct)
    {
        var role = await _identityService.GetRoleByIdAsync(id, ct);
        if (role == null) return NotFound();
        return Ok(role);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll(CancellationToken ct)
    {
        var roles = await _identityService.GetAllRolesAsync(ct);
        return Ok(roles);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto, CancellationToken ct)
    {
        var roleId = await _identityService.CreateRoleAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = roleId }, new { id = roleId });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleDto dto, CancellationToken ct)
    {
        // ใช้ Pattern Task<bool> เหมือนเดิม
        var result = await _identityService.UpdateRoleAsync(id, dto, ct);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        // ส่งแค่ Guid id ไปที่ Service
        var result = await _identityService.DeleteRoleAsync(id, ct);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpGet("exists/name/{name}")]
    public async Task<IActionResult> ExistsByName(string name, CancellationToken ct)
    {
        var exists = await _identityService.ExistsByRoleNameAsync(name, ct);
        return Ok(new { exists });
    }
}