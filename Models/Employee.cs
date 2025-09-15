using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarDealershipApi.Models;

public class Employee
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
    
    [Required]
    [StringLength(100)]
    public string Position { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Salary { get; set; }
    
    [Required]
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? TerminationDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
