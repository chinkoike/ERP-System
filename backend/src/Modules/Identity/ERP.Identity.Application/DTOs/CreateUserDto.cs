using System.ComponentModel.DataAnnotations;

namespace ERP.Identity.Application.DTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "กรุณากรอก Username")]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอก Email")]
    [EmailAddress(ErrorMessage = "รูปแบบ Email ไม่ถูกต้อง")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณากรอก Password")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "รหัสผ่านต้องมีความยาวอย่างน้อย 6 ตัวอักษร")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณาระบุชื่อจริง")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "กรุณาระบุนามสกุล")]
    public string LastName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Role is required for new employees")]
    public Guid? RoleId { get; set; }

    public string? JobTitle { get; set; }
    public string? Department { get; set; }
}