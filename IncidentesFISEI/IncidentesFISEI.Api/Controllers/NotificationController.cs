using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IncidentesFISEI.Api.Controllers;

/// <summary>
/// Controlador para la gestión de notificaciones ITIL v3
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        INotificationService notificationService,
        ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene las notificaciones del usuario actual
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotificaciones(
        [FromQuery] bool soloNoLeidas = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            var notificaciones = await _notificationService.GetNotificacionesUsuarioAsync(usuarioId, soloNoLeidas);
            
            var dtos = notificaciones.Select(n => new NotificationDto
            {
                Id = n.Id,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                TipoNotificacion = n.TipoNotificacion.ToString(),
                Prioridad = n.Prioridad.ToString(),
                Leida = n.Leida,
                IncidenteId = n.IncidenteId,
                FechaCreacion = n.CreatedAt,
                FechaLectura = n.FechaLectura
            });

            return Ok(new
            {
                notifications = dtos,
                page = page,
                pageSize = pageSize,
                total = dtos.Count()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo notificaciones del usuario");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el conteo de notificaciones no leídas
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetConteoNoLeidas()
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            var conteo = await _notificationService.GetConteoNoLeidasAsync(usuarioId);
            
            return Ok(new { count = conteo });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo conteo de notificaciones no leídas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    [HttpPut("{id}/read")]
    public async Task<ActionResult> MarcarComoLeida(int id)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            var resultado = await _notificationService.MarcarComoLeidaAsync(id, usuarioId);
            
            if (!resultado)
            {
                return NotFound("Notificación no encontrada");
            }

            return Ok(new { message = "Notificación marcada como leída" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marcando notificación {id} como leída");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Marca todas las notificaciones como leídas
    /// </summary>
    [HttpPut("read-all")]
    public async Task<ActionResult> MarcarTodasComoLeidas()
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            var resultado = await _notificationService.MarcarTodasComoLeidasAsync(usuarioId);
            
            if (!resultado)
            {
                return BadRequest("No se pudieron marcar las notificaciones como leídas");
            }

            return Ok(new { message = "Todas las notificaciones marcadas como leídas" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marcando todas las notificaciones como leídas");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene la configuración de notificaciones del usuario
    /// </summary>
    [HttpGet("settings")]
    public async Task<ActionResult<IEnumerable<NotificationSettingsDto>>> GetConfiguracion()
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            var configuraciones = new List<NotificationSettingsDto>();

            // Obtener configuración para cada tipo de evento
            foreach (TipoEventoNotificacion tipoEvento in Enum.GetValues<TipoEventoNotificacion>())
            {
                var config = await _notificationService.GetConfiguracionUsuarioAsync(usuarioId, tipoEvento);
                
                configuraciones.Add(new NotificationSettingsDto
                {
                    TipoEvento = tipoEvento.ToString(),
                    NotificarEnSistema = config.NotificarEnSistema,
                    NotificarPorEmail = config.NotificarPorEmail,
                    NotificarPorSMS = config.NotificarPorSMS,
                    NotificacionInmediata = config.NotificacionInmediata,
                    HoraInicioSilencioso = config.HoraInicioSilencioso,
                    HoraFinSilencioso = config.HoraFinSilencioso
                });
            }

            return Ok(configuraciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo configuración de notificaciones");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza la configuración de notificaciones del usuario
    /// </summary>
    [HttpPut("settings/{tipoEvento}")]
    public async Task<ActionResult> ActualizarConfiguracion(
        string tipoEvento, 
        [FromBody] UpdateNotificationSettingsDto dto)
    {
        try
        {
            if (!Enum.TryParse<TipoEventoNotificacion>(tipoEvento, out var tipo))
            {
                return BadRequest("Tipo de evento inválido");
            }

            var usuarioId = GetCurrentUserId();
            var config = await _notificationService.GetConfiguracionUsuarioAsync(usuarioId, tipo);
            
            // Actualizar configuración
            config.NotificarEnSistema = dto.NotificarEnSistema;
            config.NotificarPorEmail = dto.NotificarPorEmail;
            config.NotificarPorSMS = dto.NotificarPorSMS;
            config.NotificacionInmediata = dto.NotificacionInmediata;
            config.HoraInicioSilencioso = dto.HoraInicioSilencioso;
            config.HoraFinSilencioso = dto.HoraFinSilencioso;

            var resultado = await _notificationService.ActualizarConfiguracionAsync(usuarioId, tipo, config);
            
            if (!resultado)
            {
                return BadRequest("No se pudo actualizar la configuración");
            }

            return Ok(new { message = "Configuración actualizada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error actualizando configuración de notificaciones");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Endpoint para pruebas - crear notificación manual (solo administradores)
    /// </summary>
    [HttpPost("test")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> CrearNotificacionPrueba([FromBody] CreateNotificationDto dto)
    {
        try
        {
            if (!Enum.TryParse<TipoNotificacion>(dto.TipoNotificacion, out var tipo))
            {
                return BadRequest("Tipo de notificación inválido");
            }

            var notificacion = await _notificationService.CrearNotificacionAsync(
                dto.UsuarioId,
                tipo,
                dto.Titulo,
                dto.Mensaje,
                dto.IncidenteId
            );

            return Ok(new { 
                message = "Notificación de prueba creada",
                notificationId = notificacion.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando notificación de prueba");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("Usuario no identificado");
        }

        return userId;
    }
}