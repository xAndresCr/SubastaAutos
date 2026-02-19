using SubastaAutos.Infraestructure.Models;
using System;
using System.Collections.Generic;
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

        public string NombreCompleto { get; set; } = null!;

        public int IdRol { get; set; }

        public int IdEstadoUsuario { get; set; }

        public DateTime FechaRegistro { get; set; }

        public bool EstadoUsuario { get; set; }

        public List<RolUsuarioDTO> rol { get; set; } = new();

    }
}
