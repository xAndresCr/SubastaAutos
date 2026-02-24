using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Infraestructure.Repository.Interfaces;

namespace SubastaAutos.Application.Services.Implementations
{
    public class ServiceSubasta : IServiceSubasta
    {
        private readonly IRepositorySubasta _repository;
        private readonly IMapper _mapper;

        // Mismo patrón que ServiceAuto: recibe repositorio + mapper por DI
        public ServiceSubasta(IRepositorySubasta repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<SubastaDTO>> ListActivasAsync()
        {
            // 1. El repositorio consulta la BD y devuelve modelos con Include
            var list = await _repository.ListActivasAsync();
            // 2. AutoMapper convierte cada Subasta → SubastaDTO usando SubastaProfile
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }

        public async Task<ICollection<SubastaDTO>> ListFinalizadasAsync()
        {
            var list = await _repository.ListFinalizadasAsync();
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }

        public async Task<SubastaDTO?> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return _mapper.Map<SubastaDTO?>(entity);
        }
    }
}
