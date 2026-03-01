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
   
        }
    }
}
