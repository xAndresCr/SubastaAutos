using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Implementations;
using SubastaAutos.Application.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SubastaAutos.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IServiceUsuario _servicioUsuario;
        private readonly IServiceRolUsuario _rolUsuarioService;



        public UsuarioController(IServiceUsuario servicioUsuario, IServiceRolUsuario rolUsuarioService)
        {
            _servicioUsuario = servicioUsuario;
            _rolUsuarioService = rolUsuarioService;
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

        [HttpGet]
        public async Task<IActionResult> DetailsModal(int id)
        {
            var objeto = await _servicioUsuario.GetByIdAsync(id);
            if (objeto == null) return NotFound();
            return PartialView("_UsuarioDetailsPartial", objeto);
        }


        private async Task LoadCombosAsync(int? idRolSeleccionado = null)
        {
            var roles = await _rolUsuarioService.ListAsync();
            ViewBag.Roles = new SelectList(
                roles,
                nameof(RolUsuarioDTO.IdRol),     // valor del <option>
                nameof(RolUsuarioDTO.Nombre),    // texto visible
                idRolSeleccionado                // seleccionado
            );
        }

        // GET: Usuario/Create
        //Carga los combos de roles para el formulario si no se cae esa picha
        public async Task<IActionResult> Create()
        {
            await LoadCombosAsync();
            return View(new UsuarioDTO());
        }

        //El metodo controlador para crear un nuevo usuario, recibe un DTO con los datos del formulario, valida el modelo y si es valido lo agrega a la base de datos, luego redirige al Index
        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCombosAsync(dto.IdRol);
                return View(dto);
            }

            await _servicioUsuario.AddAsync(dto);
            return RedirectToAction(nameof(Index));
        }




    }
}
