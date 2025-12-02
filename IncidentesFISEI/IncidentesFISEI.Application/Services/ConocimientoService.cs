using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace IncidentesFISEI.Application.Services;

public class ConocimientoService : IConocimientoService
{
    private readonly IArticuloConocimientoRepository _articuloRepository;
    private readonly IEtiquetaConocimientoRepository _etiquetaRepository;
    private readonly IVersionArticuloRepository _versionRepository;
    private readonly IValidacionArticuloRepository _validacionRepository;
    private readonly IRepository<ComentarioArticulo> _comentarioRepository;
    private readonly IRepository<VotacionArticulo> _votacionRepository;
    private readonly IRepository<ArticuloEtiqueta> _articuloEtiquetaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<ConocimientoService> _logger;

    public ConocimientoService(
        IArticuloConocimientoRepository articuloRepository,
        IEtiquetaConocimientoRepository etiquetaRepository,
        IVersionArticuloRepository versionRepository,
        IValidacionArticuloRepository validacionRepository,
        IRepository<ComentarioArticulo> comentarioRepository,
        IRepository<VotacionArticulo> votacionRepository,
        IRepository<ArticuloEtiqueta> articuloEtiquetaRepository,
        IUsuarioRepository usuarioRepository,
        ILogger<ConocimientoService> logger)
    {
        _articuloRepository = articuloRepository;
        _etiquetaRepository = etiquetaRepository;
        _versionRepository = versionRepository;
        _validacionRepository = validacionRepository;
        _comentarioRepository = comentarioRepository;
        _votacionRepository = votacionRepository;
        _articuloEtiquetaRepository = articuloEtiquetaRepository;
        _usuarioRepository = usuarioRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<List<ArticuloConocimientoDto>>> GetArticulosPublicadosAsync()
    {
        try
        {
            var articulos = await _articuloRepository.GetArticulosPublicadosAsync();
            var dtos = articulos.Select(MapToDto).ToList();
            return new ApiResponse<List<ArticuloConocimientoDto>>(true, dtos, $"{dtos.Count} artículos encontrados");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener artículos publicados");
            return new ApiResponse<List<ArticuloConocimientoDto>>(false, new List<ArticuloConocimientoDto>(), "Error al obtener artículos");
        }
    }

    public async Task<ApiResponse<ArticuloDetalladoDto>> GetArticuloByIdAsync(int id, int? usuarioId = null)
    {
        try
        {
            var articulo = await _articuloRepository.GetArticuloCompletoAsync(id);
            if (articulo == null)
                return new ApiResponse<ArticuloDetalladoDto>(false, null, "Artículo no encontrado");

            await _articuloRepository.IncrementarVisualizacionesAsync(id);

            var dto = MapToDetalladoDto(articulo);
            return new ApiResponse<ArticuloDetalladoDto>(true, dto, "Artículo obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener artículo {Id}", id);
            return new ApiResponse<ArticuloDetalladoDto>(false, null, "Error al obtener artículo");
        }
    }

    public async Task<ApiResponse<ArticuloDetalladoDto>> CreateArticuloAsync(CreateArticuloDto dto, int autorId)
    {
        try
        {
            var articulo = new ArticuloConocimiento
            {
                Titulo = dto.Titulo,
                Contenido = dto.Contenido,
                Resumen = dto.Resumen,
                CategoriaId = dto.CategoriaId,
                TipoArticulo = dto.TipoArticulo,
                AutorId = autorId,
                Tags = dto.Tags,
                PasosDetallados = dto.PasosDetallados,
                Prerequisites = dto.Prerequisites,
                Limitaciones = dto.Limitaciones,
                Estado = dto.PublicarInmediatamente ? EstadoArticulo.Publicado : EstadoArticulo.Borrador,
                FechaPublicacion = dto.PublicarInmediatamente ? DateTime.UtcNow : null,
                VersionActual = 1
            };

            await _articuloRepository.AddAsync(articulo);

            // Crear versión inicial
            var version = new VersionArticulo
            {
                ArticuloConocimientoId = articulo.Id,
                NumeroVersion = 1,
                Titulo = articulo.Titulo,
                Contenido = articulo.Contenido,
                Resumen = articulo.Resumen,
                Tags = articulo.Tags,
                ModificadoPorId = autorId,
                CambiosRealizados = "Versión inicial"
            };
            await _versionRepository.AddAsync(version);

            // Asignar etiquetas
            if (dto.EtiquetasIds.Any())
            {
                foreach (var etiquetaId in dto.EtiquetasIds)
                {
                    await _articuloEtiquetaRepository.AddAsync(new ArticuloEtiqueta
                    {
                        ArticuloConocimientoId = articulo.Id,
                        EtiquetaId = etiquetaId
                    });
                    await _etiquetaRepository.IncrementarUsoAsync(etiquetaId);
                }
            }

            var articuloCompleto = await _articuloRepository.GetArticuloCompletoAsync(articulo.Id);
            var result = MapToDetalladoDto(articuloCompleto!);

            return new ApiResponse<ArticuloDetalladoDto>(true, result, "Artículo creado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear artículo");
            return new ApiResponse<ArticuloDetalladoDto>(false, null, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ArticuloDetalladoDto>> UpdateArticuloAsync(int id, UpdateArticuloDto dto, int usuarioId)
    {
        try
        {
            var articulo = await _articuloRepository.GetByIdAsync(id);
            if (articulo == null)
                return new ApiResponse<ArticuloDetalladoDto>(false, null, "Artículo no encontrado");

            // Crear nueva versión
            articulo.VersionActual++;
            var version = new VersionArticulo
            {
                ArticuloConocimientoId = articulo.Id,
                NumeroVersion = articulo.VersionActual,
                Titulo = dto.Titulo,
                Contenido = dto.Contenido,
                Resumen = dto.Resumen,
                Tags = dto.Tags,
                ModificadoPorId = usuarioId,
                CambiosRealizados = dto.CambiosRealizados
            };
            await _versionRepository.AddAsync(version);

            // Actualizar artículo
            articulo.Titulo = dto.Titulo;
            articulo.Contenido = dto.Contenido;
            articulo.Resumen = dto.Resumen;
            articulo.CategoriaId = dto.CategoriaId;
            articulo.TipoArticulo = dto.TipoArticulo;
            articulo.Tags = dto.Tags;
            articulo.PasosDetallados = dto.PasosDetallados;
            articulo.Prerequisites = dto.Prerequisites;
            articulo.Limitaciones = dto.Limitaciones;
            articulo.FechaRevision = DateTime.UtcNow;

            // Actualizar etiquetas
            var etiquetasActuales = (await _articuloEtiquetaRepository.GetAllAsync())
                .Where(ae => ae.ArticuloConocimientoId == id).ToList();
            
            foreach (var ae in etiquetasActuales)
            {
                await _articuloEtiquetaRepository.DeleteAsync(ae.ArticuloConocimientoId);
            }

            foreach (var etiquetaId in dto.EtiquetasIds)
            {
                await _articuloEtiquetaRepository.AddAsync(new ArticuloEtiqueta
                {
                    ArticuloConocimientoId = id,
                    EtiquetaId = etiquetaId
                });
            }

            await _articuloRepository.UpdateAsync(articulo);

            var articuloCompleto = await _articuloRepository.GetArticuloCompletoAsync(id);
            var result = MapToDetalladoDto(articuloCompleto!);

            return new ApiResponse<ArticuloDetalladoDto>(true, result, "Artículo actualizado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar artículo {Id}", id);
            return new ApiResponse<ArticuloDetalladoDto>(false, null, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteArticuloAsync(int id, int usuarioId)
    {
        try
        {
            var articulo = await _articuloRepository.GetByIdAsync(id);
            if (articulo == null)
                return new ApiResponse<bool>(false, false, "Artículo no encontrado");

            await _articuloRepository.DeleteAsync(id);
            return new ApiResponse<bool>(true, true, "Artículo eliminado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar artículo {Id}", id);
            return new ApiResponse<bool>(false, false, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> PublicarArticuloAsync(int id, int usuarioId)
    {
        try
        {
            var articulo = await _articuloRepository.GetByIdAsync(id);
            if (articulo == null)
                return new ApiResponse<bool>(false, false, "Artículo no encontrado");

            articulo.Estado = EstadoArticulo.Publicado;
            articulo.FechaPublicacion = DateTime.UtcNow;
            articulo.RevisadoPorId = usuarioId;

            await _articuloRepository.UpdateAsync(articulo);
            return new ApiResponse<bool>(true, true, "Artículo publicado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al publicar artículo {Id}", id);
            return new ApiResponse<bool>(false, false, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> ArchivarArticuloAsync(int id, int usuarioId)
    {
        try
        {
            var articulo = await _articuloRepository.GetByIdAsync(id);
            if (articulo == null)
                return new ApiResponse<bool>(false, false, "Artículo no encontrado");

            articulo.Estado = EstadoArticulo.Archivado;
            await _articuloRepository.UpdateAsync(articulo);
            return new ApiResponse<bool>(true, true, "Artículo archivado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al archivar artículo {Id}", id);
            return new ApiResponse<bool>(false, false, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ResultadoBusquedaArticulosDto>> BuscarArticulosAsync(BusquedaArticulosDto dto)
    {
        try
        {
            var articulos = await _articuloRepository.BusquedaAvanzadaAsync(
                dto.Termino, 
                dto.CategoriaId, 
                dto.Estado, 
                dto.TipoArticulo, 
                dto.EtiquetasIds, 
                dto.SoloValidados);

            // Ordenar
            var query = articulos.AsQueryable();
            query = dto.OrdenarPor switch
            {
                OrdenArticulos.MasReciente => query.OrderByDescending(a => a.FechaPublicacion ?? a.CreatedAt),
                OrdenArticulos.MasAntiguo => query.OrderBy(a => a.FechaPublicacion ?? a.CreatedAt),
                OrdenArticulos.MasVisto => query.OrderByDescending(a => a.Visualizaciones),
                OrdenArticulos.MejorValorado => query.OrderByDescending(a => a.VotosPositivos - a.VotosNegativos),
                OrdenArticulos.MasUtilizado => query.OrderByDescending(a => a.VecesUtilizado),
                _ => query.OrderByDescending(a => a.FechaPublicacion ?? a.CreatedAt)
            };

            var total = query.Count();
            var totalPaginas = (int)Math.Ceiling(total / (double)dto.TamañoPagina);

            var articulosPaginados = query
                .Skip((dto.Pagina - 1) * dto.TamañoPagina)
                .Take(dto.TamañoPagina)
                .Select(MapToDto)
                .ToList();

            var resultado = new ResultadoBusquedaArticulosDto
            {
                Articulos = articulosPaginados,
                TotalResultados = total,
                PaginaActual = dto.Pagina,
                TotalPaginas = totalPaginas
            };

            return new ApiResponse<ResultadoBusquedaArticulosDto>(true, resultado, $"{total} artículos encontrados");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en búsqueda de artículos");
            return new ApiResponse<ResultadoBusquedaArticulosDto>(false, new ResultadoBusquedaArticulosDto(), $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ArticuloConocimientoDto>>> GetArticulosPorCategoriaAsync(int categoriaId)
    {
        try
        {
            var articulos = await _articuloRepository.GetArticulosByCategoriaAsync(categoriaId);
            var dtos = articulos.Select(MapToDto).ToList();
            return new ApiResponse<List<ArticuloConocimientoDto>>(true, dtos, $"{dtos.Count} artículos encontrados");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener artículos por categoría");
            return new ApiResponse<List<ArticuloConocimientoDto>>(false, new List<ArticuloConocimientoDto>(), "Error");
        }
    }

    public async Task<ApiResponse<List<ArticuloConocimientoDto>>> GetArticulosPorAutorAsync(int autorId)
    {
        try
        {
            var articulos = await _articuloRepository.GetArticulosByAutorAsync(autorId);
            var dtos = articulos.Select(MapToDto).ToList();
            return new ApiResponse<List<ArticuloConocimientoDto>>(true, dtos, $"{dtos.Count} artículos encontrados");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener artículos por autor");
            return new ApiResponse<List<ArticuloConocimientoDto>>(false, new List<ArticuloConocimientoDto>(), "Error");
        }
    }

    public async Task<ApiResponse<List<EtiquetaDto>>> GetAllEtiquetasAsync()
    {
        try
        {
            var etiquetas = await _etiquetaRepository.GetAllAsync();
            var dtos = etiquetas.Select(e => new EtiquetaDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion,
                Color = e.Color,
                VecesUsada = e.VecesUsada
            }).ToList();

            return new ApiResponse<List<EtiquetaDto>>(true, dtos, $"{dtos.Count} etiquetas");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener etiquetas");
            return new ApiResponse<List<EtiquetaDto>>(false, new List<EtiquetaDto>(), "Error");
        }
    }

    public async Task<ApiResponse<EtiquetaDto>> CreateEtiquetaAsync(CreateEtiquetaDto dto)
    {
        try
        {
            var existente = await _etiquetaRepository.GetByNombreAsync(dto.Nombre);
            if (existente != null)
                return new ApiResponse<EtiquetaDto>(false, null, "Ya existe una etiqueta con ese nombre");

            var etiqueta = new EtiquetaConocimiento
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Color = dto.Color
            };

            await _etiquetaRepository.AddAsync(etiqueta);

            var result = new EtiquetaDto
            {
                Id = etiqueta.Id,
                Nombre = etiqueta.Nombre,
                Descripcion = etiqueta.Descripcion,
                Color = etiqueta.Color,
                VecesUsada = 0
            };

            return new ApiResponse<EtiquetaDto>(true, result, "Etiqueta creada correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear etiqueta");
            return new ApiResponse<EtiquetaDto>(false, null, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<EtiquetaDto>>> GetEtiquetasMasUsadasAsync(int cantidad = 10)
    {
        try
        {
            var etiquetas = await _etiquetaRepository.GetEtiquetasMasUsadasAsync(cantidad);
            var dtos = etiquetas.Select(e => new EtiquetaDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion,
                Color = e.Color,
                VecesUsada = e.VecesUsada
            }).ToList();

            return new ApiResponse<List<EtiquetaDto>>(true, dtos, $"Top {dtos.Count} etiquetas");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener etiquetas populares");
            return new ApiResponse<List<EtiquetaDto>>(false, new List<EtiquetaDto>(), "Error");
        }
    }

    public async Task<ApiResponse<ComentarioArticuloDto>> AddComentarioAsync(int articuloId, CreateComentarioArticuloDto dto, int usuarioId)
    {
        try
        {
            var comentario = new ComentarioArticulo
            {
                ArticuloConocimientoId = articuloId,
                UsuarioId = usuarioId,
                Contenido = dto.Contenido,
                ComentarioPadreId = dto.ComentarioPadreId,
                EsRespuesta = dto.ComentarioPadreId.HasValue
            };

            await _comentarioRepository.AddAsync(comentario);

            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            var result = new ComentarioArticuloDto
            {
                Id = comentario.Id,
                Contenido = comentario.Contenido,
                UsuarioNombre = $"{usuario?.FirstName} {usuario?.LastName}",
                UsuarioId = usuarioId,
                CreatedAt = comentario.CreatedAt,
                EsRespuesta = comentario.EsRespuesta,
                ComentarioPadreId = comentario.ComentarioPadreId
            };

            return new ApiResponse<ComentarioArticuloDto>(true, result, "Comentario agregado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar comentario");
            return new ApiResponse<ComentarioArticuloDto>(false, null, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteComentarioAsync(int comentarioId, int usuarioId)
    {
        try
        {
            var comentario = await _comentarioRepository.GetByIdAsync(comentarioId);
            if (comentario == null)
                return new ApiResponse<bool>(false, false, "Comentario no encontrado");

            if (comentario.UsuarioId != usuarioId)
                return new ApiResponse<bool>(false, false, "No autorizado");

            await _comentarioRepository.DeleteAsync(comentarioId);
            return new ApiResponse<bool>(true, true, "Comentario eliminado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar comentario");
            return new ApiResponse<bool>(false, false, "Error");
        }
    }

    public async Task<ApiResponse<bool>> VotarArticuloAsync(int articuloId, VotarArticuloDto dto, int usuarioId)
    {
        try
        {
            var articulo = await _articuloRepository.GetByIdAsync(articuloId);
            if (articulo == null)
                return new ApiResponse<bool>(false, false, "Artículo no encontrado");

            var votacionExistente = (await _votacionRepository.GetAllAsync())
                .FirstOrDefault(v => v.ArticuloConocimientoId == articuloId && v.UsuarioId == usuarioId);

            if (votacionExistente != null)
            {
                // Actualizar voto
                if (votacionExistente.Voto == TipoVoto.Positivo)
                    articulo.VotosPositivos--;
                else
                    articulo.VotosNegativos--;

                votacionExistente.Voto = dto.Voto;
                votacionExistente.Comentario = dto.Comentario;
                await _votacionRepository.UpdateAsync(votacionExistente);
            }
            else
            {
                // Nuevo voto
                var votacion = new VotacionArticulo
                {
                    ArticuloConocimientoId = articuloId,
                    UsuarioId = usuarioId,
                    Voto = dto.Voto,
                    Comentario = dto.Comentario
                };
                await _votacionRepository.AddAsync(votacion);
            }

            if (dto.Voto == TipoVoto.Positivo)
                articulo.VotosPositivos++;
            else
                articulo.VotosNegativos++;

            await _articuloRepository.UpdateAsync(articulo);

            return new ApiResponse<bool>(true, true, "Voto registrado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al votar artículo");
            return new ApiResponse<bool>(false, false, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ValidacionDto>> SolicitarValidacionAsync(int articuloId, SolicitarValidacionDto dto, int solicitanteId)
    {
        try
        {
            var validacion = new ValidacionArticulo
            {
                ArticuloConocimientoId = articuloId,
                SolicitadoPorId = solicitanteId,
                ValidadorId = dto.ValidadorId,
                Estado = EstadoValidacion.Pendiente
            };

            await _validacionRepository.AddAsync(validacion);

            var articulo = await _articuloRepository.GetByIdAsync(articuloId);
            articulo!.Estado = EstadoArticulo.Revision;
            await _articuloRepository.UpdateAsync(articulo);

            var result = new ValidacionDto
            {
                Id = validacion.Id,
                ArticuloId = articuloId,
                ArticuloTitulo = articulo.Titulo,
                Estado = EstadoValidacion.Pendiente,
                CreatedAt = validacion.CreatedAt
            };

            return new ApiResponse<ValidacionDto>(true, result, "Validación solicitada correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al solicitar validación");
            return new ApiResponse<ValidacionDto>(false, null, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ValidacionDto>> ValidarArticuloAsync(int validacionId, ValidarArticuloDto dto, int validadorId)
    {
        try
        {
            var validacion = await _validacionRepository.GetByIdAsync(validacionId);
            if (validacion == null)
                return new ApiResponse<ValidacionDto>(false, null, "Validación no encontrada");

            if (validacion.ValidadorId != validadorId)
                return new ApiResponse<ValidacionDto>(false, null, "No autorizado");

            validacion.Estado = dto.Aprobado ? EstadoValidacion.Aprobado : EstadoValidacion.Rechazado;
            validacion.Aprobado = dto.Aprobado;
            validacion.ComentariosValidador = dto.ComentariosValidador;
            validacion.FechaValidacion = DateTime.UtcNow;

            await _validacionRepository.UpdateAsync(validacion);

            var articulo = await _articuloRepository.GetByIdAsync(validacion.ArticuloConocimientoId);
            if (articulo != null)
            {
                articulo.Estado = dto.Aprobado ? EstadoArticulo.Publicado : EstadoArticulo.Rechazado;
                articulo.EsSolucionValidada = dto.Aprobado;
                articulo.RevisadoPorId = validadorId;
                articulo.FechaRevision = DateTime.UtcNow;
                if (dto.Aprobado)
                    articulo.FechaPublicacion = DateTime.UtcNow;

                await _articuloRepository.UpdateAsync(articulo);
            }

            var result = new ValidacionDto
            {
                Id = validacion.Id,
                ArticuloId = validacion.ArticuloConocimientoId,
                Estado = validacion.Estado,
                Aprobado = dto.Aprobado,
                ComentariosValidador = dto.ComentariosValidador,
                FechaValidacion = validacion.FechaValidacion
            };

            return new ApiResponse<ValidacionDto>(true, result, "Validación completada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar artículo");
            return new ApiResponse<ValidacionDto>(false, null, $"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ValidacionDto>>> GetValidacionesPendientesAsync(int? validadorId = null)
    {
        try
        {
            var validaciones = await _validacionRepository.GetValidacionesPendientesAsync(validadorId);
            var dtos = validaciones.Select(v => new ValidacionDto
            {
                Id = v.Id,
                ArticuloId = v.ArticuloConocimientoId,
                ArticuloTitulo = v.ArticuloConocimiento.Titulo,
                SolicitadoPorNombre = $"{v.SolicitadoPor.FirstName} {v.SolicitadoPor.LastName}",
                ValidadorNombre = v.Validador != null ? $"{v.Validador.FirstName} {v.Validador.LastName}" : null,
                Estado = v.Estado,
                CreatedAt = v.CreatedAt
            }).ToList();

            return new ApiResponse<List<ValidacionDto>>(true, dtos, $"{dtos.Count} validaciones pendientes");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener validaciones pendientes");
            return new ApiResponse<List<ValidacionDto>>(false, new List<ValidacionDto>(), "Error");
        }
    }

    public async Task<ApiResponse<EstadisticasConocimientoDto>> GetEstadisticasAsync()
    {
        try
        {
            var articulos = (await _articuloRepository.GetAllAsync()).ToList();
            var validaciones = (await _validacionRepository.GetAllAsync()).ToList();
            var comentarios = (await _comentarioRepository.GetAllAsync()).ToList();

            var stats = new EstadisticasConocimientoDto
            {
                TotalArticulos = articulos.Count,
                ArticulosPublicados = articulos.Count(a => a.Estado == EstadoArticulo.Publicado),
                ArticulosEnRevision = articulos.Count(a => a.Estado == EstadoArticulo.Revision),
                ArticulosBorrador = articulos.Count(a => a.Estado == EstadoArticulo.Borrador),
                TotalEtiquetas = (await _etiquetaRepository.GetAllAsync()).Count(),
                TotalComentarios = comentarios.Count,
                TotalValidaciones = validaciones.Count,
                VisualizacionesTotales = articulos.Sum(a => a.Visualizaciones)
            };

            if (validaciones.Any())
            {
                stats.TasaAprobacion = (validaciones.Count(v => v.Aprobado) * 100.0) / validaciones.Count;
            }

            if (articulos.Any())
            {
                stats.PromedioVotosPositivos = articulos.Average(a => a.VotosPositivos);
            }

            stats.ArticulosMasPopulares = articulos
                .OrderByDescending(a => a.Visualizaciones)
                .Take(10)
                .Select(a => new ArticuloPopularDto
                {
                    Id = a.Id,
                    Titulo = a.Titulo,
                    Visualizaciones = a.Visualizaciones,
                    VotosPositivos = a.VotosPositivos,
                    VecesUtilizado = a.VecesUtilizado
                }).ToList();

            stats.EtiquetasMasUsadas = (await _etiquetaRepository.GetEtiquetasMasUsadasAsync(10))
                .Select(e => new EtiquetaDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    VecesUsada = e.VecesUsada,
                    Color = e.Color
                }).ToList();

            var articulosPorAutor = articulos.GroupBy(a => a.AutorId);
            stats.AutoresMasProductivos = articulosPorAutor
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new AutorProductivoDto
                {
                    UsuarioId = g.Key,
                    Nombre = $"{g.First().Autor.FirstName} {g.First().Autor.LastName}",
                    TotalArticulos = g.Count(),
                    ArticulosPublicados = g.Count(a => a.Estado == EstadoArticulo.Publicado)
                }).ToList();

            return new ApiResponse<EstadisticasConocimientoDto>(true, stats, "Estadísticas generadas correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar estadísticas");
            return new ApiResponse<EstadisticasConocimientoDto>(false, new EstadisticasConocimientoDto(), "Error");
        }
    }

    public async Task<ApiResponse<List<ArticuloPopularDto>>> GetArticulosMasPopularesAsync(int cantidad = 10)
    {
        try
        {
            var articulos = await _articuloRepository.GetArticulosMasPopularesAsync(cantidad);
            var dtos = articulos.Select(a => new ArticuloPopularDto
            {
                Id = a.Id,
                Titulo = a.Titulo,
                Visualizaciones = a.Visualizaciones,
                VotosPositivos = a.VotosPositivos,
                VecesUtilizado = a.VecesUtilizado
            }).ToList();

            return new ApiResponse<List<ArticuloPopularDto>>(true, dtos, $"Top {dtos.Count} artículos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener artículos populares");
            return new ApiResponse<List<ArticuloPopularDto>>(false, new List<ArticuloPopularDto>(), "Error");
        }
    }

    // Métodos auxiliares de mapeo
    private ArticuloConocimientoDto MapToDto(ArticuloConocimiento articulo)
    {
        return new ArticuloConocimientoDto
        {
            Id = articulo.Id,
            Titulo = articulo.Titulo,
            Resumen = articulo.Resumen,
            Contenido = articulo.Contenido,
            Estado = articulo.Estado,
            TipoArticulo = articulo.TipoArticulo,
            Visualizaciones = articulo.Visualizaciones,
            VotosPositivos = articulo.VotosPositivos,
            VotosNegativos = articulo.VotosNegativos,
            EsSolucionValidada = articulo.EsSolucionValidada,
            VersionActual = articulo.VersionActual,
            VecesUtilizado = articulo.VecesUtilizado,
            TasaExito = articulo.TasaExito,
            Tags = articulo.Tags,
            Etiquetas = articulo.ArticulosEtiquetas.Select(ae => new EtiquetaDto
            {
                Id = ae.Etiqueta.Id,
                Nombre = ae.Etiqueta.Nombre,
                Color = ae.Etiqueta.Color
            }).ToList(),
            AutorNombre = $"{articulo.Autor.FirstName} {articulo.Autor.LastName}",
            AutorId = articulo.AutorId,
            RevisadoPorNombre = articulo.RevisadoPor != null ? $"{articulo.RevisadoPor.FirstName} {articulo.RevisadoPor.LastName}" : null,
            CategoriaNombre = articulo.Categoria.Nombre,
            FechaPublicacion = articulo.FechaPublicacion,
            FechaRevision = articulo.FechaRevision,
            CreatedAt = articulo.CreatedAt,
            UpdatedAt = articulo.UpdatedAt
        };
    }

    private ArticuloDetalladoDto MapToDetalladoDto(ArticuloConocimiento articulo)
    {
        var dto = new ArticuloDetalladoDto
        {
            Id = articulo.Id,
            Titulo = articulo.Titulo,
            Resumen = articulo.Resumen,
            Contenido = articulo.Contenido,
            Estado = articulo.Estado,
            TipoArticulo = articulo.TipoArticulo,
            Visualizaciones = articulo.Visualizaciones,
            VotosPositivos = articulo.VotosPositivos,
            VotosNegativos = articulo.VotosNegativos,
            EsSolucionValidada = articulo.EsSolucionValidada,
            VersionActual = articulo.VersionActual,
            VecesUtilizado = articulo.VecesUtilizado,
            TasaExito = articulo.TasaExito,
            Tags = articulo.Tags,
            Etiquetas = articulo.ArticulosEtiquetas.Select(ae => new EtiquetaDto
            {
                Id = ae.Etiqueta.Id,
                Nombre = ae.Etiqueta.Nombre,
                Color = ae.Etiqueta.Color,
                VecesUsada = ae.Etiqueta.VecesUsada
            }).ToList(),
            AutorNombre = $"{articulo.Autor.FirstName} {articulo.Autor.LastName}",
            AutorId = articulo.AutorId,
            RevisadoPorNombre = articulo.RevisadoPor != null ? $"{articulo.RevisadoPor.FirstName} {articulo.RevisadoPor.LastName}" : null,
            CategoriaNombre = articulo.Categoria.Nombre,
            FechaPublicacion = articulo.FechaPublicacion,
            FechaRevision = articulo.FechaRevision,
            CreatedAt = articulo.CreatedAt,
            UpdatedAt = articulo.UpdatedAt,
            PasosDetallados = articulo.PasosDetallados,
            Prerequisites = articulo.Prerequisites,
            Limitaciones = articulo.Limitaciones,
            Versiones = articulo.Versiones.Select(v => new VersionArticuloDto
            {
                Id = v.Id,
                NumeroVersion = v.NumeroVersion,
                Titulo = v.Titulo,
                CambiosRealizados = v.CambiosRealizados,
                ModificadoPorNombre = $"{v.ModificadoPor.FirstName} {v.ModificadoPor.LastName}",
                FechaVersion = v.FechaVersion
            }).ToList(),
            Comentarios = MapComentariosRecursivos(articulo.Comentarios.Where(c => !c.EsRespuesta).ToList()),
            TotalComentarios = articulo.Comentarios.Count,
            IncidentesRelacionados = articulo.IncidentesRelacionados.Select(i => new IncidenteRelacionadoDto
            {
                Id = i.Id,
                NumeroIncidente = i.NumeroIncidente,
                Titulo = i.Titulo,
                Estado = i.Estado,
                FechaReporte = i.FechaReporte
            }).ToList()
        };

        return dto;
    }

    private List<ComentarioArticuloDto> MapComentariosRecursivos(List<ComentarioArticulo> comentarios)
    {
        return comentarios.Select(c => new ComentarioArticuloDto
        {
            Id = c.Id,
            Contenido = c.Contenido,
            UsuarioNombre = $"{c.Usuario.FirstName} {c.Usuario.LastName}",
            UsuarioId = c.UsuarioId,
            CreatedAt = c.CreatedAt,
            EsRespuesta = c.EsRespuesta,
            ComentarioPadreId = c.ComentarioPadreId,
            Respuestas = MapComentariosRecursivos(c.Respuestas.ToList())
        }).ToList();
    }
}
