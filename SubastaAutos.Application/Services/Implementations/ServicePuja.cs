using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Infraestructure.Repository.Interfaces;

namespace SubastaAutos.Application.Services.Implementations
{
    public class ServicePuja : IServicePuja
    {
        private readonly IRepositoryPuja _repository;
        private readonly IMapper _mapper;

        public ServicePuja(IRepositoryPuja repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<PujaDTO>> ListBySubastaAsync(int idSubasta)
        {
            var list = await _repository.ListBySubastaAsync(idSubasta);
            return _mapper.Map<ICollection<PujaDTO>>(list);
        }
    }
}
