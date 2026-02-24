using SubastaAutos.Application.DTOs;

namespace SubastaAutos.Application.Services.Interfaces
{
    public interface IServiceSubasta
    {
        Task<ICollection<SubastaDTO>> ListActivasAsync();
        Task<ICollection<SubastaDTO>> ListFinalizadasAsync();
        Task<SubastaDTO?> FindByIdAsync(int id);
    }
}
