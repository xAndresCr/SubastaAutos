using SubastaAutos.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Infraestructure.Repository.Interfaces
{
    public interface IRepositorySubasta
    {
        Task<ICollection<Subasta>> ListActivasAsync();

        Task<ICollection<Subasta>> ListFinalizadasAsync();

        Task<Subasta?> FindByIdAsync(int id);
    }
}
