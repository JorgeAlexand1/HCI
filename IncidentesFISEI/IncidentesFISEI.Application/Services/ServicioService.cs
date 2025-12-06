using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Interfaces;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Application.Services;

public class ServicioService : IServicioService
{
    private readonly IServicioRepository _servicioRepository;
    private readonly ICategoriaIncidenteRepository _categoriaRepository;

    public ServicioService(
        IServicioRepository servicioRepository, 
        ICategoriaIncidenteRepository categoriaRepository)
    {
        _servicioRepository = servicioRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IEnumerable<ServicioListDto>> GetAllServiciosAsync()
    {
        var servicios = await _servicioRepository.GetAllAsync();
        return servicios.Select(MapToListDto);
    }

    public async Task<IEnumerable<ServicioListDto>> GetServiciosByCategoriaAsync(int categoriaId)
    {
        var servicios = await _servicioRepository.GetByCategoriaIdAsync(categoriaId);
        return servicios.Select(MapToListDto);
    }

    public async Task<ServicioDto?> GetServicioByIdAsync(int id)
    {
        var servicio = await _servicioRepository.GetByIdAsync(id);
        return servicio == null ? null : MapToDto(servicio);
    }

    public async Task<ServicioDto> CreateServicioAsync(CreateServicioDto createDto)
    {
        // Validar que la categoría existe
        var categoria = await _categoriaRepository.GetByIdAsync(createDto.CategoriaId);
        if (categoria == null)
        {
            throw new ArgumentException("La categoría especificada no existe.");
        }

        // Validar código único si se proporciona
        if (!string.IsNullOrEmpty(createDto.Codigo))
        {
            if (await _servicioRepository.ExistsCodigoAsync(createDto.Codigo))
            {
                throw new ArgumentException("Ya existe un servicio con el mismo código.");
            }
        }

        var servicio = new Servicio
        {
            Nombre = createDto.Nombre,
            Descripcion = createDto.Descripcion,
            Codigo = createDto.Codigo,
            CategoriaId = createDto.CategoriaId,
            ResponsableArea = createDto.ResponsableArea,
            ContactoTecnico = createDto.ContactoTecnico,
            TiempoRespuestaMinutos = createDto.TiempoRespuestaMinutos,
            TiempoResolucionMinutos = createDto.TiempoResolucionMinutos,
            Instrucciones = createDto.Instrucciones,
            EscalacionProcedure = createDto.EscalacionProcedure,
            RequiereAprobacion = createDto.RequiereAprobacion,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _servicioRepository.AddAsync(servicio);
        
        // Recargar con relaciones
        var servicioCreado = await _servicioRepository.GetByIdAsync(servicio.Id);
        return MapToDto(servicioCreado!);
    }

    public async Task<ServicioDto?> UpdateServicioAsync(int id, UpdateServicioDto updateDto)
    {
        var servicio = await _servicioRepository.GetByIdAsync(id);
        if (servicio == null)
        {
            return null;
        }

        // Validar que la categoría existe
        var categoria = await _categoriaRepository.GetByIdAsync(updateDto.CategoriaId);
        if (categoria == null)
        {
            throw new ArgumentException("La categoría especificada no existe.");
        }

        // Validar código único si se proporciona
        if (!string.IsNullOrEmpty(updateDto.Codigo))
        {
            if (await _servicioRepository.ExistsCodigoAsync(updateDto.Codigo, id))
            {
                throw new ArgumentException("Ya existe un servicio con el mismo código.");
            }
        }

        servicio.Nombre = updateDto.Nombre;
        servicio.Descripcion = updateDto.Descripcion;
        servicio.Codigo = updateDto.Codigo;
        servicio.CategoriaId = updateDto.CategoriaId;
        servicio.ResponsableArea = updateDto.ResponsableArea;
        servicio.ContactoTecnico = updateDto.ContactoTecnico;
        servicio.TiempoRespuestaMinutos = updateDto.TiempoRespuestaMinutos;
        servicio.TiempoResolucionMinutos = updateDto.TiempoResolucionMinutos;
        servicio.Instrucciones = updateDto.Instrucciones;
        servicio.EscalacionProcedure = updateDto.EscalacionProcedure;
        servicio.RequiereAprobacion = updateDto.RequiereAprobacion;
        servicio.IsActive = updateDto.IsActive;
        servicio.UpdatedAt = DateTime.UtcNow;

        await _servicioRepository.UpdateAsync(servicio);
        
        // Recargar con relaciones
        var servicioActualizado = await _servicioRepository.GetByIdAsync(id);
        return MapToDto(servicioActualizado!);
    }

    public async Task<bool> DeleteServicioAsync(int id)
    {
        var servicio = await _servicioRepository.GetByIdAsync(id);
        if (servicio == null)
        {
            return false;
        }

        // Verificar si tiene incidentes asociados
        if (servicio.Incidentes.Any())
        {
            throw new InvalidOperationException("No se puede eliminar un servicio que tiene incidentes asociados.");
        }

        await _servicioRepository.DeleteAsync(servicio);
        return true;
    }

    public async Task<IEnumerable<ServicioListDto>> SearchServiciosAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllServiciosAsync();
        }

        var servicios = await _servicioRepository.SearchServiciosAsync(searchTerm);
        return servicios.Select(MapToListDto);
    }

    public async Task<bool> ExistsCodigoAsync(string codigo, int? excludeId = null)
    {
        return await _servicioRepository.ExistsCodigoAsync(codigo, excludeId);
    }

    private static ServicioDto MapToDto(Servicio servicio)
    {
        return new ServicioDto
        {
            Id = servicio.Id,
            Nombre = servicio.Nombre,
            Descripcion = servicio.Descripcion,
            Codigo = servicio.Codigo,
            IsActive = servicio.IsActive,
            CategoriaId = servicio.CategoriaId,
            CategoriaNombre = servicio.Categoria?.Nombre,
            ResponsableArea = servicio.ResponsableArea,
            ContactoTecnico = servicio.ContactoTecnico,
            TiempoRespuestaMinutos = servicio.TiempoRespuestaMinutos,
            TiempoResolucionMinutos = servicio.TiempoResolucionMinutos,
            Instrucciones = servicio.Instrucciones,
            EscalacionProcedure = servicio.EscalacionProcedure,
            RequiereAprobacion = servicio.RequiereAprobacion,
            CreatedAt = servicio.CreatedAt,
            UpdatedAt = servicio.UpdatedAt ?? servicio.CreatedAt
        };
    }

    private static ServicioListDto MapToListDto(Servicio servicio)
    {
        return new ServicioListDto
        {
            Id = servicio.Id,
            Nombre = servicio.Nombre,
            Descripcion = servicio.Descripcion,
            Codigo = servicio.Codigo,
            IsActive = servicio.IsActive,
            CategoriaId = servicio.CategoriaId,
            CategoriaNombre = servicio.Categoria?.Nombre ?? "Sin categoría",
            ResponsableArea = servicio.ResponsableArea,
            IncidentesActivos = servicio.Incidentes?.Count(i => i.Estado != EstadoIncidente.Cerrado) ?? 0,
            TotalIncidentes = servicio.Incidentes?.Count() ?? 0
        };
    }
}