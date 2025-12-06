namespace IncidentesFISEI.Domain.Enums;

public enum TipoUsuario
{
    Usuario = 1,
    Tecnico = 2,
    Supervisor = 3,
    Administrador = 4
}

public enum RolUsuario
{
    Estudiante = 1,
    Docente = 2,
    Administrativo = 3,
    Tecnico = 4,
    SupervisorTecnico = 5,
    Administrador = 6
}

public enum EstadoIncidente
{
    Abierto = 1,
    EnProgreso = 2,
    EnEspera = 3,
    Resuelto = 4,
    Cerrado = 5,
    Cancelado = 6,
    Escalado = 7
}

public enum PrioridadIncidente
{
    Baja = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

public enum ImpactoIncidente
{
    Bajo = 1,
    Medio = 2,
    Alto = 3,
    Critico = 4
}

public enum UrgenciaIncidente
{
    Baja = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

public enum EstadoArticulo
{
    Borrador = 1,
    Revision = 2,
    Publicado = 3,
    Archivado = 4,
    Rechazado = 5
}

public enum TipoComentario
{
    Comentario = 1,
    ActualizacionEstado = 2,
    Solucion = 3,
    Escalacion = 4,
    CierreIncidente = 5
}

public enum TipoRelacion
{
    Duplicado = 1,
    Relacionado = 2,
    CausadoPor = 3,
    Padre = 4,
    Hijo = 5,
    Dependiente = 6
}

public enum TipoActividad
{
    Investigacion = 1,
    Diagnostico = 2,
    Implementacion = 3,
    Pruebas = 4,
    Documentacion = 5,
    Comunicacion = 6,
    Escalacion = 7,
    Cierre = 8
}

public enum TipoArticulo
{
    ProcedimientoGeneral = 1,
    SolucionProblema = 2,
    FAQ = 3,
    Manual = 4,
    Tutorial = 5,
    KnownError = 6,
    Workaround = 7
}

// ===== ENUMS PARA SISTEMA DE NOTIFICACIONES ITIL v3 =====

public enum TipoNotificacion
{
    // Incidentes
    IncidenteCreado = 1,
    IncidenteAsignado = 2,
    IncidenteActualizado = 3,
    IncidenteEscalado = 4,
    IncidenteResuelto = 5,
    IncidenteCerrado = 6,
    
    // SLA
    SLAProximoVencimiento = 10,
    SLAVencido = 11,
    SLAEscalacion = 12,
    
    // Sistema
    MantenimientoProgramado = 20,
    ActualizacionSistema = 21,
    
    // Base de Conocimiento
    ArticuloPublicado = 30,
    ArticuloActualizado = 31,
    
    // Recordatorios
    RecordatorioTarea = 40,
    RecordatorioSeguimiento = 41
}

public enum PrioridadNotificacion
{
    Baja = 1,
    Normal = 2,
    Alta = 3,
    Critica = 4
}

public enum TipoEventoNotificacion
{
    TodosLosIncidentes = 1,
    IncidentesAsignados = 2,
    IncidentesCreados = 3,
    EscalacionSLA = 4,
    ActualizacionEstado = 5,
    Comentarios = 6,
    BaseConocimiento = 7,
    Mantenimiento = 8
}

public enum CanalNotificacion
{
    Sistema = 1,
    Email = 2,
    SMS = 3,
    Push = 4,
    Teams = 5,
    Slack = 6
}

public enum EstadoEnvioNotificacion
{
    Pendiente = 1,
    Enviando = 2,
    Enviado = 3,
    Entregado = 4,
    Fallido = 5,
    Cancelado = 6
}