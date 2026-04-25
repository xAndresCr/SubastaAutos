using Microsoft.EntityFrameworkCore;
using SubastaAutos.Infraestructure.Data;
using SubastaAutos.Infraestructure.Repository.Interfaces;
using SubastaAutos.Infraestructure.Repository.Models;
using Entities = SubastaAutos.Infraestructure.Models;

namespace SubastaAutos.Infraestructure.Repository.Implementations
{
    public class RepositoryReporte : IRepositoryReporte
    {
        private readonly SubastaAutosContext _context;

        // Estados Subasta: 1=Activa, 2=Finalizada, 3=Cancelada, 4=Borrador
        private const int EstadoActiva = 1;
        private const int EstadoFinalizada = 2;

        public RepositoryReporte(SubastaAutosContext context)
        {
            _context = context;
        }

        // Reporte 2: Subastas por Categoría
        // Relación M:N Categoria ↔ Auto (via AutoCategoria) → Subasta.
        // Una subasta cuenta una vez por cada categoría del auto.
        // Solo Activa(1) y Finalizada(2). Filtro por Subasta.FechaInicio.
        public async Task<ICollection<ReporteCategoriaRow>> SubastasPorCategoriaAsync(
    DateTime desde, DateTime hasta)
        {
            var (desdeInclusivo, hastaExclusivo) = NormalizarRango(desde, hasta);

            // Paso 1: traer las categorías con sus conteos como tipo anónimo (EF traduce esto sin problema).
            var raw = await _context.Set<Entities.Categoria>()
                .AsNoTracking()
                .Select(c => new
                {
                    c.IdCategoria,
                    c.Nombre,
                    TotalSubastas = c.IdAuto
                        .SelectMany(a => a.Subasta)
                        .Count(s => (s.IdEstadoSubasta == EstadoActiva
                                  || s.IdEstadoSubasta == EstadoFinalizada)
                                 && s.FechaInicio >= desdeInclusivo
                                 && s.FechaInicio < hastaExclusivo),
                    TotalFinalizadas = c.IdAuto
                        .SelectMany(a => a.Subasta)
                        .Count(s => s.IdEstadoSubasta == EstadoFinalizada
                                 && s.FechaInicio >= desdeInclusivo
                                 && s.FechaInicio < hastaExclusivo)
                })
                .ToListAsync();

            // Paso 2: ordenar y mapear al record en memoria (sin SQL involucrado).
            return raw
                .OrderByDescending(r => r.TotalSubastas)
                .ThenBy(r => r.Nombre)
                .Select(r => new ReporteCategoriaRow(
                    r.IdCategoria,
                    r.Nombre,
                    r.TotalSubastas,
                    r.TotalFinalizadas))
                .ToList();
        }

        // Reporte 4: Actividad del Sistema por Periodo
        // Cada métrica filtra por su fecha propia.
        public async Task<ReporteActividadResumen> ActividadPorPeriodoAsync(
            DateTime desde, DateTime hasta)
        {
            var (desdeInclusivo, hastaExclusivo) = NormalizarRango(desde, hasta);

            // Totales globales
            var totalCreadas = await _context.Set<Entities.Subasta>()
                .AsNoTracking()
                .CountAsync(s => s.FechaCreacion >= desdeInclusivo
                              && s.FechaCreacion < hastaExclusivo);

            var totalPujas = await _context.Set<Entities.Puja>()
                .AsNoTracking()
                .CountAsync(p => p.FechaHora >= desdeInclusivo
                              && p.FechaHora < hastaExclusivo);

            var totalFinalizadas = await _context.Set<Entities.Subasta>()
                .AsNoTracking()
                .CountAsync(s => s.IdEstadoSubasta == EstadoFinalizada
                              && s.FechaCierre >= desdeInclusivo
                              && s.FechaCierre < hastaExclusivo);

            // Series agrupadas por día
            var creadasPorDia = await _context.Set<Entities.Subasta>()
                .AsNoTracking()
                .Where(s => s.FechaCreacion >= desdeInclusivo
                         && s.FechaCreacion < hastaExclusivo)
                .GroupBy(s => s.FechaCreacion!.Value.Date)
                .Select(g => new { Fecha = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            var pujasPorDia = await _context.Set<Entities.Puja>()
                .AsNoTracking()
                .Where(p => p.FechaHora >= desdeInclusivo
                         && p.FechaHora < hastaExclusivo)
                .GroupBy(p => p.FechaHora!.Value.Date)
                .Select(g => new { Fecha = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            var finalizadasPorDia = await _context.Set<Entities.Subasta>()
                .AsNoTracking()
                .Where(s => s.IdEstadoSubasta == EstadoFinalizada
                         && s.FechaCierre >= desdeInclusivo
                         && s.FechaCierre < hastaExclusivo)
                .GroupBy(s => s.FechaCierre.Date)
                .Select(g => new { Fecha = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            // Completar serie día a día con ceros donde no hay datos
            var mapCreadas = creadasPorDia.ToDictionary(x => x.Fecha, x => x.Cantidad);
            var mapPujas = pujasPorDia.ToDictionary(x => x.Fecha, x => x.Cantidad);
            var mapFinalizadas = finalizadasPorDia.ToDictionary(x => x.Fecha, x => x.Cantidad);

            var totalDias = (hastaExclusivo - desdeInclusivo).Days;
            var serie = new List<ActividadDiariaRow>(totalDias);

            for (int i = 0; i < totalDias; i++)
            {
                var fecha = desdeInclusivo.AddDays(i);
                serie.Add(new ActividadDiariaRow(
                    fecha,
                    mapCreadas.GetValueOrDefault(fecha),
                    mapPujas.GetValueOrDefault(fecha),
                    mapFinalizadas.GetValueOrDefault(fecha)));
            }

            return new ReporteActividadResumen(
                totalCreadas, totalPujas, totalFinalizadas, serie);
        }

        // Rango half-open [desde 00:00, hasta+1 00:00) para comparaciones correctas
        private static (DateTime desde, DateTime hastaExclusivo) NormalizarRango(
            DateTime desde, DateTime hasta)
        {
            var desdeInclusivo = desde.Date;
            var hastaExclusivo = hasta.Date.AddDays(1);
            if (hastaExclusivo <= desdeInclusivo)
                hastaExclusivo = desdeInclusivo.AddDays(1);
            return (desdeInclusivo, hastaExclusivo);
        }
    }
}