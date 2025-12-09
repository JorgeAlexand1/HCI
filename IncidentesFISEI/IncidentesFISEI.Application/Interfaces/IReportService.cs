namespace IncidentesFISEI.Application.Interfaces
{
    public interface IReportService
    {
        Task<PerformanceReportDto> GetPerformanceReportAsync();
        Task<SLAReportDto> GetSLAReportAsync();
        Task<UserActivityReportDto> GetUserActivityReportAsync();
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }

    public class PerformanceReportDto
    {
        public int TotalIncidents { get; set; }
        public int ResolvedIncidents { get; set; }
        public int PendingIncidents { get; set; }
        public double AverageResolutionTime { get; set; } // En horas
        public double ResolutionRate { get; set; } // Porcentaje
        public DateTime UpdatedAt { get; set; }
    }

    public class SLAReportDto
    {
        public double CompliancePercentage { get; set; }
        public int TotalSLAMetric { get; set; }
        public int SLAMet { get; set; }
        public int SLABreached { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UserActivityReportDto
    {
        public int ActiveUsers { get; set; }
        public int TotalUsers { get; set; }
        public int IncidentsCreatedToday { get; set; }
        public int IncidentsResolvedToday { get; set; }
        public List<UserActivityDetail> TopActiveUsers { get; set; } = new();
    }

    public class UserActivityDetail
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int IncidentCount { get; set; }
        public DateTime LastActivity { get; set; }
    }

    public class DashboardStatsDto
    {
        public PerformanceReportDto Performance { get; set; }
        public SLAReportDto SLA { get; set; }
        public UserActivityReportDto UserActivity { get; set; }
    }
}
