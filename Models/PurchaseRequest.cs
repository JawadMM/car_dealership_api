using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDealershipApi.Models;

public class PurchaseRequest
{
    public int Id { get; set; }
    
    [Required]
    public int CarId { get; set; }
    
    [Required]
    public string CustomerId { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal RequestedPrice { get; set; }
    
    [StringLength(1000)]
    public string? Message { get; set; }
    
    [Required]
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed
    
    [StringLength(1000)]
    public string? AdminNotes { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(CarId))]
    public Car Car { get; set; } = null!;
    
    [ForeignKey(nameof(CustomerId))]
    public User Customer { get; set; } = null!;
}

