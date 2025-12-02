using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IncidentesFISEI.Application.Services;

public class EncuestaService : IEncuestaService
{
    private readonly IPlantillaEncuestaRepository _plantillaRepository;
    private readonly IPreguntaEncuestaRepository _preguntaRepository;
    private readonly IEncuestaRepository _encuestaRepository;
    private readonly IRespuestaEncuestaRepository _respuestaRepository;
    private readonly IIncidenteRepository _incidenteRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly INotificacionService _notificacionService;
    private readonly ILogger<EncuestaService> _logger;

    public EncuestaService(
        IPlantillaEncuestaRepository plantillaRepository,
        IPreguntaEncuestaRepository preguntaRepository,
        IEncuestaRepository encuestaRepository,
        IRespuestaEncuestaRepository respuestaRepository,
        IIncidenteRepository incidenteRepository,
        IUsuarioRepository usuarioRepository,
        INotificacionService notificacionService,
        ILogger<EncuestaService> logger)
    {
        _plantillaRepository = plantillaRepository;
        _preguntaRepository = preguntaRepository;
        _encuestaRepository = encuestaRepository;
        _respuestaRepository = respuestaRepository;
        _incidenteRepository = incidenteRepository;
        _usuarioRepository = usuarioRepository;
        _notificacionService = notificacionService;
        _logger = logger;
    }

    #region Plantillas de Encuesta

    public async Task<IEnumerable<PlantillaEncuestaDto>> GetPlantillasAsync()
    {
        var plantillas = await _plantillaRepository.GetAllAsync();
        return plantillas.Select(MapToPlantillaDto);
    }

    public async Task<PlantillaEncuestaDetalladaDto?> GetPlantillaByIdAsync(int id)
    {
        var plantilla = await _plantillaRepository.GetPlantillaConPreguntasAsync(id);
        if (plantilla == null)
            return null;

        return new PlantillaEncuestaDetalladaDto
        {
            Id = plantilla.Id,
            Nombre = plantilla.Nombre,
            Descripcion = plantilla.Descripcion,
            EsActiva = plantilla.EsActiva,
            Tipo = plantilla.Tipo,
            MostrarAutomaticamente = plantilla.MostrarAutomaticamente,
            DiasVigencia = plantilla.DiasVigencia,
            CreatedAt = plantilla.CreatedAt,
            Preguntas = plantilla.Preguntas.Select(MapToPreguntaDto).ToList()
        };
    }

    public async Task<PlantillaEncuestaDto> CreatePlantillaAsync(CreatePlantillaEncuestaDto dto)
    {
        var plantilla = new PlantillaEncuesta
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion ?? string.Empty,
            Tipo = dto.Tipo,
            EsActiva = dto.EsActiva,
            MostrarAutomaticamente = dto.MostrarAutomaticamente,
            DiasVigencia = dto.DiasVigencia
        };

        await _plantillaRepository.AddAsync(plantilla);

        // Agregar preguntas si se proporcionaron
        if (dto.Preguntas.Any())
        {
            var orden = 1;
            foreach (var preguntaDto in dto.Preguntas)
            {
                var pregunta = new PreguntaEncuesta
                {
                    PlantillaEncuestaId = plantilla.Id,
                    TextoPregunta = preguntaDto.TextoPregunta,
                    Tipo = preguntaDto.Tipo,
                    Orden = preguntaDto.Orden > 0 ? preguntaDto.Orden : orden++,
                    EsObligatoria = preguntaDto.EsObligatoria,
                    OpcionesJson = preguntaDto.Opciones != null && preguntaDto.Opciones.Any()
                        ? JsonSerializer.Serialize(preguntaDto.Opciones)
                        : null,
                    ValorMinimo = preguntaDto.ValorMinimo,
                    ValorMaximo = preguntaDto.ValorMaximo,
                    EtiquetaMinimo = preguntaDto.EtiquetaMinimo,
                    EtiquetaMaximo = preguntaDto.EtiquetaMaximo
                };

                await _preguntaRepository.AddAsync(pregunta);
            }
        }

