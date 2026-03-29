namespace ERP.Sales.Application.DTOs.Requests;

public class CreateOrderWithUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // เพิ่มฟิลด์นี้
    public string ProductSku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}