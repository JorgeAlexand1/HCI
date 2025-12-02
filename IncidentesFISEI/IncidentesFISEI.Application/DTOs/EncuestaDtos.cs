using IncidentesFISEI.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace IncidentesFISEI.Application.DTOs;

#region Plantilla Encuesta DTOs

public class PlantillaEncuestaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool EsActiva { get; set; }
    public TipoEncuesta Tipo { get; set; }
    public string TipoDescripcion { get; set; } = string.Empty;
    public bool MostrarAutomaticamente { get; set; }
    public int DiasVigencia { get; set; }
    public int TotalPreguntas { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PlantillaEncuestaDetalladaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool EsActiva { get; set; }
    public TipoEncuesta Tipo { get; set; }
    public bool MostrarAutomaticamente { get; set; }
    public int DiasVigencia { get; set; }
    public List<PreguntaEncuestaDto> Preguntas { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreatePlantillaEncuestaDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200)]
    public string Nombre { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Descripcion { get; set; }
    
    [Required]
    public TipoEncuesta Tipo { get; set; }
    
    public bool EsActiva { get; set; } = true;
    public bool MostrarAutomaticamente { get; set; } = true;
    
    [Range(1, 365)]
    public int DiasVigencia { get; set; } = 30;
    
    public List<CreatePreguntaEncuestaDto> Preguntas { get; set; } = new();
}

public class UpdatePlantillaEncuestaDto
{
    [Required]
    [StringLength(200)]
    public string Nombre { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Descripcion { get; set; }
    
    public bool EsActiva { get; set; }
    public bool MostrarAutomaticamente { get; set; }
    
    [Range(1, 365)]
    public int DiasVigencia { get; set; }
}

#endregion

#region Pregunta Encuesta DTOs

public class PreguntaEncuestaDto
{
    public int Id { get; set; }
    public int PlantillaEncuestaId { get; set; }
    public string TextoPregunta { get; set; } = string.Empty;
    public TipoPregunta Tipo { get; set; }
    public string TipoDescripcion { get; set; } = string.Empty;
    public int Orden { get; set; }
    public bool EsObligatoria { get; set; }
    public List<string>? Opciones { get; set; }
    public int? ValorMinimo { get; set; }
    public int? ValorMaximo { get; set; }
    public string? EtiquetaMinimo { get; set; }
    public string? EtiquetaMaximo { get; set; }
}

public class CreatePreguntaEncuestaDto
{
    [Required(ErrorMessage = "El texto de la pregunta es requerido")]
    [StringLength(500)]
    public string TextoPregunta { get; set; } = string.Empty;
    
    [Required]
    public TipoPregunta Tipo { get; set; }
    
    public int Orden { get; set; }
    public bool EsObligatoria { get; set; } = true;
    
    public List<string>? Opciones { get; set; }
    public int? ValorMinimo { get; set; }
    public int? ValorMaximo { get; set; }
    
    [StringLength(100)]
    public string? EtiquetaMinimo { get; set; }
    
    [StringLength(100)]
    public string? EtiquetaMaximo { get; set; }
}

public class UpdatePreguntaEncuestaDto
{
    [Required]
    [StringLength(500)]
    public string TextoPregunta { get; set; } = string.Empty;
    
    public bool EsObligatoria { get; set; }
    public int Orden { get; set; }
}

#endregion

#region Encuesta DTOs

public class EncuestaDto
{
    public int Id { get; set; }
    public int IncidenteId { get; set; }
    public string IncidenteTitulo { get; set; } = string.Empty;
    public string IncidenteNumero { get; set; } = string.Empty;
    public int PlantillaEncuestaId { get; set; }
    public string PlantillaNombre { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public DateTime FechaEnvio { get; set; }
    public DateTime? FechaRespuesta { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public bool EsRespondida { get; set; }
    public bool EsVencida { get; set; }
    public double? CalificacionPromedio { get; set; }
    public int DiasRestantes { get; set; }
}

public class EncuestaDetalladaDto
{
    public int Id { get; set; }
    public int IncidenteId { get; set; }
    public string IncidenteTitulo { get; set; } = string.Empty;
    public string IncidenteNumero { get; set; } = string.Empty;
    public PlantillaEncuestaDetalladaDto Plantilla { get; set; } = null!;
    public DateTime FechaEnvio { get; set; }
    public DateTime? FechaRespuesta { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public bool EsRespondida { get; set; }
    public bool EsVencida { get; set; }
    public double? CalificacionPromedio { get; set; }
    public string? ComentariosGenerales { get; set; }
    public List<RespuestaEncuestaDto> Respuestas { get; set; } = new();
}

public class ResponderEncuestaDto
{
    [Required]
    public int EncuestaId { get; set; }
    
    [Required]
    public List<RespuestaDto> Respuestas { get; set; } = new();
    
    [StringLength(1000)]
    public string? ComentariosGenerales { get; set; }
}

public class RespuestaDto
{
    [Required]
    public int PreguntaId { get; set; }
    
    public string? RespuestaTexto { get; set; }
    public int? RespuestaNumero { get; set; }
    public bool? RespuestaBooleana { get; set; }
    public DateTime? RespuestaFecha { get; set; }
    public List<string>? RespuestasSeleccion { get; set; }
}

#endregion

#region Respuesta Encuesta DTOs

public class RespuestaEncuestaDto
{
    public int Id { get; set; }
    public int PreguntaId { get; set; }
    public string TextoPregunta { get; set; } = string.Empty;
    public TipoPregunta TipoPregunta { get; set; }
    public string? RespuestaTexto { get; set; }
    public int? RespuestaNumero { get; set; }
    public bool? RespuestaBooleana { get; set; }
    public DateTime? RespuestaFecha { get; set; }
    public List<string>? RespuestasSeleccion { get; set; }
}

#endregion

#region Estadísticas DTOs

public class EstadisticasEncuestasDto
{
    public int TotalEncuestasEnviadas { get; set; }
    public int TotalEncuestasRespondidas { get; set; }
    public int TotalEncuestasVencidas { get; set; }
    public int TotalEncuestasPendientes { get; set; }
    public double TasaRespuesta { get; set; }
    public double CalificacionPromedio { get; set; }
    public Dictionary<int, int> DistribucionCalificaciones { get; set; } = new();
    public List<EncuestaPorPeriodoDto> EncuestasPorPeriodo { get; set; } = new();
    public List<CalificacionPorTecnicoDto> CalificacionesPorTecnico { get; set; } = new();
}

public class EncuestaPorPeriodoDto
{
    public string Periodo { get; set; } = string.Empty;
    public int Enviadas { get; set; }
    public int Respondidas { get; set; }
    public double PromedioCalificacion { get; set; }
}

public class CalificacionPorTecnicoDto
{
    public int TecnicoId { get; set; }
    public string TecnicoNombre { get; set; } = string.Empty;
    public int TotalEncuestas { get; set; }
    public double CalificacionPromedio { get; set; }
}

#endregion
