using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentesFISEI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EscalacionController : ControllerBase
    {
        private readonly IEscalacionService _escalacionService;
        private readonly ILogger<EscalacionController> _logger;

        public EscalacionController(
            IEscalacionService escalacionService,
            ILogger<EscalacionController> logger)
        {
            _escalacionService = escalacionService;
            _logger = logger;
        }

        /// <summary>
        /// Escala un incidente al siguiente nivel de soporte
        /// </summary>
        /// <param name="incidenteId">ID del incidente a escalar</param>
        /// <param name="request">Datos de la escalación</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("escalar/{incidenteId}")]
        [Authorize(Roles = "Tecnico,Supervisor,Administrador")]
        public async Task<IActionResult> EscalarIncidente(
            int incidenteId, 
            [FromBody] EscalarIncidenteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Razon))
            {
                return BadRequest(new { message = "Debe proporcionar una razón para la escalación" });
            }

            var resultado = await _escalacionService.EscalarIncidenteAsync(
                incidenteId, 
                request.Razon, 
                request.TecnicoDestinoId);

            if (!resultado.Success)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        /// <summary>
        /// Escala un incidente a un nivel específico
        /// </summary>
        /// <param name="incidenteId">ID del incidente</param>
        /// <param name="request">Nivel destino y razón</param>
        [HttpPost("escalar-a-nivel/{incidenteId}")]
        [Authorize(Roles = "Supervisor,Administrador")]
        public async Task<IActionResult> EscalarANivelEspecifico(
            int incidenteId,
            [FromBody] EscalarANivelRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Razon))
            {
                return BadRequest(new { message = "Debe proporcionar una razón para la escalación" });
            }

            var resultado = await _escalacionService.EscalarANivelEspecificoAsync(
                incidenteId,
                request.NivelDestino,
                request.Razon,
                request.TecnicoDestinoId);

            if (!resultado.Success)
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        /// <summary>
        /// Verifica si un incidente debe ser escalado automáticamente
        /// </summary>
        /// <param name="incidenteId">ID del incidente</param>
        [HttpPost("verificar-escalacion-automatica/{incidenteId}")]
        [Authorize(Roles = "Tecnico,Supervisor,Administrador")]
        public async Task<IActionResult> VerificarEscalacionAutomatica(int incidenteId)
        {
            var resultado = await _escalacionService.VerificarEscalacionAutomaticaAsync(incidenteId);
            return Ok(resultado);
        }

        /// <summary>
        /// Obtiene el técnico disponible de un nivel específico con menor carga
        /// </summary>
        /// <param name="nivel">Nivel de soporte deseado</param>
        [HttpGet("tecnico-disponible/{nivel}")]
        [Authorize(Roles = "Tecnico,Supervisor,Administrador")]
        public async Task<IActionResult> ObtenerTecnicoDisponible(NivelSoporte nivel)
        {
            var resultado = await _escalacionService.ObtenerTecnicoDisponiblePorNivelAsync(nivel);
            
            if (!resultado.Success)
            {
                return NotFound(resultado);
            }

            return Ok(resultado);
        }

        /// <summary>
        /// Obtiene estadísticas generales de escalación
        /// </summary>
        [HttpGet("estadisticas")]
        [Authorize(Roles = "Supervisor,Administrador")]
        public async Task<IActionResult> ObtenerEstadisticas()
        {
            var resultado = await _escalacionService.ObtenerEstadisticasEscalacionAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Procesa escalaciones automáticas para todos los incidentes que lo requieran
        /// (Normalmente ejecutado por un background service)
        /// </summary>
        [HttpPost("procesar-automaticas")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ProcesarEscalacionesAutomaticas()
        {
            var cantidadEscalados = await _escalacionService.ProcesarEscalacionesAutomaticasAsync();
            
            return Ok(new 
            { 
                success = true,
                cantidadEscalados,
                message = $"Se procesaron {cantidadEscalados} escalaciones automáticas"
            });
        }

        /// <summary>
        /// Obtiene el historial de escalaciones de un incidente
        /// </summary>
        /// <param name="incidenteId">ID del incidente</param>
        [HttpGet("historial/{incidenteId}")]
        [Authorize(Roles = "Tecnico,Supervisor,Administrador")]
        public async Task<IActionResult> ObtenerHistorialEscalaciones(int incidenteId)
        {
            var resultado = await _escalacionService.ObtenerHistorialEscalacionesAsync(incidenteId);
            return Ok(resultado);
        }
    }

    // Request DTOs
    public class EscalarIncidenteRequest
    {
        public string Razon { get; set; } = string.Empty;
        public int? TecnicoDestinoId { get; set; }
    }

    public class EscalarANivelRequest
    {
        public NivelSoporte NivelDestino { get; set; }
        public string Razon { get; set; } = string.Empty;
        public int? TecnicoDestinoId { get; set; }
    }
}
