using Libreria.Web.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Web.Hubs;

namespace SubastaAutos.Web.Controllers
{
    public class SubastaController : Controller
    {
        private readonly IServiceSubasta _serviceSubasta;
        private readonly IServicePuja _servicePuja;
        private readonly IServiceAuto _serviceAuto;
        private readonly IHubContext<SubastaHub> _hubContext;
        private readonly IServicePago _servicePago;

        public SubastaController(
            IServiceSubasta serviceSubasta,
            IServicePuja servicePuja,
            IServiceAuto serviceAuto,
            IServicePago servicePago,
            IHubContext<SubastaHub> hubContext)
        {
            _serviceSubasta = serviceSubasta;
            _servicePuja = servicePuja;
            _serviceAuto = serviceAuto;
            _servicePago = servicePago;
            _hubContext = hubContext;
        }

        // ← Leer usuario real de la sesión
        private int GetUsuarioActualId()
        {
            return HttpContext.Session.GetInt32("UsuarioId") ?? 0;
        }

        // ── LISTADO PÚBLICO: Activas
        public async Task<IActionResult> Index()
        {
            
            var collection = await _serviceSubasta.ListActivasAsync();
            return View(collection);
        }

        // ── LISTADO PÚBLICO: Finalizadas
        public async Task<IActionResult> Finalizadas()
        {
            var collection = await _serviceSubasta.ListFinalizadasAsync();
            return View(collection);
        }

        // ── LISTADO ADMIN: Todas
        public async Task<IActionResult> IndexAdmin()
        {
            var usuarioId = GetUsuarioActualId();
            var collection = await _serviceSubasta.ListByVendedorAsync(usuarioId);
            return View(collection);
        }

        // ── DETALLE
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                    return RedirectToAction(nameof(IndexAdmin));

                var dto = await _serviceSubasta.FindByIdAsync(id.Value);
                if (dto == null)
                    throw new Exception("Subasta no encontrada.");

                var usuarioActualId = GetUsuarioActualId(); // ← sesión real

