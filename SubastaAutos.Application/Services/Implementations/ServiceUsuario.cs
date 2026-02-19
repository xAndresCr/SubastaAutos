using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
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
    }
}
