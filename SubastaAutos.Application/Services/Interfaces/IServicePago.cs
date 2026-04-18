using SubastaAutos.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.Services.Interfaces
{
    public  interface IServicePago
    {
        Task<PagoDTO?> GetBySubastaAsync(int idSubasta);
        Task<int> RegistrarPagoAsync(int idSubasta);
        Task ConfirmarPagoAsync(int idPago);
    }
}
