using System.ComponentModel.DataAnnotations;

namespace ERP.Identity.Application.DTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    // เผื่ออนาคตอยากทำระบบ "จดจำฉัน" (Remember Me)
    public bool RememberMe { get; set; } = false;
}