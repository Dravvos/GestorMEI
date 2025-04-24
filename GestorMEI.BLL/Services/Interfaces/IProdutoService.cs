using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestorMEI.DTO;

namespace GestorMEI.BLL.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoDTO>> GetProdutosAsync(Guid empresaId);
        Task<ProdutoDTO> GetProdutoByIdAsync(Guid id);
        Task<IEnumerable<ProdutoDTO>> GetProdutosByNomeAsync(string nome);
        Task CreateProdutoAsync(ProdutoDTO produto);
        Task UpdateProdutoAsync(ProdutoDTO produto);
        Task DeleteProdutoAsync(Guid id);
    }
}
