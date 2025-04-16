using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.Data.Model
{
    public class BaseModel
    {
        [Key]
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }
        [Column("DataInclusao")]
        [Required]
        public DateTime DataInclusao { get; set; }
        [Column("UsuarioInclusao")]
        [Required]
        public string UsuarioInclusao { get; set; } = null!;
        [Column("DataAlteracao")]
        public DateTime? DataAlteracao { get; set; }
        [Column("UsuarioAlteracao")]
        public string? UsuarioAlteracao { get; set; }
    }
}
