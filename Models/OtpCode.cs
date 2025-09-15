using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDealershipApi.Models;

public class OtpCode
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(6)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Purpose { get; set; } = string.Empty; // Login, Register, PurchaseRequest, UpdateVehicle
    
    [StringLength(500)]
    public string? Metadata { get; set; } // JSON string for additional data like carId, etc.
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public DateTime ExpiresAt { get; set; }
    
    public bool IsUsed { get; set; } = false;
    
    public DateTime? UsedAt { get; set; }
    
    [Required]
    public int Attempts { get; set; } = 0;
    
    [Required]
    public int MaxAttempts { get; set; } = 3;
}

