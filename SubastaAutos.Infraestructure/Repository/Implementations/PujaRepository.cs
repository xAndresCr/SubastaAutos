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
    public class PujaRepository : IPujaRepository
    {

        private readonly SubastaAutosContext _context;

        public PujaRepository(SubastaAutosContext context)
        {
            _context = context;

        }

        public async Task<int> AddAsync(Puja entity)
        {
            await _context.Set<Puja>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.IdPuja;
        }

        public Task<bool> EsLiderAsync(int idSubasta, int idUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<List<Puja>> GetBySubastaAsync(int idSubasta)
        {
            throw new NotImplementedException();
        }

        public Task<Puja?> GetPujaLiderAsync(int idSubasta)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UsuarioTienePujaAsync(int idSubasta, int idUsuario)
        {
            throw new NotImplementedException();
        }
    }
}
