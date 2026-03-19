using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Application.Profiles
{
    public class AutoProfile : Profile
    {
        public AutoProfile()
        {
            // Subasta → SubastaResumenDTO (se usa dentro de AutoDTO)
            CreateMap<Subasta, SubastaResumenDTO>()
                .ForMember(d => d.EstadoSubasta,
                    o => o.MapFrom(s => s.IdEstadoSubastaNavigation.Nombre));

            // ── ENTIDAD → DTO (lectura) ──────────────────────────
            CreateMap<Auto, AutoDTO>()
                .ForMember(d => d.NombreAuto,
                    o => o.MapFrom(s => $"{s.Marca} {s.Modelo} {s.Anio}"))
                .ForMember(d => d.Propietario,
                    o => o.MapFrom(s => s.IdVendedorNavigation.NombreCompleto))
                .ForMember(d => d.Condicion,
                    o => o.MapFrom(s => s.IdCondicionAutoNavigation.Nombre))
                .ForMember(d => d.EstadoAuto,
                    o => o.MapFrom(s => s.IdEstadoAutoNavigation.Nombre))
                .ForMember(d => d.ImagenPrincipal,
                    o => o.MapFrom((s, d) =>
                    {
                        var img = s.AutoImagen.FirstOrDefault(i => i.EsPrincipal == true)
                                  ?? s.AutoImagen.FirstOrDefault();
                        if (img?.Imagen != null && img.Imagen.Length > 0)
                            return $"data:image/jpeg;base64,{Convert.ToBase64String(img.Imagen)}";
                        return string.Empty;
                    }))
                .ForMember(d => d.IdCategoria, o => o.MapFrom(s => s.IdCategoria))
                .ForMember(d => d.AutoImagen, o => o.MapFrom(s => s.AutoImagen))
                .ForMember(d => d.Subasta, o => o.MapFrom(s => s.Subasta));

            // ── DTO → ENTIDAD (crear/editar) ─────────────────────
            CreateMap<AutoDTO, Auto>()
                .ForMember(d => d.IdCondicionAutoNavigation, o => o.Ignore())
                .ForMember(d => d.IdEstadoAutoNavigation, o => o.Ignore())
                .ForMember(d => d.IdVendedorNavigation, o => o.Ignore())
                .ForMember(d => d.IdCategoria, o => o.Ignore())
                .ForMember(d => d.AutoImagen, o => o.Ignore())
                .ForMember(d => d.Subasta, o => o.Ignore());
        }
    }
}