using Microsoft.EntityFrameworkCore;
using SubastaAutos.Infraestructure.Data;
using SubastaAutos.Infraestructure.Models;
using SubastaAutos.Infraestructure.Repository.Interfaces;

namespace SubastaAutos.Infraestructure.Repository.Implementations
{
    public class RepositorySubasta : IRepositorySubasta
    {
        private readonly SubastaAutosContext _context;

        public RepositorySubasta(SubastaAutosContext context)
        {
            _context = context;
        }

        // ── Subastas Activas (para Index) ────────────────────────────────
        // Filtra por el nombre del estado "Activa" navegando la relación.
        // Include carga las entidades relacionadas que necesita el DTO.
        public async Task<ICollection<Subasta>> ListActivasAsync()
        {
            return await _context.Set<Subasta>()
                .Include(s => s.IdAutoNavigation)              // Para NombreAuto
                    .ThenInclude(a => a.AutoImagen)             // Para ImagenPrincipal del auto
                .Include(s => s.IdVendedorNavigation)          // Para Vendedor
                .Include(s => s.IdEstadoSubastaNavigation)     // Para EstadoSubasta
                .Include(s => s.Puja)                          // Para CantidadPujas (Count)
                .Where(s => s.IdEstadoSubastaNavigation.Nombre == "Activa")
                .OrderByDescending(s => s.FechaCreacion)
                .AsNoTracking()
                .ToListAsync();
        }

        // ── Subastas Finalizadas + Canceladas ────────────────────────────
        // Mismo patrón, pero filtra por los otros dos estados.
        public async Task<ICollection<Subasta>> ListFinalizadasAsync()
        {
            return await _context.Set<Subasta>()
                .Include(s => s.IdAutoNavigation)
                    .ThenInclude(a => a.AutoImagen)
                .Include(s => s.IdVendedorNavigation)
                .Include(s => s.IdEstadoSubastaNavigation)
                .Include(s => s.Puja)
                .Where(s => s.IdEstadoSubastaNavigation.Nombre == "Finalizada"
                         || s.IdEstadoSubastaNavigation.Nombre == "Cancelada")
                .OrderByDescending(s => s.FechaCierre)
                .AsNoTracking()
                .ToListAsync();
        }

        // ── Detalle de una Subasta ───────────────────────────────────────
        // Carga TODO lo necesario para la vista Details:
        // - Auto con sus imágenes
        // - Vendedor
        // - Estado
        // - Pujas con el usuario que pujó (para el historial)
        public async Task<Subasta?> FindByIdAsync(int id)
        {
            return await _context.Set<Subasta>()
                .Where(s => s.IdSubasta == id)
                .Include(s => s.IdAutoNavigation)
                    .ThenInclude(a => a.AutoImagen)
                .Include(s => s.IdVendedorNavigation)
                .Include(s => s.IdEstadoSubastaNavigation)
                .Include(s => s.Puja)
                    .ThenInclude(p => p.IdUsuarioNavigation)   // Nombre del postor
                .FirstOrDefaultAsync();
        }
    }
}
