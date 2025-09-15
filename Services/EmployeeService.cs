using Microsoft.EntityFrameworkCore;
using CarDealershipApi.Data;
using CarDealershipApi.Models;
using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Services;

public class EmployeeService : IEmployeeService
{
    private readonly CarDealershipDbContext _context;

    public EmployeeService(CarDealershipDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _context.Employees.ToListAsync();
        return employees.Select(MapToDto);
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        return employee != null ? MapToDto(employee) : null;
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto)
    {
        var employee = new Employee
        {
            FirstName = createEmployeeDto.FirstName,
            LastName = createEmployeeDto.LastName,
            Email = createEmployeeDto.Email,
            PhoneNumber = createEmployeeDto.PhoneNumber,
            Position = createEmployeeDto.Position,
            Salary = createEmployeeDto.Salary,
            HireDate = DateTime.UtcNow,
            IsActive = true
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return MapToDto(employee);
    }

    public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateEmployeeDto)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return null;

        employee.FirstName = updateEmployeeDto.FirstName;
        employee.LastName = updateEmployeeDto.LastName;
        employee.Email = updateEmployeeDto.Email;
        employee.PhoneNumber = updateEmployeeDto.PhoneNumber;
        employee.Position = updateEmployeeDto.Position;
        employee.Salary = updateEmployeeDto.Salary;
        employee.IsActive = updateEmployeeDto.IsActive;

        if (!updateEmployeeDto.IsActive && employee.TerminationDate == null)
        {
            employee.TerminationDate = DateTime.UtcNow;
        }
        else if (updateEmployeeDto.IsActive)
        {
            employee.TerminationDate = null;
        }

        await _context.SaveChangesAsync();
        return MapToDto(employee);
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<EmployeeDto>> GetActiveEmployeesAsync()
    {
        var employees = await _context.Employees.Where(e => e.IsActive).ToListAsync();
        return employees.Select(MapToDto);
    }

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            Position = employee.Position,
            Salary = employee.Salary,
            HireDate = employee.HireDate,
            TerminationDate = employee.TerminationDate,
            IsActive = employee.IsActive
        };
    }
}

