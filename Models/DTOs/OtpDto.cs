using System.ComponentModel.DataAnnotations;

namespace CarDealershipApi.Models.DTOs;

public class OtpRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Purpose { get; set; } = string.Empty; // Login, Register, PurchaseRequest, UpdateVehicle
    
    [StringLength(500)]
    public string? Metadata { get; set; } // JSON string for additional data
}

public class OtpVerifyDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Purpose { get; set; } = string.Empty;
}

public class OtpResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int AttemptsRemaining { get; set; }
}

public class OtpVerificationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; } // For login/register
    public UserDto? User { get; set; } // For login/register
    public string? Metadata { get; set; } // For other purposes
}

