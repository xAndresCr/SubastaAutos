using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Application.Profiles
{
    public class SubastaProfile : Profile
    {
        public SubastaProfile()
        {
            CreateMap<Subasta, SubastaDTO>()

                .ForMember(
                    dest => dest.NombreAuto,
                    opt => opt.MapFrom(src =>
                        $"{src.IdAutoNavigation.Marca} {src.IdAutoNavigation.Modelo} {src.IdAutoNavigation.Anio}")
                )

         
                .ForMember(
                    dest => dest.ImagenPrincipalAuto,
                    opt => opt.MapFrom((src, dest) =>
                    {
                        var img = src.IdAutoNavigation.AutoImagen
                                      .FirstOrDefault(i => i.EsPrincipal == true)
                                  ?? src.IdAutoNavigation.AutoImagen.FirstOrDefault();

                        if (img?.Imagen != null && img.Imagen.Length > 0)
                            return $"data:image/jpeg;base64,{Convert.ToBase64String(img.Imagen)}";

                        return string.Empty;
                    })
                )

       
                .ForMember(
                    dest => dest.Vendedor,
                    opt => opt.MapFrom(src => src.IdVendedorNavigation.NombreCompleto)
                )

                .ForMember(
                    dest => dest.EstadoSubasta,
                    opt => opt.MapFrom(src => src.IdEstadoSubastaNavigation.Nombre)
                )

 
                .ForMember(
                    dest => dest.CantidadPujas,
                    opt => opt.MapFrom(src => src.Puja.Count)
                )


                .ForMember(
                    dest => dest.Pujas,
                    opt => opt.MapFrom(src => src.Puja)
                );

   
        }
    }
}
