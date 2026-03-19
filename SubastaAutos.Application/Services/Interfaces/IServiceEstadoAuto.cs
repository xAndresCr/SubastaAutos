using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaAutos.Application.DTOs;

namespace SubastaAutos.Application.Services.Interfaces
{
    public interface IServiceEstadoAuto
    {
        Task<ICollection<EstadoAutoDTO>> ListAsync();
    }
}