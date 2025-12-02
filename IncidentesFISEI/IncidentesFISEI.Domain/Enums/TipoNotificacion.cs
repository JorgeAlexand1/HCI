namespace IncidentesFISEI.Domain.Enums;

public enum TipoNotificacion
{
    // Asignaciones
    IncidenteAsignado = 1,
    IncidenteReasignado = 2,
    
    // Cambios de estado
    IncidenteNuevo = 10,
    IncidenteEnProceso = 11,
    IncidenteResuelto = 12,
    IncidenteCerrado = 13,
    IncidenteReabierto = 14,
    
    // SLA y Escalaciones
    SLAProximoVencimiento = 20,
    SLAVencido = 21,
    IncidenteEscalado = 22,
    
    // Comentarios y Actualizaciones
    NuevoComentario = 30,
    RespuestaComentario = 31,
    ActualizacionIncidente = 32,
    
    // Validaciones (Base de Conocimiento)
    SolicitudValidacion = 40,
    ArticuloAprobado = 41,
    ArticuloRechazado = 42,
    
    // Alertas del Sistema
    RecurrenciaDetectada = 50,
    AlertaSistema = 51,
    
    // Prioridad
    CambioPrioridad = 60,
    IncidenteCritico = 61
}
