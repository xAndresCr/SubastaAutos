using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Application.Profiles
{
    public class AutoProfile : Profile
    {
        public AutoProfile()
        {
            // ── Mapping 1: Subasta → SubastaResumenDTO ──────────────────────
            // Se define PRIMERO porque AutoProfile lo necesita internamente
            // cuando mapea la lista Auto.Subasta
            CreateMap<Subasta, SubastaResumenDTO>()
                      .ForMember(
                          dest => dest.EstadoSubasta,
                          opt => opt.MapFrom(src => src.IdEstadoSubastaNavigation.Nombre)
                      );
            // dest.EstadoSubasta (string en el DTO) viene de
            // src.IdEstadoSubastaNavigation.Nombre (navegando la relación)

            // ── Mapping 2: Auto → AutoDTO ────────────────────────────────────
            CreateMap<Auto, AutoDTO>()
                // NombreAuto: concatenación de Marca + Modelo + Año
                .ForMember(
                    dest => dest.NombreAuto,
                    opt => opt.MapFrom(src => $"{src.Marca} {src.Modelo} {src.Anio}")
                )
                // Propietario: viene de la navegación al Usuario dueño
                .ForMember(
                    dest => dest.Propietario,
                    opt => opt.MapFrom(src => src.IdVendedorNavigation.NombreCompleto)
                )
                // Condicion: viene de la navegación a CondicionAuto
                .ForMember(
                    dest => dest.Condicion,
                    opt => opt.MapFrom(src => src.IdCondicionAutoNavigation.Nombre)
                )
                // EstadoAuto: viene de la navegación a EstadoAuto
                .ForMember(
                    dest => dest.EstadoAuto,
                    opt => opt.MapFrom(src => src.IdEstadoAutoNavigation.Nombre)
                )
                // ImagenPrincipalUrl: CAMPO CALCULADO con LINQ
                // Busca la imagen con EsPrincipal=true.
                // Si no hay ninguna marcada, toma la primera de la lista.
                // Si no hay imágenes, devuelve string vacío.
                .ForMember(
                    dest => dest.ImagenPrincipalUrl,
                    opt => opt.MapFrom((src, dest) =>
                        src.AutoImagen.FirstOrDefault(i => i.EsPrincipal == true) != null
                            ? src.AutoImagen.First(i => i.EsPrincipal == true).UrlImagen?.ToString() ?? string.Empty
                            : src.AutoImagen.FirstOrDefault() != null
                                ? src.AutoImagen.First().UrlImagen?.ToString() ?? string.Empty
                                : string.Empty)
)
                // IdCategoria: AutoMapper mapea la lista usando CategoriaProfile
                .ForMember(
                    dest => dest.IdCategoria,
                    opt => opt.MapFrom(src => src.IdCategoria)
                )
                // AutoImagen: AutoMapper mapea la lista usando AutoImagenProfile
                .ForMember(
                    dest => dest.AutoImagen,
                    opt => opt.MapFrom(src => src.AutoImagen)
                )
                // Subasta: AutoMapper mapea la lista usando el Mapping 1 de arriba
                .ForMember(
                    dest => dest.Subasta,
                    opt => opt.MapFrom(src => src.Subasta)
                );
        }
    }
}
