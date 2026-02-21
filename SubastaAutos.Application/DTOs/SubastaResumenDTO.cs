using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SubastaAutos.Application.DTOs
{
    public record SubastaResumenDTO
    {
        [DisplayName("ID de la subasta")]
        public int IdSubasta { get; set; }

        [DisplayName("Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha de Cierre")]
        public DateTime FechaCierre { get; set; }

        [DisplayName("Estado")]
        public string EstadoSubasta { get; set; } = string.Empty;
    }
}
