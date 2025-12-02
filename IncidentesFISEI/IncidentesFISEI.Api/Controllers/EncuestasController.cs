using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IncidentesFISEI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EncuestasController : ControllerBase
{
    private readonly IEncuestaService _encuestaService;
    private readonly ILogger<EncuestasController> _logger;

    public EncuestasController(IEncuestaService encuestaService, ILogger<EncuestasController> logger)
    {
        _encuestaService = encuestaService;
        _logger = logger;
    }

    private int GetUsuarioId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    #region Plantillas de Encuesta

    [HttpGet("plantillas")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PlantillaEncuestaDto>>>> GetPlantillas()
    {
        try
        {
            var plantillas = await _encuestaService.GetPlantillasAsync();
            return Ok(new ApiResponse<IEnumerable<PlantillaEncuestaDto>>(true, plantillas, "Plantillas obtenidas exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener plantillas");
            return StatusCode(500, new ApiResponse<IEnumerable<PlantillaEncuestaDto>>(false, null, "Error al obtener plantillas"));
        }
    }

    [HttpGet("plantillas/{id}")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<PlantillaEncuestaDetalladaDto>>> GetPlantilla(int id)
    {
        try
        {
            var plantilla = await _encuestaService.GetPlantillaByIdAsync(id);
            if (plantilla == null)
                return NotFound(new ApiResponse<PlantillaEncuestaDetalladaDto>(false, null, "Plantilla no encontrada"));

            return Ok(new ApiResponse<PlantillaEncuestaDetalladaDto>(true, plantilla, "Plantilla obtenida"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener plantilla {Id}", id);
            return StatusCode(500, new ApiResponse<PlantillaEncuestaDetalladaDto>(false, null, "Error al obtener plantilla"));
        }
    }

    [HttpPost("plantillas")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<PlantillaEncuestaDto>>> CreatePlantilla([FromBody] CreatePlantillaEncuestaDto dto)
    {
        try
        {
            var plantilla = await _encuestaService.CreatePlantillaAsync(dto);
            return CreatedAtAction(nameof(GetPlantilla), new { id = plantilla.Id }, new ApiResponse<PlantillaEncuestaDto>(true, plantilla, "Plantilla creada exitosamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear plantilla");
            return StatusCode(500, new ApiResponse<PlantillaEncuestaDto>(false, null, "Error al crear plantilla"));
        }
    }

    [HttpPut("plantillas/{id}")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<PlantillaEncuestaDto>>> UpdatePlantilla(int id, [FromBody] UpdatePlantillaEncuestaDto dto)
    {
        try
        {
            var plantilla = await _encuestaService.UpdatePlantillaAsync(id, dto);
            return Ok(new ApiResponse<PlantillaEncuestaDto>(true, plantilla, "Plantilla actualizada"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<PlantillaEncuestaDto>(false, null, "Plantilla no encontrada"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar plantilla {Id}", id);
            return StatusCode(500, new ApiResponse<PlantillaEncuestaDto>(false, null, "Error al actualizar plantilla"));
        }
    }

    [HttpDelete("plantillas/{id}")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePlantilla(int id)
    {
        try
        {
            await _encuestaService.DeletePlantillaAsync(id);
            return Ok(new ApiResponse<object>(true, null, "Plantilla eliminada exitosamente"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<object>(false, null, "Plantilla no encontrada"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar plantilla {Id}", id);
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al eliminar plantilla"));
        }
    }

    #endregion

    #region Preguntas

    [HttpPost("plantillas/{plantillaId}/preguntas")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<PreguntaEncuestaDto>>> AddPregunta(int plantillaId, [FromBody] CreatePreguntaEncuestaDto dto)
    {
        try
        {
            var pregunta = await _encuestaService.AddPreguntaAsync(plantillaId, dto);
            return Ok(new ApiResponse<PreguntaEncuestaDto>(true, pregunta, "Pregunta agregada exitosamente"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<PreguntaEncuestaDto>(false, null, "Plantilla no encontrada"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar pregunta");
            return StatusCode(500, new ApiResponse<PreguntaEncuestaDto>(false, null, "Error al agregar pregunta"));
        }
    }

    [HttpPut("preguntas/{preguntaId}")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<PreguntaEncuestaDto>>> UpdatePregunta(int preguntaId, [FromBody] UpdatePreguntaEncuestaDto dto)
    {
        try
        {
            var pregunta = await _encuestaService.UpdatePreguntaAsync(preguntaId, dto);
            return Ok(new ApiResponse<PreguntaEncuestaDto>(true, pregunta, "Pregunta actualizada"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<PreguntaEncuestaDto>(false, null, "Pregunta no encontrada"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar pregunta {Id}", preguntaId);
            return StatusCode(500, new ApiResponse<PreguntaEncuestaDto>(false, null, "Error al actualizar pregunta"));
        }
    }

    [HttpDelete("preguntas/{preguntaId}")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePregunta(int preguntaId)
    {
        try
        {
            await _encuestaService.DeletePreguntaAsync(preguntaId);
            return Ok(new ApiResponse<object>(true, null, "Pregunta eliminada"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<object>(false, null, "Pregunta no encontrada"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar pregunta {Id}", preguntaId);
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al eliminar pregunta"));
        }
    }

    [HttpPut("plantillas/{plantillaId}/preguntas/reordenar")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<object>>> ReordenarPreguntas(int plantillaId, [FromBody] List<int> preguntaIds)
    {
        try
        {
            await _encuestaService.ReordenarPreguntasAsync(plantillaId, preguntaIds);
            return Ok(new ApiResponse<object>(true, null, "Preguntas reordenadas"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reordenar preguntas");
            return StatusCode(500, new ApiResponse<object>(false, null, "Error al reordenar preguntas"));
        }
    }

    #endregion

    #region Encuestas

    [HttpPost("enviar")]
    [Authorize(Policy = "Tecnico,Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<EncuestaDto>>> EnviarEncuesta([FromBody] EnviarEncuestaRequest request)
    {
        try
        {
            var encuesta = await _encuestaService.EnviarEncuestaAsync(request.IncidenteId, request.UsuarioId);
            return Ok(new ApiResponse<EncuestaDto>(true, encuesta, "Encuesta enviada exitosamente"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<EncuestaDto>(false, null, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<EncuestaDto>(false, null, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar encuesta");
            return StatusCode(500, new ApiResponse<EncuestaDto>(false, null, "Error al enviar encuesta"));
        }
    }

    [HttpGet("mis-pendientes")]
    public async Task<ActionResult<ApiResponse<IEnumerable<EncuestaDto>>>> GetEncuestasPendientes()
    {
        try
        {
            var usuarioId = GetUsuarioId();
            var encuestas = await _encuestaService.GetEncuestasPendientesByUsuarioAsync(usuarioId);
            return Ok(new ApiResponse<IEnumerable<EncuestaDto>>(true, encuestas, "Encuestas pendientes obtenidas"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener encuestas pendientes");
            return StatusCode(500, new ApiResponse<IEnumerable<EncuestaDto>>(false, null, "Error al obtener encuestas pendientes"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<EncuestaDetalladaDto>>> GetEncuesta(int id)
    {
        try
        {
            var encuesta = await _encuestaService.GetEncuestaDetalladaAsync(id);
            if (encuesta == null)
                return NotFound(new ApiResponse<EncuestaDetalladaDto>(false, null, "Encuesta no encontrada"));

            return Ok(new ApiResponse<EncuestaDetalladaDto>(true, encuesta, "Encuesta obtenida"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener encuesta {Id}", id);
            return StatusCode(500, new ApiResponse<EncuestaDetalladaDto>(false, null, "Error al obtener encuesta"));
        }
    }

    [HttpPost("responder")]
    public async Task<ActionResult<ApiResponse<EncuestaDetalladaDto>>> ResponderEncuesta([FromBody] ResponderEncuestaDto dto)
    {
        try
        {
            var encuesta = await _encuestaService.ResponderEncuestaAsync(dto);
            return Ok(new ApiResponse<EncuestaDetalladaDto>(true, encuesta, "Encuesta respondida exitosamente. ¡Gracias por tu opinión!"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<EncuestaDetalladaDto>(false, null, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<EncuestaDetalladaDto>(false, null, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al responder encuesta");
            return StatusCode(500, new ApiResponse<EncuestaDetalladaDto>(false, null, "Error al responder encuesta"));
        }
    }

    [HttpGet]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<EncuestaDto>>>> GetTodasEncuestas(
        [FromQuery] bool? respondidas = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        try
        {
            var encuestas = await _encuestaService.GetTodasEncuestasAsync(respondidas, skip, take);
            return Ok(new ApiResponse<IEnumerable<EncuestaDto>>(true, encuestas, "Encuestas obtenidas"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener encuestas");
            return StatusCode(500, new ApiResponse<IEnumerable<EncuestaDto>>(false, null, "Error al obtener encuestas"));
        }
    }

    #endregion

    #region Estadísticas

    [HttpGet("estadisticas")]
    [Authorize(Policy = "Supervisor,Administrador")]
    public async Task<ActionResult<ApiResponse<EstadisticasEncuestasDto>>> GetEstadisticas(
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        try
        {
            var estadisticas = await _encuestaService.GetEstadisticasAsync(desde, hasta);
            return Ok(new ApiResponse<EstadisticasEncuestasDto>(true, estadisticas, "Estadísticas obtenidas"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas");
            return StatusCode(500, new ApiResponse<EstadisticasEncuestasDto>(false, null, "Error al obtener estadísticas"));
        }
    }

    #endregion
}

public class EnviarEncuestaRequest
{
    public int IncidenteId { get; set; }
    public int UsuarioId { get; set; }
}
