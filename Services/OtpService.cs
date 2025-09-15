using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarDealershipApi.Data;
using CarDealershipApi.Models;
using CarDealershipApi.Models.DTOs;
using System.Text.Json;

namespace CarDealershipApi.Services;

public class OtpService : IOtpService
{
    private readonly CarDealershipDbContext _context;
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OtpService> _logger;

    public OtpService(
        CarDealershipDbContext context,
        IAuthService authService,
        UserManager<User> userManager,
        IConfiguration configuration,
        ILogger<OtpService> logger)
    {
        _context = context;
        _authService = authService;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<OtpResponseDto> GenerateOtpAsync(OtpRequestDto request)
    {
        try
        {
            // Clean up expired OTPs first
            await CleanupExpiredOtpsAsync();

            // Check if there's already a valid OTP for this email and purpose
            var existingOtp = await _context.OtpCodes
                .FirstOrDefaultAsync(o => o.Email == request.Email 
                    && o.Purpose == request.Purpose 
                    && !o.IsUsed 
                    && o.ExpiresAt > DateTime.UtcNow);

            if (existingOtp != null)
            {
                return new OtpResponseDto
                {
                    Success = false,
                    Message = "An OTP has already been sent. Please wait before requesting another.",
                    ExpiresAt = existingOtp.ExpiresAt,
                    AttemptsRemaining = existingOtp.MaxAttempts - existingOtp.Attempts
                };
            }

            // Generate 6-digit OTP
            var otpCode = GenerateRandomOtp();
            var expiresAt = DateTime.UtcNow.AddMinutes(5); // 5 minutes expiration

            var otp = new OtpCode
            {
                Code = otpCode,
                Email = request.Email,
                Purpose = request.Purpose,
                Metadata = request.Metadata,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                MaxAttempts = 3
            };

            _context.OtpCodes.Add(otp);
            await _context.SaveChangesAsync();

            // Simulate OTP delivery (console output)
            _logger.LogInformation($"OTP for {request.Email} ({request.Purpose}): {otpCode}");
            Console.WriteLine($"=== OTP DELIVERY SIMULATION ===");
            Console.WriteLine($"To: {request.Email}");
            Console.WriteLine($"Purpose: {request.Purpose}");
            Console.WriteLine($"OTP Code: {otpCode}");
            Console.WriteLine($"Expires at: {expiresAt:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"===============================");

            return new OtpResponseDto
            {
                Success = true,
                Message = "OTP sent successfully. Check your console for the code.",
                ExpiresAt = expiresAt,
                AttemptsRemaining = 3
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating OTP for {Email}", request.Email);
            return new OtpResponseDto
            {
                Success = false,
                Message = "Failed to generate OTP. Please try again."
            };
        }
    }

    public async Task<OtpVerificationResult> VerifyOtpAsync(OtpVerifyDto request)
    {
        try
        {
            var otp = await _context.OtpCodes
                .FirstOrDefaultAsync(o => o.Email == request.Email 
                    && o.Purpose == request.Purpose 
                    && !o.IsUsed);

            if (otp == null)
            {
                return new OtpVerificationResult
                {
                    IsValid = false,
                    Message = "Invalid or expired OTP."
                };
            }

            // Check if OTP has expired
            if (otp.ExpiresAt <= DateTime.UtcNow)
            {
                otp.IsUsed = true;
                otp.UsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new OtpVerificationResult
                {
                    IsValid = false,
                    Message = "OTP has expired. Please request a new one."
                };
            }

            // Check if max attempts exceeded
            if (otp.Attempts >= otp.MaxAttempts)
            {
                otp.IsUsed = true;
                otp.UsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new OtpVerificationResult
                {
                    IsValid = false,
                    Message = "Maximum attempts exceeded. Please request a new OTP."
                };
            }

            // Increment attempts
            otp.Attempts++;

            if (otp.Code != request.Code)
            {
                await _context.SaveChangesAsync();
                return new OtpVerificationResult
                {
                    IsValid = false,
                    Message = $"Invalid OTP. {otp.MaxAttempts - otp.Attempts} attempts remaining."
                };
            }

            // OTP is valid - mark as used
            otp.IsUsed = true;
            otp.UsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Handle different purposes
            return await HandleOtpPurpose(otp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying OTP for {Email}", request.Email);
            return new OtpVerificationResult
            {
                IsValid = false,
                Message = "Error verifying OTP. Please try again."
            };
        }
    }

    public async Task<bool> CleanupExpiredOtpsAsync()
    {
        try
        {
            var expiredOtps = await _context.OtpCodes
                .Where(o => o.ExpiresAt <= DateTime.UtcNow || o.IsUsed)
                .ToListAsync();

            if (expiredOtps.Any())
            {
                _context.OtpCodes.RemoveRange(expiredOtps);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleaned up {Count} expired OTPs", expiredOtps.Count);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired OTPs");
            return false;
        }
    }

    public async Task<bool> IsOtpValidAsync(string email, string purpose)
    {
        var otp = await _context.OtpCodes
            .FirstOrDefaultAsync(o => o.Email == email 
                && o.Purpose == purpose 
                && !o.IsUsed 
                && o.ExpiresAt > DateTime.UtcNow);

        return otp != null;
    }

    private string GenerateRandomOtp()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    private async Task<OtpVerificationResult> HandleOtpPurpose(OtpCode otp)
    {
        switch (otp.Purpose)
        {
            case "Login":
                return await HandleLoginOtp(otp);
            case "Register":
                return await HandleRegisterOtp(otp);
            case "PurchaseRequest":
            case "UpdateVehicle":
                return new OtpVerificationResult
                {
                    IsValid = true,
                    Message = "OTP verified successfully.",
                    Metadata = otp.Metadata
                };
            default:
                return new OtpVerificationResult
                {
                    IsValid = true,
                    Message = "OTP verified successfully."
                };
        }
    }

    private async Task<OtpVerificationResult> HandleLoginOtp(OtpCode otp)
    {
        try
        {
            // For login, we need to find the user and generate a token
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == otp.Email);
            if (user == null)
            {
                return new OtpVerificationResult
                {
                    IsValid = false,
                    Message = "User not found."
                };
            }

            // Generate JWT token (simplified - in real implementation, use proper JWT service)
            var token = await GenerateJwtTokenAsync(user);
            var userDto = await MapToUserDtoAsync(user);

            return new OtpVerificationResult
            {
                IsValid = true,
                Message = "Login successful.",
                Token = token,
                User = userDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling login OTP for {Email}", otp.Email);
            return new OtpVerificationResult
            {
                IsValid = false,
                Message = "Error processing login. Please try again."
            };
        }
    }

    private async Task<OtpVerificationResult> HandleRegisterOtp(OtpCode otp)
    {
        try
        {
            // For register, we need to parse the metadata to get user data
            if (string.IsNullOrEmpty(otp.Metadata))
            {
                return new OtpVerificationResult
                {
                    IsValid = false,
                    Message = "Invalid registration data."
                };
            }

            var registerData = JsonSerializer.Deserialize<RegisterDto>(otp.Metadata);
            if (registerData == null)
            {
                return new OtpVerificationResult
                {
                    IsValid = false,
                    Message = "Invalid registration data format."
                };
            }

            // Create the user
            var authResponse = await _authService.RegisterAsync(registerData);

            return new OtpVerificationResult
            {
                IsValid = true,
                Message = "Registration successful.",
                Token = authResponse.Token,
                User = authResponse.User
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling register OTP for {Email}", otp.Email);
            return new OtpVerificationResult
            {
                IsValid = false,
                Message = "Error processing registration. Please try again."
            };
        }
    }

    private async Task<string> GenerateJwtTokenAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<UserDto> MapToUserDtoAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            City = user.City,
            State = user.State,
            ZipCode = user.ZipCode,
            DateCreated = user.DateCreated,
            Roles = roles
        };
    }
}
