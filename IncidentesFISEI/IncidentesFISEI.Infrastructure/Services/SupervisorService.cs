using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Domain.Interfaces;

namespace IncidentesFISEI.Infrastructure.Services;

public interface ISupervisorService
{
    Task<SupervisorDashboardDto> GetDashboardAsync(int supervisorId);
    Task<List<IncidenteDto>> GetCriticalIncidentsAsync(int supervisorId);
    Task<List<TechnicianMetricsDto>> GetTeamMetricsAsync(int supervisorId);
    Task<List<SlaMetricDto>> GetSlaMetricsAsync(int supervisorId);
    Task<List<ApprovalItemDto>> GetPendingApprovalsAsync(int supervisorId);
    Task<bool> EscalateIncidentAsync(int incidentId, int supervisorId);
    Task<bool> ReassignIncidentAsync(int incidentId, int newTechnicianId, int supervisorId);
}

public class SupervisorService : ISupervisorService
{
    private readonly IIncidenteRepository _incidenteRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly INotificationService _notificationService;

    public SupervisorService(
        IIncidenteRepository incidenteRepository,
        IUsuarioRepository usuarioRepository,
        INotificationService notificationService)
    {
        _incidenteRepository = incidenteRepository;
        _usuarioRepository = usuarioRepository;
        _notificationService = notificationService;
    }

    public async Task<SupervisorDashboardDto> GetDashboardAsync(int supervisorId)
    {
        var supervisor = await _usuarioRepository.GetByIdAsync(supervisorId);
        if (supervisor == null || supervisor.TipoUsuario != TipoUsuario.Supervisor)
            return new SupervisorDashboardDto();

        var dashboard = new SupervisorDashboardDto
        {
            CriticalIncidents = 0,
            ActiveTechnicians = 0,
            TotalWorkload = 0,
            OverallSlaCompliance = 0,
            PendingApprovals = 0,
            CriticalAlerts = new List<CriticalAlertDto>(),
            TeamMembers = new List<TeamMemberDto>(),
            SlaByCategory = new List<SlaCategoryDto>(),
            PendingApprovalItems = new List<ApprovalItemDto>()
        };

        try
        {
            // Obtener todos los incidentes
            var allIncidents = await _incidenteRepository.GetAllAsync();
            
            // Incidentes críticos (estado abierto/en progreso y prioridad alta/crítica)
            var criticalIncidents = allIncidents
                .Where(i => (i.Estado == EstadoIncidente.Abierto || 
                            i.Estado == EstadoIncidente.EnProgreso ||
                            i.Estado == EstadoIncidente.Escalado) &&
                           (i.Prioridad == PrioridadIncidente.Alta || 
                            i.Prioridad == PrioridadIncidente.Critica))
                .ToList();

            dashboard.CriticalIncidents = criticalIncidents.Count;

            // Obtener equipos de técnicos (todos los técnicos activos)
            var technicians = (await _usuarioRepository.GetAllAsync())
                .Where(u => u.TipoUsuario == TipoUsuario.Tecnico && u.IsActive)
                .ToList();

            dashboard.ActiveTechnicians = technicians.Count;

            // Calcular total de incidentes asignados
            var assignedIncidents = allIncidents
                .Where(i => i.AsignadoAId.HasValue && 
                           technicians.Any(t => t.Id == i.AsignadoAId))
                .ToList();

            dashboard.TotalWorkload = assignedIncidents.Count;

            // Alertas críticas
            dashboard.CriticalAlerts = criticalIncidents
                .Take(5)
                .Select(i => new CriticalAlertDto
                {
                    Id = i.Id,
                    Title = i.NumeroIncidente,
                    Description = i.Titulo,
                    TimeRemaining = CalculateTimeRemaining(i),
                    AssignedTo = i.AsignadoA?.FirstName ?? "Sin asignar"
                })
                .ToList();

            // Métricas del equipo
            dashboard.TeamMembers = technicians
                .Select(t => new TeamMemberDto
                {
                    Id = t.Id,
                    Name = $"{t.FirstName} {t.LastName}",
                    Specialty = "Técnico de Sistemas",
                    Status = GetTechnicianStatus(t),
                    AssignedIncidents = assignedIncidents.Count(i => i.AsignadoAId == t.Id),
                    ResolvedToday = allIncidents.Count(i => i.AsignadoAId == t.Id && 
                                                           i.Estado == EstadoIncidente.Cerrado &&
                                                           i.UpdatedAt?.Date == DateTime.UtcNow.Date),
                    AvgResolutionTime = TimeSpan.FromHours(2.5)
                })
                .ToList();

            // Cumplimiento SLA general
            var slaCompliant = allIncidents.Count(i => i.Estado == EstadoIncidente.Cerrado);
            dashboard.OverallSlaCompliance = allIncidents.Count() > 0 ? 
                ((double)slaCompliant / allIncidents.Count()) * 100 : 0;

            // Métricas SLA por categoría
            var categories = allIncidents
                .GroupBy(i => i.Categoria)
                .Select(g => new SlaCategoryDto
                {
                    Name = g.Key?.Nombre ?? "Sin categoría",
                    Compliance = g.Count(i => i.Estado == EstadoIncidente.Cerrado) > 0 ?
                        ((double)g.Count(i => i.Estado == EstadoIncidente.Cerrado) / g.Count()) * 100 : 0,
                    OnTime = g.Count(i => i.Estado == EstadoIncidente.Cerrado),
                    Overdue = g.Count(i => i.FechaVencimiento < DateTime.UtcNow && 
                                           i.Estado != EstadoIncidente.Cerrado),
                    NearDeadline = g.Count(i => i.FechaVencimiento <= DateTime.UtcNow.AddHours(24) && 
                                                 i.FechaVencimiento > DateTime.UtcNow &&
                                                 i.Estado != EstadoIncidente.Cerrado)
                })
                .ToList();

            dashboard.SlaByCategory = categories;
            dashboard.PendingApprovals = 0;

            return dashboard;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en GetDashboardAsync: {ex.Message}");
            return dashboard;
        }
    }

