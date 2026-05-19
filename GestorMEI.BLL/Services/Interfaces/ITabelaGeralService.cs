using GestorMEI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.BLL.Services.Interfaces
{
    public interface ITabelaGeralService
    {
        Task<TabelaGeralDTO> GetByIdAsync(Guid id);
        Task<TabelaGeralDTO> GetByNomeAsync(string nome);
        Task<TabelaGeralDTO> AddAsync(TabelaGeralDTO dto);
        Task UpdateAsync(TabelaGeralDTO dto);
        Task DeleteAsync(Guid id);
    }
}
