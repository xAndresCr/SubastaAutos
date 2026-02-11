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
    public  class ServiceRolUsuario: IServiceRolUsuario
    {
        private readonly IRepositoryRolUsuario _repository;
        //AutoMapper que se crea para el mapeo entre el profile y el DTO
        private readonly IMapper _mapper;

        public ServiceRolUsuario(IRepositoryRolUsuario repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        //Se implementa el Automapper safisfactoriamnete para el mapeo entre el profile y el DTO
        public async Task<ICollection<RolUsuarioDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<RolUsuarioDTO>>(list);    
        }
    }
}