    public async Task<List<IncidenteDto>> GetCriticalIncidentsAsync(int supervisorId)
    {
        var supervisor = await _usuarioRepository.GetByIdAsync(supervisorId);
        if (supervisor == null || supervisor.TipoUsuario != TipoUsuario.Supervisor)
            return new List<IncidenteDto>();

        var allIncidents = await _incidenteRepository.GetAllAsync();
        
        // Para ahora, devolvemos solo una lista simple sin mapeo completo
        // en producción esto debería usar AutoMapper
        var result = new List<IncidenteDto>();
        foreach (var incident in allIncidents
            .Where(i => (i.Estado == EstadoIncidente.Abierto || 
                        i.Estado == EstadoIncidente.EnProgreso ||
                        i.Estado == EstadoIncidente.Escalado) &&
                       (i.Prioridad == PrioridadIncidente.Alta || 
                        i.Prioridad == PrioridadIncidente.Critica)))
        {
            result.Add(new IncidenteDto
            {
                Id = incident.Id,
                NumeroIncidente = incident.NumeroIncidente,
                Titulo = incident.Titulo,
                Descripcion = incident.Descripcion,
                Estado = incident.Estado,
                Prioridad = incident.Prioridad,
                Impacto = incident.Impacto,
                Urgencia = incident.Urgencia,
                FechaReporte = incident.FechaReporte,
                FechaAsignacion = incident.FechaAsignacion,
                FechaResolucion = incident.FechaResolucion,
                FechaCierre = incident.FechaCierre,
                FechaVencimiento = incident.FechaVencimiento,
                Solucion = incident.Solucion,
                CausaRaiz = incident.CausaRaiz,
                PasosReproducir = incident.PasosReproducir,
                ActivosAfectados = incident.ActivosAfectados,
                NumeroComentarios = incident.Comentarios?.Count ?? 0,
                NumeroArchivos = incident.ArchivosAdjuntos?.Count ?? 0
            });
        }
        return result;
    }

    public async Task<List<TechnicianMetricsDto>> GetTeamMetricsAsync(int supervisorId)
    {
        var supervisor = await _usuarioRepository.GetByIdAsync(supervisorId);
        if (supervisor == null || supervisor.TipoUsuario != TipoUsuario.Supervisor)
            return new List<TechnicianMetricsDto>();

        var technicians = (await _usuarioRepository.GetAllAsync())
            .Where(u => u.TipoUsuario == TipoUsuario.Tecnico && u.IsActive)
            .ToList();

        var allIncidents = await _incidenteRepository.GetAllAsync();

        return technicians
            .Select(t => new TechnicianMetricsDto
            {
                Id = t.Id,
                Name = $"{t.FirstName} {t.LastName}",
                Specialty = "Técnico",
                Status = GetTechnicianStatus(t).ToString(),
                AssignedIncidents = allIncidents.Count(i => i.AsignadoAId == t.Id),
                ResolvedToday = allIncidents.Count(i => i.AsignadoAId == t.Id && 
                                                       i.Estado == EstadoIncidente.Cerrado &&
                                                       i.UpdatedAt?.Date == DateTime.UtcNow.Date),
                AvgResolutionTime = TimeSpan.FromHours(2.5)
            })
            .ToList();
    }

