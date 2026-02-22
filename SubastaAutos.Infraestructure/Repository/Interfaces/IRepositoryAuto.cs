using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryAuto
    {
        Task<ICollection<Auto>> ListAsync();
        Task<Auto?> FindByIdAsync(int id);
    }
}
