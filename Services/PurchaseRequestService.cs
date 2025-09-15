using Microsoft.EntityFrameworkCore;
using CarDealershipApi.Data;
using CarDealershipApi.Models;
using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Services;

public class PurchaseRequestService : IPurchaseRequestService
{
    private readonly CarDealershipDbContext _context;

    public PurchaseRequestService(CarDealershipDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PurchaseRequestDto>> GetAllPurchaseRequestsAsync()
    {
        var requests = await _context.PurchaseRequests
            .Include(pr => pr.Car)
            .Include(pr => pr.Customer)
            .ToListAsync();
        return requests.Select(MapToDto);
    }

    public async Task<IEnumerable<PurchaseRequestDto>> GetPurchaseRequestsByCustomerAsync(string customerId)
    {
        var requests = await _context.PurchaseRequests
            .Include(pr => pr.Car)
            .Include(pr => pr.Customer)
            .Where(pr => pr.CustomerId == customerId)
            .ToListAsync();
        return requests.Select(MapToDto);
    }

    public async Task<PurchaseRequestDto?> GetPurchaseRequestByIdAsync(int id)
    {
        var request = await _context.PurchaseRequests
            .Include(pr => pr.Car)
            .Include(pr => pr.Customer)
            .FirstOrDefaultAsync(pr => pr.Id == id);
        return request != null ? MapToDto(request) : null;
    }

    public async Task<PurchaseRequestDto> CreatePurchaseRequestAsync(CreatePurchaseRequestDto createDto, string customerId)
    {
        // Check if car exists and is available
        var car = await _context.Cars.FindAsync(createDto.CarId);
        if (car == null || !car.IsAvailable)
        {
            throw new InvalidOperationException("Car is not available for purchase");
        }

        // Check if customer exists
        var customer = await _context.Users.FindAsync(customerId);
        if (customer == null)
        {
            throw new InvalidOperationException("Customer not found");
        }

        var request = new PurchaseRequest
        {
            CarId = createDto.CarId,
            CustomerId = customerId,
            RequestedPrice = createDto.RequestedPrice,
            Message = createDto.Message,
            RequestDate = DateTime.UtcNow,
            Status = "Pending"
        };

        _context.PurchaseRequests.Add(request);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        var createdRequest = await _context.PurchaseRequests
            .Include(pr => pr.Car)
            .Include(pr => pr.Customer)
            .FirstAsync(pr => pr.Id == request.Id);

        return MapToDto(createdRequest);
    }

    public async Task<PurchaseRequestDto?> UpdatePurchaseRequestAsync(int id, UpdatePurchaseRequestDto updateDto)
    {
        var request = await _context.PurchaseRequests
            .Include(pr => pr.Car)
            .FirstOrDefaultAsync(pr => pr.Id == id);
        if (request == null) return null;

        request.Status = updateDto.Status;
        request.AdminNotes = updateDto.AdminNotes;
        
        // If approved, mark the car as not available and set DateSold
        if (updateDto.Status == "Approved" && request.Car != null)
        {
            request.Car.IsAvailable = false;
            request.Car.DateSold = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        // Reload with navigation properties
        var updatedRequest = await _context.PurchaseRequests
            .Include(pr => pr.Car)
            .Include(pr => pr.Customer)
            .FirstAsync(pr => pr.Id == request.Id);

        return MapToDto(updatedRequest);
    }

    public async Task<bool> DeletePurchaseRequestAsync(int id)
    {
        var request = await _context.PurchaseRequests.FindAsync(id);
        if (request == null) return false;

        _context.PurchaseRequests.Remove(request);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PurchaseRequestDto>> GetPendingPurchaseRequestsAsync()
    {
        var requests = await _context.PurchaseRequests
            .Include(pr => pr.Car)
            .Include(pr => pr.Customer)
            .Where(pr => pr.Status == "Pending")
            .ToListAsync();
        return requests.Select(MapToDto);
    }

    private static PurchaseRequestDto MapToDto(PurchaseRequest request)
    {
        return new PurchaseRequestDto
        {
            Id = request.Id,
            CarId = request.CarId,
            CustomerId = request.CustomerId,
            RequestedPrice = request.RequestedPrice,
            Message = request.Message,
            RequestDate = request.RequestDate,
            Status = request.Status,
            AdminNotes = request.AdminNotes,
            Car = request.Car != null ? new CarDto
            {
                Id = request.Car.Id,
                Make = request.Car.Make,
                Model = request.Car.Model,
                Year = request.Car.Year,
                Color = request.Car.Color,
                VIN = request.Car.VIN,
                Price = request.Car.Price,
                Mileage = request.Car.Mileage,
                Transmission = request.Car.Transmission,
                FuelType = request.Car.FuelType,
                IsAvailable = request.Car.IsAvailable,
                DateAdded = request.Car.DateAdded,
                DateSold = request.Car.DateSold
            } : null,
            Customer = request.Customer != null ? new UserDto
            {
                Id = request.Customer.Id,
                FirstName = request.Customer.FirstName,
                LastName = request.Customer.LastName,
                Email = request.Customer.Email!,
                PhoneNumber = request.Customer.PhoneNumber,
                Address = request.Customer.Address,
                City = request.Customer.City,
                State = request.Customer.State,
                ZipCode = request.Customer.ZipCode,
                DateCreated = request.Customer.DateCreated,
                Roles = new List<string>() // Would need to fetch roles separately
            } : null
        };
    }
}
