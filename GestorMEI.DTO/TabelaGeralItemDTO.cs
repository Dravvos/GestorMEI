using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.DTO
{
    public class TabelaGeralItemDTO:BaseDTO
    {
        public Guid TabelaGeralId { get; set; }
        public string Sigla { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public TabelaGeralDTO? TabelaGeral { get; set; }
    }
}
