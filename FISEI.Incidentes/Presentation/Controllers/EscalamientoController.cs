using Microsoft.AspNetCore.Mvc;
using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IServices;

namespace FISEI.Incidentes.Presentation.Controllers
{
    // DTO para el escalamiento
    public class EscalarIncidenteRequest
    {
        public string MotivoEscalamiento { get; set; } = null!;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class EscalamientoController : ControllerBase
    {
        private readonly IEscalamientoService _escalamientoService;

        public EscalamientoController(IEscalamientoService escalamientoService)
        {
            _escalamientoService = escalamientoService;
        }

        /// <summary>
        /// Escala un incidente manualmente al siguiente nivel
        /// </summary>
        [HttpPost("escalar/{idIncidente}")]
        public async Task<ActionResult<Incidente>> EscalarIncidente(
            int idIncidente,
            [FromBody] EscalarIncidenteRequest request)
        {
            try
            {
                var incidente = await _escalamientoService.EscalarIncidenteAsync(
                    idIncidente,
                    request.MotivoEscalamiento
                );
                return Ok(incidente);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Verifica si un incidente debe escalar por tiempo
        /// </summary>
        [HttpGet("verificar-tiempo/{idIncidente}")]
        public async Task<ActionResult<bool>> DebeEscalarPorTiempo(int idIncidente)
        {
            var debeEscalar = await _escalamientoService.DebeEscalarPorTiempoAsync(idIncidente);
            return Ok(new { debeEscalar });
        }

        /// <summary>
        /// Verifica si un incidente debe escalar por recurrencia
        /// </summary>
        [HttpGet("verificar-recurrencia/{idIncidente}")]
        public async Task<ActionResult<bool>> DebeEscalarPorRecurrencia(int idIncidente)
        {
            var debeEscalar = await _escalamientoService.DebeEscalarPorRecurrenciaAsync(idIncidente);
            return Ok(new { debeEscalar });
        }

        /// <summary>
        /// Ejecuta el escalamiento automático (proceso en segundo plano)
        /// </summary>
        [HttpPost("escalar-automatico")]
        public async Task<IActionResult> EscalarAutomaticamente()
        {
            try
            {
                await _escalamientoService.EscalarIncidentesAutomaticamenteAsync();
                return Ok(new { message = "Proceso de escalamiento automático completado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}