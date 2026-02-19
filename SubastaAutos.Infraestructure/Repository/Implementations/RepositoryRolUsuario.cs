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
    public class RepositoryRolUsuario : IRepositoryRolUsuario
    {
        // Se crea el atributo para obtener los datos del contexto
        private readonly SubastaAutosContext _context;

        public RepositoryRolUsuario(SubastaAutosContext context)
        {
            _context = context;
        }

        //Metodo de la interfaz para iterar la consulta de Roles
        public async Task<ICollection<RolUsuario>> ListAsync()
        {
            var collection = await _context.Set<RolUsuario>()
                                            .AsNoTracking()
                                            .ToListAsync();
            return collection;
        }
    }
}
