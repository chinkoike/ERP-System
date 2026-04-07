using BCrypt.Net;
using ERP.Shared;
using ERP.Identity.Application.Services.Interfaces;
using ERP.Identity.Application.Repositories;
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
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public IdentityService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
    }


    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest, CancellationToken cancellationToken = default)
    {
        // 1. ค้นหา User
        var user = await _userRepository.GetByUsernameAsync(loginRequest.Username, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return new AuthResponseDto { IsSuccess = false, Message = "Invalid username or password." };
        }

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
        var userRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == user.Id, cancellationToken);

        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
        var roles = await _roleRepository.FindAsync(r => roleIds.Contains(r.Id), cancellationToken);

        var rolesList = roles.Select(r => r.Name).ToList();

        // 5. อัปเดตเวลา Login ล่าสุด
        user.LastLoginAt = DateTime.UtcNow;

        // 6. สร้าง Access Token และ Refresh Token
        var userDto = MapToUserDto(user);
        userDto.Roles = rolesList;
        var accessToken = _tokenService.GenerateToken(userDto, rolesList);
        var refreshToken = _tokenService.GenerateRefreshToken(); // เรียกใช้ตัวที่เราเพิ่มใหม่ใน TokenService

        // 7. อัปเดต Refresh Token และเวลา Login ล่าสุดลงใน User Entity (Database)
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // ตั้งอายุไว้ 7 วัน (หรือตาม JwtSettings)

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 8. ส่งข้อมูลกลับ
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
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var username = principal.Identity?.Name;

        var user = await _userRepository.GetByUsernameAsync(username ?? string.Empty, ct);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid refresh token attempt");
        }

        //  สร้าง Token ชุดใหม่ (Rotation)
        var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userDto = MapToUserDto(user);
        userDto.Roles = roles;
        var newAccessToken = _tokenService.GenerateToken(userDto, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        //  บันทึก Refresh Token ใหม่ลง DB พร้อมอายุใหม่
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new AuthResponseDto
        {
            IsSuccess = true,
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            User = userDto,
            Roles = roles
        };
    }

    public async Task<bool> LogoutAsync(LogoutRequestDto request, CancellationToken ct)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, ct);

        if (user == null) return false;

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _userRepository.UpdateAsync(user, ct);

        return await _unitOfWork.SaveChangesAsync(ct) > 0;
    }
    // --- User Operations ---

    public async Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetQueryable()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user == null ? null : MapToUserDto(user);
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetQueryable()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return users.Select(MapToUserDto);
    }

    public async Task<IEnumerable<UserDto>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetQueryable()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.IsActive)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return users.Select(MapToUserDto);
    }

    public async Task<UserDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var userRepo = _userRepository;

        // 1. ตรวจสอบว่า Email หรือ Username ซ้ำไหม
        var existingUsers = await _userRepository.FindAsync(u =>
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

        var targetRoleId = request.RoleId; // ใช้ตัวที่ส่งมาจากหน้าบ้านเป็นหลัก

        // ถ้าหน้าบ้านไม่ได้ส่ง RoleId มา (เป็น Guid.Empty) ให้หา Default Role
        if (targetRoleId == Guid.Empty)
        {
            var defaultRoles = await _roleRepository.FindAsync(r => r.Name == "User", cancellationToken);
            var defaultRole = defaultRoles.FirstOrDefault();
            if (defaultRole != null) targetRoleId = defaultRole.Id;
        }

        if (targetRoleId != Guid.Empty)
        {
            var userRole = new UserRole
            {
                User = newUser,
                RoleId = targetRoleId
            };
            await _userRoleRepository.AddAsync(userRole, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var resultUser = await userRepo.GetQueryable()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == newUser.Id, cancellationToken);

        return MapToUserDto(resultUser ?? newUser);
    }
    public async Task<Guid> CreateUserAsync(CreateUserDto dto, CancellationToken ct)
    {
        // 1. ตรวจสอบข้อมูลซ้ำ
        var exists = await _userRepository.AnyAsync(u =>
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

        await _userRepository.AddAsync(newUser, ct);

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
            await _userRoleRepository.AddAsync(userRole, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return newUser.Id;
    }
    public async Task<bool> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);

        if (user == null) return false;

        user.Email = dto.Email;
        user.JobTitle = dto.JobTitle;
        user.Department = dto.Department;
        user.IsActive = dto.IsActive;
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;


        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }
    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(id, ct);
        if (user == null) return false;

        _userRepository.Remove(user);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _userRepository.ExistsByUsernameAsync(username, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _userRepository.ExistsByEmailAsync(email, cancellationToken);
    }

    // --- Role Operations ---

    public async Task<RoleDto?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        return role == null ? null : MapToRoleDto(role);
    }

    public async Task<RoleDto?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByNameAsync(name, cancellationToken);
        return role == null ? null : MapToRoleDto(role);
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        return roles.Select(MapToRoleDto);
    }

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
        await _roleRepository.AddAsync(role, ct);
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
        var role = await _roleRepository.GetByIdAsync(id, ct);
        if (role == null) return false;

        role.Name = dto.Name;
        role.Description = dto.Description;

        _roleRepository.Update(role);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteRoleAsync(Guid id, CancellationToken ct)
    {
        var role = await _roleRepository.GetByIdAsync(id, ct);
        if (role == null) return false;

        _roleRepository.Remove(role); // หรือ .Delete()
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ExistsByRoleNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _roleRepository.ExistsByNameAsync(name, cancellationToken);
    }

    // --- UserRole Operations ---

    public async Task<UserRoleDto?> GetUserRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userRole = await _userRoleRepository.GetByIdAsync(id, cancellationToken);
        return userRole == null ? null : MapToUserRoleDto(userRole);
    }

    public async Task<IEnumerable<UserRoleDto>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _userRoleRepository.GetUserRolesAsync(userId, cancellationToken);
        return userRoles.Select(MapToUserRoleDto);
    }

    public async Task<IEnumerable<UserRoleDto>> GetUserRolesByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _userRoleRepository.GetRoleUsersAsync(roleId, cancellationToken);
        return userRoles.Select(MapToUserRoleDto);
    }

    public async Task<IEnumerable<UserRoleDto>> GetAllUserRolesAsync(CancellationToken cancellationToken = default)
    {
        var userRoles = await _userRoleRepository.GetAllAsync(cancellationToken);
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
        await _userRoleRepository.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateUserRoleAsync(UserRoleDto dto, CancellationToken cancellationToken = default)
    {
        var userRole = await _userRoleRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (userRole != null)
        {
            userRole.UserId = dto.UserId;
            userRole.RoleId = dto.RoleId;
            userRole.LastModifiedAt = DateTime.UtcNow;
            userRole.LastModifiedBy = dto.UpdatedBy ?? "System";

            _userRoleRepository.Update(userRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteUserRoleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userRole = await _userRoleRepository.GetByIdAsync(id, cancellationToken);
        if (userRole != null)
        {
            _userRoleRepository.Remove(userRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        await _userRoleRepository.AssignRoleToUserAsync(userId, roleId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        await _userRoleRepository.RemoveRoleFromUserAsync(userId, roleId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _userRoleRepository.UserHasRoleAsync(userId, roleId, cancellationToken);
    }

    // --- Private Helper Mappers ---

    private static UserDto MapToUserDto(User u) => new()
    {
        Id = u.Id,
        Username = u.Username,
        Email = u.Email,
        FirstName = u.FirstName,
        LastName = u.LastName,
        FullName = u.FullName,
        JobTitle = u.JobTitle,
        Department = u.Department,
        IsActive = u.IsActive,
        LastLoginAt = u.LastLoginAt,
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