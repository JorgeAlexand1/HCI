using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities;

public class ArticuloConocimiento : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public string? Resumen { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
    public EstadoArticulo Estado { get; set; } = EstadoArticulo.Borrador;
    
    // Metadatos
    public int Visualizaciones { get; set; } = 0;
    public int VotosPositivos { get; set; } = 0;
    public int VotosNegativos { get; set; } = 0;
    public DateTime? FechaPublicacion { get; set; }
    public DateTime? FechaRevision { get; set; }
    
    // Información de utilidad
    public bool EsSolucionValidada { get; set; } = false;
    public string? PasosDetallados { get; set; }
    public string? Prerequisites { get; set; }
    public string? Limitaciones { get; set; }
    
    // Relaciones
    public int AutorId { get; set; }
    public Usuario Autor { get; set; } = null!;
    
    public int? RevisadoPorId { get; set; }
    public Usuario? RevisadoPor { get; set; }
    
    public int CategoriaId { get; set; }
    public CategoriaIncidente Categoria { get; set; } = null!;
    
    // Navegación
    public ICollection<Incidente> IncidentesRelacionados { get; set; } = new List<Incidente>();
    public ICollection<ArchivoAdjunto> ArchivosAdjuntos { get; set; } = new List<ArchivoAdjunto>();
    public ICollection<ComentarioArticulo> Comentarios { get; set; } = new List<ComentarioArticulo>();
    public ICollection<VotacionArticulo> Votaciones { get; set; } = new List<VotacionArticulo>();
}