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
        Task<int> AddAsync(Auto entity, string[] selectedCategorias);
        Task UpdateAsync(Auto entity, string[] selectedCategorias);
        Task UpdateEstadoAsync(int id, int nuevoEstadoId);
        Task<bool> TieneSubastasAsync(int id);
        Task<bool> TieneSubastaActivaAsync(int id);
        Task<bool> TieneSubastaFinalizadaAsync(int id);
    }
}