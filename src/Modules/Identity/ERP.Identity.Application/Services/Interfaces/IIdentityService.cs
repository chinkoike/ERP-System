using ERP.Identity.Application.DTOs;
using ERP.Identity.Domain; // ยังต้องใช้ User ใน RegisterAsync (หรือจะเปลี่ยนเป็น UserDto ก็ได้)

namespace ERP.Identity.Application.Services.Interfaces;

public interface IIdentityService
{
    // User Operations
    Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task CreateUserAsync(UserDto userDto, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(UserDto userDto, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    // Role Operations
    Task<RoleDto?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RoleDto?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<RoleDto> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateRoleAsync(Guid id, UpdateRoleDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByRoleNameAsync(string name, CancellationToken cancellationToken = default);

    // Specialized Logic
    Task<UserDto> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken = default);

    // UserRole Operations
    Task<UserRoleDto?> GetUserRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRoleDto>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRoleDto>> GetUserRolesByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRoleDto>> GetAllUserRolesAsync(CancellationToken cancellationToken = default);
    Task CreateUserRoleAsync(UserRoleDto userRoleDto, CancellationToken cancellationToken = default);
    Task UpdateUserRoleAsync(UserRoleDto userRoleDto, CancellationToken cancellationToken = default);
    Task DeleteUserRoleAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest, CancellationToken cancellationToken = default);
}