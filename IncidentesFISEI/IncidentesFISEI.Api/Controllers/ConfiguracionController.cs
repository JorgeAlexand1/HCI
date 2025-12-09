using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IncidentesFISEI.Infrastructure.Data;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Infrastructure.Services;

namespace IncidentesFISEI.Api.Controllers;

/// <summary>
/// Controlador para la gestión de configuración del sistema
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Administrador")] // Temporalmente deshabilitado para testing
public class ConfiguracionController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ConfiguracionController> _logger;

    public ConfiguracionController(ApplicationDbContext context, ILogger<ConfiguracionController> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Configuración General del Sistema

    /// <summary>
    /// Obtener la configuración general del sistema
    /// </summary>
    [HttpGet("sistema")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> GetConfiguracionSistema()
    {
        try
        {
            var config = await _context.ConfiguracionesSistema
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();

            if (config == null)
            {
                return NotFound(new ApiResponse<object>(false, null, "No existe configuración del sistema"));
            }

            return Ok(new ApiResponse<object>(true, config, "Configuración del sistema obtenida"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener configuración del sistema");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al obtener la configuración"));
        }
    }

    /// <summary>
    /// Crear o actualizar configuración del sistema
    /// </summary>
    [HttpPost("sistema")]
    [ProducesResponseType(typeof(ApiResponse<object>), 201)]
    public async Task<IActionResult> CreateOrUpdateConfiguracionSistema([FromBody] CreateConfiguracionSistemaDto createDto)
    {
        try
        {
            var existingConfig = await _context.ConfiguracionesSistema
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();

            ConfiguracionSistema config;

            if (existingConfig == null)
            {
                config = new ConfiguracionSistema
                {
                    Nombre = createDto.Nombre,
                    Descripcion = createDto.Descripcion ?? string.Empty,
                    LogoUrl = createDto.LogoUrl ?? string.Empty,
                    EmailSoporte = createDto.EmailSoporte ?? string.Empty,
                    Telefono = createDto.Telefono ?? string.Empty,
                    TiempoRespuestaDefecto = createDto.TiempoRespuestaDefecto ?? 60,
                    TiempoResolucionDefecto = createDto.TiempoResolucionDefecto ?? 480,
                    PermitirEscalacionAutomatica = createDto.PermitirEscalacionAutomatica ?? true,
                    PortcentajeEscalacion = createDto.PortcentajeEscalacion ?? 75,
                    NotificacionesHabilitadas = createDto.NotificacionesHabilitadas ?? true,
                    NotificacionesPorEmail = createDto.NotificacionesPorEmail ?? true,
                    NotificacionesPorSistema = createDto.NotificacionesPorSistema ?? true,
                    AuditoriaHabilitada = createDto.AuditoriaHabilitada ?? true,
                    DiasRetencionLogs = createDto.DiasRetencionLogs ?? 90,
                    ZonaHoraria = createDto.ZonaHoraria ?? "America/Guayaquil",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.ConfiguracionesSistema.Add(config);
            }
            else
            {
                existingConfig.Nombre = createDto.Nombre;
                existingConfig.Descripcion = createDto.Descripcion ?? existingConfig.Descripcion;
                existingConfig.LogoUrl = createDto.LogoUrl ?? existingConfig.LogoUrl;
                existingConfig.EmailSoporte = createDto.EmailSoporte ?? existingConfig.EmailSoporte;
                existingConfig.Telefono = createDto.Telefono ?? existingConfig.Telefono;
                existingConfig.TiempoRespuestaDefecto = createDto.TiempoRespuestaDefecto ?? existingConfig.TiempoRespuestaDefecto;
                existingConfig.TiempoResolucionDefecto = createDto.TiempoResolucionDefecto ?? existingConfig.TiempoResolucionDefecto;
                existingConfig.PermitirEscalacionAutomatica = createDto.PermitirEscalacionAutomatica ?? existingConfig.PermitirEscalacionAutomatica;
                existingConfig.PortcentajeEscalacion = createDto.PortcentajeEscalacion ?? existingConfig.PortcentajeEscalacion;
                existingConfig.NotificacionesHabilitadas = createDto.NotificacionesHabilitadas ?? existingConfig.NotificacionesHabilitadas;
                existingConfig.NotificacionesPorEmail = createDto.NotificacionesPorEmail ?? existingConfig.NotificacionesPorEmail;
                existingConfig.NotificacionesPorSistema = createDto.NotificacionesPorSistema ?? existingConfig.NotificacionesPorSistema;
                existingConfig.AuditoriaHabilitada = createDto.AuditoriaHabilitada ?? existingConfig.AuditoriaHabilitada;
                existingConfig.DiasRetencionLogs = createDto.DiasRetencionLogs ?? existingConfig.DiasRetencionLogs;
                existingConfig.ZonaHoraria = createDto.ZonaHoraria ?? existingConfig.ZonaHoraria;
                existingConfig.UpdatedAt = DateTime.UtcNow;

                _context.ConfiguracionesSistema.Update(existingConfig);
                config = existingConfig;
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetConfiguracionSistema), new ApiResponse<object>(true, config, "Configuración guardada exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar configuración del sistema");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al guardar la configuración"));
        }
    }

    #endregion

    #region Configuración de SLAs

    /// <summary>
    /// Obtener todas las configuraciones de SLA
    /// </summary>
    [HttpGet("slas")]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), 200)]
    public async Task<IActionResult> GetConfiguracionesSLA()
    {
        try
        {
            var slas = await _context.ConfiguracionesSLA
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return Ok(new ApiResponse<List<object>>(true, slas.Cast<object>().ToList(), "SLAs obtenidos"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener SLAs");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al obtener las configuraciones"));
        }
    }

    /// <summary>
    /// Crear nueva configuración de SLA
    /// </summary>
    [HttpPost("slas")]
    [ProducesResponseType(typeof(ApiResponse<object>), 201)]
    public async Task<IActionResult> CreateConfiguracionSLA([FromBody] CreateConfiguracionSLADto createDto)
    {
        try
        {
            var sla = new ConfiguracionSLA
            {
                Nombre = createDto.Nombre,
                Descripcion = createDto.Descripcion ?? string.Empty,
                TiempoRespuestaCritica = createDto.TiempoRespuestaCritica,
                TiempoRespuestaAlta = createDto.TiempoRespuestaAlta,
                TiempoRespuestaMedia = createDto.TiempoRespuestaMedia,
                TiempoRespuestaBaja = createDto.TiempoRespuestaBaja,
                TiempoResolucionCritica = createDto.TiempoResolucionCritica,
                TiempoResolucionAlta = createDto.TiempoResolucionAlta,
                TiempoResolucionMedia = createDto.TiempoResolucionMedia,
                TiempoResolucionBaja = createDto.TiempoResolucionBaja,
                PermitirEscalacion = createDto.PermitirEscalacion,
                MinutosAntesDeLlegar = createDto.MinutosAntesDeLlegar,
                NotificarAlEscalar = createDto.NotificarAlEscalar,
                NotificarAlVencer = createDto.NotificarAlVencer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ConfiguracionesSLA.Add(sla);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetConfiguracionesSLA), new ApiResponse<object>(true, sla, "SLA creado exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear SLA");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al crear el SLA"));
        }
    }

    /// <summary>
    /// Actualizar configuración de SLA existente
    /// </summary>
    [HttpPut("slas/{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> UpdateConfiguracionSLA(int id, [FromBody] CreateConfiguracionSLADto updateDto)
    {
        try
        {
            var sla = await _context.ConfiguracionesSLA
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (sla == null)
            {
                return NotFound(new ApiResponse<object>(false, null, "SLA no encontrado"));
            }

            sla.Nombre = updateDto.Nombre;
            sla.Descripcion = updateDto.Descripcion ?? string.Empty;
            sla.TiempoRespuestaCritica = updateDto.TiempoRespuestaCritica;
            sla.TiempoRespuestaAlta = updateDto.TiempoRespuestaAlta;
            sla.TiempoRespuestaMedia = updateDto.TiempoRespuestaMedia;
            sla.TiempoRespuestaBaja = updateDto.TiempoRespuestaBaja;
            sla.TiempoResolucionCritica = updateDto.TiempoResolucionCritica;
            sla.TiempoResolucionAlta = updateDto.TiempoResolucionAlta;
            sla.TiempoResolucionMedia = updateDto.TiempoResolucionMedia;
            sla.TiempoResolucionBaja = updateDto.TiempoResolucionBaja;
            sla.PermitirEscalacion = updateDto.PermitirEscalacion;
            sla.MinutosAntesDeLlegar = updateDto.MinutosAntesDeLlegar;
            sla.NotificarAlEscalar = updateDto.NotificarAlEscalar;
            sla.NotificarAlVencer = updateDto.NotificarAlVencer;
            sla.UpdatedAt = DateTime.UtcNow;

            _context.ConfiguracionesSLA.Update(sla);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>(true, sla, "SLA actualizado exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar SLA");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al actualizar el SLA"));
        }
    }

    /// <summary>
    /// Eliminar (deshabilitar) configuración de SLA
    /// </summary>
    [HttpDelete("slas/{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> DeleteConfiguracionSLA(int id)
    {
        try
        {
            var sla = await _context.ConfiguracionesSLA
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (sla == null)
            {
                return NotFound(new ApiResponse<object>(false, null, "SLA no encontrado"));
            }

            // Soft delete - solo marcar como inactivo
            sla.IsActive = false;
            sla.UpdatedAt = DateTime.UtcNow;

            _context.ConfiguracionesSLA.Update(sla);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>(true, null, "SLA eliminado exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar SLA");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al eliminar el SLA"));
        }
    }

    #endregion

    #region Notificaciones

    /// <summary>
    /// Obtener todas las plantillas de notificación activas
    /// </summary>
    [HttpGet("plantillas-notificacion")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PlantillaNotificacionDto>>), 200)]
    public async Task<IActionResult> GetPlantillasNotificacion()
    {
        try
        {
            var plantillas = await _context.PlantillasNotificacion
                .Where(p => p.IsActive)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            var dtos = plantillas.Select(p => new PlantillaNotificacionDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                TipoNotificacion = p.TipoNotificacion.ToString(),
                PlantillaTitulo = p.PlantillaTitulo,
                PlantillaMensaje = p.PlantillaMensaje,
                PlantillaEmail = p.PlantillaEmail,
                PlantillaSMS = p.PlantillaSMS,
                IsActive = p.IsActive,
                VariablesDisponibles = p.VariablesDisponibles,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<PlantillaNotificacionDto>>(true, dtos, "Plantillas obtenidas"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener plantillas de notificación");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al obtener plantillas"));
        }
    }

    /// <summary>
    /// Obtener plantilla de notificación por ID
    /// </summary>
    [HttpGet("plantillas-notificacion/{id}")]
    [ProducesResponseType(typeof(ApiResponse<PlantillaNotificacionDto>), 200)]
    public async Task<IActionResult> GetPlantillaNotificacion(int id)
    {
        try
        {
            var plantilla = await _context.PlantillasNotificacion
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plantilla == null)
            {
                return NotFound(new ApiResponse<object>(false, null, "Plantilla no encontrada"));
            }

            var dto = new PlantillaNotificacionDto
            {
                Id = plantilla.Id,
                Nombre = plantilla.Nombre,
                TipoNotificacion = plantilla.TipoNotificacion.ToString(),
                PlantillaTitulo = plantilla.PlantillaTitulo,
                PlantillaMensaje = plantilla.PlantillaMensaje,
                PlantillaEmail = plantilla.PlantillaEmail,
                PlantillaSMS = plantilla.PlantillaSMS,
                IsActive = plantilla.IsActive,
                VariablesDisponibles = plantilla.VariablesDisponibles,
                CreatedAt = plantilla.CreatedAt
            };

            return Ok(new ApiResponse<PlantillaNotificacionDto>(true, dto, "Plantilla obtenida"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener plantilla de notificación");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al obtener plantilla"));
        }
    }

    /// <summary>
    /// Crear nueva plantilla de notificación
    /// </summary>
    [HttpPost("plantillas-notificacion")]
    [ProducesResponseType(typeof(ApiResponse<PlantillaNotificacionDto>), 201)]
    public async Task<IActionResult> CreatePlantillaNotificacion([FromBody] CreatePlantillaNotificacionDto dto)
    {
        try
        {
            if (!Enum.TryParse<TipoNotificacion>(dto.TipoNotificacion, true, out var tipoNotificacion))
            {
                return BadRequest(new ApiResponse<object>(false, null, "Tipo de notificación inválido"));
            }

            var plantilla = new PlantillaNotificacion
            {
                Nombre = dto.Nombre,
                TipoNotificacion = tipoNotificacion,
                PlantillaTitulo = dto.PlantillaTitulo,
                PlantillaMensaje = dto.PlantillaMensaje,
                PlantillaEmail = dto.PlantillaEmail,
                PlantillaSMS = dto.PlantillaSMS,
                VariablesDisponibles = dto.VariablesDisponibles,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.PlantillasNotificacion.Add(plantilla);
            await _context.SaveChangesAsync();

            var resultado = new PlantillaNotificacionDto
            {
                Id = plantilla.Id,
                Nombre = plantilla.Nombre,
                TipoNotificacion = plantilla.TipoNotificacion.ToString(),
                PlantillaTitulo = plantilla.PlantillaTitulo,
                PlantillaMensaje = plantilla.PlantillaMensaje,
                PlantillaEmail = plantilla.PlantillaEmail,
                PlantillaSMS = plantilla.PlantillaSMS,
                IsActive = plantilla.IsActive,
                VariablesDisponibles = plantilla.VariablesDisponibles,
                CreatedAt = plantilla.CreatedAt
            };

            return CreatedAtAction(nameof(GetPlantillaNotificacion), new { id = plantilla.Id }, 
                new ApiResponse<PlantillaNotificacionDto>(true, resultado, "Plantilla creada exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear plantilla de notificación");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al crear plantilla"));
        }
    }

    /// <summary>
    /// Actualizar plantilla de notificación
    /// </summary>
    [HttpPut("plantillas-notificacion/{id}")]
    [ProducesResponseType(typeof(ApiResponse<PlantillaNotificacionDto>), 200)]
    public async Task<IActionResult> UpdatePlantillaNotificacion(int id, [FromBody] UpdatePlantillaNotificacionDto dto)
    {
        try
        {
            var plantilla = await _context.PlantillasNotificacion
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plantilla == null)
            {
                return NotFound(new ApiResponse<object>(false, null, "Plantilla no encontrada"));
            }

            plantilla.Nombre = dto.Nombre;
            plantilla.PlantillaTitulo = dto.PlantillaTitulo;
            plantilla.PlantillaMensaje = dto.PlantillaMensaje;
            plantilla.PlantillaEmail = dto.PlantillaEmail;
            plantilla.PlantillaSMS = dto.PlantillaSMS;
            plantilla.IsActive = dto.IsActive;
            plantilla.VariablesDisponibles = dto.VariablesDisponibles;
            plantilla.UpdatedAt = DateTime.UtcNow;

            _context.PlantillasNotificacion.Update(plantilla);
            await _context.SaveChangesAsync();

            var resultado = new PlantillaNotificacionDto
            {
                Id = plantilla.Id,
                Nombre = plantilla.Nombre,
                TipoNotificacion = plantilla.TipoNotificacion.ToString(),
                PlantillaTitulo = plantilla.PlantillaTitulo,
                PlantillaMensaje = plantilla.PlantillaMensaje,
                PlantillaEmail = plantilla.PlantillaEmail,
                PlantillaSMS = plantilla.PlantillaSMS,
                IsActive = plantilla.IsActive,
                VariablesDisponibles = plantilla.VariablesDisponibles,
                CreatedAt = plantilla.CreatedAt
            };

            return Ok(new ApiResponse<PlantillaNotificacionDto>(true, resultado, "Plantilla actualizada exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar plantilla de notificación");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al actualizar plantilla"));
        }
    }

    /// <summary>
    /// Obtener logs de notificaciones enviadas
    /// </summary>
    [HttpGet("logs-notificacion")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LogNotificacionDto>>), 200)]
    public async Task<IActionResult> GetLogsNotificacion(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        [FromQuery] string? estado)
    {
        try
        {
            var query = _context.LogsNotificacion.AsQueryable();

            if (fechaInicio.HasValue)
                query = query.Where(l => l.FechaIntento >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(l => l.FechaIntento <= fechaFin.Value.AddDays(1));

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(l => l.Estado.ToString() == estado);

            var logs = await query
                .OrderByDescending(l => l.FechaIntento)
                .Take(500)
                .ToListAsync();

            var dtos = logs.Select(l => new LogNotificacionDto
            {
                Id = l.Id,
                Fecha = l.FechaIntento,
                Tipo = l.Notificacion?.TipoNotificacion.ToString() ?? "Desconocido",
                Destinatario = l.Notificacion?.Usuario.Email ?? "N/A",
                Canal = l.Canal.ToString(),
                Estado = l.Estado.ToString(),
                Error = l.ErrorDetalle
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<LogNotificacionDto>>(true, dtos, "Logs obtenidos"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener logs de notificaciones");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al obtener logs"));
        }
    }

    #endregion

    #region Configuración de Email

    /// <summary>
    /// Obtener configuración de email (sin credenciales sensibles)
    /// </summary>
    [HttpGet("email-settings")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public IActionResult GetEmailSettings([FromServices] IConfiguration configuration)
    {
        try
        {
            var emailSettings = new
            {
                SmtpHost = configuration["EmailSettings:SmtpHost"],
                SmtpPort = configuration["EmailSettings:SmtpPort"],
                FromEmail = configuration["EmailSettings:FromEmail"],
                FromName = configuration["EmailSettings:FromName"],
                IsConfigured = !string.IsNullOrEmpty(configuration["EmailSettings:SmtpPassword"])
            };

            return Ok(new ApiResponse<object>(true, emailSettings, "Configuración de email obtenida"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener configuración de email");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al obtener configuración de email"));
        }
    }

    /// <summary>
    /// Enviar email de prueba para verificar configuración
    /// </summary>
    [HttpPost("test-email")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> TestEmail([FromServices] IEmailService emailService, [FromBody] TestEmailRequest request)
    {
        try
        {
            var resultado = await emailService.EnviarEmailAsync(
                request.EmailDestino,
                "✅ Prueba de Email - IncidentesFISEI",
                $@"
                <h2>🎉 ¡Email de Prueba Exitoso!</h2>
                <p>Este es un email de prueba del sistema <strong>IncidentesFISEI</strong>.</p>
                <p>Si estás leyendo esto, significa que la configuración de email está funcionando correctamente.</p>
                <hr>
                <p><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
                <p><strong>Sistema:</strong> IncidentesFISEI - UTA</p>
                <p><strong>Servidor SMTP:</strong> {request.SmtpInfo}</p>
                "
            );

            if (resultado)
            {
                return Ok(new ApiResponse<object>(true, new { enviado = true, destino = request.EmailDestino }, "Email de prueba enviado correctamente"));
            }
            else
            {
                return Ok(new ApiResponse<object>(false, null, "No se pudo enviar el email de prueba"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email de prueba");
            return StatusCode(500, new ApiResponse<object>(false, null, $"Error al enviar email: {ex.Message}"));
        }
    }

    #endregion

    #region Salud del API

    /// <summary>
    /// Verificar que el API de configuración está funcionando
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public IActionResult Health()
    {
        return Ok(new ApiResponse<object>(true, new { status = "OK", timestamp = DateTime.UtcNow }, "API de configuración funcionando"));
    }

    #endregion
}

public record TestEmailRequest(string EmailDestino, string SmtpInfo);
