using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Application.Profiles
{
    public class PujaProfile : Profile
    {
        public PujaProfile()
        {
            CreateMap<Puja, PujaDTO>()
         
                .ForMember(
                    dest => dest.NombrePostor,
                    opt => opt.MapFrom(src => src.IdUsuarioNavigation.NombreCompleto)
                );

            // DTO → Entidad (para registrar puja)
            CreateMap<PujaDTO, Puja>()
                .ForMember(d => d.IdPuja, o => o.Ignore())
                .ForMember(d => d.IdUsuarioNavigation, o => o.Ignore())
                .ForMember(d => d.IdSubastaNavigation, o => o.Ignore());
                

        }
    }
}
