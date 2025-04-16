using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.Data.Model
{
    [Table("TabelaGeral")]
    public class TabelaGeralModel : BaseModel
    {
        [Column("Nome")]
        [Required]
        public string Nome { get; set; } = null!;
        [Column("Descricao")]
        [Required]
        public string Descricao { get; set; } = null!;
    }
}
