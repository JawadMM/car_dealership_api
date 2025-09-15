using CarDealershipApi.Models;
using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Services;

public interface ICarService
{
    Task<IEnumerable<CarDto>> GetAllCarsAsync();
    Task<CarDto?> GetCarByIdAsync(int id);
    Task<CarDto> CreateCarAsync(CreateCarDto createCarDto);
    Task<CarDto?> UpdateCarAsync(int id, UpdateCarDto updateCarDto);
    Task<bool> DeleteCarAsync(int id);
    Task<IEnumerable<CarDto>> GetAvailableCarsAsync();
    Task<IEnumerable<CarDto>> SearchCarsAsync(string? make, string? model, int? minYear, int? maxYear, decimal? maxPrice);
}