    public async Task<List<SlaMetricDto>> GetSlaMetricsAsync(int supervisorId)
    {
        var supervisor = await _usuarioRepository.GetByIdAsync(supervisorId);
        if (supervisor == null || supervisor.TipoUsuario != TipoUsuario.Supervisor)
            return new List<SlaMetricDto>();

        var allIncidents = await _incidenteRepository.GetAllAsync();

        return allIncidents
            .GroupBy(i => i.Categoria?.Nombre ?? "Sin categoría")
            .Select(g => new SlaMetricDto
            {
                Category = g.Key,
                Compliance = g.Count() > 0 ? 
                    ((double)g.Where(i => i.Estado == EstadoIncidente.Cerrado).Count() / g.Count()) * 100 : 0,
                OnTime = g.Count(i => i.Estado == EstadoIncidente.Cerrado),
                Overdue = g.Count(i => i.FechaVencimiento < DateTime.UtcNow && 
                                       i.Estado != EstadoIncidente.Cerrado)
            })
            .ToList();
    }

    public async Task<List<ApprovalItemDto>> GetPendingApprovalsAsync(int supervisorId)
    {
        return new List<ApprovalItemDto>();
    }

    public async Task<bool> EscalateIncidentAsync(int incidentId, int supervisorId)
    {
        var supervisor = await _usuarioRepository.GetByIdAsync(supervisorId);
        if (supervisor == null || supervisor.TipoUsuario != TipoUsuario.Supervisor)
            return false;

        var incidente = await _incidenteRepository.GetByIdAsync(incidentId);
        if (incidente == null)
            return false;

        incidente.Estado = EstadoIncidente.Escalado;
        incidente.UpdatedAt = DateTime.UtcNow;
        
        await _incidenteRepository.UpdateAsync(incidente);

        return true;
    }

    public async Task<bool> ReassignIncidentAsync(int incidentId, int newTechnicianId, int supervisorId)
    {
        var supervisor = await _usuarioRepository.GetByIdAsync(supervisorId);
        if (supervisor == null || supervisor.TipoUsuario != TipoUsuario.Supervisor)
            return false;

        var incidente = await _incidenteRepository.GetByIdAsync(incidentId);
        if (incidente == null)
            return false;

        var newTechnician = await _usuarioRepository.GetByIdAsync(newTechnicianId);
        if (newTechnician == null || newTechnician.TipoUsuario != TipoUsuario.Tecnico)
            return false;

        incidente.AsignadoAId = newTechnicianId;
        incidente.UpdatedAt = DateTime.UtcNow;

        await _incidenteRepository.UpdateAsync(incidente);

        return true;
    }

    private TimeSpan CalculateTimeRemaining(Incidente incidente)
    {
        return incidente.FechaVencimiento.HasValue ? 
            incidente.FechaVencimiento.Value - DateTime.UtcNow : 
            TimeSpan.Zero;
    }

    private TechnicianStatusEnum GetTechnicianStatus(Usuario technician)
    {
        if (technician.LastLoginAt.HasValue && 
            (DateTime.UtcNow - technician.LastLoginAt.Value).TotalMinutes < 5)
            return TechnicianStatusEnum.Available;
        
        return TechnicianStatusEnum.Offline;
    }
}

public class SupervisorDashboardDto
{
    public int CriticalIncidents { get; set; }
    public int ActiveTechnicians { get; set; }
    public int TotalWorkload { get; set; }
    public double OverallSlaCompliance { get; set; }
    public int PendingApprovals { get; set; }
    public List<CriticalAlertDto> CriticalAlerts { get; set; } = new();
    public List<TeamMemberDto> TeamMembers { get; set; } = new();
    public List<SlaCategoryDto> SlaByCategory { get; set; } = new();
    public List<ApprovalItemDto> PendingApprovalItems { get; set; } = new();
}

public class CriticalAlertDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public TimeSpan TimeRemaining { get; set; }
    public string AssignedTo { get; set; } = "";
}

public class TeamMemberDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Specialty { get; set; } = "";
    public TechnicianStatusEnum Status { get; set; }
    public int AssignedIncidents { get; set; }
    public int ResolvedToday { get; set; }
    public TimeSpan AvgResolutionTime { get; set; }
}

public class SlaCategoryDto
{
    public string Name { get; set; } = "";
    public double Compliance { get; set; }
    public int OnTime { get; set; }
    public int Overdue { get; set; }
    public int NearDeadline { get; set; }
}

public class SlaMetricDto
{
    public string Category { get; set; } = "";
    public double Compliance { get; set; }
    public int OnTime { get; set; }
    public int Overdue { get; set; }
}

public class ApprovalItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string RequestedBy { get; set; } = "";
    public DateTime RequestedAt { get; set; }
    public string Type { get; set; } = "";
}

public class TechnicianMetricsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Specialty { get; set; } = "";
    public string Status { get; set; } = "";
    public int AssignedIncidents { get; set; }
    public int ResolvedToday { get; set; }
    public TimeSpan AvgResolutionTime { get; set; }
}

public enum TechnicianStatusEnum
{
    Available,
    Busy,
    Offline
}

