using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IncidentesFISEI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionService _notificacionService;
    private readonly ILogger<NotificacionesController> _logger;

    public NotificacionesController(
        INotificacionService notificacionService,
        ILogger<NotificacionesController> logger)
    {
        _notificacionService = notificacionService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene las notificaciones del usuario autenticado con paginación
    /// </summary>
    [HttpGet("mis-notificaciones")]
    public async Task<ActionResult<ApiResponse<NotificacionesPaginadasDto>>> GetMisNotificaciones(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamañoPagina = 20,
        [FromQuery] bool soloNoLeidas = false)
    {
        var usuarioId = GetUsuarioId();
        var response = await _notificacionService.GetNotificacionesUsuarioAsync(usuarioId, pagina, tamañoPagina, soloNoLeidas);
        return Ok(response);
    }

    /// <summary>
    /// Obtiene el contador de notificaciones no leídas del usuario autenticado
    /// </summary>
    [HttpGet("contador-no-leidas")]
    public async Task<ActionResult<ApiResponse<int>>> GetContadorNoLeidas()
    {
        var usuarioId = GetUsuarioId();
        var response = await _notificacionService.GetCountNoLeidasAsync(usuarioId);
        return Ok(response);
    }

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    [HttpPut("{id}/marcar-leida")]
    public async Task<ActionResult<ApiResponse<bool>>> MarcarComoLeida(int id)
    {
        var response = await _notificacionService.MarcarComoLeidaAsync(id);
        return Ok(response);
    }

    /// <summary>
    /// Marca todas las notificaciones del usuario como leídas
    /// </summary>
    [HttpPut("marcar-todas-leidas")]
    public async Task<ActionResult<ApiResponse<bool>>> MarcarTodasComoLeidas()
    {
        var usuarioId = GetUsuarioId();
        var response = await _notificacionService.MarcarTodasComoLeidasAsync(usuarioId);
        return Ok(response);
    }

    /// <summary>
    /// Elimina una notificación (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> EliminarNotificacion(int id)
    {
        var response = await _notificacionService.EliminarNotificacionAsync(id);
        return Ok(response);
    }

    /// <summary>
    /// Crea una notificación manualmente (solo para Supervisores y Administradores)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<NotificacionDto>>> CrearNotificacion([FromBody] CreateNotificacionDto dto)
    {
        var response = await _notificacionService.CrearNotificacionAsync(dto);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(nameof(GetMisNotificaciones), new { id = response.Data!.Id }, response);
    }

    /// <summary>
    /// Obtiene estadísticas de notificaciones del usuario autenticado
    /// </summary>
    [HttpGet("estadisticas")]
    public async Task<ActionResult<ApiResponse<EstadisticasNotificacionesDto>>> GetEstadisticas()
    {
        var usuarioId = GetUsuarioId();
        var response = await _notificacionService.GetEstadisticasNotificacionesAsync(usuarioId);
        return Ok(response);
    }

    private int GetUsuarioId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Usuario no autenticado");
        }
        return userId;
    }
}
