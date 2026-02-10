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
    public  class RolUsuarioProfile: Profile
    {
        public RolUsuarioProfile()
        {
            CreateMap<RolUsuario, RolUsuarioDTO>();
        }
    }
}
