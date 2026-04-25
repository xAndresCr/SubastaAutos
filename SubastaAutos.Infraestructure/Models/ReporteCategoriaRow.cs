namespace SubastaAutos.Infraestructure.Repository.Models
{
    public sealed record ReporteCategoriaRow(
        int IdCategoria,
        string Nombre,
        int TotalSubastas,
        int TotalFinalizadas);
}