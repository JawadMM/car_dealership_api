namespace CarDealershipApi.Models.DTOs;

public class PurchaseRequestDto
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public decimal RequestedPrice { get; set; }
    public string? Message { get; set; }
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed
    public string? AdminNotes { get; set; }
    
    // Navigation properties
    public CarDto? Car { get; set; }
    public UserDto? Customer { get; set; }
}

public class CreatePurchaseRequestDto
{
    public int CarId { get; set; }
    public decimal RequestedPrice { get; set; }
    public string? Message { get; set; }
}

public class UpdatePurchaseRequestDto
{
    public string Status { get; set; } = string.Empty;
    public string? AdminNotes { get; set; }
}

