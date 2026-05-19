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
    public class TabelaGeralItemRepository : ITabelaGeralItemRepository
    {
        private readonly MeiContext con;

        public TabelaGeralItemRepository(MeiContext con)
        {
            this.con = con;
        }

        public async Task<TabelaGeralItemDTO> AddAsync(TabelaGeralItemDTO item)
        {
            var tabelaGeralItem = Map<TabelaGeralItemModel>.Convert(item);
            await con.TabelaGeralItem.AddAsync(tabelaGeralItem);
            await con.SaveChangesAsync();
            return Map<TabelaGeralItemDTO>.Convert(tabelaGeralItem);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var tabelaGeralItem = await con.TabelaGeralItem.FirstOrDefaultAsync(x => x.Id == id);
                if (tabelaGeralItem == null)
                    return false;
                con.TabelaGeralItem.Remove(tabelaGeralItem);
                await con.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IList<TabelaGeralItemDTO>> GetAllAsync()
        {
            var ItensTabelasGerais = await con.TabelaGeralItem.AsNoTracking().Select(x => new TabelaGeralItemDTO
            {
                Id = x.Id,
                Descricao = x.Descricao,
                Sigla = x.Sigla,
                TabelaGeral = new TabelaGeralDTO
                {
                    Id = x.TabelaGeral.Id,
                    Nome = x.TabelaGeral.Nome
                }
            }).ToListAsync();
            return ItensTabelasGerais;
        }

        public async Task<IList<TabelaGeralItemDTO>> GetAllItemsAsync(Guid? tabelaGeralId)
        {
            var ItensTabelasGerais = new List<TabelaGeralItemModel>();
            if (tabelaGeralId != null)
            {
                ItensTabelasGerais = await con.TabelaGeralItem.AsNoTracking().Where(x => x.TabelaGeralId == tabelaGeralId.Value)
                    .Select(x => new TabelaGeralItemModel
                    {
                        Id = x.Id,
                        Descricao = x.Descricao,
                        Sigla = x.Sigla,
                        TabelaGeral = new TabelaGeralModel { Id = x.TabelaGeral.Id, Nome = x.TabelaGeral.Nome }
                    }).ToListAsync();
            }
            else
            {
                ItensTabelasGerais = await con.TabelaGeralItem.AsNoTracking().Select(x => new TabelaGeralItemModel
                {
                    Id = x.Id,
                    Descricao = x.Descricao,
                    Sigla = x.Sigla,
                    TabelaGeral = new TabelaGeralModel { Id = x.TabelaGeral.Id, Nome = x.TabelaGeral.Nome }
                }).ToListAsync();
            }

            return Map<List<TabelaGeralItemDTO>>.Convert(ItensTabelasGerais);
        }

        public async Task<TabelaGeralItemDTO?> GetByIdAsync(Guid id)
        {
            var tabelaGeralItem = await con.TabelaGeralItem.AsNoTracking().Include(x => x.TabelaGeral).Where(x => x.Id == id)
                 .Select(x => new TabelaGeralItemDTO
                 {
                     Id = x.Id,
                     Descricao = x.Descricao,
                     Sigla = x.Sigla,
                     TabelaGeral = new TabelaGeralDTO
                     {
                         Id = x.TabelaGeral.Id,
                         Nome = x.TabelaGeral.Nome
                     }
                 }).FirstOrDefaultAsync();
            return tabelaGeralItem;
        }

        public async Task<TabelaGeralItemDTO?> GetBySiglaAsync(Guid tabelaGeralId, string sigla)
        {
            var tabelaGeralItem = await con.TabelaGeralItem.AsNoTracking().Where(x => x.TabelaGeralId == tabelaGeralId && x.Sigla == sigla)
                .Select(x => new TabelaGeralItemDTO
                {
                    Id = x.Id,
                    Descricao = x.Descricao,
                    Sigla = x.Sigla,
                    TabelaGeral = new TabelaGeralDTO
                    {
                        Id = x.TabelaGeral.Id,
                        Nome = x.TabelaGeral.Nome
                    }
                }).FirstOrDefaultAsync();
            return tabelaGeralItem;
        }

        public async Task<bool> ItemExists(Guid id)
        {
            return await con.TabelaGeralItem.AsNoTracking().AnyAsync(x => x.Id == id);
        }

        public async Task<bool> ItemExists(Guid tabelaGeralId, string sigla)
        {
            return await con.TabelaGeralItem.AsNoTracking().AnyAsync(x => x.TabelaGeralId == tabelaGeralId && x.Sigla == sigla);
        }

        public async Task<TabelaGeralItemDTO> UpdateAsync(TabelaGeralItemDTO item)
        {
            var model = await con.TabelaGeralItem.FirstAsync(x => x.Id == item.Id);

            model.Descricao = item.Descricao;
            model.DataAlteracao = DateTime.UtcNow.ToUniversalTime();
            model.Sigla = item.Sigla.ToUpper();

            await con.SaveChangesAsync();
            return Map<TabelaGeralItemDTO>.Convert(model);
        }
    }
}
