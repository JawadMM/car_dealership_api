using Microsoft.AspNetCore.Mvc;
using CarDealershipApi.Services;
using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OtpController : ControllerBase
{
    private readonly IOtpService _otpService;
    private readonly ILogger<OtpController> _logger;

    public OtpController(IOtpService otpService, ILogger<OtpController> logger)
    {
        _otpService = otpService;
        _logger = logger;
    }

    /// <summary>
    /// Generate and send an OTP code
    /// </summary>
    /// <param name="request">OTP request details</param>
    /// <returns>OTP generation result</returns>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(OtpResponseDto), 200)]
    [ProducesResponseType(typeof(OtpResponseDto), 400)]
    public async Task<ActionResult<OtpResponseDto>> GenerateOtp([FromBody] OtpRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new OtpResponseDto
                {
                    Success = false,
                    Message = "Invalid request data."
                });
            }

            var result = await _otpService.GenerateOtpAsync(request);
            
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
            _logger.LogError(ex, "Error generating OTP for {Email}", request.Email);
            return StatusCode(500, new OtpResponseDto
            {
                Success = false,
                Message = "Internal server error. Please try again."
            });
        }
    }

    /// <summary>
    /// Verify an OTP code
    /// </summary>
    /// <param name="request">OTP verification details</param>
    /// <returns>OTP verification result</returns>
    [HttpPost("verify")]
    [ProducesResponseType(typeof(OtpVerificationResult), 200)]
    [ProducesResponseType(typeof(OtpVerificationResult), 400)]
    public async Task<ActionResult<OtpVerificationResult>> VerifyOtp([FromBody] OtpVerifyDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new OtpVerificationResult
                {
                    IsValid = false,
                    Message = "Invalid request data."
                });
            }

            var result = await _otpService.VerifyOtpAsync(request);
            
            if (result.IsValid)
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
            _logger.LogError(ex, "Error verifying OTP for {Email}", request.Email);
            return StatusCode(500, new OtpVerificationResult
            {
                IsValid = false,
                Message = "Internal server error. Please try again."
            });
        }
    }

    /// <summary>
    /// Check if an OTP is valid for a given email and purpose
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="purpose">OTP purpose</param>
    /// <returns>OTP validity status</returns>
    [HttpGet("validate")]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<ActionResult<bool>> ValidateOtp([FromQuery] string email, [FromQuery] string purpose)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(purpose))
            {
                return BadRequest(false);
            }

            var isValid = await _otpService.IsOtpValidAsync(email, purpose);
            return Ok(isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating OTP for {Email}", email);
            return StatusCode(500, false);
        }
    }
}

