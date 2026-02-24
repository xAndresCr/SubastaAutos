using Microsoft.AspNetCore.Mvc;
using SubastaAutos.Application.Services.Interfaces;

namespace SubastaAutos.Web.Controllers
{
    public class SubastaController : Controller
    {
        private readonly IServiceSubasta _serviceSubasta;
        private readonly IServicePuja _servicePuja;

        // Se inyectan ambos servicios porque este controller
        // necesita datos de Subasta Y de Puja (para el historial)
        public SubastaController(IServiceSubasta serviceSubasta, IServicePuja servicePuja)
        {
            _serviceSubasta = serviceSubasta;
            _servicePuja = servicePuja;
        }

        // GET: /Subasta
        // Muestra SOLO las subastas activas
        public async Task<IActionResult> Index()
        {
            var collection = await _serviceSubasta.ListActivasAsync();
            return View(collection);
            // Busca: Views/Subasta/Index.cshtml
        }

        // GET: /Subasta/Finalizadas
        // Muestra subastas finalizadas y canceladas
        public async Task<IActionResult> Finalizadas()
        {
            var collection = await _serviceSubasta.ListFinalizadasAsync();
            return View(collection);
            // Busca: Views/Subasta/Finalizadas.cshtml
        }

        // GET: /Subasta/Details/5
        // Muestra el detalle completo de una subasta
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                    return RedirectToAction(nameof(Index));

                var entity = await _serviceSubasta.FindByIdAsync(id.Value);

                if (entity == null)
                    throw new Exception("Subasta no encontrada.");

                return View(entity);
                // Busca: Views/Subasta/Details.cshtml
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: /Subasta/Pujas/5
        // Muestra el historial de pujas de la subasta con id=5
        public async Task<IActionResult> Pujas(int? id)
        {
            try
            {
                if (id == null)
                    return RedirectToAction(nameof(Index));

                var collection = await _servicePuja.ListBySubastaAsync(id.Value);

                // ViewBag pasa el IdSubasta a la vista para mostrar en el título
                // y para el botón "Volver al detalle"
                ViewBag.IdSubasta = id.Value;

                return View(collection);
                // Busca: Views/Subasta/Pujas.cshtml
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
