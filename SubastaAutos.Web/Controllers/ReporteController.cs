using Microsoft.AspNetCore.Mvc;
using SubastaAutos.Application.Services.Interfaces;

namespace SubastaAutos.Web.Controllers
{
    public class ReporteController : Controller
    {
        private readonly IServiceReporte _serviceReporte;

        // Rango por defecto cuando el usuario no especifica fechas
        private const int DiasPorDefecto = 30;

        public ReporteController(IServiceReporte serviceReporte)
        {
            _serviceReporte = serviceReporte;
        }

        // ── REPORTE 2: Subastas por Categoría ──
        public IActionResult SubastasPorCategoria()
        {
            var (desde, hasta) = RangoPorDefecto();
            ViewBag.Desde = desde.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta.ToString("yyyy-MM-dd");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SubastasPorCategoriaData(DateTime? desde, DateTime? hasta)
        {
            var (d, h) = Normalizar(desde, hasta);
            try
            {
                var data = await _serviceReporte.SubastasPorCategoriaAsync(d, h);
                return Json(new
                {
                    success = true,
                    desde = d.ToString("yyyy-MM-dd"),
                    hasta = h.ToString("yyyy-MM-dd"),
                    items = data
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // ── REPORTE 4: Actividad del Sistema por Periodo ──
        public IActionResult ActividadPorPeriodo()
        {
            var (desde, hasta) = RangoPorDefecto();
            ViewBag.Desde = desde.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta.ToString("yyyy-MM-dd");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ActividadPorPeriodoData(DateTime? desde, DateTime? hasta)
        {
            var (d, h) = Normalizar(desde, hasta);
            try
            {
                var data = await _serviceReporte.ActividadPorPeriodoAsync(d, h);
                return Json(new
                {
                    success = true,
                    desde = d.ToString("yyyy-MM-dd"),
                    hasta = h.ToString("yyyy-MM-dd"),
                    data
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        private static (DateTime desde, DateTime hasta) Normalizar(DateTime? desde, DateTime? hasta)
        {
            var hastaFinal = (hasta ?? DateTime.Now).Date;
            var desdeFinal = (desde ?? hastaFinal.AddDays(-DiasPorDefecto)).Date;
            if (desdeFinal > hastaFinal)
                desdeFinal = hastaFinal.AddDays(-DiasPorDefecto);
            return (desdeFinal, hastaFinal);
        }

        private static (DateTime desde, DateTime hasta) RangoPorDefecto()
        {
            var hasta = DateTime.Now.Date;
            var desde = hasta.AddDays(-DiasPorDefecto);
            return (desde, hasta);
        }
    }
}