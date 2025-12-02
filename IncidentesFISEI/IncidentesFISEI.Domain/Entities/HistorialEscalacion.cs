using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities
{
    /// <summary>
    /// Historial de escalaciones de un incidente
    /// Permite trazabilidad completa según ITIL v4
    /// </summary>
    public class HistorialEscalacion : BaseEntity
    {
        public int IncidenteId { get; set; }
        public Incidente Incidente { get; set; } = null!;
        
        public NivelSoporte NivelOrigen { get; set; }
        public NivelSoporte NivelDestino { get; set; }
        
        public int? TecnicoOrigenId { get; set; }
        public Usuario? TecnicoOrigen { get; set; }
        
        public int? TecnicoDestinoId { get; set; }
        public Usuario? TecnicoDestino { get; set; }
        
        public string Razon { get; set; } = string.Empty;
        public bool FueAutomatico { get; set; }
        public DateTime FechaEscalacion { get; set; } = DateTime.UtcNow;
    }
}
