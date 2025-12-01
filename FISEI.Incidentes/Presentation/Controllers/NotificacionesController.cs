using Microsoft.AspNetCore.Mvc;
using FISEI.Incidentes.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using FISEI.Incidentes.Core.Interfaces.IServices;
using FISEI.Incidentes.Core.Interfaces.IRepositories;

namespace FISEI.Incidentes.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        /// Obtiene notificaciones no le�das de un usuario
        /// </summary>
        [HttpGet("no-leidas/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<Notificacion>>> GetNotificacionesNoLeidas(int idUsuario)
        {
            var notificaciones = await _notificacionService.ObtenerNotificacionesNoLeidasAsync(idUsuario);
            return Ok(notificaciones);
        }

        /// <summary>
        /// Marca una notificaci�n como le�da
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
        /// Marca todas las notificaciones de un usuario como le�das
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
        /// Env�a una notificaci�n manual
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
                return Ok(new { message = "Notificaci�n enviada" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}