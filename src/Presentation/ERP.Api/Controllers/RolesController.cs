using Microsoft.AspNetCore.Mvc;
using ERP.Identity.Application.Services.Interfaces;
using ERP.Identity.Domain;

namespace ERP.Api.Controllers;

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
    public async Task<IActionResult> GetById(Guid id)
    {
        var role = await _identityService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound();
        return Ok(role);
    }

    [HttpGet("by-name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var role = await _identityService.GetRoleByNameAsync(name);
        if (role == null)
            return NotFound();
        return Ok(role);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _identityService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Role role)
    {
        await _identityService.CreateRoleAsync(role);
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Role role)
    {
        if (id != role.Id)
            return BadRequest();

        await _identityService.UpdateRoleAsync(role);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var role = await _identityService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound();

        await _identityService.DeleteRoleAsync(role);
        return NoContent();
    }

    [HttpGet("exists/name/{name}")]
    public async Task<IActionResult> ExistsByName(string name)
    {
        var exists = await _identityService.ExistsByRoleNameAsync(name);
        return Ok(new { exists });
    }
}