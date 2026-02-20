using Microsoft.AspNetCore.Mvc;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using System;
using System.Threading.Tasks;

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

        //Metodo controlador para mostrar los detalles de un usuario (metodo original usado para el detalle mirey)
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var objeto = await _servicioUsuario.GetByIdAsync(id);
                if (objeto == null)
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

        // Nueva acción que devuelve un PartialView con los detalles (para el modal)
        //Estaba de prueba pero para no complicarse la existencia aún no lo volví a ver 
        [HttpGet]
        public async Task<IActionResult> DetailsModal(int id)
        {
            var objeto = await _servicioUsuario.GetByIdAsync(id);
            if (objeto == null) return NotFound();
            return PartialView("_UsuarioDetailsPartial", objeto);
        }
    }
}
