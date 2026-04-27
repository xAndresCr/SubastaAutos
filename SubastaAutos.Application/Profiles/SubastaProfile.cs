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
                .ForMember(d => d.NombreAuto,
                    o => o.MapFrom(s =>
                        $"{s.IdAutoNavigation.Marca} {s.IdAutoNavigation.Modelo} {s.IdAutoNavigation.Anio}"))
                .ForMember(d => d.ImagenPrincipalAuto,
                    o => o.MapFrom((s, d) =>
                    {
                        var img = s.IdAutoNavigation.AutoImagen
                                      .FirstOrDefault(i => i.EsPrincipal == true)
                                  ?? s.IdAutoNavigation.AutoImagen.FirstOrDefault();
                        if (img?.Imagen != null && img.Imagen.Length > 0)
                            return $"data:image/jpeg;base64,{Convert.ToBase64String(img.Imagen)}";
                        return string.Empty;
                    }))
                .ForMember(d => d.Vendedor,
                    o => o.MapFrom(s => s.IdVendedorNavigation.NombreCompleto))
                .ForMember(d => d.DescripcionAuto,
                    o => o.MapFrom(s => s.IdAutoNavigation.Descripcion ?? string.Empty))
                .ForMember(d => d.AutoImagenes,
                    o => o.MapFrom(s => s.IdAutoNavigation.AutoImagen))
                .ForMember(d => d.EstadoSubasta,
                    o => o.MapFrom(s => s.IdEstadoSubastaNavigation.Nombre))
                .ForMember(d => d.CantidadPujas,
                    o => o.MapFrom(s => s.Puja.Count))
                .ForMember(d => d.MontoFinal,
                    o => o.MapFrom(s => s.ResultadoSubasta != null
                        ?        s.ResultadoSubasta.MontoFinal
                                                : 0))
                .ForMember(d => d.Pujas,
                    o => o.MapFrom(s => s.Puja

        .OrderByDescending(p => p.Monto) // ← agregar esto
        .ToList()));

            // ── DTO → ENTIDAD (crear/editar) ────────────────────
            CreateMap<SubastaDTO, Subasta>()
                .ForMember(d => d.IdAutoNavigation, o => o.Ignore())
                .ForMember(d => d.IdVendedorNavigation, o => o.Ignore())
                .ForMember(d => d.IdEstadoSubastaNavigation, o => o.Ignore())
                .ForMember(d => d.Puja, o => o.Ignore())
                .ForMember(d => d.Pago, o => o.Ignore())
                .ForMember(d => d.ResultadoSubasta, o => o.Ignore());
        }
    }
}