using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IncidentesFISEI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServicioDITICController : ControllerBase
    {
        private readonly IServicioDITICRepository _servicioRepository;
        private readonly ILogger<ServicioDITICController> _logger;

        public ServicioDITICController(
            IServicioDITICRepository servicioRepository,
            ILogger<ServicioDITICController> logger)
        {
            _servicioRepository = servicioRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los servicios DITIC activos
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetServiciosActivos()
        {
            var servicios = await _servicioRepository.GetServiciosActivosAsync();
            var serviciosDto = servicios.Select(MapToDto);
            
            return Ok(new ApiResponse<IEnumerable<ServicioDITICDto>>(true, serviciosDto, 
                $"Se encontraron {serviciosDto.Count()} servicios"));
        }

        /// <summary>
        /// Obtiene un servicio específico por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServicioById(int id)
        {
            var servicio = await _servicioRepository.GetByIdAsync(id);
            
            if (servicio == null || servicio.IsDeleted)
                return NotFound(new ApiResponse<ServicioDITICDto>(false, null, "Servicio no encontrado"));

            return Ok(new ApiResponse<ServicioDITICDto>(true, MapToDto(servicio), "Servicio encontrado"));
        }

        /// <summary>
        /// Obtiene un servicio por su código
        /// </summary>
        [HttpGet("codigo/{codigo}")]
        public async Task<IActionResult> GetServicioPorCodigo(string codigo)
        {
            var servicio = await _servicioRepository.GetServicioPorCodigoAsync(codigo);
            
            if (servicio == null)
                return NotFound(new ApiResponse<ServicioDITICDto>(false, null, 
                    $"Servicio con código {codigo} no encontrado"));

            return Ok(new ApiResponse<ServicioDITICDto>(true, MapToDto(servicio), "Servicio encontrado"));
        }

        /// <summary>
        /// Obtiene servicios por tipo
        /// </summary>
        [HttpGet("tipo/{tipoServicio}")]
        public async Task<IActionResult> GetServiciosPorTipo(int tipoServicio)
        {
            var servicios = await _servicioRepository.GetServiciosPorTipoAsync(tipoServicio);
            var serviciosDto = servicios.Select(MapToDto);
            
            return Ok(new ApiResponse<IEnumerable<ServicioDITICDto>>(true, serviciosDto, 
                $"Se encontraron {serviciosDto.Count()} servicios"));
        }

        /// <summary>
        /// Obtiene solo servicios esenciales
        /// </summary>
        [HttpGet("esenciales")]
        public async Task<IActionResult> GetServiciosEsenciales()
        {
            var servicios = await _servicioRepository.GetServiciosEsencialesAsync();
            var serviciosDto = servicios.Select(MapToDto);
            
            return Ok(new ApiResponse<IEnumerable<ServicioDITICDto>>(true, serviciosDto, 
                $"Se encontraron {serviciosDto.Count()} servicios esenciales"));
        }

        /// <summary>
        /// Crea un nuevo servicio DITIC
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador,Supervisor")]
        public async Task<IActionResult> CreateServicio([FromBody] CreateServicioDITICDto dto)
        {
            // Verificar que el código no exista
            var servicioExistente = await _servicioRepository.GetServicioPorCodigoAsync(dto.Codigo);
            if (servicioExistente != null)
            {
                return BadRequest(new ApiResponse<ServicioDITICDto>(false, null, 
                    $"Ya existe un servicio con el código {dto.Codigo}"));
            }

            var servicio = new ServicioDITIC
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                DescripcionDetallada = dto.DescripcionDetallada,
                TipoServicio = dto.TipoServicio,
                EsServicioEsencial = dto.EsServicioEsencial,
                HorarioDisponibilidad = dto.HorarioDisponibilidad,
                PorcentajeDisponibilidad = dto.PorcentajeDisponibilidad,
                SLAId = dto.SLAId,
                CategoriaId = dto.CategoriaId,
                ResponsableTecnicoId = dto.ResponsableTecnicoId,
                ResponsableNegocioId = dto.ResponsableNegocioId,
                Requisitos = dto.Requisitos,
                Limitaciones = dto.Limitaciones,
                DocumentacionURL = dto.DocumentacionURL,
                CostoEstimado = dto.CostoEstimado,
                UnidadCosto = dto.UnidadCosto,
                EstaActivo = true,
                FechaInicioServicio = DateTime.UtcNow
            };

            await _servicioRepository.AddAsync(servicio);
            await _servicioRepository.SaveChangesAsync();

            _logger.LogInformation("Servicio DITIC creado: {Codigo} - {Nombre}", servicio.Codigo, servicio.Nombre);

            return CreatedAtAction(nameof(GetServicioById), new { id = servicio.Id }, 
                new ApiResponse<ServicioDITICDto>(true, MapToDto(servicio), "Servicio creado exitosamente"));
        }

        /// <summary>
        /// Actualiza un servicio existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Supervisor")]
        public async Task<IActionResult> UpdateServicio(int id, [FromBody] UpdateServicioDITICDto dto)
        {
            var servicio = await _servicioRepository.GetByIdAsync(id);
            
            if (servicio == null || servicio.IsDeleted)
                return NotFound(new ApiResponse<ServicioDITICDto>(false, null, "Servicio no encontrado"));

            // Actualizar solo campos proporcionados
            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                servicio.Nombre = dto.Nombre;
            
            if (!string.IsNullOrWhiteSpace(dto.Descripcion))
                servicio.Descripcion = dto.Descripcion;
            
            if (dto.DescripcionDetallada != null)
                servicio.DescripcionDetallada = dto.DescripcionDetallada;
            
            if (dto.EsServicioEsencial.HasValue)
                servicio.EsServicioEsencial = dto.EsServicioEsencial.Value;
            
            if (dto.HorarioDisponibilidad != null)
                servicio.HorarioDisponibilidad = dto.HorarioDisponibilidad;
            
            if (dto.PorcentajeDisponibilidad.HasValue)
                servicio.PorcentajeDisponibilidad = dto.PorcentajeDisponibilidad.Value;
            
            if (dto.SLAId.HasValue)
                servicio.SLAId = dto.SLAId.Value;
            
            if (dto.ResponsableTecnicoId.HasValue)
                servicio.ResponsableTecnicoId = dto.ResponsableTecnicoId.Value;
            
            if (dto.Limitaciones != null)
                servicio.Limitaciones = dto.Limitaciones;
            
            if (dto.EstaActivo.HasValue)
                servicio.EstaActivo = dto.EstaActivo.Value;

            servicio.UpdatedAt = DateTime.UtcNow;

            await _servicioRepository.UpdateAsync(servicio);
            await _servicioRepository.SaveChangesAsync();

            _logger.LogInformation("Servicio DITIC actualizado: {Codigo} - {Nombre}", servicio.Codigo, servicio.Nombre);

            return Ok(new ApiResponse<ServicioDITICDto>(true, MapToDto(servicio), "Servicio actualizado exitosamente"));
        }

        /// <summary>
        /// Desactiva un servicio (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteServicio(int id)
        {
            var servicio = await _servicioRepository.GetByIdAsync(id);
            
            if (servicio == null || servicio.IsDeleted)
                return NotFound(new ApiResponse<bool>(false, false, "Servicio no encontrado"));

            servicio.EstaActivo = false;
            servicio.IsDeleted = true;
            servicio.UpdatedAt = DateTime.UtcNow;
            servicio.FechaFinServicio = DateTime.UtcNow;

            await _servicioRepository.UpdateAsync(servicio);
            await _servicioRepository.SaveChangesAsync();

            _logger.LogWarning("Servicio DITIC desactivado: {Codigo} - {Nombre}", servicio.Codigo, servicio.Nombre);

            return Ok(new ApiResponse<bool>(true, true, "Servicio desactivado exitosamente"));
        }

        // Método auxiliar para mapear entidad a DTO
        private ServicioDITICDto MapToDto(ServicioDITIC servicio)
        {
            return new ServicioDITICDto
            {
                Id = servicio.Id,
                Codigo = servicio.Codigo,
                Nombre = servicio.Nombre,
                Descripcion = servicio.Descripcion,
                DescripcionDetallada = servicio.DescripcionDetallada,
                TipoServicio = servicio.TipoServicio.ToString(),
                EsServicioEsencial = servicio.EsServicioEsencial,
                HorarioDisponibilidad = servicio.HorarioDisponibilidad,
                PorcentajeDisponibilidad = servicio.PorcentajeDisponibilidad,
                SLANombre = servicio.SLA?.Nombre,
                CategoriaNombre = servicio.Categoria?.Nombre,
                ResponsableTecnicoNombre = servicio.ResponsableTecnico != null 
                    ? $"{servicio.ResponsableTecnico.FirstName} {servicio.ResponsableTecnico.LastName}" 
                    : null,
                ResponsableNegocioNombre = servicio.ResponsableNegocio != null 
                    ? $"{servicio.ResponsableNegocio.FirstName} {servicio.ResponsableNegocio.LastName}" 
                    : null,
                Requisitos = servicio.Requisitos,
                Limitaciones = servicio.Limitaciones,
                DocumentacionURL = servicio.DocumentacionURL,
                CostoEstimado = servicio.CostoEstimado,
                UnidadCosto = servicio.UnidadCosto,
                EstaActivo = servicio.EstaActivo,
                FechaInicioServicio = servicio.FechaInicioServicio,
                CreatedAt = servicio.CreatedAt
            };
        }
    }
}
