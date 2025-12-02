using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FISEI.Incidentes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FISEI.Incidentes.Presentation.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "SPOC")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verificar manualmente el correo de un usuario (útil para desarrollo/testing)
        /// </summary>
        [HttpPost("verify-user-email/{userId}")]
        public async Task<IActionResult> VerifyUserEmailManually(int userId)
        {
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado" });

            if (usuario.EmailVerificado)
                return Ok(new { message = "El correo ya estaba verificado" });

            usuario.EmailVerificado = true;
            usuario.EmailVerificationToken = null;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Correo verificado manualmente", usuario = new {
                usuario.IdUsuario,
                usuario.Nombre,
                usuario.Correo,
                usuario.EmailVerificado
            }});
        }
    }
}
