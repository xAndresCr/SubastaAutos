using SubastaAutos.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryRolUsuario
    {
        Task<ICollection<RolUsuario>> ListAsync();
    }
}
