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


        //Válida si existe un usuario con el mismo correo electrónico en la base de datos para evitar duplicados
        public async Task<bool> ExisteCorreoAsync(string correo)
        {
            return await _context.Usuario.AnyAsync(u => u.Correo == correo);
        }

        //Agrega al usuario
        public async Task<Usuario> AddAsync(Usuario usuario)
        {
            _context.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;

        }


        //Metodo para obtnener el detalle de un usuario por su id
        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Set<Usuario>()
                .Include(x => x.IdRolNavigation)
                .Include(x => x.Subasta)
                .Include(x => x.Puja)
                .AsNoTracking()
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

        public async Task UpdateAsync(Usuario entity)
        {
            _context.Entry(entity).Property(u => u.NombreCompleto).IsModified = true;
            _context.Entry(entity).Property(u => u.Correo).IsModified = true;
            _context.Entry(entity).Property(u => u.EstadoUsuario).IsModified = true;
            // Solo persistir contraseña si tiene valor
            if (!string.IsNullOrWhiteSpace(entity.PasswordHash))
                _context.Entry(entity).Property(u => u.PasswordHash).IsModified = true;

            await _context.SaveChangesAsync();
        }


        public async Task ToggleEstadoAsync(int id)
        {
            var entity = await _context.Usuario.FindAsync(id);

            if (entity == null)
                throw new Exception("Usuario no encontrado.");


            entity.EstadoUsuario = !entity.EstadoUsuario;
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> LoginAsync(string correo, string password)
        {
            return await _context.Usuario
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u =>
                    u.Correo.ToLower() == correo.ToLower() && u.PasswordHash == password);
        }
    }
}
