using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IncidentesFISEI.Infrastructure.Services;

namespace IncidentesFISEI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthController(
        IAuthService authService, 
        ILogger<AuthController> logger,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _authService = authService;
        _logger = logger;
        _emailService = emailService;
        _configuration = configuration;
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

    /// <summary>
    /// Solicitar recuperación de contraseña
    /// </summary>
    /// <param name="request">Email del usuario</param>
    /// <returns>Confirmación del envío</returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new ApiResponse<object>(false, null, "El email es requerido"));
            }

            var usuario = await _authService.GetUserByEmailAsync(request.Email);
            if (usuario == null)
            {
                return NotFound(new ApiResponse<object>(false, null, "El correo electrónico no está registrado"));
            }

            // Generar token de recuperación (válido por 1 hora)
            var resetToken = Guid.NewGuid().ToString("N");
            var expirationTime = DateTime.UtcNow.AddHours(1);
            
            // Guardar token en la base de datos
            await _authService.SavePasswordResetTokenAsync(usuario.Id, resetToken, expirationTime);

            // Crear enlace de recuperación
            var resetLink = $"https://localhost:7000/reset-password?token={resetToken}";

            // Enviar email
            var emailSubject = "Recuperación de Contraseña - IncidentesFISEI";
            var emailBody = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .button {{ display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .warning {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🔐 Recuperación de Contraseña</h1>
                        </div>
                        <div class='content'>
                            <p>Hola <strong>{usuario.FirstName} {usuario.LastName}</strong>,</p>
                            <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta en IncidentesFISEI.</p>
                            <p>Haz clic en el siguiente botón para crear una nueva contraseña:</p>
                            <p style='text-align: center;'>
                                <a href='{resetLink}' class='button'>Restablecer Contraseña</a>
                            </p>
                            <p>O copia y pega este enlace en tu navegador:</p>
                            <p style='word-break: break-all; background: #fff; padding: 10px; border: 1px solid #ddd; border-radius: 5px;'>{resetLink}</p>
                            <div class='warning'>
                                <strong>⚠️ Importante:</strong>
                                <ul>
                                    <li>Este enlace es válido por <strong>1 hora</strong></li>
                                    <li>Si no solicitaste este cambio, ignora este correo</li>
                                    <li>Nunca compartas este enlace con nadie</li>
                                </ul>
                            </div>
                            <p>Saludos,<br><strong>Equipo de IncidentesFISEI - UTA</strong></p>
                        </div>
                        <div class='footer'>
                            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
                            <p>&copy; 2024 FISEI - Universidad Técnica de Ambato</p>
                        </div>
                    </div>
                </body>
                </html>";

            await _emailService.EnviarEmailAsync(usuario.Email, emailSubject, emailBody);

            _logger.LogInformation("Token de recuperación generado para usuario {Email}", request.Email);

            return Ok(new ApiResponse<string>(true, "Email enviado exitosamente", "Se ha enviado un enlace de recuperación a tu correo"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar solicitud de recuperación de contraseña");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }

    /// <summary>
    /// Restablecer contraseña con token
    /// </summary>
    /// <param name="request">Token y nueva contraseña</param>
    /// <returns>Confirmación del cambio</returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest(new ApiResponse<object>(false, null, "Token requerido"));
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
            {
                return BadRequest(new ApiResponse<object>(false, null, "La contraseña debe tener al menos 8 caracteres"));
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new ApiResponse<object>(false, null, "Las contraseñas no coinciden"));
            }

            // Validar token y obtener usuario
            _logger.LogInformation("Intentando validar token: {Token}", request.Token);
            var usuario = await _authService.ValidatePasswordResetTokenAsync(request.Token);
            if (usuario == null)
            {
                _logger.LogWarning("Token no válido o expirado: {Token}", request.Token);
                return BadRequest(new ApiResponse<object>(false, null, "Token inválido o expirado"));
            }
            _logger.LogInformation("Token válido para usuario: {UserId}", usuario.Id);

            // Cambiar contraseña
            var result = await _authService.ResetPasswordAsync(usuario.Id, request.NewPassword);
            if (!result)
            {
                return BadRequest(new ApiResponse<object>(false, null, "No se pudo cambiar la contraseña"));
            }

            // Invalidar token
            await _authService.InvalidatePasswordResetTokenAsync(request.Token);

            _logger.LogInformation("Contraseña restablecida para usuario {UserId}", usuario.Id);

            return Ok(new ApiResponse<string>(true, "Contraseña cambiada exitosamente", "Ya puedes iniciar sesión con tu nueva contraseña"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al restablecer contraseña");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
        }
    }
}

// DTOs para recuperación de contraseña
public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}