using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities;

public class Incidente : BaseEntity
{
    public string NumeroIncidente { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public EstadoIncidente Estado { get; set; } = EstadoIncidente.Abierto;
    public PrioridadIncidente Prioridad { get; set; } = PrioridadIncidente.Media;
    public ImpactoIncidente Impacto { get; set; } = ImpactoIncidente.Medio;
    public UrgenciaIncidente Urgencia { get; set; } = UrgenciaIncidente.Media;
    
    // Fechas importantes según ITIL v3
    public DateTime FechaReporte { get; set; } = DateTime.UtcNow;
    public DateTime? FechaAsignacion { get; set; }
    public DateTime? FechaInicioTrabajo { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    
    // Escalación según ITIL v4
    public NivelSoporte NivelActual { get; set; } = NivelSoporte.L1_Tecnico;
    public int NumeroEscalaciones { get; set; } = 0;
    public DateTime? FechaUltimaEscalacion { get; set; }
    public string? RazonEscalacion { get; set; }
    public bool EscaladoAutomaticamente { get; set; } = false;
    
    // Información adicional
    public string? Solucion { get; set; }
    public string? CausaRaiz { get; set; }
    public string? PasosReproducir { get; set; }
    public string? ImpactoDetallado { get; set; }
    public string? ActivosAfectados { get; set; }
    
    // Relaciones
    public int ReportadoPorId { get; set; }
    public Usuario ReportadoPor { get; set; } = null!;
    
    public int? AsignadoAId { get; set; }
    public Usuario? AsignadoA { get; set; }
    
    public int CategoriaId { get; set; }
    public CategoriaIncidente Categoria { get; set; } = null!;
    
    public int? ArticuloConocimientoId { get; set; }
    public ArticuloConocimiento? ArticuloConocimiento { get; set; }
    
    public int? ServicioDITICId { get; set; }
    public ServicioDITIC? ServicioDITIC { get; set; }
    
    // Navegación
    public ICollection<ComentarioIncidente> Comentarios { get; set; } = new List<ComentarioIncidente>();
    public ICollection<ArchivoAdjunto> ArchivosAdjuntos { get; set; } = new List<ArchivoAdjunto>();
    public ICollection<IncidenteRelacionado> IncidentesRelacionados { get; set; } = new List<IncidenteRelacionado>();
    public ICollection<RegistroTiempo> RegistrosTiempo { get; set; } = new List<RegistroTiempo>();
}