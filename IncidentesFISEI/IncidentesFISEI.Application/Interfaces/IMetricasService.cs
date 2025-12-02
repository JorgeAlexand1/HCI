using IncidentesFISEI.Application.DTOs;

namespace IncidentesFISEI.Application.Interfaces
{
    public interface IMetricasService
    {
        /// <summary>
        /// Obtiene KPIs principales del dashboard
        /// </summary>
        Task<ApiResponse<DashboardKPIsDto>> ObtenerKPIsPrincipalesAsync(DateTime? desde = null, DateTime? hasta = null);
        
        /// <summary>
        /// Obtiene estadísticas agrupadas por categoría
        /// </summary>
        Task<ApiResponse<List<EstadisticasPorCategoriaDto>>> ObtenerEstadisticasPorCategoriaAsync(DateTime? desde = null, DateTime? hasta = null);
        
        /// <summary>
        /// Obtiene estadísticas de rendimiento por técnico
        /// </summary>
        Task<ApiResponse<List<EstadisticasPorTecnicoDto>>> ObtenerEstadisticasPorTecnicoAsync(DateTime? desde = null, DateTime? hasta = null);
        
        /// <summary>
        /// Obtiene tendencias temporales (diarias y mensuales)
        /// </summary>
        Task<ApiResponse<TendenciasDto>> ObtenerTendenciasAsync(int ultimosDias = 30);
        
        /// <summary>
        /// Obtiene reporte de disponibilidad de servicios
        /// </summary>
        Task<ApiResponse<ReporteDisponibilidadDto>> ObtenerReporteDisponibilidadAsync(DateTime? desde = null, DateTime? hasta = null);
        
        /// <summary>
        /// Obtiene top de incidentes más frecuentes
        /// </summary>
        Task<ApiResponse<TopIncidentesDto>> ObtenerTopIncidentesFrecuentesAsync(int top = 10, DateTime? desde = null, DateTime? hasta = null);
    }
}
