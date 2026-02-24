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

                // NombreAuto: concatena Marca + Modelo + Año del auto relacionado
                // Navega: Subasta → IdAutoNavigation (Auto) → Marca/Modelo/Anio
                .ForMember(
                    dest => dest.NombreAuto,
                    opt => opt.MapFrom(src =>
                        $"{src.IdAutoNavigation.Marca} {src.IdAutoNavigation.Modelo} {src.IdAutoNavigation.Anio}")
                )

                // ImagenPrincipalAuto: busca la imagen principal del auto
                // Mismo patrón que AutoProfile usa para ImagenPrincipal
                // Navega: Subasta → IdAutoNavigation → AutoImagen (colección)
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

                // Vendedor: nombre completo del usuario vendedor
                // Navega: Subasta → IdVendedorNavigation (Usuario) → NombreCompleto
                .ForMember(
                    dest => dest.Vendedor,
                    opt => opt.MapFrom(src => src.IdVendedorNavigation.NombreCompleto)
                )

                // EstadoSubasta: nombre del estado
                // Navega: Subasta → IdEstadoSubastaNavigation → Nombre
                .ForMember(
                    dest => dest.EstadoSubasta,
                    opt => opt.MapFrom(src => src.IdEstadoSubastaNavigation.Nombre)
                )

                // CantidadPujas: CAMPO CALCULADO con LINQ .Count()
                // NO se almacena en BD. Se calcula en tiempo de ejecución
                // contando los elementos de la colección Puja que EF Core cargó con Include.
                .ForMember(
                    dest => dest.CantidadPujas,
                    opt => opt.MapFrom(src => src.Puja.Count)
                )

                // Pujas: mapea la colección de Puja a List<PujaDTO>
                // AutoMapper usa PujaProfile internamente para cada elemento
                .ForMember(
                    dest => dest.Pujas,
                    opt => opt.MapFrom(src => src.Puja)
                );

            // FechaInicio, FechaCierre, PrecioBase, IncrementoMinimo y FechaCreacion
            // se mapean automáticamente porque comparten nombre entre modelo y DTO.
        }
    }
}
