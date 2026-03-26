using ERP.Shared;
using ERP.Identity.Application.Services.Interfaces;
using ERP.Identity.Domain;

namespace ERP.Identity.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly IUnitOfWork _unitOfWork;

    public IdentityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // User operations
    public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Repository<User>();
        return await userRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Repository<User>();
        var users = await userRepository.FindAsync(u => u.Username == username, cancellationToken);
        return users.FirstOrDefault();
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Repository<User>();
        var users = await userRepository.FindAsync(u => u.Email == email, cancellationToken);
        return users.FirstOrDefault();
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Repository<User>();
        return await userRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Repository<User>();
        return await userRepository.FindAsync(u => u.IsActive, cancellationToken);
    }
    public async Task<User> RegisterAsync(string username, string email, CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Repository<User>();

        // เช็คก่อนว่ามี Email นี้หรือยัง เพื่อป้องกัน Data Duplicate
        var existingUsers = await userRepository.FindAsync(u => u.Email == email, cancellationToken);
        var existingUser = existingUsers.FirstOrDefault();
        if (existingUser != null) return existingUser;

        var newUser = new User
        {
            Username = username,
            Email = email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await userRepository.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken); // บันทึกลง Identity Database

        return newUser;
    }
    public async Task CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Repository<User>();
        await userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Repository<User>();
        userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Repository<User>();
        userRepository.Remove(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Repository<User>();
        return await userRepository.ExistsAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Repository<User>();
        return await userRepository.ExistsAsync(u => u.Email == email, cancellationToken);
    }

    // Role operations
    public async Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var roleRepository = _unitOfWork.Repository<Role>();
        return await roleRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Role?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var roleRepository = _unitOfWork.Repository<Role>();
        var roles = await roleRepository.FindAsync(r => r.Name == name, cancellationToken);
        return roles.FirstOrDefault();
    }

    public async Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roleRepository = _unitOfWork.Repository<Role>();
        return await roleRepository.GetAllAsync(cancellationToken);
    }

    public async Task CreateRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        var roleRepository = _unitOfWork.Repository<Role>();
        await roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        var roleRepository = _unitOfWork.Repository<Role>();
        roleRepository.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        var roleRepository = _unitOfWork.Repository<Role>();
        roleRepository.Remove(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByRoleNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var roleRepository = _unitOfWork.Repository<Role>();
        return await roleRepository.ExistsAsync(r => r.Name == name, cancellationToken);
    }

    // UserRole operations
    public async Task<UserRole?> GetUserRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userRoleRepository = _unitOfWork.Repository<UserRole>();
        return await userRoleRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRoleRepository = _unitOfWork.Repository<UserRole>();
        return await userRoleRepository.FindAsync(ur => ur.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRoleRepository = _unitOfWork.Repository<UserRole>();
        return await userRoleRepository.FindAsync(ur => ur.RoleId == roleId, cancellationToken);
    }

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRoleRepository = _unitOfWork.Repository<UserRole>();
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };
        await userRoleRepository.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRoleRepository = _unitOfWork.Repository<UserRole>();
        var userRoles = await userRoleRepository.FindAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
        var userRole = userRoles.FirstOrDefault();
        if (userRole != null)
        {
            userRoleRepository.Remove(userRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<UserRole>> GetAllUserRolesAsync(CancellationToken cancellationToken = default)
    {
        var userRoleRepository = _unitOfWork.Repository<UserRole>();
        return await userRoleRepository.GetAllAsync(cancellationToken);
    }

    public async Task CreateUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        var userRoleRepository = _unitOfWork.Repository<UserRole>();
        await userRoleRepository.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        var userRoleRepository = _unitOfWork.Repository<UserRole>();
        userRoleRepository.Update(userRole);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        var userRoleRepository = _unitOfWork.Repository<UserRole>();
        userRoleRepository.Remove(userRole);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRoleRepository = _unitOfWork.Repository<UserRole>();
        return await userRoleRepository.ExistsAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }
}