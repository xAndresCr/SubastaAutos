using SubastaAutos.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.Services.Interfaces
{
    public interface IServiceAuto
    {
        Task<ICollection<AutoDTO>> ListAsync();
        Task<AutoDTO?> FindByIdAsync(int id);
    }
}
