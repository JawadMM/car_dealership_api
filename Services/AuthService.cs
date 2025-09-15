using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarDealershipApi.Models;
using CarDealershipApi.Models.DTOs;

namespace CarDealershipApi.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var user = new User
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            PhoneNumber = registerDto.PhoneNumber,
            Address = registerDto.Address,
            City = registerDto.City,
            State = registerDto.State,
            ZipCode = registerDto.ZipCode,
            DateCreated = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await _userManager.AddToRoleAsync(user, registerDto.Role);

        var token = await GenerateJwtTokenAsync(user);
        var userDto = await MapToUserDtoAsync(user);

        return new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddDays(7),
            User = userDto
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var token = await GenerateJwtTokenAsync(user);
        var userDto = await MapToUserDtoAsync(user);

        return new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddDays(7),
            User = userDto
        };
    }

    public async Task<bool> ValidateCredentialsAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return false;
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        return result.Succeeded;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null ? await MapToUserDtoAsync(user) : null;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            userDtos.Add(await MapToUserDtoAsync(user));
        }

        return userDtos;
    }

    public async Task<bool> UpdateUserAsync(string userId, UserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.PhoneNumber = userDto.PhoneNumber;
        user.Address = userDto.Address;
        user.City = userDto.City;
        user.State = userDto.State;
        user.ZipCode = userDto.ZipCode;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
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
