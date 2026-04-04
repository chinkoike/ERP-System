using ERP.Identity.Application.DTOs;
using System.Security.Claims;
namespace ERP.Identity.Application.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(UserDto user, List<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}


