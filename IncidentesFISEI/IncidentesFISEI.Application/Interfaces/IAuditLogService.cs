using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.Interfaces;

public interface IAuditLogService
{
    // Crear logs de auditoría
    Task RegistrarAuditoriaAsync(CreateAuditLogDto dto);
    Task RegistrarAuditoriaAsync(
        int? usuarioId,
        TipoAccionAuditoria tipoAccion,
        TipoEntidadAuditoria tipoEntidad,
        int? entidadId,
        string descripcion,
        string? valoresAnteriores = null,
        string? valoresNuevos = null,
        NivelSeveridadAuditoria nivelSeveridad = NivelSeveridadAuditoria.Informativo,
        bool esExitoso = true,
        string? mensajeError = null
    );
    
    // Consultas
    Task<IEnumerable<AuditLogDto>> GetLogsByUsuarioAsync(int usuarioId, int skip = 0, int take = 50);
    Task<IEnumerable<AuditLogDto>> GetLogsByEntidadAsync(TipoEntidadAuditoria tipoEntidad, int entidadId);
    Task<AuditLogDetalladoDto?> GetLogDetalladoAsync(int id);
    Task<IEnumerable<AuditLogDto>> BuscarLogsAsync(BuscarAuditLogsDto filtros);
    Task<IEnumerable<AuditLogDto>> GetLogsCriticosAsync(DateTime? desde = null, int take = 50);
    
    // Estadísticas
    Task<EstadisticasAuditoriaDto> GetEstadisticasAsync(DateTime desde, DateTime hasta);
    
    // Mantenimiento
    Task<int> LimpiarLogsAntiguosAsync(int diasRetencion = 90);
}
