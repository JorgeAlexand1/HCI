using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.DTOs
{
    public class ServicioDITICDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? DescripcionDetallada { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public bool EsServicioEsencial { get; set; }
        public string? HorarioDisponibilidad { get; set; }
        public double PorcentajeDisponibilidad { get; set; }
        public string? SLANombre { get; set; }
        public string? CategoriaNombre { get; set; }
        public string? ResponsableTecnicoNombre { get; set; }
        public string? ResponsableNegocioNombre { get; set; }
        public string? Requisitos { get; set; }
        public string? Limitaciones { get; set; }
        public string? DocumentacionURL { get; set; }
        public decimal? CostoEstimado { get; set; }
        public string? UnidadCosto { get; set; }
        public bool EstaActivo { get; set; }
        public DateTime? FechaInicioServicio { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateServicioDITICDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? DescripcionDetallada { get; set; }
        public TipoServicioDITIC TipoServicio { get; set; }
        public bool EsServicioEsencial { get; set; } = false;
        public string? HorarioDisponibilidad { get; set; }
        public double PorcentajeDisponibilidad { get; set; } = 99.0;
        public int? SLAId { get; set; }
        public int? CategoriaId { get; set; }
        public int? ResponsableTecnicoId { get; set; }
        public int? ResponsableNegocioId { get; set; }
        public string? Requisitos { get; set; }
        public string? Limitaciones { get; set; }
        public string? DocumentacionURL { get; set; }
        public decimal? CostoEstimado { get; set; }
        public string? UnidadCosto { get; set; }
    }

    public class UpdateServicioDITICDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? DescripcionDetallada { get; set; }
        public bool? EsServicioEsencial { get; set; }
        public string? HorarioDisponibilidad { get; set; }
        public double? PorcentajeDisponibilidad { get; set; }
        public int? SLAId { get; set; }
        public int? ResponsableTecnicoId { get; set; }
        public string? Limitaciones { get; set; }
        public bool? EstaActivo { get; set; }
    }
}
