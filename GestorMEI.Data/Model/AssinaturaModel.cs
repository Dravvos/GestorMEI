using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.Data.Model
{
    [Table("Assinatura")]
    public class AssinaturaModel
    {
        [Key]
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("UsuarioId")]
        [Required]
        public Guid UsuarioId { get; set; }

        [Column("IdTGTipoAssinatura")]
        [Required]
        public Guid IdTGTipoAssinatura { get; set; }

        [Column("DataInicio")]
        [Required]
        public DateTime DataInicio { get; set; }

        [Column("DataFim")]
        [Required]
        public DateTime DataFim { get; set; }

        [Column("IdTGStatusAssinatura")]
        [Required]
        public Guid IdTGStatusAssinatura { get; set; }

        [Column("DataInclusao")]
        [Required]
        public DateTime DataInclusao { get; set; }

        [Column("DataAlteracao")]
        public DateTime? DataAlteracao { get; set; }

        [ForeignKey("IdTGStatusAssinatura")]
        public virtual TabelaGeralItemModel? StatusAssinatura { get; set; }

        [ForeignKey("IdTGTipoAssinatura")]
        public virtual TabelaGeralItemModel? TipoAssinatura { get; set; }
    }
}
