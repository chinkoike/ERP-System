using Microsoft.AspNetCore.Mvc;
using ERP.Identity.Application.Services.Interfaces;
using ERP.Identity.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ERP.Api.Controllers;

[Authorize]
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
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken ct)
    {
        var user = await _identityService.GetUserByIdAsync(id, ct);
        if (user == null) return NotFound();
        return Ok(user);
        throw new Exception("Test Middleware Success!");
    }

    [HttpGet("by-username/{username}")]
    public async Task<ActionResult<UserDto>> GetByUsername(string username, CancellationToken ct)
    {
        var user = await _identityService.GetUserByUsernameAsync(username, ct);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken ct)
    {
        var users = await _identityService.GetAllUsersAsync(ct);
        return Ok(users);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetActive(CancellationToken ct)
    {
        var users = await _identityService.GetActiveUsersAsync(ct);
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken ct)
    {
        // รับค่าเป็น CreateUserDto (ที่มี Password)
        // และให้ Service คืนค่ามาเป็น UserDto หรือ Guid ID
        var userId = await _identityService.CreateUserAsync(dto, ct);

        return CreatedAtAction(nameof(GetById), new { id = userId }, new { id = userId, dto.Username });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto, CancellationToken ct)
    {
        var result = await _identityService.UpdateUserAsync(id, dto, ct);
        if (!result) return NotFound();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        // ปรับให้ส่งแค่ Guid ID เพื่อแก้ Error: cannot convert from UserDto to Guid
        var result = await _identityService.DeleteUserAsync(id, ct);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpGet("exists/username/{username}")]
    public async Task<IActionResult> ExistsByUsername(string username, CancellationToken ct)
    {
        var exists = await _identityService.ExistsByUsernameAsync(username, ct);
        return Ok(new { exists });
    }

    [HttpGet("exists/email/{email}")]
    public async Task<IActionResult> ExistsByEmail(string email, CancellationToken ct)
    {
        var exists = await _identityService.ExistsByEmailAsync(email, ct);
        return Ok(new { exists });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        // เรียก Service ที่คุณเพิ่งเขียน
        var user = await _identityService.RegisterAsync(request, ct);

        // ถ้าสำเร็จจะคืนค่า 200 OK พร้อมข้อมูล User (ที่ไม่มี Password)
        return Ok(user);
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken ct)
    {
        // เรียก Service เพื่อตรวจสอบ User และสร้าง Token
        var response = await _identityService.LoginAsync(request, ct);

        if (response == null) return Unauthorized(new { message = "Username หรือ Password ไม่ถูกต้อง" });

        return Ok(response);
    }
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken ct)
    {
        try
        {
            var result = await _identityService.RefreshTokenAsync(request, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request, CancellationToken ct)
    {
        var result = await _identityService.LogoutAsync(request, ct);
        if (!result) return BadRequest(new { message = "Invalid logout request." });
        return Ok(new { message = "Logged out successfully" });
    }


}