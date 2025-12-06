using Microsoft.AspNetCore.Mvc;
using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;

namespace IncidentesFISEI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiciosController : ControllerBase
{
    private readonly IServicioService _servicioService;

    public ServiciosController(IServicioService servicioService)
    {
        _servicioService = servicioService;
    }

    /// <summary>
    /// Obtiene todos los servicios
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServicioListDto>>> GetServicios()
    {
        try
        {
            var servicios = await _servicioService.GetAllServiciosAsync();
            return Ok(servicios);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene servicios por categoría
    /// </summary>
    [HttpGet("categoria/{categoriaId}")]
    public async Task<ActionResult<IEnumerable<ServicioListDto>>> GetServiciosByCategoria(int categoriaId)
    {
        try
        {
            var servicios = await _servicioService.GetServiciosByCategoriaAsync(categoriaId);
            return Ok(servicios);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene un servicio por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ServicioDto>> GetServicio(int id)
    {
        try
        {
            var servicio = await _servicioService.GetServicioByIdAsync(id);
            
            if (servicio == null)
            {
                return NotFound($"No se encontró el servicio con ID {id}");
            }

            return Ok(servicio);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    /// <summary>
    /// Busca servicios por término de búsqueda
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ServicioListDto>>> SearchServicios([FromQuery] string searchTerm)
    {
        try
        {
            var servicios = await _servicioService.SearchServiciosAsync(searchTerm);
            return Ok(servicios);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    /// <summary>
    /// Crea un nuevo servicio
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ServicioDto>> CreateServicio([FromBody] CreateServicioDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var servicio = await _servicioService.CreateServicioAsync(createDto);
            return CreatedAtAction(nameof(GetServicio), new { id = servicio.Id }, servicio);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    /// <summary>
    /// Actualiza un servicio existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ServicioDto>> UpdateServicio(int id, [FromBody] UpdateServicioDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var servicio = await _servicioService.UpdateServicioAsync(id, updateDto);
            
            if (servicio == null)
            {
                return NotFound($"No se encontró el servicio con ID {id}");
            }

            return Ok(servicio);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    /// <summary>
    /// Elimina un servicio
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServicio(int id)
    {
        try
        {
            var deleted = await _servicioService.DeleteServicioAsync(id);
            
            if (!deleted)
            {
                return NotFound($"No se encontró el servicio con ID {id}");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    /// <summary>
    /// Verifica si existe un código de servicio
    /// </summary>
    [HttpGet("exists-codigo")]
    public async Task<ActionResult<bool>> ExistsCodigo([FromQuery] string codigo, [FromQuery] int? excludeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return BadRequest("El código es requerido");
            }

            var exists = await _servicioService.ExistsCodigoAsync(codigo, excludeId);
            return Ok(exists);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}