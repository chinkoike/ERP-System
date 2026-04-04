namespace ERP.Identity.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // 1. เพิ่มชื่อ-นามสกุล และชื่อเต็ม (สำคัญมากสำหรับโชว์ในตาราง)
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName { get; set; } = string.Empty;

    // 2. ข้อมูลตำแหน่งและแผนก (สำหรับระบบ ERP)
    public string? JobTitle { get; set; }
    public string? Department { get; set; }

    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // 3. ฟิลด์เกี่ยวกับ Role (หน้าบ้านจะได้รู้ว่า User คนนี้เป็น Admin หรือ Staff)
    public List<string> Roles { get; set; } = new();

    // Audit Fields (ที่คุณมีอยู่แล้ว)
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}