                ViewBag.PujaFueSuperada = await _servicePuja
                    .PujaFueSuperadaAsync(id.Value, usuarioActualId);
                ViewBag.UsuarioActualId = usuarioActualId;

                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", ex.Message, SweetAlertMessageType.error);
                return RedirectToAction(nameof(IndexAdmin));
            }
        }

        // ── MIS SUBASTAS
        public async Task<IActionResult> MisSubastas()
        {
            var usuarioId = GetUsuarioActualId();
            if (usuarioId == 0)
                return RedirectToAction("LogIn", "Login");

            var subastas = await _serviceSubasta.ListSubastasGanadasAsync(usuarioId);

            ViewBag.PagosExpirados = new HashSet<int>();
            ViewBag.PagosSinRegistrar = new HashSet<int>();
            ViewBag.PagosConfirmados = new HashSet<int>();

            foreach (var subasta in subastas)
            {
                var pago = await _servicePago.GetBySubastaAsync(subasta.IdSubasta);
                if (pago == null)
                {
                    if (subasta.FechaCierre.AddHours(24) < DateTime.Now)
                        ((HashSet<int>)ViewBag.PagosExpirados).Add(subasta.IdSubasta);
                    else
                        ((HashSet<int>)ViewBag.PagosSinRegistrar).Add(subasta.IdSubasta);
                }
                else if (pago.IdEstadoPago == 2)
                {
                    ((HashSet<int>)ViewBag.PagosConfirmados).Add(subasta.IdSubasta);
                }
                else if (pago.EstadoPago == "Pendiente" &&
                         pago.FechaRegistro.HasValue &&
                         pago.FechaRegistro.Value.AddHours(24) < DateTime.Now)
                {
                    ((HashSet<int>)ViewBag.PagosExpirados).Add(subasta.IdSubasta);
                }
            }

            return View(subastas);
        }

        // ── PUJAS (historial)
        public async Task<IActionResult> Pujas(int? id)
        {
            try
            {
                if (id == null)
                    return RedirectToAction(nameof(Index));

                var collection = await _servicePuja.ListBySubastaAsync(id.Value);
                ViewBag.IdSubasta = id.Value;
                return View(collection);
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", ex.Message, SweetAlertMessageType.error);
                return RedirectToAction(nameof(Index));
            }
        }

        // ── AJAX: REGISTRAR PUJA
        [HttpPost]
        public async Task<IActionResult> Pujar([FromBody] PujaDTO dto)
        {
            try
            {
                await _servicePuja.AddAsync(dto, GetUsuarioActualId());

                var lider = await _servicePuja.GetPujaLiderAsync(dto.IdSubasta);
                var subasta = await _serviceSubasta.FindByIdAsync(dto.IdSubasta);
                decimal montoSiguiente = (lider?.Monto ?? subasta!.PrecioBase) + subasta!.IncrementoMinimo;

                await _hubContext.Clients
                    .Group($"subasta-{dto.IdSubasta}")
                    .SendAsync("NuevaPuja", new
                    {
                        montoLider = lider?.Monto,
                        nombreLider = lider?.NombrePostor,
                        fechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        nombrePostor = lider?.NombrePostor,
                        monto = dto.Monto,
                        idUsuarioLider = lider?.IdUsuario,
                        idUsuarioQuePujo = GetUsuarioActualId(),
                        montoSiguiente
                    });

                return Json(new
                {
                    success = true,
                    mensaje = "Puja registrada exitosamente.",
                    montoLider = lider?.Monto,
                    nombreLider = lider?.NombrePostor,
                    nombrePostor = lider?.NombrePostor,
                    fechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    monto = dto.Monto,
                    montoSiguiente
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

        // ── VISTA DE PUJAS (GET)
        [HttpGet]
        public async Task<IActionResult> Pujar(int id)
        {
            try
            {
                var dto = await _serviceSubasta.FindByIdAsync(id);
                if (dto == null)
                    throw new Exception("Subasta no encontrada.");

                var usuarioActualId = GetUsuarioActualId();

                ViewBag.PujaFueSuperada = await _servicePuja
                    .PujaFueSuperadaAsync(id, usuarioActualId);
                ViewBag.UsuarioActualId = usuarioActualId;
                ViewBag.EsVendedor = (usuarioActualId == dto.IdVendedor);

                var pujaMax = dto.Pujas.OrderByDescending(p => p.Monto).FirstOrDefault();
                ViewBag.MontoSiguientePuja = pujaMax != null
                    ? pujaMax.Monto + dto.IncrementoMinimo
                    : dto.PrecioBase + dto.IncrementoMinimo;

                ViewBag.EsGanador = false;
                ViewBag.PagoRegistrado = false;
                ViewBag.PagoConfirmado = false;
                ViewBag.IdPago = 0;
                ViewBag.EstadoPagoNombre = "Sin registrar";

                if (dto.IdEstadoSubasta == 2)
                {
                    var pujaGanadora = dto.Pujas
                        .OrderByDescending(p => p.Monto)
                        .FirstOrDefault();

                    if (pujaGanadora != null && pujaGanadora.IdUsuario == usuarioActualId)
                    {
                        ViewBag.EsGanador = true;
                        try
                        {
                            var pago = await _servicePago.GetBySubastaAsync(id);
                            if (pago != null)
                            {
                                ViewBag.PagoRegistrado = true;
                                ViewBag.IdPago = pago.IdPago;
                                ViewBag.PagoConfirmado = pago.IdEstadoPago == 2;
                                ViewBag.EstadoPagoNombre = pago.EstadoPago;
                            }
                        }
                        catch { }
                    }
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

        [HttpGet]
        public async Task<IActionResult> EstadoSubasta(int id)
        {
            try
            {
                var subasta = await _serviceSubasta.FindByIdAsync(id);
                if (subasta == null)
                    return Json(new { success = false });

                var lider = await _servicePuja.GetPujaLiderAsync(id);
                bool pujaFueSuperada = await _servicePuja
                    .PujaFueSuperadaAsync(id, GetUsuarioActualId());

                await _hubContext.Clients
                    .Group($"subasta-{id}")
                    .SendAsync("EstadoActualizado", new
                    {
                        idEstado = subasta.IdEstadoSubasta,
                        estadoNombre = subasta.EstadoSubasta,
                        montoLider = lider?.Monto ?? subasta.PrecioBase,
                        nombreLider = lider?.NombrePostor ?? "Sin pujas",
                        pujaFueSuperada
                    });

                return Json(new
                {
                    success = true,
                    idEstado = subasta.IdEstadoSubasta,
                    estadoNombre = subasta.EstadoSubasta,
                    montoLider = lider?.Monto ?? subasta.PrecioBase,
                    nombreLider = lider?.NombrePostor ?? "Sin pujas",
                    pujaFueSuperada
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CerrarSubasta(int id)
        {
            try
            {
                await _serviceSubasta.CerrarAsync(id);

                var lider = await _servicePuja.GetPujaLiderAsync(id);

                await _hubContext.Clients
                    .Group($"subasta-{id}")
                    .SendAsync("SubastaCerrada", new
                    {
                        mensaje = lider != null
                            ? "La subasta ha finalizado."
                            : "La subasta ha finalizado sin ofertas.",
                        ganador = lider?.NombrePostor ?? "",
                        montoFinal = lider?.Monto ?? 0,
                        idUsuarioGanador = lider?.IdUsuario ?? 0,
                        huboPujas = lider != null
                    });

                return Json(new { success = true, mensaje = "Subasta cerrada exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // ── COMBOS
        private async Task LoadCombosAsync(int? selectedAutoId = null)
        {
            var usuarioActualId = GetUsuarioActualId(); // ← sesión real
            var autos = await _serviceAuto.ListAsync();
            var subastas = await _serviceSubasta.ListAsync();

            var autosConSubastaPendiente = subastas
                .Where(s => s.EstadoSubasta == "Activa" || s.EstadoSubasta == "Borrador")
                .Select(s => s.IdAuto)
                .ToHashSet();

            var autosVendidos = subastas
                .Where(s => s.EstadoSubasta == "Finalizada" && s.CantidadPujas > 0)
                .Select(s => s.IdAuto)
                .ToHashSet();

            var autosDisponibles = autos
                .Where(a => a.EstadoAuto == "Activo"
                            && !autosConSubastaPendiente.Contains(a.IdAuto)
                            && !autosVendidos.Contains(a.IdAuto)
                            || a.IdAuto == selectedAutoId)
                .ToList();

            ViewBag.ListAutos = new SelectList(
                autosDisponibles.Select(a => new {
                    a.IdAuto,
                    Descripcion = $"{a.NombreAuto} — VIN: {a.Vin}"
                }),
                "IdAuto",
                "Descripcion",
                selectedAutoId);

            // ← Nombre del vendedor desde sesión real
            var autoVendedor = autos.FirstOrDefault(a => a.IdVendedor == usuarioActualId);
            ViewBag.VendedorNombre = autoVendedor?.Propietario
                ?? HttpContext.Session.GetString("UsuarioNombre")
                ?? "Usuario";
        }

        // ── CREATE GET
        public async Task<IActionResult> Create()
        {
            await LoadCombosAsync();
            return View(new SubastaDTO
            {
                IdVendedor = GetUsuarioActualId(), // ← sesión real
                IdEstadoSubasta = 4,
                FechaCreacion = DateTime.Now,
                FechaInicio = DateTime.Now.AddDays(1),
                FechaCierre = DateTime.Now.AddDays(8)
            });
        }

        // ── CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubastaDTO dto)
        {
            dto.IdVendedor = GetUsuarioActualId(); // ← sesión real
            dto.IdEstadoSubasta = 4;
            dto.FechaCreacion = DateTime.Now;

            ModelState.Remove("NombreAuto");
            ModelState.Remove("ImagenPrincipalAuto");
            ModelState.Remove("Vendedor");
            ModelState.Remove("EstadoSubasta");

            if (dto.FechaInicio <= DateTime.Now)
                ModelState.AddModelError("FechaInicio",
                    "La fecha de inicio debe ser posterior al momento actual.");

            if (dto.FechaCierre <= dto.FechaInicio)
                ModelState.AddModelError("FechaCierre",
                    "La fecha de cierre debe ser posterior a la fecha de inicio.");

            if (!ModelState.IsValid)
            {
                var errores = string.Join("<br>",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
                    "Errores de validación",
                    $"El formulario contiene errores:<br>{errores}",
                    SweetAlertMessageType.warning);

                await LoadCombosAsync(dto.IdAuto);
                return View(dto);
            }

            try
            {
                await _serviceSubasta.AddAsync(dto);
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Subasta creada",
                    "La subasta fue creada como borrador exitosamente.",
                    SweetAlertMessageType.success);
                return RedirectToAction(nameof(IndexAdmin));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Acción no permitida", ex.Message, SweetAlertMessageType.warning);
                await LoadCombosAsync(dto.IdAuto);
                return View(dto);
            }
        }

        // ── EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                bool puedeEditar = await _serviceSubasta.PuedeEditarAsync(id);
                if (!puedeEditar)
                {
                    TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                        "Acción no permitida",
                        "No se puede editar: la subasta ya inició o tiene pujas.",
                        SweetAlertMessageType.warning);
                    return RedirectToAction(nameof(IndexAdmin));
                }

                var dto = await _serviceSubasta.FindByIdAsync(id);
                if (dto == null)
                    throw new Exception("Subasta no encontrada.");

                await LoadCombosAsync(dto.IdAuto);
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", ex.Message, SweetAlertMessageType.error);
                return RedirectToAction(nameof(IndexAdmin));
            }
        }

        // ── EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubastaDTO dto)
        {
            ModelState.Remove("NombreAuto");
            ModelState.Remove("ImagenPrincipalAuto");
            ModelState.Remove("Vendedor");
            ModelState.Remove("EstadoSubasta");
            ModelState.Remove("IdAuto");
            ModelState.Remove("FechaCreacion");

            if (dto.FechaInicio <= DateTime.Now)
                ModelState.AddModelError("FechaInicio",
                    "La fecha de inicio debe ser posterior al momento actual.");

            if (dto.FechaCierre <= dto.FechaInicio)
                ModelState.AddModelError("FechaCierre",
                    "La fecha de cierre debe ser posterior a la fecha de inicio.");

            if (!ModelState.IsValid)
            {
                var errores = string.Join("<br>",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
                    "Errores de validación",
                    $"El formulario contiene errores:<br>{errores}",
                    SweetAlertMessageType.warning);

                await LoadCombosAsync(dto.IdAuto);
                return View(dto);
            }

            try
            {
                await _serviceSubasta.UpdateAsync(id, dto);
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Subasta actualizada",
                    "La subasta fue modificada exitosamente.",
                    SweetAlertMessageType.success);
                return RedirectToAction(nameof(IndexAdmin));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Acción no permitida", ex.Message, SweetAlertMessageType.warning);
                return RedirectToAction(nameof(IndexAdmin));
            }
        }

        // ── PUBLICAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publicar(int id)
        {
            try
            {
                await _serviceSubasta.PublicarAsync(id);
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Subasta publicada",
                    "La subasta fue publicada exitosamente y ya es visible.",
                    SweetAlertMessageType.success);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Acción no permitida", ex.Message, SweetAlertMessageType.warning);
            }
            return RedirectToAction(nameof(IndexAdmin));
        }

        // ── CANCELAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                await _serviceSubasta.CancelarAsync(id);
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Subasta cancelada",
                    "La subasta fue cancelada exitosamente.",
                    SweetAlertMessageType.success);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Acción no permitida", ex.Message, SweetAlertMessageType.warning);
            }
            return RedirectToAction(nameof(IndexAdmin));
        }
    }
}