using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.DTOs
{
    public record PagoDTO
    {
        public int IdPago { get; set; }
        public int IdSubasta { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public decimal Monto { get; set; }
        public int IdEstadoPago { get; set; }

        // Campos calculados para mostrar en la vista
        public string EstadoPago { get; set; } = string.Empty;
        public string NombreAuto { get; set; } = string.Empty;
        public string NombreGanador { get; set; } = string.Empty;
    }
}
