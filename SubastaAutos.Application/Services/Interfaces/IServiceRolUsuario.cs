using SubastaAutos.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.Services.Interfaces
{
    public interface IServiceRolUsuario
    {
        Task<ICollection<RolUsuarioDTO>> ListAsync();
    }
}
