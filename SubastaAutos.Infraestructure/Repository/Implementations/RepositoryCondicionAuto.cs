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
    public class RepositoryCondicionAuto : IRepositoryCondicionAuto
    {
        private readonly SubastaAutosContext _context;

        public RepositoryCondicionAuto(SubastaAutosContext context)
        {
            _context = context;
        }

        public async Task<ICollection<CondicionAuto>> ListAsync()
        {
            return await _context.Set<CondicionAuto>()
                .OrderBy(c => c.Nombre)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}