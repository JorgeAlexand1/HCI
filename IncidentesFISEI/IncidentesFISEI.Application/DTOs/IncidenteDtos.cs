using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.DTOs;

public record CreateIncidenteDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public PrioridadIncidente Prioridad { get; set; } = PrioridadIncidente.Media;
    public ImpactoIncidente Impacto { get; set; } = ImpactoIncidente.Medio;
    public UrgenciaIncidente Urgencia { get; set; } = UrgenciaIncidente.Media;
    public int CategoriaId { get; set; }
    public string? PasosReproducir { get; set; }
    public string? ActivosAfectados { get; set; }
}

public record UpdateIncidenteDto
{
    public int Id { get; set; }
    public string? Titulo { get; set; }
    public string? Descripcion { get; set; }
    public EstadoIncidente? Estado { get; set; }
    public PrioridadIncidente? Prioridad { get; set; }
    public ImpactoIncidente? Impacto { get; set; }
    public UrgenciaIncidente? Urgencia { get; set; }
    public int? AsignadoAId { get; set; }
    public string? Solucion { get; set; }
    public string? CausaRaiz { get; set; }
}

public record IncidenteDto
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
    public DateTime? FechaCierre { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string? Solucion { get; set; }
    public string? CausaRaiz { get; set; }
    public string? PasosReproducir { get; set; }
    public string? ActivosAfectados { get; set; }
    
    // Información del usuario que reportó
    public UsuarioDto ReportadoPor { get; set; } = null!;
    
    // Información del técnico asignado
    public UsuarioDto? AsignadoA { get; set; }
    
    // Información de la categoría
    public CategoriaIncidenteDto Categoria { get; set; } = null!;
    
    // Contadores
    public int NumeroComentarios { get; set; }
    public int NumeroArchivos { get; set; }
}

public record IncidenteDetalleDto : IncidenteDto
{
    public List<ComentarioIncidenteDto> Comentarios { get; set; } = new();
    public List<ArchivoAdjuntoDto> ArchivosAdjuntos { get; set; } = new();
    public List<RegistroTiempoDto> RegistrosTiempo { get; set; } = new();
}