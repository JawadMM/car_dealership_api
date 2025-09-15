using System.ComponentModel.DataAnnotations;

namespace CarDealershipApi.Models;

public class Customer
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    
    [StringLength(500)]
    public string? Address { get; set; }
    
    [StringLength(100)]
    public string? City { get; set; }
    
    [StringLength(50)]
    public string? State { get; set; }
    
    [StringLength(20)]
    public string? ZipCode { get; set; }
    
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}

