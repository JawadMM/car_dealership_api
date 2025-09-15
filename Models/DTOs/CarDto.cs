namespace CarDealershipApi.Models.DTOs;

public class CarDto
{
    public int Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public string VIN { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Mileage { get; set; }
    public string? Transmission { get; set; }
    public string? FuelType { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DateSold { get; set; }
}

public class CreateCarDto
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public string VIN { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Mileage { get; set; }
    public string? Transmission { get; set; }
    public string? FuelType { get; set; }
}

public class UpdateCarDto
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public string VIN { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Mileage { get; set; }
    public string? Transmission { get; set; }
    public string? FuelType { get; set; }
    public bool IsAvailable { get; set; }
}

