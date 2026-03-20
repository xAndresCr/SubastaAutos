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

        // ── LISTADO ──────────────────────────────────────────────
        public async Task<ICollection<Auto>> ListAsync()
        {
            return await _context.Set<Auto>()
                .Include(a => a.AutoImagen)
                .Include(a => a.IdCondicionAutoNavigation)
                .Include(a => a.IdEstadoAutoNavigation)
                .Include(a => a.IdVendedorNavigation)
                .OrderBy(a => a.Marca)
                .ThenBy(a => a.Modelo)
                .AsNoTracking()
                .ToListAsync();
        }

        // ── DETALLE ──────────────────────────────────────────────
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

        // ── CREAR ────────────────────────────────────────────────
        public async Task<int> AddAsync(Auto entity, string[] selectedCategorias)
        {
            await ApplyCategoriasAsync(entity, selectedCategorias);
            await _context.Set<Auto>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.IdAuto;
        }

        // ── EDITAR ───────────────────────────────────────────
        public async Task UpdateAsync(Auto entity, string[] selectedCategorias)
        {
            await ApplyCategoriasAsync(entity, selectedCategorias);
            await _context.SaveChangesAsync();
        }

        // ── CAMBIAR ESTADO ───────────────────────────────────────
        public async Task UpdateEstadoAsync(int id, int nuevoEstadoId)
        {
            var entity = await _context.Set<Auto>().FindAsync(id);
            if (entity == null)
                throw new Exception("Auto no encontrado.");

            entity.IdEstadoAuto = nuevoEstadoId;
            await _context.SaveChangesAsync();
        }

        // ── VALIDACIONES DE NEGOCIO ──────────────────────────────
        public async Task<bool> TieneSubastasAsync(int id)
        {
            return await _context.Set<Subasta>()
                .AnyAsync(s => s.IdAuto == id);
        }

        public async Task<bool> TieneSubastaActivaAsync(int id)
        {
            // IdEstadoSubasta = 1 es "Activa"
            return await _context.Set<Subasta>()
                .AnyAsync(s => s.IdAuto == id && s.IdEstadoSubasta == 1);
        }

        // ── HELPER: Aplicar categorías M:N ──────────────────────
        private async Task ApplyCategoriasAsync(Auto auto, string[] selectedCategorias)
        {
            if (selectedCategorias == null || selectedCategorias.Length == 0)
            {
                auto.IdCategoria = new List<Categoria>();
                return;
            }

            var ids = selectedCategorias
                .Select(x => int.TryParse(x, out var n) ? n : (int?)null)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
            {
                auto.IdCategoria = new List<Categoria>();
                return;
            }

            var categorias = await _context.Categoria
                .Where(c => ids.Contains(c.IdCategoria))
                .ToListAsync();

            auto.IdCategoria = categorias;
        }
        // Un auto se considera "vendido" si tiene una subasta Finalizada (Id=2)
        public async Task<bool> TieneSubastaFinalizadaAsync(int id)
        {
            return await _context.Set<Subasta>()
                .AnyAsync(s => s.IdAuto == id && s.IdEstadoSubasta == 2);
        }
        public async Task<bool> ExisteVinAsync(string vin, int? excluirAutoId = null)
        {
            var query = _context.Set<Auto>()
                .Where(a => a.Vin == vin);

            if (excluirAutoId.HasValue)
                query = query.Where(a => a.IdAuto != excluirAutoId.Value);

            return await query.AnyAsync();
        }
    }
}