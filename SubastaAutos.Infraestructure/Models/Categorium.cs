using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class Categorium
{
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Auto> IdAutos { get; set; } = new List<Auto>();
}
