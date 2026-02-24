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
                // NombrePostor: navega a Usuario y toma NombreCompleto
                // Mismo patrón que AutoProfile usa para Propietario
                .ForMember(
                    dest => dest.NombrePostor,
                    opt => opt.MapFrom(src => src.IdUsuarioNavigation.NombreCompleto)
                );
            // IdPuja, IdSubasta, Monto y FechaHora se mapean automáticamente
            // porque tienen el mismo nombre en el modelo y en el DTO
        }
    }
}
