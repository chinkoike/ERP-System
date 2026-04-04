using System.ComponentModel.DataAnnotations;
namespace ERP.Finance.Application.DTOs;

public class CreateAccountDto
{
    [Required]
    [StringLength(100)]
    public string AccountName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string AccountCode { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Balance { get; set; }
}
