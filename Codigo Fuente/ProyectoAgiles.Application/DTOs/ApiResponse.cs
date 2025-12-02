namespace ProyectoAgiles.Application.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public List<string>? Errors { get; set; }
}

public class ReportStatisticsDto
{
    public int TotalSolicitudes { get; set; }
    public int SolicitudesPendientes { get; set; }
    public int SolicitudesEnRevision { get; set; }
    public int SolicitudesVerificadas { get; set; }
    public int SolicitudesAprobadas { get; set; }
    public int SolicitudesRechazadas { get; set; }
    public int SolicitudesFinalizadas { get; set; }
    public int PromocionesAuxiliar { get; set; }
    public int PromocionesAgregado { get; set; }
    public int PromocionesPrincipal { get; set; }
    public int PromocionesTitular { get; set; }
    public int SolicitudesEsteMes { get; set; }
    public int SolicitudesEsteAno { get; set; }
    public Dictionary<string, int> SolicitudesPorFacultad { get; set; } = new();
    public Dictionary<string, int> SolicitudesPorEstado { get; set; } = new();
}
