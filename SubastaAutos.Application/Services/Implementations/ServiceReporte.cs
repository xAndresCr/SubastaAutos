using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Infraestructure.Repository.Interfaces;

namespace SubastaAutos.Application.Services.Implementations
{
    public class ServiceReporte : IServiceReporte
    {
        private readonly IRepositoryReporte _repository;

        public ServiceReporte(IRepositoryReporte repository)
        {
            _repository = repository;
        }

        public async Task<ICollection<ReporteCategoriaDTO>> SubastasPorCategoriaAsync(
            DateTime desde, DateTime hasta)
        {
            var rows = await _repository.SubastasPorCategoriaAsync(desde, hasta);

            return rows.Select(r => new ReporteCategoriaDTO
            {
                IdCategoria = r.IdCategoria,
                Nombre = r.Nombre,
                TotalSubastas = r.TotalSubastas,
                TotalFinalizadas = r.TotalFinalizadas
            }).ToList();
        }

        public async Task<ReporteActividadDTO> ActividadPorPeriodoAsync(
            DateTime desde, DateTime hasta)
        {
            var resumen = await _repository.ActividadPorPeriodoAsync(desde, hasta);

            return new ReporteActividadDTO
            {
                TotalSubastasCreadas = resumen.TotalSubastasCreadas,
                TotalPujas = resumen.TotalPujas,
                TotalSubastasFinalizadas = resumen.TotalSubastasFinalizadas,
                SerieDiaria = resumen.SerieDiaria.Select(d => new ActividadDiariaDTO
                {
                    Fecha = d.Fecha,
                    Creadas = d.Creadas,
                    Pujas = d.Pujas,
                    Finalizadas = d.Finalizadas
                }).ToList()
            };
        }
    }
}