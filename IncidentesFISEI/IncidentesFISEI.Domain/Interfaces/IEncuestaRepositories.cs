using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Interfaces;

public interface IPlantillaEncuestaRepository : IRepository<PlantillaEncuesta>
{
    Task<IEnumerable<PlantillaEncuesta>> GetPlantillasActivasAsync();
    Task<PlantillaEncuesta?> GetPlantillaConPreguntasAsync(int id);
    Task<PlantillaEncuesta?> GetPlantillaPorTipoAsync(TipoEncuesta tipo);
    Task<IEnumerable<PlantillaEncuesta>> GetPlantillasParaMostrarAsync();
}

public interface IPreguntaEncuestaRepository : IRepository<PreguntaEncuesta>
{
    Task<IEnumerable<PreguntaEncuesta>> GetPreguntasByPlantillaAsync(int plantillaId);
    Task<int> GetSiguienteOrdenAsync(int plantillaId);
}

public interface IEncuestaRepository : IRepository<Encuesta>
{
    Task<IEnumerable<Encuesta>> GetEncuestasPendientesByUsuarioAsync(int usuarioId);
    Task<IEnumerable<Encuesta>> GetEncuestasVencidasAsync();
    Task<Encuesta?> GetEncuestaConRespuestasAsync(int encuestaId);
    Task<Encuesta?> GetEncuestaByIncidenteAsync(int incidenteId);
    Task<IEnumerable<Encuesta>> GetEncuestasRespondidasAsync(DateTime? desde = null, DateTime? hasta = null);
    Task<double> GetPromedioCalificacionAsync(DateTime? desde = null, DateTime? hasta = null);
    Task<Dictionary<int, int>> GetDistribucionCalificacionesAsync(DateTime? desde = null, DateTime? hasta = null);
}

public interface IRespuestaEncuestaRepository : IRepository<RespuestaEncuesta>
{
    Task<IEnumerable<RespuestaEncuesta>> GetRespuestasByEncuestaAsync(int encuestaId);
    Task<IEnumerable<RespuestaEncuesta>> GetRespuestasByPreguntaAsync(int preguntaId);
}
