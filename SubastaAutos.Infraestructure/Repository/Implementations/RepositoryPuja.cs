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

        // ── Pujas de una subasta ─────────────────────────────────────────
        // Filtra por IdSubasta e incluye el Usuario para mostrar
        // el nombre del postor en la vista.
        // Orden descendente: la puja más reciente (mayor monto) primero.
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
