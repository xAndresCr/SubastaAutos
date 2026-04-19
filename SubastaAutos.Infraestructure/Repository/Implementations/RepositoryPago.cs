using Microsoft.EntityFrameworkCore;
using SubastaAutos.Infraestructure.Data;
using SubastaAutos.Infraestructure.Models;
using SubastaAutos.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Infraestructure.Repository.Implementations
{
    public class RepositoryPago : IRepositoryPago
    {
        private readonly SubastaAutosContext _context;

        public RepositoryPago(SubastaAutosContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Pago entity)
        {
            await _context.Set<Pago>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.IdPago;
        }
        //metodo para cambiar el estado del pago a confirmado
        //Al presionar "Confirmar Pago" en la interfaz, se llamará a este método para actualizar el estado del pago a "Confirmado"
        public async Task ConfirmarPagoAsync(int idPago)
        {
            var entity = await _context.Set<Pago>().FindAsync(idPago);
            if(entity == null)
                throw new Exception($"No se encontró el pago con ID {idPago}");

            entity.IdEstadoPago = 2; // Confirmado
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistePagoParaSubastaAsync(int idSubasta)
        {
            return await _context.Set<Pago>()
                .AnyAsync(p => p.IdSubasta == idSubasta);
        }

        public async Task<Pago?> GetBySubastaAsync(int idSubasta)
        {
            return await _context.Set<Pago>()
                .Include(p => p.IdEstadoPagoNavigation)
                .Include(p => p.IdSubastaNavigation)
                    .ThenInclude(s => s.IdAutoNavigation)  
                .FirstOrDefaultAsync(p => p.IdSubasta == idSubasta);    
        }
        public async Task<Pago?> GetByIdAsync(int idPago)
        {
            return await _context.Set<Pago>()
                .Include(p => p.IdEstadoPagoNavigation)
                .Include(p => p.IdSubastaNavigation)
                    .ThenInclude(s => s.IdAutoNavigation)
                .Include(p => p.IdSubastaNavigation)
                    .ThenInclude(s => s.ResultadoSubasta)
                        .ThenInclude(r => r.IdUsuarioGanadorNavigation)
                .FirstOrDefaultAsync(p => p.IdPago == idPago);
        }
    }
}
