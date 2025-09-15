using System.ComponentModel.DataAnnotations;

namespace CarDealershipApi.Models;

public class Car
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Make { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;
    
    [Required]
    public int Year { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Color { get; set; } = string.Empty;
    
    [Required]
    [StringLength(17)]
    public string VIN { get; set; } = string.Empty;
    
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    public int Mileage { get; set; }
    
    [StringLength(50)]
    public string? Transmission { get; set; }
    
    [StringLength(50)]
    public string? FuelType { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    
    public DateTime? DateSold { get; set; }
    
    // Navigation properties
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}

