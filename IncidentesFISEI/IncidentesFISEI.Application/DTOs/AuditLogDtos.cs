using IncidentesFISEI.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace IncidentesFISEI.Application.DTOs;

#region AuditLog DTOs

public class AuditLogDto
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public string? UsuarioNombre { get; set; }
    public string? DireccionIP { get; set; }
    public TipoAccionAuditoria TipoAccion { get; set; }
    public string TipoAccionDescripcion { get; set; } = string.Empty;
    public TipoEntidadAuditoria TipoEntidad { get; set; }
    public string TipoEntidadDescripcion { get; set; } = string.Empty;
    public int? EntidadId { get; set; }
    public string? EntidadDescripcion { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public NivelSeveridadAuditoria NivelSeveridad { get; set; }
    public string NivelSeveridadDescripcion { get; set; } = string.Empty;
    public bool EsExitoso { get; set; }
    public string? MensajeError { get; set; }
    public DateTime FechaHora { get; set; }
    public string? Modulo { get; set; }
    public string? Endpoint { get; set; }
}

public class AuditLogDetalladoDto
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public string? UsuarioNombre { get; set; }
    public string? DireccionIP { get; set; }
    public string? UserAgent { get; set; }
    public TipoAccionAuditoria TipoAccion { get; set; }
    public string TipoAccionDescripcion { get; set; } = string.Empty;
    public TipoEntidadAuditoria TipoEntidad { get; set; }
    public string TipoEntidadDescripcion { get; set; } = string.Empty;
    public int? EntidadId { get; set; }
    public string? EntidadDescripcion { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? ValoresAnteriores { get; set; }
    public string? ValoresNuevos { get; set; }
    public NivelSeveridadAuditoria NivelSeveridad { get; set; }
    public string NivelSeveridadDescripcion { get; set; } = string.Empty;
    public string? MetadataJson { get; set; }
    public bool EsExitoso { get; set; }
    public string? MensajeError { get; set; }
    public int? CantidadRegistros { get; set; }
    public string? FiltrosAplicados { get; set; }
    public DateTime FechaHora { get; set; }
    public string? Modulo { get; set; }
    public string? Endpoint { get; set; }
}

public class CreateAuditLogDto
{
    public int? UsuarioId { get; set; }
    public string? UsuarioNombre { get; set; }
    public string? DireccionIP { get; set; }
    public string? UserAgent { get; set; }
    
    [Required]
    public TipoAccionAuditoria TipoAccion { get; set; }
    
    [Required]
    public TipoEntidadAuditoria TipoEntidad { get; set; }
    
    public int? EntidadId { get; set; }
    public string? EntidadDescripcion { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;
    
    public string? ValoresAnteriores { get; set; }
    public string? ValoresNuevos { get; set; }
    public NivelSeveridadAuditoria NivelSeveridad { get; set; } = NivelSeveridadAuditoria.Informativo;
    public string? MetadataJson { get; set; }
    public bool EsExitoso { get; set; } = true;
    public string? MensajeError { get; set; }
    public int? CantidadRegistros { get; set; }
    public string? FiltrosAplicados { get; set; }
    public string? Modulo { get; set; }
    public string? Endpoint { get; set; }
}

public class BuscarAuditLogsDto
{
    public int? UsuarioId { get; set; }
    public TipoAccionAuditoria? TipoAccion { get; set; }
    public TipoEntidadAuditoria? TipoEntidad { get; set; }
    public NivelSeveridadAuditoria? NivelSeveridad { get; set; }
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public bool? SoloErrores { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 50;
}

public class EstadisticasAuditoriaDto
{
    public int TotalRegistros { get; set; }
    public int TotalExitosos { get; set; }
    public int TotalErrores { get; set; }
    public double TasaExito { get; set; }
    public Dictionary<string, int> AccionesPorTipo { get; set; } = new();
    public Dictionary<string, int> ActividadPorUsuario { get; set; } = new();
    public Dictionary<string, int> EntidadesPorTipo { get; set; } = new();
    public List<AuditLogDto> LogsCriticos { get; set; } = new();
    public List<AuditLogDto> UltimosErrores { get; set; } = new();
}

#endregion
