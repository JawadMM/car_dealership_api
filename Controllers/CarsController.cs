using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarDealershipApi.Services;
using CarDealershipApi.Models.DTOs;
using System.Security.Claims;
using System.Text.Json;

namespace CarDealershipApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly IOtpService _otpService;
    private readonly ILogger<CarsController> _logger;

    public CarsController(ICarService carService, IOtpService otpService, ILogger<CarsController> logger)
    {
        _carService = carService;
        _otpService = otpService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarDto>>> GetCars()
    {
        var cars = await _carService.GetAllCarsAsync();
        return Ok(cars);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CarDto>> GetCar(int id)
    {
        var car = await _carService.GetCarByIdAsync(id);
        if (car == null)
        {
            return NotFound();
        }
        return Ok(car);
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<CarDto>>> GetAvailableCars()
    {
        var cars = await _carService.GetAvailableCarsAsync();
        return Ok(cars);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<CarDto>>> SearchCars(
        [FromQuery] string? make,
        [FromQuery] string? model,
        [FromQuery] int? minYear,
        [FromQuery] int? maxYear,
        [FromQuery] decimal? maxPrice)
    {
        var cars = await _carService.SearchCarsAsync(make, model, minYear, maxYear, maxPrice);
        return Ok(cars);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CarDto>> CreateCar(CreateCarDto createCarDto)
    {
        try
        {
            var car = await _carService.CreateCarAsync(createCarDto);
            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Request OTP for updating a vehicle
    /// </summary>
    /// <param name="id">Vehicle ID</param>
    /// <param name="updateCarDto">Vehicle update data</param>
    /// <returns>OTP generation result</returns>
    [HttpPost("{id}/update/request-otp")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OtpResponseDto>> RequestUpdateVehicleOtp(int id, [FromBody] UpdateCarDto updateCarDto)
    {
        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("User email not found.");
            }

            // Serialize update data for OTP metadata
            var metadata = JsonSerializer.Serialize(new { id, updateCarDto });
            
            var otpRequest = new OtpRequestDto
            {
                Email = userEmail,
                Purpose = "UpdateVehicle",
                Metadata = metadata
            };

            var result = await _otpService.GenerateOtpAsync(otpRequest);
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting update vehicle OTP for car {Id}", id);
            return StatusCode(500, new OtpResponseDto
            {
                Success = false,
                Message = "Internal server error. Please try again."
            });
        }
    }

    /// <summary>
    /// Update a vehicle with OTP verification
    /// </summary>
    /// <param name="verifyDto">OTP verification data</param>
    /// <returns>Updated vehicle</returns>
    [HttpPut("update/verify-otp")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CarDto>> UpdateCarWithOtp([FromBody] OtpVerifyDto verifyDto)
    {
        try
        {
            var result = await _otpService.VerifyOtpAsync(verifyDto);
            
            if (!result.IsValid)
            {
                return BadRequest(result.Message);
            }

            // Parse the metadata to get the update data
            if (string.IsNullOrEmpty(result.Metadata))
            {
                return BadRequest("Invalid update data.");
            }

            var metadata = JsonSerializer.Deserialize<JsonElement>(result.Metadata);
            var id = metadata.GetProperty("id").GetInt32();
            var updateCarDto = JsonSerializer.Deserialize<UpdateCarDto>(metadata.GetProperty("updateCarDto").GetRawText());
            
            if (updateCarDto == null)
            {
                return BadRequest("Invalid update data format.");
            }

            var car = await _carService.UpdateCarAsync(id, updateCarDto);
            if (car == null)
            {
                return NotFound();
            }
            return Ok(car);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vehicle with OTP");
            return StatusCode(500, "Internal server error. Please try again.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCar(int id)
    {
        var result = await _carService.DeleteCarAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}
