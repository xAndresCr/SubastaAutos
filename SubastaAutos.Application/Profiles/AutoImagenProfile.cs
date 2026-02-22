using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Application.Profiles
{
    public class AutoImagenProfile : Profile
    {
        public AutoImagenProfile()
        {
            CreateMap<AutoImagen, AutoImagenDTO>().ReverseMap();
        }
    }
}
