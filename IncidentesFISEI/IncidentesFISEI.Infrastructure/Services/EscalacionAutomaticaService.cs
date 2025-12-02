using IncidentesFISEI.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IncidentesFISEI.Infrastructure.Services
{
    /// <summary>
    /// Servicio en segundo plano que ejecuta escalaciones automáticas
    /// Procesa incidentes estancados cada 10 minutos según tiempos ITIL
    /// </summary>
    public class EscalacionAutomaticaService : BackgroundService
    {
        private readonly ILogger<EscalacionAutomaticaService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(10);

        public EscalacionAutomaticaService(
            ILogger<EscalacionAutomaticaService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🟢 Escalación Automática Service iniciado. Intervalo: {Intervalo} minutos",
                _intervalo.TotalMinutes);

            // Esperar 2 minutos antes de la primera ejecución
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcesarEscalacionesAsync();
                    await Task.Delay(_intervalo, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Escalación Automática Service detenido");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error en Escalación Automática Service");
                    await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
                }
            }
        }

        private async Task ProcesarEscalacionesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var escalacionService = scope.ServiceProvider.GetRequiredService<IEscalacionService>();

            try
            {
                var cantidadEscalados = await escalacionService.ProcesarEscalacionesAutomaticasAsync();

                if (cantidadEscalados > 0)
                {
                    _logger.LogWarning("⬆️ Se escalaron automáticamente {Cantidad} incidentes", cantidadEscalados);
                }
                else
                {
                    _logger.LogInformation("✅ Proceso de escalación automática completado: 0 incidentes escalados");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar escalaciones automáticas");
            }
        }
    }
}
