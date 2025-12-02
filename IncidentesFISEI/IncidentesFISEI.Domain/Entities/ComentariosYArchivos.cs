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
    public string Contenido { get; set; } = string.Empty;
    public int Puntuacion { get; set; } = 0; // 1-5 estrellas
    
    // Relaciones
    public int ArticuloId { get; set; }
    public ArticuloConocimiento Articulo { get; set; } = null!;
    
    public int AutorId { get; set; }
    public Usuario Autor { get; set; } = null!;
}

public class VotacionArticulo : BaseEntity
{
    public bool EsPositivo { get; set; }
    
    // Relaciones
    public int ArticuloId { get; set; }
    public ArticuloConocimiento Articulo { get; set; } = null!;
    
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
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