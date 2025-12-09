using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace IncidentesFISEI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;
        private readonly PdfGeneratorService _pdfGenerator;

        public ReportsController(IReportService reportService, ILogger<ReportsController> logger, PdfGeneratorService pdfGenerator)
        {
            _reportService = reportService;
            _logger = logger;
            _pdfGenerator = pdfGenerator;
        }

        /// <summary>
        /// Obtiene el reporte de rendimiento del sistema
        /// </summary>
        [HttpGet("performance")]
        public async Task<ActionResult<PerformanceReportDto>> GetPerformanceReport()
        {
            try
            {
                var report = await _reportService.GetPerformanceReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de rendimiento");
                return StatusCode(500, new { message = "Error al obtener reporte de rendimiento", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el reporte de cumplimiento SLA
        /// </summary>
        [HttpGet("sla")]
        public async Task<ActionResult<SLAReportDto>> GetSLAReport()
        {
            try
            {
                var report = await _reportService.GetSLAReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte SLA");
                return StatusCode(500, new { message = "Error al obtener reporte SLA", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el reporte de actividad de usuarios
        /// </summary>
        [HttpGet("user-activity")]
        public async Task<ActionResult<UserActivityReportDto>> GetUserActivityReport()
        {
            try
            {
                var report = await _reportService.GetUserActivityReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de actividad");
                return StatusCode(500, new { message = "Error al obtener reporte de actividad", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los reportes (dashboard completo)
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            try
            {
                var stats = await _reportService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas del dashboard");
                return StatusCode(500, new { message = "Error al obtener estadísticas", error = ex.Message });
            }
        }

        /// <summary>
        /// Exporta un reporte completo en formato PDF
        /// </summary>
        [HttpGet("export-pdf")]
        public async Task<IActionResult> ExportPdf()
        {
            try
            {
                var pdfBytes = await _pdfGenerator.GenerateReportPdfAsync();
                var fileName = $"Reporte_Incidentes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar PDF");
                return StatusCode(500, new { message = "Error al generar PDF", error = ex.Message });
            }
        }
    }
}
