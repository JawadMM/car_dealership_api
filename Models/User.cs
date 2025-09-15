using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CarDealershipApi.Models;

public class User : IdentityUser
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
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

