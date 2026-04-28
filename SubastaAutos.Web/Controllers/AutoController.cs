using Libreria.Web.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SubastaAutos.Application.DTOs;
using SubastaAutos.Application.Services.Interfaces;
using SubastaAutos.Web.Filters;

namespace SubastaAutos.Web.Controllers
{
    [RolAutorizado(3)]
    public class AutoController : Controller
    {
        private readonly IServiceAuto _serviceAuto;
        private readonly IServiceCategoria _serviceCategoria;
        private readonly IServiceCondicionAuto _serviceCondicion;
        private readonly IServiceEstadoAuto _serviceEstado;

        public AutoController(
            IServiceAuto serviceAuto,
            IServiceCategoria serviceCategoria,
            IServiceCondicionAuto serviceCondicion,
            IServiceEstadoAuto serviceEstado)
        {
            _serviceAuto = serviceAuto;
            _serviceCategoria = serviceCategoria;
            _serviceCondicion = serviceCondicion;
            _serviceEstado = serviceEstado;
        }

        private int GetUsuarioActualId()
        {
            return HttpContext.Session.GetInt32("UsuarioId") ?? 0;
        }

        private string GetUsuarioNombre()
        {
            return HttpContext.Session.GetString("UsuarioNombre") ?? "Vendedor";
        }

        // ── LISTADO PÚBLICO ──────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var collection = await _serviceAuto.ListAsync();
            return View(collection);
        }

        // ── LISTADO ADMIN ────────────────────────────────────────
        public async Task<IActionResult> IndexAdmin()
        {
            var collection = await _serviceAuto.ListAsync();
            return View(collection);
        }

        // ── DETALLE ──────────────────────────────────────────────
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                    return RedirectToAction(nameof(IndexAdmin));

