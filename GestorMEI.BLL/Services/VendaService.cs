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
    public class VendaService : IVendaService
    {
        private readonly IVendaRepository _repository;
        public VendaService(IVendaRepository repository)
        {
            _repository = repository;
        }

        private void ValidarVenda(VendaDTO venda)
        {
            if (venda.IdTGTipoVenda == Guid.Empty)
                throw new Exception("Selecione o tipo de venda");
            if (venda.DataVenda == DateOnly.MinValue || venda.DataVenda == DateOnly.MaxValue)
                throw new Exception("Data da venda não informada");
            if (venda.EmpresaId == Guid.Empty)
                throw new Exception("Empresa que vendeu não informada");
            if (venda.ValorVenda <= 0)
                throw new Exception("Valor da venda não informado");
        }

        public async Task CreateVendaAsync(VendaDTO venda)
        {
            ValidarVenda(venda);
            venda.Id = Guid.NewGuid();
            venda.DataInclusao = DateTime.UtcNow.ToUniversalTime();
            await _repository.CreateVendaAsync(venda);
        }

        public async Task DeleteVendaAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new Exception("Id da venda não informado");

            var venda = await _repository.GetVendaByIdAsync(id);
            if (venda == null)
                throw new KeyNotFoundException();

            await _repository.DeleteVendaAsync(id);
        }

        public async Task<VendaDTO?> GetVendaByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new Exception("Id da venda não informado");
            return await _repository.GetVendaByIdAsync(id);
        }

        public async Task<IEnumerable<VendaDTO>> GetVendasAsync(Guid empresaId)
        {
            if (empresaId == Guid.Empty)
                throw new Exception("Id da empresa não informado");

            return await _repository.GetVendasAsync(empresaId);
        }

        public async Task UpdateVendaAsync(VendaDTO venda)
        {
            ValidarVenda(venda);
            if (venda.Id.HasValue == false || venda.Id == Guid.Empty)
                throw new Exception("Id da venda não informado");

            if ((await _repository.VendaExist(venda.Id.Value)) == false)
                throw new KeyNotFoundException();

            venda.DataAlteracao = DateTime.UtcNow.ToUniversalTime();
            await _repository.UpdateVendaAsync(venda);
        }

        public async Task<IEnumerable<VendaDTO>> GetVendasByDateAsync(Guid empresaId, DateOnly? dataInicio, DateOnly? dataFim)
        {
            return await _repository.GetVendasByDateAsync(empresaId, dataInicio, dataFim);
        }
    }
}
