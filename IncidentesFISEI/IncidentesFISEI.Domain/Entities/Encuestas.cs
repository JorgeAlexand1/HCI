using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities;

/// <summary>
/// Plantilla de encuesta configurable
/// </summary>
public class PlantillaEncuesta : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool EsActiva { get; set; } = true;
    public TipoEncuesta Tipo { get; set; }
    
    // Configuración
    public bool MostrarAutomaticamente { get; set; } = true;
    public int DiasVigencia { get; set; } = 30; // Días para responder después del cierre
    
    // Navegación
    public ICollection<PreguntaEncuesta> Preguntas { get; set; } = new List<PreguntaEncuesta>();
    public ICollection<Encuesta> Encuestas { get; set; } = new List<Encuesta>();
}

/// <summary>
/// Pregunta dentro de una plantilla de encuesta
/// </summary>
public class PreguntaEncuesta : BaseEntity
{
    public int PlantillaEncuestaId { get; set; }
    public PlantillaEncuesta PlantillaEncuesta { get; set; } = null!;
    
    public string TextoPregunta { get; set; } = string.Empty;
    public TipoPregunta Tipo { get; set; }
    public int Orden { get; set; }
    public bool EsObligatoria { get; set; } = true;
    
    // Para preguntas tipo escala o selección múltiple
    public string? OpcionesJson { get; set; } // JSON array de opciones
    public int? ValorMinimo { get; set; }
    public int? ValorMaximo { get; set; }
    public string? EtiquetaMinimo { get; set; } // Ej: "Muy insatisfecho"
    public string? EtiquetaMaximo { get; set; } // Ej: "Muy satisfecho"
    
    // Navegación
    public ICollection<RespuestaEncuesta> Respuestas { get; set; } = new List<RespuestaEncuesta>();
}

/// <summary>
/// Instancia de encuesta enviada a un usuario por un incidente
/// </summary>
public class Encuesta : BaseEntity
{
    public int IncidenteId { get; set; }
    public Incidente Incidente { get; set; } = null!;
    
    public int PlantillaEncuestaId { get; set; }
    public PlantillaEncuesta PlantillaEncuesta { get; set; } = null!;
    
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
    public DateTime? FechaRespuesta { get; set; }
    public DateTime FechaVencimiento { get; set; }
    
    public bool EsRespondida { get; set; } = false;
    public bool EsVencida { get; set; } = false;
    
    // Calificación general calculada
    public double? CalificacionPromedio { get; set; }
    public string? ComentariosGenerales { get; set; }
    
    // Navegación
    public ICollection<RespuestaEncuesta> Respuestas { get; set; } = new List<RespuestaEncuesta>();
}

/// <summary>
/// Respuesta individual a una pregunta de encuesta
/// </summary>
public class RespuestaEncuesta : BaseEntity
{
    public int EncuestaId { get; set; }
    public Encuesta Encuesta { get; set; } = null!;
    
    public int PreguntaEncuestaId { get; set; }
    public PreguntaEncuesta PreguntaEncuesta { get; set; } = null!;
    
    // Respuesta (depende del tipo de pregunta)
    public string? RespuestaTexto { get; set; }
    public int? RespuestaNumero { get; set; }
    public bool? RespuestaBooleana { get; set; }
    public DateTime? RespuestaFecha { get; set; }
    
    // Para respuestas de selección múltiple (JSON array)
    public string? RespuestasSeleccionJson { get; set; }
}
