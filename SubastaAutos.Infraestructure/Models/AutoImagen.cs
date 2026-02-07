using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class AutoImagen
{
    public int IdImagen { get; set; }

    public int IdAuto { get; set; }

    public string UrlImagen { get; set; } = null!;

    public bool? EsPrincipal { get; set; }

    public virtual Auto IdAutoNavigation { get; set; } = null!;
}
