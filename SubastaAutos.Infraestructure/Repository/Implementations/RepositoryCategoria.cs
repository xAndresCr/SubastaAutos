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
    public class RepositoryCategoria : IRepositoryCategoria
    {
        private readonly SubastaAutosContext _context;

        public RepositoryCategoria(SubastaAutosContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Categoria>> ListAsync()
        {
            return await _context.Set<Categoria>()
                .OrderBy(c => c.Nombre)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}