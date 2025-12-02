using IncidentesFISEI.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace IncidentesFISEI.Application.DTOs;

// ============ Artículos ============

public class ArticuloConocimientoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Resumen { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public EstadoArticulo Estado { get; set; }
    public TipoArticulo TipoArticulo { get; set; }
    public int Visualizaciones { get; set; }
    public int VotosPositivos { get; set; }
    public int VotosNegativos { get; set; }
    public bool EsSolucionValidada { get; set; }
    public int VersionActual { get; set; }
    public int VecesUtilizado { get; set; }
    public double? TasaExito { get; set; }
    
    public string[] Tags { get; set; } = Array.Empty<string>();
    public List<EtiquetaDto> Etiquetas { get; set; } = new();
    
    public string AutorNombre { get; set; } = string.Empty;
    public int AutorId { get; set; }
    public string? RevisadoPorNombre { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    
    public DateTime? FechaPublicacion { get; set; }
    public DateTime? FechaRevision { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateArticuloDto
{
    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El contenido es requerido")]
    [MinLength(50, ErrorMessage = "El contenido debe tener al menos 50 caracteres")]
    public string Contenido { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "El resumen no puede exceder 500 caracteres")]
    public string? Resumen { get; set; }
    
    [Required(ErrorMessage = "La categoría es requerida")]
    public int CategoriaId { get; set; }
    
    public TipoArticulo TipoArticulo { get; set; } = TipoArticulo.SolucionProblema;
    
    public string[] Tags { get; set; } = Array.Empty<string>();
    public List<int> EtiquetasIds { get; set; } = new();
    
    public string? PasosDetallados { get; set; }
    public string? Prerequisites { get; set; }
    public string? Limitaciones { get; set; }
    
    public bool PublicarInmediatamente { get; set; } = false;
}

public class UpdateArticuloDto
{
    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El contenido es requerido")]
    [MinLength(50, ErrorMessage = "El contenido debe tener al menos 50 caracteres")]
    public string Contenido { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "El resumen no puede exceder 500 caracteres")]
    public string? Resumen { get; set; }
    
    public int CategoriaId { get; set; }
    public TipoArticulo TipoArticulo { get; set; }
    
    public string[] Tags { get; set; } = Array.Empty<string>();
    public List<int> EtiquetasIds { get; set; } = new();
    
    public string? PasosDetallados { get; set; }
    public string? Prerequisites { get; set; }
    public string? Limitaciones { get; set; }
    
    [Required(ErrorMessage = "Especifica los cambios realizados")]
    public string CambiosRealizados { get; set; } = string.Empty;
}

public class ArticuloDetalladoDto : ArticuloConocimientoDto
{
    public string? PasosDetallados { get; set; }
    public string? Prerequisites { get; set; }
    public string? Limitaciones { get; set; }
    public List<VersionArticuloDto> Versiones { get; set; } = new();
    public List<ComentarioArticuloDto> Comentarios { get; set; } = new();
    public int TotalComentarios { get; set; }
    public List<IncidenteRelacionadoDto> IncidentesRelacionados { get; set; } = new();
}

// ============ Versiones ============

public class VersionArticuloDto
{
    public int Id { get; set; }
    public int NumeroVersion { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? CambiosRealizados { get; set; }
    public string ModificadoPorNombre { get; set; } = string.Empty;
    public DateTime FechaVersion { get; set; }
}

// ============ Etiquetas ============

public class EtiquetaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Color { get; set; } = "#007bff";
    public int VecesUsada { get; set; }
}

public class CreateEtiquetaDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;
    
