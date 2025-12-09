using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentesFISEI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BaseConocimientoController : ControllerBase
{
    private readonly IBaseConocimientoService _baseConocimientoService;
    private readonly ILogger<BaseConocimientoController> _logger;

    public BaseConocimientoController(IBaseConocimientoService baseConocimientoService, ILogger<BaseConocimientoController> logger)
    {
        _baseConocimientoService = baseConocimientoService;
        _logger = logger;
    }

    private bool TryResolveUserType(out int userType)
    {
        userType = 0;
        var userTypeStr = User.FindFirst("TipoUsuario")?.Value ?? User.FindFirst("UserType")?.Value;
        if (string.IsNullOrWhiteSpace(userTypeStr)) return false;

        // Preferir numérico: "1", "2", "3", "4"
        if (int.TryParse(userTypeStr, out userType)) return true;

        // Aceptar nombres que podrían venir en el token ("tecnico", "supervisor", "administrador")
        switch (userTypeStr.Trim().ToLowerInvariant())
        {
            case "usuario":
            case "usuariofinal":
            case "estudiante":
                userType = 1; return true;
            case "tecnico":
                userType = 2; return true;
            case "supervisor":
            case "supervisortecnico":
                userType = 3; return true;
            case "administrador":
                userType = 4; return true;
            default:
                return false;
        }
    }

    private bool IsAdminOrTechOrSupervisor()
    {
        return TryResolveUserType(out var userType) && (userType == 2 || userType == 3 || userType == 4);
    }

    private bool IsAdminOnly()
    {
        return TryResolveUserType(out var userType) && userType == 4; // Solo Administrador
    }

    /// <summary>
    /// Obtiene todos los artículos (publicados y en revisión) (acceso: Admin, Supervisor, Tecnico)
    /// </summary>
    [HttpGet("articulos")]
    public async Task<ActionResult<IEnumerable<ArticuloConocimientoDto>>> GetArticulos()
    {
        try
        {
            if (!IsAdminOrTechOrSupervisor())
                return StatusCode(403, new { message = "Acceso denegado. Solo Admin, Supervisor y Técnico pueden acceder a la Base de Conocimiento" });

            var articulos = await _baseConocimientoService.GetArticulosTodosAsync();
            return Ok(articulos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener artículos de base de conocimiento");
            return StatusCode(500, new { message = "Error al obtener artículos", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un artículo específico por ID
    /// </summary>
    [HttpGet("articulos/{id}")]
    public async Task<ActionResult<ArticuloConocimientoDto>> GetArticuloById(int id)
    {
        try
        {
            if (!IsAdminOrTechOrSupervisor())
                return StatusCode(403, new { message = "Acceso denegado. Solo Admin, Supervisor y Técnico pueden acceder a la Base de Conocimiento" });

            var articulo = await _baseConocimientoService.GetArticuloByIdAsync(id);
            return Ok(articulo);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Artículo no encontrado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener artículo");
            return StatusCode(500, new { message = "Error al obtener artículo", error = ex.Message });
        }
    }

    /// <summary>
    /// Busca artículos por término (título, contenido, tags)
    /// </summary>
    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<ArticuloConocimientoDto>>> SearchArticulos([FromQuery] string termino)
    {
        try
        {
            if (!IsAdminOrTechOrSupervisor())
                return StatusCode(403, new { message = "Acceso denegado. Solo Admin, Supervisor y Técnico pueden acceder a la Base de Conocimiento" });

            if (string.IsNullOrWhiteSpace(termino))
                return BadRequest(new { message = "El término de búsqueda no puede estar vacío" });

            var resultados = await _baseConocimientoService.SearchArticulosAsync(termino);
            return Ok(resultados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar artículos");
            return StatusCode(500, new { message = "Error al buscar artículos", error = ex.Message });
        }
    }

    /// <summary>
    /// Crea un nuevo artículo (solo Admin, Supervisor, Tecnico)
    /// </summary>
    [HttpPost("articulos")]
    public async Task<ActionResult<ArticuloConocimientoDto>> CreateArticulo([FromBody] CreateArticuloConocimientoDto createDto)
    {
        try
        {
            if (!IsAdminOrTechOrSupervisor())
                return StatusCode(403, new { message = "Acceso denegado. Solo Admin, Supervisor y Técnico pueden crear artículos" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var articulo = await _baseConocimientoService.CreateArticuloAsync(createDto);
            return CreatedAtAction(nameof(GetArticuloById), new { id = articulo.Id }, articulo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear artículo");
            return StatusCode(500, new { message = "Error al crear artículo", error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un artículo (solo Admin, Supervisor, Tecnico)
    /// </summary>
    [HttpPut("articulos/{id}")]
    public async Task<ActionResult<ArticuloConocimientoDto>> UpdateArticulo(int id, [FromBody] UpdateArticuloConocimientoDto updateDto)
    {
        try
        {
            if (!IsAdminOrTechOrSupervisor())
                return StatusCode(403, new { message = "Acceso denegado. Solo Admin, Supervisor y Técnico pueden editar artículos" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var articulo = await _baseConocimientoService.UpdateArticuloAsync(id, updateDto);
            return Ok(articulo);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Artículo no encontrado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar artículo");
            return StatusCode(500, new { message = "Error al actualizar artículo", error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un artículo (solo Admin)
    /// </summary>
    [HttpDelete("articulos/{id}")]
    public async Task<ActionResult<bool>> DeleteArticulo(int id)
    {
        try
        {
            if (!IsAdminOnly())
                return StatusCode(403, new { message = "Acceso denegado. Solo el Administrador puede eliminar artículos" });

            await _baseConocimientoService.DeleteArticuloAsync(id);
            return Ok(new { message = "Artículo eliminado correctamente", success = true });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Artículo no encontrado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar artículo");
            return StatusCode(500, new { message = "Error al eliminar artículo", error = ex.Message });
        }
    }

    /// <summary>
    /// Publica un artículo (solo Admin, Supervisor)
    /// </summary>
    [HttpPost("articulos/{id}/publicar")]
    public async Task<ActionResult<bool>> PublicarArticulo(int id, [FromQuery] int revisadoPorId)
    {
        try
        {
            if (!IsAdminOrTechOrSupervisor())
                return StatusCode(403, new { message = "Acceso denegado. Solo Admin, Supervisor y Técnico pueden publicar artículos" });

            var result = await _baseConocimientoService.PublicarArticuloAsync(id, revisadoPorId);
            if (result)
                return Ok(new { message = "Artículo publicado correctamente", success = true });
            
            return BadRequest(new { message = "No se pudo publicar el artículo" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Artículo no encontrado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al publicar artículo");
            return StatusCode(500, new { message = "Error al publicar artículo", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene artículos sugeridos para resolución
    /// </summary>
    [HttpGet("sugerencias")]
    public async Task<ActionResult<IEnumerable<ArticuloConocimientoDto>>> GetSugerencias([FromQuery] string categoria = "", [FromQuery] string tags = "")
    {
        try
        {
            if (!IsAdminOrTechOrSupervisor())
                return StatusCode(403, new { message = "Acceso denegado. Solo Admin, Supervisor y Técnico pueden acceder a sugerencias" });

            var articulos = await _baseConocimientoService.GetArticulosPublicosAsync();
            return Ok(articulos.Take(5));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sugerencias");
            return StatusCode(500, new { message = "Error al obtener sugerencias", error = ex.Message });
        }
    }
}
