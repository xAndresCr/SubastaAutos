using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Auto> IdAuto { get; set; } = new List<Auto>();
}
