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
    public class TabelaGeralRepository : ITabelaGeralRepository
    {
        private readonly MeiContext con;
        public TabelaGeralRepository(MeiContext con)
        {
            this.con = con;
        }

        public async Task<TabelaGeralDTO> AddAsync(TabelaGeralDTO dto)
        {
            var tabelaGeral = Map<TabelaGeralModel>.Convert(dto);
            await con.TabelaGeral.AddAsync(tabelaGeral);
            await con.SaveChangesAsync();
            return Map<TabelaGeralDTO>.Convert(tabelaGeral);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var tabelaGeral = await con.TabelaGeral.FirstOrDefaultAsync(x => x.Id == id);
                if (tabelaGeral == null)
                    return false;
                con.TabelaGeral.Remove(tabelaGeral);
                await con.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IList<TabelaGeralDTO>> GetAllAsync()
        {
            var tabelasGerais = await con.TabelaGeral.AsNoTracking().Select(x => new TabelaGeralDTO
            {
                Id = x.Id,
                Nome = x.Nome,
                Descricao = x.Descricao
            }).ToListAsync();
            return tabelasGerais;
        }

        public async Task<TabelaGeralDTO?> GetByIdAsync(Guid id)
        {
            var tabelaGeral = await con.TabelaGeral.AsNoTracking().Where(x => x.Id == id)
                .Select(x => new TabelaGeralDTO
                {
                    Id = x.Id,
                    Descricao = x.Descricao,
                    Nome = x.Nome
                }).FirstOrDefaultAsync();
            return tabelaGeral;
        }

        public async Task<TabelaGeralDTO?> GetByNomeAsync(string nome)
        {
            var tabelaGeral = await con.TabelaGeral.AsNoTracking().Where(x => x.Nome == nome)
               .Select(x => new TabelaGeralDTO
               {
                   Id = x.Id,
                   Descricao = x.Descricao,
                   Nome = x.Nome
               }).FirstOrDefaultAsync();
            return tabelaGeral;
        }

        public async Task<TabelaGeralDTO> UpdateAsync(TabelaGeralDTO dto)
        {
            var tabelaGeral = await con.TabelaGeral.FirstAsync(x => x.Id == dto.Id);
            tabelaGeral.Descricao = dto.Descricao;
            tabelaGeral.Nome = dto.Nome;
            tabelaGeral.DataAlteracao = DateTime.UtcNow.ToUniversalTime();

            await con.SaveChangesAsync();
            return Map<TabelaGeralDTO>.Convert(tabelaGeral);
        }
    }
}
