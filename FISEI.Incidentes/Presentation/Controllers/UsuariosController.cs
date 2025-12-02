using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IRepositories;
using FISEI.Incidentes.Core.DTOs;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace FISEI.Incidentes.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRolRepository _rolRepository;

        public UsuariosController(
            IUsuarioRepository usuarioRepository,
            IRolRepository rolRepository)
        {
            _usuarioRepository = usuarioRepository;
            _rolRepository = rolRepository;
        }

        /// <summary>
        /// Obtiene todos los usuarios activos con sus roles (Solo Admin)
        /// </summary>
        [Authorize(Roles = "SPOC")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioConRolDTO>>> GetUsuarios()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            
            var usuariosDTO = usuarios.Select(u => new UsuarioConRolDTO
            {
                IdUsuario = u.IdUsuario,
                Nombre = u.Nombre,
                Correo = u.Correo,
                Activo = u.Activo,
                EmailVerificado = u.EmailVerificado,
                IdRol = u.IdRol,
                NombreRol = u.Rol?.Nombre,
                DescripcionRol = u.Rol?.Descripcion
            }).ToList();

            return Ok(usuariosDTO);
        }

        /// <summary>
        /// Obtiene un usuario por ID (Solo Admin)
        /// </summary>
        [Authorize(Roles = "SPOC")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioConRolDTO>> GetUsuario(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var usuarioDTO = new UsuarioConRolDTO
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Activo = usuario.Activo,
                EmailVerificado = usuario.EmailVerificado,
                IdRol = usuario.IdRol,
                NombreRol = usuario.Rol?.Nombre,
                DescripcionRol = usuario.Rol?.Descripcion
            };

            return Ok(usuarioDTO);
        }

        /// <summary>
        /// Asigna un rol a un usuario (Solo Admin/SPOC)
        /// </summary>
        [Authorize(Roles = "SPOC")]
        [HttpPost("asignar-rol")]
        public async Task<ActionResult> AsignarRol([FromBody] AsignarRolDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = await _usuarioRepository.GetByIdAsync(dto.IdUsuario);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var rol = await _rolRepository.GetByIdAsync(dto.IdRol);
            if (rol == null)
                return NotFound(new { message = "Rol no encontrado" });

            usuario.IdRol = dto.IdRol;
            await _usuarioRepository.UpdateAsync(usuario);

            return Ok(new { 
                message = $"Rol '{rol.Nombre}' asignado exitosamente al usuario '{usuario.Nombre}'",
                usuario = new UsuarioConRolDTO
                {
                    IdUsuario = usuario.IdUsuario,
                    Nombre = usuario.Nombre,
                    Correo = usuario.Correo,
                    Activo = usuario.Activo,
                    EmailVerificado = usuario.EmailVerificado,
                    IdRol = usuario.IdRol,
                    NombreRol = rol.Nombre,
                    DescripcionRol = rol.Descripcion
                }
            });
        }

        /// <summary>
        /// Remueve el rol de un usuario (Solo Admin/SPOC)
        /// </summary>
        [Authorize(Roles = "SPOC")]
        [HttpPost("{id}/remover-rol")]
        public async Task<ActionResult> RemoverRol(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado" });

            usuario.IdRol = null;
            await _usuarioRepository.UpdateAsync(usuario);

            return Ok(new { message = $"Rol removido del usuario '{usuario.Nombre}'" });
        }

        /// <summary>
        /// Activa o desactiva un usuario (Solo Admin/SPOC)
        /// </summary>
        [Authorize(Roles = "SPOC")]
        [HttpPost("{id}/toggle-activo")]
        public async Task<ActionResult> ToggleActivo(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado" });

            usuario.Activo = !usuario.Activo;
            await _usuarioRepository.UpdateAsync(usuario);

            return Ok(new { 
                message = $"Usuario '{usuario.Nombre}' {(usuario.Activo ? "activado" : "desactivado")}",
                activo = usuario.Activo
            });
        }

        /// <summary>
        /// Obtiene todos los roles disponibles (Solo Admin)
        /// </summary>
        [Authorize(Roles = "SPOC")]
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<RolDTO>>> GetRoles()
        {
            var roles = await _rolRepository.GetAllAsync();
            
            var rolesDTO = roles.Select(r => new RolDTO
            {
                IdRol = r.IdRol,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion
            }).ToList();

            return Ok(rolesDTO);
        }

        /// <summary>
        /// Obtiene técnicos de un nivel de soporte
        /// </summary>
        [Authorize]
        [HttpGet("tecnicos/nivel/{idNivel}")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetTecnicosPorNivel(int idNivel)
        {
            var tecnicos = await _usuarioRepository.GetTecnicosPorNivelAsync(idNivel);
            return Ok(tecnicos);
        }

        /// <summary>
        /// Verifica si un usuario es SPOC
        /// </summary>
        [Authorize]
        [HttpGet("{id}/es-spoc")]
        public async Task<ActionResult<bool>> EsSPOC(int id)
        {
            var esSPOC = await _usuarioRepository.EsSPOCAsync(id);
            return Ok(new { esSPOC });
        }

        /// <summary>
        /// Obtiene el perfil del usuario actual
        /// </summary>
        [Authorize]
        [HttpGet("mi-perfil")]
        public async Task<ActionResult<UsuarioConRolDTO>> GetMiPerfil()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var usuario = await _usuarioRepository.GetByIdAsync(userId);
            
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var usuarioDTO = new UsuarioConRolDTO
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Activo = usuario.Activo,
                EmailVerificado = usuario.EmailVerificado,
                IdRol = usuario.IdRol,
                NombreRol = usuario.Rol?.Nombre,
                DescripcionRol = usuario.Rol?.Descripcion
            };

            return Ok(usuarioDTO);
        }
    }
}