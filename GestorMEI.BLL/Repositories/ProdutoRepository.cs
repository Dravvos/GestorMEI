using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestorMEI.BLL.Repositories.Interfaces;
using GestorMEI.Data;
using GestorMEI.Data.Model;
using GestorMEI.DTO;

namespace GestorMEI.BLL.Repositories
{
    public class ProdutoRepository:IProdutoRepository
    {
        private readonly MeiContext con;

        public ProdutoRepository(MeiContext con)
        {
            this.con = con;
        }

        public async Task CreateProdutoAsync(ProdutoDTO produto)
        {
            var model = Map<ProdutoModel>.Convert(produto);
            await con.Produtos.AddAsync(model);
            await con.SaveChangesAsync();
        }

        public async Task DeleteProdutoAsync(Guid id)
        {
            var produto = await con.Produtos.FirstAsync(x => x.Id == id);
            con.Produtos.Remove(produto);
            await con.SaveChangesAsync();
        }

        public async Task<ProdutoDTO> GetProdutoByIdAsync(Guid id)
        {
            var produto = await con.Produtos.FirstOrDefaultAsync(x => x.Id == id);
            return Map<ProdutoDTO>.Convert(produto);
        }

        public async Task<IEnumerable<ProdutoDTO>> GetProdutosAsync(Guid empresaId)
        {
            var produtos = await con.Produtos.Where(x => x.EmpresaId == empresaId).ToListAsync();
            return Map<IEnumerable<ProdutoDTO>>.Convert(produtos);
        }

        public async Task<IEnumerable<ProdutoDTO>> GetProdutosByNomeAsync(string nome)
        {
            var produtos = await con.Produtos.Where(x => x.Nome.Contains(nome)).ToListAsync();
            return Map<IEnumerable<ProdutoDTO>>.Convert(produtos);
        }

        public async Task UpdateProdutoAsync(ProdutoDTO produto)
        {
            var model = await con.Produtos.FirstAsync(x => x.Id == produto.Id);
            model.Nome = produto.Nome;
            model.Descricao = produto.Descricao;
            model.Preco = produto.Preco;
            model.QuantidadeEstoque = produto.QuantidadeEstoque;
            model.UsuarioAlteracao = produto.UsuarioAlteracao;
            model.DataAlteracao = produto.DataAlteracao;
            await con.SaveChangesAsync();
        }
    }
}
