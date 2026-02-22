using Microsoft.EntityFrameworkCore;
using SubastaAutos.Infraestructure.Data;
using SubastaAutos.Infraestructure.Models;
using SubastaAutos.Infraestructure.Repository.Interfaces;

namespace SubastaAutos.Infraestructure.Repository.Implementations
{
    public class RepositoryAuto : IRepositoryAuto
    {
        private readonly SubastaAutosContext _context;

        public RepositoryAuto(SubastaAutosContext context)
        {
            _context = context;
        }

        // ── MÉTODO 1: Para el listado ─────────────────────────────────────────
        public async Task<ICollection<Auto>> ListAsync()
        {
            return await _context.Set<Auto>()
                .Include(a => a.AutoImagen)              // Para ImagenPrincipalUrl (calculado)
                .Include(a => a.IdCondicionAutoNavigation) // Para Condicion
                .Include(a => a.IdEstadoAutoNavigation)   // Para EstadoAuto
                .Include(a => a.IdVendedorNavigation)     // Para Propietario
                .OrderBy(a => a.Marca)
                .ThenBy(a => a.Modelo)
                .AsNoTracking()
                .ToListAsync();
        }

        // ── MÉTODO 2: Para el detalle ─────────────────────────────────────────
        public async Task<Auto?> FindByIdAsync(int id)
        {
            return await _context.Set<Auto>()
                .Where(a => a.IdAuto == id)
                .Include(a => a.AutoImagen)
                .Include(a => a.IdCategoria)
                .Include(a => a.IdCondicionAutoNavigation)
                .Include(a => a.IdEstadoAutoNavigation)
                .Include(a => a.IdVendedorNavigation)
                .Include(a => a.Subasta)
                .ThenInclude(s => s.IdEstadoSubastaNavigation)
                .FirstOrDefaultAsync();
        }
    }
}