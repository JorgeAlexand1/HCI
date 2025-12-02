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

    public async Task<IEnumerable<Usuario>> GetSupervisoresAsync()
    {
        return await _dbSet
            .Where(u => !u.IsDeleted && u.IsActive && 
                   (u.TipoUsuario == TipoUsuario.Supervisor || u.TipoUsuario == TipoUsuario.Administrador))
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
            .Include(a => a.ArticulosEtiquetas).ThenInclude(ae => ae.Etiqueta)
            .Where(a => !a.IsDeleted && a.Estado == EstadoArticulo.Publicado)
            .OrderByDescending(a => a.FechaPublicacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<ArticuloConocimiento>> GetArticulosByAutorAsync(int autorId)
    {
        return await _dbSet
            .Include(a => a.Categoria)
            .Include(a => a.ArticulosEtiquetas).ThenInclude(ae => ae.Etiqueta)
            .Where(a => !a.IsDeleted && a.AutorId == autorId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ArticuloConocimiento>> GetArticulosByCategoriaAsync(int categoriaId)
    {
        return await _dbSet
            .Include(a => a.Autor)
            .Include(a => a.ArticulosEtiquetas).ThenInclude(ae => ae.Etiqueta)
            .Where(a => !a.IsDeleted && a.CategoriaId == categoriaId && a.Estado == EstadoArticulo.Publicado)
            .OrderByDescending(a => a.FechaPublicacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<ArticuloConocimiento>> BuscarArticulosAsync(string termino)
    {
        return await _dbSet
            .Include(a => a.Autor)
            .Include(a => a.Categoria)
            .Include(a => a.ArticulosEtiquetas).ThenInclude(ae => ae.Etiqueta)
            .Where(a => !a.IsDeleted && 
                   a.Estado == EstadoArticulo.Publicado &&
                   (a.Titulo.Contains(termino) || 
                    a.Contenido.Contains(termino) ||
                    a.Tags.Any(t => t.Contains(termino)) ||
                    a.ArticulosEtiquetas.Any(ae => ae.Etiqueta.Nombre.Contains(termino))))
            .OrderByDescending(a => a.Visualizaciones)
            .ToListAsync();
    }

    public async Task<ArticuloConocimiento?> GetArticuloCompletoAsync(int id)
    {
        return await _dbSet
            .Include(a => a.Autor)
            .Include(a => a.RevisadoPor)
            .Include(a => a.Categoria)
            .Include(a => a.Comentarios.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Usuario)
            .Include(a => a.Comentarios.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Respuestas.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.Usuario)
            .Include(a => a.ArchivosAdjuntos.Where(ar => !ar.IsDeleted))
                .ThenInclude(ar => ar.SubidoPor)
            .Include(a => a.Versiones.Where(v => !v.IsDeleted)).ThenInclude(v => v.ModificadoPor)
            .Include(a => a.ArticulosEtiquetas).ThenInclude(ae => ae.Etiqueta)
            .Include(a => a.IncidentesRelacionados.Where(i => !i.IsDeleted))
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

    public async Task<IEnumerable<ArticuloConocimiento>> BusquedaAvanzadaAsync(
        string? termino, 
        int? categoriaId, 
        EstadoArticulo? estado, 
        TipoArticulo? tipoArticulo,
        List<int>? etiquetasIds,
        bool? soloValidados)
    {
        var query = _dbSet
            .Include(a => a.Autor)
            .Include(a => a.Categoria)
            .Include(a => a.ArticulosEtiquetas).ThenInclude(ae => ae.Etiqueta)
            .Where(a => !a.IsDeleted);

        if (!string.IsNullOrWhiteSpace(termino))
        {
            query = query.Where(a => 
                a.Titulo.Contains(termino) || 
                a.Contenido.Contains(termino) ||
                a.Resumen!.Contains(termino) ||
                a.Tags.Any(t => t.Contains(termino)));
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(a => a.CategoriaId == categoriaId.Value);
        }

        if (estado.HasValue)
        {
            query = query.Where(a => a.Estado == estado.Value);
        }

        if (tipoArticulo.HasValue)
        {
            query = query.Where(a => a.TipoArticulo == tipoArticulo.Value);
        }

        if (soloValidados == true)
        {
            query = query.Where(a => a.EsSolucionValidada);
        }

        if (etiquetasIds != null && etiquetasIds.Any())
        {
            query = query.Where(a => a.ArticulosEtiquetas.Any(ae => etiquetasIds.Contains(ae.EtiquetaId)));
        }

        return await query
            .OrderByDescending(a => a.FechaPublicacion ?? a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ArticuloConocimiento>> GetArticulosMasPopularesAsync(int cantidad)
    {
        return await _dbSet
            .Include(a => a.Autor)
            .Include(a => a.Categoria)
            .Where(a => !a.IsDeleted && a.Estado == EstadoArticulo.Publicado)
            .OrderByDescending(a => a.Visualizaciones)
            .ThenByDescending(a => a.VotosPositivos)
            .Take(cantidad)
            .ToListAsync();
    }

    public async Task<IEnumerable<ArticuloConocimiento>> GetArticulosPorEtiquetaAsync(int etiquetaId)
    {
        return await _dbSet
            .Include(a => a.Autor)
            .Include(a => a.Categoria)
            .Include(a => a.ArticulosEtiquetas).ThenInclude(ae => ae.Etiqueta)
            .Where(a => !a.IsDeleted && 
                   a.Estado == EstadoArticulo.Publicado &&
                   a.ArticulosEtiquetas.Any(ae => ae.EtiquetaId == etiquetaId))
            .OrderByDescending(a => a.FechaPublicacion)
            .ToListAsync();
    }
}

// ============ Nuevos Repositorios ============

public class EtiquetaConocimientoRepository : Repository<EtiquetaConocimiento>, IEtiquetaConocimientoRepository
{
    public EtiquetaConocimientoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<EtiquetaConocimiento?> GetByNombreAsync(string nombre)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => !e.IsDeleted && e.Nombre.ToLower() == nombre.ToLower());
    }

    public async Task<IEnumerable<EtiquetaConocimiento>> GetEtiquetasMasUsadasAsync(int cantidad)
    {
        return await _dbSet
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.VecesUsada)
            .Take(cantidad)
            .ToListAsync();
    }

    public async Task IncrementarUsoAsync(int etiquetaId)
    {
        var etiqueta = await _dbSet.FindAsync(etiquetaId);
        if (etiqueta != null)
        {
            etiqueta.VecesUsada++;
            _dbSet.Update(etiqueta);
            await _context.SaveChangesAsync();
        }
    }
}

public class VersionArticuloRepository : Repository<VersionArticulo>, IVersionArticuloRepository
{
    public VersionArticuloRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<VersionArticulo>> GetVersionesByArticuloAsync(int articuloId)
    {
        return await _dbSet
            .Include(v => v.ModificadoPor)
            .Where(v => !v.IsDeleted && v.ArticuloConocimientoId == articuloId)
            .OrderByDescending(v => v.NumeroVersion)
            .ToListAsync();
    }

    public async Task<VersionArticulo?> GetVersionAsync(int articuloId, int numeroVersion)
    {
        return await _dbSet
            .Include(v => v.ModificadoPor)
            .FirstOrDefaultAsync(v => 
                !v.IsDeleted && 
                v.ArticuloConocimientoId == articuloId && 
                v.NumeroVersion == numeroVersion);
    }
}

public class ValidacionArticuloRepository : Repository<ValidacionArticulo>, IValidacionArticuloRepository
{
    public ValidacionArticuloRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ValidacionArticulo>> GetValidacionesPendientesAsync(int? validadorId = null)
    {
        var query = _dbSet
            .Include(v => v.ArticuloConocimiento)
            .Include(v => v.SolicitadoPor)
            .Include(v => v.Validador)
            .Where(v => !v.IsDeleted && 
                   (v.Estado == EstadoValidacion.Pendiente || v.Estado == EstadoValidacion.EnRevision));

        if (validadorId.HasValue)
        {
            query = query.Where(v => v.ValidadorId == validadorId.Value);
        }

        return await query
            .OrderBy(v => v.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ValidacionArticulo>> GetValidacionesByArticuloAsync(int articuloId)
    {
        return await _dbSet
            .Include(v => v.SolicitadoPor)
            .Include(v => v.Validador)
            .Where(v => !v.IsDeleted && v.ArticuloConocimientoId == articuloId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
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

public class NotificacionRepository : Repository<Notificacion>, INotificacionRepository
{
    public NotificacionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Notificacion>> GetNotificacionesByUsuarioAsync(int usuarioId, bool soloNoLeidas = false)
    {
        var query = _dbSet
            .Include(n => n.Usuario)
            .Include(n => n.Incidente)
            .Where(n => n.UsuarioId == usuarioId && !n.IsDeleted);

        if (soloNoLeidas)
        {
            query = query.Where(n => !n.Leida);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetCountNoLeidasAsync(int usuarioId)
    {
        return await _dbSet
            .Where(n => n.UsuarioId == usuarioId && !n.Leida && !n.IsDeleted)
            .CountAsync();
    }

    public async Task MarcarComoLeidaAsync(int notificacionId)
    {
        var notificacion = await _dbSet.FindAsync(notificacionId);
        if (notificacion != null && !notificacion.Leida)
        {
            notificacion.Leida = true;
            notificacion.FechaLectura = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarcarTodasComoLeidasAsync(int usuarioId)
    {
        var notificaciones = await _dbSet
            .Where(n => n.UsuarioId == usuarioId && !n.Leida && !n.IsDeleted)
            .ToListAsync();

        foreach (var notificacion in notificaciones)
        {
            notificacion.Leida = true;
            notificacion.FechaLectura = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notificacion>> GetNotificacionesPorTipoAsync(int usuarioId, TipoNotificacion tipo)
    {
        return await _dbSet
            .Include(n => n.Incidente)
            .Where(n => n.UsuarioId == usuarioId && n.Tipo == tipo && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notificacion>> GetNotificacionesPorIncidenteAsync(int incidenteId)
    {
        return await _dbSet
            .Include(n => n.Usuario)
            .Where(n => n.IncidenteId == incidenteId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task EliminarNotificacionesAntiguasAsync(DateTime fechaLimite)
    {
        var notificacionesAntiguas = await _dbSet
            .Where(n => n.CreatedAt < fechaLimite && n.Leida)
            .ToListAsync();

        foreach (var notificacion in notificacionesAntiguas)
        {
            notificacion.IsDeleted = true;
        }

        await _context.SaveChangesAsync();
    }
}

public class PlantillaEncuestaRepository : Repository<PlantillaEncuesta>, IPlantillaEncuestaRepository
{
    public PlantillaEncuestaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PlantillaEncuesta>> GetPlantillasActivasAsync()
    {
        return await _dbSet
            .Where(p => !p.IsDeleted && p.EsActiva)
            .Include(p => p.Preguntas.OrderBy(pr => pr.Orden))
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<PlantillaEncuesta?> GetPlantillaConPreguntasAsync(int id)
    {
        return await _dbSet
            .Where(p => !p.IsDeleted && p.Id == id)
            .Include(p => p.Preguntas.OrderBy(pr => pr.Orden))
            .FirstOrDefaultAsync();
    }

    public async Task<PlantillaEncuesta?> GetPlantillaPorTipoAsync(TipoEncuesta tipo)
    {
        return await _dbSet
            .Where(p => !p.IsDeleted && p.EsActiva && p.Tipo == tipo && p.MostrarAutomaticamente)
            .Include(p => p.Preguntas.OrderBy(pr => pr.Orden))
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<PlantillaEncuesta>> GetPlantillasParaMostrarAsync()
    {
        return await _dbSet
            .Where(p => !p.IsDeleted && p.EsActiva && p.MostrarAutomaticamente)
            .Include(p => p.Preguntas.OrderBy(pr => pr.Orden))
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }
}

public class PreguntaEncuestaRepository : Repository<PreguntaEncuesta>, IPreguntaEncuestaRepository
{
    public PreguntaEncuestaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PreguntaEncuesta>> GetPreguntasByPlantillaAsync(int plantillaId)
    {
        return await _dbSet
            .Where(p => !p.IsDeleted && p.PlantillaEncuestaId == plantillaId)
            .OrderBy(p => p.Orden)
            .ToListAsync();
    }

    public async Task<int> GetSiguienteOrdenAsync(int plantillaId)
    {
        var maxOrden = await _dbSet
            .Where(p => !p.IsDeleted && p.PlantillaEncuestaId == plantillaId)
            .MaxAsync(p => (int?)p.Orden);

        return (maxOrden ?? 0) + 1;
    }
}

public class EncuestaRepository : Repository<Encuesta>, IEncuestaRepository
{
    public EncuestaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Encuesta>> GetEncuestasPendientesByUsuarioAsync(int usuarioId)
    {
        return await _dbSet
            .Where(e => !e.IsDeleted && e.UsuarioId == usuarioId && !e.EsRespondida && !e.EsVencida)
            .Include(e => e.Incidente)
            .Include(e => e.PlantillaEncuesta)
            .OrderByDescending(e => e.FechaEnvio)
            .ToListAsync();
    }

    public async Task<IEnumerable<Encuesta>> GetEncuestasVencidasAsync()
    {
        var ahora = DateTime.UtcNow;
        return await _dbSet
            .Where(e => !e.IsDeleted && !e.EsRespondida && !e.EsVencida && e.FechaVencimiento < ahora)
            .ToListAsync();
    }

    public async Task<Encuesta?> GetEncuestaConRespuestasAsync(int encuestaId)
    {
        return await _dbSet
            .Where(e => !e.IsDeleted && e.Id == encuestaId)
            .Include(e => e.Incidente)
            .Include(e => e.PlantillaEncuesta)
                .ThenInclude(p => p.Preguntas.OrderBy(pr => pr.Orden))
            .Include(e => e.Respuestas)
                .ThenInclude(r => r.PreguntaEncuesta)
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync();
    }

    public async Task<Encuesta?> GetEncuestaByIncidenteAsync(int incidenteId)
    {
        return await _dbSet
            .Where(e => !e.IsDeleted && e.IncidenteId == incidenteId)
            .Include(e => e.PlantillaEncuesta)
            .OrderByDescending(e => e.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Encuesta>> GetEncuestasRespondidasAsync(DateTime? desde = null, DateTime? hasta = null)
    {
        var query = _dbSet.Where(e => !e.IsDeleted && e.EsRespondida);

        if (desde.HasValue)
            query = query.Where(e => e.FechaRespuesta >= desde.Value);

        if (hasta.HasValue)
            query = query.Where(e => e.FechaRespuesta <= hasta.Value);

        return await query
            .Include(e => e.Incidente)
            .Include(e => e.Usuario)
            .Include(e => e.PlantillaEncuesta)
            .OrderByDescending(e => e.FechaRespuesta)
            .ToListAsync();
    }

    public async Task<double> GetPromedioCalificacionAsync(DateTime? desde = null, DateTime? hasta = null)
    {
        var query = _dbSet.Where(e => !e.IsDeleted && e.EsRespondida && e.CalificacionPromedio.HasValue);

        if (desde.HasValue)
            query = query.Where(e => e.FechaRespuesta >= desde.Value);

        if (hasta.HasValue)
            query = query.Where(e => e.FechaRespuesta <= hasta.Value);

        var promedio = await query.AverageAsync(e => e.CalificacionPromedio);
        return promedio ?? 0;
    }

    public async Task<Dictionary<int, int>> GetDistribucionCalificacionesAsync(DateTime? desde = null, DateTime? hasta = null)
    {
        var query = _dbSet.Where(e => !e.IsDeleted && e.EsRespondida && e.CalificacionPromedio.HasValue);

        if (desde.HasValue)
            query = query.Where(e => e.FechaRespuesta >= desde.Value);

        if (hasta.HasValue)
            query = query.Where(e => e.FechaRespuesta <= hasta.Value);

        var calificaciones = await query
            .Select(e => (int)Math.Round(e.CalificacionPromedio!.Value))
            .ToListAsync();

        return calificaciones
            .GroupBy(c => c)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}

public class RespuestaEncuestaRepository : Repository<RespuestaEncuesta>, IRespuestaEncuestaRepository
{
    public RespuestaEncuestaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<RespuestaEncuesta>> GetRespuestasByEncuestaAsync(int encuestaId)
    {
        return await _dbSet
            .Where(r => !r.IsDeleted && r.EncuestaId == encuestaId)
            .Include(r => r.PreguntaEncuesta)
            .OrderBy(r => r.PreguntaEncuesta.Orden)
            .ToListAsync();
    }

    public async Task<IEnumerable<RespuestaEncuesta>> GetRespuestasByPreguntaAsync(int preguntaId)
    {
        return await _dbSet
            .Where(r => !r.IsDeleted && r.PreguntaEncuestaId == preguntaId)
            .Include(r => r.Encuesta)
            .ToListAsync();
    }
}

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AuditLog>> GetLogsByUsuarioAsync(int usuarioId, int skip = 0, int take = 50)
    {
        return await _dbSet
            .Where(a => !a.IsDeleted && a.UsuarioId == usuarioId)
            .OrderByDescending(a => a.FechaHora)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetLogsByEntidadAsync(TipoEntidadAuditoria tipoEntidad, int entidadId)
    {
        return await _dbSet
            .Where(a => !a.IsDeleted && a.TipoEntidad == tipoEntidad && a.EntidadId == entidadId)
            .OrderByDescending(a => a.FechaHora)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetLogsByFechaAsync(DateTime desde, DateTime hasta, int skip = 0, int take = 100)
    {
        return await _dbSet
            .Where(a => !a.IsDeleted && a.FechaHora >= desde && a.FechaHora <= hasta)
            .OrderByDescending(a => a.FechaHora)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetLogsByTipoAccionAsync(TipoAccionAuditoria tipoAccion, int skip = 0, int take = 50)
    {
        return await _dbSet
            .Where(a => !a.IsDeleted && a.TipoAccion == tipoAccion)
            .OrderByDescending(a => a.FechaHora)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetLogsCriticosAsync(DateTime? desde = null, int skip = 0, int take = 50)
    {
        var query = _dbSet.Where(a => !a.IsDeleted && 
            (a.NivelSeveridad == NivelSeveridadAuditoria.Alto || a.NivelSeveridad == NivelSeveridadAuditoria.Critico));

        if (desde.HasValue)
            query = query.Where(a => a.FechaHora >= desde.Value);

        return await query
            .OrderByDescending(a => a.FechaHora)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> BuscarLogsAsync(
        int? usuarioId = null,
        TipoAccionAuditoria? tipoAccion = null,
        TipoEntidadAuditoria? tipoEntidad = null,
        NivelSeveridadAuditoria? nivelSeveridad = null,
        DateTime? desde = null,
        DateTime? hasta = null,
        bool? soloErrores = null,
        int skip = 0,
        int take = 50)
    {
        var query = _dbSet.Where(a => !a.IsDeleted);

        if (usuarioId.HasValue)
            query = query.Where(a => a.UsuarioId == usuarioId.Value);

        if (tipoAccion.HasValue)
            query = query.Where(a => a.TipoAccion == tipoAccion.Value);

        if (tipoEntidad.HasValue)
            query = query.Where(a => a.TipoEntidad == tipoEntidad.Value);

        if (nivelSeveridad.HasValue)
            query = query.Where(a => a.NivelSeveridad == nivelSeveridad.Value);

        if (desde.HasValue)
            query = query.Where(a => a.FechaHora >= desde.Value);

        if (hasta.HasValue)
            query = query.Where(a => a.FechaHora <= hasta.Value);

        if (soloErrores.HasValue && soloErrores.Value)
            query = query.Where(a => !a.EsExitoso);

        return await query
            .OrderByDescending(a => a.FechaHora)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Dictionary<TipoAccionAuditoria, int>> GetEstadisticasPorTipoAccionAsync(DateTime desde, DateTime hasta)
    {
        return await _dbSet
            .Where(a => !a.IsDeleted && a.FechaHora >= desde && a.FechaHora <= hasta)
            .GroupBy(a => a.TipoAccion)
            .Select(g => new { TipoAccion = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TipoAccion, x => x.Count);
    }

    public async Task<Dictionary<string, int>> GetEstadisticasPorUsuarioAsync(DateTime desde, DateTime hasta, int top = 10)
    {
        return await _dbSet
            .Where(a => !a.IsDeleted && a.FechaHora >= desde && a.FechaHora <= hasta && a.UsuarioNombre != null)
            .GroupBy(a => a.UsuarioNombre!)
            .Select(g => new { Usuario = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(top)
            .ToDictionaryAsync(x => x.Usuario, x => x.Count);
    }

    public async Task<int> EliminarLogsAntiguosAsync(DateTime fechaLimite)
    {
        var logsAntiguos = await _dbSet
            .Where(a => a.FechaHora < fechaLimite && !a.IsDeleted)
            .ToListAsync();

        foreach (var log in logsAntiguos)
        {
            log.IsDeleted = true;
        }

        await _context.SaveChangesAsync();
        return logsAntiguos.Count;
    }
}
