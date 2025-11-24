using FISEI.Incidentes.Core.Entities;

namespace FISEI.Incidentes.Core.Interfaces.IServices
{
    /// <summary>
    /// Servicio de Base de Conocimiento (Knowledge Base)
    /// </summary>
    public interface IConocimientoService
    {
        Task<Conocimiento> CrearArticuloAsync(int idIncidenteResuelto);
        Task<IEnumerable<Conocimiento>> BuscarSolucionesAsync(string palabrasClave);
        Task<Conocimiento?> ObtenerSolucionSimilarAsync(int idIncidente);
        Task ValorarArticuloAsync(int idConocimiento, int calificacion);
    }
}