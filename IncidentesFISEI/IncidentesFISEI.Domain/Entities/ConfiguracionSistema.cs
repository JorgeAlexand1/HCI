using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IncidentesFISEI.Domain.Entities;

/// <summary>
/// Configuración General del Sistema
/// </summary>
public class ConfiguracionSistema : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = "FISEI - Sistema de Gestión de Incidentes";

    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    [MaxLength(500)]
    public string LogoUrl { get; set; } = string.Empty;

    [MaxLength(256)]
    public string EmailSoporte { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Telefono { get; set; } = string.Empty;

    // Configuración de SLAs por defecto (en minutos)
    public int TiempoRespuestaDefecto { get; set; } = 60;
    public int TiempoResolucionDefecto { get; set; } = 480;

    // Escalación automática
    public bool PermitirEscalacionAutomatica { get; set; } = true;
    public int PortcentajeEscalacion { get; set; } = 75; // Escalar al 75% del SLA

    // Configuración de notificaciones
    public bool NotificacionesHabilitadas { get; set; } = true;
    public bool NotificacionesPorEmail { get; set; } = true;
    public bool NotificacionesPorSistema { get; set; } = true;

    // Configuración de auditoría
    public bool AuditoriaHabilitada { get; set; } = true;
    public int DiasRetencionLogs { get; set; } = 90;

    // Zona horaria
    [MaxLength(50)]
    public string ZonaHoraria { get; set; } = "America/Guayaquil";

    public bool IsActive { get; set; } = true;
    public DateTime? UltimaActualizacion { get; set; }
    [MaxLength(256)]
    public string ActualizadoPor { get; set; } = string.Empty;
}

/// <summary>
/// Configuración de SLAs Personalizados
/// </summary>
public class ConfiguracionSLA : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    // Tiempos de respuesta y resolución por prioridad
    public int TiempoRespuestaCritica { get; set; } = 15; // minutos
    public int TiempoRespuestaAlta { get; set; } = 30;
    public int TiempoRespuestaMedia { get; set; } = 60;
    public int TiempoRespuestaBaja { get; set; } = 240;

    public int TiempoResolucionCritica { get; set; } = 60; // minutos
    public int TiempoResolucionAlta { get; set; } = 120;
    public int TiempoResolucionMedia { get; set; } = 480;
    public int TiempoResolucionBaja { get; set; } = 2880;

    // Escalación
    public bool PermitirEscalacion { get; set; } = true;
    public int MinutosAntesDeLlegar { get; set; } = 15;

    // Notificaciones
    public bool NotificarAlEscalar { get; set; } = true;
    public bool NotificarAlVencer { get; set; } = true;

    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Plantillas de Notificación
/// </summary>
public class PlantillaNotificacionConfiguracion : BaseEntity
{
    public enum TipoNotificacion
    {
        IncidenteNuevo,
        IncidenteAsignado,
        IncidenteResuelto,
        IncidenteCerrado,
        IncidenteEscalado,
        SLAProximoAVencer,
        SLAVencido,
        ComentarioAñadido,
        ReasignacionIncidente
    }

    [Required]
    public TipoNotificacion Tipo { get; set; }

    [Required]
    [MaxLength(200)]
    public string Asunto { get; set; } = string.Empty;

    [Required]
    public string CuerpoHTML { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    // Variables disponibles para la plantilla (almacenadas como referencia)
    [MaxLength(1000)]
    public string VariablesDisponibles { get; set; } = "{NumeroIncidente}, {Titulo}, {Descripcion}, {Prioridad}, {Estado}, {AsignadoA}, {NombreUsuario}";

    // Destinatarios
    public bool EnviarAReportante { get; set; } = true;
    public bool EnviarAAsignado { get; set; } = true;
    public bool EnviarASupervisor { get; set; } = false;
    public bool EnviarAAdmin { get; set; } = false;

    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Configuración de Servicios Externos
/// </summary>
public class ConfiguracionServicioExterno : BaseEntity
{
    public enum TipoServicio
    {
        Email,
        SMS,
        Slack,
        MicrosoftTeams,
        Telegram,
        WebhookPersonalizado
    }

    [Required]
    public TipoServicio Tipo { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    // Configuración
    public string ConfiguracionJSON { get; set; } = string.Empty;

    // Estado
    public bool IsActive { get; set; } = true;
    public bool EstaConectado { get; set; } = false;
    public DateTime? UltimaConexion { get; set; }

    [MaxLength(500)]
    public string MensajeError { get; set; } = string.Empty;
}

/// <summary>
/// Políticas de Seguridad del Sistema
/// </summary>
public class PoliticaSeguridad : BaseEntity
{
    // Contraseñas
    public int LongitudMinimaContraseña { get; set; } = 8;
    public bool RequerirMayusculas { get; set; } = true;
    public bool RequerirMinusculas { get; set; } = true;
    public bool RequerirNumeros { get; set; } = true;
    public bool RequerirCaracteresEspeciales { get; set; } = true;
    public int DiasExpiracionContraseña { get; set; } = 90;
    public int IntentosLoginMaximos { get; set; } = 5;
    public int MinutosBloqueoDespuesIntentos { get; set; } = 15;

    // Sesión
    public int MinutosTimeoutSesion { get; set; } = 480; // 8 horas
    public bool CerrarSesionAlCambiarContraseña { get; set; } = true;
    public bool PermitirMultiplesSesiones { get; set; } = false;

    // Acceso
    public bool RequiereVerificacionDosFactores { get; set; } = false;
    public bool PermitirAccesoDesdeIPsDiferentes { get; set; } = true;

    // Auditoría
    public bool AuditoriaActividadAdmin { get; set; } = true;
    public bool AuditoriaLecturaIncidentes { get; set; } = true;
    public bool AuditoriaModificacionIncidentes { get; set; } = true;
    public bool AuditoriaAccesoSistema { get; set; } = true;

    // Encriptación
    public bool EncriptarDatosEnTransito { get; set; } = true;
    public bool EncriptarDatosEnReposo { get; set; } = true;

    // Cumplimiento normativo
    [MaxLength(1000)]
    public string NormasAplicables { get; set; } = "ISO/IEC 27001, OWASP Top 10";

    public DateTime? UltimaRevision { get; set; }
    [MaxLength(256)]
    public string RevisadoPor { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Registro de Auditoría del Sistema
/// </summary>
public class RegistroAuditoria : BaseEntity
{
    public enum TipoAccion
    {
        Login,
        Logout,
        CrearIncidente,
        EditarIncidente,
        EliminarIncidente,
        AsignarIncidente,
        ResolverIncidente,
        CerrarIncidente,
        ReabrirIncidente,
        CrearUsuario,
        EditarUsuario,
        EliminarUsuario,
        CambiarRol,
        AccederConfiguracion,
        ModificarConfiguracion,
        ExportarDatos,
        OtroAcceso
    }

    [Required]
    public TipoAccion Accion { get; set; }

    [Required]
    [MaxLength(256)]
    public string UsuarioId { get; set; } = string.Empty;

    [MaxLength(256)]
    public string NombreUsuario { get; set; } = string.Empty;

    [MaxLength(50)]
    public string DireccionIP { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string DetallesJSON { get; set; } = string.Empty;

    public bool Exitoso { get; set; } = true;

    [MaxLength(500)]
    public string MensajeError { get; set; } = string.Empty;

    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
}
