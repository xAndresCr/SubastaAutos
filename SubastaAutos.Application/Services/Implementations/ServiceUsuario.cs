using AutoMapper;
using Microsoft.Extensions.Options;
using SubastaAutos.Application.Config;
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
using System.Security.Cryptography;
using SubastaAutos.Web.Util;

namespace SubastaAutos.Application.Services.Implementations
{
    public class ServiceUsuario : IServiceUsuario
    {
        private readonly IRepositoryUsuario repositoryUsuario;

        //AutoMapper que se crea para el mapeo entre el profile y el DTO hacia las entidades
        private readonly IMapper _mapper;
        private readonly AppConfig _appConfig;



        public ServiceUsuario(IRepositoryUsuario repositoryUsuario, IMapper mapper, IOptions<AppConfig> appConfig)
        {
            this.repositoryUsuario = repositoryUsuario;
            _mapper = mapper;
            _appConfig = appConfig.Value;   
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
         

            //Hash de la contraseña utilizando el helper, se le pasa la contraseña y el secret del appsettings para generar el hash
            usuario.PasswordHash = CryptoHelper.HashPassword(
           usuarioDTO.PasswordHash,
           _appConfig.Crypto.Secret);

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

            bool correoIgual = entity.Correo.ToLower() == dto.Correo.ToLower();
            bool nombreIgual = entity.NombreCompleto.ToLower() == dto.NombreCompleto.ToLower();
            bool estadoIgual = entity.EstadoUsuario == dto.EstadoUsuario;
            bool passwordIgual = true;

            if (!string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                var nuevaHasheada = CryptoHelper.HashPassword(
                    dto.PasswordHash,
                    _appConfig.Crypto.Secret);

                passwordIgual = entity.PasswordHash == nuevaHasheada; // fix
                if (!passwordIgual)
                    entity.PasswordHash = nuevaHasheada; // fix
            }

            if (correoIgual && nombreIgual && estadoIgual && passwordIgual)
                throw new InvalidOperationException(
                    "No se realizaron cambios, los datos son idénticos a los actuales.");

            if (!correoIgual)
            {
                bool correoExiste = await repositoryUsuario.ExisteCorreoAsync(dto.Correo);
                if (correoExiste)
                    throw new InvalidOperationException(
                        "El correo ingresado ya está registrado por otro usuario.");
            }

            entity.NombreCompleto = dto.NombreCompleto;
            entity.Correo = dto.Correo;
            entity.EstadoUsuario = dto.EstadoUsuario;

            await repositoryUsuario.UpdateAsync(entity);
        }


        public async Task ToggleEstadoAsync(int id)
        {
            await repositoryUsuario.ToggleEstadoAsync(id);
        }

        public async Task UpdateEstadoAsync(int id, bool nuevoEstado)
        {
            var entity = await repositoryUsuario.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Usuario no encontrado.");

            entity.EstadoUsuario = nuevoEstado;
            await repositoryUsuario.UpdateAsync(entity);
        }
        public async Task<UsuarioDTO?> LoginAsync(string correo, string password)
        {

            //Hash de la contraseña utilizando el helper, antes de iniciar sesión
            var passwordHasheada = CryptoHelper.HashPassword(
            password,
            _appConfig.Crypto.Secret);

            var usuario = await repositoryUsuario.LoginAsync(correo, passwordHasheada);
            if (usuario == null) return null;
            return _mapper.Map<UsuarioDTO>(usuario);
        }



    }
}
