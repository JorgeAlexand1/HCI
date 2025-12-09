using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IncidentesFISEI.Infrastructure.Services;
using System.Security.Claims;

namespace IncidentesFISEI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SupervisorTecnico")]
public class SupervisorController : ControllerBase
{
    private readonly ISupervisorService _supervisorService;
    private readonly ILogger<SupervisorController> _logger;

    public SupervisorController(ISupervisorService supervisorService, ILogger<SupervisorController> logger)
    {
        _supervisorService = supervisorService;
        _logger = logger;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var supervisorId = GetUserId();
            if (supervisorId == 0)
                return Unauthorized(new { success = false, message = "Token inválido" });

            var dashboard = await _supervisorService.GetDashboardAsync(supervisorId);
            return Ok(new { success = true, data = dashboard, message = "Dashboard cargado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener dashboard del supervisor");
            return StatusCode(500, new { success = false, message = "Error interno del servidor" });
        }
    }

    [HttpGet("critical-incidents")]
    public async Task<IActionResult> GetCriticalIncidents()
    {
        try
        {
            var supervisorId = GetUserId();
            if (supervisorId == 0)
                return Unauthorized(new { success = false, message = "Token inválido" });

            var incidents = await _supervisorService.GetCriticalIncidentsAsync(supervisorId);
            return Ok(new { success = true, data = incidents, message = "Incidentes críticos obtenidos" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener incidentes críticos");
            return StatusCode(500, new { success = false, message = "Error interno del servidor" });
        }
    }

    [HttpGet("team-metrics")]
    public async Task<IActionResult> GetTeamMetrics()
    {
        try
        {
            var supervisorId = GetUserId();
            if (supervisorId == 0)
                return Unauthorized(new { success = false, message = "Token inválido" });

            var metrics = await _supervisorService.GetTeamMetricsAsync(supervisorId);
            return Ok(new { success = true, data = metrics, message = "Métricas del equipo obtenidas" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener métricas del equipo");
            return StatusCode(500, new { success = false, message = "Error interno del servidor" });
        }
    }

    [HttpGet("sla-metrics")]
    public async Task<IActionResult> GetSlaMetrics()
    {
        try
        {
            var supervisorId = GetUserId();
            if (supervisorId == 0)
                return Unauthorized(new { success = false, message = "Token inválido" });

            var metrics = await _supervisorService.GetSlaMetricsAsync(supervisorId);
            return Ok(new { success = true, data = metrics, message = "Métricas SLA obtenidas" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener métricas SLA");
            return StatusCode(500, new { success = false, message = "Error interno del servidor" });
        }
    }

    [HttpGet("pending-approvals")]
    public async Task<IActionResult> GetPendingApprovals()
    {
        try
        {
            var supervisorId = GetUserId();
            if (supervisorId == 0)
                return Unauthorized(new { success = false, message = "Token inválido" });

            var approvals = await _supervisorService.GetPendingApprovalsAsync(supervisorId);
            return Ok(new { success = true, data = approvals, message = "Aprobaciones pendientes obtenidas" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener aprobaciones pendientes");
            return StatusCode(500, new { success = false, message = "Error interno del servidor" });
        }
    }

    [HttpPost("escalate/{incidentId}")]
    public async Task<IActionResult> EscalateIncident(int incidentId)
    {
        try
        {
            var supervisorId = GetUserId();
            if (supervisorId == 0)
                return Unauthorized(new { success = false, message = "Token inválido" });

            var result = await _supervisorService.EscalateIncidentAsync(incidentId, supervisorId);
            
            if (result)
                return Ok(new { success = true, message = "Incidente escalado exitosamente" });
            
            return BadRequest(new { success = false, message = "No se pudo escalar el incidente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al escalar incidente {IncidentId}", incidentId);
            return StatusCode(500, new { success = false, message = "Error interno del servidor" });
        }
    }

    [HttpPost("reassign/{incidentId}")]
    public async Task<IActionResult> ReassignIncident(int incidentId, [FromQuery] int newTechnicianId)
    {
        try
        {
            var supervisorId = GetUserId();
            if (supervisorId == 0)
                return Unauthorized(new { success = false, message = "Token inválido" });

            var result = await _supervisorService.ReassignIncidentAsync(incidentId, newTechnicianId, supervisorId);
            
            if (result)
                return Ok(new { success = true, message = "Incidente reasignado exitosamente" });
            
            return BadRequest(new { success = false, message = "No se pudo reasignar el incidente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reasignar incidente {IncidentId}", incidentId);
            return StatusCode(500, new { success = false, message = "Error interno del servidor" });
        }
    }

}
