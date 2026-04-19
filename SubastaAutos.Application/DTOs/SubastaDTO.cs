using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SubastaAutos.Application.DTOs
{
    public record SubastaDTO
    {
        public int IdSubasta { get; set; }

        // ── Campos editables (Create/Edit) ──────────────────────

        [DisplayName("Auto")]
        [Required(ErrorMessage = "Debe seleccionar un auto")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un auto")]
        public int IdAuto { get; set; }

        //este debió ser el correcto
        //[Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un auto.")]
       // public int IdAuto { get; set; }

        [DisplayName("Fecha de Inicio")]
        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha de Cierre")]
        [Required(ErrorMessage = "La fecha de cierre es requerida")]
        public DateTime FechaCierre { get; set; }

        [DisplayName("Precio Base")]
        [Required(ErrorMessage = "El precio base es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio base debe ser mayor a 0")]
        public decimal PrecioBase { get; set; }

        [DisplayName("Incremento Mínimo")]
        [Required(ErrorMessage = "El incremento mínimo es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El incremento mínimo debe ser mayor a 0")]
        public decimal IncrementoMinimo { get; set; }

        // ── Campos asignados internamente ───────────────────────

        public int IdVendedor { get; set; }
        public int IdEstadoSubasta { get; set; }

        [DisplayName("Fecha de Creación")]
        public DateTime? FechaCreacion { get; set; }

        // ── Campos calculados (solo lectura) ────────────────────

        [DisplayName("Auto")]
        public string NombreAuto { get; set; } = string.Empty;

        [DisplayName("Imagen")]
        public string ImagenPrincipalAuto { get; set; } = string.Empty;

        [DisplayName("Vendedor")]
        public string Vendedor { get; set; } = string.Empty;

        [DisplayName("Estado")]
        public string EstadoSubasta { get; set; } = string.Empty;

        [DisplayName("Pujas")]
        public int CantidadPujas { get; set; }

        // ── Sublista ────────────────────────────────────────────

        [DisplayName("Historial de Pujas")]
        public List<PujaDTO> Pujas { get; set; } = new();
        [DisplayName("Descripción")]
        public string DescripcionAuto { get; set; } = string.Empty;

        [DisplayName("Imágenes")]
        public List<AutoImagenDTO> AutoImagenes { get; set; } = new();
    }
}