using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.DTOs
{
    public record RolUsuarioDTO
    {
        public int IdRol { get; set; }

        [DisplayName("Rol del usuario")]
        public string Nombre { get; set; } = null!;


    }
}
