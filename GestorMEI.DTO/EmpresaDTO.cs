using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.DTO
{
    public class EmpresaDTO: BaseDTO
    {
        public string CNPJ { get; set; } = null!;
        public string RazaoSocial { get; set; } = null!;
        public string? NomeFantasia { get; set; }
        public string Endereco { get; set; } = null!;
        public uint Numero { get; set; }
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = null!;
        public string Cidade { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string CEP { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Guid UsuarioId { get; set; }
    }
}
