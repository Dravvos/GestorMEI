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
    public class EstoqueRepository:IEstoqueRepository
    {
        private readonly MeiContext con;

        public EstoqueRepository(MeiContext con)
        {
            this.con = con;
        }

        public async Task AddToEstoqueAsync(EstoqueDTO estoque)
        {
            var model = Map<EstoqueModel>.Convert(estoque);
            await con.Estoque.AddAsync(model);
            await con.SaveChangesAsync();
        }

        public async Task DeleteEstoqueAsync(Guid id)
        {
            var estoque = await con.Estoque.FirstAsync(x => x.Id == id);
            con.Estoque.Remove(estoque);
            await con.SaveChangesAsync();
        }

        public async Task<IEnumerable<EstoqueDTO>> GetEstoqueAsync(Guid empresaId)
        {
            var estoque = await con.Estoque.Where(x => x.EmpresaId == empresaId).ToListAsync();
            return Map<IEnumerable<EstoqueDTO>>.Convert(estoque);
        }

        public async Task<IEnumerable<EstoqueDTO>> GetEstoqueByProdutoAsync(Guid produtoId)
        {
            var estoques = await con.Estoque.Where(x => x.ProdutoId == produtoId).ToListAsync();
            return Map<IEnumerable<EstoqueDTO>>.Convert(estoques);
        }

        public async Task UpdateEstoqueAsync(EstoqueDTO estoque)
        {
            var model = await con.Estoque.FirstAsync(x => x.Id == estoque.Id);
            model.ProdutoId = estoque.ProdutoId;
            model.Quantidade = estoque.Quantidade;
            model.EmpresaId = estoque.EmpresaId;
            model.IdTGTipoLancamento = estoque.IdTGTipoLancamento;
            model.DataAlteracao = estoque.DataAlteracao;
            model.UsuarioAlteracao = estoque.UsuarioAlteracao;
            await con.SaveChangesAsync();
        }
    }
}
