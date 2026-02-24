namespace SubastaAutos.Application.DTOs
{
    public record EstadoSubastaDTO
    {
        public int IdEstadoSubasta { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
