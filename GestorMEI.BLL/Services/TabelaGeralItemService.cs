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
    public class TabelaGeralItemService:ITabelaGeralItemService
    {
        private readonly ITabelaGeralItemRepository _tabelaGeralItemRepository;
        public TabelaGeralItemService(ITabelaGeralItemRepository tabelaGeralItemRepository)
        {
            _tabelaGeralItemRepository = tabelaGeralItemRepository;
        }

        public async Task AddAsync(TabelaGeralItemDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Sigla)) throw new ArgumentNullException("Sigla do item não pode estar vazia");
            if (dto.Sigla.Length > 5) throw new ArgumentException("Sigla do item não pode ter mais de 5 caracteres");
            if (string.IsNullOrEmpty(dto.Descricao)) throw new ArgumentNullException("Descrição do item não pode estar vazia");
            if (dto.TabelaGeralId == Guid.Empty) throw new ArgumentException("O item precisa estar atrelado a uma tabela geral");
            var item = await _tabelaGeralItemRepository.GetBySiglaAsync(dto.TabelaGeralId, dto.Sigla);
            if (item != null) throw new Exception("Já existe uma tabela geral com essa sigla");

            dto.Id = Guid.NewGuid();
            dto.DataInclusao = DateTime.UtcNow.ToUniversalTime();
            dto.Sigla = dto.Sigla.ToUpper();
            await _tabelaGeralItemRepository.AddAsync(dto);
        }

        public async Task DeleteAsync(Guid id)
        {
            if ((await _tabelaGeralItemRepository.GetByIdAsync(id)) == null)
                throw new KeyNotFoundException();

            await _tabelaGeralItemRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TabelaGeralItemDTO>> GetAllItemsAsync(Guid? tabelaGeralId)
        {
            if (tabelaGeralId == null || tabelaGeralId == Guid.Empty)
                return await _tabelaGeralItemRepository.GetAllAsync();
            else
                return await _tabelaGeralItemRepository.GetAllItemsAsync(tabelaGeralId.Value);
        }

        public async Task<TabelaGeralItemDTO> GetByIdAsync(Guid id)
        {
            return await _tabelaGeralItemRepository.GetByIdAsync(id);
        }

        public async Task<TabelaGeralItemDTO> GetBySiglaAsync(Guid tabelaGeralId, string sigla)
        {
            return await _tabelaGeralItemRepository.GetBySiglaAsync(tabelaGeralId, sigla);
        }

        public async Task UpdateAsync(TabelaGeralItemDTO dto)
        {

            if (dto.Id == Guid.Empty) throw new ArgumentNullException(nameof(dto.Id), "Id não pode ser vazio");
            if (string.IsNullOrEmpty(dto.Sigla)) throw new ArgumentNullException(nameof(dto.Sigla), "Sigla do item não pode estar vazia");
            if (dto.Sigla.Length > 5) throw new ArgumentException(nameof(dto.Sigla), "Sigla do item não pode ter mais de 5 caracteres");
            if (string.IsNullOrEmpty(dto.Descricao)) throw new ArgumentNullException(nameof(dto.Descricao), "Descrição do item não pode estar vazia");
            if (dto.TabelaGeralId == Guid.Empty) throw new ArgumentException(nameof(dto.TabelaGeralId), "O item precisa estar atrelado a uma tabela geral");
            
            var item = await _tabelaGeralItemRepository.GetBySiglaAsync(dto.TabelaGeralId, dto.Sigla);
            if (item != null) throw new Exception("Já existe uma tabela geral com essa sigla");

            dto.DataAlteracao = DateTime.UtcNow.ToUniversalTime();
            dto.Sigla = dto.Sigla.ToUpper();
            await _tabelaGeralItemRepository.UpdateAsync(dto);

        }
    }
}
