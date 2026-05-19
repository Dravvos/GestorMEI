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
    public class AssinaturaRepository : IAssinaturaRepository
    {
        private readonly MeiContext con;

        public AssinaturaRepository(MeiContext context)
        {
            con = context;
        }

        public async Task<bool> AssinaturaExists(Guid id)
        {
            return await con.Assinatura.AsNoTracking().AnyAsync(x => x.Id == id);
        }

        public async Task CreateAssinatura(AssinaturaDTO assinatura)
        {
            var model = Map<AssinaturaModel>.Convert(assinatura);
            await con.Assinatura.AddAsync(model);
            await con.SaveChangesAsync();
        }

        public async Task DeleteAssinatura(Guid id)
        {
            var assinatura = await con.Assinatura.FirstAsync(x => x.Id == id);
            con.Assinatura.Remove(assinatura);
            await con.SaveChangesAsync();
        }

        public async Task<AssinaturaDTO> GetAssinaturaByUserId(Guid usuarioId)
        {
            var assinatura = await con.Assinatura.AsNoTracking().Where(x => x.UsuarioId == usuarioId)
               .Select(x => new AssinaturaDTO
               {
                   DataInicio = x.DataInicio,
                   DataFim = x.DataFim,
                   StatusAssinatura = new TabelaGeralItemDTO
                   {
                       Id = x.StatusAssinatura.Id,
                       Descricao = x.StatusAssinatura.Descricao,
                       Sigla = x.StatusAssinatura.Sigla,
                   },
                   TipoAssinatura = new TabelaGeralItemDTO
                   {
                       Id = x.TipoAssinatura.Id,
                       Descricao = x.TipoAssinatura.Descricao,
                       Sigla = x.TipoAssinatura.Sigla,
                   }
               }).FirstOrDefaultAsync();
            return assinatura;
        }

        public async Task UpdateAssinatura(AssinaturaDTO assinatura)
        {
            var model = await con.Assinatura.FirstAsync(x => x.Id == assinatura.Id);
            model.UsuarioId = assinatura.UsuarioId;
            model.DataInicio = assinatura.DataInicio;
            model.DataFim = assinatura.DataFim;
            model.IdTGStatusAssinatura = assinatura.IdTGStatusAssinatura;
            model.IdTGTipoAssinatura = assinatura.IdTGTipoAssinatura;
            model.DataAlteracao = assinatura.DataAlteracao;

            await con.SaveChangesAsync();
        }
    }
}
