using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OrcamentoService
    {
        private readonly IOrcamentoRepository _orcamentoRepository;
        private readonly Eletrofast_2019DbContext _eletrofastDbContext;
        private readonly ILogger<OrcamentoService> _logger;

        public OrcamentoService(IOrcamentoRepository orcamentoRepository, Eletrofast_2019DbContext eletrofastDbContext, ILogger<OrcamentoService> logger)
        {
            _orcamentoRepository = orcamentoRepository;
            _eletrofastDbContext = eletrofastDbContext;
            _logger = logger;
        }


        public async Task<IEnumerable<OrcamentoRelatorioDto>> GetRelatorioAsync(int? pagina, string orcamento, string gerente, string vendedor, string status, string uf, string mes, string ano)
        {
            try
            {
                _logger.LogInformation("Using database: Eletrofast_2019DbContext");
                return await _orcamentoRepository.GetRelatorioAsync(_eletrofastDbContext, pagina, orcamento, gerente, vendedor, status, uf, mes, ano);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the report.");
                throw new ApplicationException("An error occurred while retrieving the report.", ex);
            }
        }


        public async Task<int> GetTotalRowUSUARIOsync(int? year, int? month, int? day, int? status)
        {
            return await _orcamentoRepository.GetTotalRowUSUARIOsync(_eletrofastDbContext, year, month, day, status);
        }

        public async Task<decimal> GetTotalValueAsync(int? year, int? month, int? day, int? status, int? grpUSUARIOtus)
        {
            return await _orcamentoRepository.GetTotalValueAsync(_eletrofastDbContext, year, month, day, status, grpUSUARIOtus);
        }
        public async Task<List<string>> GetListaGerenteAsync(string? vendedor)
        {
            var listaGerenteDto = await _orcamentoRepository.GetListaGerenteAsync(_eletrofastDbContext, vendedor);
            return listaGerenteDto.Select(g => g.Gerente).ToList();
        }


        public async Task<List<string>> GetListaVendedorAsync(string? gerente)
        {
            var listaVendedor = await _orcamentoRepository.GetListaVendedorAsync(_eletrofastDbContext, gerente);
            return listaVendedor.Select(v => v.Vendedor).ToList();
        }

        public async Task<List<string>> GetListaStatuUSUARIOsync(string? ano, string? mes, string? gerente, string? uf, string? vendedor, string? numeroOrcamento)
        {
            var listaStatus = await _orcamentoRepository.GetListaStatuUSUARIOsync(_eletrofastDbContext, ano, mes, gerente, uf, vendedor, numeroOrcamento);
            return listaStatus.Select(ls => ls.Status).ToList();
        }

        public async Task<List<string>> GetListaUfAsync(string? ano, string? mes, string? gerente, string? vendedor, string? status, string? numeroOrcamento)
        {
            var listaUf = await _orcamentoRepository.GetListaUfAsync(_eletrofastDbContext, ano, mes, gerente, vendedor, status, numeroOrcamento);
            return listaUf.Select(l => l.Uf).ToList();
        }

        public async Task<List<int>> GetListaAnoAsync(string? mes, string? gerente, string uf, string? vendedor, string? status, string? numeroOrcamento)
        {
            var listaAno = await _orcamentoRepository.GetListaAnoAsync(_eletrofastDbContext, mes, gerente, uf, vendedor, status, numeroOrcamento);
            return listaAno.Select(l => l.Ano.GetValueOrDefault()).ToList(); // Adicionando ToList() e GetValueOrDefault() para retornar uma lista de inteiros
        }

        public async Task<List<int>> GetListaMeUSUARIOsync(string? ano, string? gerente, string uf, string? vendedor, string? status, string? numeroOrcamento)
        {
            var listaMes = await _orcamentoRepository.GetListaMeUSUARIOsync(_eletrofastDbContext, ano, gerente, uf, vendedor, status, numeroOrcamento);
            return listaMes.Select(l => l.Mes.GetValueOrDefault()).ToList(); // Adicionando ToList() e GetValueOrDefault() para retornar uma lista de inteiros
        }
    }
}
