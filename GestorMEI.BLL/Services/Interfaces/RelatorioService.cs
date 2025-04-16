using GestorMEI.BLL.Repositories.Interfaces;
using GestorMEI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorMEI.BLL.Services.Interfaces
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IVendaRepository _vendaRepository;
        private readonly ITabelaGeralItemRepository _tabelaGeralItemRepository;
        private readonly ITabelaGeralRepository _tabelaGeralRepository;

        public RelatorioService(IVendaRepository vendaRepository, ITabelaGeralItemRepository tabelaGeralItemRepository, ITabelaGeralRepository tabelaGeralRepository)
        {
            _vendaRepository = vendaRepository;
            _tabelaGeralItemRepository = tabelaGeralItemRepository;
            _tabelaGeralRepository = tabelaGeralRepository;
        }

        public async Task<List<RelatorioDTO>> GerarRelatorioVendas(Guid empresaId, DateOnly? dataInicio, DateOnly? dataFim)
        {
            var vendas = await _vendaRepository.GetVendasAsync(empresaId);
            if (dataInicio.HasValue)
                vendas = vendas.Where(v => v.DataVenda >= dataInicio.Value).ToList();

            if (dataFim.HasValue)
                vendas = vendas.Where(v => v.DataVenda <= dataFim.Value).ToList();

            var tgTipoVendaServico = await _tabelaGeralRepository.GetByNomeAsync("TipoVenda");

            var tipoVendaServico = await _tabelaGeralItemRepository.GetBySiglaAsync(tgTipoVendaServico.Id.Value, "SRVC");
            var tipoVendaComercio = await _tabelaGeralItemRepository.GetBySiglaAsync(tgTipoVendaServico.Id.Value, "CMRC");
            var tipoVendaIndustria = await _tabelaGeralItemRepository.GetBySiglaAsync(tgTipoVendaServico.Id.Value, "INDTR");

            var vendasServico = vendas.Where(v => v.IdTGTipoVenda == tipoVendaServico.Id).ToList();
            var vendasComercio = vendas.Where(v => v.IdTGTipoVenda == tipoVendaComercio.Id).ToList();
            var vendasIndustria = vendas.Where(v => v.IdTGTipoVenda == tipoVendaIndustria.Id).ToList();

            var retorno = new List<RelatorioDTO>();
            if (vendas == null || vendas.Any() == false)
                return retorno;

            //Vendas Serviço
            var dtoServico = new RelatorioDTO();
            dtoServico.TotalVendas = vendasServico.Sum(x=>x.ValorVenda);
            dtoServico.TotalVendasComNF = vendasServico.Where(x => x.ComNF).Sum(x => x.ValorVenda);
            dtoServico.TotalVendasSemNF = vendasServico.Where(x => !x.ComNF).Sum(x => x.ValorVenda);
            dtoServico.IdTGTipoVenda = tipoVendaServico.Id.Value;
            dtoServico.TipoVendaSigla = tipoVendaServico.Sigla;
            dtoServico.TipoVendaDescricao= tipoVendaServico.Descricao;

            retorno.Add(dtoServico);

            //Vendas Comércio
            var dtoComercio = new RelatorioDTO();
            dtoComercio.TotalVendas = vendasComercio.Sum(x => x.ValorVenda);
            dtoComercio.TotalVendasComNF = vendasComercio.Where(x => x.ComNF).Sum(x => x.ValorVenda);
            dtoComercio.TotalVendasSemNF = vendasComercio.Where(x => !x.ComNF).Sum(x => x.ValorVenda);
            dtoComercio.IdTGTipoVenda = tipoVendaComercio.Id.Value;
            dtoComercio.TipoVendaSigla = tipoVendaComercio.Sigla;
            dtoComercio.TipoVendaDescricao = tipoVendaComercio.Descricao;

            retorno.Add(dtoComercio);

            //Vendas Indústria
            var dtoIndustria = new RelatorioDTO();
            dtoIndustria.TotalVendas = vendasIndustria.Sum(x => x.ValorVenda);
            dtoIndustria.TotalVendasComNF = vendasIndustria.Where(x => x.ComNF).Sum(x => x.ValorVenda);
            dtoIndustria.TotalVendasSemNF = vendasIndustria.Where(x => !x.ComNF).Sum(x => x.ValorVenda);
            dtoIndustria.IdTGTipoVenda = tipoVendaIndustria.Id.Value;
            dtoIndustria.TipoVendaSigla = tipoVendaIndustria.Sigla;
            dtoIndustria.TipoVendaDescricao = tipoVendaIndustria.Descricao;

            retorno.Add(dtoIndustria);

            return retorno;
        }
    }
}
