using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Application.Profiles
{
    public class AutoProfile : Profile
    {
        public AutoProfile()
        {

            CreateMap<Subasta, SubastaResumenDTO>()
                      .ForMember(
                          dest => dest.EstadoSubasta,
                          opt => opt.MapFrom(src => src.IdEstadoSubastaNavigation.Nombre)
                      );
      
            CreateMap<Auto, AutoDTO>()
           
                .ForMember(
                    dest => dest.NombreAuto,
                    opt => opt.MapFrom(src => $"{src.Marca} {src.Modelo} {src.Anio}")
                )

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
       
                .ForMember(
                    dest => dest.ImagenPrincipal,
                    opt => opt.MapFrom((src, dest) =>
                    {
                        var img = src.AutoImagen.FirstOrDefault(i => i.EsPrincipal == true)
                                  ?? src.AutoImagen.FirstOrDefault();

                        if (img?.Imagen != null && img.Imagen.Length > 0)
                            return $"data:image/jpeg;base64,{Convert.ToBase64String(img.Imagen)}";

                        return string.Empty;
                    })
)

                .ForMember(
                    dest => dest.IdCategoria,
                    opt => opt.MapFrom(src => src.IdCategoria)
                )
        
                .ForMember(
                    dest => dest.AutoImagen,
                    opt => opt.MapFrom(src => src.AutoImagen)
                )
      
                .ForMember(
                    dest => dest.Subasta,
                    opt => opt.MapFrom(src => src.Subasta)
                );
        }
    }
}
