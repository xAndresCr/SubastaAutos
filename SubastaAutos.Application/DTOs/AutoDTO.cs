using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.DTOs
{
    public record AutoDTO
    {
        public int IdAuto { get; set; }

        [DisplayName("Auto")]
        public string NombreAuto { get; set; } = string.Empty;
   

        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;

        [DisplayName("Año")]
        public int Anio { get; set; }

        [DisplayName("Descripción")]
        public string? Descripcion { get; set; }

        [DisplayName("Fecha de Registro")]
        public DateTime? FechaRegistro { get; set; }

        [DisplayName("Propietario")]
        public string Propietario { get; set; } = string.Empty;
     

        [DisplayName("Condición")]
        public string Condicion { get; set; } = string.Empty;


        [DisplayName("Estado")]
        public string EstadoAuto { get; set; } = string.Empty;
 

        [DisplayName("Imagen Principal")]
        public string ImagenPrincipal { get; set; } = string.Empty;


        [DisplayName("Categorías")]
        public List<CategoriaDTO> IdCategoria { get; set; } = new();


        [DisplayName("Imágenes")]
        public List<AutoImagenDTO> AutoImagen { get; set; } = new();
   

        [DisplayName("Subastas")]
        public List<SubastaResumenDTO> Subasta { get; set; } = new();

    }
}
