using GestorMEI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.BLL.Repositories.Interfaces
{
    public interface ITabelaGeralRepository
    {
        Task<TabelaGeralDTO> GetByIdAsync(Guid id);
        Task<TabelaGeralDTO> GetByNomeAsync(string nome);
        Task<IList<TabelaGeralDTO>> GetAllAsync();
        Task<TabelaGeralDTO> AddAsync(TabelaGeralDTO dto);
        Task<TabelaGeralDTO> UpdateAsync(TabelaGeralDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
