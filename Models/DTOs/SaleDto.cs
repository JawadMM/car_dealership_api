namespace CarDealershipApi.Models.DTOs;

public class SaleDto
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public decimal SalePrice { get; set; }
    public DateTime SaleDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties for detailed view
    public CarDto? Car { get; set; }
    public CustomerDto? Customer { get; set; }
    public EmployeeDto? Employee { get; set; }
}

public class CreateSaleDto
{
    public int CarId { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public decimal SalePrice { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
}

public class UpdateSaleDto
{
    public decimal SalePrice { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
}

