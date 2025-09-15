using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Services;

public interface IOtpService
{
    Task<OtpResponseDto> GenerateOtpAsync(OtpRequestDto request);
    Task<OtpVerificationResult> VerifyOtpAsync(OtpVerifyDto request);
    Task<bool> CleanupExpiredOtpsAsync();
    Task<bool> IsOtpValidAsync(string email, string purpose);
}

