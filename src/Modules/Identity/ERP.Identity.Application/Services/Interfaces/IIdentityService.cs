using ERP.Identity.Domain;

namespace ERP.Identity.Application.Services.Interfaces;

public interface IIdentityService
{
    Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Role?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task CreateRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task UpdateRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task DeleteRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task<bool> ExistsByRoleNameAsync(string name, CancellationToken cancellationToken = default);

    Task<UserRole?> GetUserRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRole>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRole>> GetUserRolesByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRole>> GetAllUserRolesAsync(CancellationToken cancellationToken = default);
    Task CreateUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default);
    Task UpdateUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default);
    Task DeleteUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}