using System.ComponentModel;

namespace SubastaAutos.Application.DTOs
{
    public record SubastaDTO
    {
        public int IdSubasta { get; set; }

        // ── Campos calculados (NO existen en la tabla Subasta) ───────────

        [DisplayName("Auto")]
        public string NombreAuto { get; set; } = string.Empty;
        // CALCULADO: Marca + " " + Modelo + " " + Anio del auto relacionado

        [DisplayName("Imagen")]
        public string ImagenPrincipalAuto { get; set; } = string.Empty;
        // CALCULADO: base64 de la imagen principal del auto

        [DisplayName("Vendedor")]
        public string Vendedor { get; set; } = string.Empty;
        // CALCULADO: NombreCompleto del usuario vendedor

        [DisplayName("Estado")]
        public string EstadoSubasta { get; set; } = string.Empty;
        // CALCULADO: Nombre del EstadoSubasta

        [DisplayName("Pujas")]
        public int CantidadPujas { get; set; }
        // CALCULADO: Count de la colección Puja (NO se almacena en BD)

        // ── Campos directos de la tabla Subasta ──────────────────────────

        [DisplayName("Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha de Cierre")]
        public DateTime FechaCierre { get; set; }

        [DisplayName("Precio Base")]
        public decimal PrecioBase { get; set; }

        [DisplayName("Incremento Mínimo")]
        public decimal IncrementoMinimo { get; set; }

        [DisplayName("Fecha de Creación")]
        public DateTime? FechaCreacion { get; set; }

        // ── Sublista para el historial de pujas en Details ───────────────

        [DisplayName("Historial de Pujas")]
        public List<PujaDTO> Pujas { get; set; } = new();
    }
}
