using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Entities;

namespace IncidentesFISEI.Application.Interfaces;

/// <summary>
/// Servicio para gestión de asignación de incidentes según SPOC
/// </summary>
public interface IAsignacionService
{
    /// <summary>
    /// Obtiene el SPOC (Single Point of Contact) disponible
    /// </summary>
    Task<Usuario?> GetSPOCDisponibleAsync();
    
    /// <summary>
    /// Obtiene el técnico con menor carga de trabajo
    /// </summary>
    /// <param name="especialidad">Especialidad requerida (opcional)</param>
    Task<Usuario?> GetTecnicoConMenorCargaAsync(string? especialidad = null);
    
    /// <summary>
    /// Asigna un incidente automáticamente al mejor técnico disponible
    /// </summary>
    Task<ApiResponse<bool>> AsignarIncidenteAutomaticamenteAsync(int incidenteId);
    
    /// <summary>
    /// Asigna un incidente manualmente a un técnico específico (solo SPOC o Admin)
    /// </summary>
    Task<ApiResponse<bool>> AsignarIncidenteManualmenteAsync(int incidenteId, int tecnicoId, int asignadoPorId);
    
    /// <summary>
    /// Obtiene la carga de trabajo actual de todos los técnicos
    /// </summary>
    Task<Dictionary<int, int>> GetCargaTrabajoTecnicosAsync();
    
    /// <summary>
    /// Establece o quita el rol SPOC a un usuario
    /// </summary>
    Task<ApiResponse<bool>> SetSPOCAsync(int usuarioId, bool isSPOC);
    
    /// <summary>
    /// Marca la disponibilidad del SPOC
    /// </summary>
    Task<ApiResponse<bool>> SetSPOCAvailabilityAsync(int usuarioId, bool isAvailable);
    
    /// <summary>
    /// Actualiza la carga de trabajo de un técnico
    /// </summary>
    Task ActualizarCargaTrabajoAsync(int tecnicoId, int incremento);
}
