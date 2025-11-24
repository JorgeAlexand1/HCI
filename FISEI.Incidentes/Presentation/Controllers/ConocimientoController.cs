using Microsoft.AspNetCore.Mvc;
using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IServices;
using FISEI.Incidentes.Core.Interfaces.IRepositories;

namespace FISEI.Incidentes.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConocimientoController : ControllerBase
    {
        private readonly IConocimientoService _conocimientoService;
        private readonly IConocimientoRepository _conocimientoRepository;

        public ConocimientoController(
            IConocimientoService conocimientoService,
            IConocimientoRepository conocimientoRepository)
        {
            _conocimientoService = conocimientoService;
            _conocimientoRepository = conocimientoRepository;
        }

        /// <summary>
        /// Obtiene todos los artículos aprobados
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Conocimiento>>> GetArticulosAprobados()
        {
            var articulos = await _conocimientoRepository.GetArticulosAprobadosAsync();
            return Ok(articulos);
        }

        /// <summary>
        /// Busca soluciones por palabras clave
        /// </summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Conocimiento>>> BuscarSoluciones([FromQuery] string palabrasClave)
        {
            var soluciones = await _conocimientoService.BuscarSolucionesAsync(palabrasClave);
            return Ok(soluciones);
        }

        /// <summary>
        /// Obtiene solución similar para un incidente
        /// </summary>
        [HttpGet("solucion-similar/{idIncidente}")]
        public async Task<ActionResult<Conocimiento>> GetSolucionSimilar(int idIncidente)
        {
            var solucion = await _conocimientoService.ObtenerSolucionSimilarAsync(idIncidente);
            
            if (solucion == null)
                return NotFound(new { message = "No se encontró una solución similar" });

            return Ok(solucion);
        }

        /// <summary>
        /// Crea un artículo desde un incidente resuelto
        /// </summary>
        [HttpPost("crear-desde-incidente/{idIncidente}")]
        public async Task<ActionResult<Conocimiento>> CrearArticulo(int idIncidente)
        {
            try
            {
                var articulo = await _conocimientoService.CrearArticuloAsync(idIncidente);
                return CreatedAtAction(nameof(GetArticulo), new { id = articulo.IdConocimiento }, articulo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un artículo por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Conocimiento>> GetArticulo(int id)
        {
            var articulo = await _conocimientoRepository.GetByIdAsync(id);
            
            if (articulo == null)
                return NotFound(new { message = "Artículo no encontrado" });

            // Incrementar visualizaciones
            await _conocimientoRepository.IncrementarVisualizacionesAsync(id);

            return Ok(articulo);
        }

        /// <summary>
        /// Valora un artículo (1-5 estrellas)
        /// </summary>
        [HttpPut("{id}/valorar")]
        public async Task<IActionResult> ValorarArticulo(int id, [FromQuery] int calificacion)
        {
            try
            {
                await _conocimientoService.ValorarArticuloAsync(id, calificacion);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}