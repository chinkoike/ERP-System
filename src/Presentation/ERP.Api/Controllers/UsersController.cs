using Microsoft.AspNetCore.Mvc;
using ERP.Identity.Application.Services.Interfaces;
using ERP.Identity.Domain;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public UsersController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _identityService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpGet("by-username/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var user = await _identityService.GetUserByUsernameAsync(username);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpGet("by-email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var user = await _identityService.GetUserByEmailAsync(email);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _identityService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var users = await _identityService.GetActiveUsersAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User user)
    {
        await _identityService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] User user)
    {
        if (id != user.Id)
            return BadRequest();

        await _identityService.UpdateUserAsync(user);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _identityService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        await _identityService.DeleteUserAsync(user);
        return NoContent();
    }

    [HttpGet("exists/username/{username}")]
    public async Task<IActionResult> ExistsByUsername(string username)
    {
        var exists = await _identityService.ExistsByUsernameAsync(username);
        return Ok(new { exists });
    }

    [HttpGet("exists/email/{email}")]
    public async Task<IActionResult> ExistsByEmail(string email)
    {
        var exists = await _identityService.ExistsByEmailAsync(email);
        return Ok(new { exists });
    }
}