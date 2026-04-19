using Libreria.Web.Util;
using Microsoft.AspNetCore.Mvc;
using SubastaAutos.Application.Services.Interfaces;

namespace SubastaAutos.Web.Controllers
{
    public class PagoController : Controller
    {
        private readonly IServicePago _servicePago;
        private readonly IServiceSubasta _serviceSubasta;

        public PagoController(
         IServicePago servicePago,
         IServiceSubasta serviceSubasta)
        {
            _servicePago = servicePago;
            _serviceSubasta = serviceSubasta;
        }
        //Solo para validar usuario en la sesion mirey
        private int GetUsuarioActualId()
        {
            return HttpContext.Session.GetInt32("UsuarioSimulado") ?? 1;
        }

        //Ver el pago de la subasta
        [HttpGet]
        public async Task<IActionResult> Detalle(int idSubasta)
        {
            try
            {
                var subasta = await _serviceSubasta.FindByIdAsync(idSubasta);
                if (subasta == null)
                    throw new Exception("Subasta no encontrada");

                // Obtener el ganador
                var ganador = subasta.Pujas
                    .OrderByDescending(p => p.Monto)
                    .FirstOrDefault();

                // ← Verificar que el usuario actual es el ganador
                if (ganador == null || ganador.IdUsuario != GetUsuarioActualId())
                {
                    TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                        "Acceso denegado",
                        "Solo el usuario ganador puede realizar el pago.",
                        SweetAlertMessageType.warning);
                    return RedirectToAction("Index", "Subasta");
                }

                // Si llegó aquí, es el ganador
                var pago = await _servicePago.GetBySubastaAsync(idSubasta);
                ViewBag.IdSubasta = idSubasta;
                return View(pago);
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "Error", ex.Message, SweetAlertMessageType.error);
                return RedirectToAction("Index", "Subasta");
            }

        }

        //Reistrar pago AJAX
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] int idSubasta)
        {
            try
            {
                await _servicePago.RegistrarPagoAsync(idSubasta);
                var pago = await _servicePago.GetBySubastaAsync(idSubasta);

                return Json(new
                {
                    success = true,
                    mensaje = "Pago registrado exitosamente.",
                    idPago = pago?.IdPago,
                    monto = pago?.Monto,
                    estado = pago?.EstadoPago,
                    fechaRegistro = pago?.FechaRegistro?.ToString("dd/MM/yyyy HH:mm")
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }
        //Confirmar pago AJAX
        [HttpPost]
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Confirmar([FromBody] int idPago)
        {
            try
            {
                await _servicePago.ConfirmarPagoAsync(idPago);
                var pago = await _servicePago.GetByIdAsync(idPago);

                return Json(new
                {
                    success = true,
                    mensaje = "Pago confirmado exitosamente.",
                    estado = pago?.EstadoPago
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }
    }
}

