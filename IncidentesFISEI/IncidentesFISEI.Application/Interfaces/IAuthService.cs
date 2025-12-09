using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Entities;

namespace IncidentesFISEI.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto);
    Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    Task<bool> CheckEmailExists(string email);
    bool ValidateToken(string token);
    Task<Usuario?> GetUserByEmailAsync(string email);
    Task SavePasswordResetTokenAsync(int usuarioId, string token, DateTime expirationTime);
    Task<Usuario?> ValidatePasswordResetTokenAsync(string token);
    Task<bool> ResetPasswordAsync(int usuarioId, string newPassword);
    Task InvalidatePasswordResetTokenAsync(string token);
}