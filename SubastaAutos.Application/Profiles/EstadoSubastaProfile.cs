using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Application.Profiles
{
    public class EstadoSubastaProfile : Profile
    {
        public EstadoSubastaProfile()
        {
            // Mapeo directo: ambas clases tienen las mismas propiedades
            CreateMap<EstadoSubasta, EstadoSubastaDTO>();
        }
    }
}
