namespace SubastaAutos.Application.DTOs
{
    public record ReporteCategoriaDTO
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int TotalSubastas { get; set; }
        public int TotalFinalizadas { get; set; }
    }
}