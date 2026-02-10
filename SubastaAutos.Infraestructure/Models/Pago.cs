using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int IdSubasta { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public decimal Monto { get; set; }

    public int IdEstadoPago { get; set; }

    public virtual EstadoPago IdEstadoPagoNavigation { get; set; } = null!;

    public virtual Subastum IdSubastaNavigation { get; set; } = null!;
}
