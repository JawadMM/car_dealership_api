using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<bool> ValidateCredentialsAsync(LoginDto loginDto);
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(string userId, UserDto userDto);
    Task<bool> DeleteUserAsync(string userId);
}
