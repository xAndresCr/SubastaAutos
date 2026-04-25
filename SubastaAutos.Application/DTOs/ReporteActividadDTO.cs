namespace SubastaAutos.Application.DTOs
{
    public record ReporteActividadDTO
    {
        public int TotalSubastasCreadas { get; set; }
        public int TotalPujas { get; set; }
        public int TotalSubastasFinalizadas { get; set; }
        public List<ActividadDiariaDTO> SerieDiaria { get; set; } = new();
    }
}