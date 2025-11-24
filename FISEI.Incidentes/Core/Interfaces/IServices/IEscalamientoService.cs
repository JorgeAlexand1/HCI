using FISEI.Incidentes.Core.Entities;

namespace FISEI.Incidentes.Core.Interfaces.IServices
{
    /// <summary>
    /// Servicio de escalamiento según ITIL (Técnicos → Expertos → Proveedores)
    /// </summary>
    public interface IEscalamientoService
    {
        /// <summary>
        /// Escala un incidente al siguiente nivel
        /// </summary>
        Task<Incidente> EscalarIncidenteAsync(int idIncidente, string motivoEscalamiento);

        /// <summary>
        /// Verifica si un incidente debe escalar automáticamente por tiempo
        /// </summary>
        Task<bool> DebeEscalarPorTiempoAsync(int idIncidente);

        /// <summary>
        /// Verifica si un incidente es recurrente y debe escalar
        /// </summary>
        Task<bool> DebeEscalarPorRecurrenciaAsync(int idIncidente);

        /// <summary>
        /// Escala incidentes automáticamente según reglas de negocio
        /// </summary>
        Task EscalarIncidentesAutomaticamenteAsync();
    }
}