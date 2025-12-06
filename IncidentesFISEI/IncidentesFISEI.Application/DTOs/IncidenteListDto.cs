using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.DTOs;

public class IncidenteListDto
{
    public int Id { get; set; }
    public string NumeroIncidente { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public EstadoIncidente Estado { get; set; }
    public PrioridadIncidente Prioridad { get; set; }
    public ImpactoIncidente Impacto { get; set; }
    public UrgenciaIncidente Urgencia { get; set; }
    public DateTime FechaReporte { get; set; }
    public DateTime? FechaAsignacion { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public string ReportadoPor { get; set; } = string.Empty;
    public int ReportadoPorId { get; set; }
    public string? AsignadoA { get; set; }
    public int? AsignadoAId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string? ServicioNombre { get; set; }
    public int? ServicioId { get; set; }
    public string? Solucion { get; set; }
}