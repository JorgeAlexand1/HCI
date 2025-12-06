using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de notificaciones ITIL v3
/// </summary>
public interface INotificationService
{
    // Gestión de notificaciones
    Task<Notificacion> CrearNotificacionAsync(int usuarioId, TipoNotificacion tipo, string titulo, string mensaje, int? incidenteId = null);
    Task<IEnumerable<Notificacion>> GetNotificacionesUsuarioAsync(int usuarioId, bool soloNoLeidas = false);
    Task<bool> MarcarComoLeidaAsync(int notificacionId, int usuarioId);
    Task<bool> MarcarTodasComoLeidasAsync(int usuarioId);
    Task<int> GetConteoNoLeidasAsync(int usuarioId);
    
    // Configuración de preferencias
    Task<ConfiguracionNotificacion> GetConfiguracionUsuarioAsync(int usuarioId, TipoEventoNotificacion tipoEvento);
    Task<bool> ActualizarConfiguracionAsync(int usuarioId, TipoEventoNotificacion tipoEvento, ConfiguracionNotificacion config);
    
    // Notificaciones automáticas por eventos
    Task NotificarIncidenteCreadoAsync(Incidente incidente);
    Task NotificarIncidenteAsignadoAsync(Incidente incidente, Usuario? tecnicoAnterior = null);
    Task NotificarIncidenteActualizadoAsync(Incidente incidente, string cambios);
    Task NotificarIncidenteEscaladoAsync(Incidente incidente, string motivo);
    Task NotificarSLAProximoVencimientoAsync(Incidente incidente, TimeSpan tiempoRestante);
    Task NotificarSLAVencidoAsync(Incidente incidente);
    
    // Envío de notificaciones
    Task<bool> EnviarNotificacionEmailAsync(Notificacion notificacion);
    Task<bool> EnviarNotificacionSMSAsync(Notificacion notificacion, string numeroTelefono);
    Task ProcesarColaNotificacionesAsync();
    
    // Plantillas
    Task<PlantillaNotificacion> GetPlantillaAsync(TipoNotificacion tipo);
    Task<string> GenerarMensajeDesdePlantillaAsync(PlantillaNotificacion plantilla, Dictionary<string, object> variables);
}

/// <summary>
/// Interfaz para el repositorio de notificaciones
/// </summary>
public interface INotificationRepository
{
    Task<Notificacion> CreateAsync(Notificacion notificacion);
    Task<Notificacion> GetByIdAsync(int id);
    Task<IEnumerable<Notificacion>> GetByUsuarioIdAsync(int usuarioId, bool soloNoLeidas = false, int page = 1, int pageSize = 50);
    Task<int> GetCountNoLeidasAsync(int usuarioId);
    Task UpdateAsync(Notificacion notificacion);
    Task DeleteAsync(int id);
    
    // Configuraciones
    Task<ConfiguracionNotificacion> GetConfiguracionAsync(int usuarioId, TipoEventoNotificacion tipoEvento);
    Task<ConfiguracionNotificacion> CreateConfiguracionAsync(ConfiguracionNotificacion config);
    Task UpdateConfiguracionAsync(ConfiguracionNotificacion config);
    
    // Plantillas
    Task<PlantillaNotificacion> GetPlantillaAsync(TipoNotificacion tipo);
    Task<IEnumerable<PlantillaNotificacion>> GetPlantillasActivasAsync();
    
    // Logs
    Task<LogNotificacion> CreateLogAsync(LogNotificacion log);
    Task<IEnumerable<LogNotificacion>> GetLogsAsync(int notificacionId);
    
    // Notificaciones pendientes de envío
    Task<IEnumerable<Notificacion>> GetNotificacionesPendientesEnvioAsync();
    Task<IEnumerable<Notificacion>> GetNotificacionesParaReintentarAsync();
}