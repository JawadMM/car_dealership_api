using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarDealershipApi.Services;
using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CustomersController : ControllerBase
{
    private readonly IAuthService _authService;

    public CustomersController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Get all registered customers
    /// </summary>
    /// <returns>List of customers</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetCustomers()
    {
        var users = await _authService.GetAllUsersAsync();
        // Filter to only show customers
        var customers = users.Where(u => u.Roles.Contains("Customer"));
        return Ok(customers);
    }

    /// <summary>
    /// Get a specific customer by ID
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Customer details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetCustomer(string id)
    {
        var user = await _authService.GetUserByIdAsync(id);
        if (user == null || !user.Roles.Contains("Customer"))
        {
            return NotFound();
        }
        return Ok(user);
    }

    /// <summary>
    /// Search customers by name or email
    /// </summary>
    /// <param name="name">Name to search for</param>
    /// <param name="email">Email to search for</param>
    /// <returns>Filtered list of customers</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserDto>>> SearchCustomers(
        [FromQuery] string? name,
        [FromQuery] string? email)
    {
        var users = await _authService.GetAllUsersAsync();
        var customers = users.Where(u => u.Roles.Contains("Customer"));

        if (!string.IsNullOrEmpty(name))
        {
            customers = customers.Where(c => 
                c.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                c.LastName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(email))
        {
            customers = customers.Where(c => 
                c.Email.Contains(email, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(customers);
    }

    /// <summary>
    /// Update customer information
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="updateUserDto">Updated customer data</param>
    /// <returns>Update result</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(string id, UserDto updateUserDto)
    {
        try
        {
            var result = await _authService.UpdateUserAsync(id, updateUserDto);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a customer account
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Delete result</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(string id)
    {
        var result = await _authService.DeleteUserAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}