    [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
    public string? Descripcion { get; set; }
    
    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color debe ser hexadecimal válido")]
    public string Color { get; set; } = "#007bff";
}

// ============ Comentarios ============

public class ComentarioArticuloDto
{
    public int Id { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public string UsuarioNombre { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool EsRespuesta { get; set; }
    public int? ComentarioPadreId { get; set; }
    public List<ComentarioArticuloDto> Respuestas { get; set; } = new();
}

public class CreateComentarioArticuloDto
{
    [Required(ErrorMessage = "El contenido es requerido")]
    [MinLength(5, ErrorMessage = "El comentario debe tener al menos 5 caracteres")]
    [StringLength(1000, ErrorMessage = "El comentario no puede exceder 1000 caracteres")]
    public string Contenido { get; set; } = string.Empty;
    
    public int? ComentarioPadreId { get; set; }
}

// ============ Votaciones ============

public class VotarArticuloDto
{
    [Required]
    public TipoVoto Voto { get; set; }
    
    [StringLength(500, ErrorMessage = "El comentario no puede exceder 500 caracteres")]
    public string? Comentario { get; set; }
}

// ============ Validaciones ============

public class SolicitarValidacionDto
{
    [Required(ErrorMessage = "El ID del validador es requerido")]
    public int ValidadorId { get; set; }
}

public class ValidarArticuloDto
{
    [Required]
    public bool Aprobado { get; set; }
    
    [Required(ErrorMessage = "Los comentarios son requeridos")]
    [MinLength(10, ErrorMessage = "Los comentarios deben tener al menos 10 caracteres")]
    public string ComentariosValidador { get; set; } = string.Empty;
}

public class ValidacionDto
{
    public int Id { get; set; }
    public int ArticuloId { get; set; }
    public string ArticuloTitulo { get; set; } = string.Empty;
    public string SolicitadoPorNombre { get; set; } = string.Empty;
    public string? ValidadorNombre { get; set; }
    public EstadoValidacion Estado { get; set; }
    public string? ComentariosValidador { get; set; }
    public bool Aprobado { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FechaValidacion { get; set; }
}

// ============ Búsqueda ============

public class BusquedaArticulosDto
{
    public string? Termino { get; set; }
    public int? CategoriaId { get; set; }
    public EstadoArticulo? Estado { get; set; }
    public TipoArticulo? TipoArticulo { get; set; }
    public List<int>? EtiquetasIds { get; set; }
    public bool? SoloValidados { get; set; }
    public OrdenArticulos OrdenarPor { get; set; } = OrdenArticulos.MasReciente;
    public int Pagina { get; set; } = 1;
    public int TamañoPagina { get; set; } = 10;
}

public enum OrdenArticulos
{
    MasReciente,
    MasAntiguo,
    MasVisto,
    MejorValorado,
    MasUtilizado
}

public class ResultadoBusquedaArticulosDto
{
    public List<ArticuloConocimientoDto> Articulos { get; set; } = new();
    public int TotalResultados { get; set; }
    public int PaginaActual { get; set; }
    public int TotalPaginas { get; set; }
}

// ============ Estadísticas ============

public class EstadisticasConocimientoDto
{
    public int TotalArticulos { get; set; }
    public int ArticulosPublicados { get; set; }
    public int ArticulosEnRevision { get; set; }
    public int ArticulosBorrador { get; set; }
    public int TotalEtiquetas { get; set; }
    public int TotalComentarios { get; set; }
    public int TotalValidaciones { get; set; }
    
    public double TasaAprobacion { get; set; }
    public int VisualizacionesTotales { get; set; }
    public double PromedioVotosPositivos { get; set; }
    
    public List<ArticuloPopularDto> ArticulosMasPopulares { get; set; } = new();
    public List<EtiquetaDto> EtiquetasMasUsadas { get; set; } = new();
    public List<AutorProductivoDto> AutoresMasProductivos { get; set; } = new();
}

public class ArticuloPopularDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public int Visualizaciones { get; set; }
    public int VotosPositivos { get; set; }
    public int VecesUtilizado { get; set; }
}

public class AutorProductivoDto
{
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int TotalArticulos { get; set; }
    public int ArticulosPublicados { get; set; }
}

public class IncidenteRelacionadoDto
{
    public int Id { get; set; }
    public string NumeroIncidente { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public EstadoIncidente Estado { get; set; }
    public DateTime FechaReporte { get; set; }
}
