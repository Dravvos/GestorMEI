using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestorMEI.DTO;

namespace GestorMEI.BLL.Repositories.Interfaces
{
    public interface IEstoqueRepository
    {
        Task<IEnumerable<EstoqueDTO>> GetEstoqueAsync(Guid empresaId);
        Task<IEnumerable<EstoqueDTO>> GetEstoqueByProdutoAsync(Guid produtoId);
        Task AddToEstoqueAsync(EstoqueDTO estoque);
        Task UpdateEstoqueAsync(EstoqueDTO estoque);
        Task DeleteEstoqueAsync(Guid id);
    }
}
