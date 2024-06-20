using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IOrcamentoRepository
    {
        Task<IEnumerable<OrcamentoRelatorioDto>> GetRelatorioAsync(DbContext dbContext, int? pagina, string orcamento, string gerente, string vendedor, string status, string uf, string mes, string ano);
        Task<int> GetTotalRowUSUARIOsync(DbContext dbContext, int? year, int? month, int? day, int? status);
        Task<decimal> GetTotalValueAsync(DbContext dbContext, int? year, int? month, int? day, int? status, int? grpStatus);
        Task<List<ListaGerente>> GetListaGerenteAsync(DbContext dbContext, string? vendedor); // Altere aqui
        Task<List<ListaVendedor>> GetListaVendedorAsync(DbContext dbContext, string? gerente);
        Task<List<ListaStatus>> GetListaStatuUSUARIOsync(DbContext dbContext, string? ano, string? mes, string? gerente, string? uf, string? vendedor, string? numeroOrcamento);
        Task<List<ListaUf>> GetListaUfAsync(DbContext dbContext, string? ano, string? mes, string? gerente, string? vendedor, string? status, string? numeroOrcamento);
        Task<List<ListaAnoCadastro>> GetListaAnoAsync(DbContext dbContext, string? mes, string? gerente,string uf, string? vendedor, string? status, string? numeroOrcamento);
        Task<List<ListaMesCadastro>> GetListaMeUSUARIOsync(DbContext dbContext, string? ano, string? gerente, string uf, string? vendedor, string? status, string? numeroOrcamento);
    }
}
