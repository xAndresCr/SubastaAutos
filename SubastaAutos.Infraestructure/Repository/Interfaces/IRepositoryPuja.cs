using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPuja
    {

        Task<ICollection<Puja>> ListBySubastaAsync(int idSubasta);
    }
}
