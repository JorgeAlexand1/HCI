using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace IncidentesFISEI.Application.Services
{
    public class MetricasService : IMetricasService
    {
        private readonly IIncidenteRepository _incidenteRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICategoriaIncidenteRepository _categoriaRepository;
        private readonly IServicioDITICRepository _servicioRepository;
        private readonly IRepository<SLA> _slaRepository;
        private readonly ILogger<MetricasService> _logger;

        public MetricasService(
            IIncidenteRepository incidenteRepository,
            IUsuarioRepository usuarioRepository,
            ICategoriaIncidenteRepository categoriaRepository,
            IServicioDITICRepository servicioRepository,
            IRepository<SLA> slaRepository,
            ILogger<MetricasService> logger)
        {
            _incidenteRepository = incidenteRepository;
            _usuarioRepository = usuarioRepository;
            _categoriaRepository = categoriaRepository;
            _servicioRepository = servicioRepository;
            _slaRepository = slaRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<DashboardKPIsDto>> ObtenerKPIsPrincipalesAsync(DateTime? desde = null, DateTime? hasta = null)
        {
            try
            {
                var ahora = DateTime.UtcNow;
                desde ??= ahora.AddDays(-30);
                hasta ??= ahora;

                var incidentes = (await _incidenteRepository.GetAllAsync())
                    .Where(i => i.FechaReporte >= desde && i.FechaReporte <= hasta)
                    .ToList();

                var incidentesPeriodoAnterior = (await _incidenteRepository.GetAllAsync())
                    .Where(i => i.FechaReporte >= desde.Value.AddDays(-30) && i.FechaReporte < desde)
                    .ToList();

                var tecnicos = (await _usuarioRepository.GetAllAsync())
                    .Where(u => u.TipoUsuario == TipoUsuario.Tecnico || u.TipoUsuario == TipoUsuario.Supervisor)
                    .Where(u => u.IsActive)
                    .ToList();

                var kpis = new DashboardKPIsDto
                {
                    TotalIncidentes = incidentes.Count,
                    IncidentesAbiertos = incidentes.Count(i => i.Estado == EstadoIncidente.Abierto),
                    IncidentesEnProgreso = incidentes.Count(i => i.Estado == EstadoIncidente.EnProgreso),
                    IncidentesResueltos = incidentes.Count(i => i.Estado == EstadoIncidente.Resuelto),
                    IncidentesCerrados = incidentes.Count(i => i.Estado == EstadoIncidente.Cerrado),
                    
                    // Métricas por prioridad
                    IncidentesCriticos = incidentes.Count(i => i.Prioridad == PrioridadIncidente.Critica),
                    IncidentesAltos = incidentes.Count(i => i.Prioridad == PrioridadIncidente.Alta),
                    IncidentesMedios = incidentes.Count(i => i.Prioridad == PrioridadIncidente.Media),
                    IncidentesBajos = incidentes.Count(i => i.Prioridad == PrioridadIncidente.Baja),
                    
                    // Métricas de carga
                    TecnicosActivos = tecnicos.Count,
                    CargaPromedioTecnicos = tecnicos.Any() ? tecnicos.Average(t => t.CargaTrabajoActual) : 0,
                    IncidentesSinAsignar = incidentes.Count(i => !i.AsignadoAId.HasValue),
                    
                    // Escalación
                    TotalEscalaciones = incidentes.Sum(i => i.NumeroEscalaciones)
                };

                // Tiempos promedio
                var incidentesResueltos = incidentes.Where(i => i.FechaResolucion.HasValue).ToList();
                if (incidentesResueltos.Any())
                {
                    kpis.TiempoPromedioResolucion = incidentesResueltos
                        .Average(i => (i.FechaResolucion!.Value - i.FechaReporte).TotalHours);
                }

                var incidentesConRespuesta = incidentes.Where(i => i.FechaAsignacion.HasValue).ToList();
                if (incidentesConRespuesta.Any())
                {
                    kpis.TiempoPromedioRespuesta = incidentesConRespuesta
                        .Average(i => (i.FechaAsignacion!.Value - i.FechaReporte).TotalHours);
                    
                    kpis.TiempoPromedioPrimeraAsignacion = kpis.TiempoPromedioRespuesta;
                }

                // SLA
                var slas = (await _slaRepository.GetAllAsync()).ToList();
                int incidentesDentroSLA = 0;
                int incidentesFueraSLA = 0;

                foreach (var incidente in incidentesResueltos)
                {
                    var sla = slas.FirstOrDefault(s => 
                        s.Prioridad == incidente.Prioridad &&
                        s.Impacto == incidente.Impacto);
                    
                    if (sla != null)
                    {
                        var tiempoResolucion = (incidente.FechaResolucion!.Value - incidente.FechaReporte).TotalMinutes;
                        if (tiempoResolucion <= sla.TiempoResolucion)
                            incidentesDentroSLA++;
                        else
                            incidentesFueraSLA++;
                    }
                }

                kpis.IncidentesDentroSLA = incidentesDentroSLA;
                kpis.IncidentesFueraSLA = incidentesFueraSLA;
                
                if (incidentesDentroSLA + incidentesFueraSLA > 0)
                {
                    kpis.PorcentajeCumplimientoSLA = (incidentesDentroSLA * 100.0) / 
                        (incidentesDentroSLA + incidentesFueraSLA);
                }

                // Tasa de resolución en primer nivel (FCR)
                var incidentesEnL1 = incidentes.Where(i => 
                    i.NivelActual == NivelSoporte.L1_Tecnico && 
                    i.Estado == EstadoIncidente.Resuelto).Count();
                
                if (incidentesResueltos.Any())
                {
                    kpis.TasaResolucionPrimerNivel = (incidentesEnL1 * 100.0) / incidentesResueltos.Count;
                }

                // Escalaciones
                var incidentesEscalados = incidentes.Count(i => i.NumeroEscalaciones > 0);
                if (incidentes.Any())
                {
                    kpis.PorcentajeIncidentesEscalados = (incidentesEscalados * 100.0) / incidentes.Count;
                }

                // Tendencias comparativas
                if (incidentesPeriodoAnterior.Any())
                {
                    kpis.CambioIncidentesTotales = ((incidentes.Count - incidentesPeriodoAnterior.Count) * 100.0) / 
                        incidentesPeriodoAnterior.Count;
                    
                    var tiempoAnterior = incidentesPeriodoAnterior
                        .Where(i => i.FechaResolucion.HasValue)
                        .DefaultIfEmpty()
                        .Average(i => i != null ? (i.FechaResolucion!.Value - i.FechaReporte).TotalHours : 0);
                    
                    if (tiempoAnterior > 0)
                    {
                        kpis.CambioTiempoResolucion = ((kpis.TiempoPromedioResolucion - tiempoAnterior) * 100.0) / tiempoAnterior;
                    }
                }

                return new ApiResponse<DashboardKPIsDto>(true, kpis, "KPIs calculados correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular KPIs");
                return new ApiResponse<DashboardKPIsDto>(false, new DashboardKPIsDto(), $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<EstadisticasPorCategoriaDto>>> ObtenerEstadisticasPorCategoriaAsync(
            DateTime? desde = null, DateTime? hasta = null)
        {
            try
            {
                desde ??= DateTime.UtcNow.AddDays(-30);
                hasta ??= DateTime.UtcNow;

                var incidentes = (await _incidenteRepository.GetAllAsync())
                    .Where(i => i.FechaReporte >= desde && i.FechaReporte <= hasta)
                    .ToList();

                var categorias = await _categoriaRepository.GetAllAsync();
                var slas = (await _slaRepository.GetAllAsync()).ToList();
                
                var estadisticas = new List<EstadisticasPorCategoriaDto>();
                var totalIncidentes = incidentes.Count;

                foreach (var categoria in categorias)
                {
                    var incidentesCategoria = incidentes.Where(i => i.CategoriaId == categoria.Id).ToList();
                    var incidentesResueltos = incidentesCategoria.Where(i => i.FechaResolucion.HasValue).ToList();

                    var est = new EstadisticasPorCategoriaDto
                    {
                        CategoriaId = categoria.Id,
                        CategoriaNombre = categoria.Nombre,
                        TotalIncidentes = incidentesCategoria.Count,
                        IncidentesAbiertos = incidentesCategoria.Count(i => i.Estado == EstadoIncidente.Abierto),
                        IncidentesResueltos = incidentesResueltos.Count,
                        PorcentajeDelTotal = totalIncidentes > 0 ? (incidentesCategoria.Count * 100.0) / totalIncidentes : 0
                    };

                    if (incidentesResueltos.Any())
                    {
                        est.TiempoPromedioResolucion = incidentesResueltos
                            .Average(i => (i.FechaResolucion!.Value - i.FechaReporte).TotalHours);
                        
                        // Calcular SLA por categoría
                        int dentroSLA = 0;
                        int totalConSLA = 0;

                        foreach (var inc in incidentesResueltos)
                        {
                            var sla = slas.FirstOrDefault(s => s.Prioridad == inc.Prioridad && s.Impacto == inc.Impacto);
                            if (sla != null)
                            {
                                totalConSLA++;
                                var tiempo = (inc.FechaResolucion!.Value - inc.FechaReporte).TotalMinutes;
                                if (tiempo <= sla.TiempoResolucion)
                                    dentroSLA++;
                            }
                        }

                        est.PorcentajeCumplimientoSLA = totalConSLA > 0 ? (dentroSLA * 100.0) / totalConSLA : 0;
                    }

                    estadisticas.Add(est);
                }

                return new ApiResponse<List<EstadisticasPorCategoriaDto>>(true, 
                    estadisticas.OrderByDescending(e => e.TotalIncidentes).ToList(), 
                    $"Estadísticas de {estadisticas.Count} categorías");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular estadísticas por categoría");
                return new ApiResponse<List<EstadisticasPorCategoriaDto>>(false, 
                    new List<EstadisticasPorCategoriaDto>(), $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<EstadisticasPorTecnicoDto>>> ObtenerEstadisticasPorTecnicoAsync(
            DateTime? desde = null, DateTime? hasta = null)
        {
            try
            {
                desde ??= DateTime.UtcNow.AddDays(-30);
                hasta ??= DateTime.UtcNow;

                var tecnicos = (await _usuarioRepository.GetAllAsync())
                    .Where(u => u.TipoUsuario == TipoUsuario.Tecnico || u.TipoUsuario == TipoUsuario.Supervisor)
                    .ToList();

                var incidentes = (await _incidenteRepository.GetAllAsync())
                    .Where(i => i.FechaReporte >= desde && i.FechaReporte <= hasta && i.AsignadoAId.HasValue)
                    .ToList();

                var slas = (await _slaRepository.GetAllAsync()).ToList();
                var estadisticas = new List<EstadisticasPorTecnicoDto>();

                foreach (var tecnico in tecnicos)
                {
                    var incidentesTecnico = incidentes.Where(i => i.AsignadoAId == tecnico.Id).ToList();
                    var incidentesResueltos = incidentesTecnico.Where(i => i.FechaResolucion.HasValue).ToList();

                    var est = new EstadisticasPorTecnicoDto
                    {
                        TecnicoId = tecnico.Id,
                        TecnicoNombre = $"{tecnico.FirstName} {tecnico.LastName}",
                        NivelSoporte = tecnico.NivelSoporte?.ToString(),
                        IncidentesAsignados = incidentesTecnico.Count,
                        IncidentesResueltos = incidentesResueltos.Count,
                        IncidentesEnProgreso = incidentesTecnico.Count(i => i.Estado == EstadoIncidente.EnProgreso),
                        CargaActual = tecnico.CargaTrabajoActual,
                        TasaResolucion = incidentesTecnico.Any() ? 
                            (incidentesResueltos.Count * 100.0) / incidentesTecnico.Count : 0
                    };

                    if (incidentesResueltos.Any())
                    {
                        est.TiempoPromedioResolucion = incidentesResueltos
                            .Average(i => (i.FechaResolucion!.Value - i.FechaReporte).TotalHours);
                        
                        // SLA
                        int dentroSLA = 0;
                        int totalConSLA = 0;

                        foreach (var inc in incidentesResueltos)
                        {
                            var sla = slas.FirstOrDefault(s => s.Prioridad == inc.Prioridad && s.Impacto == inc.Impacto);
                            if (sla != null)
                            {
                                totalConSLA++;
                                var tiempo = (inc.FechaResolucion!.Value - inc.FechaReporte).TotalMinutes;
                                if (tiempo <= sla.TiempoResolucion)
                                    dentroSLA++;
                            }
                        }

                        est.PorcentajeCumplimientoSLA = totalConSLA > 0 ? (dentroSLA * 100.0) / totalConSLA : 0;
                    }

                    estadisticas.Add(est);
                }

                return new ApiResponse<List<EstadisticasPorTecnicoDto>>(true, 
                    estadisticas.OrderByDescending(e => e.IncidentesResueltos).ToList(), 
                    $"Estadísticas de {estadisticas.Count} técnicos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular estadísticas por técnico");
                return new ApiResponse<List<EstadisticasPorTecnicoDto>>(false, 
                    new List<EstadisticasPorTecnicoDto>(), $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TendenciasDto>> ObtenerTendenciasAsync(int ultimosDias = 30)
        {
            try
            {
                var fechaInicio = DateTime.UtcNow.AddDays(-ultimosDias);
                var incidentes = (await _incidenteRepository.GetAllAsync())
                    .Where(i => i.FechaReporte >= fechaInicio)
                    .ToList();

                var tendencias = new TendenciasDto();

                // Tendencias diarias
                for (int i = 0; i < ultimosDias; i++)
                {
                    var fecha = fechaInicio.AddDays(i).Date;
                    var incidentesDia = incidentes.Where(inc => inc.FechaReporte.Date == fecha).ToList();
                    var resueltosDia = incidentesDia.Where(inc => inc.FechaResolucion?.Date == fecha).ToList();

                    tendencias.TendenciasDiarias.Add(new TendenciaDiariaDto
                    {
                        Fecha = fecha,
                        IncidentesCreados = incidentesDia.Count,
                        IncidentesResueltos = resueltosDia.Count,
                        IncidentesCerrados = incidentesDia.Count(inc => inc.FechaCierre?.Date == fecha),
                        TiempoPromedioResolucion = resueltosDia.Any() ?
                            resueltosDia.Average(inc => (inc.FechaResolucion!.Value - inc.FechaReporte).TotalHours) : 0
                    });
                }

                // Tendencias mensuales (últimos 6 meses)
                var fechaInicioMensual = DateTime.UtcNow.AddMonths(-6);
                var incidentesMensuales = (await _incidenteRepository.GetAllAsync())
                    .Where(i => i.FechaReporte >= fechaInicioMensual)
                    .ToList();

                var mesesUnicos = incidentesMensuales
                    .Select(i => new { i.FechaReporte.Year, i.FechaReporte.Month })
                    .Distinct()
                    .OrderBy(m => m.Year).ThenBy(m => m.Month);

                foreach (var mes in mesesUnicos)
                {
                    var incidentesMes = incidentesMensuales
                        .Where(i => i.FechaReporte.Year == mes.Year && i.FechaReporte.Month == mes.Month)
                        .ToList();
                    
                    var resueltosMes = incidentesMes.Where(i => i.FechaResolucion.HasValue).ToList();

                    tendencias.TendenciasMensuales.Add(new TendenciaMensualDto
                    {
                        Año = mes.Year,
                        Mes = mes.Month,
                        MesNombre = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mes.Month),
                        TotalIncidentes = incidentesMes.Count,
                        IncidentesResueltos = resueltosMes.Count,
                        TiempoPromedioResolucion = resueltosMes.Any() ?
                            resueltosMes.Average(i => (i.FechaResolucion!.Value - i.FechaReporte).TotalHours) : 0
                    });
                }

                return new ApiResponse<TendenciasDto>(true, tendencias, "Tendencias calculadas correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular tendencias");
                return new ApiResponse<TendenciasDto>(false, new TendenciasDto(), $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ReporteDisponibilidadDto>> ObtenerReporteDisponibilidadAsync(
            DateTime? desde = null, DateTime? hasta = null)
        {
            try
            {
                desde ??= DateTime.UtcNow.AddDays(-30);
                hasta ??= DateTime.UtcNow;

                var servicios = await _servicioRepository.GetServiciosActivosAsync();
                var incidentes = (await _incidenteRepository.GetAllAsync())
                    .Where(i => i.FechaReporte >= desde && i.FechaReporte <= hasta && i.ServicioDITICId.HasValue)
                    .ToList();

                var reporte = new ReporteDisponibilidadDto();

                foreach (var servicio in servicios)
                {
                    var incidentesServicio = incidentes.Where(i => i.ServicioDITICId == servicio.Id).ToList();
                    var incidentesCriticos = incidentesServicio.Count(i => i.Prioridad == PrioridadIncidente.Critica);

                    // Calcular tiempo de inactividad (suma de tiempo de resolución de incidentes críticos)
                    var tiempoInactividad = incidentesServicio
                        .Where(i => i.Prioridad == PrioridadIncidente.Critica && i.FechaResolucion.HasValue)
                        .Sum(i => (i.FechaResolucion!.Value - i.FechaReporte).TotalHours);

                    var horasTotales = (hasta!.Value - desde!.Value).TotalHours;
                    var disponibilidadReal = horasTotales > 0 ? 
                        ((horasTotales - tiempoInactividad) * 100.0) / horasTotales : 100.0;

                    reporte.Servicios.Add(new DisponibilidadServicioDto
                    {
                        ServicioId = servicio.Id,
                        ServicioNombre = servicio.Nombre,
                        Codigo = servicio.Codigo,
                        DisponibilidadObjetivo = servicio.PorcentajeDisponibilidad,
                        DisponibilidadReal = Math.Round(disponibilidadReal, 2),
                        TotalIncidentes = incidentesServicio.Count,
                        IncidentesCriticos = incidentesCriticos,
                        TiempoInactividad = Math.Round(tiempoInactividad, 2),
                        CumpleObjetivo = disponibilidadReal >= servicio.PorcentajeDisponibilidad
                    });
                }

                reporte.DisponibilidadPromedio = reporte.Servicios.Any() ?
                    reporte.Servicios.Average(s => s.DisponibilidadReal) : 0;

                return new ApiResponse<ReporteDisponibilidadDto>(true, reporte, 
                    $"Reporte de {reporte.Servicios.Count} servicios");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de disponibilidad");
                return new ApiResponse<ReporteDisponibilidadDto>(false, new ReporteDisponibilidadDto(), 
                    $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TopIncidentesDto>> ObtenerTopIncidentesFrecuentesAsync(
            int top = 10, DateTime? desde = null, DateTime? hasta = null)
        {
            try
            {
                desde ??= DateTime.UtcNow.AddDays(-90); // Últimos 3 meses por defecto
                hasta ??= DateTime.UtcNow;

                var incidentes = (await _incidenteRepository.GetAllAsync())
                    .Where(i => i.FechaReporte >= desde && i.FechaReporte <= hasta)
                    .ToList();

                var categorias = await _categoriaRepository.GetAllAsync();

                // Agrupar por título similar (normalizado)
                var incidentesAgrupados = incidentes
                    .GroupBy(i => new { 
                        TituloNormalizado = NormalizarTitulo(i.Titulo),
                        i.CategoriaId 
                    })
                    .Select(g => new
                    {
                        Titulo = g.First().Titulo,
                        CategoriaId = g.Key.CategoriaId,
                        Ocurrencias = g.Count(),
                        TiempoPromedio = g.Where(i => i.FechaResolucion.HasValue)
                            .DefaultIfEmpty()
                            .Average(i => i != null && i.FechaResolucion.HasValue ? 
                                (i.FechaResolucion.Value - i.FechaReporte).TotalHours : 0),
                        ArticuloId = g.FirstOrDefault(i => i.ArticuloConocimientoId.HasValue)?.ArticuloConocimientoId
                    })
                    .OrderByDescending(g => g.Ocurrencias)
                    .Take(top)
                    .ToList();

                var resultado = new TopIncidentesDto();

                foreach (var grupo in incidentesAgrupados)
                {
                    var categoria = categorias.FirstOrDefault(c => c.Id == grupo.CategoriaId);
                    
                    resultado.IncidentesFrecuentes.Add(new IncidenteFrecuenteDto
                    {
                        Titulo = grupo.Titulo,
                        Categoria = categoria?.Nombre ?? "Sin categoría",
                        Ocurrencias = grupo.Ocurrencias,
                        TiempoPromedioResolucion = Math.Round(grupo.TiempoPromedio, 2),
                        ArticuloConocimientoId = grupo.ArticuloId,
                        TieneSolucion = grupo.ArticuloId.HasValue
                    });
                }

                return new ApiResponse<TopIncidentesDto>(true, resultado, 
                    $"Top {resultado.IncidentesFrecuentes.Count} incidentes frecuentes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener top incidentes frecuentes");
                return new ApiResponse<TopIncidentesDto>(false, new TopIncidentesDto(), $"Error: {ex.Message}");
            }
        }

        private string NormalizarTitulo(string titulo)
        {
            // Normalizar título para agrupar similares
            return titulo.ToLowerInvariant()
                .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                .Trim();
        }
    }
}
