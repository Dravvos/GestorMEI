using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.DTO.Auth
{
    public class SignUpDTO
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Sobrenome { get; set; }
    }
}
