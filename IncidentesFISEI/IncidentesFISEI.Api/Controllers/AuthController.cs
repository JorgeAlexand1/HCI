using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentesFISEI.Api.Controllers;

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
    /// <param name="loginDto">Credenciales de login</param>
    /// <returns>Token JWT y datos del usuario</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage)).ToArray();
                return BadRequest(new ApiResponse<object>(false, null, "Datos de entrada inválidos", errors));
            }

            var result = await _authService.LoginAsync(loginDto);
            
            _logger.LogInformation("Usuario {Email} inició sesión exitosamente", loginDto.Email);
            
            return Ok(new ApiResponse<AuthResponseDto>(true, result, "Login exitoso"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Intento de login fallido para {Email}: {Message}", loginDto.Email, ex.Message);
            return Unauthorized(new ApiResponse<object>(false, null, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el login para {Email}", loginDto.Email);
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }

    /// <summary>
    /// Registrar nuevo usuario (auto-registro)
    /// </summary>
    /// <param name="registerDto">Datos de registro</param>
    /// <returns>Resultado del registro</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage)).ToArray();
                return BadRequest(new ApiResponse<object>(false, null, "Datos de entrada inválidos", errors));
            }

            var result = await _authService.RegisterAsync(registerDto);
            
            if (result.Success)
            {
                _logger.LogInformation("Nuevo usuario registrado: {Email}", registerDto.Email);
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el registro de {Email}", registerDto.Email);
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }

    /// <summary>
    /// Cambiar contraseña del usuario autenticado
    /// </summary>
    /// <param name="changePasswordDto">Datos para cambio de contraseña</param>
    /// <returns>Resultado del cambio</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage)).ToArray();
                return BadRequest(new ApiResponse<object>(false, null, "Datos de entrada inválidos", errors));
            }

            // Obtener ID del usuario del token
            var userIdClaim = User.FindFirst("NameIdentifier")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ApiResponse<object>(false, null, "Token inválido"));
            }

            changePasswordDto.UsuarioId = userId;
            
            var result = await _authService.ChangePasswordAsync(changePasswordDto);
            
            if (result)
            {
                _logger.LogInformation("Usuario {UserId} cambió su contraseña exitosamente", userId);
                return Ok(new ApiResponse<string>(true, "Contraseña cambiada exitosamente", "La contraseña ha sido actualizada"));
            }
            
            return BadRequest(new ApiResponse<object>(false, null, "No se pudo cambiar la contraseña. Verifique su contraseña actual."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el cambio de contraseña");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }

    /// <summary>
    /// Validar token JWT
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>Resultado de la validación</returns>
    [HttpPost("validate-token")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public IActionResult ValidateToken([FromBody] string token)
    {
        try
        {
            var isValid = _authService.ValidateToken(token);
            return Ok(new ApiResponse<bool>(true, isValid, isValid ? "Token válido" : "Token inválido"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar token");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }

    /// <summary>
    /// Obtener información del usuario autenticado
    /// </summary>
    /// <returns>Información del usuario</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public IActionResult GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst("NameIdentifier")?.Value;
            var emailClaim = User.FindFirst("Email")?.Value;
            var nameClaim = User.FindFirst("Name")?.Value;
            var tipoUsuarioClaim = User.FindFirst("TipoUsuario")?.Value;
            var usernameClaim = User.FindFirst("Username")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new ApiResponse<object>(false, null, "Token inválido"));
            }

            var userInfo = new
            {
                Id = userIdClaim,
                Email = emailClaim,
                Name = nameClaim,
                TipoUsuario = tipoUsuarioClaim,
                Username = usernameClaim
            };

            return Ok(new ApiResponse<object>(true, userInfo, "Información del usuario obtenida exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener información del usuario");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }

    /// <summary>
    /// Cerrar sesión (logout)
    /// </summary>
    /// <returns>Confirmación del logout</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public IActionResult Logout()
    {
        try
        {
            // En JWT no hay logout del lado servidor, pero podemos loggear la acción
            var userIdClaim = User.FindFirst("NameIdentifier")?.Value;
            _logger.LogInformation("Usuario {UserId} cerró sesión", userIdClaim);
            
            return Ok(new ApiResponse<string>(true, "Sesión cerrada exitosamente", "La sesión ha sido cerrada"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el logout");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }
}