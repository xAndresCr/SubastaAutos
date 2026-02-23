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
        // NombreAuto NO existe en la BD. Se calcula: Marca + " " + Modelo + " " + Anio

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
        // Propietario NO existe en la BD. Viene de Usuario.NombreCompleto

        [DisplayName("Condición")]
        public string Condicion { get; set; } = string.Empty;
        // Condicion NO existe en la BD. Viene de CondicionAuto.Nombre

        [DisplayName("Estado")]
        public string EstadoAuto { get; set; } = string.Empty;
        // EstadoAuto (string) NO existe en la BD. Viene de EstadoAuto.Nombre

        [DisplayName("Imagen Principal")]
        public string ImagenPrincipal { get; set; } = string.Empty;
        // Campo CALCULADO: se busca la imagen con EsPrincipal=true en la colección

        [DisplayName("Categorías")]
        public List<CategoriaDTO> IdCategoria { get; set; } = new();
        // Lista de categorías del auto

        [DisplayName("Imágenes")]
        public List<AutoImagenDTO> AutoImagen { get; set; } = new();
        // TODAS las imágenes (para el detalle)

        [DisplayName("Subastas")]
        public List<SubastaResumenDTO> Subasta { get; set; } = new();
        // Historial de subastas donde participó este auto
    }
}
