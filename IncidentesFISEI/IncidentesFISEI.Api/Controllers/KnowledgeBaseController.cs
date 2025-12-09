using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IncidentesFISEI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KnowledgeBaseController : ControllerBase
{
    private readonly IKnowledgeBaseService _knowledgeBaseService;
    private readonly ILogger<KnowledgeBaseController> _logger;

    public KnowledgeBaseController(
        IKnowledgeBaseService knowledgeBaseService,
        ILogger<KnowledgeBaseController> logger)
    {
        _knowledgeBaseService = knowledgeBaseService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener artículos del supervisor actual
    /// </summary>
    [HttpGet("mis-articulos")]
    [Authorize(Roles = "SupervisorTecnico")]
    public async Task<IActionResult> ObtenerMisArticulos()
    {
        try
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var articulos = await _knowledgeBaseService.ObtenerMisArticulosAsync(usuarioId);
            
            var resultado = articulos.Select(a => new
            {
                a.Id,
                a.Titulo,
                a.Resumen,
                a.Contenido,
                Estado = a.Estado.ToString(),
                a.Visualizaciones,
                a.VotosPositivos,
                a.CreatedAt,
                Categoria = new { a.Categoria.Id, a.Categoria.Nombre }
            }).ToList();

            return Ok(new { success = true, data = resultado });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error obteniendo artículos del supervisor: {ex.Message}");
            return BadRequest(new { success = false, message = "Error obteniendo artículos" });
        }
    }

    /// <summary>
    /// Obtener artículos publicados (públicos)
    /// </summary>
    [HttpGet("publicados")]
    [AllowAnonymous]
    public async Task<IActionResult> ObtenerArticulosPublicados()
    {
        try
        {
            var articulos = await _knowledgeBaseService.ObtenerArticulosPublicadosAsync();
            
            var resultado = articulos.Select(a => new
            {
                a.Id,
                a.Titulo,
                a.Resumen,
                a.Visualizaciones,
                a.VotosPositivos,
                a.FechaPublicacion,
                Autor = new { a.Autor.FirstName, a.Autor.LastName },
                Categoria = new { a.Categoria.Id, a.Categoria.Nombre }
            }).ToList();

            return Ok(new { success = true, data = resultado });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error obteniendo artículos publicados: {ex.Message}");
            return BadRequest(new { success = false, message = "Error obteniendo artículos" });
        }
    }

    /// <summary>
    /// Obtener un artículo específico
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObtenerArticulo(int id)
    {
        try
        {
            var articulo = await _knowledgeBaseService.ObtenerArticuloAsync(id);
            
            if (articulo == null)
                return NotFound(new { success = false, message = "Artículo no encontrado" });

            // Incrementar visualizaciones
            await _knowledgeBaseService.IncrementarVisualizacionesAsync(id);

            var resultado = new
            {
                articulo.Id,
                articulo.Titulo,
                articulo.Resumen,
                articulo.Contenido,
                articulo.PasosDetallados,
                articulo.Prerequisites,
                articulo.Limitaciones,
                articulo.Tags,
                Estado = articulo.Estado.ToString(),
                articulo.Visualizaciones,
                articulo.VotosPositivos,
                articulo.VotosNegativos,
                articulo.FechaPublicacion,
                articulo.CreatedAt,
                Autor = new { articulo.Autor.FirstName, articulo.Autor.LastName },
                Categoria = new { articulo.Categoria.Id, articulo.Categoria.Nombre },
                Comentarios = articulo.Comentarios?.Select(c => new
                {
                    c.Id,
                    c.Contenido,
                    c.CreatedAt,
                    Autor = new { c.Autor.FirstName, c.Autor.LastName }
                }).ToList()
            };

            return Ok(new { success = true, data = resultado });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error obteniendo artículo {id}: {ex.Message}");
            return BadRequest(new { success = false, message = "Error obteniendo artículo" });
        }
    }

    /// <summary>
    /// Crear nuevo artículo como borrador
    /// </summary>
    [HttpPost("crear")]
    [Authorize(Roles = "SupervisorTecnico")]
    public async Task<IActionResult> CrearArticulo([FromBody] ArticuloCrearDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos inválidos", errors = ModelState });

            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var articulo = new ArticuloConocimiento
            {
                Titulo = dto.Titulo,
                Resumen = dto.Resumen,
                Contenido = dto.Contenido,
                PasosDetallados = dto.PasosDetallados,
                Prerequisites = dto.Prerequisites,
                Limitaciones = dto.Limitaciones,
                Tags = dto.Tags,
                CategoriaId = dto.CategoriaId,
                AutorId = usuarioId,
                Visualizaciones = 0,
                VotosPositivos = 0,
                VotosNegativos = 0
            };

            var articuloId = await _knowledgeBaseService.CrearArticuloAsync(articulo);

            return Ok(new
            {
                success = true,
                message = "Artículo guardado como borrador",
                data = new { id = articuloId }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creando artículo: {ex.Message}");
            return BadRequest(new { success = false, message = "Error creando artículo" });
        }
    }

    /// <summary>
    /// Crear y enviar artículo a revisión
    /// </summary>
    [HttpPost("crear-y-enviar")]
    [Authorize(Roles = "SupervisorTecnico")]
    public async Task<IActionResult> CrearYEnviarArticulo([FromBody] ArticuloCrearDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos inválidos", errors = ModelState });

            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var articulo = new ArticuloConocimiento
            {
                Titulo = dto.Titulo,
                Resumen = dto.Resumen,
                Contenido = dto.Contenido,
                PasosDetallados = dto.PasosDetallados,
                Prerequisites = dto.Prerequisites,
                Limitaciones = dto.Limitaciones,
                Tags = dto.Tags,
                CategoriaId = dto.CategoriaId,
                AutorId = usuarioId,
                Visualizaciones = 0,
                VotosPositivos = 0,
                VotosNegativos = 0
            };

            var articuloId = await _knowledgeBaseService.CrearArticuloAsync(articulo);
            await _knowledgeBaseService.EnviarARevisionAsync(articuloId);

            return Ok(new
            {
                success = true,
                message = "Artículo enviado a revisión",
                data = new { id = articuloId }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creando y enviando artículo: {ex.Message}");
            return BadRequest(new { success = false, message = "Error en operación" });
        }
    }

    /// <summary>
    /// Enviar artículo a revisión (artículo existente)
    /// </summary>
    [HttpPost("{id}/enviar-revision")]
    [Authorize(Roles = "SupervisorTecnico")]
    public async Task<IActionResult> EnviarARevision(int id)
    {
        try
        {
            var resultado = await _knowledgeBaseService.EnviarARevisionAsync(id);
            
            if (!resultado)
                return NotFound(new { success = false, message = "Artículo no encontrado" });

            return Ok(new { success = true, message = "Artículo enviado a revisión" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error enviando artículo a revisión: {ex.Message}");
            return BadRequest(new { success = false, message = "Error en operación" });
        }
    }

    /// <summary>
    /// Obtener artículos en revisión (solo admin)
    /// </summary>
    [HttpGet("revision/pendientes")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerArticulosEnRevision()
    {
        try
        {
            var articulos = await _knowledgeBaseService.ObtenerArticulosEnRevisionAsync();
            
            var resultado = articulos.Select(a => new
            {
                a.Id,
                a.Titulo,
                a.Resumen,
                a.Contenido,
                a.CreatedAt,
                Autor = new { a.Autor.Id, a.Autor.FirstName, a.Autor.LastName, a.Autor.Email },
                Categoria = new { a.Categoria.Id, a.Categoria.Nombre }
            }).ToList();

            return Ok(new { success = true, data = resultado });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error obteniendo artículos en revisión: {ex.Message}");
            return BadRequest(new { success = false, message = "Error obteniendo artículos" });
        }
    }

    /// <summary>
    /// Aprobar artículo (solo admin)
    /// </summary>
    [HttpPost("{id}/aprobar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> AprobarArticulo(int id)
    {
        try
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var resultado = await _knowledgeBaseService.AprobarArticuloAsync(id, adminId);
            
            if (!resultado)
                return NotFound(new { success = false, message = "Artículo no encontrado" });

            return Ok(new
            {
                success = true,
                message = "Artículo aprobado y publicado"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error aprobando artículo: {ex.Message}");
            return BadRequest(new { success = false, message = "Error en operación" });
        }
    }

    /// <summary>
    /// Rechazar artículo (solo admin)
    /// </summary>
    [HttpPost("{id}/rechazar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RechazarArticulo(int id, [FromBody] RechazarArticuloDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Motivo))
                return BadRequest(new { success = false, message = "Debe especificar el motivo del rechazo" });

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var resultado = await _knowledgeBaseService.RechazarArticuloAsync(id, adminId, dto.Motivo);
            
            if (!resultado)
                return NotFound(new { success = false, message = "Artículo no encontrado" });

            return Ok(new
            {
                success = true,
                message = "Artículo rechazado"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error rechazando artículo: {ex.Message}");
            return BadRequest(new { success = false, message = "Error en operación" });
        }
    }
}

// DTOs
public class ArticuloCrearDto
{
    public string Titulo { get; set; }
    public string Resumen { get; set; }
    public string Contenido { get; set; }
    public string? PasosDetallados { get; set; }
    public string? Prerequisites { get; set; }
    public string? Limitaciones { get; set; }
    public string[]? Tags { get; set; }
    public int CategoriaId { get; set; }
}

public class RechazarArticuloDto
{
    public string Motivo { get; set; }
}
