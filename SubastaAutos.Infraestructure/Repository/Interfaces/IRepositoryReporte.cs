using SubastaAutos.Infraestructure.Repository.Models;

namespace SubastaAutos.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryReporte
    {
        Task<ICollection<ReporteCategoriaRow>> SubastasPorCategoriaAsync(DateTime desde, DateTime hasta);
        Task<ReporteActividadResumen> ActividadPorPeriodoAsync(DateTime desde, DateTime hasta);
    }
}