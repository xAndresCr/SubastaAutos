using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SubastaAutos.Application.Profiles
{
    public class UsuarioProfile: Profile
    {
        public UsuarioProfile()
        {
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(
                    dest => dest.CantSubastasCreadas,
                    opt => opt.MapFrom(src => src.Subasta.Count)
                )
                .ForMember(
                    dest => dest.CantPujasRealizadas,
                    opt => opt.MapFrom(src => src.Puja.Count)
                );

            CreateMap<UsuarioDTO, Usuario>()
                .ForMember(dest => dest.IdRolNavigation, ori => ori.Ignore());

            ///Mapeo para ignorar los otros atributos del DTO 
            CreateMap<UsuarioDTO, Usuario>()
               .ForMember(d => d.IdRolNavigation, o => o.Ignore())
               .ForMember(d => d.PasswordHash, o => o.Ignore())
               .ForMember(d => d.Auto, o => o.Ignore())
               .ForMember(d => d.Puja, o => o.Ignore())
               .ForMember(d => d.Subasta, o => o.Ignore())
               .ForMember(d => d.ResultadoSubasta, o => o.Ignore());

        }
    }
}
