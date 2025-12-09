using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IncidentesFISEI.Infrastructure.Data;
using System.Security.Claims;

namespace IncidentesFISEI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize] // Temporalmente deshabilitado para testing
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Obtener estadísticas generales del dashboard de administración
    /// </summary>
    [HttpGet("admin-stats")]
    //[Authorize(Roles = "Administrador")] // Temporalmente deshabilitado para testing
    [ProducesResponseType(typeof(AdminDashboardStatsDto), 200)]
    public async Task<IActionResult> GetAdminStats()
    {
        try
        {
            var stats = new AdminDashboardStatsDto
            {
                // Estadísticas de usuarios
                TotalUsuarios = await _context.Usuarios.CountAsync(u => u.IsActive),
                UsuariosActivos = await _context.Usuarios.CountAsync(u => u.IsActive && u.LastLoginAt > DateTime.UtcNow.AddDays(-30)),
                UsuariosPendientes = await _context.Usuarios.CountAsync(u => !u.IsEmailConfirmed),
                AdminsCount = await _context.Usuarios.CountAsync(u => u.TipoUsuario == TipoUsuario.Administrador && u.IsActive),
                TecnicosCount = await _context.Usuarios.CountAsync(u => u.TipoUsuario == TipoUsuario.Tecnico && u.IsActive),
                // Separar docentes y estudiantes: docentes tienen "docente" en email, el resto son estudiantes
                DocentesCount = await _context.Usuarios.CountAsync(u => u.TipoUsuario == TipoUsuario.Usuario && u.IsActive && u.Email.Contains("docente")),
                EstudiantesCount = await _context.Usuarios.CountAsync(u => u.TipoUsuario == TipoUsuario.Usuario && u.IsActive && !u.Email.Contains("docente")),

                // Estadísticas de incidentes
                TotalIncidentes = await _context.Incidentes.CountAsync(),
                IncidentesAbiertos = await _context.Incidentes.CountAsync(i => i.Estado == EstadoIncidente.Abierto),
                IncidentesEnProgreso = await _context.Incidentes.CountAsync(i => i.Estado == EstadoIncidente.EnProgreso),
                IncidentesCriticos = await _context.Incidentes.CountAsync(i => i.Prioridad == PrioridadIncidente.Critica),
                IncidentesHoy = await _context.Incidentes.CountAsync(i => i.FechaReporte.Date == DateTime.UtcNow.Date),
                
                // Estadísticas por prioridad
                IncidentesAltaPrioridad = await _context.Incidentes.CountAsync(i => i.Prioridad == PrioridadIncidente.Alta),
                IncidentesMediaPrioridad = await _context.Incidentes.CountAsync(i => i.Prioridad == PrioridadIncidente.Media),
                IncidentesBajaPrioridad = await _context.Incidentes.CountAsync(i => i.Prioridad == PrioridadIncidente.Baja),

                // SLA y rendimiento
                SlaActivos = 8, // Esto podría venir de una tabla de configuración
                CumplimientoSla = await CalculateSlaCompliance(),
                
                // Reportes
                ReportesHoy = await _context.RegistrosTiempo
                    .Where(r => r.FechaInicio.Date == DateTime.UtcNow.Date)
                    .CountAsync(),

                // Categorías activas
                CategoriasActivas = await _context.Categorias.CountAsync(c => c.IsActive),
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas del dashboard");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtener lista de usuarios para administración
    /// </summary>
    [HttpGet("users")]
    //[Authorize(Roles = "Administrador")] // Temporalmente deshabilitado
    [ProducesResponseType(typeof(List<UsuarioListDto>), 200)]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var users = await _context.Usuarios
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new UsuarioListDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    FirstName = u.FirstName ?? string.Empty,
                    LastName = u.LastName ?? string.Empty,
                    Email = u.Email,
                    Phone = u.Phone,
                    TipoUsuario = u.TipoUsuario,
                    Department = u.Department,
                    IsActive = u.IsActive,
                    IsEmailConfirmed = u.IsEmailConfirmed,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt
                })
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lista de usuarios");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtener incidentes recientes
    /// </summary>
    [HttpGet("recent-incidents")]
    //[Authorize(Roles = "Administrador,Supervisor,Tecnico")] // Temporalmente deshabilitado
    [ProducesResponseType(typeof(List<IncidenteListDto>), 200)]
    public async Task<IActionResult> GetRecentIncidents([FromQuery] int count = 10)
    {
        try
        {
            var incidents = await _context.Incidentes
                .Include(i => i.ReportadoPor)
                .Include(i => i.AsignadoA)
                .Include(i => i.Categoria)
                .Include(i => i.Servicio)
                .OrderByDescending(i => i.FechaReporte)
                .Take(count)
                .Select(i => new IncidenteListDto
                {
                    Id = i.Id,
                    NumeroIncidente = i.NumeroIncidente,
                    Titulo = i.Titulo,
                    Descripcion = i.Descripcion,
                    Estado = i.Estado,
                    Prioridad = i.Prioridad,
                    FechaReporte = i.FechaReporte,
                    ReportadoPor = i.ReportadoPor.FirstName + " " + i.ReportadoPor.LastName,
                    ReportadoPorId = i.ReportadoPorId,
                    AsignadoA = i.AsignadoA != null ? i.AsignadoA.FirstName + " " + i.AsignadoA.LastName : null,
                    AsignadoAId = i.AsignadoAId,
                    CategoriaNombre = i.Categoria.Nombre,
                    CategoriaId = i.CategoriaId,
                    ServicioNombre = i.Servicio != null ? i.Servicio.Nombre : null,
                    ServicioId = i.ServicioId,
                    FechaResolucion = i.FechaResolucion
                })
                .ToListAsync();

            return Ok(incidents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener incidentes recientes");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtener datos del dashboard de supervisor técnico
    /// </summary>
    [HttpGet("supervisor")]
    //[Authorize(Roles = "SupervisorTecnico")] // Temporalmente deshabilitado para testing
    [ProducesResponseType(typeof(DashboardDataDto), 200)]
    public async Task<IActionResult> GetSupervisorDashboard()
    {
        try
        {
            // Obtener todos los técnicos activos
            var tecnicos = await _context.Usuarios
                .Where(u => u.TipoUsuario == TipoUsuario.Tecnico && u.IsActive)
                .Select(u => new
                {
                    u.Id,
                    Nombre = (u.FirstName ?? "") + " " + (u.LastName ?? ""),
                    Email = u.Email,
                    IncidentesAsignados = _context.Incidentes
                        .Count(i => i.AsignadoAId == u.Id && 
                               (i.Estado == EstadoIncidente.Abierto || 
                                i.Estado == EstadoIncidente.EnProgreso || 
                                i.Estado == EstadoIncidente.EnEspera)),
                    ResueltosHoy = _context.Incidentes
                        .Count(i => i.AsignadoAId == u.Id && 
                               i.Estado == EstadoIncidente.Resuelto && 
                               i.FechaResolucion != null && 
                               i.FechaResolucion.Value.Date == DateTime.UtcNow.Date)
                })
                .ToListAsync();

            // Calcular estadísticas generales
            var incidentesCriticos = await _context.Incidentes
                .CountAsync(i => i.Prioridad == PrioridadIncidente.Critica && 
                            (i.Estado == EstadoIncidente.Abierto || i.Estado == EstadoIncidente.EnProgreso));

            var totalWorkload = await _context.Incidentes
                .CountAsync(i => i.AsignadoAId != null && 
                            (i.Estado == EstadoIncidente.Abierto || 
                             i.Estado == EstadoIncidente.EnProgreso || 
                             i.Estado == EstadoIncidente.EnEspera));

            var dashboardData = new DashboardDataDto
            {
                ActiveTechnicians = tecnicos.Count,
                CriticalIncidents = incidentesCriticos,
                TotalWorkload = totalWorkload,
                TeamMembers = tecnicos.Select(t => new TechnicianDto
                {
                    Name = t.Nombre.Trim(),
                    Specialty = DetermineSpecialty(t.Email),
                    Status = DetermineStatus(t.IncidentesAsignados),
                    AssignedIncidents = t.IncidentesAsignados,
                    ResolvedToday = t.ResueltosHoy
                }).ToList()
            };

            return Ok(dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener datos del dashboard de supervisor");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    private string DetermineSpecialty(string email)
    {
        // Lógica para determinar la especialidad basada en el email o datos del técnico
        // Por ahora, asignamos una especialidad genérica
        if (email.Contains("redes") || email.Contains("network"))
            return "Redes y Conectividad";
        else if (email.Contains("hardware"))
            return "Hardware y Equipos";
        else if (email.Contains("software") || email.Contains("dev"))
            return "Software y Aplicaciones";
        else if (email.Contains("security") || email.Contains("seguridad"))
            return "Seguridad Informática";
        else if (email.Contains("database") || email.Contains("bd"))
            return "Base de Datos";
        else
            return "Soporte Técnico General";
    }

    private string DetermineStatus(int incidentesAsignados)
    {
        // Determinar el estado del técnico basado en su carga de trabajo
        if (incidentesAsignados == 0)
            return "Disponible";
        else if (incidentesAsignados <= 3)
            return "Ocupado";
        else
            return "Ocupado";
    }

    [HttpGet("supervisor/incidentes")]
    [ProducesResponseType(typeof(List<IncidenteListDto>), 200)]
    public async Task<IActionResult> GetSupervisorIncidentes()
    {
        try
        {
            var supervisorId = GetUserId();
            if (supervisorId == 0)
            {
                return Unauthorized("No se pudo identificar al usuario");
            }

            // Obtener todos los incidentes asignados al supervisor logueado
            var incidentes = await _context.Incidentes
                .Include(i => i.ReportadoPor)
                .Include(i => i.AsignadoA)
                .Include(i => i.Categoria)
                .Include(i => i.Servicio)
                .Include(i => i.ConfiguracionSLA)
                .Where(i => i.AsignadoAId == supervisorId)
                .OrderByDescending(i => i.FechaReporte)
                .Select(i => new IncidenteListDto
                {
                    Id = i.Id,
                    NumeroIncidente = i.NumeroIncidente,
                    Titulo = i.Titulo,
                    Descripcion = i.Descripcion,
                    Estado = i.Estado,
                    Prioridad = i.Prioridad,
                    Impacto = i.Impacto,
                    Urgencia = i.Urgencia,
                    FechaReporte = i.FechaReporte,
                    FechaAsignacion = i.FechaAsignacion,
                    FechaResolucion = i.FechaResolucion,
                    ReportadoPor = $"{i.ReportadoPor.FirstName} {i.ReportadoPor.LastName}".Trim(),
                    ReportadoPorId = i.ReportadoPorId,
                    AsignadoA = i.AsignadoA != null ? $"{i.AsignadoA.FirstName} {i.AsignadoA.LastName}".Trim() : null,
                    AsignadoAId = i.AsignadoAId,
                    CategoriaNombre = i.Categoria.Nombre,
                    CategoriaId = i.CategoriaId,
                    ServicioNombre = i.Servicio != null ? i.Servicio.Nombre : null,
                    ServicioId = i.ServicioId,
                    Solucion = i.Solucion,
                    ConfiguracionSLAId = i.ConfiguracionSLAId,
                    SLANombre = i.ConfiguracionSLA != null ? i.ConfiguracionSLA.Nombre : null,
                    FechaVencimiento = i.FechaVencimiento
                })
                .ToListAsync();

            return Ok(incidentes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener incidentes del supervisor");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    private async Task<double> CalculateSlaCompliance()
    {
        try
        {
            // Lógica básica de SLA - podría ser más compleja según los requerimientos
            var totalIncidentes = await _context.Incidentes.CountAsync();
            if (totalIncidentes == 0) return 100.0;

            var incidentesEnTiempo = await _context.Incidentes
                .CountAsync(i => i.FechaResolucion <= i.FechaVencimiento || i.FechaVencimiento == null);

            return Math.Round((incidentesEnTiempo / (double)totalIncidentes) * 100, 2);
        }
        catch
        {
            return 0.0;
        }
    }
}