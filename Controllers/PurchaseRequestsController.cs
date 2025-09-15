using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CarDealershipApi.Services;
using CarDealershipApi.Models.DTOs;
using System.Text.Json;

namespace CarDealershipApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseRequestsController : ControllerBase
{
    private readonly IPurchaseRequestService _purchaseRequestService;
    private readonly IOtpService _otpService;
    private readonly ILogger<PurchaseRequestsController> _logger;

    public PurchaseRequestsController(
        IPurchaseRequestService purchaseRequestService,
        IOtpService otpService,
        ILogger<PurchaseRequestsController> logger)
    {
        _purchaseRequestService = purchaseRequestService;
        _otpService = otpService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<PurchaseRequestDto>>> GetAllPurchaseRequests()
    {
        var requests = await _purchaseRequestService.GetAllPurchaseRequestsAsync();
        return Ok(requests);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<PurchaseRequestDto>>> GetPendingPurchaseRequests()
    {
        var requests = await _purchaseRequestService.GetPendingPurchaseRequestsAsync();
        return Ok(requests);
    }

    [HttpGet("my-requests")]
    public async Task<ActionResult<IEnumerable<PurchaseRequestDto>>> GetMyPurchaseRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var requests = await _purchaseRequestService.GetPurchaseRequestsByCustomerAsync(userId);
        return Ok(requests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseRequestDto>> GetPurchaseRequest(int id)
    {
        var request = await _purchaseRequestService.GetPurchaseRequestByIdAsync(id);
        if (request == null)
        {
            return NotFound();
        }

        // Check if user can access this request
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");
        
        if (!isAdmin && request.CustomerId != userId)
        {
            return Forbid();
        }

        return Ok(request);
    }

    /// <summary>
    /// Request OTP for creating a purchase request
    /// </summary>
    /// <param name="createDto">Purchase request data</param>
    /// <returns>OTP generation result</returns>
    [HttpPost("request-otp")]
    public async Task<ActionResult<OtpResponseDto>> RequestPurchaseOtp([FromBody] CreatePurchaseRequestDto createDto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("User email not found.");
            }

            // Serialize purchase request data for OTP metadata
            var metadata = JsonSerializer.Serialize(new { createDto, userId });
            
            var otpRequest = new OtpRequestDto
            {
                Email = userEmail,
                Purpose = "PurchaseRequest",
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
            _logger.LogError(ex, "Error requesting purchase OTP");
            return StatusCode(500, new OtpResponseDto
            {
                Success = false,
                Message = "Internal server error. Please try again."
            });
        }
    }

    /// <summary>
    /// Create a purchase request with OTP verification
    /// </summary>
    /// <param name="verifyDto">OTP verification data</param>
    /// <returns>Created purchase request</returns>
    [HttpPost("verify-otp")]
    public async Task<ActionResult<PurchaseRequestDto>> CreatePurchaseRequestWithOtp([FromBody] OtpVerifyDto verifyDto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _otpService.VerifyOtpAsync(verifyDto);
            
            if (!result.IsValid)
            {
                return BadRequest(result.Message);
            }

            // Parse the metadata to get the purchase request data
            if (string.IsNullOrEmpty(result.Metadata))
            {
                return BadRequest("Invalid purchase request data.");
            }

            var metadata = JsonSerializer.Deserialize<JsonElement>(result.Metadata);
            var createDto = JsonSerializer.Deserialize<CreatePurchaseRequestDto>(metadata.GetProperty("createDto").GetRawText());
            
            if (createDto == null)
            {
                return BadRequest("Invalid purchase request data format.");
            }

            var request = await _purchaseRequestService.CreatePurchaseRequestAsync(createDto, userId);
            return CreatedAtAction(nameof(GetPurchaseRequest), new { id = request.Id }, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase request with OTP");
            return StatusCode(500, "Internal server error. Please try again.");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePurchaseRequest(int id, UpdatePurchaseRequestDto updateDto)
    {
        try
        {
            var request = await _purchaseRequestService.UpdatePurchaseRequestAsync(id, updateDto);
            if (request == null)
            {
                return NotFound();
            }
            return Ok(request);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePurchaseRequest(int id)
    {
        var result = await _purchaseRequestService.DeletePurchaseRequestAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}