                var dto = await _serviceAuto.FindByIdAsync(id.Value);
                if (dto == null)
                    throw new Exception("Auto no encontrado.");

                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", ex.Message, SweetAlertMessageType.error);
                return RedirectToAction(nameof(IndexAdmin));
            }
        }

        // ── COMBOS ───────────────────────────────────────────────
        private async Task LoadCombosAsync(IEnumerable<string>? selectedCategoriaIds = null)
        {
            var condiciones = await _serviceCondicion.ListAsync();
            ViewBag.ListCondicion = new SelectList(condiciones,
                nameof(CondicionAutoDTO.IdCondicionAuto),
                nameof(CondicionAutoDTO.Nombre));

            var categorias = await _serviceCategoria.ListAsync();
            ViewBag.ListCategorias = new MultiSelectList(
                items: categorias,
                dataValueField: nameof(CategoriaDTO.IdCategoria),
                dataTextField: nameof(CategoriaDTO.Nombre),
                selectedValues: selectedCategoriaIds);

            // ← Nombre real desde sesión
            ViewBag.VendedorNombre = GetUsuarioNombre();
        }

        // ── CREATE GET ───────────────────────────────────────────
        public async Task<IActionResult> Create()
        {
            await LoadCombosAsync();
            return View(new AutoDTO
            {
                IdEstadoAuto = 1,
                IdVendedor = GetUsuarioActualId() // ← sesión real
            });
        }

        // ── CREATE POST ──────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AutoDTO dto, List<IFormFile> imageFiles, string[] selectedCategorias)
        {
            selectedCategorias ??= Array.Empty<string>();

            // ← Sesión real
            dto.IdVendedor = GetUsuarioActualId();
            dto.IdEstadoAuto = 1;

            if (selectedCategorias.Length == 0)
                ModelState.AddModelError("selectedCategorias",
                    "Debe seleccionar al menos una categoría.");

            if (imageFiles == null || imageFiles.Count == 0)
                ModelState.AddModelError("imageFiles",
                    "Debe seleccionar al menos una imagen.");

            bool vinExiste = await _serviceAuto.ExisteVinAsync(dto.Vin);
            if (vinExiste)
                ModelState.AddModelError("Vin", "Ya existe un vehículo con ese número de VIN.");

            ModelState.Remove("NombreAuto");
            ModelState.Remove("Propietario");
            ModelState.Remove("Condicion");
            ModelState.Remove("EstadoAuto");
            ModelState.Remove("ImagenPrincipal");

            if (!ModelState.IsValid)
            {
                var errores = string.Join("<br>",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
                    "Errores de validación",
                    $"El formulario contiene errores:<br>{errores}",
                    SweetAlertMessageType.warning);

                await LoadCombosAsync(selectedCategorias);
                return View(dto);
            }

            var imagenes = new List<byte[]>();
            foreach (var file in imageFiles)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                imagenes.Add(ms.ToArray());
            }

            await _serviceAuto.AddAsync(dto, selectedCategorias, imagenes);

            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "Auto registrado",
                $"El auto {dto.Marca} {dto.Modelo} fue registrado exitosamente.",
                SweetAlertMessageType.success);

            return RedirectToAction(nameof(IndexAdmin));
        }

        // ── EDIT GET ─────────────────────────────────────────────
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                bool tieneActiva = await _serviceAuto.TieneSubastaActivaAsync(id);
                if (tieneActiva)
                {
                    TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                        "Acción no permitida",
                        "No se puede editar un auto que tiene una subasta activa.",
                        SweetAlertMessageType.warning);
                    return RedirectToAction(nameof(IndexAdmin));
                }

                bool vendido = await _serviceAuto.TieneSubastaFinalizadaAsync(id);
                if (vendido)
                {
                    TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                        "Acción no permitida",
                        "No se puede editar un auto que ya fue vendido.",
                        SweetAlertMessageType.warning);
                    return RedirectToAction(nameof(IndexAdmin));
                }

                var dto = await _serviceAuto.FindByIdAsync(id);
                if (dto == null)
                    throw new Exception("Auto no encontrado.");

                var selected = dto.IdCategoria
                    .Select(c => c.IdCategoria.ToString())
                    .ToList();

                await LoadCombosAsync(selected);
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", ex.Message, SweetAlertMessageType.error);
                return RedirectToAction(nameof(IndexAdmin));
            }
        }

        // ── EDIT POST ────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AutoDTO dto, List<IFormFile>? imageFiles, string[] selectedCategorias)
        {
            selectedCategorias ??= Array.Empty<string>();

            // ← Sesión real
            dto.IdVendedor = GetUsuarioActualId();

            if (selectedCategorias.Length == 0)
                ModelState.AddModelError("selectedCategorias",
                    "Debe seleccionar al menos una categoría.");

            bool vinExiste = await _serviceAuto.ExisteVinAsync(dto.Vin, id);
            if (vinExiste)
                ModelState.AddModelError("Vin", "Ya existe otro vehículo con ese número de VIN.");

            ModelState.Remove("NombreAuto");
            ModelState.Remove("Propietario");
            ModelState.Remove("Condicion");
            ModelState.Remove("EstadoAuto");
            ModelState.Remove("ImagenPrincipal");
            ModelState.Remove("FechaRegistro");

            if (!ModelState.IsValid)
            {
                var errores = string.Join("<br>",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
                    "Errores de validación",
                    $"El formulario contiene errores:<br>{errores}",
                    SweetAlertMessageType.warning);

                await LoadCombosAsync(selectedCategorias);
                return View(dto);
            }

            List<byte[]>? nuevasImagenes = null;
            if (imageFiles != null && imageFiles.Count > 0)
            {
                nuevasImagenes = new List<byte[]>();
                foreach (var file in imageFiles)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    nuevasImagenes.Add(ms.ToArray());
                }
            }

            await _serviceAuto.UpdateAsync(id, dto, selectedCategorias, nuevasImagenes);

            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "Auto actualizado",
                $"El auto {dto.Marca} {dto.Modelo} fue modificado exitosamente.",
                SweetAlertMessageType.success);

            return RedirectToAction(nameof(IndexAdmin));
        }

        // ACTIVAR / DESACTIVAR 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivarDesactivar(int id)
        {
            try
            {
                await _serviceAuto.ActivarDesactivarAsync(id);
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Estado actualizado",
                    "El estado del auto fue cambiado exitosamente.",
                    SweetAlertMessageType.success);
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", ex.Message, SweetAlertMessageType.error);
            }
            return RedirectToAction(nameof(IndexAdmin));
        }

        // LIMINACIÓN LÓGICA 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarLogico(int id)
        {
            try
            {
                await _serviceAuto.EliminarLogicoAsync(id);
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Auto eliminado",
                    "El auto fue eliminado lógicamente.",
                    SweetAlertMessageType.success);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Acción no permitida", ex.Message, SweetAlertMessageType.warning);
            }
            catch (Exception ex)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Error", ex.Message, SweetAlertMessageType.error);
            }
            return RedirectToAction(nameof(IndexAdmin));
        }
    }
}