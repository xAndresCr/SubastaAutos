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

        public async Task<int> AddAsync(Puja entity)
        {
            await _context.Set<Puja>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.IdPuja;
        }

        public async Task<bool> EsLiderAsync(int idSubasta, int idUsuario)
        {
            var lider = await GetPujaLiderAsync(idSubasta);
            return lider?.IdUsuario == idUsuario;
        }

        public Task<List<Puja>> GetBySubastaAsync(int idSubasta)
        {
            throw new NotImplementedException();
        }

        // Obtener la puja más alta de una subasta
        public async Task<Puja?> GetPujaLiderAsync(int idSubasta)
        {
            return await _context.Set<Puja>()
                .Include(p => p.IdUsuarioNavigation)
                .Where (p => p.IdSubasta == idSubasta)
                .OrderByDescending(p => p.Monto)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UsuarioTienePujaAsync(int idSubasta, int idUsuario)
        {
            return await _context.Set<Puja>()
             .AnyAsync(p => p.IdSubasta == idSubasta && p.IdUsuario == idUsuario);

        }

    }
}
