using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.Interfaces;

public interface IEncuestaService
{
    // Plantillas de Encuesta
    Task<IEnumerable<PlantillaEncuestaDto>> GetPlantillasAsync();
    Task<PlantillaEncuestaDetalladaDto?> GetPlantillaByIdAsync(int id);
    Task<PlantillaEncuestaDto> CreatePlantillaAsync(CreatePlantillaEncuestaDto dto);
    Task<PlantillaEncuestaDto> UpdatePlantillaAsync(int id, UpdatePlantillaEncuestaDto dto);
    Task DeletePlantillaAsync(int id);
    
    // Preguntas
    Task<PreguntaEncuestaDto> AddPreguntaAsync(int plantillaId, CreatePreguntaEncuestaDto dto);
    Task<PreguntaEncuestaDto> UpdatePreguntaAsync(int preguntaId, UpdatePreguntaEncuestaDto dto);
    Task DeletePreguntaAsync(int preguntaId);
    Task ReordenarPreguntasAsync(int plantillaId, List<int> preguntaIds);
    
    // Encuestas (instancias)
    Task<EncuestaDto> EnviarEncuestaAsync(int incidenteId, int usuarioId);
    Task<IEnumerable<EncuestaDto>> GetEncuestasPendientesByUsuarioAsync(int usuarioId);
    Task<EncuestaDetalladaDto?> GetEncuestaDetalladaAsync(int encuestaId);
    Task<EncuestaDetalladaDto> ResponderEncuestaAsync(ResponderEncuestaDto dto);
    Task MarcarEncuestasVencidasAsync();
    
    // Estadísticas
    Task<EstadisticasEncuestasDto> GetEstadisticasAsync(DateTime? desde = null, DateTime? hasta = null);
    Task<IEnumerable<EncuestaDto>> GetTodasEncuestasAsync(bool? respondidas = null, int skip = 0, int take = 50);
}
