using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data
{
    public class Eletrofast_2019DbContext : DbContext
    {
        public Eletrofast_2019DbContext(DbContextOptions<Eletrofast_2019DbContext> options) : base(options)
        {
        }

        public DbSet<OrcamentoRelatorioDto> OrcamentoRelatorio { get; set; }
        public DbSet<CountResult> CountResults { get; set; }
        public DbSet<CountResultInt> CountResultsInt { get; set; }

        public DbSet<ListaGerente> ListaGerente { get; set; }
        public DbSet<ListaVendedor> ListaVendedor { get; set; }
        public DbSet<ListaStatus> ListaStatus { get; set; }
        public DbSet<ListaUf> ListaUf { get; set; }
        public DbSet<ListaAnoCadastro> ListaAno { get; set; }
        public DbSet<ListaMesCadastro> ListaMes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração para OrcamentoRelatorioDto como entidade sem chave
            modelBuilder.Entity<OrcamentoRelatorioDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<CountResult>().HasNoKey();
            modelBuilder.Entity<CountResultInt>().HasNoKey();
            modelBuilder.Entity<ListaGerente>().HasNoKey();
            modelBuilder.Entity<ListaVendedor>().HasNoKey();
            modelBuilder.Entity<ListaStatus>().HasNoKey();
            modelBuilder.Entity<ListaUf>().HasNoKey();
            modelBuilder.Entity<ListaAnoCadastro>().HasNoKey();
            modelBuilder.Entity<ListaMesCadastro>().HasNoKey();
        }
    }
}
