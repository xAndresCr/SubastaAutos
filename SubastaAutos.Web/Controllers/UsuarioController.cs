using Microsoft.AspNetCore.Mvc;
using SubastaAutos.Application.Services.Interfaces;
using System.Collections;

namespace SubastaAutos.Web.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly IServiceUsuario _servicioUsuario;

        public UsuarioController(IServiceUsuario servicioUsuario)
        {
            _servicioUsuario = servicioUsuario;
        }

        //Metodo controlador para mostrar los usuarios en la vista
        public async Task<IActionResult> Index()
        {
            var objeto = await _servicioUsuario.ListAsync();
            return View(objeto);
        }

        //Metodo controlador para mostrar los detalles de un usuario
        public async Task<ActionResult> Details(int id)
            {
            try
            {
                var objeto = await _servicioUsuario.GetByIdAsync(id);
                if(objeto== null)
                {
                    throw new Exception("Usuario no encontrado");
                }

                return View(objeto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
