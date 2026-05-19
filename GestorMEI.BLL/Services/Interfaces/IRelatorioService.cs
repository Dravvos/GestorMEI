using GestorMEI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.BLL.Services.Interfaces
{
    public interface IRelatorioService
    {
        Task<IList<RelatorioDTO>?> GerarRelatorioVendas(Guid empresaId, DateOnly? dataInicio, DateOnly? dataFim);
    }
}
