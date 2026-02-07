using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class ResultadoSubasta
{
    public int IdSubasta { get; set; }

    public int IdUsuarioGanador { get; set; }

    public decimal MontoFinal { get; set; }

    public DateTime FechaCierreReal { get; set; }

    public virtual Subasta IdSubastaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioGanadorNavigation { get; set; } = null!;
}
