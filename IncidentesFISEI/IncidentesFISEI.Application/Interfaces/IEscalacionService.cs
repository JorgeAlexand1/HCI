using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.Interfaces
{
    /// <summary>
    /// Servicio de escalación de incidentes según ITIL v4
    /// Gestiona la escalación funcional y jerárquica de tickets
    /// </summary>
    public interface IEscalacionService
    {
        /// <summary>
        /// Escala un incidente al siguiente nivel de soporte
        /// </summary>
        Task<ApiResponse<bool>> EscalarIncidenteAsync(int incidenteId, string razon, int? tecnicoDestinoId = null);
        
        /// <summary>
        /// Escala un incidente a un nivel específico
        /// </summary>
        Task<ApiResponse<bool>> EscalarANivelEspecificoAsync(int incidenteId, NivelSoporte nivelDestino, string razon, int? tecnicoDestinoId = null);
        
        /// <summary>
        /// Verifica si un incidente debe ser escalado automáticamente por tiempo
        /// </summary>
        Task<ApiResponse<bool>> VerificarEscalacionAutomaticaAsync(int incidenteId);
        
        /// <summary>
        /// Obtiene el técnico disponible del nivel especificado con menor carga
        /// </summary>
        Task<ApiResponse<UsuarioDto?>> ObtenerTecnicoDisponiblePorNivelAsync(NivelSoporte nivel);
        
        /// <summary>
        /// Obtiene estadísticas de escalación
        /// </summary>
        Task<ApiResponse<EstadisticasEscalacionDto>> ObtenerEstadisticasEscalacionAsync();
        
        /// <summary>
        /// Procesa escalaciones automáticas para todos los incidentes que lo requieran
        /// (Se ejecuta desde background service)
        /// </summary>
        Task<int> ProcesarEscalacionesAutomaticasAsync();
        
        /// <summary>
        /// Obtiene el historial de escalaciones de un incidente
        /// </summary>
        Task<ApiResponse<List<HistorialEscalacionDto>>> ObtenerHistorialEscalacionesAsync(int incidenteId);
    }
}
