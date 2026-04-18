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
    public class PagoProfile: Profile
    {
        public PagoProfile()
        {
            CreateMap<Pago, PagoDTO>()
                 .ForMember(d => d.EstadoPago,
                o => o.MapFrom(s => s.IdEstadoPagoNavigation.Nombre))
                 .ForMember(d => d.NombreAuto,
                 //Esto en caso de que se muestren datos del carro, pero es vara
                   o => o.MapFrom(s =>
                    $"{s.IdSubastaNavigation.IdAutoNavigation.Marca} " +
                    $"{s.IdSubastaNavigation.IdAutoNavigation.Modelo} " +
                    $"{s.IdSubastaNavigation.IdAutoNavigation.Anio}"))

                //Mapea al ganador, en el servicio hace validación 
                .ForMember(d => d.NombreGanador,
                    o => o.MapFrom(s => s.IdSubastaNavigation.ResultadoSubasta.IdUsuarioGanadorNavigation.NombreCompleto));
        }
    }
}
