using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.DTO
{
    public class RelatorioDTO
    {
        public float TotalVendas { get; set; }
        public float TotalVendasComNF{ get; set; }
        public float TotalVendasSemNF { get; set; } 
        public Guid IdTGTipoVenda { get; set; } 
        public string? TipoVendaDescricao { get; set; }
        public string? TipoVendaSigla { get; set; }
    }
}
