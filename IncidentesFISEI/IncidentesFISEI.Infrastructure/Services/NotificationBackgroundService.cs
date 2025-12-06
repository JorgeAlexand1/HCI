using IncidentesFISEI.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IncidentesFISEI.Infrastructure.Services;

/// <summary>
/// Servicio para procesar la cola de notificaciones
/// siguiendo las mejores prácticas de ITIL v3 para comunicación automatizada
/// </summary>
public class NotificationProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationProcessor> _logger;

    public NotificationProcessor(
        IServiceProvider serviceProvider,
        ILogger<NotificationProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ProcesarNotificacionesPendientesAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            
            await notificationService.ProcesarColaNotificacionesAsync();
            
            _logger.LogDebug("Cola de notificaciones procesada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando cola de notificaciones");
        }
    }
}