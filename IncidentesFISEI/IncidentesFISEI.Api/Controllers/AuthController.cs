using Microsoft.AspNetCore.Mvc;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Application.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace IncidentesFISEI.Api.Controllers;

/// <summary>
/// Controlador para autenticación y gestión de usuarios
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Iniciar sesión en el sistema
    /// </summary>
    /// <param name="loginDto">Credenciales de acceso</param>
    /// <returns>Token JWT y datos del usuario</returns>
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Iniciar sesión", Description = "Autentica un usuario y devuelve un token JWT")]
    [SwaggerResponse(200, "Inicio de sesión exitoso", typeof(AuthResponseDto))]
    [SwaggerResponse(401, "Credenciales inválidas")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            _logger.LogInformation("Usuario autenticado: {Email}", loginDto.Email);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Intento de login fallido: {Email}", loginDto.Email);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Registrar un nuevo usuario
    /// </summary>
    /// <param name="registerDto">Datos del nuevo usuario</param>
    /// <returns>Confirmación de registro</returns>
    [HttpPost("register")]
    [SwaggerOperation(Summary = "Registrar usuario", Description = "Crea un nuevo usuario en el sistema")]
    [SwaggerResponse(200, "Usuario registrado exitosamente")]
    [SwaggerResponse(400, "Datos inválidos o usuario ya existe")]
    public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var result = await _authService.RegisterAsync(registerDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            _logger.LogInformation("Nuevo usuario registrado: {Email}", registerDto.Email);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en registro");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Cambiar contraseña
    /// </summary>
    /// <param name="changePasswordDto">Datos para cambio de contraseña</param>
    /// <returns>Confirmación de cambio</returns>
    [HttpPost("change-password")]
    [SwaggerOperation(Summary = "Cambiar contraseña", Description = "Permite a un usuario cambiar su contraseña")]
    [SwaggerResponse(200, "Contraseña cambiada exitosamente")]
    [SwaggerResponse(400, "Contraseña actual incorrecta")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var result = await _authService.ChangePasswordAsync(changePasswordDto);
            
            if (!result)
            {
                return BadRequest(new { message = "Contraseña actual incorrecta" });
            }

            _logger.LogInformation("Contraseña cambiada para usuario {UserId}", changePasswordDto.UsuarioId);
            return Ok(new { message = "Contraseña cambiada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar contraseña");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Validar token JWT
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>Validez del token</returns>
    [HttpPost("validate-token")]
    [SwaggerOperation(Summary = "Validar token", Description = "Verifica si un token JWT es válido")]
    [SwaggerResponse(200, "Token válido")]
    [SwaggerResponse(401, "Token inválido o expirado")]
    public IActionResult ValidateToken([FromBody] string token)
    {
        try
        {
            var isValid = _authService.ValidateToken(token);
            
            if (!isValid)
            {
                return Unauthorized(new { message = "Token inválido o expirado" });
            }

            return Ok(new { message = "Token válido", isValid = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar token");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
