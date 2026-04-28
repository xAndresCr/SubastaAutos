using Microsoft.AspNetCore.Mvc;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Web.ViewModels;

namespace SubastaAutos.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IServiceUsuario _serviceUsuario;

        public LoginController(IServiceUsuario serviceUsuario)
        {
            _serviceUsuario = serviceUsuario;
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            // Si ya tiene sesión redirige al inicio
            if (HttpContext.Session.GetString("UsuarioNombre") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(ViewModelLogin model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var usuario = await _serviceUsuario.LoginAsync(model.User, model.Password);

                if (usuario == null)
                {
                    ViewBag.Message = "Correo o contraseña incorrectos.";
                    return View(model);
                }

                if (!usuario.EstadoUsuario)
                {
                    ViewBag.Message = "Su cuenta está desactivada. Contacte al administrador.";
                    return View(model);
                }

                // Guardar datos en sesión
                HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
                HttpContext.Session.SetString("UsuarioNombre", usuario.NombreCompleto);
                HttpContext.Session.SetInt32("UsuarioRol", usuario.IdRol);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear(); // ← limpia toda la sesión
            return RedirectToAction("LogIn", "Login");
        }

    }

}
