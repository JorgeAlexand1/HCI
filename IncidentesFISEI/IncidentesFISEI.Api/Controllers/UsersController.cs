using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Text.Json;

namespace IncidentesFISEI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Administrador")] // Temporalmente deshabilitado para testing
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Crear nuevo usuario
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UsuarioListDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest createData)
    {
        try
        {
            // Verificar si el email ya existe
            var existingUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == createData.Email);
            if (existingUser != null)
            {
                return BadRequest(new ApiResponse<object>(false, null, "El email ya está registrado"));
            }

            // Crear nuevo usuario
            var usuario = new Usuario
            {
                Username = GenerateUsernameFromEmail(createData.Email),
                Email = createData.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("TempPass123!"),
                FirstName = createData.FirstName,
                LastName = createData.LastName,
                Phone = createData.Phone,
                Department = createData.Department,
                TipoUsuario = createData.TipoUsuario,
                IsActive = createData.IsActive,
                IsEmailConfirmed = createData.IsEmailConfirmed,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var usuarioDto = MapToUsuarioListDto(usuario);
            
            _logger.LogInformation("Usuario {Email} creado exitosamente por administrador", usuario.Email);
            
            return CreatedAtAction(nameof(GetUser), new { id = usuario.Id }, 
                new ApiResponse<UsuarioListDto>(true, usuarioDto, "Usuario creado exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioListDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            
            if (usuario == null)
            {
                return NotFound(new ApiResponse<object>(false, null, "Usuario no encontrado"));
            }

            var usuarioDto = MapToUsuarioListDto(usuario);
            return Ok(new ApiResponse<UsuarioListDto>(true, usuarioDto, "Usuario obtenido exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {Id}", id);
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }

    /// <summary>
    /// Obtener usuarios asignables (técnicos y supervisores activos)
    /// </summary>
    [HttpGet("assignable")]
    [ProducesResponseType(typeof(List<UsuarioListDto>), 200)]
    public async Task<IActionResult> GetAssignableUsers()
    {
        try
        {
            var users = await _context.Usuarios
                .Where(u => u.IsActive && (u.TipoUsuario == TipoUsuario.Tecnico || u.TipoUsuario == TipoUsuario.Supervisor))
                .Select(u => new UsuarioListDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Department = u.Department,
                    TipoUsuario = u.TipoUsuario,
                    IsActive = u.IsActive,
                    IsEmailConfirmed = u.IsEmailConfirmed,
                    CreatedAt = u.CreatedAt,
                    Username = u.Username
                })
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios asignables");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtener todos los usuarios (temporal para debugging)
    /// </summary>
    [HttpGet("all")]
    [ProducesResponseType(typeof(List<UsuarioListDto>), 200)]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _context.Usuarios
                .Select(u => new UsuarioListDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Department = u.Department,
                    TipoUsuario = u.TipoUsuario,
                    IsActive = u.IsActive,
                    IsEmailConfirmed = u.IsEmailConfirmed,
                    CreatedAt = u.CreatedAt,
                    Username = u.Username
                })
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los usuarios");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualizar usuario
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioListDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest updateDto)
    {
        try
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            
            if (usuario == null)
            {
                return NotFound(new ApiResponse<object>(false, null, "Usuario no encontrado"));
            }

            // Verificar si el nuevo email ya existe en otro usuario
            if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != usuario.Email)
            {
                var emailExists = await _context.Usuarios.AnyAsync(u => u.Email == updateDto.Email && u.Id != id);
                if (emailExists)
                {
                    return BadRequest(new ApiResponse<object>(false, null, "El email ya está en uso por otro usuario"));
                }
                usuario.Email = updateDto.Email;
            }

            // Actualizar campos
            if (!string.IsNullOrEmpty(updateDto.FirstName)) usuario.FirstName = updateDto.FirstName;
            if (!string.IsNullOrEmpty(updateDto.LastName)) usuario.LastName = updateDto.LastName;
            if (!string.IsNullOrEmpty(updateDto.Phone)) usuario.Phone = updateDto.Phone;
            if (!string.IsNullOrEmpty(updateDto.Department)) usuario.Department = updateDto.Department;
            if (updateDto.TipoUsuario.HasValue) usuario.TipoUsuario = updateDto.TipoUsuario.Value;
            if (updateDto.IsActive.HasValue) usuario.IsActive = updateDto.IsActive.Value;
            if (updateDto.IsEmailConfirmed.HasValue) usuario.IsEmailConfirmed = updateDto.IsEmailConfirmed.Value;
            
            // Cambiar contraseña si se proporciona
            if (!string.IsNullOrEmpty(updateDto.NewPassword))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.NewPassword);
            }

            usuario.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var usuarioDto = MapToUsuarioListDto(usuario);
            
            _logger.LogInformation("Usuario {Id} actualizado exitosamente", id);
            
            return Ok(new ApiResponse<UsuarioListDto>(true, usuarioDto, "Usuario actualizado exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario {Id}", id);
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }

    /// <summary>
    /// Activar/Desactivar usuario
    /// </summary>
    [HttpPatch("{id}/toggle-status")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> ToggleUserStatus(int id)
    {
        try
        {
            _logger.LogInformation("Recibida solicitud para cambiar estado del usuario ID: {Id}", id);
            
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            
            if (usuario == null)
            {
                _logger.LogWarning("Usuario con ID {Id} no encontrado", id);
                return NotFound(new ApiResponse<object>(false, null, "Usuario no encontrado"));
            }

            var estadoAnterior = usuario.IsActive;
            usuario.IsActive = !usuario.IsActive;
            usuario.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Cambiando estado del usuario ID {Id}: {EstadoAnterior} -> {EstadoNuevo}", 
                id, estadoAnterior, usuario.IsActive);

            await _context.SaveChangesAsync();

            var status = usuario.IsActive ? "activado" : "desactivado";
            _logger.LogInformation("Usuario {Id} {Status} exitosamente", id, status);
            
            return Ok(new ApiResponse<object>(true, null, $"Usuario {status} exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del usuario {Id}", id);
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }

    /// <summary>
    /// Aprobar usuario pendiente
    /// </summary>
    [HttpPatch("{id}/approve")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> ApproveUser(int id)
    {
        try
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            
            if (usuario == null)
            {
                return NotFound(new ApiResponse<object>(false, null, "Usuario no encontrado"));
            }

            usuario.IsEmailConfirmed = true;
            usuario.IsActive = true;
            usuario.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario {Id} aprobado exitosamente", id);
            
            return Ok(new ApiResponse<object>(true, null, "Usuario aprobado exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aprobar usuario {Id}", id);
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }

    /// <summary>
    /// Rechazar usuario pendiente
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            
            if (usuario == null)
            {
                return NotFound(new ApiResponse<object>(false, null, "Usuario no encontrado"));
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario {Id} eliminado exitosamente", id);
            
            return Ok(new ApiResponse<object>(true, null, "Usuario eliminado exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario {Id}", id);
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }

    // Métodos helper
    private static UsuarioListDto MapToUsuarioListDto(Usuario usuario)
    {
        return new UsuarioListDto
        {
            Id = usuario.Id,
            Username = usuario.Username,
            Email = usuario.Email,
            FirstName = usuario.FirstName,
            LastName = usuario.LastName,
            TipoUsuario = usuario.TipoUsuario,
            IsActive = usuario.IsActive,
            IsEmailConfirmed = usuario.IsEmailConfirmed,
            LastLoginAt = usuario.LastLoginAt
        };
    }

    private static string GenerateUsernameFromEmail(string email)
    {
        return email.Split('@')[0].ToLower();
    }
}

// DTOs temporales para evitar conflictos
public class CreateUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public TipoUsuario TipoUsuario { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsEmailConfirmed { get; set; } = false;
}

public class UpdateUserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public TipoUsuario? TipoUsuario { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsEmailConfirmed { get; set; }
    public string? NewPassword { get; set; }
}