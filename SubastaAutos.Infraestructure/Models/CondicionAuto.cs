using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class CondicionAuto
{
    public int IdCondicionAuto { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Auto> Autos { get; set; } = new List<Auto>();
}
