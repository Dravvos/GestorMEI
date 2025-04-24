using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.Data.Model
{
    [Table("Estoque")]
    public class EstoqueModel:BaseModel
    {
        [Required]
        [Column("EmpresaId")]
        public Guid EmpresaId { get; set; }

        [Required]
        [Column("ProdutoId")]
        public Guid ProdutoId { get; set; }

        [Required]
        [Column("IdTGTipoLancamento")]
        public Guid IdTGTipoLancamento { get; set; }

        [Required]
        [Column("Quantidade")]
        [Range(0, uint.MaxValue)]
        public uint Quantidade { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual EmpresaModel Empresa { get; set; } = null!;

        [ForeignKey("ProdutoId")]
        public virtual ProdutoModel Produto { get; set; } = null!;
    }
}
