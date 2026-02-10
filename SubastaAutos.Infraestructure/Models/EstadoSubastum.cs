using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class EstadoSubastum
{
    public int IdEstadoSubasta { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Subastum> Subasta { get; set; } = new List<Subastum>();
}
