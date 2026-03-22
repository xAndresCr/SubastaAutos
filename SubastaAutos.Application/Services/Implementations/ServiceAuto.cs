using AutoMapper;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Infraestructure.Models;
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
            var entity = await _repository.FindByIdAsync(id);
            return _mapper.Map<AutoDTO?>(entity);
        }

        public async Task<int> AddAsync(AutoDTO dto, string[] selectedCategorias, List<byte[]> imagenes)
        {
            var entity = _mapper.Map<Auto>(dto);

            // Agregar imágenes: la primera es principal
            for (int i = 0; i < imagenes.Count; i++)
            {
                entity.AutoImagen.Add(new AutoImagen
                {
                    Imagen = imagenes[i],
                    EsPrincipal = i == 0
                });
            }

            return await _repository.AddAsync(entity, selectedCategorias);
        }

        public async Task UpdateAsync(int id, AutoDTO dto, string[] selectedCategorias, List<byte[]>? nuevasImagenes)
        {
            var entity = await _repository.FindByIdAsync(id);
            if (entity == null)
                throw new Exception("Auto no encontrado.");

            // Mapear propiedades escalares del DTO sobre el entity trackeado
            _mapper.Map(dto, entity);

            // Si suben imágenes nuevas, reemplazar las existentes
            if (nuevasImagenes != null && nuevasImagenes.Count > 0)
            {
                entity.AutoImagen.Clear();
                for (int i = 0; i < nuevasImagenes.Count; i++)
                {
                    entity.AutoImagen.Add(new AutoImagen
                    {
                        Imagen = nuevasImagenes[i],
                        EsPrincipal = i == 0
                    });
                }
            }

            await _repository.UpdateAsync(entity, selectedCategorias);
        }

        // Activo Inactivo 
        public async Task ActivarDesactivarAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            if (entity == null)
                throw new Exception("Auto no encontrado.");

            // Si está Activo(1) → Inactivo(2), si está Inactivo(2) → Activo(1)
            int nuevoEstado = entity.IdEstadoAuto == 1 ? 2 : 1;
            await _repository.UpdateEstadoAsync(id, nuevoEstado);
        }

        // Eliminación lógica
        public async Task EliminarLogicoAsync(int id)
        {
            // Validar: no puede tener subastas asociadas
            bool tieneSubastas = await _repository.TieneSubastasAsync(id);
            if (tieneSubastas)
                throw new InvalidOperationException("No se puede eliminar: el auto tiene subastas asociadas.");

            await _repository.UpdateEstadoAsync(id, 3); // 3 = Eliminado
        }

        public async Task<bool> TieneSubastaActivaAsync(int id)
        {
            return await _repository.TieneSubastaActivaAsync(id);
        }
        public async Task<bool> TieneSubastaFinalizadaAsync(int id)
        {
            return await _repository.TieneSubastaFinalizadaAsync(id);
        }
        public async Task<bool> ExisteVinAsync(string vin, int? excluirAutoId = null)
        {
            return await _repository.ExisteVinAsync(vin, excluirAutoId);
        }
    }
}