using Microsoft.EntityFrameworkCore;
using SubastaAutos.Infraestructure.Data;

//using SubastaAutos.Infraestructure.Data;
using SubastaAutos.Infraestructure.Models;
using SubastaAutos.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Infraestructure.Repository.Implementations
{
    public class RepositoryUsuario : IRepositoryUsuario
    {
        private readonly SubastaAutosContext _context;

        public RepositoryUsuario(SubastaAutosContext context)
        {
            _context = context;
        }


        //Metodo para obtnener el detalle de un usuario por su id
        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Set<Usuario>().
                AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }
        //Metodo para obtener la lista de usuarios
        public async Task<ICollection<Usuario>> ListAsync()
        {
            var collecton = await _context.Set<Usuario>()
                .Include(x => x.IdRolNavigation)
                .OrderBy(x => x.NombreCompleto)
                .AsNoTracking()
                .ToListAsync();
            return collecton;
        }
    }
}
