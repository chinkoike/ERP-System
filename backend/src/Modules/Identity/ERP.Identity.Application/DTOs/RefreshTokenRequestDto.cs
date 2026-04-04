
namespace ERP.Identity.Application.DTOs;

public class RefreshTokenRequestDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string ExpiredToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

