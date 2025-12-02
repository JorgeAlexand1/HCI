using Microsoft.EntityFrameworkCore;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Interfaces;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Infrastructure.Data;

namespace IncidentesFISEI.Infrastructure.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<IEnumerable<Usuario>> GetTecnicosAsync()
    {
        return await _dbSet
            .Where(u => !u.IsDeleted && u.IsActive && 
                   (u.TipoUsuario == TipoUsuario.Tecnico || 
                    u.TipoUsuario == TipoUsuario.Supervisor ||
                    u.TipoUsuario == TipoUsuario.Administrador))
            .OrderBy(u => u.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> GetUsuariosByTipoAsync(TipoUsuario tipo)
    {
        return await _dbSet
            .Where(u => !u.IsDeleted && u.IsActive && u.TipoUsuario == tipo)
            .OrderBy(u => u.FirstName)
            .ToListAsync();
    }

    public async Task<bool> ExistsUsernameAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username && !u.IsDeleted);
    }

    public async Task<bool> ExistsEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email && !u.IsDeleted);
    }
}

public class IncidenteRepository : Repository<Incidente>, IIncidenteRepository
{
    public IncidenteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Incidente>> GetIncidentesByUsuarioAsync(int usuarioId)
    {
        return await _dbSet
            .Include(i => i.ReportadoPor)
            .Include(i => i.AsignadoA)
            .Include(i => i.Categoria)
            .Where(i => !i.IsDeleted && i.ReportadoPorId == usuarioId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incidente>> GetIncidentesByTecnicoAsync(int tecnicoId)
    {
        return await _dbSet
            .Include(i => i.ReportadoPor)
            .Include(i => i.AsignadoA)
            .Include(i => i.Categoria)
            .Where(i => !i.IsDeleted && i.AsignadoAId == tecnicoId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incidente>> GetIncidentesByEstadoAsync(EstadoIncidente estado)
    {
        return await _dbSet
            .Include(i => i.ReportadoPor)
            .Include(i => i.AsignadoA)
            .Include(i => i.Categoria)
            .Where(i => !i.IsDeleted && i.Estado == estado)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incidente>> GetIncidentesByPrioridadAsync(PrioridadIncidente prioridad)
    {
        return await _dbSet
            .Include(i => i.ReportadoPor)
            .Include(i => i.AsignadoA)
            .Include(i => i.Categoria)
            .Where(i => !i.IsDeleted && i.Prioridad == prioridad)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incidente>> GetIncidentesVencidosAsync()
    {
        return await _dbSet
            .Include(i => i.ReportadoPor)
            .Include(i => i.AsignadoA)
            .Include(i => i.Categoria)
            .Where(i => !i.IsDeleted && 
                   i.FechaVencimiento.HasValue && 
                   i.FechaVencimiento < DateTime.UtcNow &&
                   i.Estado != EstadoIncidente.Cerrado &&
                   i.Estado != EstadoIncidente.Resuelto)
            .OrderBy(i => i.FechaVencimiento)
            .ToListAsync();
    }

    public async Task<IEnumerable<Incidente>> GetIncidentesPorVencerAsync(int diasAntes = 1)
    {
        var fechaLimite = DateTime.UtcNow.AddDays(diasAntes);
        return await _dbSet
            .Include(i => i.ReportadoPor)
            .Include(i => i.AsignadoA)
            .Include(i => i.Categoria)
            .Where(i => !i.IsDeleted && 
                   i.FechaVencimiento.HasValue && 
                   i.FechaVencimiento <= fechaLimite &&
                   i.FechaVencimiento > DateTime.UtcNow &&
                   i.Estado != EstadoIncidente.Cerrado &&
                   i.Estado != EstadoIncidente.Resuelto)
            .OrderBy(i => i.FechaVencimiento)
            .ToListAsync();
    }

    public async Task<Incidente?> GetIncidenteCompletoAsync(int id)
    {
        return await _dbSet
            .Include(i => i.ReportadoPor)
            .Include(i => i.AsignadoA)
            .Include(i => i.Categoria)
            .Include(i => i.Comentarios).ThenInclude(c => c.Autor)
            .Include(i => i.ArchivosAdjuntos).ThenInclude(a => a.SubidoPor)
            .Include(i => i.RegistrosTiempo).ThenInclude(r => r.Tecnico)
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task<string> GenerateNumeroIncidenteAsync()
    {
        var año = DateTime.UtcNow.Year;
        var mes = DateTime.UtcNow.Month;
        
        var ultimoNumero = await _dbSet
            .Where(i => i.CreatedAt.Year == año && i.CreatedAt.Month == mes)
            .CountAsync();
        
        var siguienteNumero = ultimoNumero + 1;
        
        return $"INC-{año:0000}-{mes:00}-{siguienteNumero:0000}";
    }

    public async Task<Dictionary<EstadoIncidente, int>> GetEstadisticasEstadoAsync()
    {
        return await _dbSet
            .Where(i => !i.IsDeleted)
            .GroupBy(i => i.Estado)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public async Task<Dictionary<PrioridadIncidente, int>> GetEstadisticasPrioridadAsync()
    {
        return await _dbSet
            .Where(i => !i.IsDeleted)
            .GroupBy(i => i.Prioridad)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }
}

public class CategoriaIncidenteRepository : Repository<CategoriaIncidente>, ICategoriaIncidenteRepository
{
    public CategoriaIncidenteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CategoriaIncidente>> GetCategoriasActivasAsync()
    {
        return await _dbSet
            .Where(c => !c.IsDeleted && c.IsActive)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<CategoriaIncidente>> GetSubCategoriasAsync(int parentId)
    {
        return await _dbSet
            .Where(c => !c.IsDeleted && c.IsActive && c.ParentCategoryId == parentId)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<CategoriaIncidente?> GetCategoriaConSubCategoriasAsync(int id)
    {
        return await _dbSet
            .Include(c => c.SubCategorias.Where(s => !s.IsDeleted && s.IsActive))
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }
}

public class ArticuloConocimientoRepository : Repository<ArticuloConocimiento>, IArticuloConocimientoRepository
{
    public ArticuloConocimientoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ArticuloConocimiento>> GetArticulosPublicadosAsync()
    {
        return await _dbSet
            .Include(a => a.Autor)
            .Include(a => a.Categoria)
            .Where(a => !a.IsDeleted && a.Estado == EstadoArticulo.Publicado)
            .OrderByDescending(a => a.FechaPublicacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<ArticuloConocimiento>> GetArticulosByAutorAsync(int autorId)
    {
        return await _dbSet
            .Include(a => a.Categoria)
            .Where(a => !a.IsDeleted && a.AutorId == autorId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ArticuloConocimiento>> GetArticulosByCategoriaAsync(int categoriaId)
    {
        return await _dbSet
            .Include(a => a.Autor)
            .Where(a => !a.IsDeleted && a.CategoriaId == categoriaId && a.Estado == EstadoArticulo.Publicado)
            .OrderByDescending(a => a.FechaPublicacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<ArticuloConocimiento>> BuscarArticulosAsync(string termino)
    {
        return await _dbSet
            .Include(a => a.Autor)
            .Include(a => a.Categoria)
            .Where(a => !a.IsDeleted && 
                   a.Estado == EstadoArticulo.Publicado &&
                   (a.Titulo.Contains(termino) || 
                    a.Contenido.Contains(termino) ||
                    a.Tags.Any(t => t.Contains(termino))))
            .OrderByDescending(a => a.Visualizaciones)
            .ToListAsync();
    }

    public async Task<ArticuloConocimiento?> GetArticuloCompletoAsync(int id)
    {
        return await _dbSet
            .Include(a => a.Autor)
            .Include(a => a.RevisadoPor)
            .Include(a => a.Categoria)
            .Include(a => a.Comentarios).ThenInclude(c => c.Autor)
            .Include(a => a.ArchivosAdjuntos).ThenInclude(ar => ar.SubidoPor)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }

    public async Task IncrementarVisualizacionesAsync(int id)
    {
        var articulo = await _dbSet.FindAsync(id);
        if (articulo != null)
        {
            articulo.Visualizaciones++;
            _dbSet.Update(articulo);
            await _context.SaveChangesAsync();
        }
    }
}

public class ComentarioIncidenteRepository : Repository<ComentarioIncidente>, IComentarioIncidenteRepository
{
    public ComentarioIncidenteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ComentarioIncidente>> GetComentariosByIncidenteAsync(int incidenteId)
    {
        return await _dbSet
            .Include(c => c.Autor)
            .Where(c => !c.IsDeleted && c.IncidenteId == incidenteId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ComentarioIncidente>> GetComentariosPublicosAsync(int incidenteId)
    {
        return await _dbSet
            .Include(c => c.Autor)
            .Where(c => !c.IsDeleted && c.IncidenteId == incidenteId && !c.EsInterno)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }
}

public class ArchivoAdjuntoRepository : Repository<ArchivoAdjunto>, IArchivoAdjuntoRepository
{
    public ArchivoAdjuntoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ArchivoAdjunto>> GetArchivosByIncidenteAsync(int incidenteId)
    {
        return await _dbSet
            .Include(a => a.SubidoPor)
            .Where(a => !a.IsDeleted && a.IncidenteId == incidenteId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ArchivoAdjunto>> GetArchivosByArticuloAsync(int articuloId)
    {
        return await _dbSet
            .Include(a => a.SubidoPor)
            .Where(a => !a.IsDeleted && a.ArticuloId == articuloId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}