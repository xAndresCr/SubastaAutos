namespace SubastaAutos.Application.DTOs
{
    public record ActividadDiariaDTO
    {
        public DateTime Fecha { get; set; }
        public int Creadas { get; set; }
        public int Pujas { get; set; }
        public int Finalizadas { get; set; }
    }
}