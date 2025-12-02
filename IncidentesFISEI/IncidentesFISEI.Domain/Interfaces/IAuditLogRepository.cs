using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Interfaces;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetLogsByUsuarioAsync(int usuarioId, int skip = 0, int take = 50);
    Task<IEnumerable<AuditLog>> GetLogsByEntidadAsync(TipoEntidadAuditoria tipoEntidad, int entidadId);
    Task<IEnumerable<AuditLog>> GetLogsByFechaAsync(DateTime desde, DateTime hasta, int skip = 0, int take = 100);
    Task<IEnumerable<AuditLog>> GetLogsByTipoAccionAsync(TipoAccionAuditoria tipoAccion, int skip = 0, int take = 50);
    Task<IEnumerable<AuditLog>> GetLogsCriticosAsync(DateTime? desde = null, int skip = 0, int take = 50);
    Task<IEnumerable<AuditLog>> BuscarLogsAsync(
        int? usuarioId = null,
        TipoAccionAuditoria? tipoAccion = null,
        TipoEntidadAuditoria? tipoEntidad = null,
        NivelSeveridadAuditoria? nivelSeveridad = null,
        DateTime? desde = null,
        DateTime? hasta = null,
        bool? soloErrores = null,
        int skip = 0,
        int take = 50
    );
    Task<Dictionary<TipoAccionAuditoria, int>> GetEstadisticasPorTipoAccionAsync(DateTime desde, DateTime hasta);
    Task<Dictionary<string, int>> GetEstadisticasPorUsuarioAsync(DateTime desde, DateTime hasta, int top = 10);
    Task<int> EliminarLogsAntiguosAsync(DateTime fechaLimite);
}
