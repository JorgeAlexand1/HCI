using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ISolicitudEscalafonService _solicitudService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        ISolicitudEscalafonService solicitudService,
        ILogger<ReportsController> logger)
    {
        _solicitudService = solicitudService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene un reporte de solicitudes de escalafón con filtros
    /// </summary>
    [HttpGet("solicitudes")]
    public async Task<ActionResult<ApiResponse<List<SolicitudEscalafonDto>>>> GetSolicitudesReport(
        [FromQuery] string? status = null,
        [FromQuery] string? promotionType = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? search = null)
    {
        try
        {
            _logger.LogInformation("Generando reporte de solicitudes con filtros: Status={Status}, PromotionType={PromotionType}, StartDate={StartDate}, EndDate={EndDate}, Search={Search}",
                status, promotionType, startDate, endDate, search);

            var solicitudes = await _solicitudService.GetAllSolicitudesAsync();

            // Aplicar filtros
            var filteredSolicitudes = solicitudes.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                filteredSolicitudes = filteredSolicitudes.Where(s => s.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(promotionType))
            {
                filteredSolicitudes = filteredSolicitudes.Where(s => 
                    s.NivelSolicitado.Equals(promotionType, StringComparison.OrdinalIgnoreCase));
            }

            if (startDate.HasValue)
            {
                filteredSolicitudes = filteredSolicitudes.Where(s => s.FechaSolicitud >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                filteredSolicitudes = filteredSolicitudes.Where(s => s.FechaSolicitud <= endDate.Value.AddDays(1));
            }

            if (!string.IsNullOrEmpty(search))
            {
                filteredSolicitudes = filteredSolicitudes.Where(s => 
                    s.DocenteNombre.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    s.DocenteCedula.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    s.DocenteEmail.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var result = filteredSolicitudes
                .OrderByDescending(s => s.FechaSolicitud)
                .ToList();

            _logger.LogInformation("Reporte generado exitosamente. Total solicitudes: {Count}", result.Count);

            return Ok(new ApiResponse<List<SolicitudEscalafonDto>>
            {
                Success = true,
                Data = result,
                Message = $"Reporte generado exitosamente. {result.Count} solicitudes encontradas."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte de solicitudes");
            return StatusCode(500, new ApiResponse<List<SolicitudEscalafonDto>>
            {
                Success = false,
                Message = "Error interno del servidor al generar el reporte"
            });
        }
    }

    /// <summary>
    /// Obtiene estadísticas generales del sistema
    /// </summary>
    [HttpGet("estadisticas")]
    public async Task<ActionResult<ApiResponse<ReportStatisticsDto>>> GetEstadisticas()
    {
        try
        {
            _logger.LogInformation("Generando estadísticas generales del sistema");

            var solicitudes = await _solicitudService.GetAllSolicitudesAsync();
            var solicitudesList = solicitudes.ToList();

            var estadisticas = new ReportStatisticsDto
            {
                TotalSolicitudes = solicitudesList.Count,
                SolicitudesPendientes = solicitudesList.Count(s => s.Status == "Pendiente"),
                SolicitudesEnRevision = solicitudesList.Count(s => s.Status == "EnRevision"),
                SolicitudesVerificadas = solicitudesList.Count(s => s.Status == "Verificado"),
                SolicitudesAprobadas = solicitudesList.Count(s => s.Status == "AprobadoConsejo" || s.Status == "Procesado"),
                SolicitudesRechazadas = solicitudesList.Count(s => s.Status.Contains("Rechazado")),
                SolicitudesFinalizadas = solicitudesList.Count(s => s.Status == "Finalizado"),
                PromocionesAuxiliar = solicitudesList.Count(s => s.NivelSolicitado == "Auxiliar"),
                PromocionesAgregado = solicitudesList.Count(s => s.NivelSolicitado == "Agregado"),
                PromocionesPrincipal = solicitudesList.Count(s => s.NivelSolicitado == "Principal"),
                PromocionesTitular = solicitudesList.Count(s => s.NivelSolicitado == "Titular"),
                SolicitudesEsteMes = solicitudesList.Count(s => s.FechaSolicitud.Month == DateTime.Now.Month && s.FechaSolicitud.Year == DateTime.Now.Year),
                SolicitudesEsteAno = solicitudesList.Count(s => s.FechaSolicitud.Year == DateTime.Now.Year)
            };

            // Estadísticas por facultad
            estadisticas.SolicitudesPorFacultad = solicitudesList
                .Where(s => !string.IsNullOrEmpty(s.Facultad))
                .GroupBy(s => s.Facultad)
                .ToDictionary(g => g.Key!, g => g.Count());

            // Estadísticas por estado
            estadisticas.SolicitudesPorEstado = solicitudesList
                .GroupBy(s => s.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            _logger.LogInformation("Estadísticas generadas exitosamente");

            return Ok(new ApiResponse<ReportStatisticsDto>
            {
                Success = true,
                Data = estadisticas,
                Message = "Estadísticas generadas exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar estadísticas del sistema");
            return StatusCode(500, new ApiResponse<ReportStatisticsDto>
            {
                Success = false,
                Message = "Error interno del servidor al generar las estadísticas"
            });
        }
    }

    /// <summary>
    /// Obtiene el detalle de una solicitud específica para exportación
    /// </summary>
    [HttpGet("solicitud/{id}/detalle")]
    public async Task<ActionResult<ApiResponse<SolicitudEscalafonDto>>> GetSolicitudDetalle(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo detalle de solicitud {SolicitudId} para exportación", id);

            var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);

            if (solicitud == null)
            {
                return NotFound(new ApiResponse<SolicitudEscalafonDto>
                {
                    Success = false,
                    Message = "Solicitud no encontrada"
                });
            }

            return Ok(new ApiResponse<SolicitudEscalafonDto>
            {
                Success = true,
                Data = solicitud,
                Message = "Detalle de solicitud obtenido exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalle de solicitud {SolicitudId}", id);
            return StatusCode(500, new ApiResponse<SolicitudEscalafonDto>
            {
                Success = false,
                Message = "Error interno del servidor al obtener el detalle de la solicitud"
            });
        }
    }

    /// <summary>
    /// Obtiene los detalles completos de una solicitud específica
    /// </summary>
    [HttpGet("solicitud/{id}")]
    public async Task<ActionResult<ApiResponse<SolicitudEscalafonDto>>> GetSolicitudById(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo detalles de solicitud {SolicitudId}", id);

            var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);

            if (solicitud == null)
            {
                return NotFound(new ApiResponse<SolicitudEscalafonDto>
                {
                    Success = false,
                    Message = "Solicitud no encontrada"
                });
            }

            return Ok(new ApiResponse<SolicitudEscalafonDto>
            {
                Success = true,
                Data = solicitud,
                Message = "Detalles de solicitud obtenidos exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalles de solicitud {SolicitudId}", id);
            return StatusCode(500, new ApiResponse<SolicitudEscalafonDto>
            {
                Success = false,
                Message = "Error interno del servidor al obtener los detalles de la solicitud"
            });
        }
    }

    /// <summary>
    /// Obtiene el historial y documentos utilizados en una solicitud de escalafón
    /// </summary>
    [HttpGet("historial/{solicitudId}")]
    public ActionResult<ApiResponse<HistorialEscalafonDto>> GetHistorialSolicitud(int solicitudId)
    {
        try
        {
            _logger.LogInformation("Obteniendo historial y documentos de solicitud {SolicitudId}", solicitudId);

            // Simulación de datos del historial con documentos - esto debería conectarse a la base de datos real
            var historial = new HistorialEscalafonDto
            {
                Id = solicitudId,
                NivelAnterior = "Auxiliar",
                NivelNuevo = "Agregado",
                FechaPromocion = DateTime.Now,
                EstadoSolicitud = "Procesado",
                DocumentosUtilizados = new List<string>
                {
                    "Curriculum Vitae actualizado",
                    "Certificados de capacitación",
                    "Artículos científicos publicados",
                    "Evaluaciones docentes",
                    "Certificados de títulos académicos"
                },
                DocumentosDetalles = new DocumentosDetallados
                {
                    Investigaciones = new List<InvestigacionUtilizada>
                    {
                        new InvestigacionUtilizada
                        {
                            Id = 1,
                            Titulo = "Análisis de metodologías ágiles en el desarrollo de software",
                            Tipo = "Artículo Científico",
                            RevistaOEditorial = "Revista de Tecnología Educativa",
                            FechaPublicacion = DateTime.Now.AddMonths(-6),
                            Filiacion = "Universidad Técnica de Ambato",
                            TieneFiliacionUTA = true
                        },
                        new InvestigacionUtilizada
                        {
                            Id = 2,
                            Titulo = "Impacto de las TIC en la educación superior",
                            Tipo = "Capítulo de Libro",
                            RevistaOEditorial = "Editorial Académica",
                            FechaPublicacion = DateTime.Now.AddMonths(-12),
                            Filiacion = "Universidad Técnica de Ambato",
                            TieneFiliacionUTA = true
                        }
                    },
                    Evaluaciones = new List<EvaluacionUtilizada>
                    {
                        new EvaluacionUtilizada
                        {
                            Id = 1,
                            PeriodoAcademico = "2023-2024",
                            Anio = 2023,
                            Semestre = 1,
                            PuntajeObtenido = 85.5m,
                            PuntajeMaximo = 100m,
                            Porcentaje = 85.5m,
                            Estado = "Aprobado"
                        },
                        new EvaluacionUtilizada
                        {
                            Id = 2,
                            PeriodoAcademico = "2023-2024",
                            Anio = 2023,
                            Semestre = 2,
                            PuntajeObtenido = 88.0m,
                            PuntajeMaximo = 100m,
                            Porcentaje = 88.0m,
                            Estado = "Aprobado"
                        }
                    },
                    Capacitaciones = new List<CapacitacionUtilizada>
                    {
                        new CapacitacionUtilizada
                        {
                            Id = 1,
                            NombreCurso = "Metodologías Activas de Aprendizaje",
                            Facilitador = "Dr. María González",
                            HorasAcademicas = 40,
                            FechaInicio = DateTime.Now.AddMonths(-3),
                            FechaFin = DateTime.Now.AddMonths(-2),
                            Tipo = "Presencial",
                            EsPedagogica = true
                        },
                        new CapacitacionUtilizada
                        {
                            Id = 2,
                            NombreCurso = "Investigación Científica y Publicación",
                            Facilitador = "Dr. Carlos Pérez",
                            HorasAcademicas = 60,
                            FechaInicio = DateTime.Now.AddMonths(-8),
                            FechaFin = DateTime.Now.AddMonths(-7),
                            Tipo = "Virtual",
                            EsPedagogica = false
                        }
                    },
                    VerificacionRequisitos = new VerificacionRequisitos
                    {
                        TotalInvestigaciones = 2,
                        InvestigacionesConUTA = 2,
                        TotalHorasCapacitacion = 100,
                        HorasPedagogicas = 40,
                        PromedioEvaluaciones = 86.75m,
                        PeriodosEvaluados = 2,
                        CumpleTodosRequisitos = true
                    }
                },
                ObservacionesFinales = "El docente cumple con todos los requisitos para la promoción al nivel Agregado.",
                AprobadoPor = "Comisión Académica de Escalafón"
            };

            return Ok(new ApiResponse<HistorialEscalafonDto>
            {
                Success = true,
                Data = historial,
                Message = "Historial y documentos obtenidos exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de solicitud {SolicitudId}", solicitudId);
            return StatusCode(500, new ApiResponse<HistorialEscalafonDto>
            {
                Success = false,
                Message = "Error interno del servidor al obtener el historial de la solicitud"
            });
        }
    }
}
