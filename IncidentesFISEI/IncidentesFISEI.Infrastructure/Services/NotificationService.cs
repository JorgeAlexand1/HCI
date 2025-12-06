using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IncidentesFISEI.Infrastructure.Services;

/// <summary>
/// Servicio de notificaciones que implementa los estándares ITIL v3
/// para comunicación automática y gestión de alertas
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;
    private readonly ISMSService _smsService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUsuarioRepository usuarioRepository,
        IEmailService emailService,
        ISMSService smsService,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _usuarioRepository = usuarioRepository;
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task<Notificacion> CrearNotificacionAsync(int usuarioId, TipoNotificacion tipo, string titulo, string mensaje, int? incidenteId = null)
    {
        try
        {
            var notificacion = new Notificacion
            {
                UsuarioId = usuarioId,
                TipoNotificacion = tipo,
                Titulo = titulo,
                Mensaje = mensaje,
                IncidenteId = incidenteId,
                Prioridad = DeterminarPrioridadPorTipo(tipo),
                CreatedAt = DateTime.UtcNow
            };

            var resultado = await _notificationRepository.CreateAsync(notificacion);
            
            // Enviar notificación según configuración del usuario
            await ProcesarEnvioNotificacionAsync(resultado);
            
            _logger.LogInformation($"Notificación creada: {tipo} para usuario {usuarioId}");
            
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creando notificación para usuario {usuarioId}");
            throw;
        }
    }

    public async Task<IEnumerable<Notificacion>> GetNotificacionesUsuarioAsync(int usuarioId, bool soloNoLeidas = false)
    {
        return await _notificationRepository.GetByUsuarioIdAsync(usuarioId, soloNoLeidas);
    }

    public async Task<bool> MarcarComoLeidaAsync(int notificacionId, int usuarioId)
    {
        try
        {
            var notificacion = await _notificationRepository.GetByIdAsync(notificacionId);
            
            if (notificacion == null || notificacion.UsuarioId != usuarioId)
                return false;

            if (!notificacion.Leida)
            {
                notificacion.MarcarComoLeida();
                await _notificationRepository.UpdateAsync(notificacion);
                
                _logger.LogInformation($"Notificación {notificacionId} marcada como leída por usuario {usuarioId}");
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marcando notificación {notificacionId} como leída");
            return false;
        }
    }

    public async Task<bool> MarcarTodasComoLeidasAsync(int usuarioId)
    {
        try
        {
            var notificaciones = await _notificationRepository.GetByUsuarioIdAsync(usuarioId, soloNoLeidas: true);
            
            foreach (var notificacion in notificaciones)
            {
                notificacion.MarcarComoLeida();
                await _notificationRepository.UpdateAsync(notificacion);
            }

            _logger.LogInformation($"Todas las notificaciones marcadas como leídas para usuario {usuarioId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marcando todas las notificaciones como leídas para usuario {usuarioId}");
            return false;
        }
    }

    public async Task<int> GetConteoNoLeidasAsync(int usuarioId)
    {
        return await _notificationRepository.GetCountNoLeidasAsync(usuarioId);
    }

    public async Task<ConfiguracionNotificacion> GetConfiguracionUsuarioAsync(int usuarioId, TipoEventoNotificacion tipoEvento)
    {
        var config = await _notificationRepository.GetConfiguracionAsync(usuarioId, tipoEvento);
        
        // Si no existe configuración, crear una por defecto
        if (config == null)
        {
            config = new ConfiguracionNotificacion
            {
                UsuarioId = usuarioId,
                TipoEvento = tipoEvento,
                NotificarEnSistema = true,
                NotificarPorEmail = true,
                NotificarPorSMS = false,
                NotificacionInmediata = true,
                CreatedAt = DateTime.UtcNow
            };
            
            config = await _notificationRepository.CreateConfiguracionAsync(config);
        }
        
        return config;
    }

    public async Task<bool> ActualizarConfiguracionAsync(int usuarioId, TipoEventoNotificacion tipoEvento, ConfiguracionNotificacion config)
    {
        try
        {
            config.UsuarioId = usuarioId;
            config.TipoEvento = tipoEvento;
            config.UpdatedAt = DateTime.UtcNow;
            
            await _notificationRepository.UpdateConfiguracionAsync(config);
            
            _logger.LogInformation($"Configuración de notificaciones actualizada para usuario {usuarioId}, evento {tipoEvento}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error actualizando configuración de notificaciones para usuario {usuarioId}");
            return false;
        }
    }

    // ===== NOTIFICACIONES AUTOMÁTICAS POR EVENTOS ITIL =====

    public async Task NotificarIncidenteCreadoAsync(Incidente incidente)
    {
        try
        {
            // Notificar al reportador
            await CrearNotificacionAsync(
                incidente.ReportadoPorId,
                TipoNotificacion.IncidenteCreado,
                "Incidente creado exitosamente",
                $"Su incidente #{incidente.NumeroIncidente} '{incidente.Titulo}' ha sido registrado y será atendido según su prioridad: {incidente.Prioridad}",
                incidente.Id
            );

            // Notificar a supervisores si es alta prioridad
            if (incidente.Prioridad >= PrioridadIncidente.Alta)
            {
                var supervisores = await GetUsuariosPorRolAsync(TipoUsuario.Supervisor);
                foreach (var supervisor in supervisores)
                {
                    await CrearNotificacionAsync(
                        supervisor.Id,
                        TipoNotificacion.IncidenteCreado,
                        $"Incidente de prioridad {incidente.Prioridad} creado",
                        $"Nuevo incidente #{incidente.NumeroIncidente}: {incidente.Titulo}. Requiere atención inmediata.",
                        incidente.Id
                    );
                }
            }

            _logger.LogInformation($"Notificaciones enviadas por creación de incidente {incidente.NumeroIncidente}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error enviando notificaciones por creación de incidente {incidente.Id}");
        }
    }

    public async Task NotificarIncidenteAsignadoAsync(Incidente incidente, Usuario? tecnicoAnterior = null)
    {
        try
        {
            if (incidente.AsignadoAId.HasValue)
            {
                // Notificar al técnico asignado
                await CrearNotificacionAsync(
                    incidente.AsignadoAId.Value,
                    TipoNotificacion.IncidenteAsignado,
                    "Incidente asignado",
                    $"Se le ha asignado el incidente #{incidente.NumeroIncidente}: {incidente.Titulo}. Prioridad: {incidente.Prioridad}",
                    incidente.Id
                );

                // Notificar al reportador sobre la asignación
                await CrearNotificacionAsync(
                    incidente.ReportadoPorId,
                    TipoNotificacion.IncidenteActualizado,
                    "Incidente asignado a técnico",
                    $"Su incidente #{incidente.NumeroIncidente} ha sido asignado a un técnico especializado y está siendo atendido.",
                    incidente.Id
                );
            }

            // Si había un técnico anterior, notificarle sobre el cambio
            if (tecnicoAnterior != null)
            {
                await CrearNotificacionAsync(
                    tecnicoAnterior.Id,
                    TipoNotificacion.IncidenteActualizado,
                    "Incidente reasignado",
                    $"El incidente #{incidente.NumeroIncidente} ha sido reasignado a otro técnico.",
                    incidente.Id
                );
            }

            _logger.LogInformation($"Notificaciones enviadas por asignación de incidente {incidente.NumeroIncidente}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error enviando notificaciones por asignación de incidente {incidente.Id}");
        }
    }

    public async Task NotificarIncidenteActualizadoAsync(Incidente incidente, string cambios)
    {
        try
        {
            var usuariosANotificar = new List<int> { incidente.ReportadoPorId };
            
            if (incidente.AsignadoAId.HasValue && incidente.AsignadoAId != incidente.ReportadoPorId)
            {
                usuariosANotificar.Add(incidente.AsignadoAId.Value);
            }

            foreach (var usuarioId in usuariosANotificar)
            {
                await CrearNotificacionAsync(
                    usuarioId,
                    TipoNotificacion.IncidenteActualizado,
                    "Incidente actualizado",
                    $"El incidente #{incidente.NumeroIncidente} ha sido actualizado. Cambios: {cambios}",
                    incidente.Id
                );
            }

            _logger.LogInformation($"Notificaciones enviadas por actualización de incidente {incidente.NumeroIncidente}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error enviando notificaciones por actualización de incidente {incidente.Id}");
        }
    }

    public async Task NotificarIncidenteEscaladoAsync(Incidente incidente, string motivo)
    {
        try
        {
            // Notificar a supervisores
            var supervisores = await GetUsuariosPorRolAsync(TipoUsuario.Supervisor);
            foreach (var supervisor in supervisores)
            {
                await CrearNotificacionAsync(
                    supervisor.Id,
                    TipoNotificacion.IncidenteEscalado,
                    "Incidente escalado",
                    $"El incidente #{incidente.NumeroIncidente} ha sido escalado. Motivo: {motivo}",
                    incidente.Id
                );
            }

            // Notificar al reportador
            await CrearNotificacionAsync(
                incidente.ReportadoPorId,
                TipoNotificacion.IncidenteEscalado,
                "Su incidente ha sido escalado",
                $"Su incidente #{incidente.NumeroIncidente} ha sido escalado para atención prioritaria.",
                incidente.Id
            );

            _logger.LogInformation($"Notificaciones enviadas por escalación de incidente {incidente.NumeroIncidente}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error enviando notificaciones por escalación de incidente {incidente.Id}");
        }
    }

    public async Task NotificarSLAProximoVencimientoAsync(Incidente incidente, TimeSpan tiempoRestante)
    {
        try
        {
            var mensaje = $"Alerta: El incidente #{incidente.NumeroIncidente} vence en {tiempoRestante.Hours} horas y {tiempoRestante.Minutes} minutos.";

            // Notificar al técnico asignado
            if (incidente.AsignadoAId.HasValue)
            {
                await CrearNotificacionAsync(
                    incidente.AsignadoAId.Value,
                    TipoNotificacion.SLAProximoVencimiento,
                    "SLA próximo a vencer",
                    mensaje,
                    incidente.Id
                );
            }

            // Notificar a supervisores
            var supervisores = await GetUsuariosPorRolAsync(TipoUsuario.Supervisor);
            foreach (var supervisor in supervisores)
            {
                await CrearNotificacionAsync(
                    supervisor.Id,
                    TipoNotificacion.SLAProximoVencimiento,
                    "SLA próximo a vencer",
                    mensaje,
                    incidente.Id
                );
            }

            _logger.LogInformation($"Notificaciones enviadas por SLA próximo a vencer: incidente {incidente.NumeroIncidente}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error enviando notificaciones por SLA próximo a vencer: incidente {incidente.Id}");
        }
    }

    public async Task NotificarSLAVencidoAsync(Incidente incidente)
    {
        try
        {
            var mensaje = $"CRÍTICO: El SLA del incidente #{incidente.NumeroIncidente} ha vencido. Se requiere escalación inmediata.";

            // Notificar a todos los supervisores y administradores
            var usuariosAdmin = await GetUsuariosPorRolAsync(TipoUsuario.Supervisor);
            var administradores = await GetUsuariosPorRolAsync(TipoUsuario.Administrador);
            
            var usuariosANotificar = usuariosAdmin.Concat(administradores);

            foreach (var usuario in usuariosANotificar)
            {
                await CrearNotificacionAsync(
                    usuario.Id,
                    TipoNotificacion.SLAVencido,
                    "SLA VENCIDO - Acción requerida",
                    mensaje,
                    incidente.Id
                );
            }

            _logger.LogWarning($"Notificaciones críticas enviadas por SLA vencido: incidente {incidente.NumeroIncidente}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error enviando notificaciones por SLA vencido: incidente {incidente.Id}");
        }
    }

    // ===== MÉTODOS PRIVADOS DE APOYO =====

    private PrioridadNotificacion DeterminarPrioridadPorTipo(TipoNotificacion tipo)
    {
        return tipo switch
        {
            TipoNotificacion.SLAVencido => PrioridadNotificacion.Critica,
            TipoNotificacion.IncidenteEscalado => PrioridadNotificacion.Alta,
            TipoNotificacion.SLAProximoVencimiento => PrioridadNotificacion.Alta,
            TipoNotificacion.IncidenteCreado => PrioridadNotificacion.Normal,
            TipoNotificacion.IncidenteAsignado => PrioridadNotificacion.Normal,
            _ => PrioridadNotificacion.Normal
        };
    }

    private async Task<IEnumerable<Usuario>> GetUsuariosPorRolAsync(TipoUsuario tipoUsuario)
    {
        // Implementar según el repositorio de usuarios
        // Esta es una implementación simplificada
        return new List<Usuario>();
    }

    private async Task ProcesarEnvioNotificacionAsync(Notificacion notificacion)
    {
        try
        {
            var configuracion = await GetConfiguracionUsuarioAsync(
                notificacion.UsuarioId, 
                DeterminarTipoEventoPorNotificacion(notificacion.TipoNotificacion)
            );

            // Envío por email si está configurado
            if (configuracion.NotificarPorEmail)
            {
                await EnviarNotificacionEmailAsync(notificacion);
            }

            // Envío por SMS si está configurado
            if (configuracion.NotificarPorSMS)
            {
                var usuario = await _usuarioRepository.GetByIdAsync(notificacion.UsuarioId);
                if (!string.IsNullOrEmpty(usuario?.Phone))
                {
                    await EnviarNotificacionSMSAsync(notificacion, usuario.Phone);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error procesando envío de notificación {notificacion.Id}");
        }
    }

    private TipoEventoNotificacion DeterminarTipoEventoPorNotificacion(TipoNotificacion tipo)
    {
        return tipo switch
        {
            TipoNotificacion.IncidenteCreado => TipoEventoNotificacion.IncidentesCreados,
            TipoNotificacion.IncidenteAsignado => TipoEventoNotificacion.IncidentesAsignados,
            TipoNotificacion.IncidenteActualizado => TipoEventoNotificacion.ActualizacionEstado,
            TipoNotificacion.IncidenteEscalado => TipoEventoNotificacion.EscalacionSLA,
            TipoNotificacion.SLAProximoVencimiento => TipoEventoNotificacion.EscalacionSLA,
            TipoNotificacion.SLAVencido => TipoEventoNotificacion.EscalacionSLA,
            _ => TipoEventoNotificacion.TodosLosIncidentes
        };
    }

    public async Task<bool> EnviarNotificacionEmailAsync(Notificacion notificacion)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(notificacion.UsuarioId);
            if (usuario == null || string.IsNullOrEmpty(usuario.Email))
                return false;

            var plantilla = await GetPlantillaAsync(notificacion.TipoNotificacion);
            var variables = new Dictionary<string, object>
            {
                ["UsuarioNombre"] = $"{usuario.FirstName} {usuario.LastName}",
                ["Titulo"] = notificacion.Titulo,
                ["Mensaje"] = notificacion.Mensaje,
                ["FechaNotificacion"] = notificacion.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            };

            var mensajeFinal = await GenerarMensajeDesdePlantillaAsync(plantilla, variables);
            
            var enviado = await _emailService.EnviarEmailAsync(
                usuario.Email, 
                notificacion.Titulo, 
                mensajeFinal
            );

            // Registrar el log
            await _notificationRepository.CreateLogAsync(new LogNotificacion
            {
                NotificacionId = notificacion.Id,
                Canal = CanalNotificacion.Email,
                Estado = enviado ? EstadoEnvioNotificacion.Enviado : EstadoEnvioNotificacion.Fallido,
                FechaIntento = DateTime.UtcNow,
                DireccionDestino = usuario.Email,
                CreatedAt = DateTime.UtcNow
            });

            if (enviado)
            {
                notificacion.NotificadoPorEmail = true;
                notificacion.FechaEnvioEmail = DateTime.UtcNow;
                await _notificationRepository.UpdateAsync(notificacion);
            }

            return enviado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error enviando email para notificación {notificacion.Id}");
            return false;
        }
    }

    public async Task<bool> EnviarNotificacionSMSAsync(Notificacion notificacion, string numeroTelefono)
    {
        try
        {
            var mensajeCorto = $"IncidentesFISEI: {notificacion.Titulo}. {notificacion.Mensaje.Substring(0, Math.Min(100, notificacion.Mensaje.Length))}...";
            
            var enviado = await _smsService.EnviarSMSAsync(numeroTelefono, mensajeCorto);

            // Registrar el log
            await _notificationRepository.CreateLogAsync(new LogNotificacion
            {
                NotificacionId = notificacion.Id,
                Canal = CanalNotificacion.SMS,
                Estado = enviado ? EstadoEnvioNotificacion.Enviado : EstadoEnvioNotificacion.Fallido,
                FechaIntento = DateTime.UtcNow,
                DireccionDestino = numeroTelefono,
                CreatedAt = DateTime.UtcNow
            });

            return enviado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error enviando SMS para notificación {notificacion.Id}");
            return false;
        }
    }

    public async Task<PlantillaNotificacion> GetPlantillaAsync(TipoNotificacion tipo)
    {
        var plantilla = await _notificationRepository.GetPlantillaAsync(tipo);
        
        if (plantilla == null)
        {
            // Plantilla por defecto
            plantilla = new PlantillaNotificacion
            {
                TipoNotificacion = tipo,
                Nombre = $"Plantilla por defecto - {tipo}",
                PlantillaTitulo = "{Titulo}",
                PlantillaMensaje = "{Mensaje}",
                IsActive = true
            };
        }

        return plantilla;
    }

    public async Task<string> GenerarMensajeDesdePlantillaAsync(PlantillaNotificacion plantilla, Dictionary<string, object> variables)
    {
        var mensaje = plantilla.PlantillaMensaje;

        foreach (var variable in variables)
        {
            var placeholder = $"{{{variable.Key}}}";
            mensaje = mensaje.Replace(placeholder, variable.Value?.ToString() ?? "");
        }

        return mensaje;
    }

    public async Task ProcesarColaNotificacionesAsync()
    {
        try
        {
            // Procesar notificaciones pendientes
            var notificacionesPendientes = await _notificationRepository.GetNotificacionesPendientesEnvioAsync();
            
            foreach (var notificacion in notificacionesPendientes)
            {
                await ProcesarEnvioNotificacionAsync(notificacion);
            }

            // Reintentar notificaciones fallidas
            var notificacionesParaReintentar = await _notificationRepository.GetNotificacionesParaReintentarAsync();
            
            foreach (var notificacion in notificacionesParaReintentar)
            {
                await ProcesarEnvioNotificacionAsync(notificacion);
            }

            _logger.LogInformation($"Procesadas {notificacionesPendientes.Count()} notificaciones pendientes y {notificacionesParaReintentar.Count()} reintentos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando cola de notificaciones");
        }
    }
}

// Interfaces auxiliares para servicios externos
public interface IEmailService
{
    Task<bool> EnviarEmailAsync(string destinatario, string asunto, string mensaje);
}

public interface ISMSService
{
    Task<bool> EnviarSMSAsync(string numeroTelefono, string mensaje);
}