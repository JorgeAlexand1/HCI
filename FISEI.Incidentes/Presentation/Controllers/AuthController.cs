using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FISEI.Incidentes.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using FISEI.Incidentes.Core.Entities;
using Microsoft.EntityFrameworkCore;
using FISEI.Incidentes.Infrastructure.Security;
using FISEI.Incidentes.Infrastructure.Services;

namespace FISEI.Incidentes.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        public AuthController(IConfiguration config,
                              ApplicationDbContext context,
                              EmailService emailService)
        {
            _config = config;
            _context = context;
            _emailService = emailService;
        }

        public record RegisterRequest(string Email, string Password, string NombreMostrar, string RolSistema);
        public record LoginRequest(string Email, string Password);
        public record RequestResetPassword(string Email);
        public record ConfirmResetPassword(string Email, string Token, string NewPassword);
        public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

        private static readonly HashSet<string> OutlookDomains = new(StringComparer.OrdinalIgnoreCase)
        {
            "outlook.com", "hotmail.com", "live.com"
        };

        private static bool IsAllowedEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var at = email.LastIndexOf('@');
                if (at <= 0 || at == email.Length - 1) return false;
                var domain = email[(at + 1)..];
                if (domain.Equals("uta.edu.ec", StringComparison.OrdinalIgnoreCase)) return true; // estudiantes/docentes
                if (OutlookDomains.Contains(domain)) return true; // otros usuarios con Outlook
                return false;
            }
            catch { return false; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!IsAllowedEmail(req.Email))
                return BadRequest(new { message = "Correo no permitido. Use @uta.edu.ec o Outlook (@outlook.com, @hotmail.com, @live.com)." });

            var existing = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == req.Email);
            if (existing != null) return Conflict(new { message = "Email ya registrado" });

            var usuario = new Usuario
            {
                Nombre = req.NombreMostrar,
                Correo = req.Email,
                Contrasena = PasswordHasher.HashPassword(req.Password),
                Activo = true
            };
            // Generar token de verificación de correo
            usuario.EmailVerificationToken = Guid.NewGuid().ToString("N");
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var baseUrl = _config["AppBaseUrl"] ?? "http://localhost:5023";
            var verifyLink = $"{baseUrl}/verify-email?email={Uri.EscapeDataString(usuario.Correo)}&token={usuario.EmailVerificationToken}";
            await _emailService.SendAsync(usuario.Correo, "Verifica tu correo - FISEI Incidentes",
                $"<p>Hola {usuario.Nombre},</p><p>Por favor verifica tu correo haciendo clic en el siguiente enlace:</p><p><a href='{verifyLink}'>Verificar correo</a></p>");

            return Ok(new { message = "Registro exitoso", idUsuario = usuario.IdUsuario });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == req.Email && u.Activo);
            if (usuario == null) return Unauthorized(new { code = "INVALID_CREDENTIALS", message = "Credenciales inválidas" });

            if (usuario.EmailVerificado == false)
                return Unauthorized(new { code = "EMAIL_NOT_VERIFIED", message = "Correo no verificado" });

            // Verificación con hash PBKDF2
            if (!PasswordHasher.Verify(req.Password, usuario.Contrasena))
                return Unauthorized(new { message = "Credenciales inválidas" });

            var roles = new List<string>();
            if (usuario.Rol?.Nombre != null)
            {
                roles.Add(usuario.Rol.Nombre);
            }
            var token = GenerateJwtTokenFromUsuario(usuario, roles);
            return Ok(new { token, roles });
        }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] RequestResetPassword req)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == req.Email && u.Activo);
        if (usuario == null) return Ok(new { message = "Si el correo existe, se enviará un mensaje" });
        if (usuario.EmailVerificado) return Ok(new { message = "El correo ya está verificado" });

        usuario.EmailVerificationToken = Guid.NewGuid().ToString("N");
        await _context.SaveChangesAsync();

        var baseUrl = _config["AppBaseUrl"] ?? "http://localhost:5023";
        var verifyLink = $"{baseUrl}/verify-email?email={Uri.EscapeDataString(usuario.Correo)}&token={usuario.EmailVerificationToken}";
        await _emailService.SendAsync(usuario.Correo, "Reenviar verificación - FISEI Incidentes",
            $"<p>Hola {usuario.Nombre},</p><p>Verifica tu correo usando este enlace:</p><p><a href='{verifyLink}'>Verificar correo</a></p>");

        return Ok(new { message = "Si el correo existe, se enviará un mensaje" });
    }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == email);
            if (usuario == null) return NotFound(new { message = "Usuario no encontrado" });
            if (usuario.EmailVerificado) return Ok(new { message = "Correo ya verificado" });
            if (usuario.EmailVerificationToken != token) return BadRequest(new { message = "Token inválido" });

            usuario.EmailVerificado = true;
            usuario.EmailVerificationToken = null;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Correo verificado exitosamente" });
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestResetPassword req)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == req.Email && u.Activo);
            if (usuario == null) return Ok(new { message = "Si el correo existe, se enviará un mensaje" });

            usuario.ResetToken = Guid.NewGuid().ToString("N");
            usuario.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            var baseUrl = _config["AppBaseUrl"] ?? "http://localhost:5023";
            var resetLink = $"{baseUrl}/reset-password?email={Uri.EscapeDataString(usuario.Correo)}&token={usuario.ResetToken}";
            await _emailService.SendAsync(usuario.Correo, "Recuperar contraseña - FISEI Incidentes",
                $"<p>Has solicitado recuperar tu contraseña. Usa el siguiente enlace dentro de 1 hora:</p><p><a href='{resetLink}'>Restablecer contraseña</a></p>");

            return Ok(new { message = "Si el correo existe, se enviará un mensaje" });
        }

        [HttpPost("confirm-password-reset")]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] ConfirmResetPassword req)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == req.Email && u.Activo);
            if (usuario == null) return BadRequest(new { message = "Solicitud inválida" });
            if (usuario.ResetToken == null || usuario.ResetTokenExpiry == null) return BadRequest(new { message = "No hay token activo" });
            if (usuario.ResetToken != req.Token) return BadRequest(new { message = "Token inválido" });
            if (usuario.ResetTokenExpiry < DateTime.UtcNow) return BadRequest(new { message = "Token expirado" });

            usuario.Contrasena = PasswordHasher.HashPassword(req.NewPassword);
            usuario.ResetToken = null;
            usuario.ResetTokenExpiry = null;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Contraseña actualizada" });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null) return Unauthorized();
            if (string.IsNullOrWhiteSpace(req.CurrentPassword) || string.IsNullOrWhiteSpace(req.NewPassword))
                return BadRequest(new { message = "Contraseñas inválidas" });

            var userId = int.Parse(userIdClaim.Value);
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == userId && u.Activo);
            if (usuario == null) return Unauthorized();

            if (!PasswordHasher.Verify(req.CurrentPassword, usuario.Contrasena))
                return BadRequest(new { message = "Contraseña actual incorrecta" });

            usuario.Contrasena = PasswordHasher.HashPassword(req.NewPassword);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Contraseña cambiada" });
        }

        private string GenerateJwtTokenFromUsuario(Usuario usuario, IList<string> roles)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection.GetValue<string>("Key")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(jwtSection.GetValue<int>("ExpirationMinutes"));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
                new Claim("displayName", usuario.Nombre)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = new JwtSecurityToken(
                issuer: jwtSection.GetValue<string>("Issuer"),
                audience: jwtSection.GetValue<string>("Audience"),
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
