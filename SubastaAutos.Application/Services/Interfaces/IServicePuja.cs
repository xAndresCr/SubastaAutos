using SubastaAutos.Application.DTOs;

namespace SubastaAutos.Application.Services.Interfaces
{
    public interface IServicePuja
    {
        Task<ICollection<PujaDTO>> ListBySubastaAsync(int idSubasta);
    }
}
