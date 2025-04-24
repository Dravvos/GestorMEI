using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.DTO
{
    public class EstoqueDTO:BaseDTO
    {
        public Guid EmpresaId { get; set; }
        public Guid ProdutoId { get; set; }
        public uint Quantidade { get; set; }
        public Guid IdTGTipoLancamento { get; set; }
        public EmpresaDTO Empresa { get; set; } = new EmpresaDTO();
        public ProdutoDTO Produto { get; set; } = new ProdutoDTO();
    }
}
