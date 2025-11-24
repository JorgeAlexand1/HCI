using Microsoft.AspNetCore.Mvc;
using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.DTOs;
using FISEI.Incidentes.Core.Interfaces.IServices;
using FISEI.Incidentes.Infrastructure.Data.Repositories;
using FISEI.Incidentes.Core.Interfaces.IRepositories;

namespace FISEI.Incidentes.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentesController : ControllerBase
    {
        private readonly IIncidenteRepository _incidenteRepository;
        private readonly IAsignacionService _asignacionService;
        private readonly INotificacionService _notificacionService;

        public IncidentesController(
            IIncidenteRepository incidenteRepository,
            IAsignacionService asignacionService,
            INotificacionService notificacionService)
        {
            _incidenteRepository = incidenteRepository;
            _asignacionService = asignacionService;
            _notificacionService = notificacionService;
        }

        /// <summary>
        /// Obtiene todos los incidentes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incidente>>> GetIncidentes()
        {
            var incidentes = await _incidenteRepository.GetAllAsync();
            return Ok(incidentes);
        }

        /// <summary>
        /// Obtiene un incidente por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Incidente>> GetIncidente(int id)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(id);
            
            if (incidente == null)
                return NotFound(new { message = "Incidente no encontrado" });

            return Ok(incidente);
        }

        /// <summary>
        /// Crea un nuevo incidente
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Incidente>> CrearIncidente([FromBody] CrearIncidenteDTO incidenteDto)
        {
            try
            {
                var incidente = new Incidente
                {
                    Titulo = incidenteDto.Titulo,
                    Descripcion = incidenteDto.Descripcion,
                    IdCategoria = incidenteDto.IdCategoria,
                    IdServicio = incidenteDto.IdServicio,
                    IdUsuario = incidenteDto.IdUsuario,
                    IdEstado = 1, // Abierto por defecto
                    IdNivelSoporte = 1, // N1 por defecto
                    FechaCreacion = DateTime.Now
                };

                var nuevoIncidente = await _incidenteRepository.AddAsync(incidente);

                // Notificar creación
                await _notificacionService.NotificarNuevoIncidenteAsync(nuevoIncidente.IdIncidente);

                // Asignar automáticamente
                await _asignacionService.AsignarAutomaticamenteAsync(nuevoIncidente.IdIncidente);

                return CreatedAtAction(
                    nameof(GetIncidente), 
                    new { id = nuevoIncidente.IdIncidente }, 
                    nuevoIncidente
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un incidente existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarIncidente(int id, [FromBody] Incidente incidente)
        {
            if (id != incidente.IdIncidente)
                return BadRequest(new { message = "El ID no coincide" });

            try
            {
                await _incidenteRepository.UpdateAsync(incidente);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un incidente
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarIncidente(int id)
        {
            try
            {
                await _incidenteRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene incidentes por estado
        /// </summary>
        [HttpGet("estado/{idEstado}")]
        public async Task<ActionResult<IEnumerable<Incidente>>> GetIncidentesPorEstado(int idEstado)
        {
            var incidentes = await _incidenteRepository.GetIncidentesPorEstadoAsync(idEstado);
            return Ok(incidentes);
        }

        /// <summary>
        /// Obtiene incidentes de un usuario
        /// </summary>
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<Incidente>>> GetIncidentesPorUsuario(int idUsuario)
        {
            var incidentes = await _incidenteRepository.GetIncidentesPorUsuarioAsync(idUsuario);
            return Ok(incidentes);
        }

        /// <summary>
        /// Obtiene incidentes sin asignar (para SPOC)
        /// </summary>
        [HttpGet("sin-asignar")]
        public async Task<ActionResult<IEnumerable<Incidente>>> GetIncidentesSinAsignar()
        {
            var incidentes = await _incidenteRepository.GetIncidentesSinAsignarAsync();
            return Ok(incidentes);
        }

        /// <summary>
        /// Obtiene incidentes recurrentes (Problem Management)
        /// </summary>
        [HttpGet("recurrentes/{idCategoria}")]
        public async Task<ActionResult<IEnumerable<Incidente>>> GetIncidentesRecurrentes(int idCategoria, [FromQuery] int minOcurrencias = 3)
        {
            var incidentes = await _incidenteRepository.GetIncidentesRecurrentesAsync(idCategoria, minOcurrencias);
            return Ok(incidentes);
        }
    }
}