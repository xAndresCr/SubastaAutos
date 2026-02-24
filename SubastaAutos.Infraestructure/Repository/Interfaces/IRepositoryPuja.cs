using SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPuja
    {
        // Devuelve todas las pujas de una subasta específica,
        // ordenadas de la más reciente a la más antigua
        Task<ICollection<Puja>> ListBySubastaAsync(int idSubasta);
    }
}
