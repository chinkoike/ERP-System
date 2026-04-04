namespace ERP.Report.Application.DTOs;

public class InventoryStatusDto
{
    public Guid ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int ReorderLevel { get; set; }
    public string StockStatus { get; set; } = string.Empty;
}
