using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IncidentesFISEI.Infrastructure.Data;
using IncidentesFISEI.Application.DTOs;

namespace IncidentesFISEI.Api.Controllers;

/// <summary>
/// Controlador para la gestión de categorías de incidentes
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize] // Temporalmente deshabilitado
public class CategoriasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(ApplicationDbContext context, ILogger<CategoriasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todas las categorías activas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoriaIncidenteDto>), 200)]
    public async Task<IActionResult> GetCategoriasActivas()
    {
        try
        {
            var categorias = await _context.Categorias
                .Where(c => !c.IsDeleted && c.IsActive)
                .OrderBy(c => c.Nombre)
                .Select(c => new CategoriaIncidenteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Activa = c.IsActive,
                    FechaCreacion = c.CreatedAt
                })
                .ToListAsync();

            return Ok(categorias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías de incidentes");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtener una categoría por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoriaIncidenteDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCategoria(int id)
    {
        try
        {
            var categoria = await _context.Categorias
                .Where(c => c.Id == id && !c.IsDeleted)
                .Select(c => new CategoriaIncidenteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Activa = c.IsActive,
                    FechaCreacion = c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound($"Categoría con ID {id} no encontrada");
            }

            return Ok(categoria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categoría por ID");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crear una nueva categoría
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaIncidenteDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateCategoria([FromBody] CreateCategoriaIncidenteDto createDto)
    {
        try
        {
            // Verificar si ya existe una categoría con el mismo nombre
            var existeCategoria = await _context.Categorias
                .AnyAsync(c => c.Nombre.ToLower() == createDto.Nombre.ToLower() && !c.IsDeleted);

            if (existeCategoria)
            {
                return BadRequest("Ya existe una categoría con ese nombre");
            }

            var categoria = new IncidentesFISEI.Domain.Entities.CategoriaIncidente
            {
                Nombre = createDto.Nombre,
                Descripcion = createDto.Descripcion,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            var categoriaDto = new CategoriaIncidenteDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Activa = categoria.IsActive,
                FechaCreacion = categoria.CreatedAt
            };

            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoriaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear categoría");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualizar una categoría existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CategoriaIncidenteDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateCategoria(int id, [FromBody] UpdateCategoriaIncidenteDto updateDto)
    {
        try
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (categoria == null)
            {
                return NotFound($"Categoría con ID {id} no encontrada");
            }

            // Verificar si otro categoría ya tiene el mismo nombre
            var existeOtraCategoria = await _context.Categorias
                .AnyAsync(c => c.Id != id && c.Nombre.ToLower() == updateDto.Nombre.ToLower() && !c.IsDeleted);

            if (existeOtraCategoria)
            {
                return BadRequest("Ya existe otra categoría con ese nombre");
            }

            categoria.Nombre = updateDto.Nombre;
            categoria.Descripcion = updateDto.Descripcion;
            categoria.IsActive = updateDto.Activa;
            categoria.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var categoriaDto = new CategoriaIncidenteDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Activa = categoria.IsActive,
                FechaCreacion = categoria.CreatedAt
            };

            return Ok(categoriaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar categoría");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Eliminar una categoría (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteCategoria(int id)
    {
        try
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (categoria == null)
            {
                return NotFound($"Categoría con ID {id} no encontrada");
            }

            // Verificar si hay incidentes asociados
            var tieneIncidentes = await _context.Incidentes
                .AnyAsync(i => i.CategoriaId == id && !i.IsDeleted);

            if (tieneIncidentes)
            {
                return BadRequest("No se puede eliminar la categoría porque tiene incidentes asociados");
            }

            categoria.IsDeleted = true;
            categoria.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Categoría eliminada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar categoría");
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
