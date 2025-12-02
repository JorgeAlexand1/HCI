using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Interfaces;
using IncidentesFISEI.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IncidentesFISEI.Application.Services;

public class AsignacionService : IAsignacionService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IIncidenteRepository _incidenteRepository;
    private readonly ICategoriaIncidenteRepository _categoriaRepository;
    private readonly ILogger<AsignacionService> _logger;

    public AsignacionService(
        IUsuarioRepository usuarioRepository,
        IIncidenteRepository incidenteRepository,
        ICategoriaIncidenteRepository categoriaRepository,
        ILogger<AsignacionService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _incidenteRepository = incidenteRepository;
        _categoriaRepository = categoriaRepository;
        _logger = logger;
    }

    public async Task<Usuario?> GetSPOCDisponibleAsync()
    {
        try
        {
            var spocs = await _usuarioRepository.FindAsync(u =>
                u.IsSPOC &&
                u.IsAvailable &&
                u.IsActive &&
                (u.TipoUsuario == TipoUsuario.Supervisor || u.TipoUsuario == TipoUsuario.Administrador)
            );

            return spocs.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener SPOC disponible");
            return null;
        }
    }

    public async Task<Usuario?> GetTecnicoConMenorCargaAsync(string? especialidad = null)
    {
        try
        {
            var tecnicos = await _usuarioRepository.GetTecnicosAsync();

            if (!string.IsNullOrEmpty(especialidad))
            {
                tecnicos = tecnicos.Where(t =>
                    !string.IsNullOrEmpty(t.Especialidad) &&
                    t.Especialidad.Contains(especialidad, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var tecnicoDisponible = tecnicos
                .Where(t => t.IsActive)
                .OrderBy(t => t.CargaTrabajoActual)
                .ThenByDescending(t => t.AñosExperiencia ?? 0)
                .FirstOrDefault();

            return tecnicoDisponible;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener técnico con menor carga");
            return null;
        }
    }

    public async Task<ApiResponse<bool>> AsignarIncidenteAutomaticamenteAsync(int incidenteId)
    {
        try
        {
            var incidente = await _incidenteRepository.GetByIdAsync(incidenteId);
            if (incidente == null)
            {
                return new ApiResponse<bool>(false, false, "Incidente no encontrado");
            }

            if (incidente.AsignadoAId != null)
            {
                return new ApiResponse<bool>(false, false, "El incidente ya está asignado");
            }

            // 1. Verificar si hay SPOC disponible
            var spoc = await GetSPOCDisponibleAsync();

            if (spoc != null && spoc.IsAvailable)
            {
                // Si hay SPOC disponible, NO asignar automáticamente
                // El SPOC debe asignar manualmente
                _logger.LogInformation(
                    "SPOC {SPOCId} disponible, esperando asignación manual para incidente {IncidenteId}",
                    spoc.Id, incidenteId
                );
                return new ApiResponse<bool>(
                    false,
                    false,
                    "Hay un SPOC disponible. La asignación debe ser manual."
                );
            }

            // 2. Si no hay SPOC disponible, asignar al técnico con menor carga
            var categoria = await _categoriaRepository.GetByIdAsync(incidente.CategoriaId);
            var especialidadRequerida = categoria?.Nombre;

            var tecnico = await GetTecnicoConMenorCargaAsync(especialidadRequerida);

            if (tecnico == null)
            {
                return new ApiResponse<bool>(
                    false,
                    false,
                    "No hay técnicos disponibles para asignación"
                );
            }

            // 3. Asignar incidente
            incidente.AsignadoAId = tecnico.Id;
            incidente.FechaAsignacion = DateTime.UtcNow;
            incidente.Estado = EstadoIncidente.EnProgreso;

            // 4. Incrementar carga de trabajo
            tecnico.CargaTrabajoActual++;

            await _incidenteRepository.UpdateAsync(incidente);
            await _usuarioRepository.UpdateAsync(tecnico);
            await _incidenteRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Incidente {NumeroIncidente} asignado automáticamente a {TecnicoNombre} (ID: {TecnicoId})",
                incidente.NumeroIncidente,
                $"{tecnico.FirstName} {tecnico.LastName}",
                tecnico.Id
            );

            return new ApiResponse<bool>(
                true,
                true,
                $"Incidente asignado a {tecnico.FirstName} {tecnico.LastName}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en asignación automática de incidente {IncidenteId}", incidenteId);
            return new ApiResponse<bool>(false, false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<bool>> AsignarIncidenteManualmenteAsync(
        int incidenteId,
        int tecnicoId,
        int asignadoPorId)
    {
        try
        {
            // 1. Verificar permisos del usuario que asigna
            var asignador = await _usuarioRepository.GetByIdAsync(asignadoPorId);
            if (asignador == null)
            {
                return new ApiResponse<bool>(false, false, "Usuario no encontrado");
            }

            // Solo SPOC, Supervisor o Admin pueden asignar manualmente
            if (!asignador.IsSPOC &&
                asignador.TipoUsuario != TipoUsuario.Supervisor &&
                asignador.TipoUsuario != TipoUsuario.Administrador)
            {
                return new ApiResponse<bool>(
                    false,
                    false,
                    "No tiene permisos para asignar incidentes"
                );
            }

            // 2. Obtener incidente
            var incidente = await _incidenteRepository.GetByIdAsync(incidenteId);
            if (incidente == null)
            {
                return new ApiResponse<bool>(false, false, "Incidente no encontrado");
            }

            // 3. Obtener técnico
            var tecnico = await _usuarioRepository.GetByIdAsync(tecnicoId);
            if (tecnico == null)
            {
                return new ApiResponse<bool>(false, false, "Técnico no encontrado");
            }

            if (!tecnico.IsActive)
            {
                return new ApiResponse<bool>(false, false, "El técnico está inactivo");
            }

            // 4. Si ya estaba asignado, decrementar carga del técnico anterior
            if (incidente.AsignadoAId.HasValue)
            {
                var tecnicoAnterior = await _usuarioRepository.GetByIdAsync(incidente.AsignadoAId.Value);
                if (tecnicoAnterior != null && tecnicoAnterior.CargaTrabajoActual > 0)
                {
                    tecnicoAnterior.CargaTrabajoActual--;
                    await _usuarioRepository.UpdateAsync(tecnicoAnterior);
                }
            }

            // 5. Asignar al nuevo técnico
            incidente.AsignadoAId = tecnicoId;
            incidente.FechaAsignacion = DateTime.UtcNow;
            incidente.Estado = EstadoIncidente.EnProgreso;

            // 6. Incrementar carga del nuevo técnico
            tecnico.CargaTrabajoActual++;

            await _incidenteRepository.UpdateAsync(incidente);
            await _usuarioRepository.UpdateAsync(tecnico);
            await _incidenteRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Incidente {NumeroIncidente} asignado manualmente por {Asignador} a {Tecnico}",
                incidente.NumeroIncidente,
                $"{asignador.FirstName} {asignador.LastName}",
                $"{tecnico.FirstName} {tecnico.LastName}"
            );

            return new ApiResponse<bool>(
                true,
                true,
                $"Incidente asignado exitosamente a {tecnico.FirstName} {tecnico.LastName}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en asignación manual");
            return new ApiResponse<bool>(false, false, "Error interno del servidor");
        }
    }

    public async Task<Dictionary<int, int>> GetCargaTrabajoTecnicosAsync()
    {
        try
        {
            var tecnicos = await _usuarioRepository.GetTecnicosAsync();

            return tecnicos.ToDictionary(
                t => t.Id,
                t => t.CargaTrabajoActual
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener carga de trabajo de técnicos");
            return new Dictionary<int, int>();
        }
    }

    public async Task<ApiResponse<bool>> SetSPOCAsync(int usuarioId, bool isSPOC)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
            {
                return new ApiResponse<bool>(false, false, "Usuario no encontrado");
            }

            // Solo supervisores o admins pueden ser SPOC
            if (isSPOC && usuario.TipoUsuario != TipoUsuario.Supervisor &&
                usuario.TipoUsuario != TipoUsuario.Administrador)
            {
                return new ApiResponse<bool>(
                    false,
                    false,
                    "Solo supervisores o administradores pueden ser SPOC"
                );
            }

            usuario.IsSPOC = isSPOC;
            if (isSPOC)
            {
                usuario.IsAvailable = true; // Por defecto disponible al asignar SPOC
            }

            await _usuarioRepository.UpdateAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Usuario {UserId} configurado como SPOC: {IsSPOC}",
                usuarioId,
                isSPOC
            );

            return new ApiResponse<bool>(
                true,
                true,
                isSPOC ? "Usuario configurado como SPOC" : "Rol SPOC removido"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al configurar SPOC");
            return new ApiResponse<bool>(false, false, "Error interno del servidor");
        }
    }

    public async Task<ApiResponse<bool>> SetSPOCAvailabilityAsync(int usuarioId, bool isAvailable)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
            {
                return new ApiResponse<bool>(false, false, "Usuario no encontrado");
            }

            if (!usuario.IsSPOC)
            {
                return new ApiResponse<bool>(false, false, "El usuario no es SPOC");
            }

            usuario.IsAvailable = isAvailable;

            await _usuarioRepository.UpdateAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Disponibilidad SPOC {UserId} actualizada: {IsAvailable}",
                usuarioId,
                isAvailable
            );

            return new ApiResponse<bool>(
                true,
                true,
                isAvailable ? "SPOC marcado como disponible" : "SPOC marcado como no disponible"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar disponibilidad SPOC");
            return new ApiResponse<bool>(false, false, "Error interno del servidor");
        }
    }

    public async Task ActualizarCargaTrabajoAsync(int tecnicoId, int incremento)
    {
        try
        {
            var tecnico = await _usuarioRepository.GetByIdAsync(tecnicoId);
            if (tecnico != null)
            {
                tecnico.CargaTrabajoActual = Math.Max(0, tecnico.CargaTrabajoActual + incremento);
                await _usuarioRepository.UpdateAsync(tecnico);
                await _usuarioRepository.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar carga de trabajo");
        }
    }
}
