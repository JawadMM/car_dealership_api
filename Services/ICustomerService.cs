using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto?> GetCustomerByIdAsync(int id);
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
    Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto);
    Task<bool> DeleteCustomerAsync(int id);
    Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string? name, string? email);
}

