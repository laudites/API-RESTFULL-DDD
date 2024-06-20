using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrcamentoController : ControllerBase
    {
        private readonly OrcamentoService _orcamentoService;

        public OrcamentoController(OrcamentoService orcamentoService)
        {
            _orcamentoService = orcamentoService;
        }

        [HttpGet("GetRelatorio")]
        public async Task<ActionResult<IEnumerable<OrcamentoRelatorioDto>>> GetRelatorio(
            [FromQuery] int? pagina,
            [FromQuery] string? orcamento,
            [FromQuery] string? gerente,
            [FromQuery] string? vendedor,
            [FromQuery] string? status,
            [FromQuery] string? uf,
            [FromQuery] string? mes,
            [FromQuery] string? ano)
        {
            if (pagina.HasValue && pagina.Value < 1)
            {
                return BadRequest("O número da página deve ser maior que zero.");
            }

            var relatorio = await _orcamentoService.GetRelatorioAsync(pagina, orcamento, gerente, vendedor, status, uf, mes, ano);
            return Ok(relatorio);
        }

        [HttpGet("totalRows")]
        public async Task<ActionResult<int>> GetTotalRows(
            [FromQuery] int? year,
            [FromQuery] int? month,
            [FromQuery] int? day,
            [FromQuery] int? status)
        {
            var totalRows = await _orcamentoService.GetTotalRowUSUARIOsync(year, month, day, status);
            return Ok(totalRows);
        }

        [HttpGet("totalValue")]
        public async Task<ActionResult<decimal>> GetTotalValue(
            [FromQuery] int? year,
            [FromQuery] int? month,
            [FromQuery] int? day,
            [FromQuery] int? status,
            [FromQuery] int? grpStatus)
        {
            var totalValue = await _orcamentoService.GetTotalValueAsync(year, month, day, status, grpStatus);
            return Ok(totalValue);
        }

        [HttpGet("listaGerente")]
        public async Task<ActionResult<List<string>>> GetListaGerente([FromQuery] string? vendedor)
        {
            try
            {
                var gerenteList = await _orcamentoService.GetListaGerenteAsync(vendedor);
                return Ok(gerenteList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.MesUSUARIOge}");
            }
        }

        [HttpGet("listaVendedor")]
        public async Task<ActionResult<List<string>>> GetListaVendedor([FromQuery] string? gerente)
        {
            try
            {
                var vendedorList = await _orcamentoService.GetListaVendedorAsync(gerente);
                return Ok(vendedorList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.MesUSUARIOge}");
            }
        }

        [HttpGet("listaStatus")]
        public async Task<ActionResult<List<string>>> GetListaStatus(
            [FromQuery] string? ano,
            [FromQuery] string? mes,
            [FromQuery] string? gerente,
            [FromQuery] string? uf,
            [FromQuery] string? vendedor,
            [FromQuery] string? numeroOrcamento)
        {
            try
            {
                var statusList = await _orcamentoService.GetListaStatuUSUARIOsync(ano, mes, gerente, uf, vendedor, numeroOrcamento);
                return Ok(statusList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.MesUSUARIOge}");
            }
        }

        [HttpGet("listaUf")]
        public async Task<ActionResult<List<string>>> GetListaUf(
            [FromQuery] string? ano,
            [FromQuery] string? mes,
            [FromQuery] string? gerente,
            [FromQuery] string? vendedor,
            [FromQuery] string? status,
            [FromQuery] string? numeroOrcamento)
        {
            try
            {
                var ufList = await _orcamentoService.GetListaUfAsync(ano, mes, gerente, vendedor, status, numeroOrcamento);
                return Ok(ufList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.MesUSUARIOge}");
            }
        }

        [HttpGet("listaAno")]
        public async Task<ActionResult<List<int>>> GetListaAno(
    [FromQuery] string? mes,
    [FromQuery] string? gerente,
    [FromQuery] string? uf,
    [FromQuery] string? vendedor,
    [FromQuery] string? status,
    [FromQuery] string? numeroOrcamento)
        {
            try
            {
                var anoLista = await _orcamentoService.GetListaAnoAsync(mes, gerente, uf, vendedor, status, numeroOrcamento);
                return Ok(anoLista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.MesUSUARIOge}");
            }
        }

        [HttpGet("listaMes")]
        public async Task<ActionResult<List<int>>> GetListaMes(
         [FromQuery] string? ano,
         [FromQuery] string? gerente,
         [FromQuery] string? uf,
         [FromQuery] string? vendedor,
         [FromQuery] string? status,
         [FromQuery] string? numeroOrcamento)
        {
            try
            {
                var mesLista = await _orcamentoService.GetListaMeUSUARIOsync(ano, gerente, uf, vendedor, status, numeroOrcamento);
                return Ok(mesLista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.MesUSUARIOge}");
            }
        }

    }
}
