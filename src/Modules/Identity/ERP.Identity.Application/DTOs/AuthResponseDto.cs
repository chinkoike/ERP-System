namespace ERP.Identity.Application.DTOs;

public class AuthResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; } // สำหรับ JWT Token ในอนาคต
    public UserDto? User { get; set; }  // ข้อมูล User เบื้องต้น (ที่ไม่มี Password)
    public List<string> Roles { get; set; } = new();
}