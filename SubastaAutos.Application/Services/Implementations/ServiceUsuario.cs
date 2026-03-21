using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Infraestructure.Models;
using SubastaAutos.Infraestructure.Repository.Implementations;
using SubastaAutos.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.Services.Implementations
{
    public class ServiceUsuario : IServiceUsuario
    {
        private readonly IRepositoryUsuario repositoryUsuario;

        //AutoMapper que se crea para el mapeo entre el profile y el DTO hacia las entidades
        private readonly IMapper _mapper;

        public ServiceUsuario(IRepositoryUsuario repositoryUsuario, IMapper mapper)
        {
            this.repositoryUsuario = repositoryUsuario;
            _mapper = mapper;
        }

        public async Task<bool> ExisteCorreoAsync(string correo)
        {
            return await repositoryUsuario.ExisteCorreoAsync(correo);
        }


        //Metodo que mapea el DTO hacia la entidad usuario por medio del AutoMappper cuando se agrega un nuevo usuario
        public async Task<UsuarioDTO> AddAsync(UsuarioDTO usuarioDTO)
        {
            var usuario = _mapper.Map<Usuario>(usuarioDTO);
            // Asignar fecha actual automáticamente
            usuario.FechaRegistro = DateTime.Now;

            usuario = await repositoryUsuario.AddAsync(usuario);
            return _mapper.Map<UsuarioDTO>(usuario);

        }


        //Metodo que mapea el DTO hacia la entidad usuario por medio del AutoMappper cuando se retorna 
        public async Task<UsuarioDTO> GetByIdAsync(int id)
        {
            var usuario = await repositoryUsuario.GetByIdAsync(id);
            return _mapper.Map<UsuarioDTO>(usuario);
        }

        //Metodo que retorna la coleccion de usuarios mapeada hacia el DTO por medio del AutoMapper
        public async Task<ICollection<UsuarioDTO>> ListAsync()
        {
           
            var list = await repositoryUsuario.ListAsync();
            var collection = _mapper.Map<ICollection<UsuarioDTO>>(list);
            return collection;
        }

        public async Task UpdateAsync(int id, UsuarioDTO dto)
        {
            var entity = await repositoryUsuario.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Usuario no encontrado.");

             //Validar que el correo nuevo sea diferente al actual
            if (entity.Correo.ToLower() == dto.Correo.ToLower())
                throw new InvalidOperationException("El correo nuevo debe ser diferente al correo actual.");

             //Validar que el correo nuevo no exista en la BD
            bool correoExiste = await repositoryUsuario.ExisteCorreoAsync(dto.Correo);
            if (correoExiste)
                throw new InvalidOperationException("El correo ingresado ya está registrado por otro usuario.");

            entity.NombreCompleto = dto.NombreCompleto;
            entity.Correo = dto.Correo;
            await repositoryUsuario.UpdateAsync(entity);
        }

        public async Task ToggleEstadoAsync(int id)
        {
            await repositoryUsuario.ToggleEstadoAsync(id);
        }
    }
}
