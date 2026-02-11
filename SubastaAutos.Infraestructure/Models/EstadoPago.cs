using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class EstadoPago
{
    public int IdEstadoPago { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pago> Pago { get; set; } = new List<Pago>();
}
