using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities;

public class ComentarioIncidente : BaseEntity
{
    public string Contenido { get; set; } = string.Empty;
    public TipoComentario Tipo { get; set; } = TipoComentario.Comentario;
    public bool EsInterno { get; set; } = false; // Solo visible para técnicos
    
    // Relaciones
    public int IncidenteId { get; set; }
    public Incidente Incidente { get; set; } = null!;
    
    public int AutorId { get; set; }
    public Usuario Autor { get; set; } = null!;
}

public class ComentarioArticulo : BaseEntity
{
    public int ArticuloConocimientoId { get; set; }
    public ArticuloConocimiento ArticuloConocimiento { get; set; } = null!;
    
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public string Contenido { get; set; } = string.Empty;
    public bool EsRespuesta { get; set; } = false;
    public int? ComentarioPadreId { get; set; }
    public ComentarioArticulo? ComentarioPadre { get; set; }
    
    public ICollection<ComentarioArticulo> Respuestas { get; set; } = new List<ComentarioArticulo>();
}

public class VotacionArticulo : BaseEntity
{
    public int ArticuloConocimientoId { get; set; }
    public ArticuloConocimiento ArticuloConocimiento { get; set; } = null!;
    
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public TipoVoto Voto { get; set; } // Positivo o Negativo
    public string? Comentario { get; set; } // Retroalimentación opcional
}

public class ArchivoAdjunto : BaseEntity
{
    public string NombreOriginal { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public string TipoMime { get; set; } = string.Empty;
    public long TamañoBytes { get; set; }
    
    // Relaciones opcionales
    public int? IncidenteId { get; set; }
    public Incidente? Incidente { get; set; }
    
    public int? ArticuloId { get; set; }
    public ArticuloConocimiento? Articulo { get; set; }
    
    public int SubidoPorId { get; set; }
    public Usuario SubidoPor { get; set; } = null!;
}