        _logger.LogInformation("Plantilla de encuesta creada: {Nombre} (ID: {Id})", plantilla.Nombre, plantilla.Id);
        return MapToPlantillaDto(plantilla);
    }

    public async Task<PlantillaEncuestaDto> UpdatePlantillaAsync(int id, UpdatePlantillaEncuestaDto dto)
    {
        var plantilla = await _plantillaRepository.GetByIdAsync(id);
        if (plantilla == null)
            throw new KeyNotFoundException($"Plantilla de encuesta con ID {id} no encontrada");

        plantilla.Nombre = dto.Nombre;
        plantilla.Descripcion = dto.Descripcion ?? string.Empty;
        plantilla.EsActiva = dto.EsActiva;
        plantilla.MostrarAutomaticamente = dto.MostrarAutomaticamente;
        plantilla.DiasVigencia = dto.DiasVigencia;

        await _plantillaRepository.UpdateAsync(plantilla);

        _logger.LogInformation("Plantilla de encuesta actualizada: {Nombre} (ID: {Id})", plantilla.Nombre, plantilla.Id);
        return MapToPlantillaDto(plantilla);
    }

    public async Task DeletePlantillaAsync(int id)
    {
        var plantilla = await _plantillaRepository.GetByIdAsync(id);
        if (plantilla == null)
            throw new KeyNotFoundException($"Plantilla de encuesta con ID {id} no encontrada");

        await _plantillaRepository.DeleteAsync(id);
        _logger.LogInformation("Plantilla de encuesta eliminada: {Nombre} (ID: {Id})", plantilla.Nombre, plantilla.Id);
    }

    #endregion

    #region Preguntas

    public async Task<PreguntaEncuestaDto> AddPreguntaAsync(int plantillaId, CreatePreguntaEncuestaDto dto)
    {
        var plantilla = await _plantillaRepository.GetByIdAsync(plantillaId);
        if (plantilla == null)
            throw new KeyNotFoundException($"Plantilla de encuesta con ID {plantillaId} no encontrada");

        var orden = dto.Orden > 0 ? dto.Orden : await _preguntaRepository.GetSiguienteOrdenAsync(plantillaId);

        var pregunta = new PreguntaEncuesta
        {
            PlantillaEncuestaId = plantillaId,
            TextoPregunta = dto.TextoPregunta,
            Tipo = dto.Tipo,
            Orden = orden,
            EsObligatoria = dto.EsObligatoria,
            OpcionesJson = dto.Opciones != null && dto.Opciones.Any()
                ? JsonSerializer.Serialize(dto.Opciones)
                : null,
            ValorMinimo = dto.ValorMinimo,
            ValorMaximo = dto.ValorMaximo,
            EtiquetaMinimo = dto.EtiquetaMinimo,
            EtiquetaMaximo = dto.EtiquetaMaximo
        };

        await _preguntaRepository.AddAsync(pregunta);

        _logger.LogInformation("Pregunta agregada a plantilla {PlantillaId}: {Texto}", plantillaId, pregunta.TextoPregunta);
        return MapToPreguntaDto(pregunta);
    }

    public async Task<PreguntaEncuestaDto> UpdatePreguntaAsync(int preguntaId, UpdatePreguntaEncuestaDto dto)
    {
        var pregunta = await _preguntaRepository.GetByIdAsync(preguntaId);
        if (pregunta == null)
            throw new KeyNotFoundException($"Pregunta con ID {preguntaId} no encontrada");

        pregunta.TextoPregunta = dto.TextoPregunta;
        pregunta.EsObligatoria = dto.EsObligatoria;
        pregunta.Orden = dto.Orden;

        await _preguntaRepository.UpdateAsync(pregunta);

        _logger.LogInformation("Pregunta actualizada: {Id}", preguntaId);
        return MapToPreguntaDto(pregunta);
    }

    public async Task DeletePreguntaAsync(int preguntaId)
    {
        var pregunta = await _preguntaRepository.GetByIdAsync(preguntaId);
        if (pregunta == null)
            throw new KeyNotFoundException($"Pregunta con ID {preguntaId} no encontrada");

        await _preguntaRepository.DeleteAsync(preguntaId);
        _logger.LogInformation("Pregunta eliminada: {Id}", preguntaId);
    }

    public async Task ReordenarPreguntasAsync(int plantillaId, List<int> preguntaIds)
    {
        var preguntas = await _preguntaRepository.GetPreguntasByPlantillaAsync(plantillaId);
        var preguntasList = preguntas.ToList();

        for (int i = 0; i < preguntaIds.Count; i++)
        {
            var pregunta = preguntasList.FirstOrDefault(p => p.Id == preguntaIds[i]);
            if (pregunta != null)
            {
                pregunta.Orden = i + 1;
                await _preguntaRepository.UpdateAsync(pregunta);
            }
        }

        _logger.LogInformation("Preguntas reordenadas en plantilla {PlantillaId}", plantillaId);
    }

    #endregion

    #region Encuestas

    public async Task<EncuestaDto> EnviarEncuestaAsync(int incidenteId, int usuarioId)
    {
        var incidente = await _incidenteRepository.GetByIdAsync(incidenteId);
        if (incidente == null)
            throw new KeyNotFoundException($"Incidente con ID {incidenteId} no encontrado");

        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario == null)
            throw new KeyNotFoundException($"Usuario con ID {usuarioId} no encontrado");

        // Verificar si ya existe una encuesta para este incidente
        var encuestaExistente = await _encuestaRepository.GetEncuestaByIncidenteAsync(incidenteId);
        if (encuestaExistente != null)
        {
            _logger.LogWarning("Ya existe una encuesta para el incidente {IncidenteId}", incidenteId);
            return MapToEncuestaDto(encuestaExistente);
        }

        // Obtener plantilla activa de satisfacción general
        var plantilla = await _plantillaRepository.GetPlantillaPorTipoAsync(TipoEncuesta.SatisfaccionGeneral);
        if (plantilla == null)
        {
            // Si no hay plantilla específica, tomar cualquier activa
            var plantillas = await _plantillaRepository.GetPlantillasActivasAsync();
            plantilla = plantillas.FirstOrDefault();
        }

        if (plantilla == null)
            throw new InvalidOperationException("No hay plantillas de encuesta activas configuradas");

        var encuesta = new Encuesta
        {
            IncidenteId = incidenteId,
            PlantillaEncuestaId = plantilla.Id,
            UsuarioId = usuarioId,
            FechaEnvio = DateTime.UtcNow,
            FechaVencimiento = DateTime.UtcNow.AddDays(plantilla.DiasVigencia),
            EsRespondida = false,
            EsVencida = false
        };

        await _encuestaRepository.AddAsync(encuesta);

        // Enviar notificación al usuario
        try
        {
            await _notificacionService.CrearNotificacionAsync(new CreateNotificacionDto
            {
                UsuarioId = usuarioId,
                IncidenteId = incidenteId,
                Tipo = TipoNotificacion.AlertaSistema,
                Titulo = "Encuesta de Satisfacción Disponible",
                Mensaje = $"Por favor, completa la encuesta de satisfacción sobre el incidente #{incidente.NumeroIncidente}. Tu opinión es importante para mejorar nuestro servicio."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación de encuesta");
        }

        _logger.LogInformation("Encuesta enviada para incidente {IncidenteId} al usuario {UsuarioId}", incidenteId, usuarioId);
        return MapToEncuestaDto(encuesta);
    }

    public async Task<IEnumerable<EncuestaDto>> GetEncuestasPendientesByUsuarioAsync(int usuarioId)
    {
        var encuestas = await _encuestaRepository.GetEncuestasPendientesByUsuarioAsync(usuarioId);
        return encuestas.Select(MapToEncuestaDto);
    }

    public async Task<EncuestaDetalladaDto?> GetEncuestaDetalladaAsync(int encuestaId)
    {
        var encuesta = await _encuestaRepository.GetEncuestaConRespuestasAsync(encuestaId);
        if (encuesta == null)
            return null;

        return new EncuestaDetalladaDto
        {
            Id = encuesta.Id,
            IncidenteId = encuesta.IncidenteId,
            IncidenteTitulo = encuesta.Incidente?.Titulo ?? "",
            IncidenteNumero = encuesta.Incidente?.NumeroIncidente ?? "",
            Plantilla = new PlantillaEncuestaDetalladaDto
            {
                Id = encuesta.PlantillaEncuesta.Id,
                Nombre = encuesta.PlantillaEncuesta.Nombre,
                Descripcion = encuesta.PlantillaEncuesta.Descripcion,
                EsActiva = encuesta.PlantillaEncuesta.EsActiva,
                Tipo = encuesta.PlantillaEncuesta.Tipo,
                MostrarAutomaticamente = encuesta.PlantillaEncuesta.MostrarAutomaticamente,
                DiasVigencia = encuesta.PlantillaEncuesta.DiasVigencia,
                CreatedAt = encuesta.PlantillaEncuesta.CreatedAt,
                Preguntas = encuesta.PlantillaEncuesta.Preguntas.Select(MapToPreguntaDto).ToList()
            },
            FechaEnvio = encuesta.FechaEnvio,
            FechaRespuesta = encuesta.FechaRespuesta,
            FechaVencimiento = encuesta.FechaVencimiento,
            EsRespondida = encuesta.EsRespondida,
            EsVencida = encuesta.EsVencida,
            CalificacionPromedio = encuesta.CalificacionPromedio,
            ComentariosGenerales = encuesta.ComentariosGenerales,
            Respuestas = encuesta.Respuestas.Select(r => new RespuestaEncuestaDto
            {
                Id = r.Id,
                PreguntaId = r.PreguntaEncuestaId,
                TextoPregunta = r.PreguntaEncuesta.TextoPregunta,
                TipoPregunta = r.PreguntaEncuesta.Tipo,
                RespuestaTexto = r.RespuestaTexto,
                RespuestaNumero = r.RespuestaNumero,
                RespuestaBooleana = r.RespuestaBooleana,
                RespuestaFecha = r.RespuestaFecha,
                RespuestasSeleccion = !string.IsNullOrEmpty(r.RespuestasSeleccionJson)
                    ? JsonSerializer.Deserialize<List<string>>(r.RespuestasSeleccionJson)
                    : null
            }).ToList()
        };
    }

    public async Task<EncuestaDetalladaDto> ResponderEncuestaAsync(ResponderEncuestaDto dto)
    {
        var encuesta = await _encuestaRepository.GetEncuestaConRespuestasAsync(dto.EncuestaId);
        if (encuesta == null)
            throw new KeyNotFoundException($"Encuesta con ID {dto.EncuestaId} no encontrada");

        if (encuesta.EsRespondida)
            throw new InvalidOperationException("Esta encuesta ya ha sido respondida");

        if (encuesta.EsVencida)
            throw new InvalidOperationException("Esta encuesta ha vencido");

        // Eliminar respuestas anteriores si existen (por si es una corrección)
        var respuestasExistentes = await _respuestaRepository.GetRespuestasByEncuestaAsync(dto.EncuestaId);
        foreach (var respuesta in respuestasExistentes)
        {
            await _respuestaRepository.DeleteAsync(respuesta.Id);
        }

        // Guardar nuevas respuestas
        var calificaciones = new List<double>();

        foreach (var respuestaDto in dto.Respuestas)
        {
            var pregunta = await _preguntaRepository.GetByIdAsync(respuestaDto.PreguntaId);
            if (pregunta == null)
                continue;

            var respuesta = new RespuestaEncuesta
            {
                EncuestaId = dto.EncuestaId,
                PreguntaEncuestaId = respuestaDto.PreguntaId,
                RespuestaTexto = respuestaDto.RespuestaTexto,
                RespuestaNumero = respuestaDto.RespuestaNumero,
                RespuestaBooleana = respuestaDto.RespuestaBooleana,
                RespuestaFecha = respuestaDto.RespuestaFecha,
                RespuestasSeleccionJson = respuestaDto.RespuestasSeleccion != null && respuestaDto.RespuestasSeleccion.Any()
                    ? JsonSerializer.Serialize(respuestaDto.RespuestasSeleccion)
                    : null
            };

            await _respuestaRepository.AddAsync(respuesta);

            // Calcular calificación si es numérica
            if (respuestaDto.RespuestaNumero.HasValue && (pregunta.Tipo == TipoPregunta.EscalaLineal || pregunta.Tipo == TipoPregunta.Calificacion))
            {
                // Normalizar a escala 0-5
                double calificacionNormalizada;
                if (pregunta.ValorMaximo.HasValue && pregunta.ValorMaximo.Value > 0)
                {
                    calificacionNormalizada = (respuestaDto.RespuestaNumero.Value / (double)pregunta.ValorMaximo.Value) * 5.0;
                }
                else
                {
                    calificacionNormalizada = respuestaDto.RespuestaNumero.Value;
                }
                calificaciones.Add(calificacionNormalizada);
            }
            else if (respuestaDto.RespuestaBooleana.HasValue && pregunta.Tipo == TipoPregunta.SiNo)
            {
                calificaciones.Add(respuestaDto.RespuestaBooleana.Value ? 5.0 : 1.0);
            }
        }

        // Actualizar encuesta
        encuesta.EsRespondida = true;
        encuesta.FechaRespuesta = DateTime.UtcNow;
        encuesta.ComentariosGenerales = dto.ComentariosGenerales;
        encuesta.CalificacionPromedio = calificaciones.Any() ? calificaciones.Average() : null;

        await _encuestaRepository.UpdateAsync(encuesta);

        _logger.LogInformation("Encuesta respondida: {EncuestaId}, Calificación: {Calificacion:F2}",
            encuesta.Id, encuesta.CalificacionPromedio ?? 0);

        // Notificar al técnico asignado si la calificación es baja
        if (encuesta.CalificacionPromedio.HasValue && encuesta.CalificacionPromedio.Value < 3.0)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(encuesta.IncidenteId);
            if (incidente != null && incidente.AsignadoAId != null)
            {
                try
                {
                    await _notificacionService.CrearNotificacionAsync(new CreateNotificacionDto
                    {
                        UsuarioId = incidente.AsignadoAId.Value,
                        IncidenteId = incidente.Id,
                        Tipo = TipoNotificacion.AlertaSistema,
                        Titulo = "Encuesta de Satisfacción - Calificación Baja",
                        Mensaje = $"El usuario ha completado la encuesta del incidente #{incidente.NumeroIncidente} con una calificación de {encuesta.CalificacionPromedio:F1}/5. Por favor, revisa los comentarios."
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al notificar calificación baja");
                }
            }
        }

        var resultado = await GetEncuestaDetalladaAsync(dto.EncuestaId);
        return resultado!;
    }

    public async Task MarcarEncuestasVencidasAsync()
    {
        var encuestasVencidas = await _encuestaRepository.GetEncuestasVencidasAsync();
        var contador = 0;

        foreach (var encuesta in encuestasVencidas)
        {
            encuesta.EsVencida = true;
            await _encuestaRepository.UpdateAsync(encuesta);
            contador++;
        }

        if (contador > 0)
        {
            _logger.LogInformation("Marcadas {Count} encuestas como vencidas", contador);
        }
    }

    #endregion

    #region Estadísticas

    public async Task<EstadisticasEncuestasDto> GetEstadisticasAsync(DateTime? desde = null, DateTime? hasta = null)
    {
        var todasEncuestas = await _encuestaRepository.GetAllAsync();
        var encuestasRespondidas = await _encuestaRepository.GetEncuestasRespondidasAsync(desde, hasta);
        var promedioCalificacion = await _encuestaRepository.GetPromedioCalificacionAsync(desde, hasta);
        var distribucion = await _encuestaRepository.GetDistribucionCalificacionesAsync(desde, hasta);

        var estadisticas = new EstadisticasEncuestasDto
        {
            TotalEncuestasEnviadas = todasEncuestas.Count(),
            TotalEncuestasRespondidas = encuestasRespondidas.Count(),
            TotalEncuestasVencidas = todasEncuestas.Count(e => e.EsVencida),
            TotalEncuestasPendientes = todasEncuestas.Count(e => !e.EsRespondida && !e.EsVencida),
            CalificacionPromedio = promedioCalificacion,
            DistribucionCalificaciones = distribucion
        };

        if (estadisticas.TotalEncuestasEnviadas > 0)
        {
            estadisticas.TasaRespuesta = (double)estadisticas.TotalEncuestasRespondidas / estadisticas.TotalEncuestasEnviadas * 100;
        }

        // Calificaciones por técnico
        var incidentesIds = encuestasRespondidas.Select(e => e.IncidenteId).ToList();
        var incidentes = new List<Incidente>();
        foreach (var id in incidentesIds)
        {
            var inc = await _incidenteRepository.GetByIdAsync(id);
            if (inc != null) incidentes.Add(inc);
        }

        var calificacionesPorTecnico = encuestasRespondidas
            .Where(e => e.CalificacionPromedio.HasValue)
            .Join(incidentes, e => e.IncidenteId, i => i.Id, (e, i) => new { e, i })
            .Where(x => x.i.AsignadoAId != null)
            .GroupBy(x => x.i.AsignadoAId!.Value)
            .Select(g => new CalificacionPorTecnicoDto
            {
                TecnicoId = g.Key,
                TecnicoNombre = "Técnico " + g.Key, // Se puede mejorar cargando el nombre real
                TotalEncuestas = g.Count(),
                CalificacionPromedio = g.Average(x => x.e.CalificacionPromedio!.Value)
            })
            .OrderByDescending(c => c.CalificacionPromedio)
            .ToList();

        estadisticas.CalificacionesPorTecnico = calificacionesPorTecnico;

        return estadisticas;
    }

    public async Task<IEnumerable<EncuestaDto>> GetTodasEncuestasAsync(bool? respondidas = null, int skip = 0, int take = 50)
    {
        var encuestas = await _encuestaRepository.GetAllAsync();

        if (respondidas.HasValue)
        {
            encuestas = encuestas.Where(e => e.EsRespondida == respondidas.Value).ToList();
        }

        return encuestas
            .OrderByDescending(e => e.FechaEnvio)
            .Skip(skip)
            .Take(take)
            .Select(MapToEncuestaDto);
    }

    #endregion

    #region Mappers

    private PlantillaEncuestaDto MapToPlantillaDto(PlantillaEncuesta plantilla)
    {
        return new PlantillaEncuestaDto
        {
            Id = plantilla.Id,
            Nombre = plantilla.Nombre,
            Descripcion = plantilla.Descripcion,
            EsActiva = plantilla.EsActiva,
            Tipo = plantilla.Tipo,
            TipoDescripcion = plantilla.Tipo.ToString(),
            MostrarAutomaticamente = plantilla.MostrarAutomaticamente,
            DiasVigencia = plantilla.DiasVigencia,
            TotalPreguntas = plantilla.Preguntas?.Count ?? 0,
            CreatedAt = plantilla.CreatedAt
        };
    }

    private PreguntaEncuestaDto MapToPreguntaDto(PreguntaEncuesta pregunta)
    {
        return new PreguntaEncuestaDto
        {
            Id = pregunta.Id,
            PlantillaEncuestaId = pregunta.PlantillaEncuestaId,
            TextoPregunta = pregunta.TextoPregunta,
            Tipo = pregunta.Tipo,
            TipoDescripcion = pregunta.Tipo.ToString(),
            Orden = pregunta.Orden,
            EsObligatoria = pregunta.EsObligatoria,
            Opciones = !string.IsNullOrEmpty(pregunta.OpcionesJson)
                ? JsonSerializer.Deserialize<List<string>>(pregunta.OpcionesJson)
                : null,
            ValorMinimo = pregunta.ValorMinimo,
            ValorMaximo = pregunta.ValorMaximo,
            EtiquetaMinimo = pregunta.EtiquetaMinimo,
            EtiquetaMaximo = pregunta.EtiquetaMaximo
        };
    }

    private EncuestaDto MapToEncuestaDto(Encuesta encuesta)
    {
        var diasRestantes = (encuesta.FechaVencimiento - DateTime.UtcNow).Days;

        return new EncuestaDto
        {
            Id = encuesta.Id,
            IncidenteId = encuesta.IncidenteId,
            IncidenteTitulo = encuesta.Incidente?.Titulo ?? "",
            IncidenteNumero = encuesta.Incidente?.NumeroIncidente ?? "",
            PlantillaEncuestaId = encuesta.PlantillaEncuestaId,
            PlantillaNombre = encuesta.PlantillaEncuesta?.Nombre ?? "",
            UsuarioId = encuesta.UsuarioId,
            UsuarioNombre = encuesta.Usuario?.NombreCompleto ?? "",
            FechaEnvio = encuesta.FechaEnvio,
            FechaRespuesta = encuesta.FechaRespuesta,
            FechaVencimiento = encuesta.FechaVencimiento,
            EsRespondida = encuesta.EsRespondida,
            EsVencida = encuesta.EsVencida,
            CalificacionPromedio = encuesta.CalificacionPromedio,
            DiasRestantes = diasRestantes > 0 ? diasRestantes : 0
        };
    }

    #endregion
}
