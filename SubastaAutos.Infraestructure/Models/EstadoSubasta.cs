using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class EstadoSubasta
{
    public int IdEstadoSubasta { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Subasta> Subasta { get; set; } = new List<Subasta>();
}
