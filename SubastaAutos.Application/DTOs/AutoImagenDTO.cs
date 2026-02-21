using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.DTOs
{
    public record AutoImagenDTO
    {
        public int IdAutoImagen { get; set; }
        public int IdAuto { get; set; }
        public string UrlImagen { get; set; } = string.Empty;
        public bool EsPrincipal { get; set; }
    }
}
