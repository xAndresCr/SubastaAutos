using SubastaAutos.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.Services.Interfaces
{
    public interface IServiceUsuario
    {
        //Método para obtener
        Task<ICollection<UsuarioDTO>> ListAsync();

        Task<UsuarioDTO> GetByIdAsync(int id);
    }
}
