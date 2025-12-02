using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Application.DTOs;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

namespace IncidentesFISEI.Api.Controllers;

/// <summary>
/// Controlador para asignación de incidentes según SPOC
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AsignacionController : ControllerBase
{
    private readonly IAsignacionService _asignacionService;
    private readonly ILogger<AsignacionController> _logger;

    public AsignacionController(
        IAsignacionService asignacionService,
        ILogger<AsignacionController> logger)
    {
        _asignacionService = asignacionService;
        _logger = logger;
    }

    /// <summary>
    /// Asignar incidente automáticamente al técnico con menor carga
    /// </summary>
    /// <param name="incidenteId">ID del incidente</param>
    [HttpPost("auto-asignar/{incidenteId}")]
    [SwaggerOperation(
        Summary = "Asignación automática",
        Description = "Asigna automáticamente al técnico con menor carga si no hay SPOC disponible"
    )]
    [SwaggerResponse(200, "Incidente asignado exitosamente")]
    [SwaggerResponse(400, "No se pudo asignar el incidente")]
    [SwaggerResponse(401, "No autorizado")]
    public async Task<ActionResult<ApiResponse<bool>>> AutoAsignar(int incidenteId)
    {
        try
        {
            var result = await _asignacionService.AsignarIncidenteAutomaticamenteAsync(incidenteId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en auto-asignación");
            return StatusCode(500, new ApiResponse<bool>(
                false, false, "Error interno del servidor"
            ));
        }
    }

    /// <summary>
    /// Asignar incidente manualmente a un técnico específico (Solo SPOC/Admin)
    /// </summary>
    /// <param name="incidenteId">ID del incidente</param>
    /// <param name="tecnicoId">ID del técnico</param>
    [HttpPost("asignar-manual/{incidenteId}/tecnico/{tecnicoId}")]
    [SwaggerOperation(
        Summary = "Asignación manual",
        Description = "SPOC o Admin asigna manualmente un incidente a un técnico específico"
    )]
    [SwaggerResponse(200, "Incidente asignado exitosamente")]
    [SwaggerResponse(403, "No tiene permisos para asignar")]
    public async Task<ActionResult<ApiResponse<bool>>> AsignarManualmente(
        int incidenteId,
        int tecnicoId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _asignacionService.AsignarIncidenteManualmenteAsync(
                incidenteId,
                tecnicoId,
                userId
            );
            
            if (!result.Success)
            {
                return result.Message.Contains("permisos")
                    ? StatusCode(403, result)
                    : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en asignación manual");
            return StatusCode(500, new ApiResponse<bool>(
                false, false, "Error interno del servidor"
            ));
        }
    }

    /// <summary>
    /// Obtener carga de trabajo de todos los técnicos
    /// </summary>
    [HttpGet("carga-trabajo")]
    [SwaggerOperation(
        Summary = "Carga de trabajo",
        Description = "Obtiene la carga de trabajo actual de todos los técnicos"
    )]
    [SwaggerResponse(200, "Carga de trabajo obtenida")]
    public async Task<ActionResult<Dictionary<int, int>>> GetCargaTrabajo()
    {
        try
        {
            var carga = await _asignacionService.GetCargaTrabajoTecnicosAsync();
            return Ok(carga);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener carga de trabajo");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Configurar usuario como SPOC (Solo Admin)
    /// </summary>
    /// <param name="usuarioId">ID del usuario</param>
    /// <param name="isSPOC">True para asignar SPOC, False para quitar</param>
    [HttpPost("configurar-spoc/{usuarioId}")]
    [Authorize(Roles = "Administrador")]
    [SwaggerOperation(
        Summary = "Configurar SPOC",
        Description = "Asigna o quita el rol de SPOC a un usuario (Solo Admin)"
    )]
    [SwaggerResponse(200, "SPOC configurado exitosamente")]
    [SwaggerResponse(403, "Solo administradores")]
    public async Task<ActionResult<ApiResponse<bool>>> ConfigurarSPOC(
        int usuarioId,
        [FromQuery] bool isSPOC)
    {
        try
        {
            var result = await _asignacionService.SetSPOCAsync(usuarioId, isSPOC);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al configurar SPOC");
            return StatusCode(500, new ApiResponse<bool>(
                false, false, "Error interno del servidor"
            ));
        }
    }

    /// <summary>
    /// Marcar disponibilidad del SPOC
    /// </summary>
    /// <param name="isAvailable">True si está disponible, False si no</param>
    [HttpPost("spoc/disponibilidad")]
    [SwaggerOperation(
        Summary = "Disponibilidad SPOC",
        Description = "El SPOC marca si está disponible o no para asignar incidentes"
    )]
    [SwaggerResponse(200, "Disponibilidad actualizada")]
    public async Task<ActionResult<ApiResponse<bool>>> SetDisponibilidad(
        [FromQuery] bool isAvailable)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _asignacionService.SetSPOCAvailabilityAsync(userId, isAvailable);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar disponibilidad");
            return StatusCode(500, new ApiResponse<bool>(
                false, false, "Error interno del servidor"
            ));
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }
}
