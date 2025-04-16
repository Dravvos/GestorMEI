using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.Data.Model
{
    [Table("Empresa")]
    public class EmpresaModel : BaseModel
    {
        [Column("RazaoSocial")]
        [Required]
        public string RazaoSocial { get; set; } = null!;
        
        [Column("CNPJ")]
        [Required]
        [StringLength(20)]
        public string CNPJ { get; set; } = null!;
        
        [Column("Email")]
        [Required]
        public string Email { get; set; } = null!;
        
        [Column("Telefone")]
        [Required]
        [StringLength(20)]
        public string Telefone { get; set; } = null!;
        
        [Column("NomeFantasia")]
        public string? NomeFantasia { get; set; }
        
        [Column("CEP")]
        [Required]
        [StringLength(10, MinimumLength = 8)]
        public string CEP { get; set; } = null!;

        [Column("Endereco")]
        [Required]
        public string Endereco { get; set; } = null!;

        [Column("Numero")]
        [Required]
        [Range(1, 99999)]
        public uint Numero { get; set; }

        [Column("Complemento")]
        public string? Complemento { get; set; }
        
        [Column("Bairro")]
        [Required]
        public string Bairro { get; set; } = null!;

        [Column("Cidade")]
        [Required]
        public string Cidade { get; set; } = null!;

        [Column("Estado")]
        [Required]
        [StringLength(2)]
        public string Estado { get; set; } = null!;

        [Column("UsuarioId")]
        [Required]
        public Guid UsuarioId { get; set; }
    }
}
