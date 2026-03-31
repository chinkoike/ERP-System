using BCrypt.Net;
using ERP.Shared;
using ERP.Identity.Application.Services.Interfaces;
using ERP.Identity.Domain;
using ERP.Identity.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using ERP.Shared.Exceptions;
using System.Security.Claims;
using ERP.Identity.Application.Exceptions;
namespace ERP.Identity.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public IdentityService(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }


    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<User>();

        // 1. ค้นหา User
        var users = await repo.FindAsync(u => u.Username == loginRequest.Username, cancellationToken);
        var user = users.FirstOrDefault();

        // 2. ตรวจสอบ User และสถานะ Active
        if (user == null || !user.IsActive)
        {
            return new AuthResponseDto { IsSuccess = false, Message = "Invalid username or password." };
        }

        // 3. ตรวจสอบ Password ด้วย BCrypt
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return new AuthResponseDto { IsSuccess = false, Message = "Invalid username or password." };
        }

        // 4. ดึงข้อมูล Roles
        var userRoles = await _unitOfWork.Repository<UserRole>()
            .FindAsync(ur => ur.UserId == user.Id, cancellationToken);

        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
        var roles = await _unitOfWork.Repository<Role>()
            .FindAsync(r => roleIds.Contains(r.Id), cancellationToken);

        var rolesList = roles.Select(r => r.Name).ToList();

        // 5. สร้าง Access Token และ Refresh Token
        var userDto = MapToUserDto(user);
        var accessToken = _tokenService.GenerateToken(userDto, rolesList);
        var refreshToken = _tokenService.GenerateRefreshToken(); // เรียกใช้ตัวที่เราเพิ่มใหม่ใน TokenService

        // 6. อัปเดต Refresh Token ลงใน User Entity (Database)
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // ตั้งอายุไว้ 7 วัน (หรือตาม JwtSettings)

        await repo.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. ส่งข้อมูลกลับ
        return new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Login successful",
            User = userDto,
            Roles = rolesList,
            Token = accessToken,
            RefreshToken = refreshToken // อย่าลืมส่งตัวนี้กลับไปให้หน้าบ้านเก็บไว้ด้วย
        };
    }
    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ct)
    {
        // 1. แกะข้อมูลจาก Token ที่หมดอายุ (Expired Access Token)
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var username = principal.Identity?.Name;

        var users = await _unitOfWork.Repository<User>()
              .FindAsync(u => u.Username == username, ct);
        var user = users.FirstOrDefault();

        // 3. ตรวจสอบว่า Refresh Token ใน DB ตรงกับที่ส่งมาไหม และยังไม่หมดอายุใช่ไหม
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid refresh token attempt");
        }

        // 4. สร้าง Token ชุดใหม่ (Rotation)
        var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var newAccessToken = _tokenService.GenerateToken(MapToUserDto(user), roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // 5. บันทึก Refresh Token ใหม่ลง DB
        user.RefreshToken = newRefreshToken;
        await _unitOfWork.Repository<User>().UpdateAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<bool> LogoutAsync(LogoutRequestDto request, CancellationToken ct)
    {
        var users = await _unitOfWork.Repository<User>()
            .FindAsync(u => u.RefreshToken == request.RefreshToken, ct);

        var user = users.FirstOrDefault();

        if (user == null) return false;

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _unitOfWork.Repository<User>().UpdateAsync(user, ct);

        return await _unitOfWork.SaveChangesAsync(ct) > 0;
    }
    // --- User Operations ---

    public async Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(id, cancellationToken);
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Repository<User>().FindAsync(u => u.Username == username, cancellationToken);
        var user = users.FirstOrDefault();
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Repository<User>().FindAsync(u => u.Email == email, cancellationToken);
        var user = users.FirstOrDefault();
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Repository<User>().GetAllAsync(cancellationToken);
        return users.Select(MapToUserDto);
    }

    public async Task<IEnumerable<UserDto>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Repository<User>().FindAsync(u => u.IsActive, cancellationToken);
        return users.Select(MapToUserDto);
    }

    public async Task<UserDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var userRepo = _unitOfWork.Repository<User>();

        // 1. ตรวจสอบว่า Email หรือ Username ซ้ำไหม (เปลี่ยนมาใช้ request.xxx)
        var existingUsers = await userRepo.FindAsync(u =>
            u.Email == request.Email || u.Username == request.Username, cancellationToken);

        if (existingUsers.Any())
        {
            // เปลี่ยนมาใช้ BadRequestException เพื่อให้ Middleware ส่ง Status 400 กลับไป
            throw new BadRequestException("Username or Email already exists.");
        }

        // 2. เตรียมสร้าง User ใหม่
        var newUser = new User
        {
            Username = request.Username,
            Email = request.Email,
            // Hash Password จาก request
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await userRepo.AddAsync(newUser, cancellationToken);

        // 3. กำหนด Role ให้ User
        // วิธีที่ 1: ใช้ RoleId ที่ส่งมาจากหน้าบ้าน (request.RoleId)
        // วิธีที่ 2: ถ้าไม่ได้ส่งมา ให้ดึงจาก DB ตามชื่อ "User" (แบบที่คุณทำ)

        var roleRepo = _unitOfWork.Repository<Role>();
        var targetRoleId = request.RoleId; // ใช้ตัวที่ส่งมาจากหน้าบ้านเป็นหลัก

        // ถ้าหน้าบ้านไม่ได้ส่ง RoleId มา (เป็น Guid.Empty) ให้หา Default Role
        if (targetRoleId == Guid.Empty)
        {
            var defaultRoles = await roleRepo.FindAsync(r => r.Name == "User", cancellationToken);
            var defaultRole = defaultRoles.FirstOrDefault();
            if (defaultRole != null) targetRoleId = defaultRole.Id;
        }

        if (targetRoleId != Guid.Empty)
        {
            var userRole = new UserRole
            {
                User = newUser, // EF จะผูก UserId ให้เองอัตโนมัติ
                RoleId = targetRoleId
            };
            await _unitOfWork.Repository<UserRole>().AddAsync(userRole, cancellationToken);
        }

        // 4. บันทึกข้อมูลทั้งหมด (Unit of Work)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. โหลดข้อมูลกลับมาพร้อม Role (เพื่อให้ JSON ไม่ว่าง)
        var resultUser = await userRepo.GetQueryable()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == newUser.Id, cancellationToken);

        return MapToUserDto(resultUser ?? newUser);
    }
    public async Task<Guid> CreateUserAsync(CreateUserDto dto, CancellationToken ct)
    {
        // 1. ตรวจสอบข้อมูลซ้ำ
        var exists = await _unitOfWork.Repository<User>().AnyAsync(u =>
            u.Username == dto.Username || u.Email == dto.Email, ct);

        if (exists)
        {
            throw new BadRequestException("Username or Email already exists.");
        }

        // 2. สร้าง User ใหม่
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            JobTitle = dto.JobTitle,
            Department = dto.Department,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Admin" // ระบุว่าสร้างโดย Admin
        };

        await _unitOfWork.Repository<User>().AddAsync(newUser, ct);

        // 3. จัดการเรื่อง Role (ตรวจสอบ dto.RoleId)
        if (dto.RoleId.HasValue)
        {
            var userRole = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = newUser.Id,
                RoleId = dto.RoleId.Value,
                AssignedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<UserRole>().AddAsync(userRole, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return newUser.Id;
    }
    public async Task<bool> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken ct)
    {
        var repo = _unitOfWork.Repository<User>();
        var user = await repo.GetByIdAsync(id, ct);

        if (user == null) return false;

        user.Email = dto.Email;
        user.IsActive = dto.IsActive;
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;


        repo.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }
    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken ct)
    {
        var repo = _unitOfWork.Repository<User>();
        var user = await repo.GetByIdAsync(id, ct);
        if (user == null) return false;

        repo.Remove(user); // หรือ .Delete() ตามที่ Repository คุณมี
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<User>().ExistsAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<User>().ExistsAsync(u => u.Email == email, cancellationToken);
    }

    // --- Role Operations ---

    public async Task<RoleDto?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _unitOfWork.Repository<Role>().GetByIdAsync(id, cancellationToken);
        return role == null ? null : MapToRoleDto(role);
    }

    public async Task<RoleDto?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var roles = await _unitOfWork.Repository<Role>().FindAsync(r => r.Name == name, cancellationToken);
        var role = roles.FirstOrDefault();
        return role == null ? null : MapToRoleDto(role);
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _unitOfWork.Repository<Role>().GetAllAsync(cancellationToken);
        return roles.Select(MapToRoleDto);
    }

    // 1. เปลี่ยน Return Type จาก Task<Guid> เป็น Task<RoleDto> ให้ตรงกับ Interface
    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto, CancellationToken ct)
    {
        // 2. สร้าง Entity จาก DTO
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
            // CreatedAt = DateTime.UtcNow // ถ้ามี
        };

        // 3. บันทึกลงฐานข้อมูล
        await _unitOfWork.Repository<Role>().AddAsync(role, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // 4. สร้าง RoleDto เพื่อส่งกลับไป (ต้อง Return เป็น RoleDto ตามที่ Interface สั่ง)
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
            // CreatedAt = role.CreatedAt
        };
    }

    public async Task<bool> UpdateRoleAsync(Guid id, UpdateRoleDto dto, CancellationToken ct)
    {
        var repo = _unitOfWork.Repository<Role>();
        var role = await repo.GetByIdAsync(id, ct);
        if (role == null) return false;

        role.Name = dto.Name;
        role.Description = dto.Description;

        repo.Update(role);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteRoleAsync(Guid id, CancellationToken ct)
    {
        var repo = _unitOfWork.Repository<Role>();
        var role = await repo.GetByIdAsync(id, ct);
        if (role == null) return false;

        repo.Remove(role); // หรือ .Delete()
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ExistsByRoleNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<Role>().ExistsAsync(r => r.Name == name, cancellationToken);
    }

    // --- UserRole Operations ---

    public async Task<UserRoleDto?> GetUserRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userRole = await _unitOfWork.Repository<UserRole>().GetByIdAsync(id, cancellationToken);
        return userRole == null ? null : MapToUserRoleDto(userRole);
    }

    public async Task<IEnumerable<UserRoleDto>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _unitOfWork.Repository<UserRole>().FindAsync(ur => ur.UserId == userId, cancellationToken);
        return userRoles.Select(MapToUserRoleDto);
    }

    public async Task<IEnumerable<UserRoleDto>> GetUserRolesByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _unitOfWork.Repository<UserRole>().FindAsync(ur => ur.RoleId == roleId, cancellationToken);
        return userRoles.Select(MapToUserRoleDto);
    }

    public async Task<IEnumerable<UserRoleDto>> GetAllUserRolesAsync(CancellationToken cancellationToken = default)
    {
        var userRoles = await _unitOfWork.Repository<UserRole>().GetAllAsync(cancellationToken);
        return userRoles.Select(MapToUserRoleDto);
    }

    public async Task CreateUserRoleAsync(UserRoleDto dto, CancellationToken cancellationToken = default)
    {
        var userRole = new UserRole
        {
            UserId = dto.UserId,
            RoleId = dto.RoleId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = dto.CreatedBy ?? "System"
        };
        await _unitOfWork.Repository<UserRole>().AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateUserRoleAsync(UserRoleDto dto, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<UserRole>();
        var userRole = await repo.GetByIdAsync(dto.Id, cancellationToken);
        if (userRole != null)
        {
            userRole.UserId = dto.UserId;
            userRole.RoleId = dto.RoleId;
            userRole.LastModifiedAt = DateTime.UtcNow;
            userRole.LastModifiedBy = dto.UpdatedBy ?? "System";

            repo.Update(userRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteUserRoleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<UserRole>();
        var userRole = await repo.GetByIdAsync(id, cancellationToken);
        if (userRole != null)
        {
            repo.Remove(userRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };
        await _unitOfWork.Repository<UserRole>().AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<UserRole>();
        var userRoles = await repo.FindAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
        var userRole = userRoles.FirstOrDefault();
        if (userRole != null)
        {
            repo.Remove(userRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<UserRole>().ExistsAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    // --- Private Helper Mappers ---

    private static UserDto MapToUserDto(User u) => new()
    {
        Id = u.Id,
        Username = u.Username,
        Email = u.Email,
        IsActive = u.IsActive,
        Roles = u.UserRoles?
    .Select(ur => ur.Role?.Name)
    .Where(name => name != null)
    .Cast<string>()
    .ToList() ?? new List<string>(),
        CreatedAt = u.CreatedAt,
        CreatedBy = u.CreatedBy ?? "System",
        UpdatedAt = u.LastModifiedAt,
        UpdatedBy = u.LastModifiedBy
    };

    private static RoleDto MapToRoleDto(Role r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Description = r.Description,
        CreatedAt = r.CreatedAt,
        CreatedBy = r.CreatedBy ?? "System",
        UpdatedAt = r.LastModifiedAt,
        UpdatedBy = r.LastModifiedBy
    };

    private static UserRoleDto MapToUserRoleDto(UserRole ur) => new()
    {
        Id = ur.Id,
        UserId = ur.UserId,
        RoleId = ur.RoleId,
        CreatedAt = ur.CreatedAt,
        CreatedBy = ur.CreatedBy ?? "System",
        UpdatedAt = ur.LastModifiedAt,
        UpdatedBy = ur.LastModifiedBy
    };
}