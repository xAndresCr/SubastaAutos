using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Infraestructure.Models;
using SubastaAutos.Infraestructure.Repository.Interfaces;

namespace SubastaAutos.Application.Services.Implementations
{
    public class ServiceSubasta : IServiceSubasta
    {
        private readonly IRepositorySubasta _repository;
        private readonly IMapper _mapper;

        public ServiceSubasta(IRepositorySubasta repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<SubastaDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }

        public async Task<ICollection<SubastaDTO>> ListActivasAsync()
        {
            var list = await _repository.ListActivasAsync();
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

        public async Task<int> AddAsync(SubastaDTO dto)
        {
            // Validar: el auto no puede tener otra subasta activa
            bool tieneActiva = await _repository.ExisteSubastaActivaParaAutoAsync(dto.IdAuto);
            if (tieneActiva)
                throw new InvalidOperationException("El auto ya tiene una subasta activa.");

            var entity = _mapper.Map<Subasta>(dto);
            return await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(int id, SubastaDTO dto)
        {
            // Validar que se puede editar
            bool puede = await PuedeEditarAsync(id);
            if (!puede)
                throw new InvalidOperationException(
                    "No se puede editar: la subasta ya inició o tiene pujas.");

            var entity = await _repository.FindByIdAsync(id);
            if (entity == null)
                throw new Exception("Subasta no encontrada.");

            // Solo actualizar campos permitidos
            entity.FechaInicio = dto.FechaInicio;
            entity.FechaCierre = dto.FechaCierre;
            entity.PrecioBase = dto.PrecioBase;
            entity.IncrementoMinimo = dto.IncrementoMinimo;

            await _repository.UpdateAsync(entity);
        }

        // Borrador(4) → Activa(1)
        public async Task PublicarAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            if (entity == null)
                throw new Exception("Subasta no encontrada.");

            if (entity.IdEstadoSubasta != 4) // Solo desde Borrador
                throw new InvalidOperationException("Solo se puede publicar una subasta en estado Borrador.");

            if (entity.FechaInicio <= DateTime.Now)
                throw new InvalidOperationException("La fecha de inicio debe ser futura para publicar.");

            await _repository.UpdateEstadoAsync(id, 1); // Activa
        }

        // → Cancelada(3)
        public async Task CancelarAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            if (entity == null)
                throw new Exception("Subasta no encontrada.");

            bool tienePujas = await _repository.TienePujasAsync(id);
            bool haIniciado = await _repository.HaIniciadoAsync(id);

            if (haIniciado && tienePujas)
                throw new InvalidOperationException(
                    "No se puede cancelar: la subasta ya inició y tiene pujas.");

            await _repository.UpdateEstadoAsync(id, 3); // Cancelada
        }

        public async Task<bool> PuedeEditarAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            if (entity == null) return false;

            // Borrador siempre se puede editar (no ha sido publicado)
            if (entity.IdEstadoSubasta == 4) // Borrador
                return true;

            // Para otros estados, validar pujas y si ya inició
            bool tienePujas = await _repository.TienePujasAsync(id);
            if (tienePujas) return false;

            bool haIniciado = await _repository.HaIniciadoAsync(id);
            if (haIniciado) return false;

            return true;
        }

        public async Task CerrarAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            if (entity == null)
                throw new Exception("Subasta no encontrada.");

            // Solo se puede cerrar si está Activa
            if (entity.IdEstadoSubasta != 1)
                throw new InvalidOperationException(
                    "Solo se puede cerrar una subasta activa.");

            // Cambiar estado a Finalizada
            await _repository.UpdateEstadoAsync(id, 2);
            // Determinar ganador — la puja de mayor monto
            var pujaGanadora = entity.Puja
                .OrderByDescending(p => p.Monto)
                .FirstOrDefault();

            // Si hubo pujas, guardar el resultado
            if (pujaGanadora != null)
            {
                var resultado = new ResultadoSubasta
                {
                    IdSubasta = id,
                    IdUsuarioGanador = pujaGanadora.IdUsuario,
                    MontoFinal = pujaGanadora.Monto,
                    FechaCierreReal = DateTime.Now
                };
                await _repository.GuardarResultadoAsync(resultado);
            }
        }
        public async Task<ICollection<SubastaDTO>> ListSubastasGanadasAsync(int idUsuario)
        {
            var list = await _repository.ListSubastasGanadasAsync(idUsuario);
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }

        public async Task<ICollection<SubastaDTO>> ListByVendedorAsync(int idVendedor)
        {
            var list = await _repository.ListByVendedorAsync(idVendedor);
            return _mapper.Map<ICollection<SubastaDTO>>(list);
        }
    }
}