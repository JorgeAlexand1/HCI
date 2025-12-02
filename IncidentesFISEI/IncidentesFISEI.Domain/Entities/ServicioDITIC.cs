using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities
{
    /// <summary>
    /// Catálogo de Servicios DITIC (Dirección de TIC) según ITIL v4
    /// Define los servicios ofrecidos por el departamento de TI
    /// </summary>
    public class ServicioDITIC : BaseEntity
    {
        public string Codigo { get; set; } = string.Empty; // Ej: SRV-001
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? DescripcionDetallada { get; set; }
        
        // Clasificación del servicio
        public TipoServicioDITIC TipoServicio { get; set; }
        public bool EsServicioEsencial { get; set; } = false;
        
        // Disponibilidad y horarios
        public string? HorarioDisponibilidad { get; set; } // Ej: "24/7", "Lunes a Viernes 8am-5pm"
        public double PorcentajeDisponibilidad { get; set; } = 99.5; // Ej: 99.5%
        
        // SLA asociado
        public int? SLAId { get; set; }
        public SLA? SLA { get; set; }
        
        // Categoría relacionada
        public int? CategoriaId { get; set; }
        public CategoriaIncidente? Categoria { get; set; }
        
        // Responsables
        public int? ResponsableTecnicoId { get; set; }
        public Usuario? ResponsableTecnico { get; set; }
        
        public int? ResponsableNegocioId { get; set; }
        public Usuario? ResponsableNegocio { get; set; }
        
        // Información adicional
        public string? Requisitos { get; set; } // Requisitos para solicitar el servicio
        public string? Limitaciones { get; set; }
        public string? DocumentacionURL { get; set; }
        
        // Costos (opcional)
        public decimal? CostoEstimado { get; set; }
        public string? UnidadCosto { get; set; } // Ej: "por usuario", "por mes", "por incidente"
        
        // Estado del servicio
        public bool EstaActivo { get; set; } = true;
        public DateTime? FechaInicioServicio { get; set; }
        public DateTime? FechaFinServicio { get; set; }
        
        // Navegación
        public ICollection<Incidente> IncidentesRelacionados { get; set; } = new List<Incidente>();
    }
}
