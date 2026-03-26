using ERP.Shared;
using ERP.Identity.Application.Services.Interfaces;
using ERP.Identity.Domain;
using ERP.Identity.Application.DTOs;

namespace ERP.Identity.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly IUnitOfWork _unitOfWork;

    public IdentityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

    public async Task<UserDto> RegisterAsync(string username, string email, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<User>();
        var existingUsers = await repo.FindAsync(u => u.Email == email, cancellationToken);
        var existingUser = existingUsers.FirstOrDefault();

        if (existingUser != null) return MapToUserDto(existingUser);

        var newUser = new User
        {
            Username = username,
            Email = email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await repo.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToUserDto(newUser);
    }

    public async Task CreateUserAsync(UserDto dto, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = dto.CreatedBy ?? "System"
        };

        await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateUserAsync(UserDto dto, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<User>();
        var user = await repo.GetByIdAsync(dto.Id, cancellationToken);
        if (user != null)
        {
            user.Username = dto.Username;
            user.Email = dto.Email;
            user.IsActive = dto.IsActive;
            user.LastModifiedAt = DateTime.UtcNow;
            user.LastModifiedBy = dto.UpdatedBy ?? "System";

            repo.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<User>();
        var user = await repo.GetByIdAsync(id, cancellationToken);
        if (user != null)
        {
            repo.Remove(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
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

    public async Task CreateRoleAsync(RoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = new Role
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = dto.CreatedBy ?? "System"
        };
        await _unitOfWork.Repository<Role>().AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRoleAsync(RoleDto dto, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<Role>();
        var role = await repo.GetByIdAsync(dto.Id, cancellationToken);
        if (role != null)
        {
            role.Name = dto.Name;
            role.Description = dto.Description;
            role.LastModifiedAt = DateTime.UtcNow;
            role.LastModifiedBy = dto.UpdatedBy ?? "System";

            repo.Update(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<Role>();
        var role = await repo.GetByIdAsync(id, cancellationToken);
        if (role != null)
        {
            repo.Remove(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
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