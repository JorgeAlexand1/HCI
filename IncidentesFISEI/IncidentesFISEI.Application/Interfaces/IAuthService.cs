using IncidentesFISEI.Application.DTOs;

namespace IncidentesFISEI.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto);
    Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    bool ValidateToken(string token);
}