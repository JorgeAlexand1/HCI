using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace IncidentesFISEI.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // Buscar usuario por email
            var usuario = await _usuarioRepository.GetByEmailAsync(loginDto.Email);
            
            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            // Verificar si el usuario está activo
            if (!usuario.IsActive)
            {
                throw new UnauthorizedAccessException("Usuario inactivo. Contacte al administrador.");
            }

            // Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.PasswordHash))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            // Actualizar último acceso
            usuario.LastLoginAt = DateTime.UtcNow;
            await _usuarioRepository.UpdateAsync(usuario);

            // Generar token JWT
            var token = GenerateJwtToken(usuario);

            // Crear respuesta
            var usuarioDto = new UsuarioDto
            {
                Id = usuario.Id,
                NombreCompleto = $"{usuario.FirstName} {usuario.LastName}",
                Email = usuario.Email,
                Rol = MapTipoUsuarioToRolString(usuario.TipoUsuario),
                Activo = usuario.IsActive,
                FechaCreacion = usuario.CreatedAt,
                FechaUltimoAcceso = usuario.LastLoginAt
            };

            return new AuthResponseDto
            {
                Token = token,
                Usuario = usuarioDto
            };
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error durante el login: {ex.Message}");
        }
    }

    public async Task<ApiResponse<string>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Verificar si el email ya existe
            var existingUser = await _usuarioRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new ApiResponse<string>(false, null, "El email ya está registrado");
            }

            // Crear nuevo usuario
            var usuario = new Usuario
            {
                Username = GenerateUsernameFromEmail(registerDto.Email),
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FirstName = ExtractFirstName(registerDto.NombreCompleto),
                LastName = ExtractLastName(registerDto.NombreCompleto),
                TipoUsuario = Domain.Enums.TipoUsuario.Usuario, // Por defecto usuario básico
                IsActive = false, // El admin debe activar la cuenta
                IsEmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _usuarioRepository.AddAsync(usuario);

            return new ApiResponse<string>(true, "Usuario registrado exitosamente", 
                "Su cuenta ha sido creada. El administrador debe activarla antes de poder acceder.");
        }
        catch (Exception ex)
        {
            return new ApiResponse<string>(false, null, $"Error al registrar usuario: {ex.Message}");
        }
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(changePasswordDto.UsuarioId);
            if (usuario == null)
            {
                return false;
            }

            // Verificar contraseña actual
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, usuario.PasswordHash))
            {
                return false;
            }

            // Actualizar contraseña
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            usuario.UpdatedAt = DateTime.UtcNow;

            await _usuarioRepository.UpdateAsync(usuario);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "");
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, $"{usuario.FirstName} {usuario.LastName}"),
            new Claim("TipoUsuario", usuario.TipoUsuario.ToString()),
            new Claim("Username", usuario.Username)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string MapTipoUsuarioToRolString(Domain.Enums.TipoUsuario tipoUsuario)
    {
        return tipoUsuario switch
        {
            Domain.Enums.TipoUsuario.Administrador => "Administrador",
            Domain.Enums.TipoUsuario.Supervisor => "SupervisorTecnico",
            Domain.Enums.TipoUsuario.Tecnico => "Tecnico",
            Domain.Enums.TipoUsuario.Usuario => "Estudiante", // Por defecto
            _ => "Estudiante"
        };
    }

    private static Domain.Enums.RolUsuario MapTipoUsuarioToRol(Domain.Enums.TipoUsuario tipoUsuario)
    {
        return tipoUsuario switch
        {
            Domain.Enums.TipoUsuario.Administrador => Domain.Enums.RolUsuario.Administrador,
            Domain.Enums.TipoUsuario.Supervisor => Domain.Enums.RolUsuario.SupervisorTecnico,
            Domain.Enums.TipoUsuario.Tecnico => Domain.Enums.RolUsuario.Tecnico,
            Domain.Enums.TipoUsuario.Usuario => Domain.Enums.RolUsuario.Estudiante, // Por defecto
            _ => Domain.Enums.RolUsuario.Estudiante
        };
    }

    private static string GenerateUsernameFromEmail(string email)
    {
        return email.Split('@')[0].ToLower();
    }

    private static string ExtractFirstName(string nombreCompleto)
    {
        var parts = nombreCompleto.Trim().Split(' ');
        return parts.Length > 0 ? parts[0] : nombreCompleto;
    }

    private static string ExtractLastName(string nombreCompleto)
    {
        var parts = nombreCompleto.Trim().Split(' ');
        return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : "";
    }
}