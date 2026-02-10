using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class Subastum
{
    public int IdSubasta { get; set; }

    public int IdAuto { get; set; }

    public int IdVendedor { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaCierre { get; set; }

    public decimal PrecioBase { get; set; }

    public decimal IncrementoMinimo { get; set; }

    public int IdEstadoSubasta { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual Auto IdAutoNavigation { get; set; } = null!;

    public virtual EstadoSubastum IdEstadoSubastaNavigation { get; set; } = null!;

    public virtual Usuario IdVendedorNavigation { get; set; } = null!;

    public virtual Pago? Pago { get; set; }

    public virtual ICollection<Puja> Pujas { get; set; } = new List<Puja>();

    public virtual ResultadoSubastum? ResultadoSubastum { get; set; }
}
