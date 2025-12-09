using AutoMapper;
using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace IncidentesFISEI.Infrastructure.Services;

public class BaseConocimientoService : IBaseConocimientoService
{
    private readonly IArticuloConocimientoRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<BaseConocimientoService> _logger;

    public BaseConocimientoService(
        IArticuloConocimientoRepository repository,
        IMapper mapper,
        ILogger<BaseConocimientoService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ArticuloConocimientoDto>> GetAllArticulosAsync()
    {
        try
        {
            var articulos = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ArticuloConocimientoDto>>(articulos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo todos los artículos");
            throw;
        }
    }

    public async Task<ArticuloConocimientoDto> GetArticuloByIdAsync(int id)
    {
        try
        {
            var articulo = await _repository.GetArticuloCompletoAsync(id);
            if (articulo == null)
                throw new KeyNotFoundException($"Artículo {id} no encontrado");

            await _repository.IncrementarVisualizacionesAsync(id);
            return _mapper.Map<ArticuloConocimientoDto>(articulo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error obteniendo artículo {id}");
            throw;
        }
    }

    public async Task<IEnumerable<ArticuloConocimientoDto>> SearchArticulosAsync(string searchTerm)
    {
        try
        {
            var articulos = await _repository.BuscarArticulosAsync(searchTerm);
            return _mapper.Map<IEnumerable<ArticuloConocimientoDto>>(articulos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error buscando artículos con término '{searchTerm}'");
            throw;
        }
    }

    public async Task<IEnumerable<ArticuloConocimientoDto>> GetArticulosByTipoAsync(TipoArticulo tipo)
    {
        try
        {
            var articulos = await _repository.GetAllAsync();
            var filtrados = articulos.Where(a => !a.IsDeleted && a.Estado == EstadoArticulo.Publicado);
            return _mapper.Map<IEnumerable<ArticuloConocimientoDto>>(filtrados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error obteniendo artículos por tipo {tipo}");
            throw;
        }
    }

    public async Task<ArticuloConocimientoDto> CreateArticuloAsync(CreateArticuloConocimientoDto createDto)
    {
        try
        {
            var articulo = new ArticuloConocimiento
            {
                Titulo = createDto.Titulo,
                Contenido = createDto.Contenido,
                Resumen = createDto.Resumen,
                AutorId = createDto.AutorId,
                CategoriaId = createDto.CategoriaId,
                Estado = EstadoArticulo.Borrador,
                Tags = createDto.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>(),
                PasosDetallados = createDto.PasosDetallados,
                Prerequisites = createDto.Prerequisites,
                Limitaciones = createDto.Limitaciones
            };

            await _repository.AddAsync(articulo);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ArticuloConocimientoDto>(articulo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando artículo");
            throw;
        }
    }

    public async Task<ArticuloConocimientoDto> UpdateArticuloAsync(int id, UpdateArticuloConocimientoDto updateDto)
    {
        try
        {
            var articulo = await _repository.GetByIdAsync(id);
            if (articulo == null)
                throw new KeyNotFoundException($"Artículo {id} no encontrado");

            articulo.Titulo = updateDto.Titulo;
            articulo.Contenido = updateDto.Contenido;
            articulo.Resumen = updateDto.Resumen;
            articulo.Tags = updateDto.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            articulo.PasosDetallados = updateDto.PasosDetallados;
            articulo.Prerequisites = updateDto.Prerequisites;
            articulo.Limitaciones = updateDto.Limitaciones;

            await _repository.UpdateAsync(articulo);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ArticuloConocimientoDto>(articulo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error actualizando artículo {id}");
            throw;
        }
    }

    public async Task<bool> DeleteArticuloAsync(int id)
    {
        try
        {
            var articulo = await _repository.GetByIdAsync(id);
            if (articulo == null)
                return false;

            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error eliminando artículo {id}");
            throw;
        }
    }

    public async Task<IEnumerable<ArticuloConocimientoDto>> GetArticulosPublicosAsync()
    {
        try
        {
            var articulos = await _repository.GetArticulosPublicadosAsync();
            return _mapper.Map<IEnumerable<ArticuloConocimientoDto>>(articulos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo artículos públicos");
            throw;
        }
    }

    public async Task<IEnumerable<ArticuloConocimientoDto>> GetArticulosTodosAsync()
    {
        try
        {
            var articulos = await _repository.GetArticulosTodosAsync();
            return _mapper.Map<IEnumerable<ArticuloConocimientoDto>>(articulos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo todos los artículos");
            throw;
        }
    }

    public async Task<bool> PublicarArticuloAsync(int id, int revisadoPorId)
    {
        try
        {
            var articulo = await _repository.GetByIdAsync(id);
            if (articulo == null)
                return false;

            articulo.Estado = EstadoArticulo.Publicado;
            articulo.FechaPublicacion = DateTime.UtcNow;
            articulo.RevisadoPorId = revisadoPorId;
            articulo.FechaRevision = DateTime.UtcNow;

            await _repository.UpdateAsync(articulo);
            await _repository.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error publicando artículo {id}");
            throw;
        }
    }

    public async Task<IEnumerable<ArticuloConocimientoDto>> GetArticulosMasConsultadosAsync(int cantidad = 10)
    {
        try
        {
            var articulos = await _repository.GetAllAsync();
            var topArticulos = articulos
                .Where(a => !a.IsDeleted && a.Estado == EstadoArticulo.Publicado)
                .OrderByDescending(a => a.Visualizaciones)
                .Take(cantidad);

            return _mapper.Map<IEnumerable<ArticuloConocimientoDto>>(topArticulos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo artículos más consultados");
            throw;
        }
    }
}
