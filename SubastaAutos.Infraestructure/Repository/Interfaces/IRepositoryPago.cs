using SubastaAutos.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPago
    {
        Task<Pago?> GetBySubastaAsync(int idSubasta);
        Task<int> AddAsync(Pago entity);
        Task ConfirmarPagoAsync(int idPago);
        Task<bool> ExistePagoParaSubastaAsync(int idSubasta);
        Task<Pago?> GetByIdAsync(int idPago);
    }
}
