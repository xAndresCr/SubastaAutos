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

        // LISTADO GENERAL (admin)
        public async Task<ICollection<Subasta>> ListAsync()
        {
            return await _context.Set<Subasta>()
                .Include(s => s.IdAutoNavigation)
                    .ThenInclude(a => a.AutoImagen)
                .Include(s => s.IdVendedorNavigation)
                .Include(s => s.IdEstadoSubastaNavigation)
                .Include(s => s.Puja)
                .OrderByDescending(s => s.FechaCreacion)
                .AsNoTracking()
                .ToListAsync();
        }

        // LISTADO ACTIVAS (público)
        public async Task<ICollection<Subasta>> ListActivasAsync()
        {
            return await _context.Set<Subasta>()
                .Include(s => s.IdAutoNavigation)
                    .ThenInclude(a => a.AutoImagen)
                .Include(s => s.IdVendedorNavigation)
                .Include(s => s.IdEstadoSubastaNavigation)
                .Include(s => s.Puja)
                .Where(s => s.IdEstadoSubasta == 1) // Activa
                .OrderByDescending(s => s.FechaCreacion)
                .AsNoTracking()
                .ToListAsync();
        }

        // LISTADO FINALIZADAS (público)
        public async Task<ICollection<Subasta>> ListFinalizadasAsync()
        {
            return await _context.Set<Subasta>()
                .Include(s => s.IdAutoNavigation)
                    .ThenInclude(a => a.AutoImagen)
                .Include(s => s.IdVendedorNavigation)
                .Include(s => s.IdEstadoSubastaNavigation)
                .Include(s => s.Puja)
                .Where(s => s.IdEstadoSubasta == 2 || s.IdEstadoSubasta == 3) // Finalizada o Cancelada
                .OrderByDescending(s => s.FechaCierre)
                .AsNoTracking()
                .ToListAsync();
        }

        // DETALLE 
        public async Task<Subasta?> FindByIdAsync(int id)
        {
            return await _context.Set<Subasta>()
                .Where(s => s.IdSubasta == id)
                .Include(s => s.IdAutoNavigation)
                    .ThenInclude(a => a.AutoImagen)
                .Include(s => s.IdVendedorNavigation)
                .Include(s => s.IdEstadoSubastaNavigation)
                .Include(s => s.Puja)
                    .ThenInclude(p => p.IdUsuarioNavigation)
                .FirstOrDefaultAsync();
        }

        //CREAR 
        public async Task<int> AddAsync(Subasta entity)
        {
            await _context.Set<Subasta>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.IdSubasta;
        }

        // EDITAR
        public async Task UpdateAsync(Subasta entity)
        {
            _context.Entry(entity).Property(s => s.FechaInicio).IsModified = true;
            _context.Entry(entity).Property(s => s.FechaCierre).IsModified = true;
            _context.Entry(entity).Property(s => s.PrecioBase).IsModified = true;
            _context.Entry(entity).Property(s => s.IncrementoMinimo).IsModified = true;
            await _context.SaveChangesAsync();
        }

        //CAMBIAR ESTADO 
        public async Task UpdateEstadoAsync(int id, int nuevoEstadoId)
        {
            var entity = await _context.Set<Subasta>().FindAsync(id);
            if (entity == null)
                throw new Exception("Subasta no encontrada.");

            entity.IdEstadoSubasta = nuevoEstadoId;

            // Si se cancela, la fecha de cierre es ahora
            if (nuevoEstadoId == 3) // Cancelada
                entity.FechaCierre = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        // ── VALIDACIONES 
        public async Task<bool> TienePujasAsync(int id)
        {
            return await _context.Set<Puja>()
                .AnyAsync(p => p.IdSubasta == id);
        }

        public async Task<bool> HaIniciadoAsync(int id)
        {
            var entity = await _context.Set<Subasta>().FindAsync(id);
            if (entity == null) return false;
            return entity.FechaInicio <= DateTime.Now;
        }

        public async Task<bool> ExisteSubastaActivaParaAutoAsync(int idAuto, int? excluirSubastaId = null)
        {
            var query = _context.Set<Subasta>()
                .Where(s => s.IdAuto == idAuto && s.IdEstadoSubasta == 1); // Activa

            if (excluirSubastaId.HasValue)
                query = query.Where(s => s.IdSubasta != excluirSubastaId.Value);

            return await query.AnyAsync();
        }
    }
}