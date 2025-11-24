using Microsoft.AspNetCore.Mvc;
using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IServices;
using FISEI.Incidentes.Core.Interfaces.IRepositories;

namespace FISEI.Incidentes.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;
        private readonly INotificacionRepository _notificacionRepository;

        public NotificacionesController(
            INotificacionService notificacionService,
            INotificacionRepository notificacionRepository)
        {
            _notificacionService = notificacionService;
            _notificacionRepository = notificacionRepository;
        }

        /// <summary>
        /// Obtiene notificaciones no leídas de un usuario
        /// </summary>
        [HttpGet("no-leidas/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<Notificacion>>> GetNotificacionesNoLeidas(int idUsuario)
        {
            var notificaciones = await _notificacionService.ObtenerNotificacionesNoLeidasAsync(idUsuario);
            return Ok(notificaciones);
        }

        /// <summary>
        /// Marca una notificación como leída
        /// </summary>
        [HttpPut("marcar-leida/{idNotificacion}")]
        public async Task<IActionResult> MarcarComoLeida(int idNotificacion)
        {
            try
            {
                await _notificacionService.MarcarComoLeidaAsync(idNotificacion);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Marca todas las notificaciones de un usuario como leídas
        /// </summary>
        [HttpPut("marcar-todas-leidas/{idUsuario}")]
        public async Task<IActionResult> MarcarTodasComoLeidas(int idUsuario)
        {
            try
            {
                await _notificacionRepository.MarcarTodasComoLeidasAsync(idUsuario);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Envía una notificación manual
        /// </summary>
        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarNotificacion(
            [FromQuery] int idUsuario, 
            [FromQuery] string mensaje, 
            [FromQuery] string tipo = "info")
        {
            try
            {
                await _notificacionService.EnviarNotificacionAsync(idUsuario, mensaje, tipo);
                return Ok(new { message = "Notificación enviada" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}