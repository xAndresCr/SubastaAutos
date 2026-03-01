using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Infraestructure.Repository.Interfaces;

namespace SubastaAutos.Application.Services.Implementations
{
    public class ServiceAuto : IServiceAuto
    {
        private readonly IRepositoryAuto _repository;
        private readonly IMapper _mapper;


        public ServiceAuto(IRepositoryAuto repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<AutoDTO>> ListAsync()
        {

            var list = await _repository.ListAsync();


            return _mapper.Map<ICollection<AutoDTO>>(list);
        }

        public async Task<AutoDTO?> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);


            return _mapper.Map<AutoDTO?>(@object);
        }
    }
}
