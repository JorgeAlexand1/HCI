using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IncidentesFISEI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConocimientoController : ControllerBase
{
    private readonly IConocimientoService _conocimientoService;
    private readonly ILogger<ConocimientoController> _logger;

    public ConocimientoController(IConocimientoService conocimientoService, ILogger<ConocimientoController> logger)
    {
        _conocimientoService = conocimientoService;
        _logger = logger;
    }

    private int GetUsuarioId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>
    /// Obtiene todos los artículos publicados
    /// </summary>
    [HttpGet("articulos")]
    public async Task<IActionResult> GetArticulosPublicados()
    {
        var result = await _conocimientoService.GetArticulosPublicadosAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obtiene un artículo por ID con todos sus detalles
    /// </summary>
    [HttpGet("articulos/{id}")]
    public async Task<IActionResult> GetArticuloById(int id)
    {
        var usuarioId = User.Identity?.IsAuthenticated == true ? GetUsuarioId() : (int?)null;
        var result = await _conocimientoService.GetArticuloByIdAsync(id, usuarioId);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Crea un nuevo artículo de conocimiento
    /// </summary>
    [HttpPost("articulos")]
    [Authorize(Roles = "Tecnico,Supervisor,Administrador")]
    public async Task<IActionResult> CreateArticulo([FromBody] CreateArticuloDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _conocimientoService.CreateArticuloAsync(dto, GetUsuarioId());
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetArticuloById), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Actualiza un artículo existente (crea nueva versión)
    /// </summary>
    [HttpPut("articulos/{id}")]
    [Authorize(Roles = "Tecnico,Supervisor,Administrador")]
    public async Task<IActionResult> UpdateArticulo(int id, [FromBody] UpdateArticuloDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _conocimientoService.UpdateArticuloAsync(id, dto, GetUsuarioId());
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Elimina un artículo (soft delete)
    /// </summary>
    [HttpDelete("articulos/{id}")]
    [Authorize(Roles = "Supervisor,Administrador")]
    public async Task<IActionResult> DeleteArticulo(int id)
    {
        var result = await _conocimientoService.DeleteArticuloAsync(id, GetUsuarioId());
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Publica un artículo (cambia estado a Publicado)
    /// </summary>
    [HttpPost("articulos/{id}/publicar")]
    [Authorize(Roles = "Supervisor,Administrador")]
    public async Task<IActionResult> PublicarArticulo(int id)
    {
        var result = await _conocimientoService.PublicarArticuloAsync(id, GetUsuarioId());
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Archiva un artículo obsoleto
    /// </summary>
    [HttpPost("articulos/{id}/archivar")]
    [Authorize(Roles = "Supervisor,Administrador")]
    public async Task<IActionResult> ArchivarArticulo(int id)
    {
        var result = await _conocimientoService.ArchivarArticuloAsync(id, GetUsuarioId());
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Búsqueda avanzada de artículos con filtros y paginación
    /// </summary>
    [HttpPost("articulos/buscar")]
    public async Task<IActionResult> BuscarArticulos([FromBody] BusquedaArticulosDto dto)
    {
        var result = await _conocimientoService.BuscarArticulosAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene artículos por categoría
    /// </summary>
    [HttpGet("articulos/categoria/{categoriaId}")]
    public async Task<IActionResult> GetArticulosPorCategoria(int categoriaId)
    {
        var result = await _conocimientoService.GetArticulosPorCategoriaAsync(categoriaId);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene artículos por autor
    /// </summary>
    [HttpGet("articulos/autor/{autorId}")]
    public async Task<IActionResult> GetArticulosPorAutor(int autorId)
    {
        var result = await _conocimientoService.GetArticulosPorAutorAsync(autorId);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene todas las etiquetas disponibles
    /// </summary>
    [HttpGet("etiquetas")]
    public async Task<IActionResult> GetAllEtiquetas()
    {
        var result = await _conocimientoService.GetAllEtiquetasAsync();
        return Ok(result);
    }

    /// <summary>
    /// Crea una nueva etiqueta
    /// </summary>
    [HttpPost("etiquetas")]
    [Authorize(Roles = "Supervisor,Administrador")]
    public async Task<IActionResult> CreateEtiqueta([FromBody] CreateEtiquetaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _conocimientoService.CreateEtiquetaAsync(dto);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Obtiene las etiquetas más utilizadas
    /// </summary>
    [HttpGet("etiquetas/populares")]
    public async Task<IActionResult> GetEtiquetasMasUsadas([FromQuery] int cantidad = 10)
    {
        var result = await _conocimientoService.GetEtiquetasMasUsadasAsync(cantidad);
        return Ok(result);
    }

    /// <summary>
    /// Agrega un comentario a un artículo
    /// </summary>
    [HttpPost("articulos/{articuloId}/comentarios")]
    [Authorize]
    public async Task<IActionResult> AddComentario(int articuloId, [FromBody] CreateComentarioArticuloDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _conocimientoService.AddComentarioAsync(articuloId, dto, GetUsuarioId());
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Elimina un comentario (solo el autor puede eliminarlo)
    /// </summary>
    [HttpDelete("comentarios/{comentarioId}")]
    [Authorize]
    public async Task<IActionResult> DeleteComentario(int comentarioId)
    {
        var result = await _conocimientoService.DeleteComentarioAsync(comentarioId, GetUsuarioId());
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Vota un artículo (positivo o negativo)
    /// </summary>
    [HttpPost("articulos/{articuloId}/votar")]
    [Authorize]
    public async Task<IActionResult> VotarArticulo(int articuloId, [FromBody] VotarArticuloDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _conocimientoService.VotarArticuloAsync(articuloId, dto, GetUsuarioId());
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Solicita validación de un artículo por un experto
    /// </summary>
    [HttpPost("articulos/{articuloId}/solicitar-validacion")]
    [Authorize(Roles = "Tecnico,Supervisor,Administrador")]
    public async Task<IActionResult> SolicitarValidacion(int articuloId, [FromBody] SolicitarValidacionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _conocimientoService.SolicitarValidacionAsync(articuloId, dto, GetUsuarioId());
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Valida un artículo (aprueba o rechaza)
    /// </summary>
    [HttpPost("validaciones/{validacionId}/validar")]
    [Authorize(Roles = "Supervisor,Administrador")]
    public async Task<IActionResult> ValidarArticulo(int validacionId, [FromBody] ValidarArticuloDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _conocimientoService.ValidarArticuloAsync(validacionId, dto, GetUsuarioId());
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Obtiene las validaciones pendientes (opcionalmente filtradas por validador)
    /// </summary>
    [HttpGet("validaciones/pendientes")]
    [Authorize(Roles = "Supervisor,Administrador")]
    public async Task<IActionResult> GetValidacionesPendientes([FromQuery] int? validadorId = null)
    {
        var result = await _conocimientoService.GetValidacionesPendientesAsync(validadorId);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene estadísticas generales del sistema de conocimiento
    /// </summary>
    [HttpGet("estadisticas")]
    [Authorize(Roles = "Supervisor,Administrador")]
    public async Task<IActionResult> GetEstadisticas()
    {
        var result = await _conocimientoService.GetEstadisticasAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obtiene los artículos más populares
    /// </summary>
    [HttpGet("articulos/populares")]
    public async Task<IActionResult> GetArticulosMasPopulares([FromQuery] int cantidad = 10)
    {
        var result = await _conocimientoService.GetArticulosMasPopularesAsync(cantidad);
        return Ok(result);
    }
}
