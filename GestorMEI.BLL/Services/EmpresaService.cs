using CpfCnpjLibrary;
using GestorMEI.BLL.Repositories.Interfaces;
using GestorMEI.BLL.Services.Interfaces;
using GestorMEI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.BLL.Services
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IEmpresaRepository _repository;

        public EmpresaService(IEmpresaRepository repository)
        {
            _repository = repository;
        }

        private void ValidarEmpresa(EmpresaDTO dto)
        {
            if (string.IsNullOrEmpty(dto.RazaoSocial))
                throw new ArgumentNullException("Preencha o nome da empresa");
            if (string.IsNullOrEmpty(dto.Email))
                throw new ArgumentNullException("Preencha o email da empresa");
            if (string.IsNullOrEmpty(dto.CNPJ))
                throw new ArgumentNullException("Preencha o CNPJ da empresa");
            if (string.IsNullOrEmpty(dto.Telefone))
                throw new ArgumentNullException("Preencha o telefone da empresa. Se não tiver preencha com 0");
            if (string.IsNullOrEmpty(dto.CEP))
                throw new ArgumentNullException("Preencha o CEP da empresa");
            if (string.IsNullOrEmpty(dto.Endereco))
                throw new ArgumentNullException("Preencha o logradouro da empresa");
            if (dto.Numero <= 0)
                throw new ArgumentOutOfRangeException("O número do endereço da empresa deve ser maior que zero");
            if (string.IsNullOrEmpty(dto.Bairro))
                throw new ArgumentNullException("Preencha o bairro da empresa");
            if (string.IsNullOrEmpty(dto.Cidade))
                throw new ArgumentNullException("Preencha a cidade da empresa");
            if (string.IsNullOrEmpty(dto.Estado))
                throw new ArgumentNullException("Preencha o UF da empresa");
            if (dto.UsuarioId == Guid.Empty)
                throw new ArgumentNullException("Usuário não informado");
            if (Cnpj.Validar(dto.CNPJ) == false)
                throw new ArgumentException("CNPJ inválido");

            if (dto.CEP.Length > 9 || dto.CEP.Length < 8)
                throw new FormatException("CEP em formato inválido");

            if (dto.CEP.IndexOf('-') != 5 && dto.CEP.Length == 9)
                throw new FormatException("CEP em formato inválido");

        }

        public async Task CreateEmpresaAsync(EmpresaDTO empresa)
        {
            ValidarEmpresa(empresa);
            empresa.Id = Guid.NewGuid();
            empresa.DataInclusao = DateTime.UtcNow.ToUniversalTime();
            empresa.CNPJ = Cnpj.FormatarComPontuacao(empresa.CNPJ);
            await _repository.CreateEmpresaAsync(empresa);
        }

        public async Task DeleteEmpresaAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException("Empresa não informada");

            if (await _repository.EmpresaExists(id) == false)
                throw new KeyNotFoundException();

            await _repository.DeleteEmpresaAsync(id);
        }

        public async Task<EmpresaDTO?> GetEmpresaByCNPJAsync(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj))
                throw new ArgumentNullException("CNPJ não informado");
            if (Cnpj.Validar(cnpj) == false)
                throw new ArgumentException("CNPJ inválido");

            return await _repository.GetEmpresaByCNPJAsync(cnpj);
        }

        public async Task<EmpresaDTO?> GetEmpresaByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException("Id da empresa não informado");

            return await _repository.GetEmpresaByIdAsync(id);
        }

        public async Task UpdateEmpresaAsync(EmpresaDTO empresa)
        {
            ValidarEmpresa(empresa);
            if (empresa.Id.HasValue == false || empresa.Id == Guid.Empty)
                throw new ArgumentNullException("Id da empresa não informado");

            if ((await _repository.GetEmpresaByIdAsync(empresa.Id.Value)) == null)
                throw new KeyNotFoundException();

            empresa.DataAlteracao = DateTime.UtcNow.ToUniversalTime();
            empresa.CNPJ = Cnpj.FormatarComPontuacao(empresa.CNPJ);

            await _repository.UpdateEmpresaAsync(empresa);
        }

        public async Task<EmpresaDTO?> GetEmpresaByUserIdAsync(Guid usuarioId)
        {
            return await _repository.GetEmpresaByUsuarioIdAsync(usuarioId);
        }

        public async Task<Guid> GetEmpresaIdByUserIdAsync(Guid userId)
        {
            return await _repository.GetEmpresaIdByUserId(userId);
        }
    }
}
