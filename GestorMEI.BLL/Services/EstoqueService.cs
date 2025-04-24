using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestorMEI.BLL.Repositories.Interfaces;
using GestorMEI.BLL.Services.Interfaces;
using GestorMEI.DTO;

namespace GestorMEI.BLL.Services
{
    public class EstoqueService : IEstoqueService
    {
        private readonly IEstoqueRepository _repository;

        public EstoqueService(IEstoqueRepository repository)
        {
            _repository = repository;
        }

        private void ValidarEstoque(EstoqueDTO dto)
        {
            if (dto.ProdutoId == Guid.Empty)
                throw new ArgumentNullException("Selecione o produto");
            if (dto.EmpresaId == Guid.Empty)
                throw new ArgumentNullException("Selecione a empresa");
            if (dto.Quantidade < 0)
                throw new ArgumentOutOfRangeException("O mínimo que pode ter em estoque é 0");
            if (dto.IdTGTipoLancamento == Guid.Empty)
                throw new ArgumentNullException("Selecione o tipo de lançamento");
        }
        public async Task AddToEstoqueAsync(EstoqueDTO estoque)
        {
            ValidarEstoque(estoque);
            estoque.Id = Guid.NewGuid();
            estoque.DataInclusao = DateTime.Now;

            await _repository.AddToEstoqueAsync(estoque);
        }

        public async Task DeleteEstoqueAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException("Id do estoque não preenchido");

            await _repository.DeleteEstoqueAsync(id);
        }

        public async Task<IEnumerable<EstoqueDTO>> GetEstoqueAsync(Guid empresaId)
        {
            if (empresaId == Guid.Empty)
                throw new ArgumentNullException("Id da empresa está vazio");

            return await _repository.GetEstoqueAsync(empresaId);
        }

        public async Task<IEnumerable<EstoqueDTO>> GetEstoqueByProdutoAsync(Guid produtoId)
        {
            if (produtoId == Guid.Empty)
                throw new ArgumentNullException("Id do produto está vazio");

            return await _repository.GetEstoqueByProdutoAsync(produtoId);
        }

        public async Task UpdateEstoqueAsync(EstoqueDTO estoque)
        {
            ValidarEstoque(estoque);
            if (estoque.Id == Guid.Empty)
                throw new ArgumentNullException("Id do estoque está vazio");

            estoque.DataAlteracao = DateTime.Now;

            await _repository.UpdateEstoqueAsync(estoque);
        }
    }
}
