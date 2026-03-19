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
    public class ServiceCondicionAuto : IServiceCondicionAuto
    {
        private readonly IRepositoryCondicionAuto _repository;
        private readonly IMapper _mapper;

        public ServiceCondicionAuto(IRepositoryCondicionAuto repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<CondicionAutoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<CondicionAutoDTO>>(list);
        }
    }
}