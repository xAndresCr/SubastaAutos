using Libreria.Web.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;

namespace SubastaAutos.Web.Controllers
{
    public class SubastaController : Controller
    {
        private readonly IServiceSubasta _serviceSubasta;
        private readonly IServicePuja _servicePuja;
        private readonly IServiceAuto _serviceAuto;

        private const int VendedorSimuladoId = 1;

        public SubastaController(
            IServiceSubasta serviceSubasta,
            IServicePuja servicePuja,
            IServiceAuto serviceAuto)
        {
            _serviceSubasta = serviceSubasta;
            _servicePuja = servicePuja;
            _serviceAuto = serviceAuto;
        }

        // ── LISTADO PÚBLICO: Activas ────────────────────────────
        public async Task<IActionResult> Index()
        {
            var collection = await _serviceSubasta.ListActivasAsync();
            return View(collection);
        }

        // ── LISTADO PÚBLICO: Finalizadas ────────────────────────
        public async Task<IActionResult> Finalizadas()
        {
            var collection = await _serviceSubasta.ListFinalizadasAsync();
            return View(collection);
        }

        // ── LISTADO ADMIN: Todas ────────────────────────────────
        public async Task<IActionResult> IndexAdmin()
        {
            var collection = await _serviceSubasta.ListAsync();
            return View(collection);
        }

        // ── DETALLE ─────────────────────────────────────────────
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                    return RedirectToAction(nameof(IndexAdmin));

                var dto = await _serviceSubasta.FindByIdAsync(id.Value);
                if (dto == null)
                    throw new Exception("Subasta no encontrada.");

                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", ex.Message, SweetAlertMessageType.error);
                return RedirectToAction(nameof(IndexAdmin));
            }
        }

        // PUJAS (historial) 
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
                throw new Exception(ex.Message);
            }
        }

        private async Task LoadCombosAsync(int? selectedAutoId = null)
        {
            var autos = await _serviceAuto.ListAsync();
            var subastas = await _serviceSubasta.ListAsync();

            // Autos con subasta activa o borrador (no disponibles)
            var autosConSubastaPendiente = subastas
                .Where(s => s.EstadoSubasta == "Activa" || s.EstadoSubasta == "Borrador")
                .Select(s => s.IdAuto)
                .ToHashSet();

            // Autos vendidos: subasta finalizada CON pujas
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

            var autoVendedor = autos.FirstOrDefault(a => a.IdVendedor == VendedorSimuladoId);
            ViewBag.VendedorNombre = autoVendedor?.Propietario ?? "Usuario #1";
        }

        // CREATE GET 
        public async Task<IActionResult> Create()
        {
            await LoadCombosAsync();
            return View(new SubastaDTO
            {
                IdVendedor = VendedorSimuladoId,
                IdEstadoSubasta = 4, // Borrador
                FechaCreacion = DateTime.Now,
                FechaInicio = DateTime.Now.AddDays(1),
                FechaCierre = DateTime.Now.AddDays(8)
            });
        }

        // CREATE POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubastaDTO dto)
        {
            // Forzar valores internos
            dto.IdVendedor = VendedorSimuladoId;
            dto.IdEstadoSubasta = 4; // Borrador
            dto.FechaCreacion = DateTime.Now;

            // Quitar validaciones de campos calculados
            ModelState.Remove("NombreAuto");
            ModelState.Remove("ImagenPrincipalAuto");
            ModelState.Remove("Vendedor");
            ModelState.Remove("EstadoSubasta");

            // Validación: fecha inicio debe ser futura
            if (dto.FechaInicio <= DateTime.Now)
                ModelState.AddModelError("FechaInicio",
                    "La fecha de inicio debe ser posterior al momento actual.");

            // Validación: fecha cierre > fecha inicio
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

        // EDIT GET 
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

        // EDIT POST
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

            // Validación: fecha inicio debe ser futura
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

        //PUBLICAR
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

        //CANCELAR 
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