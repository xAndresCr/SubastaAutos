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

        // ← Leer usuario del login real
        private int GetUsuarioActualId()
        {
            return HttpContext.Session.GetInt32("UsuarioId") ?? 0;
        }

        // Ver el pago de la subasta
        [HttpGet]
        public async Task<IActionResult> Detalle(int idSubasta)
        {
            try
            {
                var subasta = await _serviceSubasta.FindByIdAsync(idSubasta);
                if (subasta == null)
                    throw new Exception("Subasta no encontrada.");

                var ganador = subasta.Pujas
                    .OrderByDescending(p => p.Monto)
                    .FirstOrDefault();

                if (ganador == null || ganador.IdUsuario != GetUsuarioActualId())
                {
                    TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                        "Acceso denegado",
                        "Solo el usuario ganador puede realizar el pago.",
                        SweetAlertMessageType.warning);
                    return RedirectToAction("Index", "Subasta");
                }

                // ← Validar que no han pasado más de 24 horas
                if (subasta.FechaCierre.AddHours(24) < DateTime.Now)
                {
                    var pago = await _servicePago.GetBySubastaAsync(idSubasta);
                    // Si no hay pago confirmado, redirigir
                    if (pago == null || pago.IdEstadoPago != 2)
                    {
                        TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                            "Pago expirado",
                            "El plazo para realizar el pago ha expirado (24 horas).",
                            SweetAlertMessageType.warning);
                        return RedirectToAction("MisSubastas", "Subasta");
                    }
                }

                var pagoDetalle = await _servicePago.GetBySubastaAsync(idSubasta);
                ViewBag.IdSubasta = idSubasta;
                return View(pagoDetalle);
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", ex.Message, SweetAlertMessageType.error);
                return RedirectToAction("Index", "Subasta");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetalleParcial(int idSubasta)
        {
            try
            {
                var subasta = await _serviceSubasta.FindByIdAsync(idSubasta);
                if (subasta == null)
                    return PartialView("_PagoDetalle", null);

                // ← Pasar estado de expiración al ViewBag
                ViewBag.IdSubasta = idSubasta;
                ViewBag.PagoExpirado = subasta.FechaCierre.AddHours(24) < DateTime.Now;

                var pago = await _servicePago.GetBySubastaAsync(idSubasta);

                // Si expiró y no está confirmado, no mostrar pago
                if (ViewBag.PagoExpirado && (pago == null || pago.IdEstadoPago != 2))
                    return PartialView("_PagoDetalle", null);

                return PartialView("_PagoDetalle", pago);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return PartialView("_PagoDetalle", null);
            }
        }

        // Registrar pago AJAX
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

        // Confirmar pago AJAX
        [HttpPost] // ← Solo un [HttpPost]
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

