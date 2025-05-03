using GestorMEI.BLL.Repositories.Interfaces;
using GestorMEI.Data;
using GestorMEI.Data.Model;
using GestorMEI.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.BLL.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly MeiContext con;
        public VendaRepository(MeiContext con)
        {
            this.con = con;
        }

        public async Task CreateVendaAsync(VendaDTO venda)
        {
            var model = Map<VendaModel>.Convert(venda);
            await con.AddAsync(model);
            await con.SaveChangesAsync();
        }

        public async Task DeleteVendaAsync(Guid id)
        {
            var venda = await con.Vendas.FirstAsync(x => x.Id == id);
            con.Vendas.Remove(venda);
            await con.SaveChangesAsync();

        }

        public async Task<VendaDTO> GetVendaByIdAsync(Guid id)
        {
            var venda = await con.Vendas.FirstOrDefaultAsync(x => x.Id == id);
            return Map<VendaDTO>.Convert(venda);
        }

        public async Task<IEnumerable<VendaDTO>> GetVendasAsync(Guid empresaId)
        {
            var vendas = await con.Vendas.Where(x => x.EmpresaId == empresaId).Include(x => x.TipoVenda.TabelaGeral).Include(x=>x.Empresa).ToListAsync();
            return Map<List<VendaDTO>>.Convert(vendas);
        }

        public async Task<IEnumerable<VendaDTO>> GetVendasByDateAsync(Guid empresaId, DateOnly? dataInicio, DateOnly? dataFim)
        {
            var vendas = await con.Vendas.Where(x => x.EmpresaId == empresaId).Include(x => x.TipoVenda.TabelaGeral).ToListAsync();
            if (dataInicio.HasValue)
                vendas = vendas.Where(x => x.DataVenda >= dataInicio.Value).ToList();
            if (dataFim.HasValue)
                vendas = vendas.Where(x => x.DataVenda <= dataFim.Value).ToList();

            vendas = vendas.OrderBy(x => x.DataVenda).ToList();

            return Map<IEnumerable<VendaDTO>>.Convert(vendas);
        }

        public async Task UpdateVendaAsync(VendaDTO venda)
        {
            var model = await con.Vendas.FirstAsync(x => x.Id == venda.Id);
            model.DataVenda = venda.DataVenda;
            model.IdTGTipoVenda = venda.IdTGTipoVenda;
            model.ValorVenda = venda.ValorVenda;
            model.ComNF = venda.ComNF;
            model.DataAlteracao = venda.DataAlteracao;
            model.UsuarioAlteracao = venda.UsuarioAlteracao;
            model.EmpresaId = venda.EmpresaId;
            await con.SaveChangesAsync();
        }
    }
}
