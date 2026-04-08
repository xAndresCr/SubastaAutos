using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Infraestructure.Models;
using SubastaAutos.Infraestructure.Repository.Implementations;
using SubastaAutos.Infraestructure.Repository.Interfaces;

namespace SubastaAutos.Application.Services.Implementations
{
    public class ServicePuja : IServicePuja
    {
        private readonly IRepositoryPuja _repository;
        private readonly IRepositorySubasta _repositorySubasta;
        private readonly IMapper _mapper;

        public ServicePuja(IRepositoryPuja repository, IMapper mapper, IRepositorySubasta repositorySubasta)
        {
            _repository = repository;
            _mapper = mapper;
            _repositorySubasta = repositorySubasta;
        }

        public async Task<int> AddAsync(PujaDTO dto, int idUsuarioActual)
        {
            // 1. Obtener la subasta
            var subasta = await _repositorySubasta.FindByIdAsync(dto.IdSubasta);
            if (subasta == null)
                throw new Exception("Subasta no encontrada");
            //verificar que la subasta está activa 
            if (subasta.IdEstadoSubasta != 1)
                throw new Exception("No se puede pujar, la subasta no está activa");
            //Validar que el vendedor no puje
            if (subasta.IdVendedor == idUsuarioActual)
                throw new Exception("Los vendedores no pueden pujar sobre su propia subasta");
           
            //Obtener el monto más alto o actual 
            var pujaLider = await _repository.GetPujaLiderAsync(dto.IdSubasta);
            //asigna el monto actual al monto de la puja más alta, si no hay pujas deja el precio base de la subasta
            decimal montoActual = pujaLider?.Monto ?? subasta.PrecioBase;

            // 5. Validar monto mayor que puja actual
            if (dto.Monto <= montoActual)
                throw new InvalidOperationException(
                    $"El monto debe ser mayor que la puja actual (₡{montoActual:N2}).");

            // 6. Validar incremento mínimo
            if (dto.Monto < montoActual + subasta.IncrementoMinimo)
                throw new InvalidOperationException(
                    $"El monto debe cumplir el incremento mínimo de ₡{subasta.IncrementoMinimo:N2}.");

            var entity = _mapper.Map<Puja>(dto);
            entity.IdUsuario = idUsuarioActual;
            entity.FechaHora = DateTime.Now;
            return await _repository.AddAsync(entity);

        }

        public async Task<PujaDTO?> GetPujaLiderAsync(int idSubasta)
        {
            var lider = await _repository.GetPujaLiderAsync(idSubasta);
            return lider == null ? null : _mapper.Map<PujaDTO>(lider);
        }

        public async Task<ICollection<PujaDTO>> ListBySubastaAsync(int idSubasta)
        {
            var list = await _repository.ListBySubastaAsync(idSubasta);
            return _mapper.Map<ICollection<PujaDTO>>(list);
        }

        public async Task<bool> PujaFueSuperadaAsync(int idSubasta, int idUsuario)
        {
            bool tienePuja = await _repository.UsuarioTienePujaAsync(idSubasta, idUsuario);
            if (!tienePuja) return false;
            return !await _repository.EsLiderAsync(idSubasta, idUsuario);
        }
    }
}
