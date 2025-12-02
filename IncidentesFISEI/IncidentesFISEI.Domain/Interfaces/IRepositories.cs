using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Interfaces;

public interface IIncidenteRepository : IRepository<Incidente>
{
    Task<IEnumerable<Incidente>> GetIncidentesByUsuarioAsync(int usuarioId);
    Task<IEnumerable<Incidente>> GetIncidentesByTecnicoAsync(int tecnicoId);
    Task<IEnumerable<Incidente>> GetIncidentesByEstadoAsync(EstadoIncidente estado);
    Task<IEnumerable<Incidente>> GetIncidentesByPrioridadAsync(PrioridadIncidente prioridad);
    Task<IEnumerable<Incidente>> GetIncidentesVencidosAsync();
    Task<IEnumerable<Incidente>> GetIncidentesPorVencerAsync(int diasAntes = 1);
    Task<Incidente?> GetIncidenteCompletoAsync(int id);
    Task<string> GenerateNumeroIncidenteAsync();
    Task<Dictionary<EstadoIncidente, int>> GetEstadisticasEstadoAsync();
    Task<Dictionary<PrioridadIncidente, int>> GetEstadisticasPrioridadAsync();
}

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByUsernameAsync(string username);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<IEnumerable<Usuario>> GetTecnicosAsync();
    Task<IEnumerable<Usuario>> GetUsuariosByTipoAsync(TipoUsuario tipo);
    Task<bool> ExistsUsernameAsync(string username);
    Task<bool> ExistsEmailAsync(string email);
}

public interface ICategoriaIncidenteRepository : IRepository<CategoriaIncidente>
{
    Task<IEnumerable<CategoriaIncidente>> GetCategoriasActivasAsync();
    Task<IEnumerable<CategoriaIncidente>> GetSubCategoriasAsync(int parentId);
    Task<CategoriaIncidente?> GetCategoriaConSubCategoriasAsync(int id);
}

public interface IArticuloConocimientoRepository : IRepository<ArticuloConocimiento>
{
    Task<IEnumerable<ArticuloConocimiento>> GetArticulosPublicadosAsync();
    Task<IEnumerable<ArticuloConocimiento>> GetArticulosByAutorAsync(int autorId);
    Task<IEnumerable<ArticuloConocimiento>> GetArticulosByCategoriaAsync(int categoriaId);
    Task<IEnumerable<ArticuloConocimiento>> BuscarArticulosAsync(string termino);
    Task<ArticuloConocimiento?> GetArticuloCompletoAsync(int id);
    Task IncrementarVisualizacionesAsync(int id);
}

public interface IComentarioIncidenteRepository : IRepository<ComentarioIncidente>
{
    Task<IEnumerable<ComentarioIncidente>> GetComentariosByIncidenteAsync(int incidenteId);
    Task<IEnumerable<ComentarioIncidente>> GetComentariosPublicosAsync(int incidenteId);
}

public interface IArchivoAdjuntoRepository : IRepository<ArchivoAdjunto>
{
    Task<IEnumerable<ArchivoAdjunto>> GetArchivosByIncidenteAsync(int incidenteId);
    Task<IEnumerable<ArchivoAdjunto>> GetArchivosByArticuloAsync(int articuloId);
}