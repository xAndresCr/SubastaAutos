using Libreria.Application.Utils;
using Libreria.Web.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration.UserSecrets;
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
                nameof(RolUsuarioDTO.IdRol),    
                nameof(RolUsuarioDTO.Nombre),    // texto visible
                idRolSeleccionado                // seleccionado
            );
        }

        // GET: Usuario/Create
        //Carga los combos de roles para el formulario si no se cae ese serote
        public async Task<IActionResult> Create()
        {
            await LoadCombosAsync();
            return View(new UsuarioDTO());
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var dto = await _servicioUsuario.GetByIdAsync(id);
                if (dto == null)
                {
                    TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                        "No encontrado",
                        "El usuario no existe.",
                        SweetAlertMessageType.warning);
                    return RedirectToAction(nameof(Index));
                }
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", ex.Message, SweetAlertMessageType.error);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioDTO dto)
        {
            ModelState.Remove("IdRol");
            ModelState.Remove("FechaRegistro");
            ModelState.Remove("IdRolNavigation");
            ModelState.Remove("IdRolNavigation.Nombre");
            ModelState.Remove("PasswordHash");
            ModelState.Remove("RolUsuario");

            if (!ModelState.IsValid)
            {
                var errores = string.Join("<br>",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
                    "Errores de validación",
                    $"El formulario contiene errores:<br>{errores}",
                    SweetAlertMessageType.warning);
                return View(dto);
            }
            try
            {
                await _servicioUsuario.UpdateAsync(id, dto);
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Usuario actualizado",
                    $"El usuario {dto.NombreCompleto} fue modificado exitosamente.",
                    SweetAlertMessageType.success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", ex.Message, SweetAlertMessageType.error);
                return View(dto);
            }
        }

        // ── BLOQUEAR / ACTIVAR ─
        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                await _servicioUsuario.ToggleEstadoAsync(id);
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Estado actualizado",
                    "El estado del usuario fue cambiado exitosamente.",
                    SweetAlertMessageType.success);
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", ex.Message, SweetAlertMessageType.error);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
