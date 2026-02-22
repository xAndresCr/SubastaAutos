using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Application.Profiles
{
    public class EstadoAutoProfile : Profile
    {
        public EstadoAutoProfile()
        {
            CreateMap<EstadoAuto, EstadoAutoDTO>().ReverseMap();
        }
    }
}