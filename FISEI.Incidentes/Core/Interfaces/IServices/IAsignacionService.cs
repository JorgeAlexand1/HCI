using FISEI.Incidentes.Core.Entities;

namespace FISEI.Incidentes.Core.Interfaces.IServices
{
    /// <summary>
    /// Servicio para gestión de asignaciones (SPOC - Single Point of Contact)
    /// </summary>
    public interface IAsignacionService
    {
        /// <summary>
        /// Asigna un incidente al técnico con menor carga de trabajo (distribución equitativa)
        /// </summary>
        Task<Asignacion> AsignarAutomaticamenteAsync(int idIncidente);

        /// <summary>
        /// Permite al SPOC asignar manualmente
        /// </summary>
        Task<Asignacion> AsignarManualmenteAsync(int idIncidente, int idTecnico, int idUsuarioSPOC);

        /// <summary>
        /// Permite a un técnico tomar un incidente no asignado (cuando SPOC no disponible)
        /// </summary>
        Task<Asignacion> TomarIncidenteAsync(int idIncidente, int idTecnico);

        /// <summary>
        /// Reasigna un incidente a otro técnico
        /// </summary>
        Task ReasignarAsync(int idIncidente, int idNuevoTecnico);

        /// <summary>
        /// Obtiene el técnico con menor carga de trabajo
        /// </summary>
        Task<Usuario> ObtenerTecnicoConMenorCargaAsync(int idNivelSoporte);
    }
}