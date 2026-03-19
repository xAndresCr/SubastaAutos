using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SubastaAutos.Infraestructure.Data;
using SubastaAutos.Infraestructure.Models;
using SubastaAutos.Infraestructure.Repository.Interfaces;

namespace SubastaAutos.Infraestructure.Repository.Implementations
{
    public class RepositoryEstadoAuto : IRepositoryEstadoAuto
    {
        private readonly SubastaAutosContext _context;

        public RepositoryEstadoAuto(SubastaAutosContext context)
        {
            _context = context;
        }

        public async Task<ICollection<EstadoAuto>> ListAsync()
        {
            return await _context.Set<EstadoAuto>()
                .OrderBy(e => e.Nombre)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}