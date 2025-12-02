using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.Interfaces;

public interface IConocimientoService
{
    // Artículos
    Task<ApiResponse<List<ArticuloConocimientoDto>>> GetArticulosPublicadosAsync();
    Task<ApiResponse<ArticuloDetalladoDto>> GetArticuloByIdAsync(int id, int? usuarioId = null);
    Task<ApiResponse<ArticuloDetalladoDto>> CreateArticuloAsync(CreateArticuloDto dto, int autorId);
    Task<ApiResponse<ArticuloDetalladoDto>> UpdateArticuloAsync(int id, UpdateArticuloDto dto, int usuarioId);
    Task<ApiResponse<bool>> DeleteArticuloAsync(int id, int usuarioId);
    Task<ApiResponse<bool>> PublicarArticuloAsync(int id, int usuarioId);
    Task<ApiResponse<bool>> ArchivarArticuloAsync(int id, int usuarioId);
    
    // Búsqueda
    Task<ApiResponse<ResultadoBusquedaArticulosDto>> BuscarArticulosAsync(BusquedaArticulosDto dto);
    Task<ApiResponse<List<ArticuloConocimientoDto>>> GetArticulosPorCategoriaAsync(int categoriaId);
    Task<ApiResponse<List<ArticuloConocimientoDto>>> GetArticulosPorAutorAsync(int autorId);
    
    // Etiquetas
    Task<ApiResponse<List<EtiquetaDto>>> GetAllEtiquetasAsync();
    Task<ApiResponse<EtiquetaDto>> CreateEtiquetaAsync(CreateEtiquetaDto dto);
    Task<ApiResponse<List<EtiquetaDto>>> GetEtiquetasMasUsadasAsync(int cantidad = 10);
    
    // Comentarios
    Task<ApiResponse<ComentarioArticuloDto>> AddComentarioAsync(int articuloId, CreateComentarioArticuloDto dto, int usuarioId);
    Task<ApiResponse<bool>> DeleteComentarioAsync(int comentarioId, int usuarioId);
    
    // Votaciones
    Task<ApiResponse<bool>> VotarArticuloAsync(int articuloId, VotarArticuloDto dto, int usuarioId);
    
    // Validaciones
    Task<ApiResponse<ValidacionDto>> SolicitarValidacionAsync(int articuloId, SolicitarValidacionDto dto, int solicitanteId);
    Task<ApiResponse<ValidacionDto>> ValidarArticuloAsync(int validacionId, ValidarArticuloDto dto, int validadorId);
    Task<ApiResponse<List<ValidacionDto>>> GetValidacionesPendientesAsync(int? validadorId = null);
    
    // Estadísticas
    Task<ApiResponse<EstadisticasConocimientoDto>> GetEstadisticasAsync();
    Task<ApiResponse<List<ArticuloPopularDto>>> GetArticulosMasPopularesAsync(int cantidad = 10);
}
