using IncidentesFISEI.Application.DTOs;

namespace IncidentesFISEI.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto);
    Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    Task<bool> CheckEmailExists(string email);
    bool ValidateToken(string token);
}