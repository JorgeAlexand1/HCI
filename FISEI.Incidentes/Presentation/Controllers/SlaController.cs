using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FISEI.Incidentes.Application.Services;
using System.Threading.Tasks;

namespace FISEI.Incidentes.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SlaController : ControllerBase
    {
        private readonly SlaService _slaService;
        public SlaController(SlaService slaService)
        {
            _slaService = slaService;
        }

        // GET api/sla/metrics
        [HttpGet("metrics")]
        public async Task<IActionResult> GetMetrics()
        {
            var metrics = await _slaService.GetGlobalMetricsAsync();
            return Ok(metrics);
        }

        // GET api/sla/incident/5
        [HttpGet("incident/{id:int}")]
        public async Task<IActionResult> GetIncident(int id)
        {
            var detail = await _slaService.GetIncidentSlaAsync(id);
            if (detail == null) return NotFound(new { message = "Incidente o SLA no encontrado" });
            return Ok(detail);
        }
    }
}