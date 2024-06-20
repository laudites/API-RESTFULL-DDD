using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class HubDbContext : DbContext
    {
        public HubDbContext(DbContextOptions<HubDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Usuario).HasColumnName("Usuario").HasMaxLength(50); // Ajuste o tamanho conforme necessário
                entity.Property(e => e.Senha).HasColumnName("Senha").HasMaxLength(255); // Ajuste o tamanho conforme necessário
                entity.Property(e => e.RefreshToken).HasColumnName("RefreshToken").HasMaxLength(255); // Ajuste o tamanho conforme necessário
                entity.Property(e => e.RefreshTokenExpiryTime).HasColumnName("RefreshTokenExpiryTime");
            });
        }
    }
}
