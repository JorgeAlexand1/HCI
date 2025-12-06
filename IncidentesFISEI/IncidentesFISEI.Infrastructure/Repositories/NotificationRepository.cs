using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentesFISEI.Infrastructure.Repositories;

/// <summary>
/// Repositorio para la gestión de notificaciones ITIL v3
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Notificacion> CreateAsync(Notificacion notificacion)
    {
        _context.Notificaciones.Add(notificacion);
        await _context.SaveChangesAsync();
        return notificacion;
    }

    public async Task<Notificacion> GetByIdAsync(int id)
    {
        return await _context.Notificaciones
            .Include(n => n.Usuario)
            .Include(n => n.Incidente)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<IEnumerable<Notificacion>> GetByUsuarioIdAsync(int usuarioId, bool soloNoLeidas = false, int page = 1, int pageSize = 50)
    {
        IQueryable<Notificacion> query = _context.Notificaciones
            .Where(n => n.UsuarioId == usuarioId)
            .Include(n => n.Incidente);

        if (soloNoLeidas)
        {
            query = query.Where(n => !n.Leida);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountNoLeidasAsync(int usuarioId)
    {
        return await _context.Notificaciones
            .CountAsync(n => n.UsuarioId == usuarioId && !n.Leida);
    }

    public async Task UpdateAsync(Notificacion notificacion)
    {
        notificacion.UpdatedAt = DateTime.UtcNow;
        _context.Notificaciones.Update(notificacion);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var notificacion = await _context.Notificaciones.FindAsync(id);
        if (notificacion != null)
        {
            _context.Notificaciones.Remove(notificacion);
            await _context.SaveChangesAsync();
        }
    }

    // === CONFIGURACIONES ===

    public async Task<ConfiguracionNotificacion> GetConfiguracionAsync(int usuarioId, TipoEventoNotificacion tipoEvento)
    {
        return await _context.ConfiguracionesNotificacion
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.TipoEvento == tipoEvento);
    }

    public async Task<ConfiguracionNotificacion> CreateConfiguracionAsync(ConfiguracionNotificacion config)
    {
        _context.ConfiguracionesNotificacion.Add(config);
        await _context.SaveChangesAsync();
        return config;
    }

    public async Task UpdateConfiguracionAsync(ConfiguracionNotificacion config)
    {
        config.UpdatedAt = DateTime.UtcNow;
        _context.ConfiguracionesNotificacion.Update(config);
        await _context.SaveChangesAsync();
    }

    // === PLANTILLAS ===

    public async Task<PlantillaNotificacion> GetPlantillaAsync(TipoNotificacion tipo)
    {
        return await _context.PlantillasNotificacion
            .FirstOrDefaultAsync(p => p.TipoNotificacion == tipo && p.IsActive);
    }

    public async Task<IEnumerable<PlantillaNotificacion>> GetPlantillasActivasAsync()
    {
        return await _context.PlantillasNotificacion
            .Where(p => p.IsActive)
            .OrderBy(p => p.TipoNotificacion)
            .ToListAsync();
    }

    // === LOGS ===

    public async Task<LogNotificacion> CreateLogAsync(LogNotificacion log)
    {
        _context.LogsNotificacion.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<IEnumerable<LogNotificacion>> GetLogsAsync(int notificacionId)
    {
        return await _context.LogsNotificacion
            .Where(l => l.NotificacionId == notificacionId)
            .OrderByDescending(l => l.FechaIntento)
            .ToListAsync();
    }

    // === NOTIFICACIONES PENDIENTES ===

    public async Task<IEnumerable<Notificacion>> GetNotificacionesPendientesEnvioAsync()
    {
        var hace5Minutos = DateTime.UtcNow.AddMinutes(-5);
        
        return await _context.Notificaciones
            .Where(n => n.CreatedAt >= hace5Minutos && 
                       (!n.NotificadoPorEmail || !n.NotificadoPorSMS))
            .Include(n => n.Usuario)
            .Include(n => n.Incidente)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notificacion>> GetNotificacionesParaReintentarAsync()
    {
        // Obtener notificaciones que tuvieron fallos en el envío hace más de 30 minutos
        // y no se han reintentado en las últimas 2 horas
        var hace30Minutos = DateTime.UtcNow.AddMinutes(-30);
        var hace2Horas = DateTime.UtcNow.AddHours(-2);

        var notificacionesConFallos = await (
            from n in _context.Notificaciones
            join log in _context.LogsNotificacion on n.Id equals log.NotificacionId
            where log.Estado == EstadoEnvioNotificacion.Fallido &&
                  log.FechaIntento >= hace2Horas &&
                  log.FechaIntento <= hace30Minutos &&
                  !_context.LogsNotificacion.Any(l2 => 
                      l2.NotificacionId == n.Id && 
                      l2.FechaIntento > log.FechaIntento &&
                      l2.Estado == EstadoEnvioNotificacion.Enviado)
            select n
        ).Distinct()
        .Include(n => n.Usuario)
        .Include(n => n.Incidente)
        .ToListAsync();

        return notificacionesConFallos;
    }
}