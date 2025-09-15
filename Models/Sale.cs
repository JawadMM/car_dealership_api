using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDealershipApi.Models;

public class Sale
{
    public int Id { get; set; }
    
    [Required]
    public int CarId { get; set; }
    
    [Required]
    public string CustomerId { get; set; } = string.Empty;
    
    [Required]
    public int EmployeeId { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SalePrice { get; set; }
    
    [Required]
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    
    [StringLength(50)]
    public string? PaymentMethod { get; set; }
    
    [StringLength(500)]
    public string? Notes { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(CarId))]
    public Car Car { get; set; } = null!;
    
    [ForeignKey(nameof(CustomerId))]
    public User Customer { get; set; } = null!;
    
    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;
}
