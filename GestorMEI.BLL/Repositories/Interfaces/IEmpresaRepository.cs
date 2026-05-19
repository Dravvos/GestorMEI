using GestorMEI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.BLL.Repositories.Interfaces
{
    public interface IEmpresaRepository
    {
        Task<bool> EmpresaExists(Guid id);
        Task<Guid> GetEmpresaIdByUserId(Guid userId);
        Task<IEnumerable<EmpresaDTO>> GetEmpresasAsync();
        Task<EmpresaDTO?> GetEmpresaByUsuarioIdAsync(Guid usuarioId);
        Task<EmpresaDTO?> GetEmpresaByIdAsync(Guid id);
        Task<EmpresaDTO?> GetEmpresaByCNPJAsync(string cnpj);
        Task CreateEmpresaAsync(EmpresaDTO empresa);
        Task UpdateEmpresaAsync(EmpresaDTO empresa);
        Task DeleteEmpresaAsync(Guid id);
    }
}
