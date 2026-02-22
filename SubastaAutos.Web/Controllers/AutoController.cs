using Microsoft.AspNetCore.Mvc;
using SubastaAutos.Application.Services.Interfaces;

namespace SubastaAutos.Web.Controllers
{
    public class AutoController : Controller
    {
        private readonly IServiceAuto _serviceAuto;

        // ASP.NET inyecta el servicio automáticamente
        public AutoController(IServiceAuto serviceAuto)
        {
            _serviceAuto = serviceAuto;
        }

        // GET: /Auto
        // Acción para el LISTADO
        public async Task<IActionResult> Index()
        {
            var collection = await _serviceAuto.ListAsync();
            return View(collection);
            // Busca automáticamente: Views/Auto/Index.cshtml
        }

        // GET: /Auto/Details/5
        // Acción para el DETALLE de un auto con id=5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                // Si no viene id en la URL, redirige al listado
                if (id == null)
                    return RedirectToAction(nameof(Index));

                var @object = await _serviceAuto.FindByIdAsync(id.Value);

                // Si no existe ese auto, lanza excepción
                if (@object == null)
                    throw new Exception("Auto no encontrado.");

                return View(@object);
                // Busca automáticamente: Views/Auto/Details.cshtml
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
