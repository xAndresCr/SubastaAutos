using SubastaAutos.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.DTOs
{
    public record UsuarioDTO
    {
        public int IdUsuario { get; set; }

        public string Correo { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        [DisplayName("Nombre del usuario")]
        public string NombreCompleto { get; set; } = null!;

        public int IdRol { get; set; }

        public int IdEstadoUsuario { get; set; }

        public DateTime FechaRegistro { get; set; }
        [DisplayName("Estado del usuario")]
        public bool EstadoUsuario { get; set; }

        //IdRolNavigation permite el mapeo de la entidad a RolUsuario para poder
        //mostrar otros atributos con los tag helpers 
        public RolUsuarioDTO IdRolNavigation { get; set; } = new();

        public List<RolUsuarioDTO> RolUsuario { get; set; } = new();

    }
}
