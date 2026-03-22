using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SubastaAutos.Application.DTOs
{
    public record AutoDTO
    {
        public int IdAuto { get; set; }

        [DisplayName("VIN")]
        [Required(ErrorMessage = "El VIN es requerido")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "El VIN debe tener entre {2} y {1} caracteres")]
        public string Vin { get; set; } = string.Empty;

        [DisplayName("Marca")]
        [Required(ErrorMessage = "La marca es requerida")]
        [StringLength(50)]
        public string Marca { get; set; } = string.Empty;

        [DisplayName("Modelo")]
        [Required(ErrorMessage = "El modelo es requerido")]
        [StringLength(50)]
        public string Modelo { get; set; } = string.Empty;

        [DisplayName("Año")]
        [Required(ErrorMessage = "El año es requerido")]
        [Range(1900, 2030, ErrorMessage = "El año debe estar entre {1} y {2}")]
        public int Anio { get; set; }

        [DisplayName("Descripción")]
        [Required(ErrorMessage = "La descripción es requerida")]
        [MinLength(20, ErrorMessage = "La descripción debe tener al menos {1} caracteres")]
        [StringLength(500)]
        public string? Descripcion { get; set; }

        [DisplayName("Condición")]
        [Required(ErrorMessage = "Debe seleccionar una condición")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una condición")]
        public int IdCondicionAuto { get; set; }

        public int IdEstadoAuto { get; set; }

        public int IdVendedor { get; set; }

        [DisplayName("Fecha de Registro")]
        public DateTime? FechaRegistro { get; set; }

  

        [DisplayName("Auto")]
        public string NombreAuto { get; set; } = string.Empty;

        [DisplayName("Propietario")]
        public string Propietario { get; set; } = string.Empty;

        [DisplayName("Condición")]
        public string Condicion { get; set; } = string.Empty;

        [DisplayName("Estado")]
        public string EstadoAuto { get; set; } = string.Empty;

        [DisplayName("Imagen Principal")]
        public string ImagenPrincipal { get; set; } = string.Empty;

        // ── Relaciones ──

        [DisplayName("Categorías")]
        public List<CategoriaDTO> IdCategoria { get; set; } = new();

        [DisplayName("Imágenes")]
        public List<AutoImagenDTO> AutoImagen { get; set; } = new();

        [DisplayName("Subastas")]
        public List<SubastaResumenDTO> Subasta { get; set; } = new();
    }
}