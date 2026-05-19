using GestorMEI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.BLL.Services.Interfaces
{
    public interface IEmpresaService
    {
        Task<EmpresaDTO?> GetEmpresaByUserIdAsync(Guid usuarioId);
        Task<Guid> GetEmpresaIdByUserIdAsync(Guid userId);
        Task<EmpresaDTO?> GetEmpresaByIdAsync(Guid id);
        Task<EmpresaDTO?> GetEmpresaByCNPJAsync(string cnpj);
        Task CreateEmpresaAsync(EmpresaDTO empresa);
        Task UpdateEmpresaAsync(EmpresaDTO empresa);
        Task DeleteEmpresaAsync(Guid id);
    }
}
