using Microsoft.AspNetCore.Mvc;
using SubastaAutos.Application.Services.Interfaces;

namespace SubastaAutos.Web.Controllers
{
    public class RolUsuarioController : Controller
    {

        private readonly IServiceRolUsuario _serviceRolUsuario;

        public RolUsuarioController(IServiceRolUsuario serviceRolUsuario)
        {
            _serviceRolUsuario = serviceRolUsuario;
        }

        //Metodo controlador que llama al servicio
        public async Task<IActionResult> Index()
        {
            var collection = await _serviceRolUsuario.ListAsync();
            return View(collection);
        }
    }
}
