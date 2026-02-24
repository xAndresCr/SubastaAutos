using System.ComponentModel;

namespace SubastaAutos.Application.DTOs
{
    public record PujaDTO
    {
        public int IdPuja { get; set; }

        public int IdSubasta { get; set; }

        [DisplayName("Postor")]
        public string NombrePostor { get; set; } = string.Empty;
        // CALCULADO: NombreCompleto del usuario que hizo la puja

        [DisplayName("Monto")]
        public decimal Monto { get; set; }

        [DisplayName("Fecha y Hora")]
        public DateTime? FechaHora { get; set; }
    }
}
