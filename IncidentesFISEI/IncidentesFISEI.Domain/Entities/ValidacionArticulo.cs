using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities;

/// <summary>
/// Solicitud de validación/revisión de un artículo por un experto
/// </summary>
public class ValidacionArticulo : BaseEntity
{
    public int ArticuloConocimientoId { get; set; }
    public ArticuloConocimiento ArticuloConocimiento { get; set; } = null!;
    
    public int SolicitadoPorId { get; set; }
    public Usuario SolicitadoPor { get; set; } = null!;
    
    public int? ValidadorId { get; set; }
    public Usuario? Validador { get; set; }
    
    public EstadoValidacion Estado { get; set; } = EstadoValidacion.Pendiente;
    public string? ComentariosValidador { get; set; }
    public DateTime? FechaValidacion { get; set; }
    
    public bool Aprobado { get; set; } = false;
}
