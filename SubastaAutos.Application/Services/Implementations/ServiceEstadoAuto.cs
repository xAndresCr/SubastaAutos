using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Infraestructure.Repository.Interfaces;

namespace SubastaAutos.Application.Services.Implementations
{
    public class ServiceEstadoAuto : IServiceEstadoAuto
    {
        private readonly IRepositoryEstadoAuto _repository;
        private readonly IMapper _mapper;

        public ServiceEstadoAuto(IRepositoryEstadoAuto repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<EstadoAutoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<EstadoAutoDTO>>(list);
        }
    }
}