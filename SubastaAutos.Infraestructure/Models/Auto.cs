using System;
using System.Collections.Generic;

namespace SubastaAutos.Infraestructure.Models;

public partial class Auto
{
    public int IdAuto { get; set; }

    public int IdVendedor { get; set; }

    public string Vin { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public int Anio { get; set; }

    public string? Descripcion { get; set; }

    public int IdCondicionAuto { get; set; }

    public int IdEstadoAuto { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<AutoImagen> AutoImagen { get; set; } = new List<AutoImagen>();

    public virtual CondicionAuto IdCondicionAutoNavigation { get; set; } = null!;

    public virtual EstadoAuto IdEstadoAutoNavigation { get; set; } = null!;

    public virtual Usuario IdVendedorNavigation { get; set; } = null!;

    public virtual ICollection<Subasta> Subasta { get; set; } = new List<Subasta>();

    public virtual ICollection<Categoria> IdCategoria { get; set; } = new List<Categoria>();
}
