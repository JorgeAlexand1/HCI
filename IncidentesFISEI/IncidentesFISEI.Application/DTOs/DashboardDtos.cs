using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.DTOs
{
    // DTOs para el Dashboard
    public class AdminDashboardStatsDto
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosPendientes { get; set; }
        public int AdminsCount { get; set; }
        public int TecnicosCount { get; set; }
        public int DocentesCount { get; set; }
        public int EstudiantesCount { get; set; }
        
        public int TotalIncidentes { get; set; }
        public int IncidentesAbiertos { get; set; }
        public int IncidentesEnProgreso { get; set; }
        public int IncidentesCriticos { get; set; }
        public int IncidentesHoy { get; set; }
        public int IncidentesAltaPrioridad { get; set; }
        public int IncidentesMediaPrioridad { get; set; }
        public int IncidentesBajaPrioridad { get; set; }
        
        public int SlaActivos { get; set; }
        public double CumplimientoSla { get; set; }
        
        public int ReportesHoy { get; set; }
        public int CategoriasActivas { get; set; }
    }

    public class UsuarioListDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string StatusText => IsActive ? "Activo" : "Inactivo";
        public string EmailStatusText => IsEmailConfirmed ? "Confirmado" : "Pendiente";
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }



    public class DashboardKpiDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public double? PorcentajeCambio { get; set; }
        public bool EsPositivo { get; set; }
    }

    public class EstadisticasIncidenteDto
    {
        public Dictionary<EstadoIncidente, int> PorEstado { get; set; } = new();
        public Dictionary<PrioridadIncidente, int> PorPrioridad { get; set; } = new();
        public Dictionary<string, int> PorCategoria { get; set; } = new();
        public Dictionary<string, int> PorMes { get; set; } = new();
        public double TiempoPromedioResolucion { get; set; }
        public double PorcentajeCumplimientoSla { get; set; }
    }
}