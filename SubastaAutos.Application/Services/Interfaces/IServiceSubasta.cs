using SubastaAutos.Application.DTOs;

namespace SubastaAutos.Application.Services.Interfaces
{
    public interface IServiceSubasta
    {
        Task<ICollection<SubastaDTO>> ListAsync();
        Task<ICollection<SubastaDTO>> ListActivasAsync();
        Task<ICollection<SubastaDTO>> ListFinalizadasAsync();
        Task<SubastaDTO?> FindByIdAsync(int id);
        Task<int> AddAsync(SubastaDTO dto);
        Task UpdateAsync(int id, SubastaDTO dto);
        Task PublicarAsync(int id);
        Task CancelarAsync(int id);
        Task<bool> PuedeEditarAsync(int id);
        Task CerrarAsync(int id);
    }
}