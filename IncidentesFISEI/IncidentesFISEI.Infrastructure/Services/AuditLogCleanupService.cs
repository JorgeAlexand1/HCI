using IncidentesFISEI.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IncidentesFISEI.Infrastructure.Services;

/// <summary>
/// Background service que limpia logs de auditoría antiguos periódicamente
/// </summary>
public class AuditLogCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditLogCleanupService> _logger;
    private readonly TimeSpan _intervalo = TimeSpan.FromHours(24); // Ejecutar diariamente
    private readonly int _diasRetencion = 90; // Retener logs por 90 días

    public AuditLogCleanupService(
        IServiceProvider serviceProvider,
        ILogger<AuditLogCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AuditLogCleanupService iniciado. Limpieza cada {Intervalo}, retención: {Dias} días",
            _intervalo, _diasRetencion);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await LimpiarLogsAntiguosAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en limpieza automática de logs de auditoría");
            }

            await Task.Delay(_intervalo, stoppingToken);
        }
    }

    private async Task LimpiarLogsAntiguosAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var auditLogService = scope.ServiceProvider.GetRequiredService<IAuditLogService>();

        try
        {
            var eliminados = await auditLogService.LimpiarLogsAntiguosAsync(_diasRetencion);

            if (eliminados > 0)
            {
                _logger.LogInformation("Limpieza automática completada: {Count} logs eliminados", eliminados);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al limpiar logs antiguos");
        }
    }
}
