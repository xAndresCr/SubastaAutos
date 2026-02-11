using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class Puja
{
    public int IdPuja { get; set; }

    public int IdSubasta { get; set; }

    public int IdUsuario { get; set; }

    public decimal Monto { get; set; }

    public DateTime? FechaHora { get; set; }

    public virtual Subasta IdSubastaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
