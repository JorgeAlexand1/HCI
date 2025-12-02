using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> GetAllUsuariosAsync();
    Task<UsuarioDto> GetUsuarioByIdAsync(int id);
    Task<UsuarioDto> GetUsuarioByEmailAsync(string email);
    Task<IEnumerable<UsuarioDto>> GetUsuariosByRolAsync(RolUsuario rol);
    Task<UsuarioDto> CreateUsuarioAsync(CreateUsuarioDto createUsuarioDto);
    Task<UsuarioDto> UpdateUsuarioAsync(int id, UpdateUsuarioDto updateUsuarioDto);
    Task<bool> DeleteUsuarioAsync(int id);
    Task<IEnumerable<UsuarioDto>> GetUsuariosActivosAsync();
    Task<bool> UpdatePasswordAsync(int userId, string newPassword);
    Task<bool> UpdateRolAsync(int userId, RolUsuario nuevoRol);
}