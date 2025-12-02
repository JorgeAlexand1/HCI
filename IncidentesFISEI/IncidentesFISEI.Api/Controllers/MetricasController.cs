using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentesFISEI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador,Supervisor")]
    public class MetricasController : ControllerBase
    {
        private readonly IMetricasService _metricasService;
        private readonly ILogger<MetricasController> _logger;

        public MetricasController(IMetricasService metricasService, ILogger<MetricasController> logger)
        {
            _metricasService = metricasService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene los KPIs principales del dashboard
        /// </summary>
        /// <param name="desde">Fecha de inicio (opcional)</param>
        /// <param name="hasta">Fecha de fin (opcional)</param>
        /// <returns>KPIs principales: totales, tiempos, SLA, FCR, tendencias</returns>
        [HttpGet("kpis")]
        [ProducesResponseType(typeof(ApiResponse<DashboardKPIsDto>), 200)]
        public async Task<IActionResult> ObtenerKPIsPrincipales([FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null)
        {
            try
            {
                var result = await _metricasService.ObtenerKPIsPrincipalesAsync(desde, hasta);
                
                if (!result.Success)
                {
                    _logger.LogWarning("Error al obtener KPIs: {Message}", result.Message);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener KPIs principales");
                return StatusCode(500, new ApiResponse<DashboardKPIsDto>(false, new DashboardKPIsDto(), 
                    "Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene estadísticas agrupadas por categoría de incidentes
        /// </summary>
        /// <param name="desde">Fecha de inicio (opcional)</param>
        /// <param name="hasta">Fecha de fin (opcional)</param>
        /// <returns>Estadísticas por categoría: totales, tiempos, SLA, porcentaje del total</returns>
        [HttpGet("categorias")]
        [ProducesResponseType(typeof(ApiResponse<List<EstadisticasPorCategoriaDto>>), 200)]
        public async Task<IActionResult> ObtenerEstadisticasPorCategoria([FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null)
        {
            try
            {
                var result = await _metricasService.ObtenerEstadisticasPorCategoriaAsync(desde, hasta);
                
                if (!result.Success)
                {
                    _logger.LogWarning("Error al obtener estadísticas por categoría: {Message}", result.Message);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener estadísticas por categoría");
                return StatusCode(500, new ApiResponse<List<EstadisticasPorCategoriaDto>>(false, 
                    new List<EstadisticasPorCategoriaDto>(), "Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene estadísticas de rendimiento por técnico
        /// </summary>
        /// <param name="desde">Fecha de inicio (opcional)</param>
        /// <param name="hasta">Fecha de fin (opcional)</param>
        /// <returns>Estadísticas por técnico: asignados, resueltos, tiempos, tasa de resolución, SLA</returns>
        [HttpGet("tecnicos")]
        [ProducesResponseType(typeof(ApiResponse<List<EstadisticasPorTecnicoDto>>), 200)]
        public async Task<IActionResult> ObtenerEstadisticasPorTecnico([FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null)
        {
            try
            {
                var result = await _metricasService.ObtenerEstadisticasPorTecnicoAsync(desde, hasta);
                
                if (!result.Success)
                {
                    _logger.LogWarning("Error al obtener estadísticas por técnico: {Message}", result.Message);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener estadísticas por técnico");
                return StatusCode(500, new ApiResponse<List<EstadisticasPorTecnicoDto>>(false, 
                    new List<EstadisticasPorTecnicoDto>(), "Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene tendencias de incidentes en el tiempo
        /// </summary>
        /// <param name="ultimosDias">Número de días a analizar (por defecto 30)</param>
        /// <returns>Tendencias diarias y mensuales: incidentes creados, resueltos, tiempos promedio</returns>
        [HttpGet("tendencias")]
        [ProducesResponseType(typeof(ApiResponse<TendenciasDto>), 200)]
        public async Task<IActionResult> ObtenerTendencias([FromQuery] int ultimosDias = 30)
        {
            try
            {
                if (ultimosDias < 1 || ultimosDias > 365)
                {
                    return BadRequest(new ApiResponse<TendenciasDto>(false, new TendenciasDto(), 
                        "El parámetro ultimosDias debe estar entre 1 y 365"));
                }

                var result = await _metricasService.ObtenerTendenciasAsync(ultimosDias);
                
                if (!result.Success)
                {
                    _logger.LogWarning("Error al obtener tendencias: {Message}", result.Message);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener tendencias");
                return StatusCode(500, new ApiResponse<TendenciasDto>(false, new TendenciasDto(), 
                    "Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene reporte de disponibilidad de servicios DITIC
        /// </summary>
        /// <param name="desde">Fecha de inicio (opcional)</param>
        /// <param name="hasta">Fecha de fin (opcional)</param>
        /// <returns>Reporte de disponibilidad: objetivo vs real, incidentes críticos, tiempo de inactividad</returns>
        [HttpGet("disponibilidad")]
        [ProducesResponseType(typeof(ApiResponse<ReporteDisponibilidadDto>), 200)]
        public async Task<IActionResult> ObtenerReporteDisponibilidad([FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null)
        {
            try
            {
                var result = await _metricasService.ObtenerReporteDisponibilidadAsync(desde, hasta);
                
                if (!result.Success)
                {
                    _logger.LogWarning("Error al obtener reporte de disponibilidad: {Message}", result.Message);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener reporte de disponibilidad");
                return StatusCode(500, new ApiResponse<ReporteDisponibilidadDto>(false, 
                    new ReporteDisponibilidadDto(), "Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene los incidentes más frecuentes para análisis preventivo
        /// </summary>
        /// <param name="top">Número de incidentes a retornar (por defecto 10)</param>
        /// <param name="desde">Fecha de inicio (opcional, por defecto últimos 90 días)</param>
        /// <param name="hasta">Fecha de fin (opcional)</param>
        /// <returns>Top incidentes frecuentes: título, categoría, ocurrencias, tiempo promedio, solución KB</returns>
        [HttpGet("frecuentes")]
        [ProducesResponseType(typeof(ApiResponse<TopIncidentesDto>), 200)]
        public async Task<IActionResult> ObtenerTopIncidentesFrecuentes(
            [FromQuery] int top = 10, 
            [FromQuery] DateTime? desde = null, 
            [FromQuery] DateTime? hasta = null)
        {
            try
            {
                if (top < 1 || top > 100)
                {
                    return BadRequest(new ApiResponse<TopIncidentesDto>(false, new TopIncidentesDto(), 
                        "El parámetro top debe estar entre 1 y 100"));
                }

                var result = await _metricasService.ObtenerTopIncidentesFrecuentesAsync(top, desde, hasta);
                
                if (!result.Success)
                {
                    _logger.LogWarning("Error al obtener top incidentes frecuentes: {Message}", result.Message);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener top incidentes frecuentes");
                return StatusCode(500, new ApiResponse<TopIncidentesDto>(false, new TopIncidentesDto(), 
                    "Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene un resumen completo de métricas para exportación
        /// </summary>
        /// <param name="desde">Fecha de inicio (opcional)</param>
        /// <param name="hasta">Fecha de fin (opcional)</param>
        /// <returns>Objeto combinado con KPIs, categorías y técnicos</returns>
        [HttpGet("resumen")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ObtenerResumenCompleto([FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null)
        {
            try
            {
                var kpis = await _metricasService.ObtenerKPIsPrincipalesAsync(desde, hasta);
                var categorias = await _metricasService.ObtenerEstadisticasPorCategoriaAsync(desde, hasta);
                var tecnicos = await _metricasService.ObtenerEstadisticasPorTecnicoAsync(desde, hasta);
                var tendencias = await _metricasService.ObtenerTendenciasAsync(30);
                var disponibilidad = await _metricasService.ObtenerReporteDisponibilidadAsync(desde, hasta);
                var frecuentes = await _metricasService.ObtenerTopIncidentesFrecuentesAsync(10, desde, hasta);

                var resumen = new
                {
                    FechaGeneracion = DateTime.UtcNow,
                    Periodo = new { Desde = desde ?? DateTime.UtcNow.AddDays(-30), Hasta = hasta ?? DateTime.UtcNow },
                    KPIs = kpis.Data,
                    Categorias = categorias.Data,
                    Tecnicos = tecnicos.Data,
                    Tendencias = tendencias.Data,
                    Disponibilidad = disponibilidad.Data,
                    IncidentesFrecuentes = frecuentes.Data
                };

                return Ok(new ApiResponse<object>(true, resumen, "Resumen completo generado correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al generar resumen completo");
                return StatusCode(500, new ApiResponse<object>(false, null, "Error interno del servidor"));
            }
        }
    }
}
