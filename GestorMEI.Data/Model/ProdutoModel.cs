using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.Data.Model
{
    [Table("Produtos")]
    public class ProdutoModel:BaseModel
    {
        [Required]
        [Column("Nome")]
        [MaxLength(200)]
        public string? Nome { get; set; }

        [Required]
        [Column("Descricao")]
        public string? Descricao { get; set; }

        [Required]
        [Column("Preco")]
        [DataType(DataType.Currency)]
        public float Preco { get; set; }

        [Required]
        [Column("EmpresaId")]
        public Guid EmpresaId { get; set; }

        [Required]
        [Column("QuantidadeEstoque")]
        [Range(0, uint.MaxValue)]
        public uint QuantidadeEstoque { get; set; }

        [Required]
        [Column("SKU")]
        public string? SKU { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual EmpresaModel Empresa { get; set; } = new EmpresaModel();
    }
}
