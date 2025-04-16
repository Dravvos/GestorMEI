using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.DTO
{
    public class BaseDTO
    {
        public Guid? Id { get; set; }
        public DateTime DataInclusao { get; set; }
        public string? UsuarioInclusao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string? UsuarioAlteracao { get; set; }
    }
}
