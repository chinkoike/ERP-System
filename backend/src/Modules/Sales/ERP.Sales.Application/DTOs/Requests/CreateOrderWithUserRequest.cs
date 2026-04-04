namespace ERP.Sales.Application.DTOs.Requests;

public class CreateOrderWithUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public int Quantity { get; set; }
}