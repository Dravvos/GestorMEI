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
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly MeiContext con;
        public EmpresaRepository(MeiContext con)
        {
            this.con = con;
        }
        public async Task CreateEmpresaAsync(EmpresaDTO empresa)
        {
            var model = Map<EmpresaModel>.Convert(empresa);
            await con.Empresa.AddAsync(model);
            await con.SaveChangesAsync();
        }

        public async Task DeleteEmpresaAsync(Guid id)
        {
            var empresa = await con.Empresa.FirstAsync(x => x.Id == id);
            con.Empresa.Remove(empresa);
            await con.SaveChangesAsync();
        }

        public async Task<EmpresaDTO> GetEmpresaByCNPJAsync(string cnpj)
        {
            var empresa = await con.Empresa.FirstOrDefaultAsync(x => x.CNPJ == cnpj);
            return Map<EmpresaDTO>.Convert(empresa);
        }

        public async Task<EmpresaDTO> GetEmpresaByIdAsync(Guid id)
        {
            var empresa = await con.Empresa.FirstOrDefaultAsync(x => x.Id == id);
            return Map<EmpresaDTO>.Convert(empresa);
        }

        public async Task<EmpresaDTO> GetEmpresaByUsuarioIdAsync(Guid usuarioId)
        {
            var empresa = await con.Empresa.FirstOrDefaultAsync(x => x.UsuarioId == usuarioId);
            return Map<EmpresaDTO>.Convert(empresa);
        }

        public async Task<IEnumerable<EmpresaDTO>> GetEmpresasAsync()
        {
            var empresas = await con.Empresa.ToListAsync();
            return Map<List<EmpresaDTO>>.Convert(empresas);
        }

        public async Task UpdateEmpresaAsync(EmpresaDTO empresa)
        {
            var model = con.Empresa.First(x => x.Id == empresa.Id);
            model.RazaoSocial= empresa.RazaoSocial;
            model.CNPJ = empresa.CNPJ;
            model.Telefone = empresa.Telefone;
            model.Email = empresa.Email;
            model.Endereco = empresa.Endereco;
            model.Numero = empresa.Numero;
            model.Complemento = empresa.Complemento;
            model.Bairro = empresa.Bairro;
            model.Cidade = empresa.Cidade;
            model.Estado = empresa.Estado;
            model.CEP = empresa.CEP;
            model.NomeFantasia = empresa.NomeFantasia;
            model.UsuarioId = empresa.UsuarioId;
            model.DataAlteracao = empresa.DataAlteracao;
            model.UsuarioAlteracao = empresa.UsuarioAlteracao;
            
            await con.SaveChangesAsync();
        }
    }
}
