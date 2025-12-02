using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace IncidentesFISEI.Application.Services
{
    public class EscalacionService : IEscalacionService
    {
        private readonly IIncidenteRepository _incidenteRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<EscalacionService> _logger;
        private readonly IRepository<HistorialEscalacion> _historialRepository;

        // Tiempos límite por nivel (en minutos) antes de escalar automáticamente
        private readonly Dictionary<NivelSoporte, int> _tiemposLimiteEscalacion = new()
        {
            { NivelSoporte.L1_Tecnico, 120 },      // 2 horas en L1
            { NivelSoporte.L2_Experto, 240 },      // 4 horas en L2
            { NivelSoporte.L3_Especialista, 480 }, // 8 horas en L3
            { NivelSoporte.L4_Proveedor, 1440 }    // 24 horas en L4 (no se escala más allá)
        };

        public EscalacionService(
            IIncidenteRepository incidenteRepository,
            IUsuarioRepository usuarioRepository,
            ILogger<EscalacionService> logger,
            IRepository<HistorialEscalacion> historialRepository)
        {
            _incidenteRepository = incidenteRepository;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
            _historialRepository = historialRepository;
        }

        public async Task<ApiResponse<bool>> EscalarIncidenteAsync(int incidenteId, string razon, int? tecnicoDestinoId = null)
        {
            try
            {
                var incidente = await _incidenteRepository.GetByIdAsync(incidenteId);
                if (incidente == null)
                    return new ApiResponse<bool>(false, false, "Incidente no encontrado");

                if (incidente.Estado == EstadoIncidente.Resuelto || incidente.Estado == EstadoIncidente.Cerrado)
                    return new ApiResponse<bool>(false, false, "No se puede escalar un incidente resuelto o cerrado");

                // Determinar siguiente nivel
                var nivelActual = incidente.NivelActual;
                var siguienteNivel = ObtenerSiguienteNivel(nivelActual);

                if (siguienteNivel == null)
                    return new ApiResponse<bool>(false, false, "El incidente ya está en el nivel máximo de escalación");

                return await EscalarANivelEspecificoAsync(incidenteId, siguienteNivel.Value, razon, tecnicoDestinoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al escalar incidente {IncidenteId}", incidenteId);
                return new ApiResponse<bool>(false, false, $"Error al escalar incidente: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> EscalarANivelEspecificoAsync(
            int incidenteId, 
            NivelSoporte nivelDestino, 
            string razon, 
            int? tecnicoDestinoId = null)
        {
            try
            {
                var incidente = await _incidenteRepository.GetByIdAsync(incidenteId);
                if (incidente == null)
                    return new ApiResponse<bool>(false, false, "Incidente no encontrado");

                var nivelOrigen = incidente.NivelActual;

                // Validar que sea una escalación hacia arriba
                if (nivelDestino <= nivelOrigen)
                    return new ApiResponse<bool>(false, false, "Solo se puede escalar a niveles superiores");

                Usuario? tecnicoDestino = null;
                
                // Si se especificó un técnico, validar que pertenezca al nivel destino
                if (tecnicoDestinoId.HasValue)
                {
                    tecnicoDestino = await _usuarioRepository.GetByIdAsync(tecnicoDestinoId.Value);
                    if (tecnicoDestino == null || tecnicoDestino.NivelSoporte != nivelDestino)
                    {
                        return new ApiResponse<bool>(false, false, 
                            $"El técnico especificado no pertenece al nivel {nivelDestino}");
                    }
                }
                else
                {
                    // Auto-asignar al técnico con menor carga del nivel destino
                    var tecnicoResponse = await ObtenerTecnicoDisponiblePorNivelAsync(nivelDestino);
                    if (tecnicoResponse.Success && tecnicoResponse.Data != null)
                    {
                        tecnicoDestino = await _usuarioRepository.GetByIdAsync(tecnicoResponse.Data.Id);
                    }
                }

                // Registrar historial de escalación
                var historial = new HistorialEscalacion
                {
                    IncidenteId = incidenteId,
                    NivelOrigen = nivelOrigen,
                    NivelDestino = nivelDestino,
                    TecnicoOrigenId = incidente.AsignadoAId,
                    TecnicoDestinoId = tecnicoDestino?.Id,
                    Razon = razon,
                    FueAutomatico = false,
                    FechaEscalacion = DateTime.UtcNow
                };

                await _historialRepository.AddAsync(historial);

                // Actualizar incidente
                incidente.NivelActual = nivelDestino;
                incidente.NumeroEscalaciones++;
                incidente.FechaUltimaEscalacion = DateTime.UtcNow;
                incidente.RazonEscalacion = razon;
                incidente.EscaladoAutomaticamente = false;

                // Reasignar técnico si se encontró uno
                if (tecnicoDestino != null)
                {
                    // Reducir carga del técnico anterior
                    if (incidente.AsignadoAId.HasValue)
                    {
                        var tecnicoAnterior = await _usuarioRepository.GetByIdAsync(incidente.AsignadoAId.Value);
                        if (tecnicoAnterior != null && tecnicoAnterior.CargaTrabajoActual > 0)
                        {
                            tecnicoAnterior.CargaTrabajoActual--;
                            await _usuarioRepository.UpdateAsync(tecnicoAnterior);
                        }
                    }

                    // Asignar al nuevo técnico
                    incidente.AsignadoAId = tecnicoDestino.Id;
                    incidente.FechaAsignacion = DateTime.UtcNow;
                    
                    tecnicoDestino.CargaTrabajoActual++;
                    await _usuarioRepository.UpdateAsync(tecnicoDestino);
                }

                // Cambiar estado a EnProgreso si estaba en otro estado
                if (incidente.Estado == EstadoIncidente.Abierto || incidente.Estado == EstadoIncidente.EnEspera)
                {
                    incidente.Estado = EstadoIncidente.EnProgreso;
                }

                await _incidenteRepository.UpdateAsync(incidente);

                _logger.LogInformation(
                    "Incidente {NumeroIncidente} escalado de {NivelOrigen} a {NivelDestino}. Razón: {Razon}",
                    incidente.NumeroIncidente, nivelOrigen, nivelDestino, razon);

                return new ApiResponse<bool>(true, true, 
                    $"Incidente escalado exitosamente de {nivelOrigen} a {nivelDestino}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al escalar incidente {IncidenteId} a nivel {Nivel}", 
                    incidenteId, nivelDestino);
                return new ApiResponse<bool>(false, false, $"Error al escalar incidente: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> VerificarEscalacionAutomaticaAsync(int incidenteId)
        {
            try
            {
                var incidente = await _incidenteRepository.GetByIdAsync(incidenteId);
                if (incidente == null)
                    return new ApiResponse<bool>(false, false, "Incidente no encontrado");

                if (incidente.Estado == EstadoIncidente.Resuelto || incidente.Estado == EstadoIncidente.Cerrado)
                    return new ApiResponse<bool>(false, false, "El incidente ya está cerrado");

                // Verificar tiempo en el nivel actual
                var fechaReferencia = incidente.FechaUltimaEscalacion ?? incidente.FechaAsignacion ?? incidente.FechaReporte;
                var minutosEnNivelActual = (DateTime.UtcNow - fechaReferencia).TotalMinutes;

                var limiteEscalacion = _tiemposLimiteEscalacion[incidente.NivelActual];

                if (minutosEnNivelActual >= limiteEscalacion)
                {
                    // Escalar automáticamente
                    var siguienteNivel = ObtenerSiguienteNivel(incidente.NivelActual);
                    if (siguienteNivel.HasValue)
                    {
                        var razon = $"Escalación automática: Se superó el tiempo límite de {limiteEscalacion} minutos en nivel {incidente.NivelActual}";
                        
                        var resultadoEscalacion = await EscalarANivelEspecificoAsync(
                            incidenteId, 
                            siguienteNivel.Value, 
                            razon);

                        if (resultadoEscalacion.Success)
                        {
                            // Marcar como escalación automática
                            incidente.EscaladoAutomaticamente = true;
                            await _incidenteRepository.UpdateAsync(incidente);

                            _logger.LogWarning(
                                "Incidente {NumeroIncidente} escalado AUTOMÁTICAMENTE de {NivelOrigen} a {NivelDestino} por timeout",
                                incidente.NumeroIncidente, incidente.NivelActual, siguienteNivel.Value);

                            return new ApiResponse<bool>(true, true, razon);
                        }
                    }
                }

                return new ApiResponse<bool>(true, false, 
                    $"No requiere escalación. Tiempo en nivel actual: {minutosEnNivelActual:F0}/{limiteEscalacion} minutos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar escalación automática para incidente {IncidenteId}", incidenteId);
                return new ApiResponse<bool>(false, false, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UsuarioDto?>> ObtenerTecnicoDisponiblePorNivelAsync(NivelSoporte nivel)
        {
            try
            {
                var tecnicos = await _usuarioRepository.GetAllAsync();
                
                var tecnicoDisponible = tecnicos
                    .Where(t => t.IsActive && 
                                t.NivelSoporte == nivel &&
                                (t.TipoUsuario == TipoUsuario.Tecnico || t.TipoUsuario == TipoUsuario.Supervisor))
                    .OrderBy(t => t.CargaTrabajoActual)
                    .ThenByDescending(t => t.AñosExperiencia ?? 0)
                    .FirstOrDefault();

                if (tecnicoDisponible == null)
                {
                    return new ApiResponse<UsuarioDto?>(false, null, 
                        $"No hay técnicos disponibles en el nivel {nivel}");
                }

                var dto = new UsuarioDto
                {
                    Id = tecnicoDisponible.Id,
                    NombreCompleto = $"{tecnicoDisponible.FirstName} {tecnicoDisponible.LastName}",
                    Email = tecnicoDisponible.Email,
                    Rol = tecnicoDisponible.TipoUsuario.ToString(),
                    Activo = tecnicoDisponible.IsActive,
                    FechaCreacion = tecnicoDisponible.CreatedAt,
                    FechaUltimoAcceso = tecnicoDisponible.LastLoginAt
                };

                return new ApiResponse<UsuarioDto?>(true, dto, 
                    $"Técnico encontrado con {tecnicoDisponible.CargaTrabajoActual} tickets activos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar técnico disponible para nivel {Nivel}", nivel);
                return new ApiResponse<UsuarioDto?>(false, null, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<EstadisticasEscalacionDto>> ObtenerEstadisticasEscalacionAsync()
        {
            try
            {
                var incidentes = await _incidenteRepository.GetAllAsync();
                var historial = await _historialRepository.GetAllAsync();

                var stats = new EstadisticasEscalacionDto
                {
                    TotalEscalaciones = historial.Count(),
                    EscalacionesAutomaticas = historial.Count(h => h.FueAutomatico),
                    EscalacionesManuales = historial.Count(h => !h.FueAutomatico),
                    IncidentesEnL1 = incidentes.Count(i => i.NivelActual == NivelSoporte.L1_Tecnico && 
                                                            i.Estado != EstadoIncidente.Cerrado),
                    IncidentesEnL2 = incidentes.Count(i => i.NivelActual == NivelSoporte.L2_Experto && 
                                                            i.Estado != EstadoIncidente.Cerrado),
                    IncidentesEnL3 = incidentes.Count(i => i.NivelActual == NivelSoporte.L3_Especialista && 
                                                            i.Estado != EstadoIncidente.Cerrado),
                    IncidentesEnL4 = incidentes.Count(i => i.NivelActual == NivelSoporte.L4_Proveedor && 
                                                            i.Estado != EstadoIncidente.Cerrado),
                };

                // Calcular tiempo promedio hasta primera escalación
                var incidentesConEscalacion = incidentes.Where(i => i.NumeroEscalaciones > 0).ToList();
                if (incidentesConEscalacion.Any())
                {
                    var tiempos = incidentesConEscalacion
                        .Where(i => i.FechaUltimaEscalacion.HasValue)
                        .Select(i => (i.FechaUltimaEscalacion!.Value - i.FechaReporte).TotalHours);
                    
                    stats.TiempoPromedioHastaEscalacion = tiempos.Any() ? tiempos.Average() : 0;
                }

                // Calcular porcentajes de resolución en L1
                var totalIncidentes = incidentes.Count();
                if (totalIncidentes > 0)
                {
                    var resueltosEnL1 = incidentes.Count(i => i.NivelActual == NivelSoporte.L1_Tecnico && 
                                                               i.Estado == EstadoIncidente.Resuelto);
                    stats.PorcentajeResueltoEnL1 = (resueltosEnL1 * 100.0) / totalIncidentes;

                    var escaladosDeL1 = incidentes.Count(i => i.NumeroEscalaciones > 0);
                    stats.PorcentajeEscaladoDeL1 = (escaladosDeL1 * 100.0) / totalIncidentes;
                }

                return new ApiResponse<EstadisticasEscalacionDto>(true, stats, "Estadísticas obtenidas correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de escalación");
                return new ApiResponse<EstadisticasEscalacionDto>(false, new EstadisticasEscalacionDto(), 
                    $"Error: {ex.Message}");
            }
        }

        public async Task<int> ProcesarEscalacionesAutomaticasAsync()
        {
            try
            {
                var incidentes = await _incidenteRepository.GetAllAsync();
                
                var incidentesActivos = incidentes
                    .Where(i => i.Estado != EstadoIncidente.Resuelto && 
                                i.Estado != EstadoIncidente.Cerrado &&
                                i.NivelActual != NivelSoporte.L4_Proveedor) // L4 es el máximo
                    .ToList();

                int escalacionesRealizadas = 0;

                foreach (var incidente in incidentesActivos)
                {
                    var resultado = await VerificarEscalacionAutomaticaAsync(incidente.Id);
                    if (resultado.Success && resultado.Data)
                    {
                        escalacionesRealizadas++;
                    }
                }

                _logger.LogInformation(
                    "Proceso de escalación automática completado. {Cantidad} incidentes escalados de {Total} revisados",
                    escalacionesRealizadas, incidentesActivos.Count);

                return escalacionesRealizadas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar escalaciones automáticas");
                return 0;
            }
        }

        public async Task<ApiResponse<List<HistorialEscalacionDto>>> ObtenerHistorialEscalacionesAsync(int incidenteId)
        {
            try
            {
                var historial = await _historialRepository.GetAllAsync();
                var historialIncidente = historial
                    .Where(h => h.IncidenteId == incidenteId)
                    .OrderBy(h => h.FechaEscalacion)
                    .ToList();

                var resultado = new List<HistorialEscalacionDto>();

                foreach (var item in historialIncidente)
                {
                    var incidente = await _incidenteRepository.GetByIdAsync(item.IncidenteId);
                    
                    Usuario? tecnicoOrigen = null;
                    Usuario? tecnicoDestino = null;

                    if (item.TecnicoOrigenId.HasValue)
                        tecnicoOrigen = await _usuarioRepository.GetByIdAsync(item.TecnicoOrigenId.Value);
                    
                    if (item.TecnicoDestinoId.HasValue)
                        tecnicoDestino = await _usuarioRepository.GetByIdAsync(item.TecnicoDestinoId.Value);

                    resultado.Add(new HistorialEscalacionDto
                    {
                        Id = item.Id,
                        IncidenteId = item.IncidenteId,
                        NumeroIncidente = incidente?.NumeroIncidente ?? "N/A",
                        NivelOrigen = item.NivelOrigen.ToString(),
                        NivelDestino = item.NivelDestino.ToString(),
                        TecnicoOrigenNombre = tecnicoOrigen != null 
                            ? $"{tecnicoOrigen.FirstName} {tecnicoOrigen.LastName}" 
                            : null,
                        TecnicoDestinoNombre = tecnicoDestino != null 
                            ? $"{tecnicoDestino.FirstName} {tecnicoDestino.LastName}" 
                            : null,
                        Razon = item.Razon,
                        FueAutomatico = item.FueAutomatico,
                        FechaEscalacion = item.FechaEscalacion
                    });
                }

                return new ApiResponse<List<HistorialEscalacionDto>>(true, resultado, 
                    $"Se encontraron {resultado.Count} escalaciones");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de escalaciones para incidente {IncidenteId}", incidenteId);
                return new ApiResponse<List<HistorialEscalacionDto>>(false, new List<HistorialEscalacionDto>(), 
                    $"Error: {ex.Message}");
            }
        }

        // Métodos auxiliares privados
        private NivelSoporte? ObtenerSiguienteNivel(NivelSoporte nivelActual)
        {
            return nivelActual switch
            {
                NivelSoporte.L1_Tecnico => NivelSoporte.L2_Experto,
                NivelSoporte.L2_Experto => NivelSoporte.L3_Especialista,
                NivelSoporte.L3_Especialista => NivelSoporte.L4_Proveedor,
                NivelSoporte.L4_Proveedor => null, // Ya es el máximo nivel
                _ => null
            };
        }
    }
}
