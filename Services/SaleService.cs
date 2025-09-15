using Microsoft.EntityFrameworkCore;
using CarDealershipApi.Data;
using CarDealershipApi.Models;
using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Services;

public class SaleService : ISaleService
{
    private readonly CarDealershipDbContext _context;

    public SaleService(CarDealershipDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SaleDto>> GetAllSalesAsync()
    {
        var sales = await _context.Sales
            .Include(s => s.Car)
            .Include(s => s.Customer)
            .Include(s => s.Employee)
            .ToListAsync();
        return sales.Select(MapToDto);
    }

    public async Task<SaleDto?> GetSaleByIdAsync(int id)
    {
        var sale = await _context.Sales
            .Include(s => s.Car)
            .Include(s => s.Customer)
            .Include(s => s.Employee)
            .FirstOrDefaultAsync(s => s.Id == id);
        return sale != null ? MapToDto(sale) : null;
    }

    public async Task<SaleDto> CreateSaleAsync(CreateSaleDto createSaleDto)
    {
        // Check if car is available
        var car = await _context.Cars.FindAsync(createSaleDto.CarId);
        if (car == null || !car.IsAvailable)
        {
            throw new InvalidOperationException("Car is not available for sale");
        }

        // Check if customer exists
        var customer = await _context.Customers.FindAsync(createSaleDto.CustomerId);
        if (customer == null)
        {
            throw new InvalidOperationException("Customer not found");
        }

        // Check if employee exists and is active
        var employee = await _context.Employees.FindAsync(createSaleDto.EmployeeId);
        if (employee == null || !employee.IsActive)
        {
            throw new InvalidOperationException("Employee not found or inactive");
        }

        var sale = new Sale
        {
            CarId = createSaleDto.CarId,
            CustomerId = createSaleDto.CustomerId.ToString(),
            EmployeeId = createSaleDto.EmployeeId,
            SalePrice = createSaleDto.SalePrice,
            SaleDate = DateTime.UtcNow,
            PaymentMethod = createSaleDto.PaymentMethod,
            Notes = createSaleDto.Notes
        };

        _context.Sales.Add(sale);

        // Mark car as sold
        car.IsAvailable = false;
        car.DateSold = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Reload with navigation properties
        var createdSale = await _context.Sales
            .Include(s => s.Car)
            .Include(s => s.Customer)
            .Include(s => s.Employee)
            .FirstAsync(s => s.Id == sale.Id);

        return MapToDto(createdSale);
    }

    public async Task<SaleDto?> UpdateSaleAsync(int id, UpdateSaleDto updateSaleDto)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null) return null;

        sale.SalePrice = updateSaleDto.SalePrice;
        sale.PaymentMethod = updateSaleDto.PaymentMethod;
        sale.Notes = updateSaleDto.Notes;

        await _context.SaveChangesAsync();

        // Reload with navigation properties
        var updatedSale = await _context.Sales
            .Include(s => s.Car)
            .Include(s => s.Customer)
            .Include(s => s.Employee)
            .FirstAsync(s => s.Id == sale.Id);

        return MapToDto(updatedSale);
    }

    public async Task<bool> DeleteSaleAsync(int id)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null) return false;

        // Mark car as available again
        var car = await _context.Cars.FindAsync(sale.CarId);
        if (car != null)
        {
            car.IsAvailable = true;
            car.DateSold = null;
        }

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var sales = await _context.Sales
            .Include(s => s.Car)
            .Include(s => s.Customer)
            .Include(s => s.Employee)
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .ToListAsync();
        return sales.Select(MapToDto);
    }

    public async Task<IEnumerable<SaleDto>> GetSalesByEmployeeAsync(int employeeId)
    {
        var sales = await _context.Sales
            .Include(s => s.Car)
            .Include(s => s.Customer)
            .Include(s => s.Employee)
            .Where(s => s.EmployeeId == employeeId)
            .ToListAsync();
        return sales.Select(MapToDto);
    }

    public async Task<IEnumerable<SaleDto>> GetSalesByCustomerAsync(int customerId)
    {
        var sales = await _context.Sales
            .Include(s => s.Car)
            .Include(s => s.Customer)
            .Include(s => s.Employee)
            .Where(s => s.CustomerId == customerId.ToString())
            .ToListAsync();
        return sales.Select(MapToDto);
    }

    private static SaleDto MapToDto(Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            CarId = sale.CarId,
            CustomerId = int.Parse(sale.CustomerId),
            EmployeeId = sale.EmployeeId,
            SalePrice = sale.SalePrice,
            SaleDate = sale.SaleDate,
            PaymentMethod = sale.PaymentMethod,
            Notes = sale.Notes,
            Car = sale.Car != null ? new CarDto
            {
                Id = sale.Car.Id,
                Make = sale.Car.Make,
                Model = sale.Car.Model,
                Year = sale.Car.Year,
                Color = sale.Car.Color,
                VIN = sale.Car.VIN,
                Price = sale.Car.Price,
                Mileage = sale.Car.Mileage,
                Transmission = sale.Car.Transmission,
                FuelType = sale.Car.FuelType,
                IsAvailable = sale.Car.IsAvailable,
                DateAdded = sale.Car.DateAdded,
                DateSold = sale.Car.DateSold
            } : null,
            Customer = sale.Customer != null ? new CustomerDto
            {
                Id = int.Parse(sale.Customer.Id),
                FirstName = sale.Customer.FirstName,
                LastName = sale.Customer.LastName,
                Email = sale.Customer.Email!,
                PhoneNumber = sale.Customer.PhoneNumber,
                Address = sale.Customer.Address,
                City = sale.Customer.City,
                State = sale.Customer.State,
                ZipCode = sale.Customer.ZipCode,
                DateCreated = sale.Customer.DateCreated
            } : null,
            Employee = sale.Employee != null ? new EmployeeDto
            {
                Id = sale.Employee.Id,
                FirstName = sale.Employee.FirstName,
                LastName = sale.Employee.LastName,
                Email = sale.Employee.Email,
                PhoneNumber = sale.Employee.PhoneNumber,
                Position = sale.Employee.Position,
                Salary = sale.Employee.Salary,
                HireDate = sale.Employee.HireDate,
                TerminationDate = sale.Employee.TerminationDate,
                IsActive = sale.Employee.IsActive
            } : null
        };
    }
}
