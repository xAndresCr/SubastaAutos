using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaAutos.Application.DTOs
{
    public record CondicionAutoDTO
    {
        public int IdCondicionAuto { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
