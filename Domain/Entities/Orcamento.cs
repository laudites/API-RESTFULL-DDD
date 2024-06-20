using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Domain.Entities
{ 
    public class OrcamentoRelatorioDto
    {
        public string NumeroOrcamento { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? UltimaAtualizacao { get; set; }
        public string? RazaoSocial { get; set; }
        public string? NomeFantasia { get; set; }
        public string? Cidade { get; set; }
        public string? UF { get; set; }
        public string? Vendedor { get; set; }
        public string? Gerente { get; set; }
        public decimal? ValorOfertado { get; set; }
        public decimal? ValorLista { get; set; }
        public string? DescontoOver { get; set; }
        public decimal? Servicos { get; set; }
        public string? Status { get; set; }
        public string? StatusGerencial { get; set; }
        public decimal? ValorFechado { get; set; }
        public string? Observacao { get; set; }
        public string? SMR { get; set; }
        public string? PreviUSUARIOoVenda { get; set; }
    }

    public class CountResultInt
    {
        public int Total { get; set; }
    }

    public class CountResult
    {
        public decimal Total { get; set; }
    }

    public class ListaGerente
    {
        public string? Gerente { get; set; }
    }

    public class ListaVendedor
    {
        public string? Vendedor { get; set; }
    }

    public class ListaStatus
    {
        public string? Status { get; set; }
    }

    public class ListaUf
    {
        public string? Uf { get; set; }
    }

    public class ListaAnoCadastro
    {
        public int? Ano { get; set; }
    }

    public class ListaMesCadastro
    {
        public int? Mes { get; set; }
    }

}
