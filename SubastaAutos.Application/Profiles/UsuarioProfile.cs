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
           
            CreateMap<UsuarioDTO, Usuario>().ReverseMap();

            // Mapea la entidad Usuario hacia UsuarioDTO y asigna la navegación IdRolNavigation a DTO.Rol
            CreateMap<UsuarioDTO, Usuario>()
           .ForMember(dest => dest.IdRolNavigation, ori => ori.Ignore());




        }
    }
}
