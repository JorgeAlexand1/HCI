using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace IncidentesFISEI.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(
        IAuditLogRepository auditLogRepository,
        ILogger<AuditLogService> logger)
    {
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    #region Crear Logs

    public async Task RegistrarAuditoriaAsync(CreateAuditLogDto dto)
    {
        try
        {
            var auditLog = new AuditLog
            {
                UsuarioId = dto.UsuarioId,
                UsuarioNombre = dto.UsuarioNombre,
                DireccionIP = dto.DireccionIP,
                UserAgent = dto.UserAgent,
                TipoAccion = dto.TipoAccion,
                TipoEntidad = dto.TipoEntidad,
                EntidadId = dto.EntidadId,
                EntidadDescripcion = dto.EntidadDescripcion,
                Descripcion = dto.Descripcion,
                ValoresAnteriores = dto.ValoresAnteriores,
                ValoresNuevos = dto.ValoresNuevos,
                NivelSeveridad = dto.NivelSeveridad,
                MetadataJson = dto.MetadataJson,
                EsExitoso = dto.EsExitoso,
                MensajeError = dto.MensajeError,
                CantidadRegistros = dto.CantidadRegistros,
                FiltrosAplicados = dto.FiltrosAplicados,
                FechaHora = DateTime.UtcNow,
                Modulo = dto.Modulo,
                Endpoint = dto.Endpoint
            };

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            // No queremos que un error de auditoría detenga la aplicación
            _logger.LogError(ex, "Error al registrar auditoría: {Descripcion}", dto.Descripcion);
        }
    }

    public async Task RegistrarAuditoriaAsync(
        int? usuarioId,
        TipoAccionAuditoria tipoAccion,
        TipoEntidadAuditoria tipoEntidad,
        int? entidadId,
        string descripcion,
        string? valoresAnteriores = null,
        string? valoresNuevos = null,
        NivelSeveridadAuditoria nivelSeveridad = NivelSeveridadAuditoria.Informativo,
        bool esExitoso = true,
        string? mensajeError = null)
    {
        var dto = new CreateAuditLogDto
        {
            UsuarioId = usuarioId,
            TipoAccion = tipoAccion,
            TipoEntidad = tipoEntidad,
            EntidadId = entidadId,
            Descripcion = descripcion,
            ValoresAnteriores = valoresAnteriores,
            ValoresNuevos = valoresNuevos,
            NivelSeveridad = nivelSeveridad,
            EsExitoso = esExitoso,
            MensajeError = mensajeError
        };

        await RegistrarAuditoriaAsync(dto);
    }

    #endregion

    #region Consultas

    public async Task<IEnumerable<AuditLogDto>> GetLogsByUsuarioAsync(int usuarioId, int skip = 0, int take = 50)
    {
        var logs = await _auditLogRepository.GetLogsByUsuarioAsync(usuarioId, skip, take);
        return logs.Select(MapToDto);
    }

    public async Task<IEnumerable<AuditLogDto>> GetLogsByEntidadAsync(TipoEntidadAuditoria tipoEntidad, int entidadId)
    {
        var logs = await _auditLogRepository.GetLogsByEntidadAsync(tipoEntidad, entidadId);
        return logs.Select(MapToDto);
    }

    public async Task<AuditLogDetalladoDto?> GetLogDetalladoAsync(int id)
    {
        var log = await _auditLogRepository.GetByIdAsync(id);
        if (log == null)
            return null;

        return new AuditLogDetalladoDto
        {
            Id = log.Id,
            UsuarioId = log.UsuarioId,
            UsuarioNombre = log.UsuarioNombre,
            DireccionIP = log.DireccionIP,
            UserAgent = log.UserAgent,
            TipoAccion = log.TipoAccion,
            TipoAccionDescripcion = log.TipoAccion.ToString(),
            TipoEntidad = log.TipoEntidad,
            TipoEntidadDescripcion = log.TipoEntidad.ToString(),
            EntidadId = log.EntidadId,
            EntidadDescripcion = log.EntidadDescripcion,
            Descripcion = log.Descripcion,
            ValoresAnteriores = log.ValoresAnteriores,
            ValoresNuevos = log.ValoresNuevos,
            NivelSeveridad = log.NivelSeveridad,
            NivelSeveridadDescripcion = log.NivelSeveridad.ToString(),
            MetadataJson = log.MetadataJson,
            EsExitoso = log.EsExitoso,
            MensajeError = log.MensajeError,
            CantidadRegistros = log.CantidadRegistros,
            FiltrosAplicados = log.FiltrosAplicados,
            FechaHora = log.FechaHora,
            Modulo = log.Modulo,
            Endpoint = log.Endpoint
        };
    }

    public async Task<IEnumerable<AuditLogDto>> BuscarLogsAsync(BuscarAuditLogsDto filtros)
    {
        var logs = await _auditLogRepository.BuscarLogsAsync(
            filtros.UsuarioId,
            filtros.TipoAccion,
            filtros.TipoEntidad,
            filtros.NivelSeveridad,
            filtros.Desde,
            filtros.Hasta,
            filtros.SoloErrores,
            filtros.Skip,
            filtros.Take
        );

        return logs.Select(MapToDto);
    }

    public async Task<IEnumerable<AuditLogDto>> GetLogsCriticosAsync(DateTime? desde = null, int take = 50)
    {
        var logs = await _auditLogRepository.GetLogsCriticosAsync(desde, 0, take);
        return logs.Select(MapToDto);
    }

    #endregion

    #region Estadísticas

    public async Task<EstadisticasAuditoriaDto> GetEstadisticasAsync(DateTime desde, DateTime hasta)
    {
        var logsEnRango = await _auditLogRepository.GetLogsByFechaAsync(desde, hasta, 0, int.MaxValue);
        var logsList = logsEnRango.ToList();

        var estadisticas = new EstadisticasAuditoriaDto
        {
            TotalRegistros = logsList.Count,
            TotalExitosos = logsList.Count(l => l.EsExitoso),
            TotalErrores = logsList.Count(l => !l.EsExitoso)
        };

        if (estadisticas.TotalRegistros > 0)
        {
            estadisticas.TasaExito = (double)estadisticas.TotalExitosos / estadisticas.TotalRegistros * 100;
        }

        // Estadísticas por tipo de acción
        var accionesPorTipo = await _auditLogRepository.GetEstadisticasPorTipoAccionAsync(desde, hasta);
        estadisticas.AccionesPorTipo = accionesPorTipo.ToDictionary(
            kvp => kvp.Key.ToString(),
            kvp => kvp.Value
        );

        // Actividad por usuario
        estadisticas.ActividadPorUsuario = await _auditLogRepository.GetEstadisticasPorUsuarioAsync(desde, hasta, 10);

        // Entidades por tipo
        estadisticas.EntidadesPorTipo = logsList
            .GroupBy(l => l.TipoEntidad)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        // Logs críticos recientes
        var logsCriticos = await _auditLogRepository.GetLogsCriticosAsync(desde, 0, 10);
        estadisticas.LogsCriticos = logsCriticos.Select(MapToDto).ToList();

        // Últimos errores
        var ultimosErrores = logsList
            .Where(l => !l.EsExitoso)
            .OrderByDescending(l => l.FechaHora)
            .Take(10);
        estadisticas.UltimosErrores = ultimosErrores.Select(MapToDto).ToList();

        return estadisticas;
    }

    #endregion

    #region Mantenimiento

    public async Task<int> LimpiarLogsAntiguosAsync(int diasRetencion = 90)
    {
        var fechaLimite = DateTime.UtcNow.AddDays(-diasRetencion);
        var eliminados = await _auditLogRepository.EliminarLogsAntiguosAsync(fechaLimite);

        _logger.LogInformation("Limpieza de logs de auditoría: {Count} registros eliminados (más antiguos que {Fecha})",
            eliminados, fechaLimite);

        // Registrar la limpieza como auditoría
        await RegistrarAuditoriaAsync(
            null,
            TipoAccionAuditoria.Eliminacion,
            TipoEntidadAuditoria.Configuracion,
            null,
            $"Limpieza automática de logs de auditoría: {eliminados} registros eliminados",
            null,
            null,
            NivelSeveridadAuditoria.Informativo,
            true,
            null
        );

        return eliminados;
    }

    #endregion

    #region Mappers

    private AuditLogDto MapToDto(AuditLog log)
    {
        return new AuditLogDto
        {
            Id = log.Id,
            UsuarioId = log.UsuarioId,
            UsuarioNombre = log.UsuarioNombre,
            DireccionIP = log.DireccionIP,
            TipoAccion = log.TipoAccion,
            TipoAccionDescripcion = log.TipoAccion.ToString(),
            TipoEntidad = log.TipoEntidad,
            TipoEntidadDescripcion = log.TipoEntidad.ToString(),
            EntidadId = log.EntidadId,
            EntidadDescripcion = log.EntidadDescripcion,
            Descripcion = log.Descripcion,
            NivelSeveridad = log.NivelSeveridad,
            NivelSeveridadDescripcion = log.NivelSeveridad.ToString(),
            EsExitoso = log.EsExitoso,
            MensajeError = log.MensajeError,
            FechaHora = log.FechaHora,
            Modulo = log.Modulo,
            Endpoint = log.Endpoint
        };
    }

    #endregion
}
