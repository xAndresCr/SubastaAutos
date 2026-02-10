using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class EstadoAuto
{
    public int IdEstadoAuto { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Auto> Autos { get; set; } = new List<Auto>();
}
