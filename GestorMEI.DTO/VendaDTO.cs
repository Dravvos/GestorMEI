using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.DTO
{
    public class VendaDTO : BaseDTO
    {
        public Guid EmpresaId { get; set; }
        public bool ComNF { get; set; }
        public float ValorVenda { get; set; }
        public Guid IdTGTipoVenda { get; set; } // Comércio, Serviço, Indústria
        public TabelaGeralItemDTO? TipoVenda { get; set; }
        public DateOnly DataVenda { get; set; }


    }
}
