using SubastaAutos.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.DTOs
{
    public record UsuarioDTO
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Correo { get; set; } = null!;

        // Este campo no se muestra en el formulario, se llena en el controlador
        public string PasswordHash { get; set; } = null!;

        [DisplayName("Nombre del usuario")]
        [Required(ErrorMessage = "El nombre del usuario es obligatorio")]
        public string NombreCompleto { get; set; } = null!;

        [Required(ErrorMessage = "Debe seleccionar un rol")]

        [DisplayName("Perfil del o rol de usuario")]
        public int IdRol { get; set; }

        public int IdEstadoUsuario { get; set; }

        public DateTime FechaRegistro { get; set; }
        [DisplayName("Estado del usuario")]
        public bool EstadoUsuario { get; set; }

        //IdRolNavigation permite el mapeo de la entidad a RolUsuario para poder
        //mostrar otros atributos con los tag helpers 
        public RolUsuarioDTO IdRolNavigation { get; set; } = new();

        public List<RolUsuarioDTO> RolUsuario { get; set; } = new();

        [DisplayName("Subastas creadas")]
        public int CantSubastasCreadas { get; set; }

        [DisplayName("Pujas realizadas")]
        public int CantPujasRealizadas { get; set; }
    }
}
