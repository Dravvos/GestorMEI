using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.DTO
{
    public class ProdutoDTO:BaseDTO
    {
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public float Preco { get; set; }
        public string? SKU { get; set; }
        public Guid EmpresaId { get; set; }
        public uint QuantidadeEstoque { get; set; }
        public EmpresaDTO Empresa { get; set; } = new EmpresaDTO();
    }
}
