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
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _repository;

        public ProdutoService(IProdutoRepository repository)
        {
            _repository = repository;
        }

        private void ValidarProduto(ProdutoDTO dto)
        {
            if (dto.Preco <= 0)
                throw new ArgumentOutOfRangeException("Preço não pode ser menor ou igual a zero");
            if (string.IsNullOrEmpty(dto.Nome))
                throw new ArgumentNullException("Preencha o nome do produto");
            if (string.IsNullOrEmpty(dto.Descricao))
                throw new ArgumentNullException("Faça uma breve descrição do produto");
            if (dto.QuantidadeEstoque < 0)
                throw new ArgumentOutOfRangeException("Quantidade em estoque não pode ser menor que zero");
            if (dto.EmpresaId == Guid.Empty)
                throw new ArgumentNullException("Selecione a empresa");
        }

        public async Task CreateProdutoAsync(ProdutoDTO produto)
        {
            ValidarProduto(produto);
            produto.Id = Guid.NewGuid();
            produto.DataInclusao = DateTime.Now;

            await _repository.CreateProdutoAsync(produto);
        }

        public async Task DeleteProdutoAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException("Id do produto não foi passado");

            var produto = await _repository.GetProdutoByIdAsync(id);
            if (produto == null)
                throw new KeyNotFoundException();

            await _repository.DeleteProdutoAsync(id);
        }

        public async Task<ProdutoDTO> GetProdutoByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException("Selecione o produto");

            return await _repository.GetProdutoByIdAsync(id);
        }

        public async Task<IEnumerable<ProdutoDTO>> GetProdutosAsync(Guid empresaId)
        {
            if (empresaId == Guid.Empty)
                throw new ArgumentNullException("Selecione a empresa");

            return await _repository.GetProdutosAsync(empresaId);
        }

        public async Task<IEnumerable<ProdutoDTO>> GetProdutosByNomeAsync(string nome)
        {
            if (string.IsNullOrEmpty(nome))
                throw new ArgumentNullException("Digite pelo menos uma parte do nome");

            return await _repository.GetProdutosByNomeAsync(nome);
        }

        public async Task UpdateProdutoAsync(ProdutoDTO produto)
        {
            ValidarProduto(produto);
            if (produto.Id == Guid.Empty)
                throw new ArgumentNullException("Id do produto está vazio");

            produto.DataAlteracao = DateTime.Now;
            await _repository.UpdateProdutoAsync(produto);
        }
    }
}
