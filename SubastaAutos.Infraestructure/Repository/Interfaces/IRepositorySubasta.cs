using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Infraestructure.Repository.Interfaces
{
    public interface IRepositorySubasta
    {
        Task<ICollection<Subasta>> ListAsync();
        Task<ICollection<Subasta>> ListActivasAsync();
        Task<ICollection<Subasta>> ListFinalizadasAsync();
        Task<Subasta?> FindByIdAsync(int id);
        Task<int> AddAsync(Subasta entity);
        Task UpdateAsync(Subasta entity);
        Task UpdateEstadoAsync(int id, int nuevoEstadoId);
        Task<bool> TienePujasAsync(int id);
        Task<bool> HaIniciadoAsync(int id);
        Task<bool> ExisteSubastaActivaParaAutoAsync(int idAuto, int? excluirSubastaId = null);
        Task GuardarResultadoAsync(ResultadoSubasta resultado);
    }
}