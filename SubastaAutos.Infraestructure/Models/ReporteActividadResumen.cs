namespace SubastaAutos.Infraestructure.Repository.Models
{
    public sealed record ReporteActividadResumen(
        int TotalSubastasCreadas,
        int TotalPujas,
        int TotalSubastasFinalizadas,
        IReadOnlyList<ActividadDiariaRow> SerieDiaria);
}