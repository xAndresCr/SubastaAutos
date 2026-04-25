namespace SubastaAutos.Infraestructure.Repository.Models
{
    public sealed record ActividadDiariaRow(
        DateTime Fecha,
        int Creadas,
        int Pujas,
        int Finalizadas);
}