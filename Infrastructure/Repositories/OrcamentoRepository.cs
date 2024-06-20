using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class OrcamentoRepository : IOrcamentoRepository
    {
        public async Task<IEnumerable<OrcamentoRelatorioDto>> GetRelatorioAsync(DbContext dbContext, int? pagina, string orcamento, string gerente, string vendedor, string status, string uf, string mes, string ano)
        {
            int itensPorPagina = 20;
            int offset = (pagina.HasValue && pagina.Value > 0) ? (pagina.Value - 1) * itensPorPagina : 0;

            var sql = new StringBuilder();
            sql.Append(@"
                        SELECT 
                            [NumeroOrcamento],
                            [DataCadastro],
                            [UltimaAtualizacao],
                            [RazaoSocial],
                            [NomeFantasia],
                            [Cidade],
                            [UF],
                            [Vendedor],
                            [Gerente],
                            [ValorOfertado],
                            [ValorLista],
                            [DescontoOver],
                            [Servicos],
                            [Status],
                            [StatusGerencial],
                            [ValorFechado],
                            [Observacao],
                            [SMR],
                            [PreviUSUARIOoVenda]
                        FROM [WEBSO].[dbo].[OrcamentoRelatorioView]
                        WHERE 1=1");

            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(orcamento))
            {
                sql.Append(" AND [NumeroOrcamento] = @orcamento");
                parameters.Add(new SqlParameter("@orcamento", orcamento));
            }

            if (!string.IsNullOrEmpty(gerente))
            {
                sql.Append(" AND [Gerente] = @gerente");
                parameters.Add(new SqlParameter("@gerente", gerente));
            }

            if (!string.IsNullOrEmpty(vendedor))
            {
                sql.Append(" AND [Vendedor] = @vendedor");
                parameters.Add(new SqlParameter("@vendedor", vendedor));
            }

            if (!string.IsNullOrEmpty(status))
            {
                sql.Append(" AND [Status] = @status");
                parameters.Add(new SqlParameter("@status", status));
            }

            if (!string.IsNullOrEmpty(uf))
            {
                sql.Append(" AND [UF] = @uf");
                parameters.Add(new SqlParameter("@uf", uf));
            }

            if (!string.IsNullOrEmpty(mes))
            {
                sql.Append(" AND MONTH([DataCadastro]) = @mes");
                parameters.Add(new SqlParameter("@mes", mes));
            }

            if (!string.IsNullOrEmpty(ano))
            {
                sql.Append(" AND YEAR([DataCadastro]) = @ano");
                parameters.Add(new SqlParameter("@ano", ano));
            }

            sql.Append(@"
                        ORDER BY [DataCadastro] DESC
                        OFFSET @offset ROWS
                        FETCH NEXT @fetch ROWS ONLY");

            parameters.Add(new SqlParameter("@offset", offset));
            parameters.Add(new SqlParameter("@fetch", itensPorPagina));

            return await dbContext.Set<OrcamentoRelatorioDto>().FromSqlRaw(sql.ToString(), parameters.ToArray()).ToListAsync();
        }



        public async Task<int> GetTotalRowUSUARIOsync(DbContext dbContext, int? year, int? month, int? day, int? status)
        {
            var sql = new StringBuilder();
            sql.Append("SELECT COUNT(*) AS Total FROM OrcamentoGrupoValorView  WHERE 1=1");

            var parameters = new List<SqlParameter>();

            if (year.HasValue)
            {
                sql.Append(" AND YEAR(DataCadastro) = @year");
                parameters.Add(new SqlParameter("@year", year.Value));
            }

            if (month.HasValue)
            {
                sql.Append(" AND MONTH(DataCadastro) = @month");
                parameters.Add(new SqlParameter("@month", month.Value));
            }

            if (day.HasValue)
            {
                sql.Append(" AND DAY(DataCadastro) = @day");
                parameters.Add(new SqlParameter("@day", day.Value));
            }

            if (status.HasValue)
            {
                sql.Append(" AND orccab_Status = @status");
                parameters.Add(new SqlParameter("@status", status));
            }

            var result = await dbContext.Set<CountResultInt>().FromSqlRaw(sql.ToString(), parameters.ToArray()).FirstOrDefaultAsync();
            return (int)(result?.Total ?? 0);
        }

        public async Task<decimal> GetTotalValueAsync(DbContext dbContext, int? year, int? month, int? day, int? status, int? grpStatus)
        {
            var sql = new StringBuilder();
            sql.Append("SELECT ISNULL(SUM(OrcGrpSit_ValorLista), 0) + ISNULL(SUM(OrcGrpSit_ValorServicos), 0) AS Total FROM [WEBSO].[dbo].[OrcamentoGrupoValorView] WHERE base = 'Eletrofrio'");

            var parameters = new List<SqlParameter>();

            if (year.HasValue)
            {
                sql.Append(" AND YEAR(DataCadastro) = @year");
                parameters.Add(new SqlParameter("@year", year.Value));
            }

            if (month.HasValue)
            {
                sql.Append(" AND MONTH(DataCadastro) = @month");
                parameters.Add(new SqlParameter("@month", month.Value));
            }

            if (day.HasValue)
            {
                sql.Append(" AND DAY(DataCadastro) = @day");
                parameters.Add(new SqlParameter("@day", day.Value));
            }

            if (status.HasValue)
            {
                sql.Append(" AND orccab_Status = @status");
                parameters.Add(new SqlParameter("@status", status));
            }

            if (grpStatus.HasValue)
            {
                sql.Append(" AND orcgrpsit_status = @grpstatus");
                parameters.Add(new SqlParameter("@grpstatus", grpStatus));
            }
            var result = await dbContext.Set<CountResult>().FromSqlRaw(sql.ToString(), parameters.ToArray()).FirstOrDefaultAsync();
            return result?.Total ?? 0;
        }

        public async Task<List<ListaGerente>> GetListaGerenteAsync(DbContext dbContext, string vendedor)
        {
            var sql = new StringBuilder();
            sql.Append("SELECT DISTINCT filho AS Nome FROM [pesrelacionamentos] WHERE tipo = 'orcamentista/vendedor'");

            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(vendedor))
            {
                sql.Append(" AND pai LIKE @vendedor");
                parameters.Add(new SqlParameter("@vendedor", vendedor));
            }

            var result = await dbContext.Set<ListaGerente>()
                                        .FromSqlRaw(sql.ToString(), parameters.ToArray())
                                        .ToListAsync();

            return result;
        }

        public async Task<List<ListaVendedor>> GetListaVendedorAsync(DbContext dbContext, string gerente)
        {
            var sql = new StringBuilder();
            sql.Append("SELECT DISTINCT pai FROM [pesrelacionamentos] WHERE tipo = 'orcamentista/vendedor'");

            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(gerente))
            {
                sql.Append(" AND filho LIKE @gerente");
                parameters.Add(new SqlParameter("@gerente", gerente));
            }

            var result = await dbContext.Set<ListaVendedor>()
                                       .FromSqlRaw(sql.ToString(), parameters.ToArray())
                                       .ToListAsync();

            return result;
        }

        public async Task<List<ListaStatus>> GetListaStatuUSUARIOsync(DbContext dbContext, string ano, string mes, string gerente, string uf, string vendedor, string numeroOrcamento)
        {
            var sql = new StringBuilder();
            sql.Append(@"SELECT DISTINCT MAX(Status) AS Status
                         FROM (
                             SELECT CONCAT(orclst_numero, orclst_reviUSUARIOo) AS [Numero Orcamento],
                                    Orcst.st_descricao AS Status
                             FROM ORCCAB
                             INNER JOIN OrcGrpSit ON orccab.numeroOrcamento = OrcGrpSit.numeroOrcamento 
                                                  AND OrcGrpSit_Status = 0 
                                                  AND (idGrupo NOT LIKE 22 AND idGrupo NOT LIKE 30)
                             INNER JOIN prdgrp ON idGrupo = Grupo 
                                              AND base = 'Eletrofrio' 
                                              AND prdgrp.Subgrupo = 00
                             INNER JOIN Orclst ON orclst.orclst_numero + orclst.orclst_reviUSUARIOo = orccab.numeroOrcamento
                             INNER JOIN Orcst ON ORCCAB.orccab_Status = Orcst.st_codigo								
                             WHERE 1 = 1");

            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(ano) && ano != "Todos")
            {
                sql.Append(" AND YEAR(ORCCAB.orccab_Cadastro) = @Ano");
                parameters.Add(new SqlParameter("@Ano", ano));
            }

            if (!string.IsNullOrEmpty(mes) && mes != "Todos")
            {
                sql.Append(" AND MONTH(ORCCAB.orccab_Cadastro) = @Mes");
                parameters.Add(new SqlParameter("@Mes", mes));
            }

            if (!string.IsNullOrEmpty(gerente) && gerente != "Todos")
            {
                sql.Append(" AND (('Eletrofrio' = 'Eletrofrio' AND ORCCAB.orccab_vendedor LIKE @Gerente) OR ('Fast' = 'Fast' AND orccab.orccab_gerente LIKE @Gerente))");
                parameters.Add(new SqlParameter("@Gerente", gerente));
            }

            if (!string.IsNullOrEmpty(uf) && uf != "Todos")
            {
                sql.Append(" AND ORCCAB.orccab_UF LIKE @UF");
                parameters.Add(new SqlParameter("@UF", uf));
            }

            if (!string.IsNullOrEmpty(vendedor) && vendedor != "Todos")
            {
                sql.Append(" AND ORCCAB.orccab_orcamentista LIKE @Vendedor");
                parameters.Add(new SqlParameter("@Vendedor", vendedor));
            }

            if (!string.IsNullOrEmpty(numeroOrcamento))
            {
                sql.Append(" AND ORCCAB.numeroOrcamento LIKE '%' + @NumeroOrcamento + '%'");
                parameters.Add(new SqlParameter("@NumeroOrcamento", numeroOrcamento));
            }

            sql.Append(@"
                         ) AS subquery
                         LEFT JOIN
                         (
                             SELECT [NumeroOrcamento], [StatusGerencial], [ValorFechado], [Observacao]
                             FROM [HUB].[dbo].[RelatorioGerencial]
                         ) AS consulta2
                         ON subquery.[Numero Orcamento] = consulta2.[NumeroOrcamento]
                         GROUP BY [Numero Orcamento];");

            parameters.Add(new SqlParameter("@Status", "%"));

            var result = await dbContext.Set<ListaStatus>()
                                       .FromSqlRaw(sql.ToString(), parameters.ToArray())
                                       .ToListAsync();

            return result;
        }

        public async Task<List<ListaUf>> GetListaUfAsync(DbContext dbContext, string ano, string mes, string gerente, string vendedor, string status, string numeroOrcamento)
        {
            var sql = new StringBuilder();
            sql.Append(@"SELECT DISTINCT ORCCAB.orccab_UF AS UF
                         FROM ORCCAB
                         INNER JOIN Orcst ON ORCCAB.orccab_Status = Orcst.st_codigo
                         WHERE YEAR(ORCCAB.orccab_Cadastro) LIKE @Ano 
                           AND MONTH(ORCCAB.orccab_Cadastro) LIKE @Mes 
                           AND ORCCAB.orccab_vendedor LIKE @Gerente 
                           AND ORCCAB.orccab_orcamentista LIKE @Vendedor 
                           AND ORCCAB.numeroOrcamento LIKE @NumeroOrcamento 
                           AND ORCCAB.orccab_Status LIKE @Status");

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Ano", string.IsNullOrEmpty(ano) || ano == "Todos" ? "%" : ano),
                new SqlParameter("@Mes", string.IsNullOrEmpty(mes) || mes == "Todos" ? "%" : mes),
                new SqlParameter("@Gerente", string.IsNullOrEmpty(gerente) || gerente == "Todos" ? "%" : gerente),
                new SqlParameter("@Vendedor", string.IsNullOrEmpty(vendedor) || vendedor == "Todos" ? "%" : vendedor),
                new SqlParameter("@NumeroOrcamento", string.IsNullOrEmpty(numeroOrcamento) || numeroOrcamento == "Todos" ? "%" : numeroOrcamento),
                new SqlParameter("@Status", string.IsNullOrEmpty(status) || status == "Todos" ? "%" : status),
            };

            var result = await dbContext.Set<ListaUf>()
                                       .FromSqlRaw(sql.ToString(), parameters.ToArray())
                                       .ToListAsync();

            return result;
        }

        public async Task<List<ListaAnoCadastro>> GetListaAnoAsync(DbContext dbContext, string? mes, string? gerente, string uf, string? vendedor, string? status, string? numeroOrcamento)
        {
            var sql = new StringBuilder();
            sql.Append(@"SELECT DISTINCT
                        YEAR(ORCCAB.orccab_Cadastro) as [Ano Cadastro]
                            FROM 
                                ORCCAB
                            INNER JOIN 
                                Orcst ON ORCCAB.orccab_Status = Orcst.st_codigo
                            WHERE 
                                  MONTH(ORCCAB.orccab_Cadastro) LIKE @Mes AND 
                                  ORCCAB.orccab_vendedor LIKE @Gerente AND 
                                  ORCCAB.orccab_UF LIKE @UF AND 
                                  ORCCAB.orccab_orcamentista LIKE @Vendedor AND 
                                  numeroOrcamento LIKE @NumeroOrcamento AND
                                  ORCCAB.orccab_Status like @Status");

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Mes", string.IsNullOrEmpty(mes) || mes == "Todos" ? "%" : mes),
                new SqlParameter("@Gerente", string.IsNullOrEmpty(gerente) || gerente == "Todos" ? "%" : gerente),
                new SqlParameter("@UF", string.IsNullOrEmpty(uf) || uf == "Todos" ? "%" : uf),
                new SqlParameter("@Vendedor", string.IsNullOrEmpty(vendedor) || vendedor == "Todos" ? "%" : vendedor),
                new SqlParameter("@NumeroOrcamento", string.IsNullOrEmpty(numeroOrcamento) || numeroOrcamento == "Todos" ? "%" : numeroOrcamento),
                new SqlParameter("@Status", string.IsNullOrEmpty(status) || status == "Todos" ? "%" : status),
            };

            var result = await dbContext.Set<ListaAnoCadastro>()
                                       .FromSqlRaw(sql.ToString(), parameters.ToArray())
                                       .ToListAsync();

            return result;
        }

        public async Task<List<ListaMesCadastro>> GetListaMeUSUARIOsync(DbContext dbContext, string? ano, string? gerente, string uf, string? vendedor, string? status, string? numeroOrcamento)
        {
            var sql = new StringBuilder();
            sql.Append(@"SELECT DISTINCT
                        MONTH(ORCCAB.orccab_Cadastro) as [Mes Cadastro]
                            FROM 
                                ORCCAB
                            INNER JOIN 
                                Orcst ON ORCCAB.orccab_Status = Orcst.st_codigo
                            WHERE 
                                  YEAR(ORCCAB.orccab_Cadastro) LIKE @Ano AND 
                                  ORCCAB.orccab_vendedor LIKE @Gerente AND 
                                  ORCCAB.orccab_UF LIKE @UF AND 
                                  ORCCAB.orccab_orcamentista LIKE @Vendedor AND 
                                  numeroOrcamento LIKE @NumeroOrcamento AND
                                  ORCCAB.orccab_Status like @Status");

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Ano", string.IsNullOrEmpty(ano) || ano == "Todos" ? "%" : ano),
                new SqlParameter("@Gerente", string.IsNullOrEmpty(gerente) || gerente == "Todos" ? "%" : gerente),
                new SqlParameter("@UF", string.IsNullOrEmpty(uf) || uf == "Todos" ? "%" : uf),
                new SqlParameter("@Vendedor", string.IsNullOrEmpty(vendedor) || vendedor == "Todos" ? "%" : vendedor),
                new SqlParameter("@NumeroOrcamento", string.IsNullOrEmpty(numeroOrcamento) || numeroOrcamento == "Todos" ? "%" : numeroOrcamento),
                new SqlParameter("@Status", string.IsNullOrEmpty(status) || status == "Todos" ? "%" : status),
            };

            var result = await dbContext.Set<ListaMesCadastro>()
                                       .FromSqlRaw(sql.ToString(), parameters.ToArray())
                                       .ToListAsync();

            return result;
        }

    }
}
