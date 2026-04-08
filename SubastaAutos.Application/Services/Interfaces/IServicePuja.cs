using SubastaAutos.Application.DTOs;

namespace SubastaAutos.Application.Services.Interfaces
{
    public interface IServicePuja
    {
        Task<ICollection<PujaDTO>> ListBySubastaAsync(int idSubasta);

        Task<PujaDTO?> GetPujaLiderAsync(int idSubasta);
        Task<int> AddAsync(PujaDTO dto, int idUsuarioActual);
        Task<bool> PujaFueSuperadaAsync(int idSubasta, int idUsuario);
    }
}
