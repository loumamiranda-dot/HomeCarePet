using HomeCare.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeCare.Repositorio.Configuracoes;

public class ServicoConfiguracoes : IEntityTypeConfiguration<Servico>
{
    public void Configure(EntityTypeBuilder<Servico> builder)
    {
        builder.ToTable("Servico");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("ServicoId");
        builder.Property(s => s.Nome).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Descricao).HasMaxLength(500);
        builder.Property(s => s.Preco).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(s => s.DuracaoEmMinutos).IsRequired();
        builder.Property(s => s.Ativo).IsRequired();
    }
}
