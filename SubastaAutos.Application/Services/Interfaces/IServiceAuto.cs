using SubastaAutos.Application.DTOs;

namespace SubastaAutos.Application.Services.Interfaces
{
    public interface IServiceAuto
    {
        Task<ICollection<AutoDTO>> ListAsync();
        Task<AutoDTO?> FindByIdAsync(int id);
        Task<int> AddAsync(AutoDTO dto, string[] selectedCategorias, List<byte[]> imagenes);
        Task UpdateAsync(int id, AutoDTO dto, string[] selectedCategorias, List<byte[]>? nuevasImagenes);
        Task ActivarDesactivarAsync(int id);
        Task EliminarLogicoAsync(int id);
        Task<bool> TieneSubastaActivaAsync(int id);
        Task<bool> TieneSubastaFinalizadaAsync(int id);
    }
}