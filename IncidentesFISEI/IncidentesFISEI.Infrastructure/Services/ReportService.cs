using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentesFISEI.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PerformanceReportDto> GetPerformanceReportAsync()
        {
            var incidents = await _context.Incidentes.ToListAsync();
            var resolvedIncidents = incidents.Count(i => i.Estado == EstadoIncidente.Cerrado);
            var pendingIncidents = incidents.Count(i => i.Estado == EstadoIncidente.Abierto);

            // Calcular tiempo promedio de resolución (en horas)
            var resolvedWithTime = incidents
                .Where(i => i.Estado == EstadoIncidente.Cerrado && i.FechaResolucion.HasValue)
                .ToList();

            double averageResolutionTime = 0;
            if (resolvedWithTime.Any())
            {
                averageResolutionTime = resolvedWithTime
                    .Average(i => (i.FechaResolucion.Value - i.FechaReporte).TotalHours);
            }

            var resolutionRate = incidents.Any() ? (resolvedIncidents * 100.0) / incidents.Count : 0;

            return new PerformanceReportDto
            {
                TotalIncidents = incidents.Count,
                ResolvedIncidents = resolvedIncidents,
                PendingIncidents = pendingIncidents,
                AverageResolutionTime = Math.Round(averageResolutionTime, 2),
                ResolutionRate = Math.Round(resolutionRate, 2),
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<SLAReportDto> GetSLAReportAsync()
        {
            // Obtener todos los incidentes
            var incidents = await _context.Incidentes.ToListAsync();

            int slaMet = 0;
            int slaBreached = 0;
            int totalWithSLA = 0;

            foreach (var incident in incidents)
            {
                if (incident.FechaResolucion.HasValue)
                {
                    // Calcular horas de resolución basado en FechaReporte y FechaResolucion
                    var hoursToResolve = (incident.FechaResolucion.Value - incident.FechaReporte).TotalHours;
                    
                    // Definir SLA basado en prioridad (en horas)
                    int slaHours = incident.Prioridad switch
                    {
                        PrioridadIncidente.Critica => 2,
                        PrioridadIncidente.Alta => 8,
                        PrioridadIncidente.Media => 24,
                        PrioridadIncidente.Baja => 72,
                        _ => 48
                    };

                    totalWithSLA++;
                    if (hoursToResolve <= slaHours)
                    {
                        slaMet++;
                    }
                    else
                    {
                        slaBreached++;
                    }
                }
            }

            double compliancePercentage = totalWithSLA > 0 ? (slaMet * 100.0) / totalWithSLA : 0;

            return new SLAReportDto
            {
                CompliancePercentage = Math.Round(compliancePercentage, 2),
                TotalSLAMetric = totalWithSLA,
                SLAMet = slaMet,
                SLABreached = slaBreached,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<UserActivityReportDto> GetUserActivityReportAsync()
        {
            // Obtener usuarios activos (que han ingresado en los últimos 7 días)
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            
            var activeUsers = await _context.Usuarios
                .Where(u => u.IsActive && u.LastLoginAt.HasValue && u.LastLoginAt.Value >= sevenDaysAgo)
                .CountAsync();

            var totalUsers = await _context.Usuarios.Where(u => u.IsActive).CountAsync();

            // Incidentes creados hoy
            var todayStart = DateTime.Today;
            var todayEnd = DateTime.Today.AddDays(1);
            
            var incidentsCreatedToday = await _context.Incidentes
                .Where(i => i.FechaReporte >= todayStart && i.FechaReporte < todayEnd)
                .CountAsync();

            var incidentsResolvedToday = await _context.Incidentes
                .Where(i => i.FechaResolucion.HasValue && 
                       i.FechaResolucion.Value >= todayStart && 
                       i.FechaResolucion.Value < todayEnd &&
                       i.Estado == EstadoIncidente.Cerrado)
                .CountAsync();

            // Top usuarios activos (asignados con más incidentes)
            var topActiveUsers = await _context.Incidentes
                .Where(i => i.AsignadoAId.HasValue)
                .GroupBy(i => i.AsignadoAId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Join(_context.Usuarios, 
                    g => g.UserId, 
                    u => u.Id, 
                    (g, u) => new UserActivityDetail
                    {
                        UserId = u.Id,
                        UserName = $"{u.FirstName} {u.LastName}",
                        IncidentCount = g.Count,
                        LastActivity = u.LastLoginAt ?? DateTime.Now
                    })
                .ToListAsync();

            return new UserActivityReportDto
            {
                ActiveUsers = activeUsers,
                TotalUsers = totalUsers,
                IncidentsCreatedToday = incidentsCreatedToday,
                IncidentsResolvedToday = incidentsResolvedToday,
                TopActiveUsers = topActiveUsers
            };
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var performance = await GetPerformanceReportAsync();
            var sla = await GetSLAReportAsync();
            var userActivity = await GetUserActivityReportAsync();

            return new DashboardStatsDto
            {
                Performance = performance,
                SLA = sla,
                UserActivity = userActivity
            };
        }
    }
}
