using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace IncidentesFISEI.Application.Services;

public class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IIncidenteRepository _incidenteRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IArticuloConocimientoRepository _articuloRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificacionService> _logger;

    public NotificacionService(
        INotificacionRepository notificacionRepository,
        IIncidenteRepository incidenteRepository,
        IUsuarioRepository usuarioRepository,
        IArticuloConocimientoRepository articuloRepository,
        IEmailService emailService,
        ILogger<NotificacionService> logger)
    {
        _notificacionRepository = notificacionRepository;
        _incidenteRepository = incidenteRepository;
        _usuarioRepository = usuarioRepository;
        _articuloRepository = articuloRepository;
        _emailService = emailService;
        _logger = logger;
    }

    #region CRUD Básico

    public async Task<ApiResponse<NotificacionDto>> CrearNotificacionAsync(CreateNotificacionDto dto)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(dto.UsuarioId);
            if (usuario == null)
            {
                return new ApiResponse<NotificacionDto>(false, null, "Usuario no encontrado");
            }

            var notificacion = new Notificacion
            {
                UsuarioId = dto.UsuarioId,
                IncidenteId = dto.IncidenteId,
                Tipo = dto.Tipo,
                Titulo = dto.Titulo,
                Mensaje = dto.Mensaje,
                MetadataJson = dto.MetadataJson,
                Leida = false
            };

            await _notificacionRepository.AddAsync(notificacion);

            // Enviar email si es solicitado
            if (dto.EnviarEmail && !string.IsNullOrEmpty(usuario.Email))
            {
                var emailEnviado = await _emailService.EnviarEmailAsync(
                    usuario.Email,
                    dto.Titulo,
                    dto.Mensaje,
                    true
                );

                if (emailEnviado)
                {
                    notificacion.EnviadaPorEmail = true;
                    notificacion.FechaEnvioEmail = DateTime.UtcNow;
                    await _notificacionRepository.UpdateAsync(notificacion);
                }
            }

            var notificacionDto = MapToDto(notificacion, usuario);
            return new ApiResponse<NotificacionDto>(true, notificacionDto, "Notificación creada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear notificación");
            return new ApiResponse<NotificacionDto>(false, null, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<NotificacionesPaginadasDto>> GetNotificacionesUsuarioAsync(
        int usuarioId, int pagina = 1, int tamañoPagina = 20, bool soloNoLeidas = false)
    {
        try
        {
            var notificaciones = await _notificacionRepository.GetNotificacionesByUsuarioAsync(usuarioId, soloNoLeidas);
            var totalNotificaciones = notificaciones.Count();
            var noLeidas = await _notificacionRepository.GetCountNoLeidasAsync(usuarioId);

            var notificacionesPaginadas = notificaciones
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .Select(n => MapToDto(n, n.Usuario))
                .ToList();

            var resultado = new NotificacionesPaginadasDto
            {
                Notificaciones = notificacionesPaginadas,
                TotalNotificaciones = totalNotificaciones,
                NoLeidas = noLeidas,
                Pagina = pagina,
                TamañoPagina = tamañoPagina,
                TotalPaginas = (int)Math.Ceiling(totalNotificaciones / (double)tamañoPagina)
            };

            return new ApiResponse<NotificacionesPaginadasDto>(true, resultado, 
                $"{notificacionesPaginadas.Count} notificaciones obtenidas");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener notificaciones del usuario {UsuarioId}", usuarioId);
            return new ApiResponse<NotificacionesPaginadasDto>(false, null, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> MarcarComoLeidaAsync(int notificacionId)
    {
        try
        {
            await _notificacionRepository.MarcarComoLeidaAsync(notificacionId);
            return new ApiResponse<bool>(true, true, "Notificación marcada como leída");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar notificación {NotificacionId} como leída", notificacionId);
            return new ApiResponse<bool>(false, false, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> MarcarTodasComoLeidasAsync(int usuarioId)
    {
        try
        {
            await _notificacionRepository.MarcarTodasComoLeidasAsync(usuarioId);
            return new ApiResponse<bool>(true, true, "Todas las notificaciones marcadas como leídas");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar todas las notificaciones como leídas para usuario {UsuarioId}", usuarioId);
            return new ApiResponse<bool>(false, false, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<int>> GetCountNoLeidasAsync(int usuarioId)
    {
        try
        {
            var count = await _notificacionRepository.GetCountNoLeidasAsync(usuarioId);
            return new ApiResponse<int>(true, count, $"{count} notificaciones no leídas");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener contador de notificaciones no leídas");
            return new ApiResponse<int>(false, 0, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> EliminarNotificacionAsync(int notificacionId)
    {
        try
        {
            await _notificacionRepository.DeleteAsync(notificacionId);
            return new ApiResponse<bool>(true, true, "Notificación eliminada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar notificación {NotificacionId}", notificacionId);
            return new ApiResponse<bool>(false, false, $"Error: {ex.Message}");
        }
    }

    #endregion

    #region Notificaciones de Incidentes

    public async Task NotificarNuevoIncidenteAsync(int incidenteId, int reportadoPorId)
    {
        try
        {
            var incidente = await _incidenteRepository.GetIncidenteCompletoAsync(incidenteId);
            if (incidente == null) return;

            // Notificar a supervisores y administradores
            var supervisores = await _usuarioRepository.GetUsuariosByTipoAsync(TipoUsuario.Supervisor);
            var administradores = await _usuarioRepository.GetUsuariosByTipoAsync(TipoUsuario.Administrador);
            var todosSuper = supervisores.Concat(administradores);
            
            foreach (var supervisor in todosSuper)
            {
                var notificacion = new Notificacion
                {
                    UsuarioId = supervisor.Id,
                    IncidenteId = incidenteId,
                    Tipo = TipoNotificacion.IncidenteNuevo,
                    Titulo = $"Nuevo Incidente #{incidenteId}",
                    Mensaje = $"Se ha reportado un nuevo incidente: {incidente.Titulo}",
                    GrupoNotificacion = $"INCIDENTE_{incidenteId}"
                };

                await _notificacionRepository.AddAsync(notificacion);

                // Enviar email
                if (!string.IsNullOrEmpty(supervisor.Email))
                {
                    await _emailService.EnviarNotificacionIncidenteAsync(
                        supervisor.Email,
                        $"{supervisor.FirstName} {supervisor.LastName}",
                        incidenteId,
                        incidente.Titulo,
                        $"Prioridad: {incidente.Prioridad}, Categoría: {incidente.Categoria?.Nombre}",
                        "Nuevo Incidente Reportado"
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar nuevo incidente {IncidenteId}", incidenteId);
        }
    }

    public async Task NotificarAsignacionAsync(int incidenteId, int tecnicoId, int asignadoPorId)
    {
        try
        {
            var incidente = await _incidenteRepository.GetIncidenteCompletoAsync(incidenteId);
            var tecnico = await _usuarioRepository.GetByIdAsync(tecnicoId);
            var asignadoPor = await _usuarioRepository.GetByIdAsync(asignadoPorId);

            if (incidente == null || tecnico == null) return;

            var notificacion = new Notificacion
            {
                UsuarioId = tecnicoId,
                IncidenteId = incidenteId,
                Tipo = TipoNotificacion.IncidenteAsignado,
                Titulo = $"Incidente #{incidenteId} Asignado",
                Mensaje = $"Se te ha asignado el incidente: {incidente.Titulo}. Asignado por: {(asignadoPor != null ? $"{asignadoPor.FirstName} {asignadoPor.LastName}" : "Sistema")}",
                GrupoNotificacion = $"INCIDENTE_{incidenteId}"
            };

            await _notificacionRepository.AddAsync(notificacion);

            // Enviar email
            if (!string.IsNullOrEmpty(tecnico.Email))
            {
                await _emailService.EnviarNotificacionIncidenteAsync(
                    tecnico.Email,
                    $"{tecnico.FirstName} {tecnico.LastName}",
                    incidenteId,
                    incidente.Titulo,
                    $"El incidente ha sido asignado a ti por {(asignadoPor != null ? $"{asignadoPor.FirstName} {asignadoPor.LastName}" : "Sistema")}. Prioridad: {incidente.Prioridad}",
                    "Incidente Asignado"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar asignación del incidente {IncidenteId}", incidenteId);
        }
    }

    public async Task NotificarReasignacionAsync(int incidenteId, int nuevoTecnicoId, int anteriorTecnicoId, int reasignadoPorId)
    {
        try
        {
            var incidente = await _incidenteRepository.GetIncidenteCompletoAsync(incidenteId);
            var nuevoTecnico = await _usuarioRepository.GetByIdAsync(nuevoTecnicoId);
            var anteriorTecnico = await _usuarioRepository.GetByIdAsync(anteriorTecnicoId);

            if (incidente == null || nuevoTecnico == null) return;

            // Notificar al nuevo técnico
            var notificacionNuevo = new Notificacion
            {
                UsuarioId = nuevoTecnicoId,
                IncidenteId = incidenteId,
                Tipo = TipoNotificacion.IncidenteReasignado,
                Titulo = $"Incidente #{incidenteId} Reasignado a Ti",
                Mensaje = $"El incidente {incidente.Titulo} ha sido reasignado a ti.",
                GrupoNotificacion = $"INCIDENTE_{incidenteId}"
            };

            await _notificacionRepository.AddAsync(notificacionNuevo);

            // Notificar al técnico anterior
            if (anteriorTecnico != null)
            {
                var notificacionAnterior = new Notificacion
                {
                    UsuarioId = anteriorTecnicoId,
                    IncidenteId = incidenteId,
                    Tipo = TipoNotificacion.IncidenteReasignado,
                    Titulo = $"Incidente #{incidenteId} Reasignado",
                    Mensaje = $"El incidente {incidente.Titulo} ha sido reasignado a {nuevoTecnico.FirstName} {nuevoTecnico.LastName}.",
                    GrupoNotificacion = $"INCIDENTE_{incidenteId}"
                };

                await _notificacionRepository.AddAsync(notificacionAnterior);
            }

            // Emails
            if (!string.IsNullOrEmpty(nuevoTecnico.Email))
            {
                await _emailService.EnviarNotificacionIncidenteAsync(
                    nuevoTecnico.Email,
                    $"{nuevoTecnico.FirstName} {nuevoTecnico.LastName}",
                    incidenteId,
                    incidente.Titulo,
                    "El incidente ha sido reasignado a ti. Por favor, revisa los detalles.",
                    "Incidente Reasignado"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar reasignación del incidente {IncidenteId}", incidenteId);
        }
    }

    public async Task NotificarCambioEstadoAsync(int incidenteId, EstadoIncidente nuevoEstado, int cambiadoPorId)
    {
        try
        {
            var incidente = await _incidenteRepository.GetIncidenteCompletoAsync(incidenteId);
            if (incidente == null) return;

            var tipoNotificacion = nuevoEstado switch
            {
                EstadoIncidente.EnProgreso => TipoNotificacion.IncidenteEnProceso,
                EstadoIncidente.Resuelto => TipoNotificacion.IncidenteResuelto,
                EstadoIncidente.Cerrado => TipoNotificacion.IncidenteCerrado,
                _ => TipoNotificacion.ActualizacionIncidente
            };

            // Notificar al reportante
            if (incidente.ReportadoPorId > 0)
            {
                var reportante = await _usuarioRepository.GetByIdAsync(incidente.ReportadoPorId);
                if (reportante != null)
                {
                    var notificacion = new Notificacion
                    {
                        UsuarioId = reportante.Id,
                        IncidenteId = incidenteId,
                        Tipo = tipoNotificacion,
                        Titulo = $"Cambio de Estado - Incidente #{incidenteId}",
                        Mensaje = $"El incidente {incidente.Titulo} cambió a estado: {nuevoEstado}",
                        GrupoNotificacion = $"INCIDENTE_{incidenteId}"
                    };

                    await _notificacionRepository.AddAsync(notificacion);

                    if (!string.IsNullOrEmpty(reportante.Email))
                    {
                        await _emailService.EnviarNotificacionIncidenteAsync(
                            reportante.Email,
                            $"{reportante.FirstName} {reportante.LastName}",
                            incidenteId,
                            incidente.Titulo,
                            $"El estado del incidente ha cambiado a: {nuevoEstado}",
                            $"Cambio de Estado: {nuevoEstado}"
                        );
                    }
                }
            }

            // Notificar al técnico asignado si es diferente del que cambió el estado
            if (incidente.AsignadoAId.HasValue && incidente.AsignadoAId.Value != cambiadoPorId)
            {
                var tecnico = await _usuarioRepository.GetByIdAsync(incidente.AsignadoAId.Value);
                if (tecnico != null)
                {
                    var notificacion = new Notificacion
                    {
                        UsuarioId = tecnico.Id,
                        IncidenteId = incidenteId,
                        Tipo = tipoNotificacion,
                        Titulo = $"Cambio de Estado - Incidente #{incidenteId}",
                        Mensaje = $"El incidente {incidente.Titulo} cambió a estado: {nuevoEstado}",
                        GrupoNotificacion = $"INCIDENTE_{incidenteId}"
                    };

                    await _notificacionRepository.AddAsync(notificacion);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar cambio de estado del incidente {IncidenteId}", incidenteId);
        }
    }

    public async Task NotificarCambioPrioridadAsync(int incidenteId, PrioridadIncidente nuevaPrioridad, int cambiadoPorId)
    {
        try
        {
            var incidente = await _incidenteRepository.GetIncidenteCompletoAsync(incidenteId);
            if (incidente == null) return;

            var tipoNotificacion = nuevaPrioridad == PrioridadIncidente.Critica 
                ? TipoNotificacion.IncidenteCritico 
                : TipoNotificacion.CambioPrioridad;

            // Notificar al técnico asignado
            if (incidente.AsignadoAId.HasValue && incidente.AsignadoAId > 0)
            {
                var tecnico = await _usuarioRepository.GetByIdAsync(incidente.AsignadoAId.Value);
                if (tecnico != null)
                {
                    var notificacion = new Notificacion
                    {
                        UsuarioId = tecnico.Id,
                        IncidenteId = incidenteId,
                        Tipo = tipoNotificacion,
                        Titulo = $"Cambio de Prioridad - Incidente #{incidenteId}",
                        Mensaje = $"La prioridad del incidente {incidente.Titulo} cambió a: {nuevaPrioridad}",
                        GrupoNotificacion = $"INCIDENTE_{incidenteId}"
                    };

                    await _notificacionRepository.AddAsync(notificacion);

                    if (!string.IsNullOrEmpty(tecnico.Email) && nuevaPrioridad == PrioridadIncidente.Critica)
                    {
                        await _emailService.EnviarNotificacionIncidenteAsync(
                            tecnico.Email,
                            tecnico.NombreCompleto,
                            incidenteId,
                            incidente.Titulo,
                            "⚠️ Este incidente ahora es CRÍTICO. Se requiere atención inmediata.",
                            "ALERTA: Prioridad Crítica"
                        );
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar cambio de prioridad del incidente {IncidenteId}", incidenteId);
        }
    }

    #endregion

    #region Notificaciones de SLA y Escalaciones

    public async Task NotificarSLAProximoVencimientoAsync(int incidenteId, int tecnicoAsignadoId, TimeSpan tiempoRestante)
    {
        try
        {
            var incidente = await _incidenteRepository.GetIncidenteCompletoAsync(incidenteId);
            var tecnico = await _usuarioRepository.GetByIdAsync(tecnicoAsignadoId);

            if (incidente == null || tecnico == null) return;

            var notificacion = new Notificacion
            {
                UsuarioId = tecnicoAsignadoId,
                IncidenteId = incidenteId,
                Tipo = TipoNotificacion.SLAProximoVencimiento,
                Titulo = $"⏰ SLA Próximo a Vencer - Incidente #{incidenteId}",
                Mensaje = $"El SLA del incidente {incidente.Titulo} vence en {tiempoRestante.Hours}h {tiempoRestante.Minutes}m",
                GrupoNotificacion = $"SLA_{incidenteId}"
            };

            await _notificacionRepository.AddAsync(notificacion);

            // Notificación limitada (sin FechaLimiteSLA disponible en entidad Incidente actual)
            // TODO: Agregar lógica cuando esté disponible FechaLimiteSLA
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar SLA próximo a vencer del incidente {IncidenteId}", incidenteId);
        }
    }

    public async Task NotificarSLAVencidoAsync(int incidenteId, int tecnicoAsignadoId)
    {
        try
        {
            var incidente = await _incidenteRepository.GetIncidenteCompletoAsync(incidenteId);
            var tecnico = await _usuarioRepository.GetByIdAsync(tecnicoAsignadoId);

            if (incidente == null || tecnico == null) return;

            var notificacion = new Notificacion
            {
                UsuarioId = tecnicoAsignadoId,
                IncidenteId = incidenteId,
                Tipo = TipoNotificacion.SLAVencido,
                Titulo = $"⚠️ SLA VENCIDO - Incidente #{incidenteId}",
                Mensaje = $"El SLA del incidente {incidente.Titulo} ha vencido. Se requiere acción inmediata.",
                GrupoNotificacion = $"SLA_{incidenteId}"
            };

            await _notificacionRepository.AddAsync(notificacion);

            // Notificar también a supervisores
            var supervisores = await _usuarioRepository.GetSupervisoresAsync();
            foreach (var supervisor in supervisores)
            {
                var notifSupervisor = new Notificacion
                {
                    UsuarioId = supervisor.Id,
                    IncidenteId = incidenteId,
                    Tipo = TipoNotificacion.SLAVencido,
                    Titulo = $"⚠️ SLA VENCIDO - Incidente #{incidenteId}",
                    Mensaje = $"El SLA del incidente {incidente.Titulo} ha vencido. Asignado a: {tecnico.NombreCompleto}",
                    GrupoNotificacion = $"SLA_{incidenteId}"
                };

                await _notificacionRepository.AddAsync(notifSupervisor);
            }

            // Notificación limitada (sin FechaLimiteSLA disponible)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar SLA vencido del incidente {IncidenteId}", incidenteId);
        }
    }

    public async Task NotificarEscalacionAsync(int incidenteId, int supervisorId, string motivo)
    {
        try
        {
            var incidente = await _incidenteRepository.GetIncidenteCompletoAsync(incidenteId);
            var supervisor = await _usuarioRepository.GetByIdAsync(supervisorId);

            if (incidente == null || supervisor == null) return;

            var notificacion = new Notificacion
            {
                UsuarioId = supervisorId,
                IncidenteId = incidenteId,
                Tipo = TipoNotificacion.IncidenteEscalado,
                Titulo = $"🔺 Escalación - Incidente #{incidenteId}",
                Mensaje = $"El incidente {incidente.Titulo} ha sido escalado. Motivo: {motivo}",
                GrupoNotificacion = $"ESCALACION_{incidenteId}"
            };

            await _notificacionRepository.AddAsync(notificacion);

            if (!string.IsNullOrEmpty(supervisor.Email))
            {
                await _emailService.EnviarNotificacionEscalacionAsync(
                    supervisor.Email,
                    supervisor.NombreCompleto,
                    incidenteId,
                    incidente.Titulo,
                    motivo,
                    "Nivel 2"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar escalación del incidente {IncidenteId}", incidenteId);
        }
    }

    #endregion

    #region Notificaciones de Comentarios

    public async Task NotificarNuevoComentarioAsync(int incidenteId, int comentarioId, int autorId)
    {
        try
        {
            var incidente = await _incidenteRepository.GetIncidenteCompletoAsync(incidenteId);
            if (incidente == null) return;

            var usuariosANotificar = new List<int>();

            // Notificar al técnico asignado
            if (incidente.AsignadoAId.HasValue && incidente.AsignadoAId > 0 && incidente.AsignadoAId.Value != autorId)
            {
                usuariosANotificar.Add(incidente.AsignadoAId.Value);
            }

            // Notificar al reportante
            if (incidente.ReportadoPorId > 0 && incidente.ReportadoPorId != autorId)
            {
                usuariosANotificar.Add(incidente.ReportadoPorId);
            }

            var autor = await _usuarioRepository.GetByIdAsync(autorId);

            foreach (var usuarioId in usuariosANotificar.Distinct())
            {
                var notificacion = new Notificacion
                {
                    UsuarioId = usuarioId,
                    IncidenteId = incidenteId,
                    Tipo = TipoNotificacion.NuevoComentario,
                    Titulo = $"Nuevo Comentario - Incidente #{incidenteId}",
                    Mensaje = $"{autor?.NombreCompleto} ha agregado un comentario en {incidente.Titulo}",
                    GrupoNotificacion = $"INCIDENTE_{incidenteId}"
                };

                await _notificacionRepository.AddAsync(notificacion);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar nuevo comentario en incidente {IncidenteId}", incidenteId);
        }
    }

    public async Task NotificarRespuestaComentarioAsync(int incidenteId, int comentarioId, int autorRespuestaId, int autorComentarioOriginalId)
    {
        try
        {
            if (autorRespuestaId == autorComentarioOriginalId) return;

            var incidente = await _incidenteRepository.GetIncidenteCompletoAsync(incidenteId);
            var autorRespuesta = await _usuarioRepository.GetByIdAsync(autorRespuestaId);

            if (incidente == null || autorRespuesta == null) return;

            var notificacion = new Notificacion
            {
                UsuarioId = autorComentarioOriginalId,
                IncidenteId = incidenteId,
                Tipo = TipoNotificacion.RespuestaComentario,
                Titulo = $"Respuesta a tu Comentario - Incidente #{incidenteId}",
                Mensaje = $"{autorRespuesta.NombreCompleto} ha respondido a tu comentario en {incidente.Titulo}",
                GrupoNotificacion = $"INCIDENTE_{incidenteId}"
            };

            await _notificacionRepository.AddAsync(notificacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar respuesta a comentario");
        }
    }

    #endregion

    #region Notificaciones de Base de Conocimiento

    public async Task NotificarSolicitudValidacionAsync(int articuloId, int validadorId, int solicitadoPorId)
    {
        try
        {
            var articulo = await _articuloRepository.GetArticuloCompletoAsync(articuloId);
            var solicitante = await _usuarioRepository.GetByIdAsync(solicitadoPorId);

            if (articulo == null || solicitante == null) return;

            var notificacion = new Notificacion
            {
                UsuarioId = validadorId,
                Tipo = TipoNotificacion.SolicitudValidacion,
                Titulo = "Solicitud de Validación de Artículo",
                Mensaje = $"{solicitante.NombreCompleto} solicita validación del artículo: {articulo.Titulo}",
                MetadataJson = $"{{\"articuloId\": {articuloId}}}"
            };

            await _notificacionRepository.AddAsync(notificacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar solicitud de validación");
        }
    }

    public async Task NotificarArticuloValidadoAsync(int articuloId, int autorId, bool aprobado, string? comentarios)
    {
        try
        {
            var articulo = await _articuloRepository.GetArticuloCompletoAsync(articuloId);
            if (articulo == null) return;

            var tipo = aprobado ? TipoNotificacion.ArticuloAprobado : TipoNotificacion.ArticuloRechazado;
            var estado = aprobado ? "aprobado" : "rechazado";

            var notificacion = new Notificacion
            {
                UsuarioId = autorId,
                Tipo = tipo,
                Titulo = $"Artículo {estado}",
                Mensaje = $"Tu artículo '{articulo.Titulo}' ha sido {estado}. {comentarios}",
                MetadataJson = $"{{\"articuloId\": {articuloId}, \"aprobado\": {aprobado.ToString().ToLower()}}}"
            };

            await _notificacionRepository.AddAsync(notificacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar artículo validado");
        }
    }

    #endregion

    #region Estadísticas

    public async Task<ApiResponse<EstadisticasNotificacionesDto>> GetEstadisticasNotificacionesAsync(int usuarioId)
    {
        try
        {
            var todasNotificaciones = await _notificacionRepository.GetNotificacionesByUsuarioAsync(usuarioId);
            var noLeidas = await _notificacionRepository.GetCountNoLeidasAsync(usuarioId);

            var notificacionesPorTipo = todasNotificaciones
                .GroupBy(n => n.Tipo.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            var ultimasNotificaciones = todasNotificaciones
                .Take(10)
                .Select(n => MapToDto(n, n.Usuario))
                .ToList();

            var estadisticas = new EstadisticasNotificacionesDto
            {
                TotalNotificaciones = todasNotificaciones.Count(),
                NoLeidas = noLeidas,
                Leidas = todasNotificaciones.Count(n => n.Leida),
                EnviadasPorEmail = todasNotificaciones.Count(n => n.EnviadaPorEmail),
                NotificacionesPorTipo = notificacionesPorTipo,
                UltimasNotificaciones = ultimasNotificaciones
            };

            return new ApiResponse<EstadisticasNotificacionesDto>(true, estadisticas, "Estadísticas obtenidas");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de notificaciones");
            return new ApiResponse<EstadisticasNotificacionesDto>(false, null, $"Error: {ex.Message}");
        }
    }

    #endregion

    #region Helpers

    private static NotificacionDto MapToDto(Notificacion notificacion, Usuario usuario)
    {
        return new NotificacionDto
        {
            Id = notificacion.Id,
            UsuarioId = notificacion.UsuarioId,
            UsuarioNombre = usuario.NombreCompleto,
            IncidenteId = notificacion.IncidenteId,
            IncidenteTitulo = notificacion.Incidente?.Titulo,
            Tipo = notificacion.Tipo,
            TipoDescripcion = notificacion.Tipo.ToString(),
            Titulo = notificacion.Titulo,
            Mensaje = notificacion.Mensaje,
            Leida = notificacion.Leida,
            FechaLectura = notificacion.FechaLectura,
            EnviadaPorEmail = notificacion.EnviadaPorEmail,
            FechaEnvioEmail = notificacion.FechaEnvioEmail,
            CreatedAt = notificacion.CreatedAt,
            TiempoTranscurrido = CalcularTiempoTranscurrido(notificacion.CreatedAt)
        };
    }

    private static string CalcularTiempoTranscurrido(DateTime fecha)
    {
        var transcurrido = DateTime.UtcNow - fecha;

        if (transcurrido.TotalMinutes < 1)
            return "Hace unos segundos";
        if (transcurrido.TotalMinutes < 60)
            return $"Hace {(int)transcurrido.TotalMinutes} minuto(s)";
        if (transcurrido.TotalHours < 24)
            return $"Hace {(int)transcurrido.TotalHours} hora(s)";
        if (transcurrido.TotalDays < 7)
            return $"Hace {(int)transcurrido.TotalDays} día(s)";
        
        return fecha.ToString("dd/MM/yyyy HH:mm");
    }

    #endregion
}
