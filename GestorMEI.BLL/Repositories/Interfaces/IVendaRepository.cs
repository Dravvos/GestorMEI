using GestorMEI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.BLL.Repositories.Interfaces
{
    public interface IVendaRepository
    {
        Task<IEnumerable<VendaDTO>> GetVendasByDateAsync(Guid empresaId, DateOnly? dataInicio, DateOnly? dataFim);
        Task<IEnumerable<VendaDTO>> GetVendasAsync(Guid empresaId);
        Task<VendaDTO> GetVendaByIdAsync(Guid id);
        Task CreateVendaAsync(VendaDTO venda);
        Task UpdateVendaAsync(VendaDTO venda);
        Task DeleteVendaAsync(Guid id);
    }
}
