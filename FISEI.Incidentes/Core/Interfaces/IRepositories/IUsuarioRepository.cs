using FISEI.Incidentes.Core.Entities;

namespace FISEI.Incidentes.Core.Interfaces.IRepositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> GetByCorreoAsync(string correo);
        Task<IEnumerable<Usuario>> GetTecnicosPorNivelAsync(int idNivelSoporte);
        Task<bool> EsSPOCAsync(int idUsuario);
    }
}
