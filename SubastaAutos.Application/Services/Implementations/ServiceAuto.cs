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

        // El constructor recibe las dependencias por inyección
        public ServiceAuto(IRepositoryAuto repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<AutoDTO>> ListAsync()
        {
            // Paso 1: pide los datos al repositorio (devuelve modelos de BD)
            var list = await _repository.ListAsync();

            // Paso 2: convierte la lista de modelos a lista de DTOs
            return _mapper.Map<ICollection<AutoDTO>>(list);
        }

        public async Task<AutoDTO?> FindByIdAsync(int id)
        {
            var @object = await _repository.FindByIdAsync(id);

            // Si el repositorio no encontró nada, devuelve null
            return _mapper.Map<AutoDTO?>(@object);
        }
    }
}
