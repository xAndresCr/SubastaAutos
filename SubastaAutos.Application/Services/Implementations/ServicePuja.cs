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

            decimal montoMinimo;

            if (pujaLider != null)
            {
                // Ya hay pujas
                montoMinimo = pujaLider.Monto + subasta.IncrementoMinimo;
            }
            else
            {
                //Primera puja también aplica incremento
                montoMinimo = subasta.PrecioBase + subasta.IncrementoMinimo;
            }

            // 5. Validación única 
            if (dto.Monto < montoMinimo)
                throw new InvalidOperationException(
                    $"El monto debe ser al menos ₡{montoMinimo:N2}.");

            // 6. Crear entidad
            var entity = _mapper.Map<Puja>(dto);
            entity.IdUsuario = idUsuarioActual;
            entity.FechaHora = DateTime.Now;

            // 7. Guardar
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
