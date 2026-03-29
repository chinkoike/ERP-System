using ERP.Shared;

namespace ERP.Identity.Domain;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? PasswordHash { get; set; }

    // 1. Salt สำหรับการ Hash (ถ้าไม่ได้ใช้ Identity Library สำเร็จรูป)
    public string? PasswordSalt { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // 2. FullName (Computed Property) เพื่อให้หน้าบ้านดึงไปโชว์ง่ายๆ
    public string FullName => $"{FirstName} {LastName}".Trim();

    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // 3. ระบบ Audit (มักจะอยู่ใน BaseEntity แต่ถ้าไม่มีให้เพิ่มที่นี่)
    public DateTime? UpdatedAt { get; set; }

    // 4. ข้อมูลเสริมสำหรับ User ในระบบ ERP
    public string? JobTitle { get; set; }      // ตำแหน่งงาน
    public string? Department { get; set; }    // แผนก (สำคัญมากสำหรับ ERP)
    public string? AvatarUrl { get; set; }     // รูปโปรไฟล์

    // 5. Security & Verification
    public bool EmailConfirmed { get; set; } = false;
    public string? RefreshToken { get; set; }  // สำหรับระบบ Login (JWT)
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = [];
}