using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class ResultadoSubastum
{
    public int IdSubasta { get; set; }

    public int IdUsuarioGanador { get; set; }

    public decimal MontoFinal { get; set; }

    public DateTime FechaCierreReal { get; set; }

    public virtual Subastum IdSubastaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioGanadorNavigation { get; set; } = null!;
}
