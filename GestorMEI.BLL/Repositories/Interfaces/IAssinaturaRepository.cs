using GestorMEI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.BLL.Repositories.Interfaces
{
    public interface IAssinaturaRepository
    {
        Task CreateAssinatura(AssinaturaDTO assinatura);
        Task UpdateAssinatura(AssinaturaDTO assinatura);
        Task DeleteAssinatura(Guid id);
        Task<AssinaturaDTO> GetAssinaturaByUserId(Guid usuarioId);
    }
}
