using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Correo { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public int IdRol { get; set; }

    public DateTime FechaRegistro { get; set; }

    public bool EstadoUsuario { get; set; }

    public virtual ICollection<Auto> Autos { get; set; } = new List<Auto>();

    public virtual RolUsuario IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Puja> Pujas { get; set; } = new List<Puja>();

    public virtual ICollection<ResultadoSubastum> ResultadoSubasta { get; set; } = new List<ResultadoSubastum>();

    public virtual ICollection<Subastum> Subasta { get; set; } = new List<Subastum>();
}
