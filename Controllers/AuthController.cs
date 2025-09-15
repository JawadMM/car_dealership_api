using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarDealershipApi.Services;
using CarDealershipApi.Models.DTOs;
using System.Text.Json;

namespace CarDealershipApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IOtpService _otpService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IOtpService otpService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _otpService = otpService;
        _logger = logger;
    }

    /// <summary>
    /// Request OTP for user registration
    /// </summary>
    /// <param name="registerDto">User registration data</param>
    /// <returns>OTP generation result</returns>
    [HttpPost("register/request-otp")]
    public async Task<ActionResult<OtpResponseDto>> RequestRegisterOtp([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new OtpResponseDto
                {
                    Success = false,
                    Message = "Invalid registration data."
                });
            }

            // Serialize registration data for OTP metadata
            var metadata = JsonSerializer.Serialize(registerDto);
            
            var otpRequest = new OtpRequestDto
            {
                Email = registerDto.Email,
                Purpose = "Register",
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
            _logger.LogError(ex, "Error requesting registration OTP for {Email}", registerDto.Email);
            return StatusCode(500, new OtpResponseDto
            {
                Success = false,
                Message = "Internal server error. Please try again."
            });
        }
    }

    /// <summary>
    /// Complete user registration with OTP verification
    /// </summary>
    /// <param name="verifyDto">OTP verification data</param>
    /// <returns>Registration result with auth token</returns>
    [HttpPost("register/verify-otp")]
    public async Task<ActionResult<AuthResponseDto>> VerifyRegisterOtp([FromBody] OtpVerifyDto verifyDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid verification data.");
            }

            var result = await _otpService.VerifyOtpAsync(verifyDto);
            
            if (result.IsValid && !string.IsNullOrEmpty(result.Token) && result.User != null)
            {
                var authResponse = new AuthResponseDto
                {
                    Token = result.Token,
                    Expiration = DateTime.UtcNow.AddDays(7),
                    User = result.User
                };
                return Ok(authResponse);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying registration OTP for {Email}", verifyDto.Email);
            return StatusCode(500, "Internal server error. Please try again.");
        }
    }

    /// <summary>
    /// Request OTP for user login
    /// </summary>
    /// <param name="loginDto">User login data</param>
    /// <returns>OTP generation result</returns>
    [HttpPost("login/request-otp")]
    public async Task<ActionResult<OtpResponseDto>> RequestLoginOtp([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new OtpResponseDto
                {
                    Success = false,
                    Message = "Invalid login data."
                });
            }

            // Validate credentials before issuing OTP
            var isValid = await _authService.ValidateCredentialsAsync(loginDto);
            if (!isValid)
            {
                return Unauthorized(new OtpResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                });
            }

            var otpRequest = new OtpRequestDto
            {
                Email = loginDto.Email,
                Purpose = "Login"
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
            _logger.LogError(ex, "Error requesting login OTP for {Email}", loginDto.Email);
            return StatusCode(500, new OtpResponseDto
            {
                Success = false,
                Message = "Internal server error. Please try again."
            });
        }
    }

    /// <summary>
    /// Complete user login with OTP verification
    /// </summary>
    /// <param name="verifyDto">OTP verification data</param>
    /// <returns>Login result with auth token</returns>
    [HttpPost("login/verify-otp")]
    public async Task<ActionResult<AuthResponseDto>> VerifyLoginOtp([FromBody] OtpVerifyDto verifyDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid verification data.");
            }

            var result = await _otpService.VerifyOtpAsync(verifyDto);
            
            if (result.IsValid && !string.IsNullOrEmpty(result.Token) && result.User != null)
            {
                var authResponse = new AuthResponseDto
                {
                    Token = result.Token,
                    Expiration = DateTime.UtcNow.AddDays(7),
                    User = result.User
                };
                return Ok(authResponse);
            }
            else
            {
                return Unauthorized(result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying login OTP for {Email}", verifyDto.Email);
            return StatusCode(500, "Internal server error. Please try again.");
        }
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("users/{userId}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUser(string userId)
    {
        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPut("users/{userId}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(string userId, UserDto userDto)
    {
        try
        {
            var result = await _authService.UpdateUserAsync(userId, userDto);
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

    [HttpDelete("users/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var result = await _authService.DeleteUserAsync(userId);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}
