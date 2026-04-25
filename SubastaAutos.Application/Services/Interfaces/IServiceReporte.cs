using SubastaAutos.Application.DTOs;

namespace SubastaAutos.Application.Services.Interfaces
{
    public interface IServiceReporte
    {
        Task<ICollection<ReporteCategoriaDTO>> SubastasPorCategoriaAsync(DateTime desde, DateTime hasta);
        Task<ReporteActividadDTO> ActividadPorPeriodoAsync(DateTime desde, DateTime hasta);
    }
}