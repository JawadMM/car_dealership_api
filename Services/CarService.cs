using Microsoft.EntityFrameworkCore;
using CarDealershipApi.Data;
using CarDealershipApi.Models;
using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Services;

public class CarService : ICarService
{
    private readonly CarDealershipDbContext _context;

    public CarService(CarDealershipDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CarDto>> GetAllCarsAsync()
    {
        var cars = await _context.Cars.ToListAsync();
        return cars.Select(MapToDto);
    }

    public async Task<CarDto?> GetCarByIdAsync(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        return car != null ? MapToDto(car) : null;
    }

    public async Task<CarDto> CreateCarAsync(CreateCarDto createCarDto)
    {
        var car = new Car
        {
            Make = createCarDto.Make,
            Model = createCarDto.Model,
            Year = createCarDto.Year,
            Color = createCarDto.Color,
            VIN = createCarDto.VIN,
            Price = createCarDto.Price,
            Mileage = createCarDto.Mileage,
            Transmission = createCarDto.Transmission,
            FuelType = createCarDto.FuelType,
            IsAvailable = true,
            DateAdded = DateTime.UtcNow
        };

        _context.Cars.Add(car);
        await _context.SaveChangesAsync();

        return MapToDto(car);
    }

    public async Task<CarDto?> UpdateCarAsync(int id, UpdateCarDto updateCarDto)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return null;

        car.Make = updateCarDto.Make;
        car.Model = updateCarDto.Model;
        car.Year = updateCarDto.Year;
        car.Color = updateCarDto.Color;
        car.VIN = updateCarDto.VIN;
        car.Price = updateCarDto.Price;
        car.Mileage = updateCarDto.Mileage;
        car.Transmission = updateCarDto.Transmission;
        car.FuelType = updateCarDto.FuelType;
        car.IsAvailable = updateCarDto.IsAvailable;

        await _context.SaveChangesAsync();
        return MapToDto(car);
    }

    public async Task<bool> DeleteCarAsync(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return false;

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CarDto>> GetAvailableCarsAsync()
    {
        var cars = await _context.Cars.Where(c => c.IsAvailable).ToListAsync();
        return cars.Select(MapToDto);
    }

    public async Task<IEnumerable<CarDto>> SearchCarsAsync(string? make, string? model, int? minYear, int? maxYear, decimal? maxPrice)
    {
        var query = _context.Cars.AsQueryable();

        if (!string.IsNullOrEmpty(make))
            query = query.Where(c => c.Make.Contains(make));

        if (!string.IsNullOrEmpty(model))
            query = query.Where(c => c.Model.Contains(model));

        if (minYear.HasValue)
            query = query.Where(c => c.Year >= minYear.Value);

        if (maxYear.HasValue)
            query = query.Where(c => c.Year <= maxYear.Value);

        if (maxPrice.HasValue)
            query = query.Where(c => c.Price <= maxPrice.Value);

        var cars = await query.ToListAsync();
        return cars.Select(MapToDto);
    }

    private static CarDto MapToDto(Car car)
    {
        return new CarDto
        {
            Id = car.Id,
            Make = car.Make,
            Model = car.Model,
            Year = car.Year,
            Color = car.Color,
            VIN = car.VIN,
            Price = car.Price,
            Mileage = car.Mileage,
            Transmission = car.Transmission,
            FuelType = car.FuelType,
            IsAvailable = car.IsAvailable,
            DateAdded = car.DateAdded,
            DateSold = car.DateSold
        };
    }
}

