using ERP.Identity.Application.DTOs;

namespace ERP.Identity.Application.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(UserDto user, List<string> roles);
}