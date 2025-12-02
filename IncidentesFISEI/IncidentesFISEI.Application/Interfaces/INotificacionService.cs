using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.Interfaces;

public interface INotificacionService
{
    // CRUD básico
    Task<ApiResponse<NotificacionDto>> CrearNotificacionAsync(CreateNotificacionDto dto);
    Task<ApiResponse<NotificacionesPaginadasDto>> GetNotificacionesUsuarioAsync(int usuarioId, int pagina = 1, int tamañoPagina = 20, bool soloNoLeidas = false);
    Task<ApiResponse<bool>> MarcarComoLeidaAsync(int notificacionId);
    Task<ApiResponse<bool>> MarcarTodasComoLeidasAsync(int usuarioId);
    Task<ApiResponse<int>> GetCountNoLeidasAsync(int usuarioId);
    Task<ApiResponse<bool>> EliminarNotificacionAsync(int notificacionId);
    
    // Notificaciones de incidentes
    Task NotificarNuevoIncidenteAsync(int incidenteId, int reportadoPorId);
    Task NotificarAsignacionAsync(int incidenteId, int tecnicoId, int asignadoPorId);
    Task NotificarReasignacionAsync(int incidenteId, int nuevoTecnicoId, int anteriorTecnicoId, int reasignadoPorId);
    Task NotificarCambioEstadoAsync(int incidenteId, EstadoIncidente nuevoEstado, int cambiadoPorId);
    Task NotificarCambioPrioridadAsync(int incidenteId, PrioridadIncidente nuevaPrioridad, int cambiadoPorId);
    
    // Notificaciones de SLA y escalaciones
    Task NotificarSLAProximoVencimientoAsync(int incidenteId, int tecnicoAsignadoId, TimeSpan tiempoRestante);
    Task NotificarSLAVencidoAsync(int incidenteId, int tecnicoAsignadoId);
    Task NotificarEscalacionAsync(int incidenteId, int supervisorId, string motivo);
    
    // Notificaciones de comentarios
    Task NotificarNuevoComentarioAsync(int incidenteId, int comentarioId, int autorId);
    Task NotificarRespuestaComentarioAsync(int incidenteId, int comentarioId, int autorRespuestaId, int autorComentarioOriginalId);
    
    // Notificaciones de base de conocimiento
    Task NotificarSolicitudValidacionAsync(int articuloId, int validadorId, int solicitadoPorId);
    Task NotificarArticuloValidadoAsync(int articuloId, int autorId, bool aprobado, string? comentarios);
    
    // Estadísticas
    Task<ApiResponse<EstadisticasNotificacionesDto>> GetEstadisticasNotificacionesAsync(int usuarioId);
}
