using Microsoft.AspNetCore.Mvc;
using ERP.Identity.Application.Services.Interfaces;
using ERP.Identity.Domain;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserRolesController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public UserRolesController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userRole = await _identityService.GetUserRoleByIdAsync(id);
        if (userRole == null)
            return NotFound();
        return Ok(userRole);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var userRoles = await _identityService.GetUserRolesByUserIdAsync(userId);
        return Ok(userRoles);
    }

    [HttpGet("role/{roleId}")]
    public async Task<IActionResult> GetByRoleId(Guid roleId)
    {
        var userRoles = await _identityService.GetUserRolesByRoleIdAsync(roleId);
        return Ok(userRoles);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userRoles = await _identityService.GetAllUserRolesAsync();
        return Ok(userRoles);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserRole userRole)
    {
        await _identityService.CreateUserRoleAsync(userRole);
        return CreatedAtAction(nameof(GetById), new { id = userRole.Id }, userRole);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserRole userRole)
    {
        if (id != userRole.Id)
            return BadRequest();

        await _identityService.UpdateUserRoleAsync(userRole);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userRole = await _identityService.GetUserRoleByIdAsync(id);
        if (userRole == null)
            return NotFound();

        await _identityService.DeleteUserRoleAsync(userRole);
        return NoContent();
    }

    [HttpGet("exists/user/{userId}/role/{roleId}")]
    public async Task<IActionResult> ExistsByUserAndRole(Guid userId, Guid roleId)
    {
        var exists = await _identityService.ExistsByUserAndRoleAsync(userId, roleId);
        return Ok(new { exists });
    }
}