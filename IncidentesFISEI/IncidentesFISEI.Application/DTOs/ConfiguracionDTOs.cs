using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ConfiguracionServicioExternoEntity = IncidentesFISEI.Domain.Entities.ConfiguracionServicioExterno;
using RegistroAuditoriaEntity = IncidentesFISEI.Domain.Entities.RegistroAuditoria;

namespace IncidentesFISEI.Application.DTOs;

// ============= Configuración General =============
public class ConfiguracionSistemaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string EmailSoporte { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public int TiempoRespuestaDefecto { get; set; }
    public int TiempoResolucionDefecto { get; set; }
    public bool PermitirEscalacionAutomatica { get; set; }
    public int PortcentajeEscalacion { get; set; }
    public bool NotificacionesHabilitadas { get; set; }
    public bool NotificacionesPorEmail { get; set; }
    public bool NotificacionesPorSistema { get; set; }
    public bool AuditoriaHabilitada { get; set; }
    public int DiasRetencionLogs { get; set; }
    public string ZonaHoraria { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? UltimaActualizacion { get; set; }
    public string ActualizadoPor { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateConfiguracionSistemaDto
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [EmailAddress]
    [MaxLength(256)]
    public string? EmailSoporte { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Telefono { get; set; }

    [Range(5, 1440)]
    public int? TiempoRespuestaDefecto { get; set; }

    [Range(5, 10080)]
    public int? TiempoResolucionDefecto { get; set; }

    public bool? PermitirEscalacionAutomatica { get; set; }

    [Range(1, 99)]
    public int? PortcentajeEscalacion { get; set; }

    public bool? NotificacionesHabilitadas { get; set; }
    public bool? NotificacionesPorEmail { get; set; }
    public bool? NotificacionesPorSistema { get; set; }

    public bool? AuditoriaHabilitada { get; set; }

    [Range(1, 365)]
    public int? DiasRetencionLogs { get; set; }

    [MaxLength(50)]
    public string? ZonaHoraria { get; set; }
}

public class UpdateConfiguracionSistemaDto
{
    [MaxLength(100)]
    public string? Nombre { get; set; }

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [EmailAddress]
    [MaxLength(256)]
    public string? EmailSoporte { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Telefono { get; set; }

    [Range(5, 1440)]
    public int? TiempoRespuestaDefecto { get; set; }

    [Range(5, 10080)]
    public int? TiempoResolucionDefecto { get; set; }

    public bool? PermitirEscalacionAutomatica { get; set; }

    [Range(1, 99)]
    public int? PortcentajeEscalacion { get; set; }

    public bool? NotificacionesHabilitadas { get; set; }
    public bool? NotificacionesPorEmail { get; set; }
    public bool? NotificacionesPorSistema { get; set; }

    public bool? AuditoriaHabilitada { get; set; }

    [Range(1, 365)]
    public int? DiasRetencionLogs { get; set; }

    [MaxLength(50)]
    public string? ZonaHoraria { get; set; }
}

// ============= Configuración de SLA =============
public class ConfiguracionSLADto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int TiempoRespuestaCritica { get; set; }
    public int TiempoRespuestaAlta { get; set; }
    public int TiempoRespuestaMedia { get; set; }
    public int TiempoRespuestaBaja { get; set; }
    public int TiempoResolucionCritica { get; set; }
    public int TiempoResolucionAlta { get; set; }
    public int TiempoResolucionMedia { get; set; }
    public int TiempoResolucionBaja { get; set; }
    public bool PermitirEscalacion { get; set; }
    public int MinutosAntesDeLlegar { get; set; }
    public bool NotificarAlEscalar { get; set; }
    public bool NotificarAlVencer { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateConfiguracionSLADto
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    [Range(5, 1440)]
    public int TiempoRespuestaCritica { get; set; } = 15;

    [Range(5, 1440)]
    public int TiempoRespuestaAlta { get; set; } = 30;

    [Range(5, 1440)]
    public int TiempoRespuestaMedia { get; set; } = 60;

    [Range(5, 1440)]
    public int TiempoRespuestaBaja { get; set; } = 240;

    [Range(15, 10080)]
    public int TiempoResolucionCritica { get; set; } = 60;

    [Range(15, 10080)]
    public int TiempoResolucionAlta { get; set; } = 120;

    [Range(15, 10080)]
    public int TiempoResolucionMedia { get; set; } = 480;

    [Range(15, 10080)]
    public int TiempoResolucionBaja { get; set; } = 2880;

    public bool PermitirEscalacion { get; set; } = true;

    [Range(1, 120)]
    public int MinutosAntesDeLlegar { get; set; } = 15;

    public bool NotificarAlEscalar { get; set; } = true;
    public bool NotificarAlVencer { get; set; } = true;
}

public class UpdateConfiguracionSLADto
{
    [MaxLength(100)]
    public string? Nombre { get; set; }

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    [Range(5, 1440)]
    public int? TiempoRespuestaCritica { get; set; }

    [Range(5, 1440)]
    public int? TiempoRespuestaAlta { get; set; }

    [Range(5, 1440)]
    public int? TiempoRespuestaMedia { get; set; }

    [Range(5, 1440)]
    public int? TiempoRespuestaBaja { get; set; }

    [Range(15, 10080)]
    public int? TiempoResolucionCritica { get; set; }

    [Range(15, 10080)]
    public int? TiempoResolucionAlta { get; set; }

    [Range(15, 10080)]
    public int? TiempoResolucionMedia { get; set; }

    [Range(15, 10080)]
    public int? TiempoResolucionBaja { get; set; }

    public bool? PermitirEscalacion { get; set; }

    [Range(1, 120)]
    public int? MinutosAntesDeLlegar { get; set; }

    public bool? NotificarAlEscalar { get; set; }
    public bool? NotificarAlVencer { get; set; }
}

// ============= Política de Seguridad =============
public class PoliticaSeguridadDto
{
    public int Id { get; set; }
    public bool RequiereContraseña { get; set; }
    public int LongitudMinimaContraseña { get; set; }
    public bool RequiereMayusculas { get; set; }
    public bool RequiereNumeros { get; set; }
    public bool RequiereCaracteresEspeciales { get; set; }
    public int VidaUtilContraseña { get; set; }
    public int HistorialContraseña { get; set; }
    public int SesionTimeoutMinutos { get; set; }
    public int ReintentosFallidos { get; set; }
    public int BloqueoTiempoMinutos { get; set; }
    public bool RequiereVerificacion2FA { get; set; }
    public bool PermiteAccesoAPI { get; set; }
    public int LimiteTiempoAPIMinutos { get; set; }
    public int LimiteIntentosSinBloqueo { get; set; }
    public bool AuditoriaActividadSensible { get; set; }
    public bool EncriptacionDatos { get; set; }
    public bool RequiereIPBlanca { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreatePoliticaSeguridadDto
{
    public bool? RequiereContraseña { get; set; }

    [Range(6, 20)]
    public int? LongitudMinimaContraseña { get; set; }

    public bool? RequiereMayusculas { get; set; }
    public bool? RequiereNumeros { get; set; }
    public bool? RequiereCaracteresEspeciales { get; set; }

    [Range(1, 365)]
    public int? VidaUtilContraseña { get; set; }

    [Range(1, 24)]
    public int? HistorialContraseña { get; set; }

    [Range(5, 1440)]
    public int? SesionTimeoutMinutos { get; set; }

    [Range(1, 10)]
    public int? ReintentosFallidos { get; set; }

    [Range(1, 240)]
    public int? BloqueoTiempoMinutos { get; set; }

    public bool? RequiereVerificacion2FA { get; set; }
    public bool? PermiteAccesoAPI { get; set; }

    [Range(5, 1440)]
    public int? LimiteTiempoAPIMinutos { get; set; }

    [Range(1, 50)]
    public int? LimiteIntentosSinBloqueo { get; set; }

    public bool? AuditoriaActividadSensible { get; set; }
    public bool? EncriptacionDatos { get; set; }
    public bool? RequiereIPBlanca { get; set; }
}

// ============= Servicio Externo =============
public class ConfiguracionServicioExternoDto
{
    public int Id { get; set; }
    public ConfiguracionServicioExternoEntity.TipoServicio TipoServicio { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? ApiKey { get; set; } = null;
    public string UrlBase { get; set; } = string.Empty;
    public bool Habilitado { get; set; }
    public int PrioridadUso { get; set; }
    public int TimeoutSegundos { get; set; }
    public int ReintentosFallidos { get; set; }
    public DateTime? UltimaPruebaExitosa { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateConfiguracionServicioExternoDto
{
    [Required]
    public ConfiguracionServicioExternoEntity.TipoServicio TipoServicio { get; set; }

    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Descripcion { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string UrlBase { get; set; } = string.Empty;

    public bool? Habilitado { get; set; }

    [Range(1, 10)]
    public int? PrioridadUso { get; set; }

    [Range(5, 300)]
    public int? TimeoutSegundos { get; set; }

    [Range(0, 10)]
    public int? ReintentosFallidos { get; set; }
}

public class UpdateConfiguracionServicioExternoDto
{
    [MaxLength(200)]
    public string? Nombre { get; set; }

    [MaxLength(1000)]
    public string? Descripcion { get; set; }

    public string? ApiKey { get; set; }

    [MaxLength(500)]
    public string? UrlBase { get; set; }

    public bool? Habilitado { get; set; }

    [Range(1, 10)]
    public int? PrioridadUso { get; set; }

    [Range(5, 300)]
    public int? TimeoutSegundos { get; set; }

    [Range(0, 10)]
    public int? ReintentosFallidos { get; set; }
}

// ============= Auditoría =============
public class RegistroAuditoriaDto
{
    public int Id { get; set; }
    public RegistroAuditoriaEntity.TipoAccion TipoAccion { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string DetallesAccion { get; set; } = string.Empty;
    public string DireccionIP { get; set; } = string.Empty;
    public DateTime FechaAccion { get; set; }
    public bool Exitoso { get; set; }
    public string EntidadAfectada { get; set; } = string.Empty;
    public string IdEntidadAfectada { get; set; } = string.Empty;
    public string CambiosRealizados { get; set; } = string.Empty;
}
