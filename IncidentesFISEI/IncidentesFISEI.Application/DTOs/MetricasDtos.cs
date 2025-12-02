namespace IncidentesFISEI.Application.DTOs
{
    /// <summary>
    /// KPIs principales del sistema de gestión de incidentes según ITIL v4
    /// </summary>
    public class DashboardKPIsDto
    {
        // Métricas generales
        public int TotalIncidentes { get; set; }
        public int IncidentesAbiertos { get; set; }
        public int IncidentesEnProgreso { get; set; }
        public int IncidentesResueltos { get; set; }
        public int IncidentesCerrados { get; set; }
        
        // Métricas de tiempo (en horas)
        public double TiempoPromedioResolucion { get; set; }
        public double TiempoPromedioRespuesta { get; set; }
        public double TiempoPromedioPrimeraAsignacion { get; set; }
        
        // Métricas de SLA
        public double PorcentajeCumplimientoSLA { get; set; }
        public int IncidentesDentroSLA { get; set; }
        public int IncidentesFueraSLA { get; set; }
        
        // Métricas de escalación
        public int TotalEscalaciones { get; set; }
        public double PorcentajeIncidentesEscalados { get; set; }
        public double TasaResolucionPrimerNivel { get; set; } // FCR - First Contact Resolution
        
        // Métricas de carga de trabajo
        public int TecnicosActivos { get; set; }
        public double CargaPromedioTecnicos { get; set; }
        public int IncidentesSinAsignar { get; set; }
        
        // Métricas por prioridad
        public int IncidentesCriticos { get; set; }
        public int IncidentesAltos { get; set; }
        public int IncidentesMedios { get; set; }
        public int IncidentesBajos { get; set; }
        
        // Tendencias (comparación con período anterior)
        public double CambioIncidentesTotales { get; set; } // % cambio
        public double CambioTiempoResolucion { get; set; }
        public double CambioCumplimientoSLA { get; set; }
    }

    /// <summary>
    /// Estadísticas detalladas por categoría
    /// </summary>
    public class EstadisticasPorCategoriaDto
    {
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public int TotalIncidentes { get; set; }
        public int IncidentesAbiertos { get; set; }
        public int IncidentesResueltos { get; set; }
        public double TiempoPromedioResolucion { get; set; }
        public double PorcentajeCumplimientoSLA { get; set; }
        public double PorcentajeDelTotal { get; set; }
    }

    /// <summary>
    /// Estadísticas por técnico/usuario
    /// </summary>
    public class EstadisticasPorTecnicoDto
    {
        public int TecnicoId { get; set; }
        public string TecnicoNombre { get; set; } = string.Empty;
        public string? NivelSoporte { get; set; }
        public int IncidentesAsignados { get; set; }
        public int IncidentesResueltos { get; set; }
        public int IncidentesEnProgreso { get; set; }
        public double TasaResolucion { get; set; } // % resueltos de asignados
        public double TiempoPromedioResolucion { get; set; }
        public int CargaActual { get; set; }
        public double PorcentajeCumplimientoSLA { get; set; }
    }

    /// <summary>
    /// Tendencias temporales (serie de tiempo)
    /// </summary>
    public class TendenciasDto
    {
        public List<TendenciaDiariaDto> TendenciasDiarias { get; set; } = new();
        public List<TendenciaMensualDto> TendenciasMensuales { get; set; } = new();
    }

    public class TendenciaDiariaDto
    {
        public DateTime Fecha { get; set; }
        public int IncidentesCreados { get; set; }
        public int IncidentesResueltos { get; set; }
        public int IncidentesCerrados { get; set; }
        public double TiempoPromedioResolucion { get; set; }
    }

    public class TendenciaMensualDto
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public string MesNombre { get; set; } = string.Empty;
        public int TotalIncidentes { get; set; }
        public int IncidentesResueltos { get; set; }
        public double PorcentajeCumplimientoSLA { get; set; }
        public double TiempoPromedioResolucion { get; set; }
    }

    /// <summary>
    /// Reporte de disponibilidad de servicios
    /// </summary>
    public class ReporteDisponibilidadDto
    {
        public List<DisponibilidadServicioDto> Servicios { get; set; } = new();
        public double DisponibilidadPromedio { get; set; }
    }

    public class DisponibilidadServicioDto
    {
        public int ServicioId { get; set; }
        public string ServicioNombre { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public double DisponibilidadObjetivo { get; set; }
        public double DisponibilidadReal { get; set; }
        public int TotalIncidentes { get; set; }
        public int IncidentesCriticos { get; set; }
        public double TiempoInactividad { get; set; } // Horas
        public bool CumpleObjetivo { get; set; }
    }

    /// <summary>
    /// Top incidentes más frecuentes
    /// </summary>
    public class TopIncidentesDto
    {
        public List<IncidenteFrecuenteDto> IncidentesFrecuentes { get; set; } = new();
    }

    public class IncidenteFrecuenteDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int Ocurrencias { get; set; }
        public double TiempoPromedioResolucion { get; set; }
        public int? ArticuloConocimientoId { get; set; }
        public bool TieneSolucion { get; set; }
    }
}
