using AutoMapper;
using IncidentesFISEI.Application.DTOs;
using IncidentesFISEI.Domain.Entities;

namespace IncidentesFISEI.Application.Mappings;

public class ArticuloConocimientoMappingProfile : Profile
{
    public ArticuloConocimientoMappingProfile()
    {
        // Mapeo de ArticuloConocimiento a ArticuloConocimientoDto
        CreateMap<ArticuloConocimiento, ArticuloConocimientoDto>()
            .ForMember(dest => dest.AutorNombre, 
                opt => opt.MapFrom(src => src.Autor != null ? $"{src.Autor.FirstName} {src.Autor.LastName}" : ""))
            .ForMember(dest => dest.RevisadoPorNombre, 
                opt => opt.MapFrom(src => src.RevisadoPor != null ? $"{src.RevisadoPor.FirstName} {src.RevisadoPor.LastName}" : null))
            .ForMember(dest => dest.CategoriaNombre, 
                opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : ""));

        // Mapeo de CreateArticuloConocimientoDto a ArticuloConocimiento
        CreateMap<CreateArticuloConocimientoDto, ArticuloConocimiento>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) => ParseTags(src.Tags)))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(_ => Domain.Enums.EstadoArticulo.Revision))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Mapeo de UpdateArticuloConocimientoDto a ArticuloConocimiento
        CreateMap<UpdateArticuloConocimientoDto, ArticuloConocimiento>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) => ParseTags(src.Tags)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }

    private static string[] ParseTags(string tags)
    {
        if (string.IsNullOrEmpty(tags))
            return Array.Empty<string>();
        
        return tags.Split(',').Select(t => t.Trim()).ToArray();
    }
}
