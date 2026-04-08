using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPuja
    {

        Task<ICollection<Puja>> ListBySubastaAsync(int idSubasta);
        Task<int> AddAsync(Puja entity);
        Task<List<Puja>> GetBySubastaAsync(int idSubasta);
        Task<Puja?> GetPujaLiderAsync(int idSubasta);
        Task<bool> UsuarioTienePujaAsync(int idSubasta, int idUsuario);
        Task<bool> EsLiderAsync(int idSubasta, int idUsuario);
    }
}
