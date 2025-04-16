using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.Data.Model
{
    public class VendaModel:BaseModel
    {
        [Required]
        [Column("EmpresaId")]
        public Guid EmpresaId { get; set; }
        
        [Required]
        [Column("ComNF")]
        public bool ComNF { get; set; }

        [Required]
        [Column("ValorVenda")]
        [DataType(DataType.Currency)]
        public float ValorVenda { get; set; }

        [Required]
        [Column("IdTGTipoVenda")]
        public Guid IdTGTipoVenda { get; set; } // Comércio, Serviço, Indústria

        [Required]
        [Column("DataVenda")]
        public DateOnly DataVenda { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual EmpresaModel? Empresa { get; set; }

        [ForeignKey("IdTGTipoVenda")]
        public virtual TabelaGeralItemModel? TipoVenda { get; set; }
    }
}
