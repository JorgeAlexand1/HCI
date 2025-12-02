using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IncidentesFISEI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AuditLogController> _logger;

    public AuditLogController(IAuditLogService auditLogService, ILogger<AuditLogController> logger)
    {
        _auditLogService = auditLogService;
        _logger = logger;
    }

    private int GetUsuarioId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    private string? GetDireccionIP()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private string? GetUserAgent()
    {
        return Request.Headers["User-Agent"].ToString();
    }

    [HttpGet("mis-logs")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AuditLogDto>>>> GetMisLogs(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            var logs = await _auditLogService.GetLogsByUsuarioAsync(usuarioId, skip, take);
            return Ok(new ApiResponse<IEnumerable<AuditLogDto>>(true, logs, "Logs obtenidos"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener logs del usuario");
            return StatusCode(500, new ApiResponse<IEnumerable<AuditLogDto>>(false, null, "Error al obtener logs"));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<AuditLogDetalladoDto>>> GetLogDetallado(int id)
    {
        try
        {
            var log = await _auditLogService.GetLogDetalladoAsync(id);
            if (log == null)
                return NotFound(new ApiResponse<AuditLogDetalladoDto>(false, null, "Log no encontrado"));

            return Ok(new ApiResponse<AuditLogDetalladoDto>(true, log, "Log obtenido"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener log {Id}", id);
            return StatusCode(500, new ApiResponse<AuditLogDetalladoDto>(false, null, "Error al obtener log"));
        }
    }

    [HttpGet("entidad/{tipoEntidad}/{entidadId}")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AuditLogDto>>>> GetLogsByEntidad(
        TipoEntidadAuditoria tipoEntidad,
        int entidadId)
    {
        try
        {
            var logs = await _auditLogService.GetLogsByEntidadAsync(tipoEntidad, entidadId);
            return Ok(new ApiResponse<IEnumerable<AuditLogDto>>(true, logs, $"Logs de {tipoEntidad} #{entidadId} obtenidos"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener logs de entidad {Tipo} {Id}", tipoEntidad, entidadId);
            return StatusCode(500, new ApiResponse<IEnumerable<AuditLogDto>>(false, null, "Error al obtener logs"));
        }
    }

    [HttpPost("buscar")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AuditLogDto>>>> BuscarLogs([FromBody] BuscarAuditLogsDto filtros)
    {
        try
        {
            var logs = await _auditLogService.BuscarLogsAsync(filtros);
            return Ok(new ApiResponse<IEnumerable<AuditLogDto>>(true, logs, "Búsqueda completada"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar logs");
            return StatusCode(500, new ApiResponse<IEnumerable<AuditLogDto>>(false, null, "Error al buscar logs"));
        }
    }

    [HttpGet("criticos")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AuditLogDto>>>> GetLogsCriticos(
        [FromQuery] DateTime? desde = null,
        [FromQuery] int take = 50)
    {
        try
        {
            var logs = await _auditLogService.GetLogsCriticosAsync(desde, take);
            return Ok(new ApiResponse<IEnumerable<AuditLogDto>>(true, logs, "Logs críticos obtenidos"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener logs críticos");
            return StatusCode(500, new ApiResponse<IEnumerable<AuditLogDto>>(false, null, "Error al obtener logs críticos"));
        }
    }

    [HttpGet("estadisticas")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<EstadisticasAuditoriaDto>>> GetEstadisticas(
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        try
        {
            var desdeDate = desde ?? DateTime.UtcNow.AddDays(-30);
            var hastaDate = hasta ?? DateTime.UtcNow;

            var estadisticas = await _auditLogService.GetEstadisticasAsync(desdeDate, hastaDate);
            return Ok(new ApiResponse<EstadisticasAuditoriaDto>(true, estadisticas, "Estadísticas obtenidas"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de auditoría");
            return StatusCode(500, new ApiResponse<EstadisticasAuditoriaDto>(false, null, "Error al obtener estadísticas"));
        }
    }

    [HttpPost("limpiar")]
    [Authorize(Policy = "Administrador")]
    public async Task<ActionResult<ApiResponse<int>>> LimpiarLogsAntiguos([FromQuery] int diasRetencion = 90)
    {
        try
        {
            var eliminados = await _auditLogService.LimpiarLogsAntiguosAsync(diasRetencion);

            // Registrar esta acción
            await _auditLogService.RegistrarAuditoriaAsync(new CreateAuditLogDto
            {
                UsuarioId = GetUsuarioId(),
                DireccionIP = GetDireccionIP(),
                UserAgent = GetUserAgent(),
                TipoAccion = TipoAccionAuditoria.Eliminacion,
                TipoEntidad = TipoEntidadAuditoria.Configuracion,
                Descripcion = $"Limpieza manual de logs: {eliminados} registros eliminados (retención: {diasRetencion} días)",
                NivelSeveridad = NivelSeveridadAuditoria.Alto,
                Modulo = "API",
                Endpoint = Request.Path
            });

            return Ok(new ApiResponse<int>(true, eliminados, $"{eliminados} logs eliminados exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al limpiar logs antiguos");
            return StatusCode(500, new ApiResponse<int>(false, 0, "Error al limpiar logs"));
        }
    }

    [HttpPost]
    [Authorize(Policy = "Administrador")]
    public async Task<ActionResult<ApiResponse<object>>> RegistrarLog([FromBody] CreateAuditLogDto dto)
    {
        try
        {
            // Agregar información de contexto
            dto.UsuarioId = dto.UsuarioId ?? GetUsuarioId();
            dto.DireccionIP = dto.DireccionIP ?? GetDireccionIP();
            dto.UserAgent = dto.UserAgent ?? GetUserAgent();
            dto.Modulo = dto.Modulo ?? "API";

            await _auditLogService.RegistrarAuditoriaAsync(dto);
            return Ok(new ApiResponse<object>(true, null, "Log registrado exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar log manual");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al registrar log"));
        }
    }
}
