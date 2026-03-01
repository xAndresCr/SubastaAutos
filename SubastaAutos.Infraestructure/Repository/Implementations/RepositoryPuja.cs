using Microsoft.EntityFrameworkCore;
using SubastaAutos.Infraestructure.Data;
using SubastaAutos.Infraestructure.Models;
using SubastaAutos.Infraestructure.Repository.Interfaces;

namespace SubastaAutos.Infraestructure.Repository.Implementations
{
    public class RepositoryPuja : IRepositoryPuja
    {
        private readonly SubastaAutosContext _context;

        public RepositoryPuja(SubastaAutosContext context)
        {
            _context = context;
        }

   
        public async Task<ICollection<Puja>> ListBySubastaAsync(int idSubasta)
        {
            return await _context.Set<Puja>()
                .Where(p => p.IdSubasta == idSubasta)
                .Include(p => p.IdUsuarioNavigation)       // Para NombrePostor
                .OrderByDescending(p => p.FechaHora)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
