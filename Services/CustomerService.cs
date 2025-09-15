using Microsoft.EntityFrameworkCore;
using CarDealershipApi.Data;
using CarDealershipApi.Models;
using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Services;

public class CustomerService : ICustomerService
{
    private readonly CarDealershipDbContext _context;

    public CustomerService(CarDealershipDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _context.Customers.ToListAsync();
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        return customer != null ? MapToDto(customer) : null;
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
    {
        var customer = new Customer
        {
            FirstName = createCustomerDto.FirstName,
            LastName = createCustomerDto.LastName,
            Email = createCustomerDto.Email,
            PhoneNumber = createCustomerDto.PhoneNumber,
            Address = createCustomerDto.Address,
            City = createCustomerDto.City,
            State = createCustomerDto.State,
            ZipCode = createCustomerDto.ZipCode,
            DateCreated = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return MapToDto(customer);
    }

    public async Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return null;

        customer.FirstName = updateCustomerDto.FirstName;
        customer.LastName = updateCustomerDto.LastName;
        customer.Email = updateCustomerDto.Email;
        customer.PhoneNumber = updateCustomerDto.PhoneNumber;
        customer.Address = updateCustomerDto.Address;
        customer.City = updateCustomerDto.City;
        customer.State = updateCustomerDto.State;
        customer.ZipCode = updateCustomerDto.ZipCode;

        await _context.SaveChangesAsync();
        return MapToDto(customer);
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string? name, string? email)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(c => c.FirstName.Contains(name) || c.LastName.Contains(name));
        }

        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(c => c.Email.Contains(email));
        }

        var customers = await query.ToListAsync();
        return customers.Select(MapToDto);
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            City = customer.City,
            State = customer.State,
            ZipCode = customer.ZipCode,
            DateCreated = customer.DateCreated
        };
    }
}

